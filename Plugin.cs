using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Archipelago.MultiClient.Net.Enums;
using BepInEx;
using BepInEx.Logging;
using LunacidAP.Data;
using Newtonsoft.Json.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;

namespace LunacidAP
{
    [BepInPlugin(PluginInfo.PLUGIN_GUID, PluginInfo.PLUGIN_NAME, PluginInfo.PLUGIN_VERSION)]
    public class Plugin : BaseUnityPlugin
    {
        public ManualLogSource Log { get; private set; }
        public GeneralTweaks GeneralTweaks { get; private set; }
        public ArchipelagoClient Archipelago { get; private set; }
        public LogicHelper LunacidLogic { get; private set; }
        public static ManualLogSource LOG { get; private set; }
        public LocationHandler LocationHandler { get; private set; }
        public ItemHandler ItemHandler { get; private set; }
        public SwitchLocker SwitchLocker { get; private set; }
        public DoorHandler DoorHandler { get; private set; }
        public TeleportHandler TeleportHandler { get; private set; }
        public ShopHandler ShopHandler { get; private set; }
        public WeaponHandler WeaponHandler { get; private set; }
        public SaveHandler SaveHandler { get; private set; }
        public ExpHandler ExpHandler { get; private set; }
        public SwapperHandler SwapperHandler { get; private set; }
        public MuseHandler MuseHandler { get; private set; }
        public EnemyHandler EnemyHandler { get; private set; }
        public QuenchHandler QuenchHandler { get; private set; }
        public AlkiHandler AlkiHandler { get; private set; }
        public NewGameUI UI { get; private set; }
        public LivingGateHandler LivingGateHandler { get; private set; }
        public Colors Colors { get; private set; }
        private string CurrentSceneName;
        private GameObject HubLevel;
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
                GeneralTweaks = new GeneralTweaks(Log);
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
                EnemyHandler = new EnemyHandler(Log);
                QuenchHandler = new QuenchHandler(Log);
                AlkiHandler = new AlkiHandler(Log);
                Colors = new Colors(Log);
                FlagHandler.Awake(Log);
                CommunionHint.Awake(Log);
                ReadDialogueHelper.Awake(Log);
                UI = new NewGameUI(Log);
                LivingGateHandler = new LivingGateHandler(Log);
                MuseHandler = new MuseHandler(Log);
                StoreCustomAudio();
                Log.LogInfo($"Plugin {PluginInfo.PLUGIN_GUID} has been loaded!  Have fun!");
            }
            catch (Exception ex)
            {
                Log.LogError($"Plugin {PluginInfo.PLUGIN_GUID} failed to load session!");
                Log.LogError(ex);

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
            if (sceneName == "MainMenu")
            {
                var versionLabel = GameObject.Find("PLAYER").transform.Find("Canvas").Find("HUD").Find("ROOT").Find("MAIN").Find("version_label");
                versionLabel.position = new Vector3(-75.0263f, -20.5211f, -262.409f);
                var gameVersion = versionLabel.GetComponent<TextMeshProUGUI>().text;
                versionLabel.GetComponent<TextMeshProUGUI>().text = $"Game Version {gameVersion}\nRandomizer Version {PluginInfo.PLUGIN_VERSION}";
            }
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
            CurrentSceneName = "";
            HubLevel = null;
        }
        private void AddSceneIfNotIncluded(string sceneName)
        {
            if (!LunacidDoors.SceneToDisplayName.TryGetValue(sceneName, out var newScene))
            {
                Log.LogWarning($"Could not find appropriate scene for {newScene}");
                return;
            }

            var oldScene = ArchipelagoClient.AP.Session.DataStorage[Scope.Slot, "currentScene"];

            if (oldScene == newScene)
            {
                return;
            }

            ArchipelagoClient.AP.Session.DataStorage[Scope.Slot, "currentScene"] = newScene;

            if (!ConnectionData.EnteredScenes.Contains(newScene))
            {
                ConnectionData.EnteredScenes.Add(newScene);
            }
        }

        private void CheckForVictory(string sceneName)
        {
            if (!ArchipelagoClient.AP.Authenticated)
            {
                return;
            }
            var anyEndingScenes = new List<string>() { "END_E", "END_B", "END_A", "WhatWillBeAtTheEnd" };
            if (sceneName == "END_E" && ArchipelagoClient.AP.HasGoal(Goal.EndingE))
            {
                CallForVictory();
            }
            else if (sceneName == "END_B" && ArchipelagoClient.AP.HasGoal(Goal.EndingB))
            {
                CallForVictory();
            }
            else if (sceneName == "END_A" && ArchipelagoClient.AP.HasGoal(Goal.EndingA))
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
                ArchipelagoClient.AP.Disconnect();
            }
            else if ((ArchipelagoClient.IsInGame && ArchipelagoClient.AP.IsCurrentlyDeathLinked) || !ArchipelagoClient.IsInGame)
            {
                ArchipelagoClient.AP.IsCurrentlyDeathLinked = false;
            }
        }

        private void StoreCustomAudio()
        {
            StartCoroutine(GetCustomAudio());
            MuseHandler.InitializeTrackInfo();
        }

        // Thanks aihodge from the wild internet unity forums for this banger
        private IEnumerator GetCustomAudio()
        {
            var dir = Application.dataPath.Replace("LUNACID_Data", "") + "CustomMusic/";
            if (!Directory.Exists(dir))
            {
                Directory.CreateDirectory(dir);
            }
            var songs = Directory.GetFiles(dir, "*.mp3", SearchOption.AllDirectories).ToList();
            foreach (var song in songs)
            {
                var songName = song.Replace(dir, "").Replace(".mp3", "");
                using (var uwr = UnityWebRequestMultimedia.GetAudioClip("file://" + song, AudioType.MPEG))
                {
                    ((DownloadHandlerAudioClip)uwr.downloadHandler).streamAudio = true;

                    yield return uwr.SendWebRequest();

                    if (uwr.result == UnityWebRequest.Result.ConnectionError || uwr.result == UnityWebRequest.Result.ProtocolError)
                    {
                        Debug.LogError(uwr.error);
                        yield break;
                    }

                    DownloadHandlerAudioClip dlHandler = (DownloadHandlerAudioClip)uwr.downloadHandler;

                    if (dlHandler.isDone)
                    {
                        AudioClip audioClip = dlHandler.audioClip;

                        if (audioClip != null)
                        {
                            MuseHandler.storedSongs[songName] = DownloadHandlerAudioClip.GetContent(uwr);

                        }
                    }
                }
            }
        }

        private void Update()
        {
            if (ArchipelagoClient.AP.Authenticated && ArchipelagoClient.AP.IsCurrentlyDeathLinked)
            {
                StartCoroutine(ArchipelagoClient.AP.ReceiveDeathLink(ArchipelagoClient.AP.CurrentDLData[0], ArchipelagoClient.AP.CurrentDLData[1]));
            }
            if (CurrentSceneName == "")
            {
                CurrentSceneName = SceneManager.GetActiveScene().name;
            }
            if (CurrentSceneName != "HUB_01")
            {
                return;
            }
            if (HubLevel is null)
            {
                HubLevel = GameObject.Find("LEVEL");
            }
            GeneralTweaks.EnsureAftermathAfterKill(HubLevel.transform);
        }
    }
}
