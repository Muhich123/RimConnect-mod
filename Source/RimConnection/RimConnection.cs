using UnityEngine;
using Verse;

namespace RimConnection
{
    public class RimConnection : Mod
    {
        RimConnectSettings settings;

        public RimConnection(ModContentPack content) : base(content)
        {
            settings = GetSettings<RimConnectSettings>();
            InitializeEventList();
        }

        public override void DoSettingsWindowContents(Rect inRect)
        {
            settings.DoWindowContents(inRect);
        }

        public override string SettingsCategory()
        {
            return "RimConnect";
        }

        private void InitializeEventList()
        {
            ActionList.GenerateActionLookup();
            var validCommands = ActionList.ActionListToApi().validCommands;
            Settings.CommandOptionListController.commandOptionList = new CommandOptionList()
            {
                commandOptions = validCommands.ConvertAll(vc => vc.toCommandOption())
            };
            Log.Message($"[RimConnect] Initialized {Settings.CommandOptionListController.commandOptionList.commandOptions.Count} events locally.");
        }
    }
}
