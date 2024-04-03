using System;
using System.IO;
using HarmonyLib;
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

        public SaveHandler(ManualLogSource log)
        {
            _log = log;
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
        [HarmonyPostfix]
        private static void Load_LoadArchipelagoData(int Save_Slot)
        {
            try
            {
                ReadSave(Save_Slot);
            }
            catch (Exception ex)
            {
                _log.LogError($"Failure in {nameof(Load_LoadArchipelagoData)}");
                _log.LogError($"Reason: {ex.Message}");
            }

        }

        public static void SaveData(int Save_Slot)
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
                Port = ConnectionData.Port,
                Password = ConnectionData.Password,
                Symbols = ConnectionData.Index,
                DeathLink = ConnectionData.DeathLink,
                ObtainedItems = ConnectionData.ReceivedItems,
                CheckedLocations = ConnectionData.CompletedLocations,
                CommunionHints = ConnectionData.CommunionHints,
                Elements = ConnectionData.Elements,
                Entrances = ConnectionData.Entrances
            };
            if (ArchipelagoClient.AP.Authenticated && (ConnectionData.Seed == 0))
            {
                newAPSaveData.Seed = ArchipelagoClient.AP.SlotData.Seed;
            }
            string json = JsonConvert.SerializeObject(newAPSaveData);
            File.WriteAllText(savePath, json);
            _log.LogInfo("Save complete!");
        }

        public static void ReadSave(int Save_Slot)
        {
            try
            {
                if (ArchipelagoClient.IsInGame)
                {
                    return; // Don't keep spam loading in situations it isn't relevant; causes data loss.
                }
                var dir = Application.absoluteURL + "ArchSaves/";
                if (!Directory.Exists(dir))
                {
                    Directory.CreateDirectory(dir);
                }
                var savePath = Path.Combine(dir, $"Save{Save_Slot}.json");
                if (File.Exists(savePath))
                {
                    using StreamReader reader = new StreamReader(savePath);
                    string text = reader.ReadToEnd();
                    var loadedSave = JsonConvert.DeserializeObject<APSaveData>(text);
                    ConnectionData.WriteConnectionData(loadedSave.HostName, loadedSave.Port, loadedSave.SlotName, loadedSave.Password,
                    loadedSave.Seed, loadedSave.Symbols, loadedSave.DeathLink, loadedSave.ObtainedItems, loadedSave.CheckedLocations, 
                    loadedSave.CommunionHints, loadedSave.Elements, loadedSave.Entrances);
                    return;
                }

                _log.LogError("SAVE not found");

            }
            catch (Exception ex)
            {
                _log.LogError($"Failed to parse json for save {Save_Slot}");
                _log.LogError($"{ex}");
            }
        }

    }

    internal class APSaveData
    {
        public string SlotName;
        public string HostName;
        public int Port;
        public string Password;
        public int Seed;
        public int Symbols;
        public bool DeathLink;
        public List<ReceivedItem> ObtainedItems;
        public List<long> CheckedLocations;
        public Dictionary<string, CommunionHint.HintData> CommunionHints;
        public Dictionary<string, string> Elements;
        public Dictionary<string, string> Entrances;
    }
}