using System;
using System.IO;
using HarmonyLib;
using BepInEx;
using Newtonsoft.Json;
using UnityEngine;
using System.Collections.Generic;
using BepInEx.Logging;

namespace LunacidAP
{
    public class SaveHandler
    {
        private static string savePath;
        public static List<string> ReceivedItems;
        public static List<string> CollectedLocations;
        private static ManualLogSource _archLogger;

        public static void Initialize(ManualLogSource archLogger)
        {
            _archLogger = archLogger;
            ReceivedItems = new(){};
            CollectedLocations = new(){};
        }

        public static void Awake()
        {
            Harmony.CreateAndPatchAll(typeof(SaveHandler));
        }

        [HarmonyPatch(typeof(Save), "SAVE_FILE")]
        [HarmonyPostfix]
        public static void SaveFile_SaveArchipelagoData(int SaveSlot, Vector3 POS, CONTROL CON)
        {
            try
            {
                savePath = Path.Combine(Application.absoluteURL, "UserData/ArchSaves/LastLogin.json");
                var newAPSaveData = new APSaveData()
                {
                SlotId = SaveSlot,
                SlotName = "",
                IPAddress = "localhost",
                Port = "38281",
                Password = "",
                Position = POS,
                Control = CON,
                ObtainedItems = ReceivedItems,
                CheckedLocations = CollectedLocations
                };
                string json = JsonConvert.SerializeObject(newAPSaveData);
                File.WriteAllText(savePath, json);
            }
            catch
            {
                _archLogger.LogError("Method SaveFile_SaveArchipelagoData failed.");
            }
        }
    }

    internal class APSaveData
    {
        public int SlotId;
        public string SlotName;
        public string IPAddress;
        public string Port;
        public string Password;
        public Vector3 Position; // Sanity check field
        public CONTROL Control; // Sanity check field
        public List<string> ObtainedItems;
        public List<string> CheckedLocations;
    }
}