using Verse;
using RimWorld;
using UnityEngine;

namespace RimConnection
{
    static class AlertManager
    {
        private static LetterDef donationEventLetterDef = DefDatabase<LetterDef>.GetNamed("DonationEvent");
        private static LetterDef badDonationEventLetterDef = DefDatabase<LetterDef>.GetNamed("DangerousDonationEvent");

        private static void EventNotification(string label, string description, LetterDef letterDef, IntVec3? location)
        {
            var currentMap = Find.CurrentMap;

            if(location == null)
            {
                Find.LetterStack.ReceiveLetter( new TaggedString(label), new TaggedString(description), letterDef);
            } else
            {
                // Have to make sure that location isn't null otherwise compiler complains
                var newVector = location.GetValueOrDefault();
                Find.LetterStack.ReceiveLetter(new TaggedString(label), new TaggedString(description), letterDef, new LookTargets(newVector, currentMap));
            }
        }
        
        public static void BadEventNotification(string description)
        {
            EventNotification("Donation Event", description, badDonationEventLetterDef, null );
        }
        
        public static void BadEventNotification(string description, IntVec3 location)
        {
            EventNotification("Donation Event", description, badDonationEventLetterDef, location);
        }

        public static void BadEventNotification(string description, string boughtBy) 
        {
            EventNotification("Donation Event", ParseNotificationMessage(description, boughtBy), badDonationEventLetterDef, null);
        }

        public static void BadEventNotification(string description, IntVec3 location, string boughtBy) 
        {
            EventNotification("Donation Event", ParseNotificationMessage(description, boughtBy), badDonationEventLetterDef, location);
        }
        
        public static void NormalEventNotification(string description)
        {
            EventNotification("Donation Event", description, donationEventLetterDef, null);
        }
        
        public static void ResourceDropNotification(string description, IntVec3 location)
        {
            EventNotification("Donation Drop", description, donationEventLetterDef, location);
        }

        public static void NormalEventNotification(string description, string boughtBy)
        {
            EventNotification("Donation Event", ParseNotificationMessage(description, boughtBy), donationEventLetterDef, null);
        }

        public static void ResourceDropNotification(string description, IntVec3 location, string boughtBy)
        {
            EventNotification("Donation Drop", ParseNotificationMessage(description, boughtBy), donationEventLetterDef, location);
        }
        
        public static string ParseNotificationMessage(string message, string boughtBy) 
        {
            if (boughtBy == "Poll") { boughtBy = "Your viewers"; }
            boughtBy = $"<color=#9147ff>{boughtBy}</color>"; 
            return string.Format(message, boughtBy);
        }

    }
}
