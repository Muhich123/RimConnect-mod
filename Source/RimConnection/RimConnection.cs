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
        }

        public override void DoSettingsWindowContents(Rect inRect)
        {
            settings.DoWindowContents(inRect);
        }

        public override string SettingsCategory()
        {
            return "RimConnect";
        }

        // Event list generation now occurs after game defs are loaded via
        // DonationInitialise to ensure DefOfs are available.
    }
}
