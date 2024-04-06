using System.Collections.Generic;
using System.Reflection;
using BepInEx.Logging;
using HarmonyLib;
using LunacidAP.Data;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.UIElements;

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



        public NewGameUI(ManualLogSource log)
        {
            _log = log;
            Harmony.CreateAndPatchAll(typeof(NewGameUI));
        }


        public void ModifyCharCreateForArchipelago()
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
            _log.LogInfo($"Which {which}, Query {__instance.current_query}, EQ_SLOT2 {EQ_SLOT2}");
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
                    __instance.StartCoroutine(ArchipelagoClient.AP.Connect(ConnectionData.SlotName, ConnectionData.HostName, ConnectionData.Port, ConnectionData.Password, false, slotID: saveSlot));
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
                    __instance.StartCoroutine(ArchipelagoClient.AP.Connect(ConnectionData.SlotName, ConnectionData.HostName, ConnectionData.Port, ConnectionData.Password, false, _currentSave));
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
            }
            else if (which == 28 && __instance.current_query == 6) // Trying to load save fully but connection isn't made yet
            {
                if (!ArchipelagoClient.AP.Authenticated)
                {
                    return false;
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
                    __instance.StartCoroutine(ArchipelagoClient.AP.Connect(slotName, hostName, port, password, false, _currentSave));
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
                        if (!ArchipelagoClient.AP.Authenticated && GameObject.Find("PLAYER").transform.GetChild(1).GetChild(0).GetChild(4).GetChild(2).GetChild(18).gameObject.activeSelf)
                        {
                            var create = GameObject.Find("PLAYER").transform.GetChild(1).GetChild(0).GetChild(4).GetChild(2);
                            create.GetChild(12).GetComponent<TextMeshProUGUI>().GetComponent<TMP_Text>().text = "SLOT NAME\nHOST\nPORT\nPASSWORD";
                            HostEntry.SetActive(value: true);
                            PortEntry.SetActive(value: true);
                            PasswordEntry.SetActive(value: true);
                            create.GetChild(8).gameObject.SetActive(value: false);
                            create.GetChild(9).gameObject.SetActive(value: false);
                            create.GetChild(17).gameObject.SetActive(value: false);
                            create.GetChild(18).gameObject.SetActive(value: false);
                            create.GetChild(26).gameObject.SetActive(value: false);
                            create.GetChild(10).gameObject.SetActive(value: false);
                            create.GetChild(11).gameObject.SetActive(value: false);
                            create.GetChild(7).GetChild(0).GetComponent<TextMeshProUGUI>().text = "Connect";
                            return;
                        }
                        else if (ArchipelagoClient.AP.Authenticated && PortEntry.activeSelf)
                        {
                            var create = GameObject.Find("PLAYER").transform.GetChild(1).GetChild(0).GetChild(4).GetChild(2);
                            create.GetChild(12).GetComponent<TextMeshProUGUI>().GetComponent<TMP_Text>().text = "NAME\nBEAUTY\nCLASS\nPRONOUNS";
                            HostEntry.SetActive(value: false);
                            PortEntry.SetActive(value: false);
                            PasswordEntry.SetActive(value: false);
                            create.GetChild(8).gameObject.SetActive(value: true);
                            create.GetChild(9).gameObject.SetActive(value: true);
                            create.GetChild(17).gameObject.SetActive(value: true);
                            create.GetChild(18).gameObject.SetActive(value: true);
                            create.GetChild(26).gameObject.SetActive(value: true);
                            create.GetChild(10).gameObject.SetActive(value: true);
                            create.GetChild(11).gameObject.SetActive(value: true);
                            create.GetChild(7).GetChild(0).GetComponent<TextMeshProUGUI>().text = "Confirm";
                        }
                        return;
                    }
                case "Gameover":
                    {
                        var load = GameObject.Find("PLAYER").transform.GetChild(1).GetChild(0).GetChild(2).GetChild(0);
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
    }
}