namespace LunacidAP
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;
    using System.Runtime.CompilerServices;
    using Archipelago.MultiClient.Net;
    using Archipelago.MultiClient.Net.Enums;
    using Archipelago.MultiClient.Net.Helpers;
    using Archipelago.MultiClient.Net.Packets;
    using BepInEx.Logging;
    using HarmonyLib;
    using LunacidAP.Data;
    using UnityEngine;
    using UnityEngine.Rendering;
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
        public static readonly List<string> ScenesNotInGame = new(){"MainMenu", "CHAR_CREATE", "Gameover"}; 

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
            Session.Socket.ErrorReceived += Session_ErrorReceived;
            Session.Socket.SocketClosed += Session_SocketClosed;
            Session.Items.ItemReceived += Session_ItemRecieved;

            _log.LogInfo(Session.ToString());
            var minimumVersion = new Version(0, 4, 4);
            var tags = new[] { "AP" };
            LoginResult result = Session.TryConnectAndLogin(GAME_NAME, slotName, ItemsHandlingFlags.AllItems, minimumVersion, tags, null, password);
            ConnectionData.WriteConnectionData(hostName, slotName, password);

            _log.LogInfo(Session.ToString());
            if (result is LoginSuccessful loginSuccess)
            {
                Authenticated = true;
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
            if (helper.Index! > ConnectionData.ItemIndex)
            {
                var itemID = helper.PeekItem().Item;
                string player = Session.Players.GetPlayerName(helper.PeekItem().Player);
                bool self = false;

                if (player == ConnectionData.SlotName) self = true;
                GiveLunacidItem(itemID, player, self);
                ConnectionData.ItemIndex++;
            }
            helper.DequeueItem();
        }

        public IEnumerator ReceiveMissingItems()
        {
            yield return new WaitForSecondsRealtime(2f);
            while (ConnectionData.ItemIndex < Session.Items.Index)
            {
                var item = Session.Items.AllItemsReceived[Convert.ToInt32(ConnectionData.ItemIndex)];
                long itemID = item.Item;
                string player = Session.Players.GetPlayerName(item.Player);
                bool self = false;

                if (ConnectionData.SlotName == player) self = true;

                GiveLunacidItem(itemID, player, self);
                ConnectionData.ItemIndex++;
            }
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
            ConnectionData.ReceivedItems.AddItem(itemID);
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
                /*var weaponList = Control.CURRENT_PL_DATA.WEPS.ToList() ?? new List<string>() { };
                var weaponLength = weaponList.Count();
                if (weaponLength != 0 && weaponList.Any(x => x.Contains(Name)))
                {
                    _log.LogInfo($"Weapon list already has {Name}; returning.");
                    return;
                }
                else
                {
                    weaponList.Add(Name);
                }
                Control.CURRENT_PL_DATA.WEPS = weaponList.ToArray();*/
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
            }



        }

        private void GiveSpell(string Name, string player = "", bool self = false)
        {
            try
            {
                PopupCommand(2, Name, player, self);
                /*var spellList = Control.CURRENT_PL_DATA.SPELLS.ToList() ?? new List<string>(){};
                var isEmpty = spellList.Any();
                if (!isEmpty && spellList.Any(x => x.Contains(Name)))
                {
                    _log.LogInfo($"Spell list already has {Name}; returning.");
                    return;
                }
                else
                {
                    spellList.Add(Name);
                }
                Control.CURRENT_PL_DATA.SPELLS = spellList.ToArray();*/
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
            /*var itemList = Control.CURRENT_PL_DATA.ITEMS.ToList() ??  new List<string>(){};
            var itemCount = itemList.Count();

            bool hasItem = false;
            string heldItem = "";
            int itemHeldCount = 0;
            if (itemCount != 0 && itemList.Any(x => x.Contains(Name)))
            {
                hasItem = true;
                heldItem = itemList.First(x => x.Contains(Name));
                itemHeldCount = int.Parse(heldItem.Substring(heldItem.Length - 2, 2));
                _log.LogInfo($"Noting that play already has {itemHeldCount} {Name}(s).");

            }
            if (itemCount == 0 || !hasItem)
            {
                itemList.Add(Name + "01");
                _log.LogInfo($"Given first instance of {Name}");
            }
            else if (hasItem)
            {
                var newCount = itemHeldCount + 1;
                if (newCount > 98)
                {
                    return; // Don't add the item.
                }
                var indexPosition = itemList.IndexOf(heldItem);
                var newItem = StaticFuncs.REMOVE_NUMS(heldItem);
                if (newCount < 10)
                {
                    newItem += "0" + newCount.ToString();
                }
                else
                {
                    newItem += newCount.ToString();
                }
                itemList[indexPosition] = newItem;
                _log.LogInfo($"Updated count for {Name}; appended {newItem} to list");


            }
            Control.CURRENT_PL_DATA.ITEMS = itemList.ToArray();*/
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
            // Normally this utilizes arrays, but I find trying to direct work with it causes errors.
            // Since to me it screams "use lists", relying on them instead.
            /*var materList = Control.CURRENT_PL_DATA.MATER.ToList() ??  new List<string>(){};
            var isEmpty = materList.Any();
            bool hasMater = false;
            string heldMater = "";
            int heldMaterCount = 0;
            if (!isEmpty && materList.Any(x => x.Substring(0, x.Length - 2) == actualName))
            {
                hasMater = true;
                heldMater = materList.First(x => x.Substring(0, x.Length - 2) == actualName);
                heldMaterCount = int.Parse(heldMater.Substring(heldMater.Length - 2, 2));

            }
            if (isEmpty || !hasMater)
            {
                materList.Add(actualName + "01");
            }
            else if (hasMater)
            {
                var newCount = heldMaterCount + 1;
                if (newCount > 98)
                {
                    return; // Don't add the item.
                }
                var indexPosition = materList.IndexOf(heldMater);
                if (newCount < 10)
                {
                    heldMater += "0" + newCount.ToString();
                }
                else
                {
                    heldMater += newCount.ToString();
                }
                materList[indexPosition] = heldMater;


            }
            Control.CURRENT_PL_DATA.ITEMS = materList.ToArray();*/
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