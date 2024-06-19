using System.Collections.Generic;
using Archipelago.Gifting.Net.Traits;

namespace LunacidAP.Data
{
    public static class LunacidTraits
    {
        private static readonly List<GiftTrait> BloodWineTraits = new(){
            new GiftTrait(GiftFlag.Drink, 1, 5),
            new GiftTrait(GiftFlag.Cure, 1, 5),
            new GiftTrait(GiftFlag.Heal, 1, 5),
            new GiftTrait(GiftFlag.Mana, 1, 5),
            new GiftTrait(GiftFlag.Consumable, 1, 1),
        };

        private static readonly List<GiftTrait> LightUrnTraits = new(){
            new GiftTrait(GiftFlag.Weapon, 1, 2),
            new GiftTrait(GiftFlag.Consumable, 1, 2),
            new GiftTrait(GiftFlag.Damage, 1, 3),
        };

        private static readonly List<GiftTrait> DarkUrnTraits = new(){
            new GiftTrait(GiftFlag.Weapon, 1, 2),
            new GiftTrait(GiftFlag.Consumable, 1, 2),
            new GiftTrait(GiftFlag.Damage, 1, 3),
        };

        private static readonly List<GiftTrait> PoisonUrnTraits = new(){
            new GiftTrait(GiftFlag.Weapon, 1, 2),
            new GiftTrait(GiftFlag.Consumable, 1, 2),
            new GiftTrait(GiftFlag.Damage, 1, 3),
        };

        private static readonly List<GiftTrait> CureTraits = new(){
            new GiftTrait(GiftFlag.Consumable, 1, 1),
            new GiftTrait(GiftFlag.Heal, 1, 1),
            new GiftTrait(GiftFlag.Cure, 1, 5),
        };

        private static readonly List<GiftTrait> BombTraits = new(){
            new GiftTrait(GiftFlag.Bomb, 1, 5),
            new GiftTrait(GiftFlag.Weapon, 1, 1),
            new GiftTrait(GiftFlag.Consumable, 1, 1),
        };

        private static readonly List<GiftTrait> WispHeartTrait = new(){
            new GiftTrait(GiftFlag.Energy, 1, 1),
            new GiftTrait(GiftFlag.Damage, 1, 0.8),
            new GiftTrait("Light", 1, 1),
            new GiftTrait(GiftFlag.Consumable, 1, 1),
        };

        private static readonly List<GiftTrait> StaffOfOsirisTrait = new(){
            new GiftTrait(GiftFlag.Damage, 1, 1.2),
            new GiftTrait(GiftFlag.Tool, 1, 1),
            new GiftTrait(GiftFlag.Weapon, 1, 1),
            new GiftTrait(GiftFlag.Consumable, 1, 1),
        };

        private static readonly List<GiftTrait> MoonlightVialTrait = new(){
            new GiftTrait(GiftFlag.Drink, 1, 1),
            new GiftTrait(GiftFlag.Mana, 1, 5),
            new GiftTrait(GiftFlag.Consumable, 1, 1),
        };

        private static readonly List<GiftTrait> SpectralCandleTrait = new(){
            new GiftTrait(GiftFlag.Consumable, 1, 1),
            new GiftTrait(GiftFlag.Energy, 1, 1),
        };

        private static readonly List<GiftTrait> HealthVialTrait = new(){
            new GiftTrait(GiftFlag.Heal, 1, 5),
            new GiftTrait(GiftFlag.Consumable, 1, 1),
        };

        private static readonly List<GiftTrait> ManaVialTrait = new(){
            new GiftTrait(GiftFlag.Mana, 1, 5),
            new GiftTrait(GiftFlag.Consumable, 1, 1),
        };

        private static readonly List<GiftTrait> FairyMossTrait = new(){
            new GiftTrait(GiftFlag.Life, 1, 1),
            new GiftTrait(GiftFlag.Consumable, 1, 1),
            new GiftTrait(GiftFlag.Grass, 1, 1),
        };

        private static readonly List<GiftTrait> ThrowingKnifeTrait = new(){
            new GiftTrait(GiftFlag.Damage, 1, 1),
            new GiftTrait(GiftFlag.Weapon, 1, 1),
            new GiftTrait(GiftFlag.Consumable, 1, 1),
        };

        private static readonly List<GiftTrait> SurveyBannerTrait = new(){
            new GiftTrait(GiftFlag.Consumable, 1, 1),
            new GiftTrait(GiftFlag.Key, 1, 1),
            new GiftTrait(GiftFlag.Tool, 1, 1),
        };

        private static readonly List<GiftTrait> CrystalShardTrait = new(){
            new GiftTrait(GiftFlag.Consumable, 1, 1),
            new GiftTrait(GiftFlag.Speed, 1, 5),
            new GiftTrait("Teleport", 1, 1),
        };

        private static readonly List<GiftTrait> TreeGirlBathWater = new(){
            new GiftTrait(GiftFlag.Trap, 1, 1),
            new GiftTrait(GiftFlag.Damage, 1, 1),
            new GiftTrait(GiftFlag.Drink, 1, 1),
        };

        private static readonly List<GiftTrait> EggnogTraits = new(){
            new GiftTrait(GiftFlag.Trap, 1, 1),
            new GiftTrait(GiftFlag.Drink, 1, 1),
            new GiftTrait(GiftFlag.Mana, 1, 1),
        };

        private static readonly List<GiftTrait> CoalTraits = new(){
            new GiftTrait(GiftFlag.Trap, 1, 1),
            new GiftTrait(GiftFlag.Damage, 1, 1),
            new GiftTrait(GiftFlag.Heat, 1, 1),
            new GiftTrait(GiftFlag.Ore, 1, 1),
        };

        private static readonly List<GiftTrait> ShrimpTraits = new(){
            new GiftTrait(GiftFlag.Food, 1, 1),
            new GiftTrait(GiftFlag.Heal, 1, 1),
        };

        private static readonly List<GiftTrait> AngelFeatherTraits = new(){
            new GiftTrait(GiftFlag.Heal, 1, 999),
            new GiftTrait(GiftFlag.Damage, 1, 999),
            new GiftTrait(GiftFlag.Mana, 1, 999),
        };

        private static readonly List<GiftTrait> GenericMaterial = new(){
            new GiftTrait(GiftFlag.Material, 1, 1),
        };

        private static readonly List<GiftTrait> GenericTrap = new(){
            new GiftTrait(GiftFlag.Trap, 1, 1),
        };

        public static Dictionary<string, List<GiftTrait>> LunacidItemTraits = new()
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
            {"Ectoplasm", GenericMaterial}, 
            {"Snowflake Obsidian", GenericMaterial}, 
            {"Moon Petal", GenericMaterial}, 
            {"Fire Opal", GenericMaterial}, 
            {"Ashes", GenericMaterial},
            {"Opal", GenericMaterial}, 
            {"Yellow Morel", GenericMaterial}, 
            {"Lotus Seed Pod", GenericMaterial}, 
            {"Obsidian", GenericMaterial}, 
            {"Onyx", GenericMaterial}, 
            {"Ocean Bone Shard", GenericMaterial}, 
            {"Bloodweed", GenericMaterial}, 
            {"Ikurr'ilb Root", GenericMaterial},
            {"Destroying Angel Mushroom", GenericMaterial}, 
            {"Ocean Bone Shell", GenericMaterial},
            {"Bleed Trap", GenericTrap},
            {"Poison Trap", GenericTrap},
            {"Curse Trap", GenericTrap},
            {"Slowness Trap", GenericTrap},
            {"Blindness Trap", GenericTrap},
            {"Mana Drain Trap", GenericTrap},
            {"XP Drain Trap", GenericTrap}
        };
    }
}