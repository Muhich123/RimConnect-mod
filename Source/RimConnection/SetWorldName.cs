using Verse;

namespace RimConnection
{
    // Legacy component kept for save compatibility
    class SetWorldName : GameComponent
    {
        public SetWorldName(Game game) { }

        public SetWorldName() { }

        public override void FinalizeInit()
        {
            base.FinalizeInit();
            // No-op: previously updated world name on server
        }
    }
}
