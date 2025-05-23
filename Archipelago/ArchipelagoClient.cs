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
using Archipelago.Gifting.Net.Versioning.Gifts.Current;
using static LunacidAP.Data.LunacidGifts;
using Newtonsoft.Json.Linq;
using System.Threading;
using Archipelago.MultiClient.Net.Helpers;
using System.Security.Cryptography.X509Certificates;

namespace LunacidAP
{
    public class ArchipelagoClient : MonoBehaviour
    {
        public const string Game = "Lunacid";
        private static ManualLogSource _log;
        public static ArchipelagoClient AP;
        private static GameObject Obj;
        public System.Random RandomStatic;
        public int SlotID;
        private int cheatedCount;
        private CONTROL Control;
        private POP_text_scr Popup;
        public ArchipelagoSession Session;
        public SlotData SlotData { get; private set; }
        public GiftingService Gifting { get; private set; }
        private GiftHelper giftHelper { get; set; }
        public const long LOCATION_INIT_ID = 0;

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
        private static Queue<ReceivedItem> ItemsToProcess = new();
        private bool processingItem = false;

        public static void Setup(ManualLogSource log)
        {
            _log = log;
            Obj = new();
            Obj.name = "ArchipelagoClient";
            DontDestroyOnLoad(Obj);
            AP = Obj.AddComponent<ArchipelagoClient>();
        }


        /// <summary>
        /// call to connect to an Archipelago session. Connection info should already be set up on ServerData
        /// </summary>
        /// <returns></returns>
        public void Connect(string slotName, string hostName, int port, string password, bool reconnect = false, int slotID = -1)
        {
            if (Authenticated || IsConnecting) return;
            IsConnecting = true;

            try
            {
                Session = ArchipelagoSessionFactory.CreateSession(hostName, port);
                SetupSession();
            }
            catch (Exception e)
            {
                _log.LogError(e);
            }
            TryConnect(slotName, hostName, port, password, reconnect, slotID);
        }

        /// <summary>
        /// add handlers for Archipelago events
        /// </summary>
        private void SetupSession()
        {
            Session.Items.ItemReceived += OnItemReceived;
            Session.Socket.ErrorReceived += Session_ErrorReceived;
            Session.Socket.SocketClosed += Session_SocketClosed;
        }



        /// <summary>
        /// attempt to connect to the server with our connection info
        /// </summary>
        private void TryConnect(string slotName, string hostName, int port, string password, bool reconnect = false, int slotID = -1)
        {
            try
            {
                // it's safe to thread this function call but unity notoriously hates threading so do not use excessively
                ThreadPool.QueueUserWorkItem(
                    _ => HandleConnectResult(
                        Session.TryConnectAndLogin(
                            Game,
                            slotName,
                            ItemsHandlingFlags.AllItems, // TODO make sure to change this line
                            new Version(PluginInfo.PLUGIN_VERSION),
                            password: password,
                            requestSlotData: true, // ServerData.NeedSlotData
                            uuid: hostName + port.ToString()
                        ),
                        slotName, hostName, port, password, reconnect, slotID));
            }
            catch (Exception e)
            {
                _log.LogError(e);
                HandleConnectResult(new LoginFailure(e.ToString()), slotName, hostName, port, password, reconnect, slotID);
                IsConnecting = false;
            }
        }

        /// <summary>
        /// handle the connection result and do things
        /// </summary>
        /// <param name="result"></param>
        private void HandleConnectResult(LoginResult result, string slotName, string hostName, int port, string password, bool reconnect = false, int slotID = -1)
        {
            string outText;
            _log.LogInfo("Handling connect result");
            if (result.Successful)
            {
                var success = (LoginSuccessful)result;
                if (!APVersionIsAcceptable(success.SlotData, out var apworldVersion))
                {
                    var outMes = $"Version mismatch.  APWorld: {apworldVersion}, Client: {PluginInfo.PLUGIN_VERSION}";
                    Authenticated = false;
                    Disconnect();
                    IsConnecting = false;
                }
                SlotData = new SlotData(success.SlotData, _log);
                ConnectionData.WriteConnectionData(hostName, port, slotName, password);
                int seed = SlotData.GetSlotSetting("seed", 0);
                SlotID = success.Slot;
                CommunionHint.DetermineHints(seed);
                if (success.SlotData["death_link"].ToString() == "1")
                {
                    ConnectionData.DeathLink = true;
                    InitializeDeathLink();
                }
                SetUpGifting();
                MuseHandler.InitializeChosenSongs(seed);
                RandomStatic = new System.Random(seed);
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
                foreach (long locationID in Session.Locations.AllLocationsChecked)
                {
                    if (!ConnectionData.CompletedLocations.Contains(locationID))
                    {
                        ConnectionData.CompletedLocations.Add(locationID);
                    }
                }
                BuildLocations(SlotData.Seed);
                outText = $"Successfully connected to as {slotName}!";
                //GrabAllTrapOrientedCutscenes();
                Authenticated = true;
            }
            else
            {
                var failure = (LoginFailure)result;
                outText = $"Failed to connect to {hostName}:{port} as {slotName}.";
                outText = failure.Errors.Aggregate(outText, (current, error) => current + $"\n    {error}");

                _log.LogError(outText);

                Authenticated = false;
                Disconnect();
            }
            IsConnecting = false;
        }

        private bool APVersionIsAcceptable(Dictionary<string, object> slotData, out string version)
        {
            var apworldVersion = (string)slotData["client_version"];
            var apworldVersionArray = apworldVersion.Split('.');
            var clientVersionArray = PluginInfo.PLUGIN_VERSION.Split('.');
            if (apworldVersionArray[1] != clientVersionArray[1])
            {
                version = apworldVersion;
                return false;
            }
            version = "";
            return true;
        }

        private void BuildLocations(int seed)
        {
            if (ConnectionData.Seed != seed)
                {
                    BuildLocationTable();
                    // Scout unchecked locations
                    var uncheckedLocationIDs = from locationID in LocationTable.Keys select locationID;
                    var locations = Session.Locations.AllLocations;
                    foreach (var location in uncheckedLocationIDs)
                    {
                        if (locations.Contains(location))
                        {
                            continue;
                        }
                        _log.LogWarning($"There's a location you're trying to scout that isn't there!  Location: {location}");
                    }
                    Task<Dictionary<long, ScoutedItemInfo>> scoutedInfoTask = Task.Run(async () => await Session.Locations.ScoutLocationsAsync(false, uncheckedLocationIDs.ToArray()));
                    //Task<LocationInfoPacket> locationInfoTask = Task.Run(async () => await Session.Locations.ScoutLocationsAsync(false, uncheckedLocationIDs.ToArray()));
                    if (scoutedInfoTask.IsFaulted)
                    {
                        _log.LogError(scoutedInfoTask.Exception.GetBaseException().Message);
                        return;
                    }
                    var scoutedInfo = scoutedInfoTask.Result;

                    foreach (var item in scoutedInfo.Values)
                    {
                        int locationID = (int)item.LocationId;
                        bool collected = ConnectionData.CompletedLocations.Contains(locationID);
                        LocationTable[locationID] = new ArchipelagoItem(item, collected);
                    }
                    ConnectionData.ScoutedLocations = LocationTable;
                }
        }

        // Need to figure this out; A lot of stuff isn't initialized when its best to serve this method.
        public void SendVisibleError(string msg, string type)
        {
            Control ??= GameObject.Find("CONTROL").GetComponent<CONTROL>();
            Popup ??= Control.PAPPY;

            if (type == "ERROR")
            {
                Popup.POP($"<color=FF0000>{msg}</color>", 1f, 1);
            }
            if (type == "WARNING")
            {
                Popup.POP($"<color=FFFF00>{msg}</color>", 1f, 1);
            }

        }

        public void SetUpGifting()
        {
            Gifting = new GiftingService(Session);
            giftHelper = new GiftHelper(_log);
            giftHelper.InitializeTraits();
            Gifting.OpenGiftBox();
            Gifting.OnNewGift += Gifting_WhenGiftWasReceived;
            Gifting.CheckGiftBox();
        }

        public void AttemptConnectFromDeath(int currentSave)
        {
            Connect(ConnectionData.SlotName, ConnectionData.HostName, ConnectionData.Port, ConnectionData.Password, false, currentSave);
        }

        public void Gifting_WhenGiftWasReceived(Gift incomingGift)
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
            var wasSent = sendGiftTask.Result.Success;
            if (wasSent)
            {
                Control = GameObject.Find("CONTROL").GetComponent<CONTROL>();
                Popup = Control.PAPPY;
                if (Popup is null)
                {
                    _log.LogWarning("We tried to tell you about sending an item but we got null output!");
                    yield break;
                }
                Popup.POP($"Gifted <color={Colors.GetGiftColor()}>{giftVector.GiftItem.Name}</color> to {slotName}", 1f, 0);
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
                Connect(ConnectionData.SlotName, ConnectionData.HostName, ConnectionData.Port, ConnectionData.Password, true);
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
            _deathLinkService = null;
            Session = null;
            Authenticated = false;
            HasReceivedItems = false;
        }

        private void OnItemReceived(ReceivedItemsHelper helper)
        {
            var item = helper.DequeueItem();
            var receivedItem = new ReceivedItem(item, helper.Index);
            ItemsToProcess.Enqueue(receivedItem);
        }

        private void Update()
        {
            Control = GameObject.Find("CONTROL").GetComponent<CONTROL>();
            if (!IsInNormalGameState())
            {
                return;
            }
            while (true)
            {
                if (ArchipelagoClient.ItemsToProcess.Count() == 0)
                {
                    break;
                }
                // Reflects the old method in OnItemReceived
                // If we can get a better UI made, this can be toned down some.
                var item = ArchipelagoClient.ItemsToProcess.Dequeue();
                if (item.LocationId < 0)
                {
                    cheatedCount += 1;
                }
                if (item.Index < ConnectionData.Index)
                    {
                        continue;
                    }
                var isSelf = item.PlayerName == ConnectionData.SlotName;
                var isCheated = item.LocationId < 0;
                if (ConnectionData.ReceivedItems.ContainsKey(item.Identifier))
                {
                    continue;
                }
                processingItem = true;
                ItemHandler.GiveLunacidItem(item, isSelf, isCheated);
                ConnectionData.ReceivedItems[item.Identifier] = item;
                ConnectionData.Index++;
            }
        }

        private string GetHeaderFromItem(ItemInfo itemInfo)
        {
            if (itemInfo.LocationId < 0) return "X";
            return itemInfo.Player.Slot.ToString();
        }

        private string GetFooterFromItem(ItemInfo itemInfo)
        {
            if (itemInfo.LocationId < 0) return cheatedCount.ToString();
            return itemInfo.LocationId.ToString();
        }

        public void InitializeDeathLink()
        {
            if (_deathLinkService == null)
            {
                _deathLinkService = Session.CreateDeathLinkService();
                _deathLinkService.OnDeathLinkReceived += DeathLinkReceieved;
            }
            _deathLinkService.EnableDeathLink();
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
            try
            {
                if (!IsInGame)
                {
                    yield break;
                }
                var you = GameObject.Find("PLAYER").GetComponent<Player_Control_scr>();
                Control = GameObject.Find("CONTROL").GetComponent<CONTROL>();
                you.Die();
                IsCurrentlyDeathLinked = false;
            }
            catch
            {
                _log.LogWarning("Tried to die somewhere where the player does not exist.");
            }
        }

        private void BuildLocationTable()
        {
            List<int> locations = new();
            foreach (var region in LunacidLocations.APLocationData)
            {
                foreach (var location in region.Value)
                {
                    var allowedList = new List<string>() { "TOWER", "ARENA2" };
                    if (SlotData.RolledMonth != 12 && LunacidLocations.ChristmasLocations.Contains(location.APLocationName))
                    {
                        continue;
                    }
                    if (location.IgnoreLocationHandler == true && !allowedList.Contains(region.Key))
                    {
                        continue;
                    }
                    if (SlotData.RolledMonth != 10 && LunacidLocations.SpookyLocations.Contains(location.APLocationName))
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
            if (SlotData.Quenchsanity)
            {
                foreach (var location in LunacidLocations.QuenchLocation)
                {
                    if (location.IgnoreLocationHandler == true)
                    {
                        continue;
                    }
                    locations.Add((int)location.APLocationID);
                }
            }
            if (SlotData.EtnasPupil)
            {
                foreach (var location in LunacidLocations.AlkiLocation)
                {
                    if (location.IgnoreLocationHandler == true)
                    {
                        continue;
                    }
                    locations.Add((int)location.APLocationID);
                }
            }
            if (SlotData.Levelsanity)
            {
                var playerLevel = 0;
                if (SlotData.StartingClass == 9)
                {
                    playerLevel = SlotData.CustomStats["Level"];
                }
                else
                {
                    switch (SlotData.StartingClass)
                    {
                        case 0:
                            {
                                playerLevel = 5;
                                break;
                            }
                        case 1:
                            {
                                playerLevel = 10;
                                break;
                            }
                        case 2:
                            {
                                playerLevel = 7;
                                break;
                            }
                        case 3:
                            {
                                playerLevel = 9;
                                break;
                            }
                        case 4:
                            {
                                playerLevel = 8;
                                break;
                            }
                        case 5:
                            {
                                playerLevel = 6;
                                break;
                            }
                        case 6:
                            {
                                playerLevel = 8;
                                break;
                            }
                        case 7:
                            {
                                playerLevel = 9;
                                break;
                            }
                        case 8:
                            {
                                playerLevel = 1;
                                break;
                            }
                    }
                }
                while (playerLevel < 100)
                {
                    locations.Add(801 + playerLevel);
                    playerLevel += 1;
                }
            }
            if (SlotData.Bookworm)
            {
                foreach (var locationkvp in LunacidLocations.LoreLocations)
                {
                    foreach (var location in locationkvp.Value)
                    {
                        if (location.IgnoreLocationHandler == true)
                        {
                            continue;
                        }
                        locations.Add((int)location.APLocationID);
                    }
                }
            }
            if (SlotData.GrassSanity)
            {
                foreach (var locationkvp in LunacidLocations.GrassLocations)
                {
                    foreach (var location in locationkvp.Value)
                    {
                        if (location.IgnoreLocationHandler == true)
                        {
                            continue;
                        }
                        locations.Add((int)location.APLocationID);
                    }
                }
            }
            if (SlotData.Breakables)
            {
                foreach (var locationkvp in LunacidLocations.BreakLocations)
                {
                    foreach (var location in locationkvp.Value)
                    {
                        if (location.IgnoreLocationHandler == true)
                        {
                            continue;
                        }
                        locations.Add((int)location.APLocationID);
                    }
                }
            }

            foreach (var id in locations)
            {
                LocationTable[id] = null;
            }
        }

        public ArchipelagoItem SendLocationGivenLocationDataSendingGift(LocationData locationData)
        {

            var item = ConnectionData.ScoutedLocations[locationData.APLocationID];
            var isRepeatable = item.Classification == ItemFlags.None || item.Classification.HasFlag(ItemFlags.Trap) || LunacidItems.Materials.Contains(item.Name);
            if (ArchipelagoClient.AP.IsLocationChecked(locationData.APLocationID))
            {
                if (item.SlotName == ConnectionData.SlotName && isRepeatable)
                {
                    ItemHandler.GiveLunacidItem(item.Name, item.Classification, item.SlotName, true, overrideColor: Colors.GetGiftColor()); // Hey its junk.  Let them grind.  Let them suffer.
                    return item;
                }
                else if (GiftHelper.GiftItemToOtherPlayer(item))
                {
                    return item;
                }
                return null;
            }
            LocationHandler.DetermineOwnerAndDirectlyGiveIfSelf(locationData, item);
            return item;

        }

        public ArchipelagoItem ScoutLocation(long locationId)
        {
            if (!ConnectionData.ScoutedLocations.TryGetValue(locationId, out var archipelagoItem))
            {
                _log.LogError($"Could not find location for {locationId}");
            }
            return archipelagoItem;
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
            return Session.Locations.GetLocationIdFromName(Game, locationName);
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
                if (receivedItem.Value.ItemName == itemName)
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
                if (receivedItem.Value.ItemName == itemName)
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
            return !processingItem && Control.LOADED && IsInGame && !isInEnding && GameObject.Find("PLAYER") is not null && Control.Current_Gameplay_State == 0;
        }
    }
}