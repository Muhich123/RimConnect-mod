using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Threading.Tasks;
using Verse;
using RimConnection.Settings;

namespace RimConnection
{
    class DonationAlertsPoller : GameComponent
    {
        static DateTime lastCheck = DateTime.UtcNow;
        static readonly TimeSpan interval = TimeSpan.FromSeconds(30d);
        static ConcurrentQueue<Command> queue = new ConcurrentQueue<Command>();

        public DonationAlertsPoller(Game game)
        {
        }

        public override void GameComponentTick()
        {
            if (DateTime.UtcNow - lastCheck > interval)
            {
                lastCheck = DateTime.UtcNow;
                CheckDonations();
            }

            if (queue.TryDequeue(out Command command))
            {
                IAction action = ActionList.actionLookup[command.actionHash];
                action.Execute(command.amount, command.boughtBy);
                Find.TickManager.slower.SignalForceNormalSpeedShort();
            }
        }

        public static async void CheckDonations()
        {
            await Task.Run(() =>
            {
                var donations = DonationAlertsAPI.GetNewDonations();
                foreach (var donation in donations)
                {
                    var cmd = DonationToCommand(donation.amount);
                    if (cmd != null)
                    {
                        queue.Enqueue(cmd);
                    }
                }
            });
        }

        private static Command DonationToCommand(float amount)
        {
            int val = (int)Math.Round(amount);
            var option = CommandOptionListController.commandOptionList?.commandOptions
                .FirstOrDefault(co => co.costSilverStore == val);
            if (option == null) return null;
            return new Command
            {
                actionHash = option.actionHash,
                amount = val,
                boughtBy = "DonationAlerts"
            };
        }
    }
}
