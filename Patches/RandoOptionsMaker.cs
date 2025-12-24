using System.IO;
using System.Net;
using BepInEx.Logging;
using HarmonyLib;
using Newtonsoft.Json;
using TMPro;
using UnityEngine;

namespace LunacidAP.Patches;

public class RandoOptionsMaker
{
    private static ManualLogSource _log;
    public RandoOptionsMaker(ManualLogSource log)
    {
        _log = log;
        Harmony.CreateAndPatchAll(typeof(RandoOptionsMaker));
    }

    public static void CreatePortModificationSetting(GameObject textInputTemplate)
    {
        var parent = GameObject.Find("PLAYER/Canvas/HUD/ROOT/SETTINGS").transform;
        if (GameObject.Find("PLAYER/Canvas/HUD/ROOT/SETTINGS/Slot1") is not null) return;
        var slot1 = Object.Instantiate(textInputTemplate, parent, true);
        slot1.name = "Slot1";
        slot1.transform.position = new Vector3(-76.6362f, -19.6223f, -260.5781f);
        slot1.transform.localScale = new Vector3(0.7f, 0.7f, 0.7f);
        slot1.transform.GetChild(0).GetChild(0).GetComponent<TextMeshProUGUI>().text = "SLOT 1";
        var slot2 = Object.Instantiate(textInputTemplate, parent, true);
        slot2.name = "Slot2";
        slot2.transform.position = new Vector3(-76.5062f, -19.6223f, -261.0481f);
        slot2.transform.localScale = new Vector3(0.7f, 0.7f, 0.7f);
        slot2.transform.GetChild(0).GetChild(0).GetComponent<TextMeshProUGUI>().text = "SLOT 2";
        var slot3 = Object.Instantiate(textInputTemplate, parent, true);
        slot3.name = "Slot3";
        slot3.transform.position = new Vector3(-76.4262f, -19.6123f, -261.5082f);
        slot3.transform.localScale = new Vector3(0.7f, 0.7f, 0.7f);
        slot3.transform.GetChild(0).GetChild(0).GetComponent<TextMeshProUGUI>().text = "SLOT 3";
    }

    private static void LoadPortValues()
    {
        if (!GameObject.Find("PLAYER/Canvas/HUD/ROOT/SETTINGS/LABELS1").GetComponent<TextMeshProUGUI>().text
                .Contains("SLOT DATA"))
        {
            GameObject.Find("PLAYER/Canvas/HUD/ROOT/SETTINGS/LABELS1").GetComponent<TextMeshProUGUI>().text += "\n\n SLOT DATA";
        }
        var mainDir = Path.Combine(Path.Combine(BepInEx.Paths.PluginPath, "LunacidAP"), "Saves");
        if (!Directory.Exists(mainDir)) return;
        var slot1Dir = Path.Combine(mainDir, "Save0");
        if (Directory.Exists(slot1Dir))
        {
            var connectionInfo1 = Path.Combine(slot1Dir, "ConnectionData.json");
            if (File.Exists(connectionInfo1))
            {
                using var connectionReader1 = new StreamReader(connectionInfo1);
                var text1 = connectionReader1.ReadToEnd();
                var cDS1 = JsonConvert.DeserializeObject<ConnectionDataSave>(text1);
                var slot1Port = cDS1.Port;
                var slot1Text = GameObject.Find("PLAYER/Canvas/HUD/ROOT/SETTINGS/Slot1").GetComponent<TMP_InputField>();
                slot1Text.SetTextWithoutNotify(slot1Port.ToString());
                connectionReader1.Close();
            }   
        }
        var slot2Dir = Path.Combine(mainDir, "Save1");
        if (Directory.Exists(slot2Dir))
        {
            var connectionInfo2 = Path.Combine(slot2Dir, "ConnectionData.json");
            if (File.Exists(connectionInfo2))
            {
                _log.LogInfo("Connection 2 exists");
                using var connectionReader2 = new StreamReader(connectionInfo2);
                var text2 = connectionReader2.ReadToEnd();
                var cDS2 = JsonConvert.DeserializeObject<ConnectionDataSave>(text2);
                var slot2Port = cDS2.Port;
                var slot2Text = GameObject.Find("PLAYER/Canvas/HUD/ROOT/SETTINGS/Slot2").GetComponent<TMP_InputField>();
                slot2Text.SetTextWithoutNotify(slot2Port.ToString());
                connectionReader2.Close();
            }
        }
        var slot3Dir = Path.Combine(mainDir, "Save2");
        if (!Directory.Exists(slot3Dir)) return;
        var connectionInfo3 = Path.Combine(slot3Dir, "ConnectionData.json");
        if (!File.Exists(connectionInfo3)) return;
        using var connectionReader3 = new StreamReader(connectionInfo3);
        var text3 = connectionReader3.ReadToEnd();
        var cDS3 = JsonConvert.DeserializeObject<ConnectionDataSave>(text3);
        var slot3Port = cDS3.Port;
        var slot3Text = GameObject.Find("PLAYER/Canvas/HUD/ROOT/SETTINGS/Slot3").GetComponent<TMP_InputField>();
        slot3Text.SetTextWithoutNotify(slot3Port.ToString());
        connectionReader3.Close();
    }

    private static void SavePortValues()
    {
        var mainDir = Path.Combine(Path.Combine(BepInEx.Paths.PluginPath, "LunacidAP"), "Saves");
        if (!Directory.Exists(mainDir)) return;
        var slot1Dir = Path.Combine(mainDir, "Save0");
        if (Directory.Exists(slot1Dir))
        {
            var connectionInfo1 = Path.Combine(slot1Dir, "ConnectionData.json");
            if (File.Exists(connectionInfo1))
            {
                using var connectionReader1 = new StreamReader(connectionInfo1);
                var text1 = connectionReader1.ReadToEnd();
                var cDS1 = JsonConvert.DeserializeObject<ConnectionDataSave>(text1);
                var slot1Text = GameObject.Find("PLAYER/Canvas/HUD/ROOT/SETTINGS/Slot1").GetComponent<TMP_InputField>().text;
                if (!int.TryParse(slot1Text, out cDS1.Port))
                {
                    _log.LogWarning($"Couldn't parse the typed value to an int.  Was given {slot1Text}");
                }
                var json1 = JsonConvert.SerializeObject(cDS1);
                connectionReader1.Close();
                File.WriteAllText(connectionInfo1, json1);
            }
        }
        var slot2Dir = Path.Combine(mainDir, "Save1");
        if (Directory.Exists(slot2Dir))
        {
            var connectionInfo2 = Path.Combine(slot2Dir, "ConnectionData.json");
            if (File.Exists(connectionInfo2))
            {
                using var connectionReader2 = new StreamReader(connectionInfo2);
                var text2 = connectionReader2.ReadToEnd();
                var cDS2 = JsonConvert.DeserializeObject<ConnectionDataSave>(text2);
                var slot2Text = GameObject.Find("PLAYER/Canvas/HUD/ROOT/SETTINGS/Slot2").GetComponent<TMP_InputField>().text;
                if (int.TryParse(slot2Text, out cDS2.Port))
                {
                    _log.LogWarning($"Couldn't parse the typed value to an int.  Was given {slot2Text}");
                }
                var json2 = JsonConvert.SerializeObject(cDS2);
                connectionReader2.Close();
                File.WriteAllText(connectionInfo2, json2);
            }
        }
        var slot3Dir = Path.Combine(mainDir, "Save2");
        if (!Directory.Exists(slot3Dir)) return;
        var connectionInfo3 = Path.Combine(slot3Dir, "ConnectionData.json");
        if (!File.Exists(connectionInfo3)) return;
        using var connectionReader3 = new StreamReader(connectionInfo3);
        var text3 = connectionReader3.ReadToEnd();
        var cDS3 = JsonConvert.DeserializeObject<ConnectionDataSave>(text3);
        var slot3Text = GameObject.Find("PLAYER/Canvas/HUD/ROOT/SETTINGS/Slot3").GetComponent<TMP_InputField>().text;
        if (int.TryParse(slot3Text, out cDS3.Port))
        {
            _log.LogWarning($"Couldn't parse the typed value to an int.  Was given {slot3Text}");
        }

        var json3 = JsonConvert.SerializeObject(cDS3);
        connectionReader3.Close();
        File.WriteAllText(connectionInfo3, json3);
    }
    
    [HarmonyPatch(typeof(Menus), "Click")]
    [HarmonyPostfix]
    private static void Click_WhatWasPicked(Menus __instance, int which)
    {
        if (__instance.current_menu != 3) return;
        if (__instance.sub_menu != 3) return;
        if (which == 19 && __instance.current_menu == 3 && __instance.sub_menu == 3) SavePortValues();
    }

    [HarmonyPatch(typeof(Menus), "LoadSub")]
    [HarmonyPostfix]
    private static void LoadSub_UpdateSlotDataIfSettingMenu(Menus __instance)
    {
        if (__instance.sub_menu == 3 && __instance.current_menu == 3) LoadPortValues();
    }
}