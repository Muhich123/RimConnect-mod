using Multiplayer.API;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Verse;

namespace RimConnection
{
    class ServerPoller : GameComponent
    {
        static DateTime lastGETRequest = DateTime.UtcNow;
        static readonly TimeSpan timeBetweenRequests = TimeSpan.FromSeconds(30d);
        static ConcurrentQueue<Command> commandQueue = new ConcurrentQueue<Command>();

        public ServerPoller(Game game)
        {
        }

        public override void GameComponentTick()
        {
            // Only do this stuff if the mod successfully connected to the server
            if (RimConnectSettings.initialiseSuccessful)
            {
                if (DateTime.UtcNow - lastGETRequest > timeBetweenRequests)
                {
                    lastGETRequest = DateTime.UtcNow;
                    DonationChecker();
                }
            }

            if (commandQueue.TryDequeue(out Command command))
            {
                IAction action = ActionList.actionLookup[command.actionHash];
                action.Execute(command.amount, command.boughtBy);
                Find.TickManager.slower.SignalForceNormalSpeedShort();
            }
        }

        public static async void DonationChecker()
        {
            await Task.Run(() =>
            {
                List<Donation> donations = DonationAlertsAPI.GetNewDonations(RimConnectSettings.token);
                foreach (Donation donation in donations)
                {
                    int amountValue = (int)Math.Floor(donation.amount);
                    foreach (var option in Settings.CommandOptionListController.commandOptionList.commandOptions)
                    {
                        if (option.costSilverStore == amountValue)
                        {
                            Command command = new Command
                            {
                                actionHash = option.actionHash,
                                amount = 1,
                                boughtBy = donation.username
                            };
                            addCommandToQueue(command);
                        }
                    }
                }
            });
        }

        [SyncMethod]
        public static void addCommandToQueue(Command command)
        {
            commandQueue.Enqueue(command);
        }
    }
}
