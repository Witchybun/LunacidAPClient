using System;
using System.IO;
using Newtonsoft.Json;
using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using BepInEx.Logging;
using HarmonyLib;
using LunacidAP.Archipelago;
using LunacidAP.Data;
using UnityEngine.SceneManagement;
using static LunacidAP.Data.LunacidGifts;
using static LunacidAP.Data.LunacidEnemies;

namespace LunacidAP.Patches
{
    public class SaveHandler
    {
        private static ManualLogSource _log;
        public static SaveSlotData CurrentSaveData { get; set; }
        public static RandoSettings MainRandoSettings { get; private set; }

        public SaveHandler(ManualLogSource log)
        {
            _log = log;
            CurrentSaveData = new SaveSlotData();
            MainRandoSettings = new RandoSettings();
            Harmony.CreateAndPatchAll(typeof(SaveHandler));
        }

        [Serializable]
        public class RandoSettings
        {
            public int ExpRate { get; set; } = 100;
            public int WexpRate { get; set; } = 100;
            public bool IsNormalized { get; set; } = false;
            public bool PlayCustomMusic { get; set; } = false;
            public Colors ItemColors { get; set; } = Colors.Archipelago;
            public bool AutoHint { get; set; } = false;

            public enum Colors
            {
                Archipelago = 0,
                Multiworldgg = 1,
                Custom = 2
            }

            public Dictionary<SaveHandler.RandoSettings.Colors, string> ColorSettingToName = new()
            {
                { Colors.Archipelago, "[Archipelago]" },
                { Colors.Multiworldgg, "[MultiworldGG]" },
                { Colors.Custom, "[Custom]" },
            };
        }

        [Serializable]
        private class MultiworldSettings
        {
            public SystemData MainData { get; set; }
            public RandoSettings RandoData { get; set; }

            public MultiworldSettings(SystemData mainData, RandoSettings randoData)
            {
                MainData = mainData;
                RandoData = randoData;
            }
        }

        [Serializable]
        public class SaveSlotData
        {
            public string HostName { get; set; } = "";
            public int Port { get; set; }
            public string SlotName { get; set; } = "";
            public int Seed { get; private set; }
            public string Password { get; set; } = "";
            public int Index { get; set; }
            public bool DeathLink { get; set; }
            public int CheatedCount { get; private set; }
            public int StoredLevel { get; set; }
            public int StoredExperience { get; set; }
            public Dictionary<string, ReceivedItem> ReceivedItems { get; private set; } = new();
            public List<long> CompletedLocations { get; private set; } = new();
            public Dictionary<string, CreatureHintData> CommunionHints { get; set; } = new();
            public Dictionary<string, string> Elements { get; private set; } = new(StringComparer.OrdinalIgnoreCase);
            public Dictionary<string, LunacidEquipStats.WeaponData> RandomizedWeaponData { get; set; } = new();
            public Dictionary<string, LunacidEquipStats.SpellData> RandomizedSpellData { get; set; } = new();
            public Dictionary<string, string> Entrances { get; set; } = new();
            public Dictionary<string, string> TraversedEntrances { get; private set; } = new();
            public SortedDictionary<long, ArchipelagoItem> ScoutedLocations = new();
            public List<string> EnteredScenes = new();
            public HashSet<string> BoughtItems = new();
            public Dictionary<string, string> ItemColors = new();
            public Dictionary<string, List<RandomizedEnemyData>> RandomEnemyData = new();

            public SaveSlotData()
            {
                HostName = "";
                Port = 0;
                SlotName = "";
                Password = "";
                Seed = 0;
                Index = 0;
                DeathLink = false;
                CheatedCount = 0;
                StoredLevel = 5;
                StoredExperience = 0;
                ReceivedItems = new Dictionary<string, ReceivedItem>();
                CompletedLocations = new List<long>();
                CommunionHints = new Dictionary<string, CreatureHintData>();
                Elements = new Dictionary<string, string>();
                RandomizedWeaponData = new Dictionary<string, LunacidEquipStats.WeaponData>();
                RandomizedSpellData = new Dictionary<string, LunacidEquipStats.SpellData>();
                Entrances = new Dictionary<string, string>();
                TraversedEntrances = new Dictionary<string, string>();
                ScoutedLocations = new SortedDictionary<long, ArchipelagoItem>();
                EnteredScenes = new List<string>();
                BoughtItems = new HashSet<string>();
                ItemColors = new Dictionary<string, string>();
                RandomEnemyData = new Dictionary<string, List<RandomizedEnemyData>>();
            }
        }

        [Serializable]
        private class MultiworldSave
        {
            public readonly PlayerData PlayerData;
            public readonly SaveSlotData SaveSlotData;

            public MultiworldSave(PlayerData playerData, SaveSlotData saveSlotData)
            {
                PlayerData = playerData;
                SaveSlotData = saveSlotData;
            }
        }

        public static SaveSlotData GrabSaveSlotData(int slot)
        {
            var path = Application.dataPath + "/SAVE_" + slot + ".MIDNIGHTMULTI";
            if (File.Exists(path))
            {
                var binaryFormatter = new BinaryFormatter();
                var fileStream = new FileStream(path, FileMode.Open);
                var result = binaryFormatter.Deserialize(fileStream) as MultiworldSave;
                fileStream.Close();
                return result.SaveSlotData;
            }

            Debug.Log("SAVE not found");
            return new SaveSlotData();
        }

        public static void ModifyPortOfSlot(int slot, int newPort)
        {
            var path = Application.dataPath + "/SAVE_" + slot + ".MIDNIGHTMULTI";
            if (File.Exists(path))
            {
                var binaryFormatter = new BinaryFormatter();
                var fileStream = new FileStream(path, FileMode.Open);
                var result = binaryFormatter.Deserialize(fileStream) as MultiworldSave;
                fileStream.Close();
                result.SaveSlotData.Port = newPort;
                var saveFileStream = new FileStream(path, FileMode.Create);
                binaryFormatter.Serialize(saveFileStream, result);
                saveFileStream.Close();
                return;
            }

            Debug.Log("SAVE not found");
        }


        [HarmonyPatch(typeof(Save), "SAVE_FILE")]
        [HarmonyPrefix]
        private static bool Save_UseNewSaveFormat(int Save_Slot, Vector3 POS, CONTROL CON)
        {
            var previousSave = GrabSaveSlotData(Save_Slot);
            if (previousSave.SlotName != "" && CurrentSaveData.SlotName == "")
            {
                _log.LogWarning("There is an attempt to remove slot info.  If you see this and you aren't deleting report it.");
            }
            var binaryFormatter = new BinaryFormatter();
            var fileStream = new FileStream(Application.dataPath + "/SAVE_" + Save_Slot + ".MIDNIGHTMULTI",
                FileMode.Create);
            var graph = new PlayerData(POS, CON);
            var completedSave = new MultiworldSave(graph, CurrentSaveData);
            binaryFormatter.Serialize(fileStream, completedSave);
            fileStream.Close();
            PlayerPrefs.Save();
            return false;
        }

        [HarmonyPatch(typeof(Save), "LOAD_FILE")]
        [HarmonyPrefix]
        private static bool Load_UseNewSaveFormat(int Save_Slot, ref PlayerData __result)
        {
            var path = Application.dataPath + "/SAVE_" + Save_Slot + ".MIDNIGHTMULTI";
            if (File.Exists(path))
            {
                var binaryFormatter = new BinaryFormatter();
                var fileStream = new FileStream(path, FileMode.Open);
                var result = binaryFormatter.Deserialize(fileStream) as MultiworldSave;
                __result = result.PlayerData;
                if (SceneManager.GetActiveScene().name == "MainMenu" || SceneManager.GetActiveScene().name == "Gameover")
                {
                    CurrentSaveData = result.SaveSlotData;
                }
                fileStream.Close();
                return false;
            }

            Debug.Log("SAVE not found");
            CurrentSaveData = new SaveSlotData();
            __result = Save.ResetData();
            return false;
        }

        [HarmonyPatch(typeof(Save), "SAVE_SYSTEM")]
        [HarmonyPrefix]
        private static bool SYS_SAVE_SaveRandoSettings(CONTROL CON)
        {
            var binaryFormatter = new BinaryFormatter();
            var fileStream = new FileStream(Application.dataPath + "/SETTINGS.MIDNIGHTMULTI", FileMode.Create);
            var graph = new SystemData(CON);
            var systemTotal = new MultiworldSettings(graph, MainRandoSettings);
            binaryFormatter.Serialize(fileStream, systemTotal);
            fileStream.Close();
            PlayerPrefs.Save();
            return false;
        }

        [HarmonyPatch(typeof(Save), "LOAD_SYSTEM")]
        [HarmonyPrefix]
        private static bool SYS_LOAD_LoadRandoSettings(ref SystemData __result)
        {
            string path = Application.dataPath + "/SETTINGS.MIDNIGHTMULTI";
            if (File.Exists(path))
            {
                var binaryFormatter = new BinaryFormatter();
                var fileStream = new FileStream(path, FileMode.Open);
                var result = binaryFormatter.Deserialize(fileStream) as MultiworldSettings;
                __result = result.MainData;
                MainRandoSettings = result.RandoData;
                fileStream.Close();
                return false;
            }

            Debug.Log("System SAVE not found");
            MainRandoSettings = new RandoSettings();
            __result = Save.ResetSYS(null);
            return false;
        }

        [HarmonyPatch(typeof(CONTROL), "OnReset")]
        [HarmonyPrefix]
        private static bool OnReset_UseNewSaveInstead(CONTROL __instance)
        {
            if (PlayerPrefs.GetInt("DEMO_MODE", 0) != 1) return false;
            __instance.CURRENT_SYS_DATA.SAVE0_INFO = "";
            __instance.CURRENT_SYS_DATA.SAVE1_INFO = "";
            __instance.CURRENT_SYS_DATA.SAVE2_INFO = "";
            string path = Application.dataPath + "/SAVE_" + 0 + ".MIDNIGHTMULTI";
            if (File.Exists(path))
            {
                File.Delete(path);
            }
            path = Application.dataPath + "/SAVE_" + 1 + ".MIDNIGHTMULTI";
            if (File.Exists(path))
            {
                File.Delete(path);
            }
            path = Application.dataPath + "/SAVE_" + 2 + ".MIDNIGHTMULTI";
            if (File.Exists(path))
            {
                File.Delete(path);
            }
            Save.ResetData();
            Save.SAVE_SYSTEM(__instance);
            SceneManager.LoadScene(0);
            return false;
        }
    }
}