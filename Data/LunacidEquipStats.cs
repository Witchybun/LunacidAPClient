using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace LunacidAP.Data;

public static class LunacidEquipStats
{
    public class WeaponData
    {
        public readonly float Damage;
        public readonly float Speed;
        public readonly float Guard;
        public readonly float Backstep;
        public readonly float Thrust;

        public WeaponData(float damage, float speed, float guard, float backstep, float thrust)
        {
            Damage = damage;
            Speed = speed;
            Guard = guard;
            Backstep = backstep;
            Thrust = thrust;
        }
    }

    public class SpellData
    {
        public readonly float Damage;
        public readonly float CastTime;
        public readonly float MinCastTime;
        public readonly float Cost;

        public SpellData(float damage, float castTime, float minCastTime, float cost)
        {
            Damage = damage;
            CastTime = castTime;
            MinCastTime = minCastTime;
            Cost = cost;
        }
    }
    
    public static readonly Dictionary<string, WeaponData> UsableWeaponData = new();
    public static readonly Dictionary<string, SpellData> UsableMagicData = new();
    public static readonly Dictionary<string, SpellData> UsableAttackMagicData = new();
    public static readonly Dictionary<string, SpellData> UsableSupportMagicData = new();

    public static void InitializeEquipStatLookups()
    {
        var weapons = Resources.LoadAll<GameObject>("WEPS/").ToList();
        var magic = Resources.LoadAll<GameObject>("MAGIC/").ToList();
        magic = magic.Where(x => !x.name.Contains("_")).ToList();  // Get rid of not real spells.
        foreach (var weapon in weapons)
        {
            var weaponScr = weapon.GetComponent<Weapon_scr>();
            var weaponData = new WeaponData(weaponScr.WEP_DAMAGE, weaponScr.WEP_COOLDOWN, weaponScr.WEP_GUARD, weaponScr.WEP_BACKSTEP, weaponScr.WEP_WEIGHT);
            UsableWeaponData[weapon.name] = weaponData;
        }
        foreach (var spell in magic)
        {
            if (!spell.TryGetComponent<Magic_scr>(out var magicScr))
            {
                Plugin.LOG.LogError($"There is no component for {spell.name}");
                continue;
            }
            var spellData = new SpellData(magicScr.MAG_DAMAGE, magicScr.MAG_CHARGE_TIME, magicScr.MIN_CHARGE_TIME,
                magicScr.MAG_COST);
            UsableMagicData[spell.name] = spellData;
            if (spellData.Damage > 0)
            {
                UsableAttackMagicData[spell.name] = spellData;
            }
            else
            {
                UsableSupportMagicData[spell.name] = spellData;
            }
        }
    }
    
    public class EquipAlternatives
    {
        public readonly string Syntax;
        public readonly Dictionary<string, string> ElementReplacement;

        public EquipAlternatives(string syntax, Dictionary<string, string> replacement)
        {
            Syntax = syntax;
            ElementReplacement = replacement;
        }
    }

    public static readonly Dictionary<string, EquipAlternatives> EquipmentLookup = new()
    {
        { "AXE OF HARMING", new EquipAlternatives("AXE OF %", 
            new()
            {
                { "Normal", "HARMING" },
                {"Fire", "BURNING"},
                {"Ice", "FREEZING"},
                {"Poison", "TOXICITY"},
                {"Light", "BLINDING"},
                {"Dark", "PARANOIA"},
                {"Dark and Light", "ISOLATION"},
                {"Normal and Fire", "HEATSTROKE"},
                {"Ice and Poison", "FROSTBITE"},
                {"Dark and Fire", "INCINERATOR"}
            }) },
        { "BATTLE AXE", new EquipAlternatives("% AXE", 
            new()
            {
                { "Normal", "BATTLE" },
                {"Fire", "FIREFIGHTER"},
                {"Ice", "WINTER WAR"},
                {"Poison", "CHEMICAL WAR"},
                {"Light", "CRUSADE"},
                {"Dark", "RAID"},
                {"Dark and Light", "CALAMITY"},
                {"Normal and Fire", "BOMBARDMENT"},
                {"Ice and Poison", "ETERNAL WAR"},
                {"Dark and Fire", "WAR HORROR"}
            }) },
        { "BLESSED WIND", new EquipAlternatives("BLESSED %", 
            new()
            {
                { "Normal", "WIND" },
                {"Fire", "FLAME"},
                {"Ice", "ICE"},
                {"Poison", "POISON"},
                {"Light", "LIGHT"},
                {"Dark", "DARK"},
                {"Dark and Light", "CHAOS"},
                {"Normal and Fire", "EXPLOSION"},
                {"Ice and Poison", "ETERNITY"},
                {"Dark and Fire", "HUMANITY"}
            }) }, 
        "BROKEN HILT", 
        "BROKEN LANCE",
        "CORRUPTED DAGGER", 
        "DARK RAPIER", 
        "ELFEN BOW", 
        "ELFEN SWORD", 
        "FISHING SPEAR",
        "HALBERD", 
        "IRON CLAW",
        "MOONLIGHT", 
        "OBSIDIAN SEAL", 
        "REPLICA SWORD", 
        "RITUAL DAGGER", 
        "RUSTED SWORD", 
        "SERPENT FANG", 
        "SHADOW BLADE",
        "STEEL SPEAR", 
        "STONE CLUB", 
        "TORCH", 
        "TWISTED STAFF", 
        "VAMPIRE HUNTER SWORD", 
        "WAND OF POWER", 
        "WOLFRAM GREATSWORD",
        "WOODEN SHIELD", 
        "CROSSBOW", 
        "STEEL NEEDLE", 
        "HAMMER OF CRUELTY", 
        "LUCID BLADE", 
        "JOTUNN SLAYER", 
        "RAPIER", 
        "PRIVATEER MUSKET",
        "BRITTLE ARMING SWORD", 
        "GOLDEN KHOPESH", 
        "GOLDEN SICKLE", 
        "ICE SICKLE", 
        "JAILOR'S CANDLE", 
        "OBSIDIAN CURSEBRAND", 
        "OBSIDIAN POISONGUARD",
        "SKELETON AXE", 
        "SUCSARIAN DAGGER", 
        "SUCSARIAN SPEAR", 
        "CURSED BLADE", 
        "LYRIAN LONGSWORD", 
        "RUSTED SWORD", 
        "MARAUDER BLACK FLAIL", 
        "DOUBLE CROSSBOW",
        "FIRE SWORD", 
        "STEEL LANCE", 
        "ELFEN LONGSWORD", 
        "STEEL CLAW", 
        "STEEL CLUB", 
        "LYRIAN GREATSWORD", 
        "SAINT ISHII", 
        "SILVER RAPIER", 
        "HERITAGE SWORD",
        "DARK GREATSWORD", 
        "SHINING BLADE", 
        "POISON CLAW", 
        "IRON CLUB", 
        "IRON TORCH",
        "GHOST SWORD", 
        "CAVALRY SABER"
        "BLOOD DRAIN", 
        "BLOOD STRIKE", 
        "BLUE FLAME ARC", 
        "EARTH STRIKE",
        "EARTH THORN", 
        "FIRE WORM", 
        "FLAME FLARE", 
        "FLAME SPEAR", 
        "ICE SPEAR", 
        "ICE TEAR",
        "IGNIS CALOR", 
        "LAVA CHASM", 
        "LIGHTNING", 
        "MOON BEAM", 
        "SLIME ORB",
        "WIND SLICER", 
        "TORNADO", 
        "DARK SKULL",
        "JINGLE BELLS", 
        "POISON MIST", 
        "PUMPKIN POP"
    };
}