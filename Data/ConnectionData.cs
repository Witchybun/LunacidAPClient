using System;
using System.Collections.Generic;
using static LunacidAP.Data.LunacidEnemies;
using static LunacidAP.Data.LunacidGifts;

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
        public static int CheatedCount {get; set;} = 0;
        public static List<ReceivedItem> ReceivedItems {get; set;} = new List<ReceivedItem>(){};
        public static List<long> CompletedLocations {get; set;} = new List<long>(){};
        public static Dictionary<string, CommunionHint.HintData> CommunionHints {get; set;} = new Dictionary<string, CommunionHint.HintData>(){};
        public static Dictionary<string, string> Elements {get; set;} = new (StringComparer.OrdinalIgnoreCase){};
        public static Dictionary<string, string> Entrances {get; set;} = new (){};
        public static SortedDictionary<long, ArchipelagoItem> ScoutedLocations = new(){};
        public static List<string> EnteredScenes = new(){};
        public static List<ReceivedGift> ReceivedGifts = new(){};
        public static Dictionary<string, string> ItemColors = new(){};
        public static Dictionary<string, List<RandomizedEnemyData>> RandomEnemyData = new(){};

        public static void WriteConnectionData(string hostName, int port, string slotName, string password, 
        int seed = 0, int index = 0, bool deathLink = false, int cheatedCount = -1, List<ReceivedItem> receivedItems = null, List<long> completedLocations = null, 
        Dictionary<string, CommunionHint.HintData> communionHints = null, Dictionary<string, string> elements = null, Dictionary<string, string> entrances = null,
        SortedDictionary<long, ArchipelagoItem> scouts = null, List<string> enteredScenes = null, List<ReceivedGift> receivedGifts = null, 
        Dictionary<string, string> itemColors = null, Dictionary<string, List<RandomizedEnemyData>> randomEnemyData = null)
        {
            HostName = hostName;
            Port = port;
            SlotName = slotName;
            Password = password;
            Seed = seed;
            Index = index;
            DeathLink = deathLink;
            if (cheatedCount > -1)
            {
                CheatedCount = cheatedCount;
            }
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
            if (scouts is not null)
            {
                ScoutedLocations = scouts;
            }
            if (enteredScenes is not null)
            {
                EnteredScenes = enteredScenes;
            }
            if (receivedGifts is not null)
            {
                ReceivedGifts = receivedGifts;
            }
            if (itemColors is not null)
            {
                ItemColors = itemColors;
            }
            if (randomEnemyData is not null)
            {
                RandomEnemyData = randomEnemyData;
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
            CheatedCount = 0;
            ReceivedItems = new List<ReceivedItem>(){};
            CompletedLocations = new List<long>(){};
            CommunionHints = new Dictionary<string, CommunionHint.HintData>(){};
            Elements = new(){};
            Entrances = new(){};
            ScoutedLocations = new(){};
            EnteredScenes = new(){};
            ReceivedGifts = new(){};
            ItemColors = new(){};
            RandomEnemyData = new(){};
        }

        public static readonly Dictionary<int, string> ClassEnumToName = new(){
            {0, "THIEF"},
            {1, "KNIGHT"},
            {2, "WITCH"},
            {3, "VAMPIRE"},
            {4, "UNDEAD"},
            {5, "ROYAL"},
            {6, "CLERIC"},
            {7, "SHINOBI"},
            {8, "FORSAKEN"},
            {9, "RANDOM"},
        };
    }
}