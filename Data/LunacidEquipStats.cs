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
            var magicScr = spell.GetComponent<Magic_scr>();
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
}