
using System;
using System.Collections;
using System.Collections.Generic;
using Archipelago.MultiClient.Net.Enums;
using Archipelago.MultiClient.Net.MessageLog.Messages;
using Archipelago.MultiClient.Net.Packets;
using BepInEx;
using BepInEx.Logging;
using LunacidAP.Data;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace LunacidAP
{
    [BepInPlugin(PluginInfo.PLUGIN_GUID, PluginInfo.PLUGIN_NAME, PluginInfo.PLUGIN_VERSION)]
    public class Plugin : BaseUnityPlugin
    {
        public ManualLogSource Log { get; private set; }
        public ArchipelagoClient Archipelago {get; private set;}
        public LogicHelper LunacidLogic {get; private set;}
        public static ManualLogSource LOG {get; private set;}
        public LocationHandler LocationHandler {get; private set;}
        public ItemHandler ItemHandler {get; private set;}
        public SwitchLocker SwitchLocker {get; private set;}
        public DoorHandler DoorHandler {get; private set;}
        public TeleportHandler TeleportHandler {get; private set;}
        public ShopHandler ShopHandler {get; private set;}
        public WeaponHandler WeaponHandler {get; private set;}
        public SaveHandler SaveHandler {get; private set;}
        public ExpHandler ExpHandler {get; private set;}
        public SwapperHandler SwapperHandler {get; private set;}
        public MuseHandler MuseHandler {get; private set;}
        public NewGameUI UI {get; private set;}
        private void Awake()
        {
            try
            {
                // Plugin startup logic
                Log = new ManualLogSource("LunacidAP");
                LOG = Log;
                BepInEx.Logging.Logger.Sources.Add(Log);
                Archipelago = new ArchipelagoClient();
                ArchipelagoClient.Setup(Log);
                LocationHandler = new LocationHandler(Log);
                LunacidLogic = new LogicHelper(Log);
                ItemHandler = new ItemHandler(LunacidLogic, Log);
                SwitchLocker = new SwitchLocker(Log);
                DoorHandler = new DoorHandler(Log);
                SaveHandler = new SaveHandler(Log);
                ExpHandler = new ExpHandler(Log);
                WeaponHandler = new WeaponHandler(Log);
                ShopHandler = new ShopHandler(Log);
                TeleportHandler = new TeleportHandler(Log);
                SwapperHandler = new SwapperHandler(Log);
                FlagHandler.Awake(Log);
                CommunionHint.Awake(Log);
                ReadDialogueHelper.Awake(Log);
                UI = new NewGameUI(Log);
                MuseHandler = new MuseHandler(Log);
                Log.LogInfo($"Plugin {PluginInfo.PLUGIN_GUID} has been loaded!  Have fun!");
            }
            catch
            {
                Log.LogInfo($"Plugin {PluginInfo.PLUGIN_GUID} failed to load session!");
            }

        }

        private void OnEnable()
        {
            SceneManager.sceneLoaded += OnSceneLoaded;
        }

        private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            var sceneName = scene.name;
            ArchipelagoClient.IsInGame = !ArchipelagoClient.ScenesNotInGame.Contains(sceneName);
            if (!ArchipelagoClient.IsInGame && ArchipelagoClient.AP.Session is not null)
            {
                ArchipelagoClient.AP.Disconnect();
                ConnectionData.WriteConnectionData();
            }
            if (ArchipelagoClient.IsInGame)
            {
                CheckForVictory(sceneName);
                CheckForDeath(sceneName);
                AddSceneIfNotIncluded(sceneName);

            }
            if (sceneName == "CHAR_CREATE")
            {
                UI.ModifyCharCreateForArchipelago();
            }
            

        }

        private void AddSceneIfNotIncluded(string sceneName)
        {
            ConnectionData.EnteredScenes ??= new List<string>();
            if (!LunacidDoors.SceneToDisplayName.TryGetValue(sceneName, out var displayName))
            {
                Log.LogWarning($"Could not find appropriate scene for {sceneName}");
                return;
            }
            if (ConnectionData.EnteredScenes.Contains(displayName))
            {
                return;
            }
            ConnectionData.EnteredScenes.Add(displayName);
            ArchipelagoClient.AP.Session.DataStorage["CurrentMap"] = displayName;
            ArchipelagoClient.AP.Session.DataStorage["EnteredScenes"] = ConnectionData.EnteredScenes.ToArray();

        }

        private void CheckForVictory(string sceneName)
        {
            if (!ArchipelagoClient.AP.Authenticated)
            {
                return;
            }
            var anyEndingScenes = new List<string>(){"END_E", "END_B", "END_A", "WhatWillBeAtTheEnd"};
            if (sceneName == "END_E" && ArchipelagoClient.AP.HasGoal(Goal.EndingE))
            {
                CallForVictory();
            }
            else if (sceneName == "END_B" && ArchipelagoClient.AP.HasGoal(Goal.EndingB))
            {
                CallForVictory();
            }
            else if(sceneName == "END_A" && ArchipelagoClient.AP.HasGoal(Goal.EndingA))
            {
                CallForVictory();
            }
            else if (sceneName == "WhatWillBeAtTheEnd" && ArchipelagoClient.AP.HasGoal(Goal.EndingCD))
            {
                CallForVictory();
            }
            else if (anyEndingScenes.Contains(sceneName) && ArchipelagoClient.AP.SlotData.Ending == Goal.AnyEnding) // Done because its zero.
            {
                CallForVictory();
            }
            
                
        }

        private void CallForVictory()
        {
            ArchipelagoClient.AP.Session.SetGoalAchieved();
        }

        private void CheckForDeath(string sceneName)
        {
            if (!ArchipelagoClient.AP.Authenticated || !ConnectionData.DeathLink)
            {
                return;
            }
            if (sceneName == "GAME_OVER")
            {
                ArchipelagoClient.AP.SendDeathLink();
            }
            else if ((ArchipelagoClient.IsInGame && ArchipelagoClient.AP.IsCurrentlyDeathLinked) || !ArchipelagoClient.IsInGame)
            {
                ArchipelagoClient.AP.IsCurrentlyDeathLinked = false;
            }
        }

        private void Update()
        {
            if (ArchipelagoClient.AP.Authenticated && ArchipelagoClient.AP.IsCurrentlyDeathLinked)
            {
                StartCoroutine(ArchipelagoClient.AP.ReceiveDeathLink(ArchipelagoClient.AP.CurrentDLData[0], ArchipelagoClient.AP.CurrentDLData[1]));
            }
        }
    }
}
