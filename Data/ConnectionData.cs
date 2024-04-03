using System;
using System.Collections.Generic;

namespace LunacidAP.Data
{
    public static class ConnectionData
    {
        public static string HostName {get; set;} = "";
        public static int Port {get; set;} = 0;
        public static string SlotName {get; set;} = "";
        public static int Seed {get; set;} = 0;
        public static string Password {get; set;} = "";
        public static int Index {get; set;} = 0;
        public static bool DeathLink {get; set;} = false;
        public static List<ReceivedItem> ReceivedItems {get; set;} = new List<ReceivedItem>(){};
        public static List<long> CompletedLocations {get; set;} = new List<long>(){};
        public static Dictionary<string, CommunionHint.HintData> CommunionHints {get; set;} = new Dictionary<string, CommunionHint.HintData>(){};
        public static Dictionary<string, string> Elements {get; set;} = new (StringComparer.OrdinalIgnoreCase){};
        public static Dictionary<string, string> Entrances {get; set;} = new (){};

        public static void WriteConnectionData(string hostName, int port, string slotName, string password, 
        int seed = 0, int index = 0, bool deathLink = false, List<ReceivedItem> receivedItems = null, List<long> completedLocations = null, 
        Dictionary<string, CommunionHint.HintData> communionHints = null, Dictionary<string, string> elements = null, Dictionary<string, string> entrances = null)
        {
            HostName = hostName;
            Port = port;
            SlotName = slotName;
            Password = password;
            Seed = seed;
            Index = index;
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
            if (entrances is not null)
            {
                Entrances = entrances;
            }
        }

        public static void WriteConnectionData()
        {
            HostName = "";
            Port = 0;
            SlotName = "";
            Password = "";
            Seed = 0;
            Index = 0;
            DeathLink = false;
            ReceivedItems = new List<ReceivedItem>(){};
            CompletedLocations = new List<long>(){};
            CommunionHints = new Dictionary<string, CommunionHint.HintData>(){};
            Elements = new(){};
            Entrances = new(){};

        }

    }
}