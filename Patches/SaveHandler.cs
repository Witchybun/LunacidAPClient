using System;
using System.IO;
using Newtonsoft.Json;
using UnityEngine;
using System.Collections.Generic;
using BepInEx.Logging;
using LunacidAP.Archipelago;
using LunacidAP.Data;
using static LunacidAP.Data.LunacidGifts;
using static LunacidAP.Data.LunacidEnemies;

namespace LunacidAP.Patches
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
            var mainDir = Path.Combine(Path.Combine(BepInEx.Paths.PluginPath, "LunacidAP"), "Saves");
            if (!Directory.Exists(mainDir))
            {
                Directory.CreateDirectory(mainDir);
            }

            var dir = Path.Combine(mainDir, $"Save{Save_Slot}");
            if (!Directory.Exists(dir))
            {
                Directory.CreateDirectory(dir);
            }
            var savePath = Path.Combine(dir, "ConnectionData.json");
            var slotPath = Path.Combine(dir, "SlotData.json");
            var newConnectionData = new ConnectionDataSave()
            {
                SlotName = ConnectionData.SlotName,
                HostName = ConnectionData.HostName,
                Port = ConnectionData.Port,
                Password = ConnectionData.Password,
            };
            var newSlotData = new SlotDataSave()
            {

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
                RandomizedWeaponData = ConnectionData.RandomizedWeaponData,
                RandomizedSpellData = ConnectionData.RandomizedSpellData,
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
                newSlotData.Seed = ArchipelagoClient.AP.SlotData.Seed;
            }
            string json = JsonConvert.SerializeObject(newConnectionData);
            File.WriteAllText(savePath, json);
            json = JsonConvert.SerializeObject(newSlotData);
            File.WriteAllText(slotPath, json);
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
                var mainDir = Path.Combine(Path.Combine(BepInEx.Paths.PluginPath, "LunacidAP"), "Saves");
                if (!Directory.Exists(mainDir))
                {
                    _log.LogError("There is no save directory; are you loading a vanilla save?");
                }

                var dir = Path.Combine(mainDir, $"Save{Save_Slot}");
                var savePath = Path.Combine(dir, "ConnectionData.json");
                var slotPath = Path.Combine(dir, "SlotData.json");
                if (!File.Exists(savePath) || !File.Exists(slotPath))
                {
                    _log.LogError("SAVE not found");
                    return;
                }
                using var connectionReader = new StreamReader(savePath);
                var text = connectionReader.ReadToEnd();
                var cDS = JsonConvert.DeserializeObject<ConnectionDataSave>(text);
                using var slotReader = new StreamReader(slotPath);
                text = slotReader.ReadToEnd();
                var sDS = JsonConvert.DeserializeObject<SlotDataSave>(text);
                ConnectionData.WriteConnectionData(cDS.HostName, cDS.Port, cDS.SlotName, cDS.Password, sDS.StoredLevel, sDS.StoredExperience,
                    sDS.Seed, sDS.Index, sDS.DeathLink, sDS.CheatCount, sDS.ObtainedItems, sDS.CheckedLocations, 
                    sDS.CommunionHints, sDS.Elements, sDS.RandomizedWeaponData, sDS.RandomizedSpellData, 
                    sDS.Entrances, sDS.TraversedEntrances, sDS.ScoutedLocations, sDS.EnteredScenes, sDS.BoughtItems,
                    sDS.ReceivedGifts, sDS.ItemColors, sDS.RandomEnemyData);

            }
            catch (Exception ex)
            {
                _log.LogError($"Failed to parse json for save {Save_Slot}");
                _log.LogError($"{ex}");
            }
        }

    }

    internal class ConnectionDataSave
    {
        public string SlotName;
        public string HostName;
        public int Port;
        public string Password;
    }

    internal class SlotDataSave
    {
        
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
        public Dictionary<string, LunacidEquipStats.WeaponData> RandomizedWeaponData;
        public Dictionary<string, LunacidEquipStats.SpellData> RandomizedSpellData;
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