using RestSharp;
using System.Collections.Generic;
using System.Linq;

namespace RimConnection
{
    public class Donation
    {
        public int id { get; set; }
        public string name { get; set; }
        public string username { get; set; }
        public string message { get; set; }
        public decimal amount { get; set; }
        public string currency { get; set; }
        public int is_shown { get; set; }
        public string created_at { get; set; }
        public string shown_at { get; set; }
    }

    public class DonationList
    {
        public List<Donation> data { get; set; }
    }

    public static class DonationAlertsAPI
    {
        private static RestClient client = new RestClient("https://www.donationalerts.com/api/v1");

        public static List<Donation> GetDonations(string token)
        {
            var request = new RestRequest("alerts/donations", Method.GET);
            request.AddHeader("Authorization", $"Bearer {token}");
            var response = client.Execute<DonationList>(request);
            if (response.Data != null)
            {
                return response.Data.data ?? new List<Donation>();
            }
            return new List<Donation>();
        }
    }
}
