using System;
using System.Collections.Generic;
using System.Linq;
using Archipelago.MultiClient.Net;
using Archipelago.MultiClient.Net.Enums;
using Archipelago.MultiClient.Net.BounceFeatures.DeathLink;
using BepInEx.Logging;
using LunacidAP.Data;
using LunacidAP.Patches;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using System.Threading.Tasks;
using static LunacidAP.Data.LunacidLocations;
using Archipelago.MultiClient.Net.Models;
using Archipelago.Gifting.Net.Service;
using Archipelago.Gifting.Net.Versioning.Gifts.Current;
using static LunacidAP.Data.LunacidGifts;
using System.Threading;
using Archipelago.MultiClient.Net.Helpers;
using Archipelago.MultiClient.Net.Packets;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Object = System.Object;

namespace LunacidAP.Archipelago
{
    public class ArchipelagoClient : MonoBehaviour
    {
        public const string Game = "Lunacid";
        public const string Version = "0.6.1";
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
        private TrapHandler _trapHandler { get; set; }
        public int CurrentRingLinkUuid { get; set; }

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
        public bool IsCurrentlyDeathLinked;
        public string[] CurrentDLData = new string[2] { "", "" };
        public static bool IsInGame = false;
        public static readonly List<string> ScenesNotInGame = new() { "MainMenu", "CHAR_CREATE", "Gameover" };
        private readonly SortedDictionary<long, ArchipelagoItem> _locationTable = new() { };
        private static readonly Queue<ReceivedItem> ItemsToProcess = new();
        public static readonly Queue<Gift> GiftsToProcess = new();
        public bool allowCoroutines; // Can be set to false in order to cleanly kill the coroutines initialized upon connecting.

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
                            ItemsHandlingFlags.AllItems,
                            new Version(Version),
                            password: password,
                            requestSlotData: true, 
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
            if (result.Successful)
            {
                var success = (LoginSuccessful)result;
                if (!APVersionIsAcceptable(success.SlotData, out var apworldVersion))
                {
                    var outMes = $"Version mismatch.  APWorld: {apworldVersion}, Client: {Version}";
                    Authenticated = false;
                    Disconnect();
                    IsConnecting = false;
                    _log.LogError(outMes);
                    return;
                }
                SlotData = new SlotData(success.SlotData, _log);
                ConnectionData.WriteConnectionData(hostName, port, slotName, password);
                int seed = SlotData.GetSlotSetting("seed", 0);
                SlotID = success.Slot;
                if (success.SlotData["death_link"].ToString() == "1")
                {
                    ConnectionData.DeathLink = true;
                    InitializeDeathLink();
                }
                SetUpGifting();
                MuseHandler.InitializeChosenSongs(seed);
                RandomStatic = new System.Random(seed);
                var unverifiedLocations = ConnectionData.CompletedLocations.Where(x => !Session.Locations.AllLocationsChecked.Contains(x)).ToArray();
                Session.Locations.CompleteLocationChecks(unverifiedLocations);
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
                CommunionHint.DetermineHints(seed);
                RandomizeEquipData(seed);
                _trapHandler = new TrapHandler();
                allowCoroutines = true;
                CurrentRingLinkUuid = UnityEngine.Random.Range(0, SlotID + seed + DateTime.Now.Second + DateTime.Now.Millisecond);
                _log.LogInfo("Starting RingLink");
                if (SlotData.RingLink)
                {
                    Session.ConnectionInfo.UpdateConnectionOptions(Session.ConnectionInfo.Tags.Append("RingLink").ToArray());
                    Session.Socket.PacketReceived += RingLink_OnValueChanged;
                }
                _log.LogInfo("Done");
                StartCoroutine(_trapHandler.BleedPlayerWhenPossible());
                StartCoroutine(_trapHandler.PoisonPlayerWhenPossible());
                StartCoroutine(_trapHandler.DropRatsWhenPossible());
                StartCoroutine(_trapHandler.CursePlayerWhenPossible());
                StartCoroutine(_trapHandler.BlindPlayerWhenPossible());
                StartCoroutine(_trapHandler.SlowPlayerWhenPossible());
                StartCoroutine(_trapHandler.DrainManaOfPlayerWhenPossible());
                StartCoroutine(_trapHandler.DrainXPOfPlayerWhenPossible());
                StartCoroutine(_trapHandler.GoDateDeathWhenNotDoingSoAlready());
                StartCoroutine(HandleQueuedItems());
                StartCoroutine(giftHelper.HandleIncomingGifts());
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

        public void SendRingLinkPacket(int amount)
        {
            var packet = new BouncePacket{
                Tags = new List<string> { "RingLink" },
                Data = new Dictionary<string, JToken> {
                    {"time", (double)DateTimeOffset.UtcNow.ToUnixTimeSeconds() },
                    {"source", CurrentRingLinkUuid},
                    {"amount",  amount},
                }
            };
            Session.Socket.SendPacketAsync(packet);
        }

        private void BuildLocations(int seed)
        {
            if (ConnectionData.Seed != seed)
            {
                BuildLocationTable();
                // Scout unchecked locations
                var uncheckedLocationIDs = from locationID in _locationTable.Keys select locationID;
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
                    _locationTable[locationID] = new ArchipelagoItem(item, collected);
                    if (!ArchipelagoGames.GameData.ContainsKey(item.ItemGame))
                    {
                        ArchipelagoGames.ConstructNewGameData(item.ItemGame);
                    }
                }
                ConnectionData.ScoutedLocations = _locationTable;
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
            Gifting.OpenGiftBox(false, LunacidTraits.DesiredTraits);
            Gifting.OnNewGift += Gifting_WhenGiftWasReceived;
            Gifting.CheckGiftBox();
        }

        public void AttemptConnectFromDeath(int currentSave)
        {
            Connect(ConnectionData.SlotName, ConnectionData.HostName, ConnectionData.Port, ConnectionData.Password, false, currentSave);
        }

        public void Gifting_WhenGiftWasReceived(Gift incomingGift)
        {
            GiftsToProcess.Enqueue(incomingGift);
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
                _log.LogError(sendGiftTask.Exception?.GetBaseException().Message);
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
            if (Session is { Socket: not null })
            {
                Session.Socket.DisconnectAsync();
                _log.LogInfo($"Disconnecting.");
            }
            _deathLinkService = null;
            allowCoroutines = false;
            if (SceneManager.GetActiveScene().name == "MainMenu")
            {
                ConnectionData.WriteConnectionData();
            }
            Session = null;
            Authenticated = false;
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
        }

        private IEnumerator HandleQueuedItems()
        {
            while (allowCoroutines)
            {
                while (!ItemsToProcess.Any())
                {
                    if (!allowCoroutines)
                    {
                        ItemsToProcess.Clear();
                        yield break;
                    }
                    yield return null;
                }
                while (!IsInNormalGameState())
                {
                    yield return null;
                }
                var item = ItemsToProcess.Dequeue();
                if (item.LocationId < 0)
                {
                    cheatedCount += 1;
                }
                if (item.Index < ConnectionData.Index)
                {
                    continue;
                }
                // I wanna get rid of this so friggen bad.
                if (ConnectionData.ReceivedItems.ContainsKey(item.Identifier))
                {
                    continue;
                }
                var isSelf = item.PlayerName == ConnectionData.SlotName;
                var isCheated = item.LocationId < 0;
                var isLevelLocation = item.LocationName.Contains("Reach Level");
                var isSelfLevelLocation = isLevelLocation && isSelf;
                ItemHandler.GiveLunacidItem(item, isSelf, isCheated, isSelfLevelLocation);
                yield return new WaitForSeconds(1f);
                ConnectionData.ReceivedItems[item.Identifier] = item;
                ConnectionData.Index++;
            }
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
                return;
            }
            IsCurrentlyDeathLinked = true;  // Done to avoid spamming as the game itself calls die repeatedly.
            var deathLink = new DeathLink(ConnectionData.SlotName, $"{ConnectionData.SlotName} perished in the Great Well!");
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
                IsCurrentlyDeathLinked = true;
                you.Die();
            }
            catch
            {
                _log.LogWarning("Tried to die somewhere where the player does not exist.");
            }
        }

        private void BuildLocationTable()
        {
            List<int> locations = new();
            foreach (var region in APLocationData)
            {
                foreach (var location in region.Value)
                {
                    var allowedList = new List<string> { "TOWER", "ARENA2" };
                    if (SlotData.RolledMonth != 12 && ChristmasLocations.Contains(location.APLocationName))
                    {
                        continue;
                    }
                    if (location.IgnoreLocationHandler && !allowedList.Contains(region.Key))
                    {
                        continue;
                    }
                    if (SlotData.RolledMonth != 10 && SpookyLocations.Contains(location.APLocationName))
                    {
                        continue;
                    }
                    locations.Add((int)location.APLocationID);
                }
            }
            if (SlotData.Shopsanity)
            {
                foreach (var location in ShopLocations)
                {
                    if (location.IgnoreLocationHandler)
                    {
                        continue;
                    }
                    locations.Add((int)location.APLocationID);
                }
            }
            if (SlotData.Dropsanity != Dropsanity.Off)
            {
                foreach (var location in UniqueDropLocations)
                {
                    if (location.IgnoreLocationHandler)
                    {
                        continue;
                    }
                    locations.Add((int)location.APLocationID);
                }
            }
            if (SlotData.Dropsanity == Dropsanity.Randomized)
            {
                foreach (var location in OtherDropLocations)
                {
                    if (location.IgnoreLocationHandler)
                    {
                        continue;
                    }
                    locations.Add((int)location.APLocationID);
                }
            }
            if (SlotData.Quenchsanity)
            {
                foreach (var location in QuenchLocation)
                {
                    if (location.IgnoreLocationHandler)
                    {
                        continue;
                    }
                    locations.Add((int)location.APLocationID);
                }
            }
            if (SlotData.EtnasPupil)
            {
                foreach (var location in AlkiLocation)
                {
                    if (location.IgnoreLocationHandler)
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
                foreach (var locationkvp in LoreLocations)
                {
                    foreach (var location in locationkvp.Value)
                    {
                        if (location.IgnoreLocationHandler)
                        {
                            continue;
                        }
                        locations.Add((int)location.APLocationID);
                    }
                }
            }
            if (SlotData.GrassSanity)
            {
                foreach (var locationkvp in GrassLocations)
                {
                    locations.AddRange(from location in locationkvp.Value where !location.IgnoreLocationHandler select (int)location.APLocationID);
                }
            }
            if (SlotData.Breakables)
            {
                foreach (var locationkvp in BreakLocations)
                {
                    locations.AddRange(from location in locationkvp.Value where !location.IgnoreLocationHandler select (int)location.APLocationID);
                }
            }

            foreach (var id in locations)
            {
                _locationTable[id] = null;
            }
        }
        
        private void RingLink_OnValueChanged(ArchipelagoPacketBase packet)
        {
            if (packet is not BouncePacket packetBounce)
            {
                return;
            }
            if (!packetBounce.Tags.Contains("RingLink"))
            {
                return;
            }
            _log.LogInfo("Got rings.");
            var data = packetBounce.Data;
            var uuid = (int)data["source"];
            _log.LogInfo($"We have {uuid} vs {CurrentRingLinkUuid}");
            if (uuid == CurrentRingLinkUuid)
            {
                return;
            }
            var amount = (int)data["amount"];
            _log.LogInfo($"We were given {amount}");
            StartCoroutine(UpdateGameSilverWhenPossible(amount));
        }

        private IEnumerator UpdateGameSilverWhenPossible(int amount)
        {
            _log.LogInfo("1");
            while (!IsInNormalGameState())
            {
                _log.LogInfo("Waiting...");
                if (!allowCoroutines)
                {
                    yield break;
                }
                
                yield return new WaitForSeconds(1f);
            }
            _log.LogInfo("Attempt to change gold.");
            var con = GameObject.Find("CONTROL").GetComponent<CONTROL>();
            con.CURRENT_PL_DATA.GOLD = Math.Max(0, con.CURRENT_PL_DATA.GOLD + amount);
        }

        public ArchipelagoItem SendLocationGivenLocationDataSendingGift(LocationData locationData)
        {

            var item = ConnectionData.ScoutedLocations[locationData.APLocationID];
            var isRepeatable = item.Classification == ItemFlags.None || item.Classification.HasFlag(ItemFlags.Trap) || LunacidItems.Materials.Contains(item.Name);
            if (IsLocationChecked(locationData.APLocationID))
            {
                if (item.SlotName == ConnectionData.SlotName && !isRepeatable)
                    return item;
                if (item.SlotName != ConnectionData.SlotName)
                    return GiftHelper.GiftItemToOtherPlayer(item) ? item : null;
                ItemHandler.GiveLunacidItem(item.Name, item.Classification, item.SlotName, true, overrideColor: Colors.GetGiftColor()); // Hey its junk.  Let them grind.  Let them suffer.
                return item;

            }
            LocationHandler.SendLocationCoveringPatchouliCase(locationData);
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
            return Control.LOADED && IsInGame && !isInEnding && GameObject.Find("PLAYER") is not null && Control.Current_Gameplay_State == 0;
        }

        public void SendRandomGiftViaAsync(string itemName)
        {
            StartCoroutine(GiftHelper.GiftRandomGiftToRandomPlayer(itemName));
        }
        
        public void RandomizeEquipData(int seed)
        {
            var random = new System.Random(seed);
            if (ConnectionData.RandomizedWeaponData.Count > 0 && ConnectionData.RandomizedSpellData.Count > 0) return;
            switch (SlotData.RandomEquipStats)
            {
                case RandomEquip.Off:
                {
                    ConnectionData.RandomizedWeaponData = LunacidEquipStats.UsableWeaponData;
                    ConnectionData.RandomizedSpellData = LunacidEquipStats.UsableMagicData;
                    break;
                }
                case RandomEquip.Shuffled:
                {
                    var copiedWeaponData = LunacidEquipStats.UsableWeaponData;
                    var copiedMagicData = LunacidEquipStats.UsableAttackMagicData;
                    var copiedSupportMagicData = LunacidEquipStats.UsableSupportMagicData;
                    foreach (var weapon in LunacidEquipStats.UsableWeaponData.Keys)
                    {
                        var chosenKey = copiedWeaponData.Keys.ToList()[random.Next(copiedWeaponData.Count)];
                        var chosenWeapon = copiedWeaponData[chosenKey];
                        copiedWeaponData.Remove(chosenKey);
                        ConnectionData.RandomizedWeaponData[weapon] = chosenWeapon;
                    }
                    foreach (var spell in LunacidEquipStats.UsableMagicData.Keys)
                    {
                        var chosenKey = copiedMagicData.Keys.ToList()[random.Next(copiedMagicData.Count)];
                        var chosenMagic = copiedMagicData[chosenKey];
                        copiedMagicData.Remove(chosenKey);
                        ConnectionData.RandomizedSpellData[spell] = chosenMagic;
                    }
                    foreach (var spell in LunacidEquipStats.UsableSupportMagicData.Keys)
                    {
                        var chosenKey = copiedSupportMagicData.Keys.ToList()[random.Next(copiedSupportMagicData.Count)];
                        var chosenSupportMagic = copiedSupportMagicData[chosenKey];
                        copiedSupportMagicData.Remove(chosenKey);
                        ConnectionData.RandomizedSpellData[spell] = chosenSupportMagic;
                    }
                    break;
                }
                case RandomEquip.Randomized:
                {
                    var weaponCount = LunacidEquipStats.UsableWeaponData.Count;
                    var magicCount = LunacidEquipStats.UsableMagicData.Count;
                    var attackMagicCount = LunacidEquipStats.UsableAttackMagicData.Count;
                    var weaponList = LunacidEquipStats.UsableWeaponData.Values.ToList();
                    foreach (var weapon in LunacidEquipStats.UsableWeaponData.Keys)
                    {
                        var damage = Math.Max(1, weaponList[random.Next(weaponCount)].Damage); // Avoid a 0 damage scenario.  From where idk.
                        var speed = weaponList[random.Next(weaponCount)].Speed;
                        var guard = weaponList[random.Next(weaponCount)]
                            .Guard;
                        var backstep = weaponList[random.Next(weaponCount)]
                            .Backstep;
                        var thrust = weaponList[random.Next(weaponCount)]
                            .Thrust;
                        var weaponData = new LunacidEquipStats.WeaponData(damage, speed, guard, backstep, thrust);
                        ConnectionData.RandomizedWeaponData[weapon] = weaponData;
                    }

                    foreach (var spell in LunacidEquipStats.UsableAttackMagicData.Keys)
                    {
                        var damage = LunacidEquipStats.UsableAttackMagicData.Values.ToList()[random.Next(attackMagicCount)].Damage;
                        var castChoice =
                            LunacidEquipStats.UsableMagicData.Values.ToList()[
                                random.Next(magicCount)];
                        var castTime = castChoice.CastTime;
                        var minCastTime =  castChoice.MinCastTime;
                        var cost = LunacidEquipStats.UsableMagicData.Values.ToList()[
                            random.Next(magicCount)].Cost;
                        var spellData = new LunacidEquipStats.SpellData(damage, castTime, minCastTime, cost);
                        ConnectionData.RandomizedSpellData[spell] = spellData;
                    }
                    foreach (var spell in LunacidEquipStats.UsableSupportMagicData.Keys)
                    {
                        var castChoice =
                            LunacidEquipStats.UsableMagicData.Values.ToList()[
                                random.Next(magicCount)];
                        var castTime = castChoice.CastTime;
                        var minCastTime =  castChoice.MinCastTime;
                        var cost = LunacidEquipStats.UsableMagicData.Values.ToList()[
                            random.Next(magicCount)].Cost;
                        var spellData = new LunacidEquipStats.SpellData(0, castTime, minCastTime, cost);
                        ConnectionData.RandomizedSpellData[spell] = spellData;
                    }
                    break;
                }
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }
}