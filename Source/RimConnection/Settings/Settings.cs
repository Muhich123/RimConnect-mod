using System.Text.RegularExpressions;
using RimConnection.API;
using RimConnection.Settings;
using RimWorld;
using UnityEngine;
using Verse;

namespace RimConnection
{
    public class RimConnectSettings : ModSettings
    {
        public static string[] validCommands;

#if DEBUG
        public static string BASE_URL = "http://localhost:8080/";
#else
        public static string BASE_URL = "http://rimconnect-backend.herokuapp.com/";
#endif

        public static string donationAlertsToken = "";
        // Legacy fields required by some API classes
        public static string secret = "";
        public static string token = "";
        public static int lastDonationId = 0;
        public static bool initialiseSuccessful = false;

        public static int silverAwardPoints = -1;

        private static float defaultWidth = 200f;

        public override void ExposeData()
        {
            base.ExposeData();

            Scribe_Values.Look(ref donationAlertsToken, "donationAlertsToken", "");
            Scribe_Values.Look(ref secret, "secret", "");
            Scribe_Values.Look(ref token, "token", "");
            Scribe_Values.Look(ref silverAwardPoints, "silverAwardPoints");
            Scribe_Values.Look(ref lastDonationId, "lastDonationId");

        }

        public void DoWindowContents(Rect rect)
        {
            Rect tokenHeader = new Rect(0, 32f, rect.width, 64f);
            Widgets.Label(tokenHeader, "<size=32>DonationAlerts</size>");

            Rect tokenGroup = new Rect(0, tokenHeader.y + tokenHeader.height + 10f, rect.width, 48f);
            GUI.BeginGroup(tokenGroup);

            Rect tokenLabel = new Rect(0, 0, defaultWidth, 24f);
            Widgets.Label(tokenLabel, "Token:");
            tokenLabel.x += tokenLabel.width + WidgetRow.LabelGap;
            donationAlertsToken = Widgets.TextField(tokenLabel, donationAlertsToken);

            GUI.EndGroup();

            Rect loyaltyStoreHeader = new Rect(0, tokenGroup.y + tokenGroup.height + 10f, rect.width, 64f);
            Widgets.Label(loyaltyStoreHeader, "<size=32>Loyalty Settings</size>");

            Rect itemStoreGroup = new Rect(0, loyaltyStoreHeader.y + loyaltyStoreHeader.height + 10f, rect.width, 24f);

            if (CommandOptionListController.commandOptionList != null)
            {
                GUI.BeginGroup(itemStoreGroup);

                Rect itemLabel = new Rect(0, 0, defaultWidth, 24f);
                Widgets.Label(itemLabel, "Loyalty Store Items:");

                itemLabel.x += itemLabel.width + WidgetRow.LabelGap;

                if (Widgets.ButtonText(itemLabel, "Items"))
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

            Rect silversPerGroup = new Rect(0, itemStoreGroup.y + itemStoreGroup.height + 10f, rect.width, 24f);
            GUI.BeginGroup(silversPerGroup);

            Rect silverLabel = new Rect(0, 0, defaultWidth, 24f);
            Widgets.Label(silverLabel, "Silver per 2 minutes:");

            silverLabel.x += silverLabel.width + WidgetRow.LabelGap;

            string silverAwardPointsBuffer = silverAwardPoints.ToString();
            Widgets.TextFieldNumeric(silverLabel, ref silverAwardPoints, ref silverAwardPointsBuffer);

            GUI.EndGroup();
        }
    }
}
