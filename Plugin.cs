using System.Diagnostics;
using Archipelago.MultiClient.Net;
using Archipelago.MultiClient.Net.Enums;
using BepInEx;
using BepInEx.Logging;
using UnityEngine.SceneManagement;

namespace LunacidAP
{
    [BepInPlugin(PluginInfo.PLUGIN_GUID, PluginInfo.PLUGIN_NAME, PluginInfo.PLUGIN_VERSION)]
    public class Plugin : BaseUnityPlugin
    {

        public ManualLogSource Log;
        public ArchipelagoSession Session;

        private void Awake()
        {
            try{
                // Plugin startup logic
                Log = new ManualLogSource("LunacidAP");
                BepInEx.Logging.Logger.Sources.Add(Log);
                LocationHandler.Initialize(Log);
                LocationHandler.Awake();
                Log.LogInfo($"Session is loaded");
            }
            catch
            {
                Logger.LogInfo($"Plugin {PluginInfo.PLUGIN_GUID} failed to load session!");
            }
            
        }

        private void OnEnable()
        {
            SceneManager.sceneLoaded += OnSceneLoaded;
        }

        private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            if (scene.name == "PITT_A1")
            {
                Session = ArchipelagoSessionFactory.CreateSession("localhost", 38281);
                LoginResult result = Session.TryConnectAndLogin("Lunacid", "Player1", ItemsHandlingFlags.AllItems);
                if (!result.Successful)
                {
                    LoginFailure failure = (LoginFailure)result;
                    string errorMessage  = "Failed to Connect";
                    UnityEngine.Debug.Log(errorMessage);
                }
            }
        }

    }
}
