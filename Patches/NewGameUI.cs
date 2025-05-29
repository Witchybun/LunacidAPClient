using System;
using System.Collections.Generic;
using System.Reflection;
using BepInEx.Logging;
using HarmonyLib;
using LunacidAP.Data;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
namespace LunacidAP
{
    public class NewGameUI
    {
        private static ManualLogSource _log;
        private static GameObject HostEntry;
        private static GameObject PortEntry;
        private static GameObject PasswordEntry;
        private static int _currentSave;
        public static NewGameUI UI;

        public static readonly Dictionary<Goal, string> GoalToString = new(){
            {Goal.AnyEnding, "Any"},
            {Goal.EndingA, "A"},
            {Goal.EndingB, "B"},
            {Goal.EndingCD, "CD"},
            {Goal.EndingE, "E"},
        };

        public static readonly Dictionary<Dropsanity, string> DropsanityToString = new(){
            {Dropsanity.Off, "OFF"},
            {Dropsanity.Unique, "UNQ"},
            {Dropsanity.Randomized, "RAN"}
        };

        public static readonly Dictionary<int, int> StartingClassToLevel = new(){
            {0, 5},
            {1, 10},
            {2, 7},
            {3, 9},
            {4, 8},
            {5, 6},
            {6, 8},
            {7, 9},
            {8, 1},
        };

        public NewGameUI(ManualLogSource log)
        {
            _log = log;
            Harmony.CreateAndPatchAll(typeof(NewGameUI));
        }


        public void ModifyCharCreateForArchipelago()
        {

            var create = GameObject.FindObjectOfType<Menus>().transform.GetChild(4).GetChild(2);
            var textField = create.GetChild(6).gameObject;

            HostEntry = GameObject.Instantiate(textField);
            HostEntry.name = "Host";
            var hostInputComponent = HostEntry.GetComponent<TMP_InputField>();
            hostInputComponent.placeholder.GetComponent<TMP_Text>().text = "input hostname";
            hostInputComponent.characterLimit = 30;
            HostEntry.transform.position = new Vector3(62.0523f, 14.18f, -66.08f);
            HostEntry.transform.SetParent(create);
            HostEntry.transform.localScale = new Vector3(1f, 0.8f, 1f);

            PortEntry = GameObject.Instantiate(textField);
            PortEntry.name = "Port";
            var portInputComponent = PortEntry.GetComponent<TMP_InputField>();
            portInputComponent.placeholder.GetComponent<TMP_Text>().text = "input port";
            portInputComponent.characterLimit = 30;
            PortEntry.transform.position = new Vector3(62.0523f, 14.081f, -66.08f);
            PortEntry.transform.SetParent(create);
            PortEntry.transform.localScale = new Vector3(1f, 0.8f, 1f);

            PasswordEntry = GameObject.Instantiate(textField);
            PasswordEntry.name = "Password";
            var passwordInputComponent = PasswordEntry.GetComponent<TMP_InputField>();
            passwordInputComponent.placeholder.GetComponent<TMP_Text>().text = "input password";
            passwordInputComponent.characterLimit = 30;
            PasswordEntry.transform.position = new Vector3(62.0523f, 13.9689f, -66.08f);
            PasswordEntry.transform.SetParent(create);
            PasswordEntry.transform.localScale = new Vector3(1f, 0.8f, 1f);
        }

        [HarmonyPatch(typeof(Menus), "Click")]
        [HarmonyPrefix]
        private static bool Click_GatherData(int which, Menus __instance)
        {
            var eqSlot2Field = __instance.GetType().GetField("EQ_SLOT2", BindingFlags.Instance | BindingFlags.NonPublic);
            var EQ_SLOT2 = (int)eqSlot2Field.GetValue(__instance);
            var eqselField = __instance.GetType().GetField("EQ_SEL", BindingFlags.Instance | BindingFlags.NonPublic);
            var EQ_SEL = (int)eqselField.GetValue(__instance);
            var loadTextMethod = __instance.GetType().GetMethod("LoadText", BindingFlags.Instance | BindingFlags.NonPublic);
            if (which == 51 && (__instance.current_query == 6 || __instance.current_query == 0)) // Loading a save from main menu
            {
                var eqSlotField = __instance.GetType().GetField("EQ_SLOT", BindingFlags.Instance | BindingFlags.NonPublic);
                var EQ_SLOT = (int)eqSlotField.GetValue(__instance);
                PlayerPrefs.SetInt("CURRENT_SAVE", EQ_SLOT);
                var saveSlot = PlayerPrefs.GetInt("CURRENT_SAVE", EQ_SLOT);
                _currentSave = saveSlot;
                if (EQ_SLOT2 != 10 && !StaticFuncs.IS_NULL(OfferSaveInfo(__instance.CON, saveSlot))) // Some menu instances are similar but not quite
                {
                    SaveHandler.ReadSave(saveSlot);
                    ArchipelagoClient.AP.Connect(ConnectionData.SlotName, ConnectionData.HostName, ConnectionData.Port, ConnectionData.Password, false, slotID: saveSlot);
                }
                return true;
            }
            else if (which == 55 && __instance.current_query == 0) // Player has died but wants to resume.  Need some feedback.
            {
                if (ArchipelagoClient.AP.IsConnecting)
                {
                    return false;
                }
                if (ArchipelagoClient.AP.Authenticated)
                {
                    return true;
                }
                else
                {
                    _log.LogInfo($"Reloading {_currentSave}");
                    SaveHandler.ReadSave(_currentSave);
                    ArchipelagoClient.AP.AttemptConnectFromDeath(_currentSave);
                    return false; // don't let the player play.
                }
            }
            else if (which == 28) // Query for deleting save data
            {
                if (__instance.current_query == 0) // saving in-game
                {
                    SaveHandler.SaveData(_currentSave);
                }
                else if (__instance.current_query == 5) // deleting a save
                {
                    var eqSlotField = __instance.GetType().GetField("EQ_SLOT", BindingFlags.Instance | BindingFlags.NonPublic);
                    var EQ_SLOT = (int)eqSlotField.GetValue(__instance);
                    PlayerPrefs.SetInt("CURRENT_SAVE", EQ_SLOT);
                    var saveSlot = PlayerPrefs.GetInt("CURRENT_SAVE", EQ_SLOT);
                    _currentSave = saveSlot;
                    ConnectionData.WriteConnectionData();
                    SaveHandler.SaveData(saveSlot);
                }
                else if (__instance.current_query == 6) // Trying to load save fully but connection isn't made yet
                {
                    if (!ArchipelagoClient.AP.Authenticated)
                    {
                        return false;
                    }
                }
            }
            else if (which == 29 && __instance.current_query == 6) // Say no to a load at menu
            {
                ArchipelagoClient.AP.Disconnect();
            }
            else if (which == 37)
            {
                var audioMethod = __instance.GetType().GetMethod("Audio", BindingFlags.Instance | BindingFlags.NonPublic);
                var slotName = __instance.ITEMS[20].GetComponent<TMP_InputField>().text;
                var hostName = GameObject.Find("PLAYER").transform.GetChild(1).GetChild(0).GetChild(4).GetChild(2).GetChild(27).gameObject.GetComponent<TMP_InputField>().text;
                var portName = GameObject.Find("PLAYER").transform.GetChild(1).GetChild(0).GetChild(4).GetChild(2).GetChild(28).gameObject.GetComponent<TMP_InputField>().text;
                var port = int.Parse(portName);
                var password = GameObject.Find("PLAYER").transform.GetChild(1).GetChild(0).GetChild(4).GetChild(2).GetChild(29).gameObject.GetComponent<TMP_InputField>().text;
                if (StaticFuncs.IS_NULL(slotName) || StaticFuncs.IS_NULL(hostName) || StaticFuncs.IS_NULL(portName))
                {
                    audioMethod.Invoke(__instance, new object[] { 5 });
                    return false;
                }
                if (!ArchipelagoClient.AP.Authenticated)
                {
                    _log.LogInfo($"Telling the game to save AP data at {_currentSave}");
                    ArchipelagoClient.AP.Connect(slotName, hostName, port, password, false, _currentSave);
                    return false;
                }
                __instance.sub_menu = 9;
                __instance.LoadSub();
                __instance.TXT[31].text = "Finalize Creation?";
                __instance.current_query = 1;
                EventSystem.current.SetSelectedGameObject(__instance.ITEMS[10]);
                audioMethod.Invoke(__instance, new object[] { 8 });
                _log.LogInfo($"Saving to {_currentSave}");
                SaveHandler.SaveData(_currentSave);
            }
            else if (which == 41)
            {
                ShopHandler.EnsureEnchantedKey();
            }
            return true;
        }

        [HarmonyPatch(typeof(Menus), "Click")]
        [HarmonyPostfix]
        private static void Click_FixDueToPronouns_Postfix(int which, Menus __instance)
        {
            if (which == 83 || which == 84)
            {
                FixConnectionInfoDisplay();
            }
        }

        private static string OfferSaveInfo(CONTROL control, int saveSlot)
        {
            switch (saveSlot)
            {
                case 0:
                    return control.CURRENT_SYS_DATA.SAVE0_INFO;
                case 1:
                    return control.CURRENT_SYS_DATA.SAVE1_INFO;
                case 2:
                    return control.CURRENT_SYS_DATA.SAVE2_INFO;
            }
            return control.CURRENT_SYS_DATA.SAVE0_INFO;

        }

        [HarmonyPatch(typeof(Menus), "Update")]
        [HarmonyPostfix]
        private static void Update_ChangeMenuStateToNormalIfOnline(Menus __instance)
        {
            var sceneName = __instance.gameObject.scene.name;
            switch (sceneName)
            {
                case "CHAR_CREATE":
                    {
                        var create = GameObject.FindObjectOfType<Menus>().transform.GetChild(4).GetChild(2);
                        if (!ArchipelagoClient.AP.Authenticated && GameObject.FindObjectOfType<Menus>().transform.GetChild(4).GetChild(2).GetChild(18).gameObject.activeSelf)
                        {
                            var names = create.Find("Labels");
                            names.GetComponent<TextMeshProUGUI>().GetComponent<TMP_Text>().text = "SLOT NAME";
                            names.Find("B").GetComponent<TextMeshProUGUI>().GetComponent<TMP_Text>().text = "HOST";
                            names.Find("C").GetComponent<TextMeshProUGUI>().GetComponent<TMP_Text>().text = "PORT";
                            names.Find("D").GetComponent<TextMeshProUGUI>().GetComponent<TMP_Text>().text = "PASSWORD";
                            HostEntry.SetActive(value: true);
                            PortEntry.SetActive(value: true);
                            PasswordEntry.SetActive(value: true);
                            create.GetChild(13).GetComponent<TextMeshProUGUI>().text = "Archipelago\nsettings";
                            create.GetChild(14).gameObject.SetActive(value: false);
                            create.GetChild(14).GetChild(0).gameObject.SetActive(value: false);
                            create.GetChild(14).GetChild(1).gameObject.SetActive(value: false);
                            create.GetChild(15).gameObject.SetActive(value: false);
                            create.GetChild(15).GetChild(0).gameObject.SetActive(value: false);
                            create.GetChild(15).GetChild(1).gameObject.SetActive(value: false);
                            create.GetChild(16).gameObject.SetActive(value: false);
                            create.GetChild(16).GetChild(0).gameObject.SetActive(value: false);
                            create.GetChild(16).GetChild(1).gameObject.SetActive(value: false);
                            create.GetChild(19).gameObject.SetActive(value: false);
                            create.GetChild(20).gameObject.SetActive(value: false);
                            create.GetChild(21).gameObject.SetActive(value: false);
                            create.GetChild(22).gameObject.SetActive(value: false);
                            create.GetChild(23).gameObject.SetActive(value: false);
                            create.GetChild(17).gameObject.SetActive(value: false);
                            create.GetChild(18).gameObject.SetActive(value: false);
                            create.GetChild(26).gameObject.SetActive(value: false);
                            create.GetChild(10).gameObject.SetActive(value: false);
                            create.GetChild(11).gameObject.SetActive(value: false);
                            create.GetChild(7).GetChild(0).GetComponent<TextMeshProUGUI>().text = "Connect";
                            create.GetChild(8).gameObject.SetActive(value: false);
                            create.GetChild(9).gameObject.SetActive(value: false);
                            return;
                        }
                        else if (ArchipelagoClient.AP.Authenticated && PortEntry.activeSelf)
                        {

                            var names = create.Find("Labels");
                            names.GetComponent<TextMeshProUGUI>().GetComponent<TMP_Text>().text = "NAME";
                            names.Find("B").GetComponent<TextMeshProUGUI>().GetComponent<TMP_Text>().text = "BEAUTY";
                            names.Find("C").GetComponent<TextMeshProUGUI>().GetComponent<TMP_Text>().text = "CLASS";
                            names.Find("D").GetComponent<TextMeshProUGUI>().GetComponent<TMP_Text>().text = "PRONOUNS";
                            HostEntry.SetActive(value: false);
                            PortEntry.SetActive(value: false);
                            PasswordEntry.SetActive(value: false);
                            create.GetChild(18).gameObject.SetActive(value: true);
                            create.GetChild(11).gameObject.SetActive(value: true);
                            var count = 0;
                            var button = create.GetChild(11).GetComponent<UnityEngine.UI.Button>();
                            var classCount = Math.Min(8, ArchipelagoClient.AP.SlotData.StartingClass); // Random utilizes Forsaken's slot anyway.
                            if (ArchipelagoClient.AP.SlotData.StartingClass == 9)
                            {
                                ConnectionData.StoredLevel = ArchipelagoClient.AP.SlotData.CustomStats["Level"];
                            }
                            else
                            {
                                ConnectionData.StoredLevel = StartingClassToLevel[ArchipelagoClient.AP.SlotData.StartingClass];
                            }
                            while (count < classCount)
                            {
                                button.onClick.Invoke();
                                count += 1;
                            }
                            if (ArchipelagoClient.AP.SlotData.StartingClass == 9)
                            {
                                create.GetChild(18).GetComponent<TextMeshProUGUI>().text = ArchipelagoClient.AP.SlotData.CustomName;
                                create.GetChild(24).GetComponent<TextMeshProUGUI>().text = ArchipelagoClient.AP.SlotData.CustomDescription;
                            }
                            create.GetChild(11).gameObject.SetActive(value: false);
                            create.GetChild(14).gameObject.SetActive(value: true);
                            create.GetChild(14).GetComponent<TextMeshProUGUI>().text = "ENDING\nSHOPS\nDROPS\nSWITCH\nDOOR";
                            create.GetChild(15).gameObject.SetActive(value: true);
                            create.GetChild(15).GetComponent<TextMeshProUGUI>().text = "SECRET\nER\nELEMENT\nREQ COIN";
                            create.GetChild(16).gameObject.SetActive(value: true);
                            create.GetChild(16).GetComponent<TextMeshProUGUI>().text = "ETNA\nQUENCH\nNORMDROP\nDEATH";
                            create.GetChild(19).gameObject.SetActive(value: true);
                            create.GetChild(19).GetComponent<TextMeshProUGUI>().text = $"{Ending()}\n{Shopsanity()}\n{Drops()}\n{Switchlock()}\n{Doorlock()}";
                            create.GetChild(20).gameObject.SetActive(value: true);
                            create.GetChild(20).GetComponent<TextMeshProUGUI>().text = $"{Secretdoor()}\n{(ER())}\n{Elements()}\n{RequiredCoins()}";
                            create.GetChild(21).gameObject.SetActive(value: true);
                            create.GetChild(21).GetComponent<TextMeshProUGUI>().text = $"{Etna()}\n{Quench()}\n{NormalizedDrops()}\n{Death()}";
                            create.GetChild(8).gameObject.SetActive(value: true);
                            create.GetChild(9).gameObject.SetActive(value: true);
                            create.GetChild(17).gameObject.SetActive(value: true);
                            create.GetChild(26).gameObject.SetActive(value: true);
                            create.GetChild(7).GetChild(0).GetComponent<TextMeshProUGUI>().text = "Confirm";
                        }

                        return;
                    }
                case "Gameover":
                    {
                        var load = GameObject.FindObjectOfType<Menus>().transform.GetChild(3).GetChild(0);
                        if (!ArchipelagoClient.AP.Authenticated)
                        {
                            load.GetChild(0).GetComponent<TextMeshProUGUI>().text = "connect";
                        }
                        else
                        {
                            load.GetChild(0).GetComponent<TextMeshProUGUI>().text = "load";

                        }
                        return;
                    }
            }

        }

        [HarmonyPatch(typeof(CONTROL), "SkillSet")]
        [HarmonyPostfix]
        private static void SkillSet_RestoreCustomStats(CONTROL __instance)
        {
            if (ArchipelagoClient.AP.SlotData.StartingClass != 9)
            {
                return;
            }
            __instance.NORMAL_MULT = ArchipelagoClient.AP.SlotData.CustomStats["Normal Res"] / 100f;
            __instance.FIRE_MULT = ArchipelagoClient.AP.SlotData.CustomStats["Fire Res"] / 100f;
            __instance.ICE_MULT = ArchipelagoClient.AP.SlotData.CustomStats["Ice Res"] / 100f;
            __instance.POISON_MULT = ArchipelagoClient.AP.SlotData.CustomStats["Poison Res"] / 100f;
            __instance.LIGHT_MULT = ArchipelagoClient.AP.SlotData.CustomStats["Light Res"] / 100f;
            __instance.DARK_MULT = ArchipelagoClient.AP.SlotData.CustomStats["Dark Res"] / 100f;
        }

        [HarmonyPatch(typeof(Menus), "LoadText")]
        [HarmonyPostfix]
        private static void LoadText_FixCustomName(Menus __instance, int text2load)
        {
            if (!ArchipelagoClient.AP.Authenticated)
            {
                return;
            }
            if (text2load != 0)
            {
                return;
            }
            var className = __instance.TXT[1].text.Split(' ')[2];
            if (ArchipelagoClient.AP.SlotData.StartingClass == 9)
            {
                className = ArchipelagoClient.AP.SlotData.CustomName.ToUpper();
            }
            var level = __instance.CON.CURRENT_PL_DATA.PLAYER_LVL.ToString();
            if (ArchipelagoClient.AP.SlotData.Levelsanity)
            {
                level = level + " " + $"(Stored {ConnectionData.StoredLevel.ToString()})";
            }
            __instance.TXT[1].text = "LEVEL " + level + ": " + className;

        }

        private static void FixConnectionInfoDisplay()
        {
            var create = GameObject.FindObjectOfType<Menus>().transform.GetChild(4).GetChild(2);
            if (ArchipelagoClient.AP.SlotData.StartingClass == 9)
            {
                create.GetChild(18).GetComponent<TextMeshProUGUI>().text = ArchipelagoClient.AP.SlotData.CustomName;
                create.GetChild(24).GetComponent<TextMeshProUGUI>().text = ArchipelagoClient.AP.SlotData.CustomDescription;
            }
            create.GetChild(14).GetComponent<TextMeshProUGUI>().text = "ENDING\nSHOPS\nDROPS\nSWITCH\nDOOR";
            create.GetChild(15).GetComponent<TextMeshProUGUI>().text = "SECRET\nER\nELEMENT\nREQ COIN";
            create.GetChild(16).GetComponent<TextMeshProUGUI>().text = "ETNA\nQUENCH\nNORMDROP\nDEATH";
            create.GetChild(19).GetComponent<TextMeshProUGUI>().text = $"{Ending()}\n{Shopsanity()}\n{Drops()}\n{Switchlock()}\n{Doorlock()}";
            create.GetChild(20).GetComponent<TextMeshProUGUI>().text = $"{Secretdoor()}\n{(ER())}\n{Elements()}\n{RequiredCoins()}";
            create.GetChild(21).GetComponent<TextMeshProUGUI>().text = $"{Etna()}\n{Quench()}\n{NormalizedDrops()}\n{Death()}";

        }

        [HarmonyPatch(typeof(Menus), "Click")]
        [HarmonyPostfix]
        public static void Click_InitializeCustom(Menus __instance, int which)
        {
            if (which != 28 || __instance.current_query != 1)
            {
                return;
            }
            if (ArchipelagoClient.AP.SlotData.StartingClass != 9)
            {
                return;
            }
            var control = GameObject.Find("CONTROL").GetComponent<CONTROL>();
            var statData = ArchipelagoClient.AP.SlotData.CustomStats;
            control.CURRENT_PL_DATA.PLAYER_LVL = statData["Level"];
            control.CURRENT_PL_DATA.PLAYER_STR = statData["Strength"];
            control.CURRENT_PL_DATA.PLAYER_DEF = statData["Defense"];
            control.CURRENT_PL_DATA.PLAYER_SPD = statData["Speed"];
            control.CURRENT_PL_DATA.PLAYER_DEX = statData["Dexterity"];
            control.CURRENT_PL_DATA.PLAYER_INT = statData["Intelligence"];
            control.CURRENT_PL_DATA.PLAYER_RES = statData["Resistance"];

            control.CURRENT_PL_DATA.PLAYER_H = control.PLAYER_MAX_HP;
            control.CURRENT_PL_DATA.PLAYER_B = control.PLAYER_MAX_HP;
            control.CURRENT_PL_DATA.PLAYER_M = control.PLAYER_MAX_MP;
            control.CURRENT_PL_DATA.PLAYER_L = 0f;


            control.NORMAL_MULT = statData["Normal Res"] / 100f;
            control.FIRE_MULT = statData["Fire Res"] / 100f;
            control.ICE_MULT = statData["Ice Res"] / 100f;
            control.POISON_MULT = statData["Poison Res"] / 100f;
            control.LIGHT_MULT = statData["Light Res"] / 100f;
            control.DARK_MULT = statData["Dark Res"] / 100f;
            //To double ensure a reload is okay
            Save.SAVE_FILE(PlayerPrefs.GetInt("CURRENT_SAVE", 0), new Vector3(0.1f, 1f, 0f), __instance.CON);
            Hold_Data.HD = __instance.CON.CURRENT_PL_DATA;
        }

        private static string Ending()
        {
            return GoalToString[ArchipelagoClient.AP.SlotData.Ending];
        }

        private static string Shopsanity()
        {
            return ArchipelagoClient.AP.SlotData.Shopsanity ? "Y" : "N";
        }

        private static string Drops()
        {
            return DropsanityToString[ArchipelagoClient.AP.SlotData.Dropsanity];
        }

        private static string Switchlock()
        {
            return ArchipelagoClient.AP.SlotData.Switchlock ? "Y" : "N";
        }

        private static string Doorlock()
        {
            return ArchipelagoClient.AP.SlotData.Doorlock ? "Y" : "N";
        }

        private static string Secretdoor()
        {
            return ArchipelagoClient.AP.SlotData.FalseWalls ? "Y" : "N";
        }

        private static string ER()
        {
            return ArchipelagoClient.AP.SlotData.EntranceRandomizer ? "Y" : "N";
        }

        private static string Elements()
        {
            return ArchipelagoClient.AP.SlotData.RandomElements ? "Y" : "N";
        }

        private static string RequiredCoins()
        {
            return ArchipelagoClient.AP.SlotData.RequiredCoins.ToString();
        }

        private static string NormalizedDrops()
        {
            return ArchipelagoClient.AP.SlotData.NormalizedDrops ? "Y" : "N";
        }

        private static string Death()
        {
            return ArchipelagoClient.AP.SlotData.DeathLink ? "Y" : "N";
        }

        private static string Etna()
        {
            return ArchipelagoClient.AP.SlotData.EtnasPupil ? "Y" : "N";
        }

        private static string Quench()
        {
            return ArchipelagoClient.AP.SlotData.Quenchsanity ? "Y" : "N";
        }
    }
}