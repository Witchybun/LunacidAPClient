using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Archipelago.MultiClient.Net.Enums;
using Archipelago.MultiClient.Net.Models;
using BepInEx.Logging;
using HarmonyLib;
using LunacidAP.Data;
using UnityEngine;
using UnityEngine.Networking.Match;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements.Experimental;

namespace LunacidAP
{
    public class ReadDialogueHelper
    {
        private static ManualLogSource _log;
        public static void Awake(ManualLogSource log)
        {
            _log = log;
            Harmony.CreateAndPatchAll(typeof(ReadDialogueHelper));
        }

        [HarmonyPatch(typeof(LoreBlock), "ACT")]
        [HarmonyPrefix]
        private static bool ACT_ExchangeLoreForHelp(LoreBlock __instance)
        {
            var scene = SceneManager.GetActiveScene().name;
            switch (scene)
            {
                case "PITT_A1":
                    {
                        switch (__instance.which)
                        {
                            case 0: // Hollow Basin Hints
                                {
                                    __instance.test_string = DetermineCheckedLocations("PITT_A1", "Hollow Basin", 0);
                                    break;
                                }
                            case 2: // Goal Hint
                                {
                                    __instance.test_string = LoreDialogue.GoalHint[SlotData.Ending];
                                    break;
                                }
                            case 14: // Temple of Silence Hints
                                {
                                    __instance.test_string = DetermineCheckedLocations("PITT_A1", "Temple of Silence", 1);
                                    break;
                                }
                        }
                        return true;
                    }
                case "HUB_01":
                    {
                        switch (__instance.which)
                        {
                            case 1:
                                {
                                    __instance.test_string = DetermineCheckedLocations("HUB_01", "Wing's Rest");
                                    break;
                                }
                            case 75:
                                {
                                    __instance.test_string = DetermineRequestedHints();
                                    break;
                                }
                        }
                        return true;
                    }
                case "ARCHIVES":
                    {
                        switch (__instance.which)
                        {
                            case 0:
                                {
                                    __instance.test_string = DetermineCheckedLocations("ARCHIVES", "Forbidden Archives First Floor", 2);
                                    break;
                                }
                            case 1:
                                {
                                    __instance.test_string = DetermineCheckedLocations("ARCHIVES", "Forbidden Archives Second Floor");
                                    break;
                                }
                            case 2:
                                {
                                    __instance.test_string = DetermineCheckedLocations("ARCHIVES", "Forbidden Archives Third Floor", 1);
                                    break;
                                }
                        }
                        return true;
                    }
                case "SEWER_A1":
                    {
                        switch (__instance.which)
                        {
                            case 0:
                                {
                                    __instance.test_string = DetermineCheckedLocations("SEWER_A1", "The Fetid Mire");
                                    break;
                                }
                            case 1:
                                {
                                    __instance.test_string = DetermineCheckedLocations("SEWER_A1", "Underworks", 1);
                                    break;
                                }
                        }
                        return true;
                    }
                case "FOREST_A1":
                    {
                        switch (__instance.which)
                        {
                            case 4:
                                {
                                    __instance.test_string = DetermineCheckedLocations("FOREST_A1", "Yosei Forest");
                                    break;
                                }
                            case 29:
                                {
                                    __instance.test_string = DetermineCheckedLocations("FOREST_A1", "Yosei Forest Depths", 1);
                                    break;
                                }
                        }
                        return true;
                    }
                case "FOREST_B1":
                    {
                        switch (__instance.which)
                        {
                            case 40:
                                {
                                    __instance.test_string = DetermineCheckedLocations("FOREST_B1", "Forest Canopy");
                                    break;
                                }
                        }
                        return true;
                    }
                case "WALL_01":
                    {
                        switch (__instance.which)
                        {
                            case 52:
                                {
                                    __instance.test_string = DetermineCheckedLocations("WALL_01", "Laetus Chasm");
                                    break;
                                }
                        }
                        return true;
                    }
                case "HAUNT":
                    {
                        switch (__instance.which)
                        {
                            case 19:
                                {
                                    __instance.test_string = DetermineCheckedLocations("HAUNT", "Accursed Tomb Crypts");
                                    break;
                                }
                            case 20:
                                {
                                    __instance.test_string = DetermineCheckedLocations("HAUNT", "Accursed Tomb", 2);
                                    break;
                                }
                            case 22:
                                {
                                    __instance.test_string = DetermineCheckedLocations("HAUNT", "Mausoleum", 1);
                                    break;
                                }
                        }
                        return true;
                    }
                case "LAKE":
                    {
                        switch (__instance.which)
                        {
                            case 0:
                                {
                                    __instance.test_string = DetermineCheckedLocations("LAKE", "Sanguine Sea");
                                    break;
                                }
                        }
                        return true;
                    }
                case "CAS_1":
                    {
                        switch (__instance.which)
                        {
                            case 0:
                                {
                                    __instance.test_string = DetermineCheckedLocations("CAS_1", "Castle Le Fanu Lower Floor", 1);
                                    break;
                                }
                            case 1:
                                {
                                    __instance.test_string = DetermineCheckedLocations("CAS_1", "Castle Le Fanu Upper Floor", 2);
                                    break;
                                }
                            case 42:
                                {
                                    __instance.test_string = DetermineCheckedLocations("CAS_1", "Cattle Cells");
                                    break;
                                }
                        }
                        return true;
                    }
                case "CAS_3":
                    {
                        switch (__instance.which)
                        {
                            case 54:
                                {
                                    __instance.test_string = DetermineCheckedLocations("CAS_3", "Sealed Ballroom");
                                    break;
                                }
                        }
                        return true;
                    }
                case "CAVE":
                    {
                        switch (__instance.which)
                        {
                            case 0:
                                {
                                    __instance.test_string = DetermineCheckedLocations("CAVE", "Sand Temple", 1);
                                    break;
                                }
                            case 60:
                                {
                                    __instance.test_string = DetermineCheckedLocations("CAVE", "Boiling Grotto");
                                    break;
                                }
                        }
                        return true;
                    }
                case "TOWER":
                    {
                        switch (__instance.which)
                        {
                            case 61:
                                {
                                    __instance.test_string = DetermineCheckedLocations("TOWER", "Tower of Abyss");
                                    break;
                                }
                        }
                        return true;
                    }
                case "PRISON":
                    {
                        switch (__instance.which)
                        {
                            case 69:
                                {
                                    __instance.test_string = DetermineCheckedLocations("PRISON", "Terminus Prison Second Floor", 1);
                                    break;
                                }
                            case 70:
                                {
                                    __instance.test_string = DetermineCheckedLocations("PRISON", "Terminus Prison Fourth Floor", 4);
                                    break;
                                }
                            case 84:
                                {
                                    __instance.test_string = DetermineCheckedLocations("PRISON", "Terminus Prison Second Basement", 3);
                                    break;
                                }
                            case 89:
                                {
                                    __instance.test_string = DetermineCheckedLocations("PRISON", "Terminus Prison First Floor", 2);
                                    break;
                                }
                            case 90:
                                {
                                    __instance.test_string = DetermineCheckedLocations("PRISON", "Terminus Prison Third Floor");
                                    break;
                                }
                        }
                        return true;
                    }
                case "VOID":
                    {
                        switch (__instance.which)
                        {
                            case 76:
                                {
                                    __instance.test_string = DetermineCheckedLocations("VOID", "Labyrinth of Ash");
                                    break;
                                }
                        }
                        return true;
                    }
                case "ARENA":
                    {
                        switch (__instance.which)
                        {
                            case 0:
                                {
                                    __instance.test_string = DetermineCheckedLocations("ARENA", "Temple of Water", 1);
                                    break;
                                }
                            case 1:
                                {
                                    __instance.test_string = DetermineCheckedLocations("ARENA", "Forlorn Arena and Temple of Earth");
                                    break;
                                }
                        }
                        return true;
                    }
            }
            return true;
        }

        private static string DetermineCheckedLocations(string scene, string title, int spacerSpot = 0)
        {
            string completedMessage = $"{title}'s Locations\n";
            int spacerCount = 0;
            foreach (var locationData in LunacidLocations.APLocationData[scene])
            {
                if (locationData.APLocationName == "Spacer")
                {
                    spacerCount += 1;
                    continue;
                }
                if (spacerCount != spacerSpot)
                {
                    continue;
                }
                var isLocationChecked = ArchipelagoClient.AP.IsLocationChecked(locationData.APLocationID) ? "<sprite=4>" : "";
                completedMessage += isLocationChecked + locationData.APLocationName + isLocationChecked + "\n";
            }
            return completedMessage;
        }

        private static string DetermineRequestedHints()
        {
            var hintString = "Clint's Hintbook \n (Be sure to pick up anything in here!) \n \n";
            var hints = ArchipelagoClient.AP.Session.DataStorage.GetHints(ArchipelagoClient.AP.SlotID);
            var goodHints = new List<Hint>() { };
            foreach (var hint in hints)
            {
                if (!hint.Found && hint.ItemFlags.HasFlag(ItemFlags.Advancement))
                {
                    goodHints.Add(hint);
                }
            }
            if (goodHints.Count() == 0)
            {
                return hintString + "Nothing to do; good job!";
            }
            foreach (var hint in goodHints)
            {
                var item = ArchipelagoClient.AP.LocationTable[hint.LocationId];
                var locationName = ArchipelagoClient.AP.GetLocationNameFromID(hint.LocationId);
                hintString += item.SlotName + " wants " + item.Name + " at " + locationName + "\n";
            }
            return hintString;

        }

        public static void InstantiateSignsForHints(string sceneName)
        {

            switch (sceneName)
            {
                case "ARENA":
                    {
                        var dummyStone = GameObject.Find("ARENA").transform.GetChild(6).GetChild(5);

                        var waterStone = GameObject.Instantiate(dummyStone);
                        waterStone.name = "Water Stone";
                        waterStone.SetParent(dummyStone.parent);
                        waterStone.position = new Vector3(-2.2389f, 6f, -9.0245f);
                        waterStone.localScale = new Vector3(1f, 1f, 1f);
                        waterStone.Rotate(new Vector3(0f, 40f, 0f));
                        waterStone.GetChild(0).GetComponent<LoreBlock>().which = 0;
                        waterStone.gameObject.SetActive(value: true);

                        var earthStone = GameObject.Instantiate(dummyStone);
                        waterStone.name = "Earth Stone";
                        earthStone.SetParent(dummyStone.parent);
                        earthStone.position = new Vector3(-1.979f, 6f, 9.3064f);
                        earthStone.localScale = new Vector3(1f, 1f, 1f);
                        earthStone.Rotate(new Vector3(0f, 140f, 0f));
                        earthStone.GetChild(0).GetComponent<LoreBlock>().which = 1;
                        earthStone.gameObject.SetActive(value: true);

                        break;
                    }
                case "SEWER_A1":
                    {
                        var dummySign = GameObject.Find("SEWER").transform.GetChild(0).GetChild(6);

                        var restAreaSign = GameObject.Instantiate(dummySign);
                        restAreaSign.name = "Rest Sign";
                        restAreaSign.SetParent(dummySign.parent);
                        restAreaSign.position = new Vector3(-50.7244f, 0f, -127.7889f);
                        restAreaSign.localScale = new Vector3(1f, 1f, 0.7f);
                        restAreaSign.GetChild(0).GetComponent<LoreBlock>().which = 0;
                        restAreaSign.gameObject.SetActive(value: true);

                        var sewerSign = GameObject.Instantiate(dummySign);
                        sewerSign.name = "Sewer Sign";
                        sewerSign.SetParent(dummySign.parent);
                        sewerSign.position = new Vector3(14.2067f, -28f, -301.1524f);
                        sewerSign.localScale = new Vector3(1f, 1f, 0.7f);
                        sewerSign.Rotate(new Vector3(0f, -90f, 0f));
                        sewerSign.GetChild(0).GetComponent<LoreBlock>().which = 1;
                        sewerSign.gameObject.SetActive(value: true);

                        break;
                    }
                case "CAS_1":
                    {
                        var dummySign = GameObject.Find("CAS_1").transform.GetChild(10).GetChild(13);

                        var whiteAreaSign = GameObject.Instantiate(dummySign);
                        whiteAreaSign.name = "Lobby Sign";
                        whiteAreaSign.SetParent(dummySign.parent);
                        whiteAreaSign.position = new Vector3(7.9675f, 6.1f, -71.0511f);
                        whiteAreaSign.localScale = new Vector3(1.5f, 1.5f, 1f);
                        whiteAreaSign.Rotate(new Vector3(0f, -195f, 0f));
                        whiteAreaSign.GetChild(0).GetComponent<LoreBlock>().which = 0;
                        whiteAreaSign.gameObject.SetActive(value: true);

                        var blueAreaSign = GameObject.Instantiate(dummySign);
                        blueAreaSign.name = "Upstairs Sign";
                        blueAreaSign.SetParent(dummySign.parent);
                        blueAreaSign.position = new Vector3(22f, 10.1f, -141.9587f);
                        blueAreaSign.localScale = new Vector3(1.5f, 1.5f, 1f);
                        blueAreaSign.Rotate(new Vector3(0f, 45f, 0f));
                        blueAreaSign.GetChild(0).GetComponent<LoreBlock>().which = 1;
                        blueAreaSign.gameObject.SetActive(value: true);

                        break;
                    }
                case "ARCHIVES":
                    {
                        var dummySign = GameObject.Find("ARCHIVES").transform.GetChild(7).GetChild(1);
                        dummySign.Rotate(new Vector3(0f, -70f, 0f));

                        var secondFloorSign = GameObject.Instantiate(dummySign);
                        secondFloorSign.name = "Second Sign";
                        secondFloorSign.SetParent(dummySign.parent);
                        secondFloorSign.position = new Vector3(-52f, -1.8f, 5.7193f);
                        secondFloorSign.localScale = new Vector3(1f, 1f, 1f);
                        secondFloorSign.GetChild(0).GetComponent<LoreBlock>().which = 1;
                        secondFloorSign.gameObject.SetActive(value: true);

                        var firstFloorSign = GameObject.Instantiate(dummySign);
                        firstFloorSign.name = "First Sign";
                        firstFloorSign.SetParent(dummySign.parent);
                        firstFloorSign.position = new Vector3(-52f, -1.8f, -5.2234f);
                        firstFloorSign.localScale = new Vector3(1f, 1f, 1f);
                        firstFloorSign.GetChild(0).GetComponent<LoreBlock>().which = 0;
                        firstFloorSign.gameObject.SetActive(value: true);

                        var thirdFloorSign = GameObject.Instantiate(dummySign);
                        thirdFloorSign.name = "Third Sign";
                        thirdFloorSign.SetParent(dummySign.parent);
                        thirdFloorSign.position = new Vector3(-52f, -1.8f, 8.7193f);
                        thirdFloorSign.localScale = new Vector3(1f, 1f, 1f);
                        thirdFloorSign.GetChild(0).GetComponent<LoreBlock>().which = 2;
                        thirdFloorSign.gameObject.SetActive(value: true);

                        break;
                    }
                case "PITT_B1":
                    {
                        var messages = GameObject.Find("THE_PIT_B1").transform.GetChild(6);
                        messages.gameObject.SetActive(value: true);
                        foreach (Transform child in messages)
                        {
                            child.gameObject.SetActive(value: false);
                        }
                        var mainSign = messages.GetChild(0);
                        mainSign.position = new Vector3(-8.6517f, 0f, 15.9076f);
                        mainSign.Rotate(new Vector3(0f, -180f, 0f));
                        mainSign.gameObject.SetActive(value: true);

                        break;
                    }
                case "LAKE":
                    {
                        var sign = GameObject.Find("BLOOD_LAKE").transform.GetChild(6).GetChild(0);
                        sign.position = new Vector3(189.29f, 1.7339f, -30.1422f);
                        sign.Rotate(new Vector3(0f, -90f, 0f));
                        sign.gameObject.SetActive(value: true);

                        break;
                    }
                // The rest are maps which contain no dynamic batch message objects, and so one must be made.
                case "CAVE":
                    {
                        var cave = GameObject.Find("CAVE").transform;
                        var book = cave.GetChild(7).GetChild(7).GetChild(26);
                        var signReference = cave.GetChild(5).GetChild(1);
                        var messageBlockReference = cave.GetChild(5).GetChild(0);
                        var newBook = GameObject.Instantiate(book);
                        GameObject.Destroy(newBook.GetComponent<OBJ_HEALTH>());
                        GameObject.Destroy(newBook.GetComponent<Rigidbody>());
                        var newBookRead = GameObject.Instantiate(signReference.GetChild(0));
                        newBookRead.SetParent(newBook);
                        newBookRead.GetComponent<LoreBlock>().which = 0;
                        newBookRead.localPosition = new Vector3(0f, 0f, 0f);
                        newBook.position = new Vector3(-304.8741f, 16.4f, -110.9111f);
                        newBookRead.Rotate(new Vector3(0, 130, 42));
                        newBook.Rotate(new Vector3(0, 130, 42));
                        newBook.SetParent(signReference.parent);
                        newBookRead.name = "READ";
                        newBook.name = "Book Hint";

                        break;
                    }
            }
        }

        /*[HarmonyPatch(typeof(Dialog), "Greet")]
        [HarmonyPostfix]
        private static void Greet_RequestHints(Dialog __instance)
        {
            if (__instance.npc_name != "CLIVE" || __instance.LINES[3].value.Contains("Clive"))
            {
                return;
            }
            foreach (var line in __instance.LINES)
            {
                var create = GameObject.Find("PLAYER").transform.GetChild(1).GetChild(0).GetChild(4).GetChild(2);
            var textField = create.GetChild(6).gameObject;

            HostEntry = GameObject.Instantiate(textField);
            HostEntry.name = "Host";
            var hostInputComponent = HostEntry.GetComponent<TMP_InputField>();
            hostInputComponent.placeholder.GetComponent<TMP_Text>().text = "input hostname";
            hostInputComponent.characterLimit = 30;
            HostEntry.transform.position = new Vector3(62.0523f, 14.18f, -66.08f);
            HostEntry.transform.SetParent(create);
            HostEntry.transform.localScale = new Vector3(1f, 0.8f, 1f);
                _log.LogInfo($"{line.value}");
                _log.LogInfo($"NXT: {line.NXT}");
                _log.LogInfo($"special: {line.special}");
                _log.LogInfo($"NEW_SAID: {line.NEW_SAID}");
                _log.LogInfo($"expssion: [{line.exprssion.x}, {line.exprssion.y}, {line.exprssion.z}]");
                _log.LogInfo($"LOAD_LINE: {line.LOAD_LINE}");
            }
        }*/
    }
}