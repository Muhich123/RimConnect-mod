using System.Linq;
using Verse;
using RimConnection.Settings;

namespace RimConnection
{
    [StaticConstructorOnStartup]
    public static class ServerInitialise
    {
        static ServerInitialise()
        {
            Init();
        }

        public static bool Init()
        {
            Log.Message("Initialising DonationAlerts mode");

            // Build a local list of command options so the loyalty store works
            CommandOptionList optionList = new CommandOptionList();
            var validCommands = ActionList.ActionListToApi().validCommands;
            optionList.commandOptions = validCommands.Select(vc => vc.toCommandOption()).ToList();
            Settings.CommandOptionListController.commandOptionList = optionList;

            RimConnectSettings.initialiseSuccessful = true;
            return true;
        }
    }
}
