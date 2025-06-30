using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Verse;
using RimWorld;

namespace RimConnection
{
    [StaticConstructorOnStartup]
    public static class ServerInitialise
    {
        static ServerInitialise() { Init(); }

        public static bool Init()
        {
            // Generate this before validation so that MP clients have this list available otherwise
            // desyncs occur
            ValidCommandPayloadGenerator validCommandPayloadGenerator = ActionList.ActionListToApi();
            try
            {
                Log.Message("Initialising DonationAlerts integration");

                RimConnectSettings.token = RimConnectSettings.donationToken;

                CommandOptionList commandOptionList = new CommandOptionList();
                commandOptionList.commandOptions = validCommandPayloadGenerator.validCommands
                    .Select(vc => vc.toCommandOption()).ToList();
                Settings.CommandOptionListController.commandOptionList = commandOptionList;

                RimConnectSettings.initialiseSuccessful = !string.IsNullOrEmpty(RimConnectSettings.token);

                return RimConnectSettings.initialiseSuccessful;
            } catch (Exception err)
            {
                Log.Error(err.ToString());
                return false;
            }
        }
    }
}
