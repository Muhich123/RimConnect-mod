using Verse;

namespace RimConnection
{
    // Legacy class retained for backward compatibility with older save games.
    // It now simply uses the DonationPoller logic.
    class ServerPoller : DonationPoller
    {
        public ServerPoller(Game game)
            : base(game)
        {
        }

        public ServerPoller()
            : base(null)
        {
        }
    }
}
