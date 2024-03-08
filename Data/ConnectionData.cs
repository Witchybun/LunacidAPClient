using System;
using System.Collections.Generic;

namespace LunacidAP.Data
{
    public static class ConnectionData
    {
        public static string HostName {get; set;} = "";
        public static string SlotName {get; set;} = "";
        public static int Seed {get; set;} = 0;
        public static string Password {get; set;} = "";
        public static int Symbols {get; set;} = 0;
        public static bool DeathLink {get; set;} = false;
        public static List<ReceivedItem> ReceivedItems {get; set;} = new List<ReceivedItem>(){};
        public static List<string> CompletedLocations {get; set;} = new List<string>(){};
        public static Dictionary<string, CommunionHint.HintData> CommunionHints {get; set;} = new Dictionary<string, CommunionHint.HintData>(){};
        public static Dictionary<string, string> Elements {get; set;} = new (StringComparer.OrdinalIgnoreCase){};

        public static void WriteConnectionData(string hostName, string slotName, string password, 
        int seed = 0, int symbols = 0, bool deathLink = false, List<ReceivedItem> receivedItems = null, List<string> completedLocations = null, 
        Dictionary<string, CommunionHint.HintData> communionHints = null, Dictionary<string, string> elements = null)
        {
            HostName = hostName;
            SlotName = slotName;
            Password = password;
            Seed = seed;
            Symbols = symbols;
            DeathLink = deathLink;
            if (receivedItems is not null)
            {
                ReceivedItems = receivedItems;
            }
            if (completedLocations is not null)
            {
                CompletedLocations = completedLocations;
            }
            if (communionHints is not null)
            {
                CommunionHints = communionHints;
            }
            if (elements is not null)
            {
                Elements = elements;
            }
        }

        public static void WriteConnectionData()
        {
            HostName = "";
            SlotName = "";
            Password = "";
            Seed = 0;
            Symbols = 0;
            DeathLink = false;
            ReceivedItems = new List<ReceivedItem>(){};
            CompletedLocations = new List<string>(){};
            CommunionHints = new Dictionary<string, CommunionHint.HintData>(){};
            Elements = new(){};

        }

    }
}