using System;
using System.Collections.Generic;
using System.Linq;
using BepInEx.Logging;
using HarmonyLib;
using LunacidAP.Data;
using UnityEngine;

namespace LunacidAP
{
    public class FlagHandler
    {
        private static ArchipelagoClient _archipelago;
        private static CONTROL CON;
        private static ManualLogSource _log;
        private static string[] errorData { get; set; }

        public static void Awake(ArchipelagoClient archipelago, ManualLogSource log)
        {
            _archipelago = archipelago;
            _log = log;
            Harmony.CreateAndPatchAll(typeof(FlagHandler));
        }

        public static string LoadFlag(int Zone)
        {
            CON = GameObject.Find("CONTROL").GetComponent<CONTROL>();
            var current_string = ZoneDataPicker(Zone);
            _log.LogInfo($"Giving the following ({Zone}): {current_string}");
            return current_string;
        }

        public static void ModifyFlag(int Zone, int Slot, int Value)
        {
            var current_string = LoadFlag(Zone);
            _log.LogInfo($"Before ({Zone}): {current_string}");
            current_string = current_string.Substring(0, Slot - 1) + Value + current_string.Substring(Slot, current_string.Length - Slot);
            _log.LogInfo($"After ({Zone}): {current_string}");
            switch (Zone)
            {
                case 0:
                    CON.CURRENT_PL_DATA.ZONE_0 = current_string;
                    break;
                case 1:
                    CON.CURRENT_PL_DATA.ZONE_1 = current_string;
                    break;
                case 2:
                    CON.CURRENT_PL_DATA.ZONE_2 = current_string;
                    break;
                case 3:
                    CON.CURRENT_PL_DATA.ZONE_3 = current_string;
                    break;
                case 4:
                    CON.CURRENT_PL_DATA.ZONE_4 = current_string;
                    break;
                case 5:
                    CON.CURRENT_PL_DATA.ZONE_5 = current_string;
                    break;
                case 6:
                    CON.CURRENT_PL_DATA.ZONE_6 = current_string;
                    break;
                case 7:
                    CON.CURRENT_PL_DATA.ZONE_7 = current_string;
                    break;
                case 8:
                    CON.CURRENT_PL_DATA.ZONE_8 = current_string;
                    break;
                case 9:
                    CON.CURRENT_PL_DATA.ZONE_9 = current_string;
                    break;
                case 10:
                    CON.CURRENT_PL_DATA.ZONE_10 = current_string;
                    break;
                case 11:
                    CON.CURRENT_PL_DATA.ZONE_11 = current_string;
                    break;
                case 12:
                    CON.CURRENT_PL_DATA.ZONE_12 = current_string;
                    break;
                case 13:
                    CON.CURRENT_PL_DATA.ZONE_13 = current_string;
                    break;
                case 14:
                    CON.CURRENT_PL_DATA.ZONE_14 = current_string;
                    break;
                case 15:
                    CON.CURRENT_PL_DATA.ZONE_15 = current_string;
                    break;
                case 16:
                    CON.CURRENT_PL_DATA.ZONE_16 = current_string;
                    break;
            }
        }

        [HarmonyPatch(typeof(AREA_SAVED_ITEM), "Load")]
        [HarmonyPrefix]
        private static bool Load_RetainItemPickups(AREA_SAVED_ITEM __instance)
        {
            try
            {
                errorData = new string[5] { "", "", "", "", "" };
                errorData[0] = __instance.name;
                errorData[3] = __instance.STATES.Count().ToString();
                errorData[4] = __instance.value.ToString();
                CON = GameObject.Find("CONTROL").GetComponent<CONTROL>();
                var sceneName = __instance.gameObject.scene.name;
                __instance.current_string = ZoneDataPicker(__instance.Zone);
                __instance.value = int.Parse(__instance.current_string.Substring(__instance.Slot - 1, 1));
                var stateController = __instance.name;
                GameObject[] sTATES = __instance.STATES;
                if (sceneName == "FOREST_A1" && stateController == "PATCHI")
                {
                    if (_archipelago.WasItemReceived("Skull of Josiah") && _archipelago.IsLocationChecked("YF: Patchouli's Canopy Offer"))
                    {
                        if (__instance.value <= 5)
                        {
                            __instance.value = 5;
                        }
                    }
                    if (__instance.value > 6)
                    {
                        __instance.value = 6; // try to stop it from incrementing too high.
                    }
                }
                for (int i = 0; i < sTATES.Length; i++)
                {
                    errorData[1] = i.ToString();
                    errorData[2] = sTATES[i].name;
                    if (sceneName == "FOREST_B1" && stateController == "save2" && sTATES[i].name == "SKULL_PICKUP")
                    {
                        __instance.STATES[i].SetActive(value: true);  //turn this spot on.
                    }
                    else
                    {
                        sTATES[i].SetActive(value: false);
                    }
                }
                errorData[1] = "Out of loop";
                errorData[2] = "Out of loop";
                __instance.STATES[__instance.value].SetActive(value: true);
                if (sceneName == "PRISON")
                {
                    var prisonKey = GameObject.Find("PRISON").transform.GetChild(4).GetChild(6).gameObject;
                    if (!prisonKey.activeSelf)
                    {
                        prisonKey.SetActive(value: true);
                    }
                    var hammerPickup = GameObject.Find("PRISON").transform.GetChild(7).GetChild(3).gameObject;
                    if (!hammerPickup.activeSelf)
                    {
                        hammerPickup.SetActive(value: true);
                    }
                }
                else if (sceneName == "PITT_A1")
                {
                    var pittObjects = GameObject.Find("THE_PIT_A1");
                    var woodenGate = pittObjects.transform.GetChild(3).GetChild(4).gameObject;
                    if (woodenGate.activeSelf)
                    {
                        woodenGate.SetActive(value: false);
                    }
                    if (stateController == "META")
                    {
                        sTATES[1].SetActive(value: true);
                        sTATES[2].SetActive(value: true);
                        sTATES[3].SetActive(value: true);
                    }
                }
                else if (sceneName == "CAS_1" && stateController == "SAVE_0")
                {
                    sTATES[0].SetActive(value: true);
                    sTATES[1].SetActive(value: true);
                    sTATES[2].SetActive(value: true);
                }
                else if (sceneName == "ARENA" && (stateController == "CHEST3" || stateController == "CHEST4"))
                {
                    sTATES[0].SetActive(value: true); // always force the chest in a waiting state.
                    sTATES[1].SetActive(value: false);
                }
                return false;
            }
            catch (Exception ex)
            {
                _log.LogError($"Method {nameof(Load_RetainItemPickups)} has failed:");
                _log.LogError($"Name: {errorData[0]}, Iteration: {errorData[1]}, State Name: {errorData[2]}");
                _log.LogError($"State Count: {errorData[3]}, Value: {errorData[4]}");
                _log.LogError($"{ex.Message}");

                return false;
            }
        }

        public static bool DoesPlayerHaveItem(string itemName)
        {
            CON = GameObject.Find("CONTROL").GetComponent<CONTROL>();
            var playerInventory = CON.CURRENT_PL_DATA.ITEMS;
            for (var i = 0; i < 128; i++)
            {
                if (playerInventory[i] == "" || playerInventory[i] == null )
                {
                    return false;
                }
                if (StaticFuncs.REMOVE_NUMS(playerInventory[i]) == itemName)
                {
                    return true;
                }

            }
            return false;
        }

        private static string ZoneDataPicker(int Zone)
        {
            string current_string = "";
            switch (Zone)
            {
                case 0:
                    current_string = CON.CURRENT_PL_DATA.ZONE_0;
                    break;
                case 1:
                    current_string = CON.CURRENT_PL_DATA.ZONE_1;
                    break;
                case 2:
                    current_string = CON.CURRENT_PL_DATA.ZONE_2;
                    break;
                case 3:
                    current_string = CON.CURRENT_PL_DATA.ZONE_3;
                    break;
                case 4:
                    current_string = CON.CURRENT_PL_DATA.ZONE_4;
                    break;
                case 5:
                    current_string = CON.CURRENT_PL_DATA.ZONE_5;
                    break;
                case 6:
                    current_string = CON.CURRENT_PL_DATA.ZONE_6;
                    break;
                case 7:
                    current_string = CON.CURRENT_PL_DATA.ZONE_7;
                    break;
                case 8:
                    current_string = CON.CURRENT_PL_DATA.ZONE_8;
                    break;
                case 9:
                    current_string = CON.CURRENT_PL_DATA.ZONE_9;
                    break;
                case 10:
                    current_string = CON.CURRENT_PL_DATA.ZONE_10;
                    break;
                case 11:
                    current_string = CON.CURRENT_PL_DATA.ZONE_11;
                    break;
                case 12:
                    current_string = CON.CURRENT_PL_DATA.ZONE_12;
                    break;
                case 13:
                    current_string = CON.CURRENT_PL_DATA.ZONE_13;
                    break;
                case 14:
                    current_string = CON.CURRENT_PL_DATA.ZONE_14;
                    break;
                case 15:
                    current_string = CON.CURRENT_PL_DATA.ZONE_15;
                    break;
                case 16:
                    current_string = CON.CURRENT_PL_DATA.ZONE_16;
                    break;
            }
            return current_string;
        }
    }
}