using System;
using System.Collections.Generic;
using RestSharp;
using Verse;

namespace RimConnection.API
{
    public class Donation
    {
        public int id { get; set; }
        public string username { get; set; }
        public string message { get; set; }
        public double amount { get; set; }
    }

    public class DonationList
    {
        public List<Donation> data { get; set; }
    }

    public static class DonationAlertsAPI
    {
        static DonationAlertsAPI()
        {
            System.Net.ServicePointManager.SecurityProtocol = System.Net.SecurityProtocolType.Tls12;
            client = new RestClient("https://www.donationalerts.com/api/v1/");
        }

        private static RestClient client;

        public static List<Donation> GetDonations(string token)
        {
            if (string.IsNullOrEmpty(token))
            {
                Log.Warning("DonationAlertsAPI missing token");
                return new List<Donation>();
            }

            var request = new RestRequest("alerts/donations", Method.GET);
            request.AddHeader("Authorization", $"Bearer {token}");
            request.AddHeader("Accept", "application/json");

            try
            {
                var response = client.Execute<DonationList>(request);
                if ((int)response.StatusCode == 0)
                {
                    Log.Warning($"DonationAlertsAPI failed: {response.ErrorMessage}");
                    return new List<Donation>();
                }
                if (response.StatusCode == System.Net.HttpStatusCode.Forbidden)
                {
                    Log.Warning("DonationAlertsAPI failed: Forbidden - check your token");
                    return new List<Donation>();
                }
                if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                {
                    Log.Warning("DonationAlertsAPI failed: Unauthorized - check your DonationAlerts token");
                    return new List<Donation>();
                }

                if (response.StatusCode != System.Net.HttpStatusCode.OK || response.Data == null)
                {
                    Log.Warning($"DonationAlertsAPI failed: {response.StatusCode}");
                    return new List<Donation>();
                }

                return response.Data.data ?? new List<Donation>();
            }
            catch (Exception e)
            {
                Log.Error($"DonationAlertsAPI error: {e.Message}");
                return new List<Donation>();
            }
        }
    }
}
