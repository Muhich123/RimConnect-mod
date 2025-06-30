using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading.Tasks;
using RestSharp;
using Verse;

namespace RimConnection
{
    public class DonationPoller : GameComponent
    {
        public static int lastDonationId = 0;
        private static readonly TimeSpan checkInterval = TimeSpan.FromSeconds(10);
        private static DateTime lastCheckTime = DateTime.UtcNow;
        private static ConcurrentQueue<Command> commandQueue = new ConcurrentQueue<Command>();

        public DonationPoller(Game game)
        {
        }

        public override void GameComponentTick()
        {
            if (RimConnectSettings.initialiseSuccessful)
            {
                if (DateTime.UtcNow - lastCheckTime > checkInterval)
                {
                    lastCheckTime = DateTime.UtcNow;
                    CheckForDonations();
                }
            }

            if (commandQueue.TryDequeue(out Command cmd))
            {
                if (ActionList.actionLookup != null && ActionList.actionLookup.TryGetValue(cmd.actionHash, out IAction action))
                {
                    action.Execute(cmd.amount, cmd.boughtBy);
                    Find.TickManager.slower.SignalForceNormalSpeedShort();
                }
            }
        }

        public static async void CheckForDonations()
        {
            await Task.Run(() =>
            {
                try
                {
                    // Force TLS 1.2 for HTTPS requests
                    System.Net.ServicePointManager.SecurityProtocol = System.Net.SecurityProtocolType.Tls12;
                    var client = new RestClient("https://www.donationalerts.com/api/v1/");
                    var request = new RestRequest("alerts/donations", Method.GET);
                    request.AddHeader("Authorization", $"Bearer {RimConnectSettings.donationToken}");
                    var response = client.Execute<DonationAlertsResponse>(request);
                    if (response.StatusCode == System.Net.HttpStatusCode.OK && response.Data != null)
                    {
                        var donations = response.Data.data;
                        if (donations != null)
                        {
                            List<DonationAlert> newDonations = new List<DonationAlert>();
                            foreach (var donation in donations)
                            {
                                if (donation.id > lastDonationId)
                                    newDonations.Add(donation);
                            }

                            if (newDonations.Count > 0)
                            {
                                newDonations.Sort((a, b) => a.id.CompareTo(b.id));
                                foreach (var donation in newDonations)
                                {
                                    int amountInt = (int)Math.Floor(donation.amount);
                                    var optionList = Settings.CommandOptionListController.commandOptionList;
                                    if (optionList != null)
                                    {
                                        var match = optionList.commandOptions.Find(opt => opt.costSilverStore == amountInt);
                                        if (match != null)
                                        {
                                            Command newCommand = new Command
                                            {
                                                actionHash = match.actionHash,
                                                amount = 1,
                                                boughtBy = donation.username
                                            };
                                            commandQueue.Enqueue(newCommand);
                                            Log.Message($"[RimConnect] Queued event for donation {donation.amount} {donation.currency} from {donation.username}");
                                        }
                                        else
                                        {
                                            Log.Message($"[RimConnect] Donation of {donation.amount}{donation.currency} from {donation.username} does not match any configured event.");
                                        }
                                    }
                                    if (donation.id > lastDonationId)
                                        lastDonationId = donation.id;
                                }
                            }
                        }
                    }
                    else if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                    {
                        Log.Error("[RimConnect] DonationAlerts token unauthorized or expired. Stopping connection.");
                        RimConnectSettings.initialiseSuccessful = false;
                    }
                    else
                    {
                        Log.Error($"[RimConnect] Failed to fetch donations. HTTP {response.StatusCode}: {response.ErrorMessage}");
                    }
                }
                catch (Exception e)
                {
                    Log.Error("[RimConnect] Error in donation check: " + e.Message);
                }
            });
        }
    }
}
