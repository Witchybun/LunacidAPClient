using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using BepInEx.Logging;
using HarmonyLib;
using I2.Loc;
using LunacidAP.Archipelago;
using LunacidAP.Data;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Object = UnityEngine.Object;

namespace LunacidAP.Patches
{
    public class WeaponHandler
    {
        private static ManualLogSource _log;
        private static List<string> freeSpells = new()
        {
            "WARP_CAST", "BARRIER_CAST", "FLIGHT_CAST", "BRIDGE_CAST"
        };

        public WeaponHandler(ManualLogSource log)
        {
            _log = log;
            Harmony.CreateAndPatchAll(typeof(WeaponHandler));
        }

        // Kudos to Tesseract for suggesting this instead.
        [HarmonyPatch(typeof(CONTROL), "OnSwap")]
        [HarmonyPostfix]
        private static void OnSwap_ModifyWeaponStats(CONTROL __instance)
        {
            if (__instance.EQ_WEP is null)
            {
                return;
            }
            var weaponName = StaticFuncs.REMOVE_NUMS(__instance.EQ_WEP.name.Replace("(Clone)", ""));
            if (ArchipelagoClient.AP.SlotData.RandomElements)
            {
                if (ConnectionData.Elements.TryGetValue(weaponName, out string elementName))
                {
                    if (LunacidItems.ElementToID.TryGetValue(elementName, out int element))
                    {
                        __instance.EQ_WEP.WEP_ELEMENT = element;
                    }
                }
            }

            if (ArchipelagoClient.AP.SlotData.RandomEquipStats != RandomEquip.Off)
            {
                if (!ConnectionData.RandomizedWeaponData.TryGetValue(weaponName, out var data))
                {
                    _log.LogWarning($"Could not find data for {weaponName}");
                }
                else
                {
                    // Recalculate damage with new value.
                    __instance.EQ_WEP.WEP_DAMAGE = GetRealDamageForWeapon(__instance, data.Damage, __instance.EQ_WEP);
                    // The rest doesn't need this.
                    __instance.EQ_WEP.WEP_COOLDOWN = data.Speed;
                    __instance.EQ_WEP.WEP_GUARD = data.Guard;
                    __instance.EQ_WEP.WEP_BACKSTEP = data.Backstep;
                    __instance.EQ_WEP.WEP_WEIGHT = data.Thrust;
                }
            }
            if (weaponName is "THORN" or "GHOST SWORD")
            {
                var currentWeapon = __instance.EQ_WEP.gameObject;
                var oldHand = currentWeapon.transform.Find("HAND");
                GameObject relevantWeapon = (GameObject)Resources.Load("WEPS/MOONLIGHT");
                if (weaponName == "THORN")
                {
                    relevantWeapon = (GameObject)Resources.Load("WEPS/REPLICA SWORD");
                }
                var newHand = Object.Instantiate(relevantWeapon.transform.Find("HAND"), oldHand.parent, true);
                newHand.localPosition = oldHand.localPosition;
                newHand.localRotation = oldHand.localRotation;
                newHand.localScale = oldHand.localScale;
                oldHand.gameObject.SetActive(false);
            }
        }

        private static float GetRealDamageForWeapon(CONTROL control, float damage, Weapon_scr weapon)
        {
            if (weapon.type == 0 && weapon.special != 14)
            {
                damage *= StaticFuncs.calcStat(5, control.CURRENT_PL_DATA.PLAYER_STR, null);
            }
            else if (weapon.type == 1 || weapon.special == 14)
            {
                damage *= StaticFuncs.calcStat(5, control.CURRENT_PL_DATA.PLAYER_DEX, null);
            }
            return damage;
        }

        [HarmonyPatch(typeof(CONTROL), "EQMagic")]
        [HarmonyPostfix]
        private static void EQMagic_ModifyEquippedSpell(CONTROL __instance)
        {
            if (!string.IsNullOrEmpty(__instance.CURRENT_PL_DATA.MAG1) && __instance.CURRENT_PL_DATA.MAG1 != "EMPTY")
            {
                if (!ConnectionData.RandomizedSpellData.TryGetValue(__instance.CURRENT_PL_DATA.MAG1, out var data))
                {
                    _log.LogWarning($"Could not find data for {__instance.CURRENT_PL_DATA.MAG1}");
                }
                else
                {
                    __instance.EQ_MAG1.MAG_DAMAGE = StaticFuncs.calcStat(6, __instance.CURRENT_PL_DATA.PLAYER_INT, null);
                    __instance.EQ_MAG1.MIN_CHARGE_TIME = data.MinCastTime;
                    __instance.EQ_MAG1.MAG_CHARGE_TIME = data.CastTime;
                    __instance.EQ_MAG1.MAG_CHARGE_TIME -= StaticFuncs.calcStat(7, __instance.CURRENT_PL_DATA.PLAYER_INT, null);
                    __instance.EQ_MAG1.MAG_COST = data.Cost;
                    if (freeSpells.Contains(__instance.CURRENT_PL_DATA.MAG2))
                    {
                        __instance.EQ_MAG2.MAG_COST = 0;
                    }
                    if (ArchipelagoClient.AP.SlotData.RandomElements)
                    {
                        if (ConnectionData.Elements.TryGetValue(__instance.CURRENT_PL_DATA.MAG1, out var element))
                        {
                            __instance.EQ_MAG1.MAG_ELEM = LunacidItems.ElementToID[element];
                        }
                    }
                    __instance.EQ_MAG1.SetValues();
                }
            }

            if (string.IsNullOrEmpty(__instance.CURRENT_PL_DATA.MAG2) ||
                __instance.CURRENT_PL_DATA.MAG2 == "EMPTY") return;
            {
                if (!ConnectionData.RandomizedSpellData.TryGetValue(__instance.CURRENT_PL_DATA.MAG2, out var data))
                {
                    _log.LogWarning($"Could not find data for {__instance.CURRENT_PL_DATA.MAG2}");
                }
                else
                {
                    __instance.EQ_MAG2.MAG_DAMAGE = GetRealDamageForSpell(__instance, data.Damage);
                    __instance.EQ_MAG2.MIN_CHARGE_TIME = data.MinCastTime;
                    __instance.EQ_MAG2.MAG_CHARGE_TIME = GetRealCastTimeForSpell(__instance, data.CastTime, data.MinCastTime);
                    __instance.EQ_MAG2.MAG_COST = data.Cost;
                    if (freeSpells.Contains(__instance.CURRENT_PL_DATA.MAG2))
                    {
                        __instance.EQ_MAG2.MAG_COST = 0;
                    }
                    if (ArchipelagoClient.AP.SlotData.RandomElements)
                    {
                        if (ConnectionData.Elements.TryGetValue(__instance.CURRENT_PL_DATA.MAG2, out var element))
                        {
                            __instance.EQ_MAG2.MAG_ELEM = LunacidItems.ElementToID[element];
                        }
                    }

                    __instance.EQ_MAG2.SetValues();
                }
            }
        }

        private static float GetRealDamageForSpell(CONTROL control, float damage)
        {
            damage *= StaticFuncs.calcStat(6, control.CURRENT_PL_DATA.PLAYER_INT, null);
            if (control.CURRENT_PL_DATA.PLAYER_CLASS.ToUpper() != "UNDEAD" || !control.EQ_MAG2.MAG_BL) return damage;
            damage *= 2f;
            return damage;
        }

        private static float GetRealCastTimeForSpell(CONTROL control, float castTime, float minCastTime)
        {
            castTime -= StaticFuncs.calcStat(7, control.CURRENT_PL_DATA.PLAYER_INT, null);
            if (castTime < minCastTime)
            {
                castTime = minCastTime;
            }
            return castTime;
        }
        
        [HarmonyPatch(typeof(Menus), "ItemLoad")]
        [HarmonyPostfix]
        private static void ItemLoad_FixStatsInMenu(Menus __instance)
        {
            var eqSelField = __instance.GetType().GetField("EQ_SEL", BindingFlags.Instance | BindingFlags.NonPublic);
            int EQ_SEL = (int)eqSelField.GetValue(__instance);
            int num = EQ_SEL = int.Parse(EventSystem.current.currentSelectedGameObject.name.Substring(3));
            switch (__instance.sub_menu)
            {
                case 4:
                    {
                        if (StaticFuncs.IS_NULL(__instance.CON.CURRENT_PL_DATA.WEPS[num]))
                        {
                            break;
                        }

                        GameObject gameObject2;
                        if (int.TryParse(__instance.CON.CURRENT_PL_DATA.WEPS[num].Substring(__instance.CON.CURRENT_PL_DATA.WEPS[num].Length - 2, 2), out _))
                        {
                            gameObject2 = Object.Instantiate(Resources.Load("WEPS/" + __instance.CON.CURRENT_PL_DATA.WEPS[num].Substring(0, __instance.CON.CURRENT_PL_DATA.WEPS[num].Length - 2))) as GameObject;
                        }
                        else
                        {
                            gameObject2 = Object.Instantiate(Resources.Load("WEPS/" + __instance.CON.CURRENT_PL_DATA.WEPS[num])) as GameObject;
                        }

                        if (gameObject2 != null)
                        {
                            Weapon_scr component2 = gameObject2.GetComponent<Weapon_scr>();
                            var nameWithoutClone = component2.name.Replace("(Clone)", "");
                            var elementSprite = IsElementShuffled(nameWithoutClone, out var element) ? __instance.ELEM[element] : __instance.ELEM[component2.WEP_ELEMENT];
                            __instance.TXT[9].transform.GetChild(0).GetComponent<Image>().sprite = elementSprite;
                            var statData = ConnectionData.RandomizedWeaponData[nameWithoutClone];
                            var CON = GameObject.Find("CONTROL").GetComponent<CONTROL>();
                            var trueDamage = GetRealDamageForWeapon(CON, statData.Damage, component2);
                            __instance.TXT[10].text = trueDamage.ToString("F0") + "\n" + 
                                                      Mathf.Round(10f * (1f / statData.Speed)) + "\n" + 
                                                      component2.WEP_REACH + "\n" + 
                                                      (100f - statData.Guard * 100f) + "%\n" + 
                                                      statData.Backstep + "\n" + statData.Thrust;
                        }
                        
                        Object.Destroy(gameObject2);
                        break;
                    }
                case 5:
                    {
                        if (StaticFuncs.IS_NULL(__instance.CON.CURRENT_PL_DATA.SPELLS[num]))
                        {
                            break;
                        }
                        var gameObject = Object.Instantiate(Resources.Load("MAGIC/" + __instance.CON.CURRENT_PL_DATA.SPELLS[num])) as GameObject;
                        if (gameObject != null)
                        {
                            var component = gameObject.GetComponent<Magic_scr>();
                            var nameWithoutClone = component.name.Replace("(Clone)", "");
                            var statData = ConnectionData.RandomizedSpellData[nameWithoutClone];
                            var num6 = StaticFuncs.RemoveTMPSUB(__instance.TXT[23].transform.GetChild(0));
                            __instance.TXT[23].transform.GetChild(num6).GetComponent<TextMeshProUGUI>().text = statData.Cost.ToString();
                            var CON = GameObject.Find("CONTROL").GetComponent<CONTROL>();
                            var castTime = GetRealCastTimeForSpell(CON, statData.CastTime, statData.MinCastTime);
                            __instance.TXT[23].transform.GetChild(num6 + 1).GetComponent<TextMeshProUGUI>().text = castTime > 0.3f ? castTime.ToString("F2") : LocalizationManager.GetTranslation("Spells/Instant");

                            var damage = GetRealDamageForSpell(CON, statData.Damage);
                            __instance.TXT[23].transform.GetChild(num6 + 2).GetComponent<TextMeshProUGUI>().text = damage.ToString("F2").Substring(0, damage.ToString("F2").Length - 3);

                            if (nameWithoutClone == "BLOOD STRIKE")
                            {
                                break; // To keep the original flavor of this spell which denotes it as blood
                            }
                            var elementSprite = IsElementShuffled(nameWithoutClone, out var element) ? __instance.ELEM[element] : __instance.ELEM[component.MAG_ELEM];
                            __instance.TXT[22].transform.GetChild(0).GetComponent<Image>().sprite = elementSprite;
                        }

                        Object.Destroy(gameObject);
                        break;
                    }
            }
        }

        [HarmonyPatch(typeof(Magic_scr), "Charging")]
        [HarmonyPrefix]
        private static void Charging_MakeImportantSpellsFree(Magic_scr __instance, float amount)
        {
            if (freeSpells.Contains(__instance.MAG_CHILD))
            {
                __instance.MAG_COST = 0;
            }
        }

        private static bool IsElementShuffled(string weaponName, out int element)
        {
            if (ConnectionData.Elements is null)
            {
                element = -1;
                return false;
            }
            if (ConnectionData.Elements.TryGetValue(weaponName, out string givenElement))
            {
                if (LunacidItems.WeaponsWithDefaultElement.Contains(weaponName))
                {
                    element = -1;
                    return false;
                }
                element = LunacidItems.ElementToID[givenElement];
                return true;
            }
            _log.LogError($"The weapon {weaponName} was not in the element dictionary");
            element = -1;
            return false;
        }

        [HarmonyPatch(typeof(Weapon_scr), "Attack")]
        [HarmonyPostfix]
        private static void Attack_DrainStoredEXPInstead(Weapon_scr __instance)
        {
            if (!ArchipelagoClient.AP.SlotData.Levelsanity)
            {
                return;
            }
            if (__instance.special == 5)
            {
                if (__instance.Player.GetComponent<Player_Control_scr>().CON.GetComponent<SimpleMoon>().MOON_MULT >= 10f)
                {
                    decimal currentExp = __instance.Player.GetComponent<Player_Control_scr>().CON.CURRENT_PL_DATA.XP;
                    var nearestHundred = Math.Ceiling(currentExp / 100) * 100;
                    __instance.Player.GetComponent<Player_Control_scr>().CON.CURRENT_PL_DATA.XP = Convert.ToInt32(nearestHundred);
                    ConnectionData.StoredExperience = Mathf.Max(0, ConnectionData.StoredExperience - 1);
                }
            }
        }
    }
}