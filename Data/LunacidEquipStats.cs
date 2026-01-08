using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace LunacidAP.Data;

public static class LunacidEquipStats
{
    [Serializable]
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

    [Serializable]
    public class SpellData
    {
        public readonly float Damage;
        public readonly float CastTime;
        public readonly float MinCastTime;
        public float Cost;

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
    public static readonly Dictionary<string, Color> SpellColors = new();

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
        GatherSpellColors();
    }

    private static void GatherSpellColors()
    {
        var earthStrike = Resources.Load<GameObject>("MAGIC/EARTH STRIKE");
        SpellColors["Normal"] = earthStrike.GetComponent<Magic_scr>().MAG_COLOR;
        var flameSpear = Resources.Load<GameObject>("MAGIC/FLAME SPEAR");
        SpellColors["Fire"] = flameSpear.GetComponent<Magic_scr>().MAG_COLOR;
        var iceSpear = Resources.Load<GameObject>("MAGIC/ICE SPEAR");
        SpellColors["Ice"] = iceSpear.GetComponent<Magic_scr>().MAG_COLOR;
        var lightning = Resources.Load<GameObject>("MAGIC/LIGHTNING");
        SpellColors["Light"] = lightning.GetComponent<Magic_scr>().MAG_COLOR;
        var darkSkull =  Resources.Load<GameObject>("MAGIC/DARK SKULL");
        SpellColors["Dark"] = darkSkull.GetComponent<Magic_scr>().MAG_COLOR;
        
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
            new Dictionary<string, string>
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
            new Dictionary<string, string>
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
                {"Dark and Fire", "WAR CRIME"}
            }) },
        { "BLESSED WIND", new EquipAlternatives("BLESSED %", 
            new Dictionary<string, string>
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
        { "BROKEN HILT", new EquipAlternatives( "% HILT", 
            new Dictionary<string, string>
            {
                { "Normal", "BROKEN" },
                {"Fire", "UNLIT"},
                {"Ice", "MELTED"},
                {"Poison", "CLEANSED"},
                {"Light", "DIM"},
                {"Dark", "EXPOSED"},
                {"Dark and Light", "ORDERLY"},
                {"Normal and Fire", "EXPIRED"},
                {"Ice and Poison", "FINITE"},
                {"Dark and Fire", "MANUFACTURED"}
            }) },
        { "BROKEN LANCE", new EquipAlternatives("% LANCE", 
            new Dictionary<string, string>
            {
                { "Normal", "BROKEN" },
                {"Fire", "UNLIT"},
                {"Ice", "MELTED"},
                {"Poison", "CLEANSED"},
                {"Light", "DIM"},
                {"Dark", "EXPOSED"},
                {"Dark and Light", "ORDERLY"},
                {"Normal and Fire", "EXPIRED"},
                {"Ice and Poison", "FINITE"},
                {"Dark and Fire", "MANUFACTURED"}
            }) },
        { "CORRUPTED DAGGER", new EquipAlternatives("% DAGGER", 
            new Dictionary<string, string>
            {
                { "Normal", "WARPED" },
                {"Fire", "SMOKY"},
                {"Ice", "BLACK ICE"},
                {"Poison", "DEADLY"},
                {"Light", "ZEALOT"},
                {"Dark", "CORRUPTED"},
                {"Dark and Light", "DOMINATING"},
                {"Normal and Fire", "NUCLEAR"},
                {"Ice and Poison", "IMPRISONED"},
                {"Dark and Fire", "DEMONIC"}
            }) },
        { "DARK RAPIER", new EquipAlternatives("% RAPIER", 
            new Dictionary<string, string>
            {
                { "Normal", "STEEL" },
                {"Fire", "FIRE"},
                {"Ice", "ICE"},
                {"Poison", "POISON"},
                {"Light", "LIGHT"},
                {"Dark", "DARK"},
                {"Dark and Light", "CHAOS"},
                {"Normal and Fire", "EXPLOSIVE"},
                {"Ice and Poison", "ETERNAL"},
                {"Dark and Fire", "SOUL"}
            }) },
        { "ELFEN BOW", new EquipAlternatives("% BOW", 
            new Dictionary<string, string>
            {
                { "Normal", "ELFEN" },
                {"Fire", "DEMONIC"},
                {"Ice", "TROLL"},
                {"Poison", "GOBLIN"},
                {"Light", "CELESTIAL"},
                {"Dark", "SHADOWFOLK"},
                {"Dark and Light", "RABBITFOLK"},
                {"Normal and Fire", "GOLEM"},
                {"Ice and Poison", "SUCSARIAN"},
                {"Dark and Fire", "HOMUNCULUS"}
            }) },
        { "ELFEN SWORD", new EquipAlternatives("% SWORD", 
            new Dictionary<string, string>
            {
                { "Normal", "ELFEN" },
                {"Fire", "DEMONIC"},
                {"Ice", "TROLL"},
                {"Poison", "GOBLIN"},
                {"Light", "CELESTIAL"},
                {"Dark", "SHADOWFOLK"},
                {"Dark and Light", "RABBITFOLK"},
                {"Normal and Fire", "GOLEM"},
                {"Ice and Poison", "SUCSARIAN"},
                {"Dark and Fire", "HOMUNCULUS"}
            }) },
        { "FISHING SPEAR", new EquipAlternatives("% SPEAR", 
            new Dictionary<string, string>
            {
                { "Normal", "FISHING" },
                {"Fire", "LARGE GAME"},
                {"Ice", "SURVIVAL"},
                {"Poison", "TRAPPING"},
                {"Light", "SCOUTING"},
                {"Dark", "STALKING"},
                {"Dark and Light", "FORAGING"},
                {"Normal and Fire", "HUNTING"},
                {"Ice and Poison", "CRABBING"},
                {"Dark and Fire", "FARMING"}
            }) },
        { "HALBERD", new EquipAlternatives("%HALBERD", 
            new Dictionary<string, string>
            {
                { "Normal", "" },
                {"Fire", "FIRE "},
                {"Ice", "ICE "},
                {"Poison", "POISON "},
                {"Light", "LIGHT "},
                {"Dark", "DARK "},
                {"Dark and Light", "CHAOS "},
                {"Normal and Fire", "MECHANICAL "},
                {"Ice and Poison", "ETERNAL "},
                {"Dark and Fire", "SOUL "}
            }) },
        { "IRON CLAW", new EquipAlternatives("% CLAW", 
            new Dictionary<string, string>
            {
                { "Normal", "IRON" },
                {"Fire", "SPARK"},
                {"Ice", "SNOW"},
                {"Poison", "DIRTY"},
                {"Light", "SHINY"},
                {"Dark", "DIM"},
                {"Dark and Light", "WARPED"},
                {"Normal and Fire", "BULK"},
                {"Ice and Poison", "STILL"},
                {"Dark and Fire", "AWARE"}
            }) },
        { "MOONLIGHT", new EquipAlternatives("%LIGHT", 
            new Dictionary<string, string>
            {
                { "Normal", "EARTH" },
                {"Fire", "MARS"},
                {"Ice", "PLUTO"},
                {"Poison", "MERCURY"},
                {"Light", "MOON"},
                {"Dark", "VOID"},
                {"Dark and Light", "STAR"},
                {"Normal and Fire", "SATTE"},
                {"Ice and Poison", "URANUS"},
                {"Dark and Fire", "VENUS"}
            }) },
        { "OBSIDIAN SEAL", new EquipAlternatives("% SEAL", 
            new Dictionary<string, string>
            {
                { "Normal", "IRON" },
                {"Fire", "PERIDOT"},
                {"Ice", "OPAL"},
                {"Poison", "CINNABAR"},
                {"Light", "QUARTZ"},
                {"Dark", "OBSIDIAN"},
                {"Dark and Light", "ONYX"},
                {"Normal and Fire", "TITANIUM"},
                {"Ice and Poison", "MERCURY"},
                {"Dark and Fire", "POTASSIUM"}
            }) },
        { "REPLICA SWORD", new EquipAlternatives("% SWORD", 
            new Dictionary<string, string>
            {
                { "Normal", "REPLICA" },
                {"Fire", "FORGED"},
                {"Ice", "MOLDED"},
                {"Poison", "STOLEN"},
                {"Light", "DUPLICATE"},
                {"Dark", "MIMIC"},
                {"Dark and Light", "MIRRORED"},
                {"Normal and Fire", "MANUFACTURED"},
                {"Ice and Poison", "CYCLICAL"},
                {"Dark and Fire", "SPAWNED"}
            }) },
        { "RITUAL DAGGER", new EquipAlternatives("% DAGGER", 
            new Dictionary<string, string>
            {
                { "Normal", "PLAN" },
                {"Fire", "DUEL"},
                {"Ice", "BOND"},
                {"Poison", "RITUAL"},
                {"Light", "CEREMONY"},
                {"Dark", "PACT"},
                {"Dark and Light", "DUALITY"},
                {"Normal and Fire", "WORK"},
                {"Ice and Poison", "CURSE"},
                {"Dark and Fire", "SACRIFICIAL"}
            })},
        { "RUSTED SWORD", new EquipAlternatives("% SWORD", 
            new Dictionary<string, string>
            {
                { "Normal", "RUSTED" },
                {"Fire", "CHARRED"},
                {"Ice", "FROZEN"},
                {"Poison", "ACIDIC"},
                {"Light", "BLEACHED"},
                {"Dark", "CORRUPTED"},
                {"Dark and Light", "WARPED"},
                {"Normal and Fire", "BULK"},
                {"Ice and Poison", "CURSED"},
                {"Dark and Fire", "SINNER'S"}
            }) },
        { "SERPENT FANG" , new EquipAlternatives("% FANG", 
            new Dictionary<string, string>
            {
                { "Normal", "WOLF" },
                {"Fire", "WOLVERINE"},
                {"Ice", "SHARK"},
                {"Poison", "COBRA"},
                {"Light", "LION"},
                {"Dark", "SERPENT"},
                {"Dark and Light", "LEOPARD"},
                {"Normal and Fire", "SPIDER"},
                {"Ice and Poison", "PAYARA"},
                {"Dark and Fire", "MONKEY"}
            })},
        { "SHADOW BLADE", new EquipAlternatives("% BLADE", 
            new Dictionary<string, string>
            {
                { "Normal", "ILLUSION" },
                {"Fire", "SMOKE"},
                {"Ice", "BLIZZARD"},
                {"Poison", "MIASMA"},
                {"Light", "BLINDING"},
                {"Dark", "SHADOW"},
                {"Dark and Light", "CONFUSION"},
                {"Normal and Fire", "SHORT-TERM"},
                {"Ice and Poison", "EYELESS"},
                {"Dark and Fire", "SHORT-SIGHTED"}
            }) },
        { "STEEL SPEAR", new EquipAlternatives("% SPEAR", 
            new Dictionary<string, string>
            {
                { "Normal", "STEEL" },
                {"Fire", "FIRE"},
                {"Ice", "ICE"},
                {"Poison", "POISON"},
                {"Light", "LIGHT"},
                {"Dark", "DARK"},
                {"Dark and Light", "CHAOS"},
                {"Normal and Fire", "MECHANICAL"},
                {"Ice and Poison", "ETERNAL"},
                {"Dark and Fire", "SOUL"}
            }) },
        { "STONE CLUB", new EquipAlternatives("% CLUB", 
            new Dictionary<string, string>
            {
                { "Normal", "STONE" },
                {"Fire", "SPARK"},
                {"Ice", "SNOW"},
                {"Poison", "DIRTY"},
                {"Light", "SHINY"},
                {"Dark", "DIM"},
                {"Dark and Light", "WARPED"},
                {"Normal and Fire", "BULK"},
                {"Ice and Poison", "STILL"},
                {"Dark and Fire", "AWARE"}
            }) },
        { "TORCH", new EquipAlternatives("%TORCH", 
            new Dictionary<string, string>
            {
                { "Normal", "GLOW " },
                {"Fire", ""},
                {"Ice", "FROST "},
                {"Poison", "MIASMA "},
                {"Light", "SOLAR "},
                {"Dark", "VOID "},
                {"Dark and Light", "CURIOUS "},
                {"Normal and Fire", "MECHANICAL "},
                {"Ice and Poison", "STILL-FLAME "},
                {"Dark and Fire", "DEEP "}
            }) },
        { "TWISTED STAFF", new EquipAlternatives("% STAFF", 
            new Dictionary<string, string>
            {
                { "Normal", "BRAIDED" },
                {"Fire", "TWISTED"},
                {"Ice", "ICICLE"},
                {"Poison", "THORNY"},
                {"Light", "HELIX"},
                {"Dark", "CREEPING"},
                {"Dark and Light", "DUALITY"},
                {"Normal and Fire", "CABLE"},
                {"Ice and Poison", "INFINITY"},
                {"Dark and Fire", "SPIRAL"}
            }) },
        { "VAMPIRE HUNTER SWORD", new EquipAlternatives("% HUNTER SWORD", 
            new Dictionary<string, string>
            {
                { "Normal", "WEREWOLF" },
                {"Fire", "YUKI-ONNA"},
                {"Ice", "DEMON"},
                {"Poison", "DRAGON"},
                {"Light", "VAMPIRE"},
                {"Dark", "ANGEL"},
                {"Dark and Light", "DEITY"},
                {"Normal and Fire", "AUTOMATON"},
                {"Ice and Poison", "HUMAN"},
                {"Dark and Fire", "HOMUNCULUS"}
            }) },
        { "WOLFRAM GREATSWORD", new EquipAlternatives("% GREATSWORD", 
            new Dictionary<string, string>
            {
                { "Normal", "WOLFRAM" },
                {"Fire", "BRANDO"},
                {"Ice", "ISA"},
                {"Poison", "ADELMAR"},
                {"Light", "DAGOBERT"},
                {"Dark", "WILLEHAD"},
                {"Dark and Light", "WIDOGAST"},
                {"Normal and Fire", "HELMOLD"},
                {"Ice and Poison", "FARAMUND"},
                {"Dark and Fire", "LEUTHAR"}
            }) },
        { "WOODEN SHIELD", new EquipAlternatives("% SHIELD", 
            new Dictionary<string, string>
            {
                { "Normal", "WOODEN" },
                {"Fire", "HEATED"},
                {"Ice", "FROSTED"},
                {"Poison", "POISONED"},
                {"Light", "MIRROR"},
                {"Dark", "DIFFUSER"},
                {"Dark and Light", "CORRUPTED"},
                {"Normal and Fire", "STEEL"},
                {"Ice and Poison", "IMMUNE"},
                {"Dark and Fire", "DEPTHS"}
            }) },
        { "CROSSBOW", new EquipAlternatives("%CROSSBOW", 
            new Dictionary<string, string>
            {
                { "Normal", "" },
                {"Fire", "FIRE "},
                {"Ice", "ICE "},
                {"Poison", "TOXIC "},
                {"Light", "HOLY "},
                {"Dark", "VOID "},
                {"Dark and Light", "CHAOS "},
                {"Normal and Fire", "AUTO "},
                {"Ice and Poison", "TIME "},
                {"Dark and Fire", "SOUL "}
            }) },
        { "STEEL NEEDLE", new EquipAlternatives("% NEEDLE", 
            new Dictionary<string, string>
            {
                { "Normal", "STEEL" },
                {"Fire", "FIRE"},
                {"Ice", "ICE"},
                {"Poison", "POISON"},
                {"Light", "LIGHT"},
                {"Dark", "DARK"},
                {"Dark and Light", "CHAOS"},
                {"Normal and Fire", "MECHANICAL"},
                {"Ice and Poison", "ETERNAL"},
                {"Dark and Fire", "SOUL"}
            }) },
        { "HAMMER OF CRUELTY", new EquipAlternatives("HAMMER OF %", 
            new Dictionary<string, string>
            {
                { "Normal", "BATTLE" },
                {"Fire", "BURNING"},
                {"Ice", "DROWNING"},
                {"Poison", "PLAGUES"},
                {"Light", "DOMINATION"},
                {"Dark", "BLINDNESS"},
                {"Dark and Light", "CRUELTY"},
                {"Normal and Fire", "ENSLAVEMENT"},
                {"Ice and Poison", "IMPRISONMENT"},
                {"Dark and Fire", "WAR"}
            }) },
        { "LUCID BLADE", new EquipAlternatives("% BLADE", 
            new Dictionary<string, string>
            {
                { "Normal", "CLEAR" },
                {"Fire", "FIERCE"},
                {"Ice", "DISTANT"},
                {"Poison", "LYING"},
                {"Light", "LUCID"},
                {"Dark", "CRYPTIC"},
                {"Dark and Light", "CRITICAL"},
                {"Normal and Fire", "REHEARSED"},
                {"Ice and Poison", "STEADFAST"},
                {"Dark and Fire", "EMPATHETIC"}
            }) },
        { "JOTUNN SLAYER", new EquipAlternatives("% SLAYER", 
            new Dictionary<string, string>
            {
                { "Normal", "VINDR" },
                {"Fire", "MJOLL"},
                {"Ice", "EIDR"},
                {"Poison", "YMIR"},
                {"Light", "AMR"},
                {"Dark", "HRODR"},
                {"Dark and Light", "DROFN"},
                {"Normal and Fire", "FJOLVERKR"},
                {"Ice and Poison", "JORMUNGANDR"},
                {"Dark and Fire", "JOTUNN"}
            }) },
        { "RAPIER", new EquipAlternatives("%RAPIER", 
            new Dictionary<string, string>
            {
                { "Normal", "" },
                {"Fire", "WARM "},
                {"Ice", "COLD "},
                {"Poison", "TAINTED "},
                {"Light", "HOLY "},
                {"Dark", "BLACK "},
                {"Dark and Light", "CORRUPTED "},
                {"Normal and Fire", "MECHANICAL "},
                {"Ice and Poison", "TIME "},
                {"Dark and Fire", "SPIRIT "}
            }) },
        { "PRIVATEER MUSKET", new EquipAlternatives("% MUSKET", 
            new Dictionary<string, string>
            {
                { "Normal", "FIGHTER" },
                {"Fire", "KILLER"},
                {"Ice", "HUNTER"},
                {"Poison", "ASSASSIN"},
                {"Light", "GUARD"},
                {"Dark", "PRIVATEER"},
                {"Dark and Light", "ANARCHIST"},
                {"Normal and Fire", "SOLDIER"},
                {"Ice and Poison", "SNIPER"},
                {"Dark and Fire", "DEFENDER"}
            }) },
        { "BRITTLE ARMING SWORD", new EquipAlternatives("BRITTLE % SWORD", 
            new Dictionary<string, string>
            {
                { "Normal", "ARMING" },
                {"Fire", "FIGHTING"},
                {"Ice", "HUNTING"},
                {"Poison", "KILLING"},
                {"Light", "CLEANSING"},
                {"Dark", "OFFERING"},
                {"Dark and Light", "RIOTING"},
                {"Normal and Fire", "CRAFTING"},
                {"Ice and Poison", "SLAYING"},
                {"Dark and Fire", "SINNING"}
            }) },
        { "GOLDEN KHOPESH", new EquipAlternatives("% KHOPESH", 
            new Dictionary<string, string>
            {
                { "Normal", "GOLDEN" },
                {"Fire", "BRONZE"},
                {"Ice", "SILVER"},
                {"Poison", "TUNGSTEN"},
                {"Light", "MYTHRIL"},
                {"Dark", "SILVER"},
                {"Dark and Light", "DAMASCUS"},
                {"Normal and Fire", "STEEL"},
                {"Ice and Poison", "LEAD"},
                {"Dark and Fire", "OBSIDIAN"}
            }) },
        { "GOLDEN SICKLE", new EquipAlternatives("% SICKLE", 
            new Dictionary<string, string>
            {
                { "Normal", "GOLDEN" },
                {"Fire", "BRONZE"},
                {"Ice", "SILVER"},
                {"Poison", "TUNGSTEN"},
                {"Light", "MYTHRIL"},
                {"Dark", "SILVER"},
                {"Dark and Light", "DAMASCUS"},
                {"Normal and Fire", "STEEL"},
                {"Ice and Poison", "LEAD"},
                {"Dark and Fire", "OBSIDIAN"}
            }) },
        { "ICE SICKLE", new EquipAlternatives("% SICKLE", 
            new Dictionary<string, string>
            {
                { "Normal", "IRON" },
                {"Fire", "FIRE"},
                {"Ice", "ICE"},
                {"Poison", "POISON"},
                {"Light", "LIGHT"},
                {"Dark", "DARK"},
                {"Dark and Light", "CHAOS"},
                {"Normal and Fire", "MECHANICAL"},
                {"Ice and Poison", "ETERNAL"},
                {"Dark and Fire", "SOUL"}
            }) },
        { "OBSIDIAN CURSEBRAND", new EquipAlternatives("OBSIDIAN %BRAND", 
            new Dictionary<string, string>
            {
                { "Normal", "EARTH" },
                {"Fire", "HEAT"},
                {"Ice", "COLD"},
                {"Poison", "BANE"},
                {"Light", "HEAL"},
                {"Dark", "CURSE"},
                {"Dark and Light", "CHAOS"},
                {"Normal and Fire", "FORGE"},
                {"Ice and Poison", "VENOM"},
                {"Dark and Fire", "SOUL"}
            }) },
        { "OBSIDIAN POISONGUARD", new EquipAlternatives("OBSIDIAN %GUARD", 
            new Dictionary<string, string>
            {
                { "Normal", "STONE" },
                {"Fire", "FIRE"},
                {"Ice", "ICE"},
                {"Poison", "VENOM"},
                {"Light", "LIGHT"},
                {"Dark", "POISON"},
                {"Dark and Light", "CHAOS"},
                {"Normal and Fire", "STEEL"},
                {"Ice and Poison", "TIME"},
                {"Dark and Fire", "SOUL"}
            }) },
        { "SKELETON AXE", new EquipAlternatives("% AXE", 
            new Dictionary<string, string>
            {
                { "Normal", "SOLDIER" },
                {"Fire", "DEMON"},
                {"Ice", "WRAITH"},
                {"Poison", "SLIME"},
                {"Light", "ANGEL"},
                {"Dark", "SKELETON"},
                {"Dark and Light", "ZOMBIE"},
                {"Normal and Fire", "GOLEM"},
                {"Ice and Poison", "LICH"},
                {"Dark and Fire", "GHOST"}
            })},
        { "CURSED BLADE", new EquipAlternatives("% BLADE", 
            new Dictionary<string, string>
            {
                { "Normal", "QUICK" },
                {"Fire", "HOT"},
                {"Ice", "COLD"},
                {"Poison", "TOXIC"},
                {"Light", "BLESSED"},
                {"Dark", "CURSED"},
                {"Dark and Light", "CHAOTIC"},
                {"Normal and Fire", "FORGED"},
                {"Ice and Poison", "DEATH"},
                {"Dark and Fire", "SPIRIT"}
            }) },
        { "MARAUDER BLACK FLAIL", new EquipAlternatives("MARAUDER % FLAIL", 
            new Dictionary<string, string>
            {
                { "Normal", "BLACK" },
                {"Fire", "RED"},
                {"Ice", "BLUE"},
                {"Poison", "GREEN"},
                {"Light", "WHITE"},
                {"Dark", "VOID"},
                {"Dark and Light", "GRAY"},
                {"Normal and Fire", "PURPLE"},
                {"Ice and Poison", "BROWN"},
                {"Dark and Fire", "MAROON"}
            }) },
        { "DOUBLE CROSSBOW" , new EquipAlternatives("DOUBLE %CROSSBOW", 
            new Dictionary<string, string>
            {
                { "Normal", "" },
                {"Fire", "FIRE "},
                {"Ice", "ICE "},
                {"Poison", "TOXIC "},
                {"Light", "HOLY "},
                {"Dark", "VOID "},
                {"Dark and Light", "CHAOS "},
                {"Normal and Fire", "AUTO "},
                {"Ice and Poison", "TIME "},
                {"Dark and Fire", "SOUL "}
            })},
        { "FIRE SWORD", new EquipAlternatives("% SWORD", 
            new Dictionary<string, string>
            {
                { "Normal", "STONE" },
                {"Fire", "FIRE"},
                {"Ice", "ICE"},
                {"Poison", "VENOM"},
                {"Light", "LIGHT"},
                {"Dark", "POISON"},
                {"Dark and Light", "CHAOS"},
                {"Normal and Fire", "STEEL"},
                {"Ice and Poison", "TIME"},
                {"Dark and Fire", "SOUL"}
            }) },
        { "STEEL LANCE", new EquipAlternatives("% LANCE", 
            new Dictionary<string, string>
            {
                { "Normal", "STEEL" },
                {"Fire", "FIRE"},
                {"Ice", "ICE"},
                {"Poison", "POISON"},
                {"Light", "LIGHT"},
                {"Dark", "DARK"},
                {"Dark and Light", "CHAOS"},
                {"Normal and Fire", "MECHANICAL"},
                {"Ice and Poison", "ETERNAL"},
                {"Dark and Fire", "SOUL"}
            }) },
        { "ELFEN LONGSWORD", new EquipAlternatives("% LONGSWORD", 
            new Dictionary<string, string>
            {
                { "Normal", "ELFEN" },
                {"Fire", "DEMONIC"},
                {"Ice", "TROLL"},
                {"Poison", "GOBLIN"},
                {"Light", "CELESTIAL"},
                {"Dark", "SHADOWFOLK"},
                {"Dark and Light", "RABBITFOLK"},
                {"Normal and Fire", "GOLEM"},
                {"Ice and Poison", "SUCSARIAN"},
                {"Dark and Fire", "HOMUNCULUS"}
            }) },
        { "STEEL CLAW", new EquipAlternatives("% CLAW", 
            new Dictionary<string, string>
            {
                { "Normal", "STEEL" },
                {"Fire", "FIRE"},
                {"Ice", "ICE"},
                {"Poison", "POISON"},
                {"Light", "LIGHT"},
                {"Dark", "DARK"},
                {"Dark and Light", "CHAOS"},
                {"Normal and Fire", "MECHANICAL"},
                {"Ice and Poison", "ETERNAL"},
                {"Dark and Fire", "SOUL"}
            }) },
        { "STEEL CLUB", new EquipAlternatives("% CLUB", 
            new Dictionary<string, string>
            {
                { "Normal", "STEEL" },
                {"Fire", "FIRE"},
                {"Ice", "ICE"},
                {"Poison", "POISON"},
                {"Light", "LIGHT"},
                {"Dark", "DARK"},
                {"Dark and Light", "CHAOS"},
                {"Normal and Fire", "MECHANICAL"},
                {"Ice and Poison", "ETERNAL"},
                {"Dark and Fire", "SOUL"}
            }) },
        { "SAINT ISHII", new EquipAlternatives("SAINT %", 
            new Dictionary<string, string>
            {
                { "Normal", "ISHIDA" },
                {"Fire", "HINO"},
                {"Ice", "YUKIMURA"},
                {"Poison", "INABA"},
                {"Light", "MOCHIZUKI"},
                {"Dark", "NOGUCHI"},
                {"Dark and Light", "AKAI"},
                {"Normal and Fire", "NISHIMURA"},
                {"Ice and Poison", "NOZAWA"},
                {"Dark and Fire", "ISHII"}
            }) },
        { "SILVER RAPIER", new EquipAlternatives("% RAPIER", 
            new Dictionary<string, string>
            {
                { "Normal", "GOLDEN" },
                {"Fire", "BRONZE"},
                {"Ice", "SILVER"},
                {"Poison", "TUNGSTEN"},
                {"Light", "SILVER"},
                {"Dark", "OBSIDIAN"},
                {"Dark and Light", "DAMASCUS"},
                {"Normal and Fire", "STEEL"},
                {"Ice and Poison", "LEAD"},
                {"Dark and Fire", "OBSIDIAN"}
            }) },
        { "HERITAGE SWORD", new EquipAlternatives("% SWORD", 
            new Dictionary<string, string>
            {
                { "Normal", "HERITAGE" },
                {"Fire", "FAMILIAL"},
                {"Ice", "SIBLING"},
                {"Poison", "COUP"},
                {"Light", "KINGDOM"},
                {"Dark", "LONER"},
                {"Dark and Light", "TRIBAL"},
                {"Normal and Fire", "TRADITIONAL"},
                {"Ice and Poison", "DYNASTY"},
                {"Dark and Fire", "BLOODLINE"}
            }) },
        { "DARK GREATSWORD", new EquipAlternatives("% GREATSWORD", 
            new Dictionary<string, string>
            {
                { "Normal", "STEEL" },
                {"Fire", "FIRE"},
                {"Ice", "ICE"},
                {"Poison", "POISON"},
                {"Light", "LIGHT"},
                {"Dark", "DARK"},
                {"Dark and Light", "CHAOS"},
                {"Normal and Fire", "MECHANICAL"},
                {"Ice and Poison", "ETERNAL"},
                {"Dark and Fire", "SOUL"}
            }) },
        { "SHINING BLADE", new EquipAlternatives("% BLADE", 
            new Dictionary<string, string>
            {
                { "Normal", "BRIGHT" },
                {"Fire", "BURNING"},
                {"Ice", "REFLECTING"},
                {"Poison", "ALTERING"},
                {"Light", "SHINING"},
                {"Dark", "FORTUITY"},
                {"Dark and Light", "CHANGING"},
                {"Normal and Fire", "INNOVATIVE"},
                {"Ice and Poison", "ETERNITY"},
                {"Dark and Fire", "DEPRAVITY"}
            }) },
        { "POISON CLAW", new EquipAlternatives("% CLAW", 
            new Dictionary<string, string>
            {
                { "Normal", "TITANIUM" },
                {"Fire", "VOLCANO"},
                {"Ice", "ICEBERG"},
                {"Poison", "POISON"},
                {"Light", "SUN"},
                {"Dark", "VOID"},
                {"Dark and Light", "UNIVERSE"},
                {"Normal and Fire", "PERFECT"},
                {"Ice and Poison", "DEATH"},
                {"Dark and Fire", "HUMANITY"}
            }) },
        { "IRON CLUB", new EquipAlternatives("% CLUB", 
            new Dictionary<string, string>
            {
                { "Normal", "IRON" },
                {"Fire", "FIRE"},
                {"Ice", "ICE"},
                {"Poison", "POISON"},
                {"Light", "LIGHT"},
                {"Dark", "DARK"},
                {"Dark and Light", "CHAOS"},
                {"Normal and Fire", "MECHANICAL"},
                {"Ice and Poison", "ETERNAL"},
                {"Dark and Fire", "SOUL"}
            }) },
        { "IRON TORCH", new EquipAlternatives("% TORCH", 
            new Dictionary<string, string>
            {
                { "Normal", "STONE" },
                {"Fire", "IRON"},
                {"Ice", "ICE"},
                {"Poison", "POISON"},
                {"Light", "LIGHT"},
                {"Dark", "DARK"},
                {"Dark and Light", "CHAOS"},
                {"Normal and Fire", "MECHANICAL"},
                {"Ice and Poison", "ETERNAL"},
                {"Dark and Fire", "SOUL"}
            }) },
        { "GHOST SWORD", new EquipAlternatives("% SWORD", 
            new Dictionary<string, string>
            {
                { "Normal", "WITCH" },
                {"Fire", "DEMON"},
                {"Ice", "WRAITH"},
                {"Poison", "ZOMBIE"},
                {"Light", "GHOST"},
                {"Dark", "SKELETON"},
                {"Dark and Light", "POLTERGEIST"},
                {"Normal and Fire", "GOLEM"},
                {"Ice and Poison", "LICH"},
                {"Dark and Fire", "SPIRIT"}
            }) },
        { "CAVALRY SABER", new EquipAlternatives("% SABER", 
            new Dictionary<string, string>
            {
                { "Normal", "FIGHTER" },
                {"Fire", "KILLER"},
                {"Ice", "HUNTER"},
                {"Poison", "ASSASSIN"},
                {"Light", "GUARD"},
                {"Dark", "CAVALRY"},
                {"Dark and Light", "ANARCHIST"},
                {"Normal and Fire", "SOLDIER"},
                {"Ice and Poison", "SNIPER"},
                {"Dark and Fire", "COURTESAN"}
            }) },

        { "BLUE FLAME ARC", new EquipAlternatives("% ARC", 
            new Dictionary<string, string>
            {
                { "Normal", "WHITE STONE" },
                {"Fire", "BLUE FLAME"},
                {"Ice", "BLUE ICE"},
                {"Poison", "GREEN POISON"},
                {"Light", "YELLOW LIGHT"},
                {"Dark", "DARK VOID"},
            }) },
        { "EARTH STRIKE", new EquipAlternatives("% STRIKE", 
            new Dictionary<string, string>
            {
                { "Normal", "EARTH" },
                {"Fire", "FLAME"},
                {"Ice", "SNOW"},
                {"Poison", "SLIME"},
                {"Light", "ORB"},
                {"Dark", "DUNG"},
            }) },
        { "EARTH THORN", new EquipAlternatives("% THORN", 
            new Dictionary<string, string>
            {
                { "Normal", "EARTH" },
                {"Fire", "FLAME"},
                {"Ice", "SNOW"},
                {"Poison", "SLIME"},
                {"Light", "ORB"},
                {"Dark", "DUNG"},
            }) },
        { "FIRE WORM", new EquipAlternatives("% WORM", 
            new Dictionary<string, string>
            {
                { "Normal", "ROCK" },
                {"Fire", "FIRE"},
                {"Ice", "ICE"},
                {"Poison", "SLIME"},
                {"Light", "LIGHT"},
                {"Dark", "DARK"},
            }) },
        { "FLAME FLARE", new EquipAlternatives("% FLARE", 
            new Dictionary<string, string>
            {
                { "Normal", "EARTH" },
                {"Fire", "FLAME"},
                {"Ice", "SNOW"},
                {"Poison", "SLIME"},
                {"Light", "LIGHT"},
                {"Dark", "DARK"},
            }) },
        { "FLAME SPEAR", new EquipAlternatives("% SPEAR", 
            new Dictionary<string, string>
            {
                { "Normal", "EARTH" },
                {"Fire", "FLAME"},
                {"Ice", "SNOW"},
                {"Poison", "SLIME"},
                {"Light", "LIGHT"},
                {"Dark", "DARK"},
            }) },
        { "ICE SPEAR", new EquipAlternatives("% SPEAR", 
            new Dictionary<string, string>
            {
                { "Normal", "ROCK" },
                {"Fire", "FIRE"},
                {"Ice", "ICE"},
                {"Poison", "POISON"},
                {"Light", "LIGHTNING"},
                {"Dark", "VOID"},
            }) },
        { "ICE TEAR", new EquipAlternatives("% TEAR", 
            new Dictionary<string, string>
            {
                { "Normal", "ROCK" },
                {"Fire", "FIRE"},
                {"Ice", "ICE"},
                {"Poison", "POISON"},
                {"Light", "LIGHTNING"},
                {"Dark", "VOID"},
            }) },
        { "IGNIS CALOR", new EquipAlternatives("% CALOR", 
            new Dictionary<string, string>
            {
                { "Normal", "LAPIS" },
                {"Fire", "IGNIS"},
                {"Ice", "GLACIES"},
                {"Poison", "VENENUM"},
                {"Light", "LUX"},
                {"Dark", "INANIS"},
            }) },
        { "LAVA CHASM", new EquipAlternatives("% CHASM", 
            new Dictionary<string, string>
            {
                { "Normal", "EARTH" },
                {"Fire", "LAVA"},
                {"Ice", "GLACIER"},
                {"Poison", "MARSH"},
                {"Light", "MIRROR"},
                {"Dark", "VOID"},
            }) },
        { "LIGHTNING", new EquipAlternatives("%", 
            new Dictionary<string, string>
            {
                { "Normal", "BULLET SHOT" },
                {"Fire", "FIRE RAY"},
                {"Ice", "ICE CRASH"},
                {"Poison", "ACID RAY"},
                {"Light", "LIGHTNING"},
                {"Dark", "DARK BURST"},
            }) },
        { "MOON BEAM", new EquipAlternatives("% BEAM", 
            new Dictionary<string, string>
            {
                { "Normal", "EARTH" },
                {"Fire", "MARS"},
                {"Ice", "PLUTO"},
                {"Poison", "NEPTUNE"},
                {"Light", "MOON"},
                {"Dark", "URANUS"},
            }) },
        { "SLIME ORB", new EquipAlternatives("% ORB", 
            new Dictionary<string, string>
            {
                { "Normal", "ROCK" },
                {"Fire", "FLAME"},
                {"Ice", "SNOW"},
                {"Poison", "SLIME"},
                {"Light", "LIGHT"},
                {"Dark", "DARK"},
            }) },
        { "WIND SLICER", new EquipAlternatives("% SLICER", 
            new Dictionary<string, string>
            {
                { "Normal", "WIND" },
                {"Fire", "LAVA"},
                {"Ice", "ICICLE"},
                {"Poison", "FANG"},
                {"Light", "LIGHT"},
                {"Dark", "VOID"},
            }) },
        { "TORNADO", new EquipAlternatives("%", 
            new Dictionary<string, string>
            {
                { "Normal", "TORNADO" },
                {"Fire", "FIRE TORNADO"},
                {"Ice", "HURRICANE"},
                {"Poison", "MIASMA SCREW"},
                {"Light", "LIGHT DANCE"},
                {"Dark", "BLACK HOLE"},
            }) },
        { "DARK SKULL", new EquipAlternatives("% SKULL", 
            new Dictionary<string, string>
            {
                { "Normal", "STONE" },
                {"Fire", "FIRE"},
                {"Ice", "ICE"},
                {"Poison", "ACID"},
                {"Light", "LIGHT"},
                {"Dark", "DARK"},
            }) },
        { "POISON MIST", new EquipAlternatives("% MIST", 
            new Dictionary<string, string>
            {
                { "Normal", "WINDY" },
                {"Fire", "HOT"},
                {"Ice", "DEEP"},
                {"Poison", "POISON"},
                {"Light", "HOLY"},
                {"Dark", "DARK"},
            }) },
        { "PUMPKIN POP", new EquipAlternatives("PUMPKIN %", 
            new Dictionary<string, string>
            {
                { "Normal", "SNAP" },
                {"Fire", "BANG"},
                {"Ice", "CRACK"},
                {"Poison", "OOZE"},
                {"Light", "BURST"},
                {"Dark", "POP"},
            }) }
    };
    
    public static string ChangeNameBasedOnElement(string name, string element)
    {
        if (!EquipmentLookup.TryGetValue(name, out var variantLookup))
        {
            return name;
        }
        var template = variantLookup.Syntax;
        var term = variantLookup.ElementReplacement[element];
        return template.Replace("%", term);
    }
}