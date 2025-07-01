using Multiplayer.API;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Verse;

namespace RimConnection
{
    // Maintains the old class name used in saved games
    class ServerPoller : GameComponent
    {
        static DateTime lastGETRequest = DateTime.UtcNow;
        static readonly TimeSpan timeBetweenRequests = TimeSpan.FromSeconds(30d);
        static ConcurrentQueue<Donation> donationQueue = new ConcurrentQueue<Donation>();
        static int lastDonationId = 0;

        public ServerPoller(Game game) { }

        public override void FinalizeInit()
        {
            base.FinalizeInit();
        }

        public override void GameComponentTick()
        {
            if (!string.IsNullOrEmpty(RimConnectSettings.donationToken))
            {
                if (DateTime.UtcNow - lastGETRequest > timeBetweenRequests)
                {
                    lastGETRequest = DateTime.UtcNow;
                    CheckDonations();
                }
            }

            if (donationQueue.TryDequeue(out Donation donation))
            {
                var options = Settings.CommandOptionListController.commandOptionList?.commandOptions;
                if (options != null)
                {
                    var match = options.FirstOrDefault(o => o.costSilverStore == (int)donation.amount);
                    if (match != null)
                    {
                        IAction action = ActionList.actionLookup[match.actionHash];
                        action.Execute(1, donation.username);
                        Find.TickManager.slower.SignalForceNormalSpeedShort();
                    }
                }
            }
        }

        public static async void CheckDonations()
        {
            await Task.Run(() =>
            {
                List<Donation> donations = DonationAlertsAPI.GetDonations(RimConnectSettings.donationToken);
                foreach (var donation in donations.Where(d => d.id > lastDonationId))
                {
                    lastDonationId = Math.Max(lastDonationId, donation.id);
                    EnqueueDonation(donation);
                }
            });
        }

        [SyncMethod]
        public static void EnqueueDonation(Donation donation)
        {
            donationQueue.Enqueue(donation);
        }
    }
}
