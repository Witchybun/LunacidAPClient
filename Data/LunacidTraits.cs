using System.Collections.Generic;
using Archipelago.Gifting.Net.Traits;

namespace LunacidAP.Data
{
    public static class LunacidTraits
    {
        private static readonly GiftTrait[] BloodWineTraits = new[]{
            new GiftTrait(GiftFlag.Drink, 1, 5),
            new GiftTrait(GiftFlag.Cure, 1, 5),
            new GiftTrait(GiftFlag.Heal, 1, 5),
            new GiftTrait(GiftFlag.Mana, 1, 5),
            new GiftTrait(GiftFlag.Consumable, 1, 1),
        };

        private static readonly GiftTrait[] LightUrnTraits = new[]{
            new GiftTrait(GiftFlag.Weapon, 1, 2),
            new GiftTrait(GiftFlag.Consumable, 1, 2),
            new GiftTrait(GiftFlag.Damage, 1, 3),
        };

        private static readonly GiftTrait[] DarkUrnTraits = new[]{
            new GiftTrait(GiftFlag.Weapon, 1, 2),
            new GiftTrait(GiftFlag.Consumable, 1, 2),
            new GiftTrait(GiftFlag.Damage, 1, 3),
        };

        private static readonly GiftTrait[] PoisonUrnTraits = new[]{
            new GiftTrait(GiftFlag.Weapon, 1, 2),
            new GiftTrait(GiftFlag.Consumable, 1, 2),
            new GiftTrait(GiftFlag.Damage, 1, 3),
        };

        private static readonly GiftTrait[] CureTraits = new[]{
            new GiftTrait(GiftFlag.Consumable, 1, 1),
            new GiftTrait(GiftFlag.Heal, 1, 1),
            new GiftTrait(GiftFlag.Cure, 1, 5),
        };

        private static readonly GiftTrait[] BombTraits = new[]{
            new GiftTrait(GiftFlag.Bomb, 1, 5),
            new GiftTrait(GiftFlag.Weapon, 1, 1),
            new GiftTrait(GiftFlag.Consumable, 1, 1),
        };

        private static readonly GiftTrait[] WispHeartTrait = new[]{
            new GiftTrait(GiftFlag.Energy, 1, 1),
            new GiftTrait(GiftFlag.Damage, 1, 0.8),
            new GiftTrait("Light", 1, 1),
            new GiftTrait(GiftFlag.Consumable, 1, 1),
        };

        private static readonly GiftTrait[] StaffOfOsirisTrait = new[]{
            new GiftTrait(GiftFlag.Damage, 1, 1.2),
            new GiftTrait(GiftFlag.Tool, 1, 1),
            new GiftTrait(GiftFlag.Weapon, 1, 1),
            new GiftTrait(GiftFlag.Consumable, 1, 1),
        };

        private static readonly GiftTrait[] MoonlightVialTrait = new[]{
            new GiftTrait(GiftFlag.Drink, 1, 1),
            new GiftTrait(GiftFlag.Mana, 1, 5),
            new GiftTrait(GiftFlag.Consumable, 1, 1),
        };

        private static readonly GiftTrait[] SpectralCandleTrait = new[]{
            new GiftTrait(GiftFlag.Consumable, 1, 1),
            new GiftTrait(GiftFlag.Energy, 1, 1),
            new GiftTrait("Light", 1, 1),
        };

        private static readonly GiftTrait[] HealthVialTrait = new[]{
            new GiftTrait(GiftFlag.Heal, 1, 5),
            new GiftTrait(GiftFlag.Consumable, 1, 1),
        };

        private static readonly GiftTrait[] ManaVialTrait = new[]{
            new GiftTrait(GiftFlag.Mana, 1, 5),
            new GiftTrait(GiftFlag.Consumable, 1, 1),
        };

        private static readonly GiftTrait[] FairyMossTrait = new[]{
            new GiftTrait(GiftFlag.Life, 1, 1),
            new GiftTrait(GiftFlag.Consumable, 1, 1),
            new GiftTrait(GiftFlag.Grass, 1, 1),
        };

        private static readonly GiftTrait[] ThrowingKnifeTrait = new[]{
            new GiftTrait(GiftFlag.Damage, 1, 1),
            new GiftTrait(GiftFlag.Weapon, 1, 1),
            new GiftTrait(GiftFlag.Consumable, 1, 1),
        };

        private static readonly GiftTrait[] SurveyBannerTrait = new[]{
            new GiftTrait(GiftFlag.Consumable, 1, 1),
            new GiftTrait(GiftFlag.Key, 1, 1),
            new GiftTrait(GiftFlag.Tool, 1, 1),
        };

        private static readonly GiftTrait[] CrystalShardTrait = new[]{
            new GiftTrait(GiftFlag.Consumable, 1, 1),
            new GiftTrait(GiftFlag.Speed, 1, 5),
            new GiftTrait("Teleport", 1, 1),
        };

        private static readonly GiftTrait[] TreeGirlBathWater = new[]{
            new GiftTrait(GiftFlag.Trap, 1, 1),
            new GiftTrait(GiftFlag.Damage, 1, 1),
            new GiftTrait(GiftFlag.Drink, 1, 1),
        };

        private static readonly GiftTrait[] EggnogTraits = new[]{
            new GiftTrait(GiftFlag.Trap, 1, 1),
            new GiftTrait(GiftFlag.Drink, 1, 1),
            new GiftTrait(GiftFlag.Mana, 1, 1),
        };

        private static readonly GiftTrait[] CoalTraits = new[]{
            new GiftTrait(GiftFlag.Energy, 1, 1),
            new GiftTrait(GiftFlag.Heat, 1, 1),
            new GiftTrait(GiftFlag.Ore, 1, 1),
        };

        private static readonly GiftTrait[] ShrimpTraits = new[]{
            new GiftTrait(GiftFlag.Food, 1, 1),
            new GiftTrait(GiftFlag.Heal, 1, 1),
            new GiftTrait(GiftFlag.Fish, 1, 1),
        };

        private static readonly GiftTrait[] AngelFeatherTraits = new[]{
            new GiftTrait(GiftFlag.Heal, 1, 99),
            new GiftTrait(GiftFlag.Damage, 1, 99),
            new GiftTrait(GiftFlag.Mana, 1, 99),
        };

        private static readonly GiftTrait[] EctoplasmTraits = new[]{
            new GiftTrait(GiftFlag.Material, 1, 1),
            new GiftTrait(GiftFlag.Energy, 1, 1),
            new GiftTrait(GiftFlag.Monster, 1, 1),
        };

        private static readonly GiftTrait[] SnowflakeObsidianTraits = new[]{
            new GiftTrait(GiftFlag.Material, 1, 1),
            new GiftTrait(GiftFlag.Cold, 1, 1),
            new GiftTrait(GiftFlag.Ore, 1, 1),
        };

        private static readonly GiftTrait[] MoonPetalTraits = new[]{
            new GiftTrait(GiftFlag.Material, 1, 1),
            new GiftTrait(GiftFlag.Grass, 1, 1),
            new GiftTrait(GiftFlag.Vegetable, 1, 1),
        };

        private static readonly GiftTrait[] FireOpalTraits = new[]{
            new GiftTrait(GiftFlag.Material, 1, 1),
            new GiftTrait(GiftFlag.Heat, 1, 1),
            new GiftTrait(GiftFlag.Ore, 1, 1),
        };

        private static readonly GiftTrait[] AshesTraits = new[]{
            new GiftTrait(GiftFlag.Material, 1, 1),
            new GiftTrait(GiftFlag.Wood, 1, 1),
            new GiftTrait(GiftFlag.Heat, 1, 1),
            new GiftTrait(GiftFlag.Monster, 1, 1),
        };

        private static readonly GiftTrait[] OpalTraits = new[]{
            new GiftTrait(GiftFlag.Material, 1, 1),
            new GiftTrait(GiftFlag.Cure, 1, 1),
            new GiftTrait(GiftFlag.Ore, 1, 1),
        };

        private static readonly GiftTrait[] YellowMorelTraits = new[]{
            new GiftTrait(GiftFlag.Material, 1, 1),
            new GiftTrait(GiftFlag.Vegetable, 1, 1),
            new GiftTrait(GiftFlag.Wood, 1, 1),
        };

        private static readonly GiftTrait[] LotusSeedPotTraits = new[]{
            new GiftTrait(GiftFlag.Material, 1, 1),
            new GiftTrait(GiftFlag.Vegetable, 1, 1),
            new GiftTrait(GiftFlag.Food, 1, 1),
        };

        private static readonly GiftTrait[] ObsidianTraits = new[]{
            new GiftTrait(GiftFlag.Material, 1, 1),
            new GiftTrait(GiftFlag.Heat, 1, 1),
            new GiftTrait(GiftFlag.Stone, 1, 1),
        };

        private static readonly GiftTrait[] OnyxTraits = new[]{
            new GiftTrait(GiftFlag.Material, 1, 1),
            new GiftTrait(GiftFlag.Buff, 1, 1),
            new GiftTrait(GiftFlag.Stone, 1, 1),
        };

        private static readonly GiftTrait[] OceanBoneShardTraits = new[]{
            new GiftTrait(GiftFlag.Material, 1, 1),
            new GiftTrait(GiftFlag.Monster, 1, 1),
            new GiftTrait(GiftFlag.Stone, 1, 1),
        };

        private static readonly GiftTrait[] BloodweedTraits = new[]{
            new GiftTrait(GiftFlag.Material, 1, 1),
            new GiftTrait(GiftFlag.Grass, 1, 1),
            new GiftTrait(GiftFlag.Mana, 1, 1),
        };

        private static readonly GiftTrait[] IkurrilbRootTraits = new[]{
            new GiftTrait(GiftFlag.Material, 1, 1),
            new GiftTrait(GiftFlag.Wood, 1, 1),
            new GiftTrait(GiftFlag.Monster, 1, 1),
        };

        private static readonly GiftTrait[] AngelMushroomTraits = new[]{
            new GiftTrait(GiftFlag.Material, 1, 1),
            new GiftTrait(GiftFlag.Damage, 1, 1),
            new GiftTrait(GiftFlag.Food, 1, 1),
        };

        private static readonly GiftTrait[] OceanBoneShellTraits = new[]{
            new GiftTrait(GiftFlag.Material, 1, 1),
            new GiftTrait(GiftFlag.Monster, 1, 1),
            new GiftTrait(GiftFlag.Animal, 1, 1),
        };

        private static readonly GiftTrait[] BleedTrapTraits = new[]{
            new GiftTrait(GiftFlag.Trap, 1, 1),
            new GiftTrait(GiftFlag.Damage, 1, 1),
            new GiftTrait(GiftFlag.Life, 1, 1),
        };

        private static readonly GiftTrait[] PoisonTrapTraits = new[]{
            new GiftTrait(GiftFlag.Trap, 1, 1),
            new GiftTrait(GiftFlag.Damage, 1, 1),
            new GiftTrait(GiftFlag.Cure, 1, 1),
        };

        private static readonly GiftTrait[] CurseTrapTraits = new[]{
            new GiftTrait(GiftFlag.Trap, 1, 1),
            new GiftTrait(GiftFlag.Buff, 1, 1),
            new GiftTrait(GiftFlag.Stone, 1, 1),
        };

        private static readonly GiftTrait[] SlownessTrapTraits = new[]{
            new GiftTrait(GiftFlag.Trap, 1, 1),
            new GiftTrait(GiftFlag.Speed, 1, 1),
            new GiftTrait(GiftFlag.Buff, 1, 1),
        };

        private static readonly GiftTrait[] BlindnenssTrapTraits = new[]{
            new GiftTrait(GiftFlag.Trap, 1, 1),
            new GiftTrait(GiftFlag.Buff, 1, 1),
            new GiftTrait(GiftFlag.Vegetable, 1, 1),
        };

        private static readonly GiftTrait[] ManaDrainTrapTraits = new[]{
            new GiftTrait(GiftFlag.Trap, 1, 1),
            new GiftTrait(GiftFlag.Mana, 1, 1),
            new GiftTrait(GiftFlag.Damage, 1, 1),
        };

        private static readonly GiftTrait[] XPrainTrapTraits = new[]{
            new GiftTrait(GiftFlag.Trap, 1, 1),
            new GiftTrait(GiftFlag.Key, 1, 1),
            new GiftTrait(GiftFlag.Damage, 1, 1),
        };

        public static Dictionary<string, GiftTrait[]> LunacidItemTraits = new()
        {
            {"Blood Wine", BloodWineTraits},
            {"Light Urn", LightUrnTraits}, 
            {"Cloth Bandage", CureTraits}, 
            {"Dark Urn", DarkUrnTraits}, 
            {"Bomb", BombTraits}, 
            {"Poison Urn", PoisonUrnTraits},
            {"Wisp Heart", WispHeartTrait}, 
            {"Staff of Osiris", StaffOfOsirisTrait},
            {"Moonlight Vial", MoonlightVialTrait}, 
            {"Spectral Candle", SpectralCandleTrait}, 
            {"Health Vial", HealthVialTrait}, 
            {"Mana Vial", ManaVialTrait}, 
            {"Fairy Moss", FairyMossTrait}, 
            {"Crystal Shard", CrystalShardTrait}, 
            {"Poison Throwing Knife", ThrowingKnifeTrait},
            {"Throwing Knife", ThrowingKnifeTrait}, 
            {"Holy Water", CureTraits}, 
            {"Antidote", CureTraits}, 
            {"Survey Banner", SurveyBannerTrait}, 
            {"Health ViaI", TreeGirlBathWater}, 
            {"Eggnog", EggnogTraits},
            {"Coal", CoalTraits}, 
            {"Pink Shrimp", ShrimpTraits}, 
            {"Angel Feather", AngelFeatherTraits}, 
            {"Ectoplasm", EctoplasmTraits}, 
            {"Snowflake Obsidian", SnowflakeObsidianTraits}, 
            {"Moon Petal", MoonPetalTraits}, 
            {"Fire Opal", FireOpalTraits}, 
            {"Ashes", AshesTraits},
            {"Opal", OpalTraits}, 
            {"Yellow Morel", YellowMorelTraits}, 
            {"Lotus Seed Pod", LotusSeedPotTraits}, 
            {"Obsidian",ObsidianTraits}, 
            {"Onyx", OnyxTraits}, 
            {"Ocean Bone Shard", OceanBoneShardTraits}, 
            {"Bloodweed", BloodweedTraits}, 
            {"Ikurr'ilb Root", IkurrilbRootTraits},
            {"Destroying Angel Mushroom", AngelMushroomTraits}, 
            {"Ocean Bone Shell", OceanBoneShellTraits},
            {"Bleed Trap", BleedTrapTraits},
            {"Poison Trap", PoisonTrapTraits},
            {"Curse Trap", CurseTrapTraits},
            {"Slowness Trap", SlownessTrapTraits},
            {"Blindness Trap", BlindnenssTrapTraits},
            {"Mana Drain Trap", ManaDrainTrapTraits},
            {"XP Drain Trap", XPrainTrapTraits}
        };
    }
}