using System.Collections.Generic;

namespace LunacidAP.Data
{
    public class LunacidItems
    {
        public static readonly List<string> OneCountItems = new(){
            "VHS Tape", "White VHS Tape", "Corrupt Key", "Skull of Josiah", "Fractured Life", "Fractured Death",
            "Earth Talisman", "Water Talisman", "Enchanted Key", "Crystal Lantern", "Oil Lantern", "Terminus Prison Key", "Broken Sword",
            "Black Book", "Vampiric Symbol (W)", "Vampiric Symbol (E)", "Vampiric Symbol (A)", "Earth Elixir", "Ocean Elixir", "Skeleton Egg", "Dried Rat", 
            "Skeleton Rattle"
        };
        
        public static readonly List<string> Weapons = new(){
           "Axe of Harming", "Battle Axe", "Blade of Jusztina", "Blade of Ophelia", "Blessed Wind", "Broken Hilt", "Broken Lance",
            "Corrupted Dagger", "Dark Rapier", "Elfen Bow", "Elfen Sword", "Fishing Spear", "Flail", "Halberd", "Iron Claw",
            "Moonlight", "Obsidian Seal", "Replica Sword", "Ritual Dagger", "Ruested Sword", "Serpent Fang", "Shadow Blade",
            "Steel Spear", "Stone Club", "Torch", "Twisted Staff", "Vampire Hunter Sword", "Wand of Power", "Wolfram Greatsword",
            "Wooden Shield", "Crossbow", "Steel Needle", "Hammer of Cruelty", "Lucid Blade", "Jotunn Slayer", "Rapier", "Privateer Musket",
            "Brittle Arming Sword", "Golden Kopesh", "Golden Sickle", "Ice Sickle", "Jailor's Candle", "Obsidian Cursebrand", "Obsidian Poisonguard",
            "Skeleton Axe", "Sucsarian Dagger", "Sucsarian Spear", "Cursed Blade", "Lyrian Longsword", "Rusted Sword", "Marauder Black Flail", "Double Crossbow",
            "Fire Sword", "Steel Lance", "Elfen Longsword", "Steel Claw", "Steel Club", "Lyrian Greatsword", "Saint Ishii", "Silver Rapier", "Heritage Sword",
            "Dark Greatsword", "Shining Blade", "Poison Claw", "Iron Club", "Iron Torch"
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
            "Skeleton Egg", "Dried Rat", "Skeleton Rattle"
        };

        public static readonly List<string> Materials = new(){
            "Ectoplasm", "Snowflake Obsidian", "Moon Petal", "Fractured Life", "Fractured Death", "Broken Sword", "Fire Opal", "Ashes", 
            "Opal", "Yellow Morel", "Lotus Seed Pod", "Obsidian", "Onyx", "Ocean Bone Shard", "Bloodweed", "Ikurr'ilb Root",
            "Destroying Angel Mushroom"
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
            { "Destroying Angel", 16 },
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
    }
}