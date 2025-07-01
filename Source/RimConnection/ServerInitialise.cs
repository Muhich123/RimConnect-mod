using System;
using System.Linq;
using Verse;

namespace RimConnection
{
    [StaticConstructorOnStartup]
    public static class ServerInitialise
    {
        static ServerInitialise() { Init(); }

        public static bool Init()
        {
            try
            {
                Log.Message("Initialising Donation Alerts connection");
                var donations = DonationAlertsAPI.GetDonations(RimConnectSettings.donationToken);
                RimConnectSettings.initialiseSuccessful = donations != null;

                if (Settings.CommandOptionListController.commandOptionList == null)
                {
                    var validCommands = ActionList.ActionListToApi().validCommands;
                    CommandOptionList commandOptionList = new CommandOptionList();
                    commandOptionList.commandOptions = validCommands.Select(vc => vc.toCommandOption()).ToList();
                    Settings.CommandOptionListController.commandOptionList = commandOptionList;
                }

                return RimConnectSettings.initialiseSuccessful;
            }
            catch (Exception err)
            {
                Log.Error(err.ToString());
                RimConnectSettings.initialiseSuccessful = false;
                return false;
            }
        }
    }
}
