using RestSharp;
using RimWorld;
using UnityEngine;
using Verse;
using RimConnection.Settings;

namespace RimConnection
{
    public class RimConnectSettings : ModSettings
    {
        public static string donationToken = "";
        public static bool initialiseSuccessful = false;
        private bool showToken = false;
        private static float defaultWidth = 200f;

        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_Values.Look(ref donationToken, "donationToken", "", true);
        }

        public void DoWindowContents(Rect rect)
        {
            Rect accountLinkHeader = new Rect(0, 32f, rect.width, 64f);
            Widgets.Label(accountLinkHeader, "<size=32>Account Link</size>");

            Rect statusGroup = new Rect(0, accountLinkHeader.y + accountLinkHeader.height + 10f, rect.width, 48f);
            GUI.BeginGroup(statusGroup);

            Rect statusLabel = new Rect(0, 0, defaultWidth, 24f);
            Rect connectionButton = new Rect(defaultWidth + WidgetRow.LabelGap, 24f, defaultWidth, 24f);
            Widgets.Label(statusLabel, "Status:");
            statusLabel.x += statusLabel.width + WidgetRow.LabelGap;
            if (initialiseSuccessful)
            {
                Widgets.Label(statusLabel, "<color=green>Connected!</color>");
                if (Widgets.ButtonText(connectionButton, "Reconnect"))
                {
                    TryConnectDonationAlerts();
                }
            }
            else
            {
                Widgets.Label(statusLabel, "<color=red>Disconnected</color>");
                if (Widgets.ButtonText(connectionButton, "Connect"))
                {
                    TryConnectDonationAlerts();
                }
            }
            GUI.EndGroup();

            Rect tokenGroup = new Rect(0, statusGroup.y + statusGroup.height + 20f, rect.width, 72f);
            GUI.BeginGroup(tokenGroup);

            Rect tokenLabel = new Rect(0, 0, defaultWidth, 24f);
            Rect pasteButton = new Rect(defaultWidth, 24f, defaultWidth, 24f);
            Rect warningLabel = new Rect(0, 48, 400f, 24f);

            Widgets.Label(tokenLabel, "Donation Token:");
            tokenLabel.x = tokenLabel.width + WidgetRow.LabelGap;
            if (showToken)
            {
                donationToken = Widgets.TextField(tokenLabel, donationToken);
            }
            else
            {
                Widgets.Label(tokenLabel, new string('*', donationToken.Length));
            }

            tokenLabel.x += tokenLabel.width + WidgetRow.LabelGap;
            if (!showToken && Widgets.ButtonText(tokenLabel, "Show"))
            {
                showToken = true;
            }
            else if (showToken && Widgets.ButtonText(tokenLabel, "Hide"))
            {
                showToken = false;
            }

            if (Widgets.ButtonText(pasteButton, "Paste from Clipboard"))
            {
                donationToken = GUIUtility.systemCopyBuffer;
            }
            Widgets.Label(warningLabel, "<color=red>Warning: Do not show your token on stream!</color>");
            GUI.EndGroup();

            Rect eventsHeader = new Rect(0, tokenGroup.y + tokenGroup.height + 10f, rect.width, 64f);
            Widgets.Label(eventsHeader, "<size=32>Donation Event Settings</size>");

            Rect itemStoreGroup = new Rect(0, eventsHeader.y + eventsHeader.height + 10f, rect.width, 24f);
            GUI.BeginGroup(itemStoreGroup);
            Rect itemLabel = new Rect(0, 0, defaultWidth, 24f);
            Widgets.Label(itemLabel, "Events:");
            itemLabel.x += itemLabel.width + WidgetRow.LabelGap;
            if (Widgets.ButtonText(itemLabel, "Edit"))
            {
                CommandOptionSettings window = new CommandOptionSettings();
                Find.WindowStack.TryRemove(window.GetType());
                Find.WindowStack.Add(window);
            }
            itemLabel.x += itemLabel.width + WidgetRow.LabelGap;
            if (Widgets.ButtonText(itemLabel, "Reset"))
            {
                ResetCommandOptionsModal window = new ResetCommandOptionsModal();
                Find.WindowStack.TryRemove(window.GetType());
                Find.WindowStack.Add(window);
            }
            GUI.EndGroup();
        }

        private void TryConnectDonationAlerts()
        {
            if (string.IsNullOrEmpty(donationToken))
            {
                Log.Error("DonationAlerts token is empty! Please enter your token.");
                initialiseSuccessful = false;
                return;
            }
            // Ensure TLS 1.2 so HTTPS requests succeed on older .NET installs
            System.Net.ServicePointManager.SecurityProtocol = System.Net.SecurityProtocolType.Tls12;
            var client = new RestClient("https://www.donationalerts.com/api/v1/");
            var request = new RestRequest("alerts/donations", Method.GET);
            request.AddHeader("Authorization", $"Bearer {donationToken}");
            try
            {
                var response = client.Execute<DonationAlertsResponse>(request);
                if (response.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    DonationAlertsResponse data = response.Data;
                    if (data != null && data.data != null)
                    {
                        int maxId = 0;
                        foreach (var donation in data.data)
                        {
                            if (donation.id > maxId)
                                maxId = donation.id;
                        }
                        initialiseSuccessful = true;
                        DonationPoller.lastDonationId = maxId;
                        Log.Message("[RimConnect] Connected to DonationAlerts successfully.");
                    }
                    else
                    {
                        initialiseSuccessful = true;
                        DonationPoller.lastDonationId = 0;
                        Log.Message("[RimConnect] Connected to DonationAlerts (no donations yet).");
                    }
                }
                else
                {
                    initialiseSuccessful = false;
                    string extra = string.IsNullOrEmpty(response.ErrorMessage) ? "" : $" ({response.ErrorMessage})";
                    if (response.ErrorException != null)
                    {
                        extra += $" ({response.ErrorException.GetType().Name}: {response.ErrorException.Message})";
                    }
                    if (response.StatusCode == 0 && string.IsNullOrEmpty(extra))
                    {
                        extra = " (no response)";
                    }
                    Log.Error($"[RimConnect] Failed to connect to DonationAlerts. HTTP {(int)response.StatusCode}{extra}. Check your token or connection.");
                }
            }
            catch (System.Exception e)
            {
                initialiseSuccessful = false;
                Log.Error("[RimConnect] Error connecting to DonationAlerts: " + e.Message);
            }
        }
    }
}
