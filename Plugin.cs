using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Archipelago.MultiClient.Net.Enums;
using BepInEx;
using BepInEx.Logging;
using I2.Loc;
using LunacidAP.Archipelago;
using LunacidAP.Data;
using LunacidAP.Patches;
using Newtonsoft.Json;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;

namespace LunacidAP
{
    [BepInPlugin(PluginInfo.PLUGIN_GUID, PluginInfo.PLUGIN_NAME, PluginInfo.PLUGIN_VERSION)]
    public class Plugin : BaseUnityPlugin
    {
        private ManualLogSource Log { get; set; }
        public GeneralTweaks GeneralTweaks { get; private set; }
        public ArchipelagoClient Archipelago { get; private set; }
        private LogicHelper LunacidLogic { get; set; }
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
        public GrassBreakHandler GrassBreakHandler { get; private set; }
        public RandoOptionsMaker RandoOptionsMaker { get; private set;  }
        private NewGameUI UI { get; set; }
        public LivingGateHandler LivingGateHandler { get; private set; }
        public Colors Colors { get; private set; }
        
        public static RandoSettings randoSettings = new RandoSettings(100, 100, false, false, RandoSettings.Colors.Archipelago);
        private string _currentSceneName;
        private GameObject _hubLevel;
        private void Awake()
        {
            try
            {
                // Plugin startup logic
                Log = new ManualLogSource("LunacidAP");
                LOG = Log;
                BepInEx.Logging.Logger.Sources.Add(Log);
                var versionArray = LocalizationManager.GetVersion().Substring(0, 6).Split('.');
                if (int.Parse(versionArray[0]) > 2 || int.Parse(versionArray[1]) > 8)
                {
                    Log.LogError("Your game is too old for this randomizer!  Update your game.");
                    return;
                }
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
                GrassBreakHandler = new GrassBreakHandler(Log);
                RandoOptionsMaker = new RandoOptionsMaker(Log);
                Colors = new Colors(Log);
                FlagHandler.Awake(Log);
                CommunionHint.Awake(Log);
                ReadDialogueHelper.Awake(Log);
                UI = new NewGameUI(Log);
                LivingGateHandler = new LivingGateHandler(Log);
                MuseHandler = new MuseHandler(Log);
                StoreCustomAudio();
                ArchipelagoGames.ConstructData();
                LunacidEquipStats.InitializeEquipStatLookups();
                LoadCustomRandoSettings();
                Colors.GrabCustomColors();
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
            ArchipelagoClient.AP.IsCurrentlyDeathLinked = false;
            if (sceneName == "MainMenu")
            {
                var versionLabel = GameObject.Find("PLAYER/Canvas/HUD/ROOT/MAIN/version_label").transform;
                versionLabel.position = new Vector3(-75.0263f, -20.5211f, -262.409f);
                var gameVersion = versionLabel.GetComponent<TextMeshProUGUI>().text;
                versionLabel.GetComponent<TextMeshProUGUI>().text = $"Game Version {gameVersion}\nRandomizer Version {PluginInfo.PLUGIN_VERSION}";
                var textBox = FindObjectOfType<Menus>().transform.GetChild(4).GetChild(2).GetChild(5).gameObject;
                RandoOptionsMaker.CreatePortModificationSetting(textBox);
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
                var menu4 = GameObject.Find("PLAYER/Canvas/HUD/MAIN/menu4").transform;
                if (menu4.Find("EXP Slider") is null)
                {
                    var baseParent = menu4;
                    RandoOptionsMaker.CreateInGameRandoSettings(baseParent);
                }
                SaveCustomRandoSettings();
            }
            if (sceneName == "CHAR_CREATE")
            {
                UI.ModifyCharCreateForArchipelago();
            }
            _currentSceneName = "";
            GrassBreakHandler.AddArchipelagoData(sceneName);
            GetInformationAboutScene();
            Cleanup();
            _hubLevel = null;
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

        private void Cleanup()
        {
            var objects = Resources.FindObjectsOfTypeAll<GameObject>().Where(obj => obj.name == "New Game Object");
            var count = 0;
            foreach (var item in objects)
            {
                if (item.transform.childCount == 0 && item.GetComponents(typeof(Component)).Length == 1)
                {
                    Destroy(item);

                    count += 1;
                }
            }
            Log.LogInfo($"Deleted {count} null objects to save scene space");
        }

        private void LoadCustomRandoSettings()
        {
            var mainDir = Path.Combine(Path.Combine(BepInEx.Paths.PluginPath, "LunacidAP"), "RandoSettings.json");
            if (!File.Exists(mainDir))
            {
                var serializedDefault = JsonConvert.SerializeObject(randoSettings);
                File.WriteAllText(mainDir, serializedDefault);
                return;
            }
            using var settingReader = new StreamReader(mainDir);
            var text2 = settingReader.ReadToEnd();
            var settings = JsonConvert.DeserializeObject<RandoSettings>(text2);
            randoSettings = settings;
            settingReader.Close();
        }

        public static void SaveCustomRandoSettings()
        {
            var mainDir = Path.Combine(Path.Combine(Paths.PluginPath, "LunacidAP"), "RandoSettings.json");
            var text = JsonConvert.SerializeObject(randoSettings);
            File.WriteAllText(mainDir, text);
        }

        private void GetInformationAboutScene()
        {
            var stateItems = GameObject.FindObjectsOfType(typeof(AREA_SAVED_ITEM)) as AREA_SAVED_ITEM[];
            var zoneLookup = new Dictionary<string, List<int>>();
            foreach (var item in stateItems)
            {
                if (item.Zone != 7 && item.Slot != 6)
                {
                    continue;
                }
                LOG.LogInfo(
                    $"Object {item.name} will look for Zone {item.Zone} at position {item.Slot} for value {item.value}");
                if (!zoneLookup.ContainsKey(item.name))
                {
                    zoneLookup.Add(item.name, new List<int>());
                }

                if (!zoneLookup[item.name].Contains(item.value))
                {
                    zoneLookup[item.name].Add(item.value);
                }
                
            }

            foreach (var kvp in zoneLookup)
            {
                Log.LogInfo($"In the end, Zone {kvp.Key} has {string.Join(" ", kvp.Value)}");
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
                ArchipelagoClient.AP.Disconnect();
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
            var dir = Path.Combine(Path.Combine(Paths.PluginPath, "LunacidAP"), "CustomMusic");
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
            if (_currentSceneName == "")
            {
                _currentSceneName = SceneManager.GetActiveScene().name;
            }
            if (_currentSceneName != "HUB_01")
            {
                return;
            }
            if (_hubLevel is null)
            {
                _hubLevel = GameObject.Find("LEVEL");
            }
            GeneralTweaks.EnsureAftermathAfterKill(_hubLevel.transform);
        }
        
        public class RandoSettings
        {
            public int ExpRate { get; set; }
            public int WexpRate  { get; set; }
            public bool IsNormalized { get; set; }
            public bool PlayCustomMusic { get; set; }
            public Colors ItemColors { get; set; }
            public enum Colors
            {
                Archipelago = 0,
                Multiworldgg = 1,
                Custom = 2
            }

            public RandoSettings(int expRate, int wexpRate, bool playCustomMusic, bool isNormalized, Colors itemColors)
            {
                ExpRate = expRate;
                WexpRate = wexpRate;
                IsNormalized = isNormalized;
                PlayCustomMusic = playCustomMusic;
                ItemColors = itemColors;
            }
        }
    }
}
