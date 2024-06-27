using System;
using System.Collections.Generic;
using System.Linq;
using Archipelago.MultiClient.Net;
using Archipelago.MultiClient.Net.Enums;
using Archipelago.MultiClient.Net.BounceFeatures.DeathLink;
using BepInEx.Logging;
using LunacidAP.Data;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using System.Threading.Tasks;
using static LunacidAP.Data.LunacidLocations;
using Archipelago.MultiClient.Net.Models;
using Archipelago.Gifting.Net.Service;
using Archipelago.Gifting.Net.Gifts.Versions.Current;
using static LunacidAP.Data.LunacidGifts;

namespace LunacidAP
{
    public class ArchipelagoClient : MonoBehaviour
    {
        public const string GAME_NAME = "Lunacid";
        public const string GIFT_COLOR = "#9DAE11";
        private static ManualLogSource _log;
        public static ArchipelagoClient AP;
        private static GameObject Obj;
        public System.Random RandomStatic;
        public int SlotID;
        private int cheatedCount;
        private int Stack;
        private CONTROL Control;
        private POP_text_scr Popup;
        public ArchipelagoSession Session;
        public SlotData SlotData { get; private set; }
        public GiftingService Gifting { get; private set; }
        private GiftHelper giftHelper { get; set; }
        public const long LOCATION_INIT_ID = 771111110;

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
            Obj.name = "ArchipelagoClient";
            DontDestroyOnLoad(Obj);
            AP = Obj.AddComponent<ArchipelagoClient>();
        }

        public IEnumerator Connect(string slotName, string hostName, int port, string password, bool reconnect = false, int slotID = -1)
        {

            _log.LogInfo($"Connecting: {slotName}, {hostName}, {port}");
            IsConnecting = true;
            Authenticated = false;
            var minimumVersion = new Version(0, 5, 0);
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
            var gameVersion = PluginInfo.PLUGIN_VERSION.Split('.');
            var archipelagoVersion = SlotData.ClientVersion.Split('.');
            if (int.Parse(gameVersion[0]) > int.Parse(archipelagoVersion[0]) || int.Parse(gameVersion[1]) > int.Parse(archipelagoVersion[1]))
            {
                _log.LogError($"The server's game was made for {SlotData.ClientVersion}, but this game is {PluginInfo.PLUGIN_VERSION}.");
                IsConnecting = false;
                yield break;
            }
            else if (int.Parse(gameVersion[2]) > int.Parse(archipelagoVersion[2]))
            {
                _log.LogWarning($"The server's game was made for {SlotData.ClientVersion} but the game is newer.  Should be fine though.");
            }
            ConnectionData.Seed = seed;

            RandomStatic = new System.Random(seed);
            if (!reconnect && ConnectionData.ScoutedLocations.Count() == 0)
            {
                BuildLocationTable();

                // Scout unchecked locations
                var uncheckedLocationIDs = from locationID in LocationTable.Keys select locationID;
                Task<Dictionary<long, ScoutedItemInfo>> scoutedInfoTask = Task.Run(async () => await Session.Locations.ScoutLocationsAsync(false, uncheckedLocationIDs.ToArray()));
                //Task<LocationInfoPacket> locationInfoTask = Task.Run(async () => await Session.Locations.ScoutLocationsAsync(false, uncheckedLocationIDs.ToArray()));
                yield return new WaitUntil(() => scoutedInfoTask.IsCompleted);
                if (scoutedInfoTask.IsFaulted)
                {
                    _log.LogError(scoutedInfoTask.Exception.GetBaseException().Message);
                    yield break;
                }
                var scoutedInfo = scoutedInfoTask.Result;

                foreach (var item in scoutedInfo.Values)
                {
                    int locationID = (int)item.LocationId;
                    LocationTable[locationID] = new ArchipelagoItem(item, false);
                }
                ConnectionData.ScoutedLocations = LocationTable;
            }
            CommunionHint.DetermineHints(SlotData.Seed);
            // Sync checked locations
            var checkedLocationIDs = from locationID in ConnectionData.ScoutedLocations.Keys where IsLocationChecked(locationID) select locationID;
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
            Gifting = new GiftingService(Session);
            giftHelper = new GiftHelper(_log);
            Gifting.OpenGiftBox();
            Gifting.SubscribeToNewGifts(Gifting_WhenGiftWasReceived);
            Gifting.CheckGiftBox();
            cheatedCount = 0;
            IsConnecting = false;
            _log.LogInfo("Successfully connected to server!");
        }
        
        public void AttemptConnectFromDeath(int currentSave)
        {
            StartCoroutine(Connect(ConnectionData.SlotName, ConnectionData.HostName, ConnectionData.Port, ConnectionData.Password, false, currentSave));
        }

        public void Gifting_WhenGiftWasReceived(Dictionary<string, Gift> incomingGifts)
        {
            Control = GameObject.Find("CONTROL").GetComponent<CONTROL>();
            Popup = Control.PAPPY;
            StartCoroutine(giftHelper.HandleIncomingGifts());
        }

        public void PrepareGift(GiftVector giftVector, string slotName)
        {
            StartCoroutine(SendGift(giftVector, slotName));   
        }

        private IEnumerator SendGift(GiftVector giftVector, string slotName)
        {
            var sendGiftTask = Task.Run(async () => await Gifting.SendGiftAsync(giftVector.GiftItem, giftVector.GiftTraits, slotName, 0));
            yield return new WaitUntil(() => sendGiftTask.IsCompleted);
            if (sendGiftTask.IsFaulted)
            {
                _log.LogError(sendGiftTask.Exception.GetBaseException().Message);
                yield break;
            }
            var wasSent = sendGiftTask.Result;
            if (wasSent)
            {
                Control = GameObject.Find("CONTROL").GetComponent<CONTROL>();
                Popup = Control.PAPPY;
                if (Popup is null)
                {
                    _log.LogWarning("We tried to tell you about sending an item but we got null output!");
                    yield break;
                }
                Popup.POP($"Gifted <color={GIFT_COLOR}>{giftVector.GiftItem.Name}</color> to {slotName}", 1f, 0);
            }
            else
            {
                _log.LogWarning($"Player {slotName} isn't accepting gifts.");
            }
            
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
            if (Session is not null && IsInNormalGameState())
            {
                if (Session.Items.Any())
                {
                    HandleReceivedItems();
                }
            }
        }

        private void HandleReceivedItems()
        {
            var item = Session.Items.DequeueItem();
            if (item.LocationId >= 0 && !ConnectionData.ReceivedItems.Any(x => x.PlayerId == item.Player && x.LocationId == item.LocationId && item.ItemId == x.ItemId))
            {
                var receivedItem = new ReceivedItem(item);
                ConnectionData.ReceivedItems.Add(receivedItem);
                StartCoroutine(ReceiveItem(receivedItem));
            }
            else if (item.LocationId < 0)
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

        public IEnumerator ReceiveDeathLink(string source, string reason)
        {
            yield return new WaitForSeconds(2f);
            var you = GameObject.Find("PLAYER").GetComponent<Player_Control_scr>();
            Control = GameObject.Find("CONTROL").GetComponent<CONTROL>();
            Control.PAPPY.POP($"The death of {source} by {reason} causes you to perish.", 1f, 1);
            you.Die();
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
                    if (SlotData.ExcludeCoinLocations && LunacidLocations.CoinLocations.Contains(location.APLocationName))
                    {
                        continue;
                    }
                    if (SlotData.ExcludeTower && LunacidLocations.TowerLocations.Contains(location.APLocationName))
                    {
                        continue;
                    }
                    if (SlotData.StartingClass == 5 && location.APLocationName == "WR: Demi's Introduction Gift")
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
            if (SlotData.Dropsanity != Dropsanity.Off)
            {
                foreach (var location in LunacidLocations.UniqueDropLocations)
                {
                    if (location.IgnoreLocationHandler == true)
                    {
                        continue;
                    }
                    locations.Add((int)location.APLocationID);
                }
            }
            if (SlotData.Dropsanity == Dropsanity.Randomized)
            {
                foreach (var location in LunacidLocations.OtherDropLocations)
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
            return ConnectionData.ScoutedLocations[locationId];
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
            return Session.Locations.GetLocationNameFromId(location, "Lunacid");
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
            return SlotData.Ending.HasFlag(goal);
        }

        public bool IsInNormalGameState()
        {
            var isInEnding = new List<string>() { "WhatWillBeAtTheEnd", "END_A", "END_B", "END_E" }.Contains(SceneManager.GetActiveScene().name);
            return Control.LOADED && IsInGame && !isInEnding && GameObject.Find("PLAYER") is not null && Control.Current_Gameplay_State == 0;
        }
    }
}