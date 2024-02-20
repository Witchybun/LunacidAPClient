
using System.Collections;
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
                SaveHandler.Awake(Archipelago, Log);
                SwitchLocker.Awake(Archipelago, Log);
                ExpHandler.Awake();
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
            while (!ArchipelagoClient.Authenticated && timer < 20)
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

    }
}
