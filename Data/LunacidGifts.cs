using System.Collections.Generic;
using Archipelago.Gifting.Net.Versioning.Gifts;
using Archipelago.Gifting.Net.Versioning.Gifts.Current;
using UnityEngine;

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

        public static readonly Dictionary<string, GiftVector> LunacidItemsToGifts = new(){
            {"Blood Wine", new GiftVector(new GiftItem("Blood Wine", 1, 0), LunacidTraits.LunacidItemTraits["Blood Wine"])},
            {"Light Urn", new GiftVector(new GiftItem("Light Urn", 1, 0), LunacidTraits.LunacidItemTraits["Light Urn"])},
            {"Cloth Bandage", new GiftVector(new GiftItem("Cloth Bandage", 1, 0), LunacidTraits.LunacidItemTraits["Cloth Bandage"])},
            {"Dark Urn", new GiftVector(new GiftItem("Dark Urn", 1, 0), LunacidTraits.LunacidItemTraits["Dark Urn"])},
            {"Bomb", new GiftVector(new GiftItem("Bomb", 1, 0), LunacidTraits.LunacidItemTraits["Bomb"])},
            {"Poison Urn", new GiftVector(new GiftItem("Poison Urn", 1, 0), LunacidTraits.LunacidItemTraits["Poison Urn"])},
            {"Wisp Heart", new GiftVector(new GiftItem("Wisp Heart", 1, 0), LunacidTraits.LunacidItemTraits["Wisp Heart"])},
            {"Staff of Osiris", new GiftVector(new GiftItem("Staff of Osiris", 1, 0), LunacidTraits.LunacidItemTraits["Staff of Osiris"])},
            {"Moonlight Vial", new GiftVector(new GiftItem("Moonlight Vial", 1, 0), LunacidTraits.LunacidItemTraits["Moonlight Vial"])},
            {"Spectral Candle", new GiftVector(new GiftItem("Spectral Candle", 1, 0), LunacidTraits.LunacidItemTraits["Spectral Candle"])},
            {"Health Vial", new GiftVector(new GiftItem("Health Vial", 1, 0), LunacidTraits.LunacidItemTraits["Health Vial"])},
            {"Mana Vial", new GiftVector(new GiftItem("Mana Vial", 1, 0), LunacidTraits.LunacidItemTraits["Mana Vial"])},
            {"Fairy Moss", new GiftVector(new GiftItem("Fairy Moss", 1, 0), LunacidTraits.LunacidItemTraits["Fairy Moss"])},
            {"Crystal Shard", new GiftVector(new GiftItem("Crystal Shard", 1, 0), LunacidTraits.LunacidItemTraits["Crystal Shard"])},
            {"Poison Throwing Knife", new GiftVector(new GiftItem("Poison Throwing Knife", 1, 0), LunacidTraits.LunacidItemTraits["Poison Throwing Knife"])},
            {"Throwing Knife", new GiftVector(new GiftItem("Throwing Knife", 1, 0), LunacidTraits.LunacidItemTraits["Throwing Knife"])},
            {"Holy Water", new GiftVector(new GiftItem("Holy Water", 1, 0), LunacidTraits.LunacidItemTraits["Holy Water"])},
            {"Antidote", new GiftVector(new GiftItem("Antidote", 1, 0), LunacidTraits.LunacidItemTraits["Antidote"])},
            {"Survey Banner", new GiftVector(new GiftItem("Survey Banner", 1, 0), LunacidTraits.LunacidItemTraits["Survey Banner"])},
            {"Health ViaI", new GiftVector(new GiftItem("Health ViaI", 1, 0), LunacidTraits.LunacidItemTraits["Health ViaI"])},
            {"Eggnog", new GiftVector(new GiftItem("Eggnog", 1, 0), LunacidTraits.LunacidItemTraits["Eggnog"])},
            {"Coal", new GiftVector(new GiftItem("Coal", 1, 0), LunacidTraits.LunacidItemTraits["Coal"])},
            {"Pink Shrimp", new GiftVector(new GiftItem("Pink Shrimp", 1, 0), LunacidTraits.LunacidItemTraits["Pink Shrimp"])},
            {"Angel Feather", new GiftVector(new GiftItem("Angel Feather", 1, 0), LunacidTraits.LunacidItemTraits["Angel Feather"])},
            {"Ectoplasm", new GiftVector(new GiftItem("Ectoplasm", 1, 0), LunacidTraits.LunacidItemTraits["Ectoplasm"])},
            {"Snowflake Obsidian", new GiftVector(new GiftItem("Snowflake Obsidian", 1, 0), LunacidTraits.LunacidItemTraits["Snowflake Obsidian"])},
            {"Moon Petal", new GiftVector(new GiftItem("Moon Petal", 1, 0), LunacidTraits.LunacidItemTraits["Moon Petal"])},
            {"Fire Opal", new GiftVector(new GiftItem("Fire Opal", 1, 0), LunacidTraits.LunacidItemTraits["Fire Opal"])},
            {"Ashes", new GiftVector(new GiftItem("Ashes", 1, 0), LunacidTraits.LunacidItemTraits["Ashes"])},
            {"Opal", new GiftVector(new GiftItem("Opal", 1, 0), LunacidTraits.LunacidItemTraits["Opal"])},
            {"Yellow Morel", new GiftVector(new GiftItem("Yellow Morel", 1, 0), LunacidTraits.LunacidItemTraits["Yellow Morel"])},
            {"Lotus Seed Pod", new GiftVector(new GiftItem("Lotus Seed Pod", 1, 0), LunacidTraits.LunacidItemTraits["Lotus Seed Pod"])},
            {"Obsidian", new GiftVector(new GiftItem("Obsidian", 1, 0), LunacidTraits.LunacidItemTraits["Obsidian"])},
            {"Onyx", new GiftVector(new GiftItem("Onyx", 1, 0), LunacidTraits.LunacidItemTraits["Onyx"])},
            {"Ocean Bone Shard", new GiftVector(new GiftItem("Ocean Bone Shard", 1, 0), LunacidTraits.LunacidItemTraits["Ocean Bone Shard"])},
            {"Bloodweed", new GiftVector(new GiftItem("Bloodweed", 1, 0), LunacidTraits.LunacidItemTraits["Bloodweed"])},
            {"Ikurr'ilb Root", new GiftVector(new GiftItem("Ikurr'ilb Root", 1, 0), LunacidTraits.LunacidItemTraits["Ikurr'ilb Root"])},
            {"Destroying Angel Mushroom", new GiftVector(new GiftItem("Destroying Angel Mushroom", 1, 0), LunacidTraits.LunacidItemTraits["Destroying Angel Mushroom"])},
            {"Ocean Bone Shell", new GiftVector(new GiftItem("Ocean Bone Shell", 1, 0), LunacidTraits.LunacidItemTraits["Ocean Bone Shell"])},
            {"Bleed Trap", new GiftVector(new GiftItem("Bleed Trap", 1, 0), LunacidTraits.LunacidItemTraits["Bleed Trap"])},
            {"Poison Trap", new GiftVector(new GiftItem("Poison Trap", 1, 0), LunacidTraits.LunacidItemTraits["Poison Trap"])},
            {"Curse Trap", new GiftVector(new GiftItem("Curse Trap", 1, 0), LunacidTraits.LunacidItemTraits["Curse Trap"])},
            {"Slowness Trap", new GiftVector(new GiftItem("Slowness Trap", 1, 0), LunacidTraits.LunacidItemTraits["Slowness Trap"])},
            {"Blindness Trap", new GiftVector(new GiftItem("Blindness Trap", 1, 0), LunacidTraits.LunacidItemTraits["Blindness Trap"])},
            {"Mana Drain Trap", new GiftVector(new GiftItem("Mana Drain Trap", 1, 0), LunacidTraits.LunacidItemTraits["Mana Drain Trap"])},
            {"XP Drain Trap", new GiftVector(new GiftItem("XP Drain Trap", 1, 0), LunacidTraits.LunacidItemTraits["XP Drain Trap"])}
        };

        public class GiftVector
        {
            public GiftItem GiftItem;
            public GiftTrait[] GiftTraits;

            public GiftVector(GiftItem giftItem, GiftTrait[] giftTrait)
            {
                GiftItem = giftItem;
                GiftTraits = giftTrait;

            }

            public GiftVector(Gift gift)
            {
                GiftItem = new GiftItem(gift.ItemName, gift.Amount, gift.ItemValue);
                GiftTraits = gift.Traits;
            }

            public double TraitDistance(GiftVector comparedGift)
            {
                var distance = 0f;
                var handledTraits = new List<string>();
                foreach (var inTrait in this.GiftTraits)
                {
                    var wasHandled = false;
                    foreach (var outTrait in comparedGift.GiftTraits)
                    {
                        if (inTrait.Trait == outTrait.Trait)
                        {
                            distance += Vector2.Distance(
                            new Vector2((float)inTrait.Duration, (float)inTrait.Quality),
                            new Vector2((float)outTrait.Duration, (float)outTrait.Quality)
                            );
                            wasHandled = true;
                            break;
                        }
                    }
                    if (!wasHandled)
                    {
                        distance += Vector2.Distance(
                        new Vector2((float)inTrait.Duration, (float)inTrait.Quality),
                        new Vector2(0f, 0f)
                        );
                    }
                    handledTraits.Add(inTrait.Trait);
                }
                foreach (var outTrait in comparedGift.GiftTraits)
                {
                    if (handledTraits.Contains(outTrait.Trait))
                    {
                        continue;
                    }
                    distance += Vector2.Distance(
                        new Vector2((float)outTrait.Duration, (float)outTrait.Quality),
                        new Vector2(0f, 0f)
                        );
                }
                return distance;
            }
        }
    }
}