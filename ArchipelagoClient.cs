namespace LunacidAP
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Archipelago.MultiClient.Net;
    using Archipelago.MultiClient.Net.Enums;
    using Archipelago.MultiClient.Net.Helpers;
    using Archipelago.MultiClient.Net.Packets;
    using BepInEx.Logging;
    using LunacidAP.Data;
    using UnityEngine;
    using UnityEngine.SceneManagement;

    public class ArchipelagoClient
    {
        public const string GAME_NAME = "Lunacid";
        private static ManualLogSource _log;
        private CONTROL Control;
        private POP_text_scr Popup;
        public ArchipelagoSession Session;
        public static bool Authenticated;
        public static bool IsInGame = false;
        public static bool HasReceivedItems = false;
        public static readonly List<string> ScenesNotInGame = new() { "MainMenu", "CHAR_CREATE", "Gameover" };
        public const string SEED_KEY = "seed";
        public Dictionary<string, object> SlotData;


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
                SlotData = loginSuccess.SlotData;
                Session.Socket.ErrorReceived += Session_ErrorReceived;
                Session.Socket.SocketClosed += Session_SocketClosed;
                Session.Items.ItemReceived += Session_ItemRecieved;
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

            var seed = SlotData[SEED_KEY].ToString();
            if (ConnectionData.Seed != "" && ConnectionData.Seed != seed)
            {
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
                GiveLunacidItem(receivedItem, self);
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

        private void GiveLunacidItem(long itemID, string player = "", bool self = false)
        {
            string Name = Session.Items.GetItemName(itemID);
            var type = IdentifyItemGetType(Name);
            Name = Name.ToUpper();
            switch (type)
            {
                case 1:
                    {
                        GiveWeapon(Name, player, self);
                        break;
                    }
                case 2:
                    {
                        GiveSpell(Name, player, self);
                        break;
                    }
                case 0:
                    {
                        GiveSilver(Name, player, self);
                        break;
                    }
                case 4:
                    {
                        GiveItem(Name, player, self);
                        break;
                    }
                case 3:
                    {
                        GiveMaterial(Name, player, self);
                        break;
                    }
                case -1:
                    {
                        GiveSwitch(Name, player, self);
                        break;
                    }
            }
        }

        private void GiveLunacidItem(ReceivedItem receivedItem, bool self)
        {
            GiveLunacidItem(receivedItem.ItemId, receivedItem.PlayerName, self);
            ConnectionData.ReceivedItems.Add(receivedItem);
        }

        private void PopupCommand(int sprite, string Name, string player, bool self)
        {
            Control = GameObject.Find("CONTROL").GetComponent<CONTROL>();
            Popup = Control.PAPPY;
            if (self)
                Popup?.POP(Name + $"<sprite={sprite}> ACQUIRED", 1f, 0);
            else
                Popup?.POP(Name + $"<sprite={sprite}> RECEIVED FROM {player}", 1f, 0);
        }

        private void GiveWeapon(string Name, string player = "", bool self = false)
        {
            try
            {
                PopupCommand(1, Name, player, self);
                for (int j = 0; j < 128; j++)
                {
                    if (Control.CURRENT_PL_DATA.WEPS[j] == "" || Control.CURRENT_PL_DATA.WEPS[j] == null || StaticFuncs.REMOVE_NUMS(Control.CURRENT_PL_DATA.WEPS[j]) == Name)
                    {
                        Control.CURRENT_PL_DATA.WEPS[j] = Name;
                        j = 999;
                    }
                }
            }
            catch (Exception ex)
            {
                _log.LogInfo($"Method {nameof(GiveWeapon)} failed.");
                _log.LogInfo($"Reason: {ex.Message}");
            }



        }

        private void GiveSpell(string Name, string player = "", bool self = false)
        {
            try
            {
                PopupCommand(2, Name, player, self);
                for (int k = 0; k < 128; k++)
                {
                    if (Control.CURRENT_PL_DATA.SPELLS[k] == "" || Control.CURRENT_PL_DATA.SPELLS[k] == null || StaticFuncs.REMOVE_NUMS(Control.CURRENT_PL_DATA.SPELLS[k]) == Name)
                    {
                        Control.CURRENT_PL_DATA.SPELLS[k] = Name;
                        k = 999;
                    }
                }

            }
            catch (Exception ex)
            {
                _log.LogInfo($"Failure in {nameof(GiveSpell)}.");
                _log.LogInfo($"Reason: {ex.Message}");
            }

        }

        private void GiveSilver(string Name, string player = "", bool self = false)
        {
            PopupCommand(0, Name, player, self);
            var currencyAmount = Name.Split('(', ')')[1];
            Control.CURRENT_PL_DATA.GOLD += int.Parse(currencyAmount);
        }

        private void GiveItem(string Name, string player = "", bool self = false)
        {
            PopupCommand(4, Name, player, self);
            Useable_Item[] eQ_ITEMS = Control.EQ_ITEMS;
            foreach (Useable_Item useable_Item in eQ_ITEMS)
            {
                if (!(useable_Item.ITEM_NAME == Name.Replace(" ", "_")))
                {
                    continue;
                }
                if (useable_Item.Count > 98)
                {
                    return;
                }
                useable_Item.Count += 2;
                useable_Item.TakeOne();
                useable_Item.CON.ITM_SLOT--;
                useable_Item.CON.SendMessage("OnNextITEM");
                return;
            }
            for (int m = 0; m < 128; m++)
            {
                if (Control.CURRENT_PL_DATA.ITEMS[m] == "" || Control.CURRENT_PL_DATA.ITEMS[m] == null)
                {
                    Control.CURRENT_PL_DATA.ITEMS[m] = Name + "01";
                    m = 999;
                }
                else
                {
                    if (!(StaticFuncs.REMOVE_NUMS(Control.CURRENT_PL_DATA.ITEMS[m]) == Name))
                    {
                        continue;
                    }
                    int num2 = int.Parse(Control.CURRENT_PL_DATA.ITEMS[m].Substring(Control.CURRENT_PL_DATA.ITEMS[m].Length - 2, 2));
                    num2++;
                    if (num2 > 98)
                    {
                        m = 999;
                        continue;
                    }
                    if (num2 < 10)
                    {
                        Control.CURRENT_PL_DATA.ITEMS[m] = Name + "0" + num2;
                    }
                    else
                    {
                        Control.CURRENT_PL_DATA.ITEMS[m] = Name + num2;
                    }
                    m = 999;
                }
            }
        }

        private void GiveMaterial(string Name, string player = "", bool self = false)
        {
            if (!LunacidItems.MaterialNames.Keys.Contains(Name))
            {
                _log.LogInfo($"MaterialNames has no definition for {Name}; cannot give it!");
                return;
            }
            var actualName = LunacidItems.MaterialNames[Name].ToString();
            PopupCommand(3, Name, player, self);
            for (int i = 0; i < 128; i++)
            {
                if (Control.CURRENT_PL_DATA.MATER[i] == "" || Control.CURRENT_PL_DATA.MATER[i] == null)
                {
                    Control.CURRENT_PL_DATA.MATER[i] = actualName + "01";  // Used Alt_Name previously
                    i = 999;
                }
                else
                {
                    if (!(Control.CURRENT_PL_DATA.MATER[i].Substring(0, Control.CURRENT_PL_DATA.MATER[i].Length - 2) == actualName))  // Used Alt_Name previously
                    {
                        continue;
                    }
                    int num = int.Parse(Control.CURRENT_PL_DATA.MATER[i].Substring(Control.CURRENT_PL_DATA.MATER[i].Length - 2, 2));
                    num++;
                    if (num > 99)
                    {
                        i = 999;
                        continue;
                    }
                    if (num < 10)
                    {
                        Control.CURRENT_PL_DATA.MATER[i] = actualName + "0" + num;  // Used Alt_Name previously
                    }
                    else
                    {
                        Control.CURRENT_PL_DATA.MATER[i] = actualName + num;  // Used Alt_Name previously
                    }
                    i = 999;
                }
            }
        }

        private void GiveSwitch(string Name, string player = "", bool self = false)
        {
            // Not yet implemented
        }

        public long GetLocationIDFromName(string locationName)
        {
            return Session.Locations.GetLocationIdFromName(GAME_NAME, locationName);
        }

        public static int IdentifyItemGetType(string name)
        {
            if (LunacidItems.Items.Contains(name))
            {
                return 4;
            }
            else if (LunacidItems.Materials.Contains(name))
            {
                return 3;
            }
            else if (LunacidItems.Weapons.Contains(name))
            {
                return 1;
            }
            else if (LunacidItems.Spells.Contains(name))
            {
                return 2;
            }
            else if (name.Contains("Silver ("))
            {
                return 0;
            }
            else if (name.Contains(" Switch"))
            {
                return -1;
            }
            return -2;
        }

        private bool ActuallyConnected
        {
            get
            {
                return Session?.Socket?.Connected ?? false;
            }
        }
    }
}