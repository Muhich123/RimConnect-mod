using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Verse;
using RimWorld;

namespace RimConnection
{
    public static class ServerInitialise
    {

        public static bool Init()
        {
            Log.Message("DonationAlerts mode active - no server initialisation");
            return true;
        }
    }
}
