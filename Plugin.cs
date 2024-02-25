
using System.Collections;
using System.Linq;
using Archipelago.MultiClient.Net.Enums;
using Archipelago.MultiClient.Net.Packets;
using BepInEx;
using BepInEx.Logging;
using LunacidAP.APGUI;
using LunacidAP.Data;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace LunacidAP
{
    [BepInPlugin(PluginInfo.PLUGIN_GUID, PluginInfo.PLUGIN_NAME, PluginInfo.PLUGIN_VERSION)]
    public class Plugin : BaseUnityPlugin
    {
        public ArchipelagoClient Archipelago { get; set; }
        public ManualLogSource Log { get; set; }

        private void Awake()
        {
            try
            {
                // Plugin startup logic
                Log = new ManualLogSource("LunacidAP");
                BepInEx.Logging.Logger.Sources.Add(Log);
                Archipelago = new ArchipelagoClient(Log);
                GameLog.Awake(Archipelago, Log);
                ArchipelagoLoginGUI.Awake(Archipelago, Log);
                LocationHandler.Awake(Archipelago, Log);
                ItemHandler.Awake(Archipelago, Log);
                SaveHandler.Awake(Archipelago, Log);
                SwitchLocker.Awake(Archipelago, Log);
                ExpHandler.Awake();
                FlagHandler.Awake(Archipelago, Log);
                CommunionHint.Awake(Archipelago, Log);
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
            if (!ArchipelagoClient.IsInGame && Archipelago.Session is not null)
            {
                Archipelago.Disconnect();
                ConnectionData.WriteConnectionData();
            }
            if (ArchipelagoClient.IsInGame)
            {
                StartCoroutine(AutoConnect());
                CheckForVictory(sceneName);
                
            }
            

        }

        private void OnGUI()
        {
            GameLog.OnGUI();
            ArchipelagoLoginGUI.OnGUI();
        }

        private IEnumerator AutoConnect()
        {
            yield return new WaitForSeconds(2f);
            var timer = 0;
            while (!ArchipelagoClient.Authenticated && timer < 10)
            {
                if (ArchipelagoClient.IsInGame)
                {
                    Archipelago.Connect(ConnectionData.SlotName, ConnectionData.HostName, ConnectionData.Password, out var isSuccessful);
                    var isVerified = Archipelago.VerifySeed();
                    if (isSuccessful && isVerified)
                    {
                        Archipelago.ReceiveAllItems();
                        Archipelago.CollectLocationsFromSave();
                    }
                    
                }
                timer += 1;
            }

        }

        private void CheckForVictory(string sceneName)
        {
            if (!ArchipelagoClient.Authenticated)
            {
                return;
            }
            if (sceneName == "END_E" && Archipelago.HasGoal(Goal.EndingE) || 
            sceneName == "END_B" && Archipelago.HasGoal(Goal.EndingB) ||
            sceneName == "END_A" && Archipelago.HasGoal(Goal.EndingA) ||
            sceneName == "WhatWillBeAtTheEnd" && Archipelago.HasGoal(Goal.EndingCD))
            {
                var statusUpdatePacket = new StatusUpdatePacket();
            statusUpdatePacket.Status = ArchipelagoClientState.ClientGoal;
            Archipelago.Session.Socket.SendPacket(statusUpdatePacket);
            }
        }

    }
}
