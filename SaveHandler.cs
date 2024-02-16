using System;
using System.IO;
using HarmonyLib;
using BepInEx;
using Newtonsoft.Json;
using UnityEngine;
using System.Collections.Generic;
using BepInEx.Logging;
using LunacidAP.Data;
using UnityEngine.SceneManagement;

namespace LunacidAP
{
    public class SaveHandler
    {
        private static ManualLogSource _log;
        private static ArchipelagoClient _archipelago;

        public static void Awake(ArchipelagoClient archipelago, ManualLogSource log)
        {
            _log = log;
            _archipelago = archipelago;
            Harmony.CreateAndPatchAll(typeof(SaveHandler));
        }

        [HarmonyPatch(typeof(Save), "SAVE_FILE")]
        [HarmonyPrefix]
        private static void SaveFile_SaveArchipelagoData(int Save_Slot, Vector3 POS, CONTROL CON)
        {
            try
            {
                SaveData(Save_Slot);
            }
            catch
            {
                _log.LogError("Could not save data!");
            }
        }

        [HarmonyPatch(typeof(Save), "LOAD_FILE")]
        [HarmonyPrefix]
        private static void Load_LoadArchipelagoData(int Save_Slot)
        {
            var dir = Application.absoluteURL + "ArchSaves/";
            if (ArchipelagoClient.ScenesNotInGame.Contains(SceneManager.GetActiveScene().name))
            {
                return;
            }
            if (!Directory.Exists(dir))
            {
                Directory.CreateDirectory(dir);
            }
            var savePath = Path.Combine(dir, $"Save{Save_Slot}.json");
            if (File.Exists(savePath))
            {
                ReadSave(Save_Slot, savePath);
                _log.LogInfo("Save loaded.  Contents:");
                _log.LogInfo($"SlotName: {ConnectionData.SlotName}, HostName: {ConnectionData.HostName}");
                return;
            }

            _log.LogError("SAVE not found");
            return;
        }

        private static void SaveData(int Save_Slot)
        {
            var dir = Application.absoluteURL + "ArchSaves/";
                if (!Directory.Exists(dir))
                {
                    Directory.CreateDirectory(dir);
                }
                var savePath = Path.Combine(dir, $"Save{Save_Slot}.json");
                _log.LogInfo($"Saving to {savePath}...");
                if (!File.Exists(savePath))
                {

                }
                var newAPSaveData = new APSaveData()
                {
                    SlotName = ConnectionData.SlotName,
                    HostName = ConnectionData.HostName,
                    Password = ConnectionData.Password,
                    ItemIndex = ConnectionData.ItemIndex,
                    ObtainedItems = ConnectionData.ReceivedItems,
                    CheckedLocations = ConnectionData.CompletedLocations
                };
                string json = JsonConvert.SerializeObject(newAPSaveData);
                File.WriteAllText(savePath, json);
                _log.LogInfo("Save complete!");
        }

        private static void ReadSave(int Save_Slot, string savePath)
        {
            try
            {
                _log.LogInfo($"Loading save for slot '{Save_Slot}'...");
                using StreamReader reader = new StreamReader(savePath);
                string text = reader.ReadToEnd();
                var loadedSave = JsonConvert.DeserializeObject<APSaveData>(text);
                ConnectionData.WriteConnectionData(loadedSave.HostName, loadedSave.SlotName, loadedSave.Password, 
                loadedSave.ItemIndex, loadedSave.ObtainedItems, loadedSave.CheckedLocations);

            }
            catch
            {
                _log.LogWarning($"Failed to parse json for save {Save_Slot}");
            }
        }

    }

    internal class APSaveData
    {
        public string SlotName;
        public string HostName;
        public string Password;
        public int ItemIndex;
        public List<long> ObtainedItems;
        public List<string> CheckedLocations;
    }
}