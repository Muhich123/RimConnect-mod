using Verse;

namespace RimConnection
{
    /// <summary>
    /// Initializes the command option list after game defs are loaded.
    /// This replaces the old server-based initialization logic.
    /// </summary>
    [StaticConstructorOnStartup]
    public static class DonationInitialise
    {
        static DonationInitialise()
        {
            Init();
        }

        public static void Init()
        {
            // Ensure HTTPS requests use TLS 1.2 which DonationAlerts requires.
            System.Net.ServicePointManager.SecurityProtocol |= System.Net.SecurityProtocolType.Tls12;

            // Build the lookup of all actions and generate a default command option list.
            ActionList.GenerateActionLookup();
            var validCommands = ActionList.ActionListToApi().validCommands;
            Settings.CommandOptionListController.commandOptionList = new CommandOptionList
            {
                commandOptions = validCommands.ConvertAll(vc => vc.toCommandOption())
            };
            Log.Message($"[RimConnect] Initialized {Settings.CommandOptionListController.commandOptionList.commandOptions.Count} events locally.");
        }
    }
}

