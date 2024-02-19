using System.Collections.Generic;

namespace LunacidAP.Data
{
    public class LunacidItems
    {
        public static readonly List<string> APItems = new(){
            "Progressive Vampiric Symbol"
        };
        
        public static readonly List<string> Weapons = new(){
           "Axe of Harming", "Battle Axe", "Blade of Jusztina", "Blade of Ophelia", "Blessed Wind", "Broken Hilt", "Broken Lance",
            "Corrupted Dagger", "Dark Rapier", "Elfen Bow", "Elfen Sword", "Fishing Spear", "Flail", "Halberd", "Iron Claw",
            "Moonlight", "Obsidian Seal", "Replica Sword", "Ritual Dagger", "Ruested Sword", "Serpent Fang", "Shadow Blade",
            "Steel Spear", "Stone Club", "Torch", "Twisted Staff", "Vampire Hunter Sword", "Wand of Power", "Wolfram Greatsword",
            "Wooden Shield", "Crossbow", "Steel Needle", "Hammer of Cruelty", "Lucid Blade", "Jotunn Slayer", "Rapier", "Privateer Musket",
            "Brittle Arming Sword", "Golden Kopesh", "Golden Sickle", "Ice Sickle", "Jailor's Candle", "Obsidian Cursebrand", "Obsidian Poisonguard",
            "Skeleton Axe", "Sucsarian Dagger", "Sucsarian Spear", "Cursed Blade", "Lyrian Longsword"
        };

        public static readonly List<string> Spells = new(){
            "Barrier", "Bestial Communion", "Blood Drain", "Blood Strike", "Blue Flame Arc", "Coffin", "Corpse Transformation", "Earth Strike",
            "Earth Thorn", "Fire Worm", "Flame Flare", "Flame Spear", "Ghost Light", "Holy Warmth", "Icarian Flight", "Ice Spear", "Ice Tear",
            "Ignis Calor", "Lava Chasm", "Light Reveal", "Lightning", "Lithomancy", "Moon Beam", "Poison Mist", "Rock Bridge", "Slime Orb",
            "Spirit Warp", "Summon Fairy", "Summon Ice Sword", "Wind Dash", "Wind Slicer", "Summon Snail", "Summon Kodama", "Tornado", "Dark Skull",
            "Quick Stride"
        };

        public static readonly List<string> Items = new(){
            "Blood Wine", "Light Urn", "Cloth Bandage", "Dark Urn", "Bomb", "Poison Urn", "Limbo", "Wisp Heart", "Staff of Osiris", 
            "Moonlight Vial", "Spectral Candle", "Health Vial", "Mana Vial", "Fairy Moss", "Crystal Shard", "Poison Throwing Knife", 
            "Throwing Knife", "Holy Water", "Antidote", "White VHS Tape", "Ocean Elixir", "Earth Elixir", "Black Book", "Enchanted Key",
            "VHS Tape", "Corrupted Key", "Skull of Josiah", "Vampiric Symbol (W)", "Vampiric Symbol (A)", "Vampiric Symbol (E)", "Crystal Lantern",
            "Terminus Prison Key", "Survey Banner", "Water Talisman", "Earth Talisman", "Oil Lantern", "Strange Coin"
        };

        public static readonly List<string> Materials = new(){
            "Ectoplasm", "Snowflake Obsidian", "Moon Petal", "Fractured Life", "Fractured Death", "Broken Sword", "Fire Opal", "Ashes", 
            "Opal", "Yellow Morel", "Lotus Seed Pod", "Obsidian", "Onyx", "Ocean Bone Shard", "Bloodweed", "Ikurr'ilb Root",
            "Destroying Angel Mushroom"
        };

        public static readonly Dictionary<string, int> MaterialNames = new(){
            { "Obsidian", 10 },
            { "Ashes", 20 },
            { "Ocean Bone Shard", 4},
            { "Onyx", 8},
            { "Yellow Morel", 14 },
            { "Bones", 36 },
            { "Ectoplasm", 30 },
            { "Ikurr'ilb Root", 32 },
            { "Snowflake Obsidian", 12 },
            { "Fool's Gold", 42},


        };

        public static readonly List<string> UniqueItems = new(){ // Items that are either unique, or are always unique on a given map.
            "COIN_PICKUP", "ENKEY_PICKUP"
        };
    }
}