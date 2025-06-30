using System;
using System.Collections.Generic;
using System.Linq;
using RestSharp;
using Verse;

namespace RimConnection
{
    public class DonationAlertsAPI
    {
        private static readonly RestClient client = new RestClient("https://www.donationalerts.com/api/v1/");
        private static int lastId = 0;

        public class Donation
        {
            public int id { get; set; }
            public string name { get; set; }
            public string username { get; set; }
            public string message { get; set; }
            public float amount { get; set; }
            public string currency { get; set; }
            public int is_shown { get; set; }
            public string created_at { get; set; }
            public string shown_at { get; set; }
        }

        public class DonationResponse
        {
            public List<Donation> data { get; set; }
        }

        public static List<Donation> GetNewDonations()
        {
            if (string.IsNullOrEmpty(RimConnectSettings.donationAlertsToken))
            {
                return new List<Donation>();
            }

            var request = new RestRequest("alerts/donations", Method.GET);
            request.AddHeader("Authorization", $"Bearer {RimConnectSettings.donationAlertsToken}");

            var response = client.Execute<DonationResponse>(request);
            if (response.StatusCode != System.Net.HttpStatusCode.OK)
            {
                Log.Error($"DonationAlerts API error: {response.StatusCode}");
                return new List<Donation>();
            }

            var donations = response.Data?.data ?? new List<Donation>();
            var newDonations = donations.Where(d => d.id > lastId).OrderBy(d => d.id).ToList();
            if (newDonations.Count > 0)
            {
                lastId = newDonations.Last().id;
            }
            return newDonations;
        }
    }
}
