using Verse;

namespace RimConnection
{
    // Legacy component kept for compatibility with older save games.
    class SetWorldName : GameComponent
    {
        public SetWorldName(Game game) : base(game)
        {
        }

        public SetWorldName() : base(null)
        {
        }

        public override void FinalizeInit()
        {
            base.FinalizeInit();
            string worldName = Find.World.info.name;
            Log.Message($"World name is {worldName}");
            // Previously this sent the world name to a server. No longer needed.
        }
    }
}
