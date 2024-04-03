using System;
using System.Linq;
using BepInEx.Logging;
using HarmonyLib;
using LunacidAP.Data;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace LunacidAP
{
    public class FlagHandler
    {
        private static CONTROL CON;
        private static ManualLogSource _log;
        private static string[] errorData { get; set; }

        public static void Awake(ManualLogSource log)
        {
            _log = log;
            Harmony.CreateAndPatchAll(typeof(FlagHandler));
        }

        public static void HandleFlagGivenItemName(string Name)
        {
            var sceneName = SceneManager.GetActiveScene().name;
            var itemData = LunacidFlags.ItemToFlag[Name];
            ApplyFlag(Name);
            if (sceneName == itemData.Scene)
            {
                RefreshSceneEntities(Name);
            }
        }

        private static void ApplyFlag(string Name)
        {
            var flagData = LunacidFlags.ItemToFlag[Name].Flag;
            if (Name == "Progressive Vampiric Symbol")
            {
                ModifyFlag(flagData[0], flagData[1], Math.Min(3, ConnectionData.Index));
                return;
            }
            if (!DoesPlayerHaveItem(Name))
            {
                ModifyFlag(flagData[0], flagData[1], flagData[2]);
            }
        }

        public static string LoadFlag(int Zone)
        {
            CON = GameObject.Find("CONTROL").GetComponent<CONTROL>();
            var current_string = ZoneDataPicker(Zone);
            return current_string;
        }

        public static int ReadFlagValue(int Zone, int Slot)
        {
            var flag = LoadFlag(Zone);
            char value = flag[Slot];
            return value;

        }

        public static void ModifyFlag(int Zone, int Slot, int Value)
        {
            var current_string = LoadFlag(Zone);
            current_string = current_string.Substring(0, Slot - 1) + Value + current_string.Substring(Slot, current_string.Length - Slot);

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
                var finalValue = __instance.value;
                foreach (var flag in LunacidFlags.MaximumPlotFlags)
                {
                    if (__instance.Zone == flag[0] && __instance.Slot == flag[1])
                    {
                        if (__instance.value > flag[2])
                        {
                            __instance.value = flag[2];
                            __instance.Save(); // Fix it from here on.
                        }
                        finalValue = Math.Min(flag[2], finalValue);
                        break;
                    }
                }
                if (sceneName == "FOREST_A1" && stateController == "PATCHI")
                {
                    var canopyID = ArchipelagoClient.AP.GetLocationIDFromName("YF: Patchouli's Canopy Offer");
                    if (ArchipelagoClient.AP.WasItemReceived("Skull of Josiah") && ArchipelagoClient.AP.IsLocationChecked(canopyID))
                    {
                        if (finalValue <= 5)
                        {
                            finalValue = 5;
                        }
                    }
                }
                for (int i = 0; i < sTATES.Length; i++)
                {
                    errorData[1] = i.ToString();
                    errorData[2] = sTATES[i].name;
                    sTATES[i].SetActive(value: false);
                }
                
                errorData[2] = sTATES[finalValue].name;
                __instance.STATES[finalValue].SetActive(value: true);

                switch (sceneName)
                {
                    case "FOREST_A1":
                        {
                            if (stateController == "PATCHI" && DoesPlayerHaveItem("Skull of Josiah") && !ArchipelagoClient.AP.IsLocationChecked("YF: Patchouli's Reward"))
                            {
                                __instance.transform.GetChild(2).gameObject.SetActive(value: false);
                                __instance.transform.GetChild(3).gameObject.SetActive(value: false);
                                __instance.transform.GetChild(4).gameObject.SetActive(value: false);
                                __instance.transform.GetChild(5).gameObject.SetActive(value: true);
                                __instance.transform.GetChild(6).gameObject.SetActive(value: false);
                            }
                            break;
                        }
                    case "FOREST_B1":
                        {
                            if (stateController == "save2")
                            {
                                var skull = __instance.transform.GetChild(1).gameObject;
                                if (!skull.activeSelf)
                                {
                                    skull.SetActive(value: true);
                                }

                            }
                            break;
                        }
                    case "PRISON":
                        {
                            if (stateController == "GARRAT") // Not super necessary; its just so this doesn't trigger a million times.
                            {
                                var prison = GameObject.Find("PRISON").transform;
                                var prisonKey = prison.GetChild(4).GetChild(6).gameObject;
                                if (!prisonKey.activeSelf)
                                {
                                    prisonKey.SetActive(value: true);
                                }
                                var hammerPickup = prison.GetChild(7).GetChild(3).gameObject;
                                if (!hammerPickup.activeSelf)
                                {
                                    hammerPickup.SetActive(value: true);
                                }
                            }
                            break;
                        }
                    case "PITT_A1":
                        {
                            var pittObjects = GameObject.Find("THE_PIT_A1");
                            var woodenGate = pittObjects.transform.GetChild(3).GetChild(4).gameObject;
                            if (woodenGate.activeSelf)
                            {
                                woodenGate.SetActive(value: false);
                            }
                            if (stateController == "META")
                            {
                                var vhsTape = pittObjects.transform.GetChild(1).GetChild(22).gameObject;
                                var secretID = ArchipelagoClient.AP.GetLocationIDFromName("HB: Temple Hidden Room In Sewer");
                                if (!ArchipelagoClient.AP.IsLocationChecked(secretID))
                                {
                                    vhsTape.SetActive(value: true);
                                }
                            }
                            break;
                        }
                    case "CAS_1":
                        {
                            if (stateController == "SAVE_0")
                            {
                                sTATES[0].SetActive(value: true);
                                sTATES[1].SetActive(value: true);
                                sTATES[2].SetActive(value: true);
                            }
                            else if (stateController == "CAS_DOOR")
                            {
                                var playerClass = GameObject.Find("CONTROL").GetComponent<CONTROL>().CURRENT_PL_DATA.PLAYER_CLASS;
                                _log.LogInfo($"{playerClass}");
                                if (playerClass == "Vampire")
                                {
                                    sTATES[1].SetActive(value: true);
                                }
                                else
                                {
                                    sTATES[3].SetActive(value: true);
                                }
                            }
                            break;
                        }
                    case "ARENA":
                        {
                            if (stateController == "CHEST3" || stateController == "CHEST4")
                            {
                                sTATES[0].SetActive(value: true); // always force the chest in a waiting state.
                                sTATES[1].SetActive(value: false);
                            }
                            break;
                        }
                    case "HAUNT":
                        {
                            if (stateController == "META")
                            {
                                var corrupt = ArchipelagoClient.AP.GetLocationIDFromName("AT: Corrupted Room");
                                if (!ArchipelagoClient.AP.WasItemReceived("Corrupt Key"))
                                {
                                    sTATES[0].SetActive(value: true);
                                }
                                else if (ArchipelagoClient.AP.WasItemReceived("Corrupt Key") && !DoesPlayerHaveItem("Corrupt Key"))
                                {
                                    sTATES[0].SetActive(value: false);
                                }
                            }
                            else if (stateController == "TV")
                            {
                                if (ArchipelagoClient.AP.WasItemReceived("VHS Tape"))
                                {
                                    sTATES[0].SetActive(value: true);
                                    sTATES[1].SetActive(value: true);
                                    sTATES[2].SetActive(value: false);
                                    sTATES[3].SetActive(value: false);
                                }
                            }
                            break;
                        }
                    case "HUB_01":
                        {
                            var book = GameObject.Find("LEVEL").transform.GetChild(9).GetChild(1);
                            if (!book.gameObject.activeSelf)
                            {
                                book.gameObject.SetActive(value: true);
                            }
                            break;
                        }
                        case "ARCHIVES":
                            {
                                if (stateController == "FINAL_STAGES")
                                {
                                    var hasBook3 = __instance.transform.parent.GetChild(8).GetChild(9).gameObject;
                                    if (hasBook3.activeSelf)
                                    {
                                        __instance.transform.GetChild(1).gameObject.SetActive(value: true);
                                        hasBook3.SetActive(value: false);
                                    }
                                }
                                else if (stateController == "STAGES")
                                {
                                    var giveBook3 = __instance.transform.parent.GetChild(7).GetChild(1).gameObject;
                                    if (giveBook3.activeSelf)
                                    {
                                        __instance.transform.GetChild(9).gameObject.SetActive(value: false);
                                        giveBook3.SetActive(value: true);
                                    }
                                }
                                break;
                            }
                }
                return false;
            }
            catch (Exception ex)
            {
                _log.LogError($"Method {nameof(Load_RetainItemPickups)} has failed:");
                _log.LogError($"Name: {errorData[0]}, Iteration: {errorData[1]}, State Name: {errorData[2]}");
                _log.LogError($"State Count: {errorData[3]}, Value: {errorData[4]}");
                _log.LogError($"{ex}");

                return true;
            }
        }

        public static void RefreshSceneEntities(string itemName)
        {
            var sceneName = SceneManager.GetActiveScene().name;
            switch (itemName)
            {
                case "Progressive Vampiric Symbol":
                    {
                        var receivedCount = ArchipelagoClient.AP.Session.Items.AllItemsReceived.Count(x => ArchipelagoClient.AP.Session.Items.GetItemName(x.Item) == "Progressive Vampiric Symbol");
                        Transform map = GameObject.Find(sceneName).transform;
                        if (sceneName == "ARCHIVES")
                        {
                            map.GetChild(3).GetChild(28).GetComponent<AREA_SAVED_ITEM>().Load();
                            break;
                        }
                        switch (receivedCount)
                        {
                            case 1:
                                {
                                    var casDoor = map.GetChild(7).GetChild(10);
                                    casDoor.GetComponent<AREA_SAVED_ITEM>().Load();
                                    casDoor.GetChild(3).GetComponent<AREA_SAVED_ITEM>().Load();
                                    break;
                                }
                            case 2:
                                {
                                    var casDoor2 = map.GetChild(7).GetChild(13);
                                    foreach (Transform child in casDoor2)
                                    {
                                        child.GetComponent<AREA_SAVED_ITEM>().Load();
                                    }
                                    break;
                                }
                            case 3:
                                {
                                    var casDoor2 = map.GetChild(7).GetChild(13);
                                    foreach (Transform child in casDoor2)
                                    {
                                        child.GetComponent<AREA_SAVED_ITEM>().Load();
                                    }
                                    break;
                                }
                        }
                        break;
                    }
                case "VHS Tape":
                    {
                        // The effect really isn't felt in the same map anyway.
                        break;
                    }
                case "White VHS Tape":
                    {
                        // The effect really isn't felt in the same map anyway.
                        break;
                    }
                case "Skull of Josiah":
                    {
                        // The effect really isn't felt in the same map anyway.
                        break;
                    }
                case "Hammer of Cruelty":
                    {
                        var hammerPickup = GameObject.Find("PRISON").transform.GetChild(7).GetChild(4);
                        hammerPickup.GetComponent<AREA_SAVED_ITEM>().Load();
                        break;
                    }
                case "Terminus Prison Key":
                    {
                        var jailDoors = GameObject.Find("PRISON").transform.GetChild(4).GetChild(7);
                        jailDoors.GetComponent<AREA_SAVED_ITEM>().Load();
                        break;
                    }
                case "Water Talisman":
                    {
                        var talisman = GameObject.Find("ARENA").transform.GetChild(4).GetChild(13).GetChild(1);
                        talisman.GetComponent<AREA_SAVED_ITEM>().Load();
                        break;
                    }
                case "Earth Talisman":
                    {
                        var talisman = GameObject.Find("ARENA").transform.GetChild(4).GetChild(13);
                        talisman.GetComponent<AREA_SAVED_ITEM>().Load();
                        break;
                    }
            }
        }

        public static bool DoesPlayerHaveItem(string itemName)
        {
            CON = GameObject.Find("CONTROL").GetComponent<CONTROL>();
            var playerInventory = CON.CURRENT_PL_DATA.ITEMS;
            for (var i = 0; i < 128; i++)
            {
                if (playerInventory[i] == "" || playerInventory[i] == null)
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