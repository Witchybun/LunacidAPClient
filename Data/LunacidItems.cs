using System;
using System.Collections.Generic;
using Archipelago.Gifting.Net.Giftboxes;
using Archipelago.Gifting.Net.Traits;
using Archipelago.MultiClient.Net.Enums;

namespace LunacidAP.Data
{
    public class LunacidItems
    {
        public class ItemData
        {
            public long ItemID { get; set; }
            public string ItemName { get; set; }
            public string GameObjectName { get; set; }
            public ItemType Type {get; set;}
            public bool OneCount {get; set;}
            public int CountMult {get; set;}
            public string Element {get; set;}

            public ItemData(long itemID, string itemName, string gameObject, ItemType type, bool oneCount = true, int countMult = 1, string element = "NA")
            {
                ItemID = itemID;
                ItemName = ItemName;
                GameObjectName = gameObject;
                Type = type;
                OneCount = oneCount;
                CountMult = countMult;
                Element = element;
            }
        }

        public static readonly List<string> OneCountItems = new(){
            "VHS Tape", "White VHS Tape", "Corrupt Key", "Skull of Josiah", "Fractured Life", "Fractured Death",
            "Earth Talisman", "Water Talisman", "Enchanted Key", "Crystal Lantern", "Oil Lantern", "Terminus Prison Key", "Broken Sword",
            "Black Book", "Vampiric Symbol (W)", "Vampiric Symbol (E)", "Vampiric Symbol (A)", "Earth Elixir", "Ocean Elixir", "Skeleton Egg", "Dried Rat",
            "Skeletal Rattle", "Dusty Crystal Orb", "Great Well Doors Keyring", "Great Well Switches Keyring", "Orb of a Lost Archipelago", "Tent", "Strange Coin"
        };

        public static readonly List<string> Weapons = new(){
           "Axe of Harming", "Battle Axe", "Blade of Jusztina", "Blade of Ophelia", "Blessed Wind", "Broken Hilt", "Broken Lance",
            "Corrupted Dagger", "Dark Rapier", "Elfen Bow", "Elfen Sword", "Fishing Spear", "Flail", "Halberd", "Iron Claw",
            "Moonlight", "Obsidian Seal", "Replica Sword", "Ritual Dagger", "Rusted Sword", "Serpent Fang", "Shadow Blade",
            "Steel Spear", "Stone Club", "Torch", "Twisted Staff", "Vampire Hunter Sword", "Wand of Power", "Wolfram Greatsword",
            "Wooden Shield", "Crossbow", "Steel Needle", "Hammer of Cruelty", "Lucid Blade", "Jotunn Slayer", "Rapier", "Privateer Musket",
            "Brittle Arming Sword", "Golden Khopesh", "Golden Sickle", "Ice Sickle", "Jailor's Candle", "Obsidian Cursebrand", "Obsidian Poisonguard",
            "Skeleton Axe", "Sucsarian Dagger", "Sucsarian Spear", "Cursed Blade", "Lyrian Longsword", "Rusted Sword", "Marauder Black Flail", "Double Crossbow",
            "Fire Sword", "Steel Lance", "Elfen Longsword", "Steel Claw", "Steel Club", "Lyrian Greatsword", "Saint Ishii", "Silver Rapier", "Heritage Sword",
            "Dark Greatsword", "Shining Blade", "Poison Claw", "Iron Club", "Iron Torch"
        };

        public static readonly List<string> Swords = new(){
            "Blade of Jusztina", "Blade of Ophelia", "Blessed Wind", "Broken Hilt", "Corrupted Dagger", "Dark Rapier","Elfen Sword",
            "Moonlight", "Obsidian Seal", "Replica Sword", "Ritual Dagger", "Rusted Sword", "Serpent Fang", "Shadow Blade",
            "Vampire Hunter Sword", "Wolfram Greatsword", "Steel Needle", "Jotunn Slayer", "Rapier",
            "Brittle Arming Sword", "Golden Khopesh", "Golden Sickle", "Ice Sickle", "Obsidian Cursebrand", 
            "Sucsarian Dagger", "Cursed Blade", "Lyrian Longsword", "Rusted Sword", 
            "Elfen Longsword", "Lyrian Greatsword", "Saint Ishii", "Silver Rapier", "Heritage Sword",
            "Dark Greatsword", "Shining Blade"
        };

        public static readonly List<string> Axes = new(){
            "Axe of Harming", "Battle Axe", "Skeleton Axe", 
        };

        public static readonly List<string> Spears = new(){
            "Broken Lance", "Steel Spear", "Fishing Spear", "Sucsarian Spear", "Steel Lance", 
        };

        public static readonly List<string> Shields = new(){
            "Wooden Shield", "Obsidian Poisonguard",
        };

        public static readonly List<string> Bows = new(){
            "Elfen Bow", "Double Crossbow", "Crossbow", "Privateer Musket",
        };

        public static readonly List<string> Hammers = new(){
            "Stone Club", "Hammer of Cruelty", "Steel Club", "Iron Club", 
        };

        public static readonly List<string> Gloves = new(){
            "Iron Claw", "Steel Claw", "Poison Claw"
        };

        public static readonly List<string> WeaponsWithDefaultElement = new(){
            "LUCID BLADE", "WAND OF POWER", "BARRIER", "BESTIAL COMMUNION", "BLOOD DRAIN", "COFFIN", "CORPSE TRANSFORMATION",
            "FLAME FLARE", "GHOST LIGHT", "HOLY WARMTH", "ICARIAN FLIGHT", "LIGHT REVEAL", "LITHOMANCY", "POISON MIST",
            "SPIRIT WARP", "SUMMON FAIRY", "SUMMON ICE SWORD", "WIND DASH", "SUMMON SNAIL", "SUMMON KODAMA", "QUICK STRIDE"
        };

        public static readonly List<string> Spells = new(){
            "Barrier", "Bestial Communion", "Blood Drain", "Blood Strike", "Blue Flame Arc", "Coffin", "Corpse Transformation", "Earth Strike",
            "Earth Thorn", "Fire Worm", "Flame Flare", "Flame Spear", "Ghost Light", "Holy Warmth", "Icarian Flight", "Ice Spear", "Ice Tear",
            "Ignis Calor", "Lava Chasm", "Light Reveal", "Lightning", "Lithomancy", "Moon Beam", "Rock Bridge", "Slime Orb",
            "Spirit Warp", "Summon Fairy", "Summon Ice Sword", "Wind Dash", "Wind Slicer", "Summon Snail", "Summon Kodama", "Tornado", "Dark Skull",
            "Quick Stride", "Jingle Bells", "Poison Mist"
        };

        public static readonly List<string> Items = new(){
            "Blood Wine", "Light Urn", "Cloth Bandage", "Dark Urn", "Bomb", "Poison Urn", "Limbo", "Wisp Heart", "Staff of Osiris",
            "Moonlight Vial", "Spectral Candle", "Health Vial", "Mana Vial", "Fairy Moss", "Crystal Shard", "Poison Throwing Knife",
            "Throwing Knife", "Holy Water", "Antidote", "White VHS Tape", "Ocean Elixir", "Earth Elixir", "Black Book", "Enchanted Key",
            "VHS Tape", "Corrupt Key", "Skull of Josiah", "Vampiric Symbol (W)", "Vampiric Symbol (A)", "Vampiric Symbol (E)", "Crystal Lantern",
            "Terminus Prison Key", "Survey Banner", "Water Talisman", "Earth Talisman", "Oil Lantern", "Strange Coin", "Health ViaI", "Eggnog", "Dusty Crystal Orb",
            "Skeleton Egg", "Dried Rat", "Skeletal Rattle", "Coal", "Pink Shrimp", "Angel Feather", "Tent", "Curry"
        };

        public static readonly List<string> Materials = new(){
            "Ectoplasm", "Snowflake Obsidian", "Moon Petal", "Fractured Life", "Fractured Death", "Broken Sword", "Fire Opal", "Ashes",
            "Opal", "Yellow Morel", "Lotus Seed Pod", "Obsidian", "Onyx", "Ocean Bone Shard", "Bloodweed", "Ikurr'ilb Root",
            "Destroying Angel Mushroom", "Ocean Bone Shell"
        };

        public static readonly List<string> Switches = new(){
            "Hollow Basin Switch Key", "Temple of Silence Switch Key", "Fetid Mire Switch Key", "Accursed Tomb Switch Keyring",
            "Prometheus Fire Switch Keyring", "Forbidden Archives Shortcut Switch Key", "Forbidden Archives Elevator Switch Keyring", 
            "Sealed Ballroom Switch Key", "Grotto Fire Switch Keyring", "Sand Temple Switches Keyring",
            "Terminus Prison Back Alley Switch Key", "Forlorn Arena Gate Switch Key", "Temple of Water Switch Key",
            "Temple of Earth Switch Key", "Labyrinth of Ash Switch Key"
        };

        public static readonly List<string> Keys = new(){
            "Broken Steps Door Key", "Lower Rickety Bridge Door Key", "Sewers Door Key", "Treetop Door Key", "Tomb Secret Door Key",
            "Sewers Sea Door Key", "Accursed Door Key", "Castle Doors Key", "Library Exit Door Key", "Surface Door Key",
            "Light Accursed Door Key", "Queen's Throne Door Key", "Prison Main Door Key", "Secondary Lock Key",
            "Burning Hot Key", "Forbidden Door Key", "Sucsarian Key", "Dreamer Key", "Ballroom Side Rooms Keyring",
            "Tower of Abyss Keyring"
        };

        public static readonly Dictionary<string, int> MaterialNames = new(){
            { "Opal", 0 },
            { "Fire Opal", 2 },
            { "Ocean Bone Shard", 4},
            { "Ocean Bone Shell", 6 },
            { "Onyx", 8},
            { "Obsidian", 10 },
            { "Snowflake Obsidian", 12 },
            { "Yellow Morel", 14 },
            { "Destroying Angel Mushroom", 16 },
            { "Lotus Seed Pod", 18 },
            { "Ashes", 20 },
            { "Ectoplasm", 30 },
            { "Ikurr'ilb Root", 32 },
            { "Vampiric Ashes", 34 },
            { "Bones", 36 },
            { "Bloodweed", 38 },
            { "Moon Petal", 40 },
            { "Fool's Gold", 42 },
            { "Fire Coral", 44 },
            { "Fiddlehead", 46 },
            { "Fractured Death", 48 },
            { "Fractured Life", 50 },
            { "Broken Sword", 52 },
        };

        public static readonly Dictionary<string, int> TrapToHarm = new(){
            {"Bleed Trap", 0},
            {"Poison Trap", 1},
            {"Curse Trap", 2},
            {"Slowness Trap", 4},
            {"Blindness Trap", 5},
            {"Mana Drain Trap", 6},
            {"XP Drain Trap", 7}
        };

        public static readonly Dictionary<string, int> ElementToID = new(){
            {"Normal", 0},
            {"Fire", 1},
            {"Ice", 2},
            {"Poison", 3},
            {"Light", 4},
            {"Dark", 5},
            {"Dark and Light", 8},
            {"Normal and Fire", 9},
            {"Ice and Poison", 10},
            {"Dark and Fire", 11}
        };

        public static readonly List<string> UniqueItems = new(){ // Items that are either unique, or are always unique on a given map.
            "COIN_PICKUP", "ENKEY_PICKUP"
        };

        public static Dictionary<string, string> ArrowToWeapon = new(){
            {"ARROW_CAST", "ELFEN BOW"},
            {"TWISTED_STAFF_CAST", "TWISTED STAFF"},
            {"BOLT_CAST", "CROSSBOW"},
            {"BOLT_CAST2", "DOUBLE CROSSBOW"},
            {"HOOK_CAST", "FISHING SPEAR"},
            {"MOONLIGHT_CAST", "MOONLIGHT"},
            {"BULLET_CAST", "PRIVATEER MUSKET"},
            {"JAILOR_CANDLE_CAST", "JAILORS CANDLE"},
        };

        public static Dictionary<string, string> ItemToPickup = new(){
            {"Blood Wine", "BLOOD_WINE_PICKUP"},
            {"Light Urn", "L_URN_PICKUP"},
            {"Cloth Bandage", "BANDAGE_PICKUP"},
            {"Dark Urn", "D_URN_PICKUP"},
            {"Bomb", "BOMB_PICKUP"},
            {"Poison Urn", "P_URN_PICKUP"},
            {"Wisp Heart", "WISP_HEART_PICKUP"},
            {"Staff of Osiris", "STAFF_PICKUP"},
            {"Moonlight Vial", "Moon_Vial_PICKUP"},
            {"Spectral Candle", "CANDLE_PICKUP"},
            {"Health Vial", "HEALTH_VIAL_PICKUP"},
            {"Mana Vial", "MANA_VIAL_PICKUP"},
            {"Fairy Moss", "FAIRY_MOSS_PICKUP"},
            {"Crystal Shard", "CRYSTAL_SHARD_PICKUP"},
            {"Poison Throwing Knife", "P_THROWING_KNIFE_PICKUP"},
            {"Throwing Knife", "THROWING_KNIFE_PICKUP"},
            {"Holy Water", "HOLY_WATER_PICKUP"},
            {"Antidote", "ANTIDOTE_PICKUP"},
            {"White VHS Tape", "VHS_PICKUP"},
            {"Ocean Elixir", "OCEAN_ELIXIR_PICKUP"},
            {"Earth Elixir", "EARTH_ELIXIR_PICKUP"},
            {"Black Book", "BOOK_PICKUP"}, 
            {"Enchanted Key", "ENKEY_PICKUP"},
            {"VHS Tape", "VHS_PICKUP"},
            {"Corrupt Key", "ENKEY_PICKUP"},
            {"Skull of Josiah", "SKULL_PICKUP"},
            {"Terminus Prison Key", "ENKEY_PICKUP"},
            {"Strange Coin", "GOLD_10"}, 
            {"Health ViaI", "HEALTH_VIAL_PICKUP"},
            {"Angel Feather", "ANGEL_PICKUP"},
            {"Shrimp", "SHRIMP_PICKUP"}
        };

        public static ItemType DetermineItemType(string item)
        {
            if (Weapons.Contains(item))
            {
                return ItemType.Weapon;
            }
            else if (Spells.Contains(item))
            {
                return ItemType.Spell;
            }
            else if (Items.Contains(item))
            {
                return ItemType.Item;
            }
            else if (Materials.Contains(item))
            {
                return ItemType.Material;
            }
            else if (Switches.Contains(item))
            {
                return ItemType.Switch;
            }
            else if (Keys.Contains(item))
            {
                return ItemType.Door;
            }
            else if (item.Contains("Trap"))
            {
                return ItemType.Trap;
            }
            else if (item.Contains("Silver"))
            {
                return ItemType.Gold;
            }
            return ItemType.Unknown;
        }

        public enum ItemType
        {
            Weapon = 0,
            Spell = 1,
            Gold = 2,
            Item = 3,
            Material = 4,
            Switch = 10,
            Trap = 11,
            Door = 12,
            Unknown = 20,
        }
    }

}