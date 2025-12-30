using System;
using System.Collections.Generic;
using static LunacidAP.Data.LunacidEnemies;
using static LunacidAP.Data.LunacidGifts;

namespace LunacidAP.Data
{
    public static class ConnectionData
    {
        public static string HostName {get; private set;} = "";
        public static int Port {get; private set;}
        public static string SlotName {get; private set;} = "";
        public static int Seed {get; private set;}
        public static string Password {get; private set;} = "";
        public static int Index {get; set;}
        public static bool DeathLink {get; set;}
        public static int CheatedCount {get; private set;}
        public static int StoredLevel {get; set;}
        public static int StoredExperience {get; set;}
        public static Dictionary<string, ReceivedItem> ReceivedItems {get; private set;} = new();
        public static List<long> CompletedLocations {get; private set;} = new();
        public static Dictionary<string, string> CommunionHints {get; set;} = new();
        public static Dictionary<string, string> Elements {get; private set;} = new (StringComparer.OrdinalIgnoreCase);
        public static Dictionary<string, LunacidEquipStats.WeaponData> RandomizedWeaponData { get; set; } = new();
        public static Dictionary<string, LunacidEquipStats.SpellData> RandomizedSpellData { get; set; } = new();
        public static Dictionary<string, string> Entrances {get; set;} = new ();
        public static Dictionary<string, string> TraversedEntrances { get; private set; } = new();
        public static SortedDictionary<long, ArchipelagoItem> ScoutedLocations = new();
        public static List<string> EnteredScenes = new();
        public static HashSet<string> BoughtItems = new();
        public static List<ReceivedGift> ReceivedGifts = new();
        public static Dictionary<string, string> ItemColors = new();
        public static Dictionary<string, List<RandomizedEnemyData>> RandomEnemyData = new();

        

        public static void WriteConnectionData(string hostName, int port, string slotName, string password, int storedLevel = 0, int storedExperience = 0,
        int seed = 0, int index = 0, bool deathLink = false, int cheatedCount = -1, Dictionary<string, ReceivedItem> receivedItems = null, List<long> completedLocations = null, 
        Dictionary<string, string> communionHints = null, Dictionary<string, string> elements = null, Dictionary<string, LunacidEquipStats.WeaponData> randomWeaponData = null, 
        Dictionary<string, LunacidEquipStats.SpellData> randomSpellData = null, Dictionary<string, string> entrances = null,
        Dictionary<string, string> traversedEntrances = null, SortedDictionary<long, ArchipelagoItem> scouts = null, List<string> enteredScenes = null, HashSet<string> boughtItems = null, List<ReceivedGift> receivedGifts = null, 
        Dictionary<string, string> itemColors = null, Dictionary<string, List<RandomizedEnemyData>> randomEnemyData = null)
        {
            HostName = hostName;
            Port = port;
            SlotName = slotName;
            Password = password;
            Seed = seed;
            DeathLink = deathLink;
            if (index > 0)
            {
                Index = index;
            }
            if (storedLevel > 0)
            {
                StoredLevel = storedLevel;
            }
            if (storedExperience > 0)
            {
                StoredExperience = storedExperience;
            }
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

            if (randomWeaponData is not null)
            {
                RandomizedWeaponData = randomWeaponData;
            }

            if (randomSpellData is not null)
            {
                RandomizedSpellData = randomSpellData;
            }
            if (entrances is not null)
            {
                Entrances = entrances;
            }
            if (traversedEntrances is not null)
            {
                TraversedEntrances = traversedEntrances;
            }
            if (scouts is not null)
            {
                ScoutedLocations = scouts;
            }
            if (enteredScenes is not null)
            {
                EnteredScenes = enteredScenes;
            }
            if (boughtItems is not null)
            {
                BoughtItems = boughtItems;
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
            StoredLevel = 5;
            StoredExperience = 0;
            ReceivedItems = new Dictionary<string, ReceivedItem>();
            CompletedLocations = new List<long>();
            CommunionHints = new Dictionary<string, string>();
            Elements = new Dictionary<string, string>();
            RandomizedWeaponData = new Dictionary<string, LunacidEquipStats.WeaponData>();
            RandomizedSpellData = new Dictionary<string, LunacidEquipStats.SpellData>();
            Entrances = new Dictionary<string, string>();
            TraversedEntrances = new Dictionary<string, string>();
            ScoutedLocations = new SortedDictionary<long, ArchipelagoItem>();
            EnteredScenes = new List<string>();
            BoughtItems = new HashSet<string>();
            ReceivedGifts = new List<ReceivedGift>();
            ItemColors = new Dictionary<string, string>();
            RandomEnemyData = new Dictionary<string, List<RandomizedEnemyData>>();
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