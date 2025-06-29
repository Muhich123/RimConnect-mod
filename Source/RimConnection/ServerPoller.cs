using Multiplayer.API;
using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Verse;
using RimConnection.API;

namespace RimConnection
{
    class ServerPoller : GameComponent
    {
        static DateTime lastGETRequest = DateTime.UtcNow;
        static readonly TimeSpan timeBetweenRequests = TimeSpan.FromSeconds(30d);
        static ConcurrentQueue<Command> commandQueue = new ConcurrentQueue<Command>();

        private DateTime previousDateTime;

        public ServerPoller(Game game)
        {
        }

        public override void FinalizeInit()
        {
            previousDateTime = DateTime.Now;
        }

        public override void GameComponentTick()
        {
            if (!string.IsNullOrEmpty(RimConnectSettings.donationAlertsToken))
            {
                if (DateTime.UtcNow - lastGETRequest > timeBetweenRequests)
                {
                    lastGETRequest = DateTime.UtcNow;
                    ServerChecker();
                }
            }

            if (commandQueue.TryDequeue(out Command command))
            {
                IAction action = ActionList.actionLookup[command.actionHash];
                action.Execute(command.amount, command.boughtBy);
                Find.TickManager.slower.SignalForceNormalSpeedShort();
            }
        }

        public static async void ServerChecker()
        {
            await Task.Run(() =>
            {
                var donations = DonationAlertsAPI.GetDonations(RimConnectSettings.donationAlertsToken);

                foreach (var donation in donations)
                {
                    if (donation.id <= RimConnectSettings.lastDonationId)
                        continue;

                    RimConnectSettings.lastDonationId = donation.id;

                    if (CommandOptionListController.commandOptionList == null)
                        continue;

                    foreach (var option in CommandOptionListController.commandOptionList.commandOptions)
                    {
                        if (option.costSilverStore == (int)donation.amount)
                        {
                            Command cmd = new Command { actionHash = option.actionHash, amount = 1, boughtBy = donation.username };
                            addCommandToQueue(cmd);
                            break;
                        }
                    }
                }
            });

            return;
        }

        [SyncMethod]
        public static void addCommandToQueue(Command command)
        {
            commandQueue.Enqueue(command);
        }
    }
}
