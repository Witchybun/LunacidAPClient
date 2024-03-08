using System.Linq;
using System.Reflection;
using BepInEx.Logging;
using HarmonyLib;
using LunacidAP.Data;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace LunacidAP
{
    public class WeaponHandler
    {
        private static ArchipelagoClient _archipelago;
        private static ManualLogSource _log;
        private static string CurrentCastChild { get; set; } = "";
        private static int CurrentElement { get; set; } = 0;

        public static void Awake(ArchipelagoClient archipelago, ManualLogSource log)
        {
            _archipelago = archipelago;
            _log = log;
            Harmony.CreateAndPatchAll(typeof(WeaponHandler));
        }

        [HarmonyPatch(typeof(Weapon_scr), "Attack")]
        [HarmonyPrefix]
        private static bool Attack_ModifyWeaponElement(Weapon_scr __instance)
        {
            if (!SlotData.RandomElements)
            {
                return true;
            }
            var weaponName = StaticFuncs.REMOVE_NUMS(__instance.name.Replace("(Clone)", ""));
            if (!ConnectionData.Elements.TryGetValue(weaponName, out string elementName) || LunacidItems.WeaponsWithDefaultElement.Contains(weaponName))
            {
                _log.LogInfo($"{weaponName} not in list for elements.  This may or may not be an error.  Returning.");
                return true;
            }
            if (LunacidItems.ElementToID.TryGetValue(elementName, out int element))
            {
                _log.LogInfo($"Changing element to {elementName}");
                __instance.WEP_ELEMENT = element;
            }
            return true;
        }

        [HarmonyPatch(typeof(Magic_scr), "Cast")]
        [HarmonyPrefix]
        private static bool Cast_ModifySpellElement(Magic_scr __instance)
        {
            if (!SlotData.RandomElements)
            {
                return true;
            }
            var spellName = StaticFuncs.REMOVE_NUMS(__instance.name.Replace("(Clone)", ""));
            if (!ConnectionData.Elements.TryGetValue(spellName, out string elementName) || LunacidItems.WeaponsWithDefaultElement.Contains(spellName))
            {
                _log.LogInfo($"{spellName} not in list for elements.  This may or may not be an error.  Returning.");
                return true;
            }
            CurrentCastChild = __instance.MAG_CHILD;
            if (LunacidItems.ElementToID.TryGetValue(elementName, out int element))
            {
                _log.LogInfo($"Changing element to {elementName}");
                CurrentElement = element;
            }
            return true;
        }

        [HarmonyPatch(typeof(Damage_Trigger), "Hurt")]
        [HarmonyPrefix]
        private static bool Hurt_WhatElementIsIt(Damage_Trigger __instance)
        {
            _log.LogInfo($"Comparing {__instance.name} and {CurrentCastChild}");
            var castName = StaticFuncs.REMOVE_NUMS(__instance.name.Replace("(Clone)", ""));
            if (castName == CurrentCastChild)
            {
                __instance.element = CurrentElement;
            }
            else if (LunacidItems.ArrowToWeapon.TryGetValue(castName, out string weapon))
            {
                __instance.element = LunacidItems.ElementToID[ConnectionData.Elements[weapon]];
            }
            else
            {
                _log.LogInfo($"Ignoring as its not the recent cast");
            }
            return true;
        }

        [HarmonyPatch(typeof(Menus), "ItemLoad")]
        [HarmonyPostfix]
        private static void ItemLoad_FixElementInMenu(Menus __instance)
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
                        int result2 = -1;
                        GameObject gameObject2;
                        if (int.TryParse(__instance.CON.CURRENT_PL_DATA.WEPS[num].Substring(__instance.CON.CURRENT_PL_DATA.WEPS[num].Length - 2, 2), out result2))
                        {
                            gameObject2 = UnityEngine.Object.Instantiate(Resources.Load("WEPS/" + __instance.CON.CURRENT_PL_DATA.WEPS[num].Substring(0, __instance.CON.CURRENT_PL_DATA.WEPS[num].Length - 2))) as GameObject;
                        }
                        else
                        {
                            gameObject2 = UnityEngine.Object.Instantiate(Resources.Load("WEPS/" + __instance.CON.CURRENT_PL_DATA.WEPS[num])) as GameObject;
                        }
                        Weapon_scr component2 = gameObject2.GetComponent<Weapon_scr>();
                        Sprite elementSprite;
                        var nameWithoutClone = component2.name.Replace("(Clone)", "");
                        if (IsElementShuffled(nameWithoutClone, out var element))
                        {
                            elementSprite = __instance.ELEM[element];
                        }
                        else
                        {
                            elementSprite = __instance.ELEM[component2.WEP_ELEMENT];
                        }
                        __instance.TXT[9].transform.GetChild(0).GetComponent<Image>().sprite = elementSprite;
                        UnityEngine.Object.Destroy(gameObject2);
                        break;
                    }
                case 5:
                    {
                        if (StaticFuncs.IS_NULL(__instance.CON.CURRENT_PL_DATA.SPELLS[num]))
                        {
                            break;
                        }
                        GameObject gameObject = UnityEngine.Object.Instantiate(Resources.Load("MAGIC/" + __instance.CON.CURRENT_PL_DATA.SPELLS[num])) as GameObject;
                        Magic_scr component = gameObject.GetComponent<Magic_scr>();
                        Sprite elementSprite;
                        var nameWithoutClone = component.name.Replace("(Clone)", "");
                        if (IsElementShuffled(nameWithoutClone, out var element))
                        {
                            elementSprite = __instance.ELEM[element];
                        }
                        else
                        {
                            elementSprite = __instance.ELEM[component.MAG_ELEM];
                        }
                        __instance.TXT[22].transform.GetChild(0).GetComponent<Image>().sprite = elementSprite;
                        UnityEngine.Object.Destroy(gameObject);
                        break;
                    }
            }
        }

        private static bool IsElementShuffled(string weaponName, out int element)
        {
            if (ConnectionData.Elements.TryGetValue(weaponName, out string givenElement))
            {
                if (LunacidItems.WeaponsWithDefaultElement.Contains(weaponName))
                {
                    element = -1;
                    return false;
                }
                _log.LogInfo($"Found {givenElement} for weapon {weaponName}");
                element = LunacidItems.ElementToID[givenElement];
                return true;
            }
            _log.LogError($"The weapon {weaponName} was not in the element dictionary");
            element = -1;
            return false;
        }
    }
}