using System.Collections.Generic;

namespace LunacidAP.Data
{
    public static class ConnectionData
    {
        public static string HostName {get; set;} = "localhost:38281";
        public static string SlotName {get; set;} = "Player1";
        public static string Password {get; set;} = "";
        public static int ItemIndex {get; set;} = 1;
        public static List<long> ReceivedItems {get; set;} = new List<long>(){};
        public static List<string> CompletedLocations {get; set;} = new List<string>(){};

        public static void WriteConnectionData(string hostName, string slotName, string password, 
        int itemIndex = 0, List<long> receivedItems = null, List<string> completedLocations = null)
        {
            HostName = hostName;
            SlotName = slotName;
            Password = password;
            if (itemIndex > 0)
            {
                ItemIndex = itemIndex;
            }
            if (receivedItems is not null)
            {
                ReceivedItems = receivedItems;
            }
            if (completedLocations is not null)
            {
                CompletedLocations = completedLocations;
            }
        }

        public static void WriteConnectionData()
        {
            HostName = "localhost:38281";
            SlotName = "Player1";
            Password = "";
            ItemIndex = 1;
            ReceivedItems = new List<long>(){};
            CompletedLocations = new List<string>(){};

        }

    }
}