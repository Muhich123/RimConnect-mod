using System;
using System.IO;
using Verse;

namespace RimConnection
{
    class DonationFilePoller : GameComponent
    {
        private readonly string queuePath = Path.Combine(GenFilePaths.ConfigFolderPath, "RimConnect", "donations_queue.txt");

        public DonationFilePoller(Game game) { }

        public override void GameComponentTick()
        {
            if (!File.Exists(queuePath))
            {
                return;
            }

            string[] lines;
            try
            {
                lines = File.ReadAllLines(queuePath);
                File.Delete(queuePath);
            }
            catch (Exception e)
            {
                Log.Error($"DonationFilePoller failed to read queue: {e.Message}");
                return;
            }

            foreach (string line in lines)
            {
                if (string.IsNullOrWhiteSpace(line))
                    continue;

                string[] parts = line.Split(';');
                if (parts.Length < 2)
                    continue;

                if (!int.TryParse(parts[0], out int amount))
                    continue;
                string boughtBy = parts[1];

                TriggerEventByAmount(amount, boughtBy);
            }
        }

        private void TriggerEventByAmount(int amount, string boughtBy)
        {
            var list = Settings.CommandOptionListController.commandOptionList;
            if (list == null)
                return;

            foreach (var option in list.commandOptions)
            {
                try
                {
                    if (option.costSilverStore == amount && option.Action().Category == "Event")
                    {
                        option.Action().Execute(1, boughtBy);
                        break;
                    }
                }
                catch (Exception e)
                {
                    Log.Error($"DonationFilePoller error executing action: {e.Message}");
                }
            }
        }
    }
}
