using System;
using System.IO;
using HarmonyLib;
using Newtonsoft.Json;
using UnityEngine;
using System.Collections.Generic;
using BepInEx.Logging;
using LunacidAP.Data;
using static LunacidAP.Data.LunacidGifts;
using static LunacidAP.Data.LunacidEnemies;

namespace LunacidAP
{
    public class SaveHandler
    {
        private static ManualLogSource _log;

        public SaveHandler(ManualLogSource log)
        {
            _log = log;
        }

        public static void SaveData(int Save_Slot)
        {
            var dir = Application.absoluteURL + "ArchSaves/";
            if (!Directory.Exists(dir))
            {
                Directory.CreateDirectory(dir);
            }
            var backupPath = Path.Combine(dir, $"Save{Save_Slot}.json.bak");
            var savePath = Path.Combine(dir, $"Save{Save_Slot}.json");
            if (File.Exists(backupPath))
            {
                File.Delete(backupPath);
                File.Move(savePath, backupPath);
            }
            _log.LogInfo($"Saving to {savePath}...");
            _log.LogInfo($"In SaveData.  Stored Level: {ConnectionData.StoredLevel}");
            var newAPSaveData = new APSaveData()
            {
                SlotName = ConnectionData.SlotName,
                HostName = ConnectionData.HostName,
                Port = ConnectionData.Port,
                Password = ConnectionData.Password,
                Index = ConnectionData.Index,
                StoredLevel = ConnectionData.StoredLevel,
                StoredExperience = ConnectionData.StoredExperience,
                ItemColors = ConnectionData.ItemColors,
                DeathLink = ConnectionData.DeathLink,
                CheatCount = ConnectionData.CheatedCount,
                ObtainedItems = ConnectionData.ReceivedItems,
                CheckedLocations = ConnectionData.CompletedLocations,
                CommunionHints = ConnectionData.CommunionHints,
                Elements = ConnectionData.Elements,
                Entrances = ConnectionData.Entrances,
                TraversedEntrances = ConnectionData.TraversedEntrances,
                ScoutedLocations = ConnectionData.ScoutedLocations,
                EnteredScenes = ConnectionData.EnteredScenes,
                BoughtItems = ConnectionData.BoughtItems,
                ReceivedGifts = ConnectionData.ReceivedGifts,
                RandomEnemyData = ConnectionData.RandomEnemyData,
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
                _log.LogInfo($"Reading save {Save_Slot}");
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
                    ConnectionData.WriteConnectionData(loadedSave.HostName, loadedSave.Port, loadedSave.SlotName, loadedSave.Password, loadedSave.StoredLevel, loadedSave.StoredExperience,
                    loadedSave.Seed, loadedSave.Index, loadedSave.DeathLink, loadedSave.CheatCount, loadedSave.ObtainedItems, loadedSave.CheckedLocations, 
                    loadedSave.CommunionHints, loadedSave.Elements, loadedSave.Entrances, loadedSave.TraversedEntrances, loadedSave.ScoutedLocations, loadedSave.EnteredScenes, loadedSave.BoughtItems,
                    loadedSave.ReceivedGifts, loadedSave.ItemColors, loadedSave.RandomEnemyData);
                    _log.LogInfo($"We have {loadedSave.StoredLevel} vs {ConnectionData.StoredLevel}");
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
        public int Index;
        public int StoredLevel;
        public int StoredExperience;
        public int Seed;
        public bool DeathLink;
        public int CheatCount;
        public Dictionary<string, ReceivedItem> ObtainedItems;
        public List<long> CheckedLocations;
        public Dictionary<string, string> CommunionHints;
        public Dictionary<string, string> Elements;
        public Dictionary<string, string> Entrances;
        public Dictionary<string, string> TraversedEntrances;
        public SortedDictionary<long, ArchipelagoItem> ScoutedLocations;
        public List<string> EnteredScenes;
        public HashSet<string> BoughtItems;
        public List<ReceivedGift> ReceivedGifts;
        public Dictionary<string, string> ItemColors;
        public Dictionary<string, List<RandomizedEnemyData>> RandomEnemyData;
    }
}