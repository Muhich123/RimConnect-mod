using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;

namespace RimConnection
{
    public class Donation
    {
        public int id { get; set; }
        public string username { get; set; }
        public string message { get; set; }
        public decimal amount { get; set; }
        public string currency { get; set; }
    }

    public class DonationList
    {
        public List<Donation> data { get; set; }
    }

    public static class DonationAlertsAPI
    {
        private static RestClient client = new RestClient("https://www.donationalerts.com/api/v1/");
        private static int lastId = 0;

        public static List<Donation> GetNewDonations(string token)
        {
            if (string.IsNullOrEmpty(token)) return new List<Donation>();

            var request = new RestRequest("alerts/donations", Method.GET);
            request.AddHeader("Authorization", $"Bearer {token}");

            try
            {
                var response = client.Execute<DonationList>(request);
                var donations = response.Data?.data ?? new List<Donation>();
                var newDonations = donations.Where(d => d.id > lastId).ToList();
                if (newDonations.Any())
                {
                    lastId = newDonations.Max(d => d.id);
                }
                return newDonations;
            }
            catch (Exception)
            {
                return new List<Donation>();
            }
        }
    }
}
