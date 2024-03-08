using System.Collections.Generic;

namespace LunacidAP.Data
{
    public static class ConnectionData
    {
        public static string HostName {get; set;} = "localhost:38281";
        public static string SlotName {get; set;} = "Player1";
        public static int Seed {get; set;} = 0;
        public static string Password {get; set;} = "";
        public static int Symbols {get; set;} = 0;
        public static bool DeathLink {get; set;} = false;
        public static List<ReceivedItem> ReceivedItems {get; set;} = new List<ReceivedItem>(){};
        public static List<string> CompletedLocations {get; set;} = new List<string>(){};
        public static Dictionary<string, CommunionHint.HintData> CommunionHints {get; set;} = new Dictionary<string, CommunionHint.HintData>(){};

        public static void WriteConnectionData(string hostName, string slotName, string password, 
        int seed = 0, List<ReceivedItem> receivedItems = null, List<string> completedLocations = null, 
        Dictionary<string, CommunionHint.HintData> communionHints = null)
        {
            HostName = hostName;
            SlotName = slotName;
            Password = password;
            Seed = seed;
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
        }

        public static void WriteConnectionData()
        {
            HostName = "localhost:38281";
            SlotName = "Player1";
            Password = "";
            Seed = 0;
            ReceivedItems = new List<ReceivedItem>(){};
            CompletedLocations = new List<string>(){};
            CommunionHints = new Dictionary<string, CommunionHint.HintData>(){};

        }

    }
}