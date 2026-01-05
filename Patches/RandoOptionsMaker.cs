using System.Collections.Generic;
using System.IO;
using System.Net;
using BepInEx.Logging;
using HarmonyLib;
using Newtonsoft.Json;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace LunacidAP.Patches;

public class RandoOptionsMaker
{
    private static ManualLogSource _log;

    private static Dictionary<Plugin.RandoSettings.Colors, string> ColorSettingToName = new()
    {
        {Plugin.RandoSettings.Colors.Archipelago, "[Archipelago]"},
        {Plugin.RandoSettings.Colors.Multiworldgg, "[MultiworldGG]"},
        {Plugin.RandoSettings.Colors.Custom, "[Custom]"},
    };
    
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

    public static void CreateInGameRandoSettings(Transform baseParent)
    {
        var leftSideParent = baseParent.Find("LABELS1");
        var rightSideParent = baseParent.Find("LABELS2");
        var settingNameParent = baseParent.Find("Settings");
        rightSideParent.Find("E").gameObject.SetActive(false);
        // Create EXP Slider
        var expName = Object.Instantiate(rightSideParent.Find("D"), rightSideParent, true);
        Object.Destroy(expName.GetComponent<I2.Loc.Localize>());  // DIE
        expName.position = new Vector3(expName.position.x -0.4f, expName.position.y - 5f, expName.position.z);
        expName.name = "EXP Name";
        expName.GetComponent<TextMeshProUGUI>().text = "EXP Multiplier";
        var expSlider = Object.Instantiate(baseParent.Find("MUSE_SLIDER"), baseParent, true);
        expSlider.position = new Vector3(expSlider.position.x, expSlider.position.y - 21f, expSlider.position.z + 2f);
        expSlider.name = "EXP Slider";
        var expSliderData = expSlider.GetComponent<Slider>();
        expSliderData.onValueChanged.m_PersistentCalls.m_Calls[0].arguments.intArgument = 100;
        expSliderData.maxValue = 400;
        expSliderData.minValue = 0;
        expSliderData.SetValueWithoutNotify(Plugin.randoSettings.ExpRate);
        // Create WEXP Slider
        var wexpName = Object.Instantiate(expName, rightSideParent, true);
        wexpName.position = new Vector3(wexpName.position.x - 2f, wexpName.position.y - 5f, wexpName.position.z);
        wexpName.name = "WEXP Name";
        wexpName.GetComponent<TextMeshProUGUI>().text = "WEXP Multiplier";
        var wexpSlider = Object.Instantiate(expSlider, baseParent, true);
        wexpSlider.position = new Vector3(wexpSlider.position.x - 2f, wexpSlider.position.y - 5f, wexpSlider.position.z);
        wexpSlider.name = "WEXP Slider";
        var wexpSliderData = wexpSlider.GetComponent<Slider>();
        wexpSliderData.onValueChanged.m_PersistentCalls.m_Calls[0].arguments.intArgument = 101;
        wexpSliderData.SetValueWithoutNotify(Plugin.randoSettings.WexpRate);
        // Normalized Drops Button
        var normalName = Object.Instantiate(wexpName, rightSideParent, true);
        normalName.position = new Vector3(normalName.position.x, normalName.position.y - 5f, normalName.position.z);
        normalName.name = "Normal Drops Name";
        normalName.GetComponent<TextMeshProUGUI>().text = "Normalize Drops";
        var normalStateName = Object.Instantiate(settingNameParent.Find("H"), settingNameParent, true);
        normalStateName.position = new Vector3(normalStateName.position.x -4f, normalStateName.position.y - 14.5f, normalStateName.position.z);
        normalStateName.name = "Normal Drops Setting Name";
        var stateText = Plugin.randoSettings.IsNormalized ? "[On]" : "[Off]";
        normalStateName.GetComponent<TextMeshProUGUI>().text = stateText;
        var normalButton = Object.Instantiate(baseParent.Find("leg_button"), baseParent, true);
        normalButton.position = new Vector3(normalButton.position.x, normalButton.position.y - 14, normalButton.position.z);
        normalButton.name = "Normal Drops Button";
        var normalButtonData =  normalButton.GetComponent<Button>();
        normalButtonData.onClick.m_PersistentCalls.m_Calls[0].arguments.intArgument = 102;
        // Item Colors Button
        var colorName = Object.Instantiate(leftSideParent.Find("G"), leftSideParent, true);
        Object.Destroy(colorName.GetComponent<I2.Loc.Localize>());  // DIE
        colorName.position = new Vector3(colorName.position.x, colorName.position.y - 5f, colorName.position.z);
        colorName.name = "Color Name";
        colorName.GetComponent<TextMeshProUGUI>().text = "Item Colors";
        var colorStateName = Object.Instantiate(settingNameParent.Find("E"), settingNameParent, true);
        colorStateName.position = new Vector3(colorStateName.position.x, colorStateName.position.y -15f, colorStateName.position.z);
        colorStateName.name = "Color State";
        colorStateName.GetComponent<TextMeshProUGUI>().text = ColorSettingToName[Plugin.randoSettings.ItemColors];
        var colorButton = Object.Instantiate(baseParent.Find("feet_button"), baseParent, true);
        colorButton.position = new Vector3(colorButton.position.x, colorButton.position.y - 15.5f, colorButton.position.z);
        colorButton.name = "Color Button";
        var colorButtonData = colorButton.GetComponent<Button>();
        colorButtonData.onClick.m_PersistentCalls.m_Calls[0].arguments.intArgument = 103;
        // Custom Music Button
        var musicName = Object.Instantiate(colorName, leftSideParent, true);
        musicName.position = new Vector3(musicName.position.x, musicName.position.y - 5f, musicName.position.z);
        musicName.name = "music Name";
        musicName.GetComponent<TextMeshProUGUI>().text = "Custom Music";
        var musicStateName = Object.Instantiate(colorStateName, settingNameParent, true);
        musicStateName.position = new Vector3(musicStateName.position.x, musicStateName.position.y -5f, musicStateName.position.z);
        musicStateName.name = "Music State";
        musicStateName.GetComponent<TextMeshProUGUI>().text = Plugin.randoSettings.PlayCustomMusic ? "[On]" : "[Off]";
        var musicButton = Object.Instantiate(colorButton, baseParent, true);
        musicButton.position = new Vector3(musicButton.position.x, musicButton.position.y - 5f, musicButton.position.z);
        musicButton.name = "Music Button";
        var musicButtonData = musicButton.GetComponent<Button>();
        musicButtonData.onClick.m_PersistentCalls.m_Calls[0].arguments.intArgument = 104;
        // Auto Hint Toggle
        var autoHintName = Object.Instantiate(musicName, leftSideParent, true);
        autoHintName.position = new Vector3(autoHintName.position.x, autoHintName.position.y - 5f, autoHintName.position.z);
        autoHintName.name = "Auto Hint Name";
        autoHintName.GetComponent<TextMeshProUGUI>().text = "Auto Hint";
        var autoHintStateName = Object.Instantiate(musicStateName, settingNameParent, true);
        autoHintStateName.position = new Vector3(autoHintStateName.position.x, autoHintStateName.position.y -5f, autoHintStateName.position.z);
        autoHintStateName.name = "Auto Hint State";
        autoHintStateName.GetComponent<TextMeshProUGUI>().text = Plugin.randoSettings.AutoHint ? "[On]" : "[Off]";
        var autoHintButton = Object.Instantiate(musicButton, baseParent, true);
        autoHintButton.position = new Vector3(autoHintButton.position.x, autoHintButton.position.y - 5f, autoHintButton.position.z);
        autoHintButton.name = "Auto Hint Button";
        var autoHintButtonData = autoHintButton.GetComponent<Button>();
        autoHintButtonData.onClick.m_PersistentCalls.m_Calls[0].arguments.intArgument = 105;
    }

    private static void LoadPortValues()
    {
        if (!GameObject.Find("PLAYER/Canvas/HUD/ROOT/SETTINGS/LABELS1").GetComponent<TextMeshProUGUI>().text
                .Contains("SAVE PORTS"))
        {
            GameObject.Find("PLAYER/Canvas/HUD/ROOT/SETTINGS/LABELS1").GetComponent<TextMeshProUGUI>().text += "\n\n SAVE PORTS";
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
    [HarmonyPrefix]
    private static bool Click_AddNewSettingFunctionality(Menus __instance, int which)
    {
        switch (which)
        {
            case 100:
            {
                var expSlider = GameObject.Find("PLAYER/Canvas/HUD/MAIN/menu4/EXP Slider");
                Plugin.randoSettings.ExpRate = (int) expSlider.GetComponent<Slider>().value;
                return false;
            }
            case 101:
            {
                var wexpSlider = GameObject.Find("PLAYER/Canvas/HUD/MAIN/menu4/WEXP Slider");
                Plugin.randoSettings.WexpRate = (int) wexpSlider.GetComponent<Slider>().value;
                return false;
            }
            case 102:
            {
                Plugin.randoSettings.IsNormalized = !Plugin.randoSettings.IsNormalized;
                var normalSetting = __instance.MENUS[0].transform.Find("menu4").Find("Settings")
                    .Find("Normal Drops Setting Name");
                normalSetting.GetComponent<TextMeshProUGUI>().text = Plugin.randoSettings.IsNormalized ? "[On]" : "[Off]";
                return false;
            }
            case 103:
            {
                var intLookup = (int)Plugin.randoSettings.ItemColors;
                var newLookup = (intLookup + 1) % 3;
                Plugin.randoSettings.ItemColors = (Plugin.RandoSettings.Colors)newLookup;
                var colorSetting = __instance.MENUS[0].transform.Find("menu4").Find("Settings")
                    .Find("Color State");
                colorSetting.GetComponent<TextMeshProUGUI>().text = ColorSettingToName[Plugin.randoSettings.ItemColors];
                return false;
            }
            case 104:
            {
                Plugin.randoSettings.PlayCustomMusic = !Plugin.randoSettings.PlayCustomMusic;
                var normalSetting = __instance.MENUS[0].transform.Find("menu4").Find("Settings")
                    .Find("Music State");
                normalSetting.GetComponent<TextMeshProUGUI>().text = Plugin.randoSettings.PlayCustomMusic ? "[On]" : "[Off]";
                return false;
            }
            case 105:
                Plugin.randoSettings.AutoHint = !Plugin.randoSettings.AutoHint;
                var hintSetting = __instance.MENUS[0].transform.Find("menu4").Find("Settings").Find("Auto Hint State");
                hintSetting.GetComponent<TextMeshProUGUI>().text = Plugin.randoSettings.AutoHint ? "[On]" : "[Off]";
                return false;
        }

        return true;
    }
    
    [HarmonyPatch(typeof(Menus), "Click")]
    [HarmonyPostfix]
    private static void Click_WhatWasPicked(Menus __instance, int which)
    {
        if (__instance.current_menu != 3) return;
        if (__instance.sub_menu != 3) return;
        if (which == 19) SavePortValues();
    }

    [HarmonyPatch(typeof(Menus), "LoadSub")]
    [HarmonyPostfix]
    private static void LoadSub_UpdateSlotDataIfSettingMenu(Menus __instance)
    {
        if (__instance.sub_menu == 3 && __instance.current_menu == 3) LoadPortValues();
    }
}