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

    public class ArchipelagoClient
    {
        public const string GAME_NAME = "Lunacid";
        private static ManualLogSource _log;
        private CONTROL Control;
        private POP_text_scr Popup;
        public ArchipelagoSession Session;
        public SlotData SlotData { get; private set; }
        public static bool Authenticated;
        private DeathLinkService _deathLinkService;
        public bool IsCurrentlyDeathLinked = false;
        public string[] CurrentDLData = new string[2]{"", ""};
        public static bool IsInGame = false;
        public static bool HasReceivedItems = false;
        public static readonly List<string> ScenesNotInGame = new() { "MainMenu", "CHAR_CREATE", "Gameover" };



        public ArchipelagoClient(ManualLogSource log)
        {
            _log = log;
        }

        public bool Connect(string slotName, string hostName, string password, out bool isSuccessful)
        {
            if (Authenticated)
            {
                isSuccessful = true;
                return true;
            }
            if (!IsInGame)
            {
                _log.LogWarning("Please load a save before attempting to connect.");
                isSuccessful = false;
                return false;
            }
            var hostNameContents = hostName.Split(':');
            var isPort = int.TryParse(hostNameContents[1], out int hostPort);
            Session = ArchipelagoSessionFactory.CreateSession(hostNameContents[0], hostPort);

            var minimumVersion = new Version(0, 4, 4);
            var tags = new[] { "AP" };
            LoginResult result = Session.TryConnectAndLogin(GAME_NAME, slotName, ItemsHandlingFlags.AllItems, minimumVersion, tags, null, password);
            ConnectionData.WriteConnectionData(hostName, slotName, password);

            _log.LogInfo(Session.ToString());
            if (result is LoginSuccessful loginSuccess)
            {
                Authenticated = true;
                SlotData = new SlotData(loginSuccess.SlotData, _log);
                Session.Socket.ErrorReceived += Session_ErrorReceived;
                Session.Socket.SocketClosed += Session_SocketClosed;
                Session.Items.ItemReceived += Session_ItemRecieved;
                CommunionHint.DetermineHints(SlotData.Seed);
                if (loginSuccess.SlotData["death_link"].ToString() == "1")
                {
                    ConnectionData.DeathLink = true;
                    InitializeDeathLink();
                }
                _log.LogInfo("Successfully connected to server!");
            }
            else if (result is LoginFailure loginFailure)
            {
                Authenticated = false;
                _log.LogError("Connection Error: " + String.Join("\n", loginFailure.Errors));
                Session.Socket.DisconnectAsync();
                Session = null;
            }
            isSuccessful = result.Successful;
            return result.Successful;
        }

        public bool VerifySeed()
        {

            var seed = SlotData.Seed;
            if (ConnectionData.Seed != 0 && ConnectionData.Seed != seed)
            {
                Control = GameObject.Find("CONTROL").GetComponent<CONTROL>();
                Popup = Control.PAPPY;
                Authenticated = false;
                Popup.POP("Wrong save loaded with connection!", 1f, 0);
                Session.Socket.DisconnectAsync();
                Session = null;
                return false;
            }
            return true;
        }

        private void Session_SocketClosed(string reason)
        {
            if (SceneManager.GetActiveScene().name == "Gameover")
            {
                reason = "Reason: Player has died.";
            }
            _log.LogError("Lost connection to Archipelago server. " + reason);
            Disconnect();
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
            }
            Session = null;
            Authenticated = false;
            HasReceivedItems = false;
        }

        public void Session_ItemRecieved(ReceivedItemsHelper helper)
        {
            ReceiveAllItems();
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

        public List<string> GetAllCheckedLocations()
        {
            if (!Authenticated)
            {
                return new List<string>();
            }

            var allLocationsCheckedIds = Session.Locations.AllLocationsChecked;
            var allLocationsChecked = new List<string>();
            foreach (var id in allLocationsCheckedIds)
            {
                allLocationsChecked.Add(Session.Locations.GetLocationNameFromId(id));
            }
            return allLocationsChecked;
        }

        public void CollectLocationsFromSave()
        {
            var allLocations = GetAllCheckedLocations();
            foreach (var location in ConnectionData.CompletedLocations)
            {
                if (!allLocations.Contains(location))
                {
                    var locationID = Session.Locations.GetLocationIdFromName(GAME_NAME, location);
                    Session.Locations.CompleteLocationChecks(locationID);
                }

            }
            ConnectionData.CompletedLocations = ConnectionData.CompletedLocations.Union(allLocations).ToList();
        }

        public List<ReceivedItem> GetAllReceivedItems()
        {
            if (!Authenticated)
            {
                _log.LogInfo($"You're not connected; returning empty list");
                return new List<ReceivedItem>();
            }
            var allReceivedItems = new List<ReceivedItem>();
            var apItems = Session.Items.AllItemsReceived.ToArray();
            for (var itemIndex = 0; itemIndex < apItems.Length; itemIndex++)
            {
                var apItem = apItems[itemIndex];
                var itemName = Session.Items.GetItemName(apItem.Item);
                var playerName = Session.Players.GetPlayerName(apItem.Player);
                var locationName = Session.Locations.GetLocationNameFromId(apItem.Location) ?? "The Great Well";

                var receivedItem = new ReceivedItem(locationName, itemName, playerName, apItem.Location, apItem.Item,
                    apItem.Player, itemIndex);
                _log.LogInfo($"Appending {itemName} to Received Items List");
                allReceivedItems.Add(receivedItem);
            }

            return allReceivedItems;
        }

        public void ReceiveAllItems()
        {
            var allReceivedItems = GetAllReceivedItems();
            int maxIndex = 0;
            var self = false;

            foreach (var receivedItem in allReceivedItems)
            {
                if (IsItemInSave(receivedItem)) continue;
                if (receivedItem.PlayerName == ConnectionData.SlotName) self = true;
                ItemHandler.GiveLunacidItem(receivedItem, self);
                maxIndex = Math.Max(maxIndex, receivedItem.UniqueId);

            }
        }

        private bool IsItemInSave(ReceivedItem receivedItem)
        {
            foreach (var saveItem in ConnectionData.ReceivedItems)
            {
                var verifier = true;
                if (saveItem.UniqueId != receivedItem.UniqueId)
                {
                    verifier = false;
                }
                if (saveItem.LocationName != receivedItem.LocationName)
                {
                    verifier = false;
                }
                if (verifier == true)
                {
                    return true;
                }
            }
            return false;
        }

        public static readonly Dictionary<ItemFlags, string> ProgressionFlagToString = new()
        {
          {ItemFlags.Advancement, "Progression"},
          {ItemFlags.NeverExclude, "Useful"},
          {ItemFlags.None, "Filler"},
          {ItemFlags.Trap, "Trap"}  
        };

        public LocationInfoPacket ScoutLocation(long locationId, bool createAsHint)
        {
            var scoutTask = Session.Locations.ScoutLocationsAsync(createAsHint, locationId);
            scoutTask.Wait();
            return scoutTask.Result;
        }

        public bool IsLocationChecked(string locationName)
        {
            return ConnectionData.CompletedLocations.Contains(locationName);
        }

        public long GetLocationIDFromName(string locationName)
        {
            return Session.Locations.GetLocationIdFromName(GAME_NAME, locationName);
        }

        public string GetItemNameFromID(long itemID)
        {
            return Session.Items.GetItemName(itemID);
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

        public bool HasGoal(Goal goal)
        {
            return SlotData.Ending.HasFlag(goal) || SlotData.Ending.HasFlag(Goal.AnyEnding);
        }
    }
}