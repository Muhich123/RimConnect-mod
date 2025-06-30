using System;
using System.Collections.Generic;

namespace RimConnection
{
    public class DonationAlertsResponse
    {
        public List<DonationAlert> data { get; set; }
    }

    public class DonationAlert
    {
        public int id { get; set; }
        public string username { get; set; }
        public string message { get; set; }
        public double amount { get; set; }
        public string currency { get; set; }
    }
}
