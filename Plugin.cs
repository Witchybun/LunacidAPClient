
using System;
using System.Collections;
using Archipelago.MultiClient.Net.Enums;
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
        public ManualLogSource Log { get; set; }
        public static ManualLogSource LOG {get; set;}
        public NewGameUI UI {get; set;}
        private void Awake()
        {
            try
            {
                // Plugin startup logic
                Log = new ManualLogSource("LunacidAP");
                BepInEx.Logging.Logger.Sources.Add(Log);
                ArchipelagoClient.Setup(Log);
                LOG = Log;
                LocationHandler.Awake(Log);
                ItemHandler.Awake(Log);
                SaveHandler.Awake(Log);
                SwitchLocker.Awake(Log);
                ExpHandler.Awake();
                FlagHandler.Awake(Log);
                CommunionHint.Awake(Log);
                WeaponHandler.Awake(Log);
                ShopHandler.Awake(Log);
                UI = new NewGameUI(Log);
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
                // StartCoroutine(AutoConnect());
                CheckForVictory(sceneName);
                CheckForDeath(sceneName);
            }
            if (sceneName == "CHAR_CREATE")
            {
                UI.ModifyCharCreateForArchipelago();
            }
            

        }

        /*private IEnumerator AutoConnect()
        {
            yield return new WaitForSeconds(2f);
            var timer = 0;
            while (!Archipelago.Authenticated && timer < 1)
            {
                if (ArchipelagoClient.IsInGame && ConnectionData.HostName != "")
                {
                    StartCoroutine(Archipelago.Connect(ConnectionData.SlotName, ConnectionData.HostName, ConnectionData.Password));
                }
                timer += 1;
            }
        }*/

        private void CheckForVictory(string sceneName)
        {
            if (!ArchipelagoClient.AP.Authenticated)
            {
                return;
            }
            if (sceneName == "END_E" && ArchipelagoClient.AP.HasGoal(Goal.EndingE) || 
            sceneName == "END_B" && ArchipelagoClient.AP.HasGoal(Goal.EndingB) ||
            sceneName == "END_A" && ArchipelagoClient.AP.HasGoal(Goal.EndingA) ||
            sceneName == "WhatWillBeAtTheEnd" && ArchipelagoClient.AP.HasGoal(Goal.EndingCD))
            {
                var statusUpdatePacket = new StatusUpdatePacket();
            statusUpdatePacket.Status = ArchipelagoClientState.ClientGoal;
            ArchipelagoClient.AP.Session.Socket.SendPacket(statusUpdatePacket);
            }
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
            else if (ArchipelagoClient.IsInGame && ArchipelagoClient.AP.IsCurrentlyDeathLinked)
            {
                ArchipelagoClient.AP.IsCurrentlyDeathLinked = false;
            }
        }

        private void Update()
        {
            if (ArchipelagoClient.AP.Authenticated && ArchipelagoClient.AP.IsCurrentlyDeathLinked)
            {
                StartCoroutine(ArchipelagoClient.AP.UnleashGhosts(ArchipelagoClient.AP.CurrentDLData[0], ArchipelagoClient.AP.CurrentDLData[1]));
            }
        }

    }
}
