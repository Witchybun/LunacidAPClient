namespace LunacidAP
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Archipelago.MultiClient.Net;
    using Archipelago.MultiClient.Net.Enums;
    using Archipelago.MultiClient.Net.Helpers;
    using Archipelago.MultiClient.Net.Packets;
    using Archipelago.MultiClient.Net.BounceFeatures.DeathLink;
    using BepInEx.Logging;
    using LunacidAP.Data;
    using UnityEngine;
    using UnityEngine.SceneManagement;
    using System.Collections;
    using System.Threading.Tasks;
    using static LunacidAP.Data.LunacidLocations;
    using Archipelago.MultiClient.Net.Models;


    public class ArchipelagoClient : MonoBehaviour
    {
        public const string GAME_NAME = "Lunacid";
        private static ManualLogSource _log;
        public static ArchipelagoClient AP;
        private static GameObject Obj;
        public System.Random RandomStatic;
        public int SlotID;
        private int cheatedCount;
        private int Stack;
        public const long LOCATION_INIT_ID = 771111110;
        private CONTROL Control;
        public ArchipelagoSession Session;
        public SlotData SlotData { get; private set; }

        private bool _connected;
        public bool Authenticated
        {
            get { return _connected; }
            set
            {
                _connected = value;
            }
        }
        private bool _connecting;
        public bool IsConnecting
        {
            get { return _connecting; }
            set
            {
                _connecting = value;
            }
        }
        private DeathLinkService _deathLinkService;
        public bool IsCurrentlyDeathLinked = false;
        public string[] CurrentDLData = new string[2] { "", "" };
        public static bool IsInGame = false;
        public static bool HasReceivedItems = false;
        public static readonly List<string> ScenesNotInGame = new() { "MainMenu", "CHAR_CREATE", "Gameover" };
        public readonly SortedDictionary<long, ArchipelagoItem> LocationTable = new() { };

        public static void Setup(ManualLogSource log)
        {
            _log = log;
            Obj = new();
            DontDestroyOnLoad(Obj);
            AP = Obj.AddComponent<ArchipelagoClient>();
        }

        public IEnumerator Connect(string slotName, string hostName, int port, string password, bool reconnect = false, int slotID = -1)
        {

            _log.LogInfo($"Connecting: {slotName}, {hostName}, {port}");
            IsConnecting = true;
            Authenticated = false;
            var minimumVersion = new Version(0, 4, 5);
            Session = ArchipelagoSessionFactory.CreateSession(hostName, port);
            var connectTask = Task.Run(Session.ConnectAsync);
            yield return new WaitUntil(() => connectTask.IsCompleted);
            if (!connectTask.Status.HasFlag(TaskStatus.RanToCompletion))
            {
                _log.LogError($"Failed to connect to Archipelago server at {hostName}:{port}.");
                yield break;
            }
            var loginTask = Task.Run(async () => await Session.LoginAsync(GAME_NAME, slotName, ItemsHandlingFlags.AllItems, minimumVersion, password: password));
            yield return new WaitUntil(() => loginTask.IsCompleted);
            if (loginTask.IsFaulted)
            {
                _log.LogError(loginTask.Exception.GetBaseException().Message);
                IsConnecting = false;
                yield break;
            }
            LoginResult result = loginTask.Result;
            if (!result.Successful)
            {
                LoginFailure failure = (LoginFailure)result;
                _log.LogError(string.Join("\n", failure.Errors));
                IsConnecting = false;
                yield break;
            }

            LoginSuccessful login = (LoginSuccessful)result;
            SlotID = login.Slot;
            SlotData = new SlotData(login.SlotData, _log);
            int seed = SlotData.GetSlotSetting("seed", 0);
            if (ConnectionData.Seed != 0 && ConnectionData.Seed != seed)
            {
                _log.LogError("The server's seed does not match the save file's seed.  Make sure you're connecting with the right save file");
                Disconnect();
                IsConnecting = false;
                yield break;
            }
            if (PluginInfo.PLUGIN_VERSION != SlotData.ClientVersion)
            {
                _log.LogError($"The server's game was made for {SlotData.ClientVersion}, but this game is {PluginInfo.PLUGIN_VERSION}.");
                IsConnecting = false;
                yield break;
            }
            ConnectionData.Seed = seed;

            RandomStatic = new System.Random(seed);
            if (!reconnect)
            {
                BuildLocationTable();

                // Scout unchecked locations
                var uncheckedLocationIDs = from locationID in LocationTable.Keys select locationID;
                Task<LocationInfoPacket> locationInfoTask = Task.Run(async () => await Session.Locations.ScoutLocationsAsync(false, uncheckedLocationIDs.ToArray()));
                yield return new WaitUntil(() => locationInfoTask.IsCompleted);
                if (locationInfoTask.IsFaulted)
                {
                    _log.LogError(locationInfoTask.Exception.GetBaseException().Message);
                    yield break;
                }
                var locationInfo = locationInfoTask.Result;

                foreach (var item in locationInfo.Locations)
                {
                    int locationID = (int)item.Location;
                    LocationTable[locationID] = new ArchipelagoItem(item, false);
                }

                CommunionHint.DetermineHints(SlotData.Seed);
            }
            // Sync checked locations
            var checkedLocationIDs = from locationID in LocationTable.Keys where IsLocationChecked(locationID) select locationID;
            var locationCheckTask = Task.Run(async () => await Session.Locations.CompleteLocationChecksAsync(checkedLocationIDs.ToArray()));
            yield return new WaitUntil(() => locationCheckTask.IsCompleted);
            if (locationCheckTask.IsFaulted)
            {
                _log.LogError("Locating Syncing has failed.");
                _log.LogError(locationCheckTask.Exception.GetBaseException().Message);
                IsConnecting = false;
                yield break;
            }
            // Sync collected locations
            foreach (long locationID in Session.Locations.AllLocationsChecked)
            {
                if (!ConnectionData.CompletedLocations.Contains(locationID))
                {
                    ConnectionData.CompletedLocations.Add(locationID);
                }
            }

            // Connection successful
            ConnectionData.WriteConnectionData(hostName, port, slotName, password);
            Authenticated = true;
            if (slotID > -1)
            {
                SaveHandler.SaveData(slotID); // to ensure the updated information isn't lost at load.
            }
            Session.Socket.ErrorReceived += Session_ErrorReceived;
            Session.Socket.SocketClosed += Session_SocketClosed;
            if (login.SlotData["death_link"].ToString() == "1")
            {
                ConnectionData.DeathLink = true;
                InitializeDeathLink();
            }
            cheatedCount = 0;
            IsConnecting = false;
            _log.LogInfo("Successfully connected to server!");
        }

        private void Session_SocketClosed(string _)
        {
            if (Authenticated)
            {
                Authenticated = false;
                StartCoroutine(TryReconnect());
            }
        }

        private IEnumerator TryReconnect()
        {
            while (!Authenticated && IsInGame)
            {
                yield return new WaitForSeconds(60);
                yield return Connect(ConnectionData.SlotName, ConnectionData.HostName, ConnectionData.Port, ConnectionData.Password, true);
            }
        }

        public void Session_ErrorReceived(Exception e, string message)
        {
            _log.LogError(message);
            if (e != null) _log.LogError(e.ToString());
            //Disconnect();
        }

        public void Disconnect()
        {
            if (Session != null && Session.Socket != null)
            {
                Session.Socket.DisconnectAsync();
                _log.LogInfo($"Disconnecting.");
            }
            Session = null;
            Authenticated = false;
            HasReceivedItems = false;
        }

        private void Update()
        {     
            Control = GameObject.Find("CONTROL").GetComponent<CONTROL>();
            if (Session is not null && Session.Items.Any() && IsInNormalGameState())
            {
                var item = Session.Items.DequeueItem();
                if (item.Location >= 0 && !ConnectionData.ReceivedItems.Any(x => x.PlayerId == item.Player && x.LocationId == item.Location && item.Item == x.ItemId))
                {
                    var receivedItem = new ReceivedItem(item);
                    ConnectionData.ReceivedItems.Add(receivedItem);
                    StartCoroutine(ReceiveItem(receivedItem));
                }
                else if (item.Location < 0)
                {
                    _log.LogInfo($"Cheated Count {cheatedCount} before increment vs {ConnectionData.CheatedCount}");
                    cheatedCount++;
                    if (cheatedCount > ConnectionData.CheatedCount)
                    {
                        ConnectionData.CheatedCount = cheatedCount;
                        var receivedItem = new ReceivedItem(item);
                        ConnectionData.ReceivedItems.Add(receivedItem);
                        StartCoroutine(ReceiveItem(receivedItem));
                    }
                }
            }
        }

        private void AppendItemToReceived(ReceivedItem receivedItem)
        {
            foreach (var item in ConnectionData.ReceivedItems.ToList())
            {
                if (item.ItemId == receivedItem.ItemId && item.LocationId == item.LocationId && item.PlayerId == receivedItem.PlayerId)
                {
                    return;
                }
                ConnectionData.ReceivedItems.Add(receivedItem);
            }
        }

        public void InitializeDeathLink()
        {
            if (_deathLinkService == null)
            {
                _deathLinkService = Session.CreateDeathLinkService();
                _deathLinkService.OnDeathLinkReceived += DeathLinkReceieved;
            }
            if (ConnectionData.DeathLink)
            {
                _deathLinkService.EnableDeathLink();
            }
            else
            {
                _deathLinkService.DisableDeathLink();
            }
        }

        private void DeathLinkReceieved(DeathLink deathLink)
        {
            IsCurrentlyDeathLinked = true;
            CurrentDLData[0] = deathLink.Source;
            CurrentDLData[1] = deathLink.Cause;

        }

        public void SendDeathLink()
        {
            if (IsCurrentlyDeathLinked)
            {
                IsCurrentlyDeathLinked = false;
                return;
            }
            var deathLink = new DeathLink(ConnectionData.SlotName, "perished in the Great Well");
            _deathLinkService.SendDeathLink(deathLink);
        }

        public IEnumerator UnleashGhosts(string source, string reason)
        {
            yield return new WaitForSeconds(2f);
            var ghost = GameObject.Find("UNDETH").transform.GetChild(0).gameObject;
            if (!ghost.activeSelf)
            {
                GameObject.Find("MUSE").GetComponent<MUSE_scr>().Undeth();
                ghost.SetActive(true);
            }
            Control ??= GameObject.Find("CONTROL").GetComponent<CONTROL>();
            Control.PAPPY.POP($"The death of {source} by {reason} causes the ghosts to stir...", 1f, 11);
            IsCurrentlyDeathLinked = false;
        }

        private void BuildLocationTable()
        {
            List<int> locations = new();
            foreach (var region in LunacidLocations.APLocationData)
            {
                foreach (var location in region.Value)
                {
                    var allowedList = new List<string>() { "TOWER", "ARENA2" };
                    if (location.IgnoreLocationHandler == true && !allowedList.Contains(region.Key))
                    {
                        continue;
                    }
                    locations.Add((int)location.APLocationID);
                }
            }
            if (SlotData.Shopsanity)
            {
                foreach (var location in LunacidLocations.ShopLocations)
                {
                    if (location.IgnoreLocationHandler == true)
                    {
                        continue;
                    }
                    locations.Add((int)location.APLocationID);
                }
            }
            if (SlotData.Dropsanity)
            {
                foreach (var location in LunacidLocations.DropLocations)
                {
                    if (location.IgnoreLocationHandler == true)
                    {
                        continue;
                    }
                    locations.Add((int)location.APLocationID);
                }
            }

            foreach (var id in locations)
            {
                LocationTable[id] = null;
            }
        }

        private IEnumerator ReceiveItem(ReceivedItem item)
        {
            var instanceStack = Stack + 1;
            Stack++;
            while (instanceStack < Stack)
            {
                yield return new WaitForSeconds(1f);
            }
            var self = false;
            if (item.PlayerName == ConnectionData.SlotName) self = true;
            ItemHandler.GiveLunacidItem(item, self);
            Stack--;
        }

        public static readonly Dictionary<ItemFlags, string> ProgressionFlagToString = new()
        {
          {ItemFlags.Advancement, "Progression"},
          {ItemFlags.NeverExclude, "Useful"},
          {ItemFlags.None, "Filler"},
          {ItemFlags.Trap, "Trap"}
        };

        public static string FlagColor(ItemFlags itemFlag)
        {
            if (itemFlag.HasFlag(ItemFlags.Advancement))
            {
                return "#AF99EF";
            }
            else if (itemFlag.HasFlag(ItemFlags.NeverExclude))
            {
                return "#6D8BE8";
            }
            else if (itemFlag.HasFlag(ItemFlags.Trap))
            {
                return "#FA8072";
            }
            else if (itemFlag.HasFlag(ItemFlags.None))
            {

                return "#00EEEE";
            }
            return "#FFFFFF";
        }

        public ArchipelagoItem ScoutLocation(long locationId)
        {
            return LocationTable[locationId];
        }

        public bool IsLocationChecked(long location)
        {
            return ConnectionData.CompletedLocations.Contains(location);
        }

        public bool IsLocationChecked(string location)
        {
            var locationID = GetLocationIDFromName(location);
            return ConnectionData.CompletedLocations.Contains(locationID);
        }

        public bool IsLocationChecked(LocationData location)
        {
            return ConnectionData.CompletedLocations.Contains(location.APLocationID);
        }

        public long GetLocationIDFromName(string locationName)
        {
            return Session.Locations.GetLocationIdFromName(GAME_NAME, locationName);
        }

        public string GetLocationNameFromID(long location)
        {
            return Session.Locations.GetLocationNameFromId(location);
        }

        public string GetItemNameFromID(long itemID)
        {
            return Session.Items.GetItemName(itemID);
        }

        public string GetPlayerNameFromSlot(int slot)
        {
            return Session.Players.GetPlayerName(slot);
        }

        public bool WasItemReceived(string itemName)
        {
            foreach (var receivedItem in ConnectionData.ReceivedItems)
            {
                if (receivedItem.ItemName == itemName)
                {
                    return true;
                }
            }
            return false;
        }

        public bool WasItemCountReceived(string itemName, int count)
        {
            var foundCount = 0;
            foreach (var receivedItem in ConnectionData.ReceivedItems)
            {
                if (receivedItem.ItemName == itemName)
                {
                    foundCount += 1;
                }
            }
            return count <= foundCount;
        }

        public bool HasGoal(Goal goal)
        {
            return SlotData.Ending.HasFlag(goal) || SlotData.Ending.HasFlag(Goal.AnyEnding);
        }

        private bool IsInNormalGameState()
        {
            var isInEnding = new List<string>() { "WhatWillBeAtTheEnd", "END_A", "END_B", "END_E" }.Contains(SceneManager.GetActiveScene().name);
            return Control.LOADED && IsInGame && !isInEnding && GameObject.Find("PLAYER") is not null && Control.Current_Gameplay_State == 0;
        }
    }
}