using System.Collections.Generic;
using Archipelago.Gifting.Net.Gifts;

namespace LunacidAP.Data
{
    public class LunacidGifts
    {
        public class ReceivedGift
        {
            public string Name;
            public string ID;
            public ReceivedGift(string name, string id)
            {
                Name = name;
                ID = id;
            }
        }
        
        public static readonly Dictionary<string, GiftItem> LunacidItemsToGifts = new(){
            {"Blood Wine", new GiftItem("Blood Wine", 1, 0)}, 
            {"Light Urn", new GiftItem("Light Urn", 1, 0)}, 
            {"Cloth Bandage", new GiftItem("Cloth Bandage", 1, 0)}, 
            {"Dark Urn", new GiftItem("Dark Urn", 1, 0)}, 
            {"Bomb", new GiftItem("Bomb", 1, 0)}, 
            {"Poison Urn", new GiftItem("Poison Urn", 1, 0)},
            {"Wisp Heart", new GiftItem("Wisp Heart", 1, 0)}, 
            {"Staff of Osiris", new GiftItem("Staff of Osiris", 1, 0)},
            {"Moonlight Vial", new GiftItem("Moonlight Vial", 1, 0)}, 
            {"Spectral Candle", new GiftItem("Spectral Candle", 1, 0)}, 
            {"Health Vial", new GiftItem("Health Vial", 1, 0)}, 
            {"Mana Vial", new GiftItem("Mana Vial", 1, 0)}, 
            {"Fairy Moss", new GiftItem("Fairy Moss", 1, 0)}, 
            {"Crystal Shard", new GiftItem("Crystal Shard", 1, 0)}, 
            {"Poison Throwing Knife", new GiftItem("Poison Throwing Knife", 1, 0)},
            {"Throwing Knife", new GiftItem("Throwing Knife", 1, 0)}, 
            {"Holy Water", new GiftItem("Holy Water", 1, 0)}, 
            {"Antidote", new GiftItem("Antidote", 1, 0)}, 
            {"Survey Banner", new GiftItem("Survey Banner", 1, 0)}, 
            {"Health ViaI", new GiftItem("Health ViaI", 1, 0)}, 
            {"Eggnog", new GiftItem("Eggnog", 1, 0)},
            {"Coal", new GiftItem("Coal", 1, 0)}, 
            {"Pink Shrimp", new GiftItem("Pink Shrimp", 1, 0)}, 
            {"Angel Feather", new GiftItem("Angel Feather", 1, 0)}, 
            {"Ectoplasm", new GiftItem("Ectoplasm", 1, 0)}, 
            {"Snowflake Obsidian", new GiftItem("Snowflake Obsidian", 1, 0)}, 
            {"Moon Petal", new GiftItem("Moon Petal", 1, 0)}, 
            {"Fire Opal", new GiftItem("Fire Opal", 1, 0)}, 
            {"Ashes", new GiftItem("Ashes", 1, 0)},
            {"Opal", new GiftItem("Opal", 1, 0)}, 
            {"Yellow Morel", new GiftItem("Yellow Morel", 1, 0)}, 
            {"Lotus Seed Pod", new GiftItem("Lotus Seed Pod", 1, 0)}, 
            {"Obsidian", new GiftItem("Obsidian", 1, 0)}, 
            {"Onyx", new GiftItem("Onyx", 1, 0)}, 
            {"Ocean Bone Shard", new GiftItem("Ocean Bone Shard", 1, 0)}, 
            {"Bloodweed", new GiftItem("Bloodweed", 1, 0)}, 
            {"Ikurr'ilb Root", new GiftItem("Ikurr'ilb Root", 1, 0)},
            {"Destroying Angel Mushroom", new GiftItem("Destroying Angel Mushroom", 1, 0)}, 
            {"Ocean Bone Shell", new GiftItem("Ocean Bone Shell", 1, 0)}
        };
    }
}