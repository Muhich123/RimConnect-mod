using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RimConnection
{
    public class AuthMod
    {
        public string token = RimConnectSettings.donationToken;
    }

    public class AuthModResponse
    {
        public string token { get; set; }
    }
}