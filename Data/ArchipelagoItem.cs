using Archipelago.MultiClient.Net.Enums;
using Archipelago.MultiClient.Net.Models;
using BepInEx.Logging;
using UnityEngine;

namespace LunacidAP.Data
{
    public class ArchipelagoItem
    {
        public long ID;
        public string Name;
        public int SlotID;
        public string SlotName;
        public string Game;
        public ItemFlags Classification;

        public ArchipelagoItem(NetworkItem item, bool received)
        {
            ID = item.Item;
            try
            {
                Name = ArchipelagoClient.AP.Session.Items.GetItemName(ID);
            }
            catch
            {
                Name = "an item";
            }
            SlotID = received ? ArchipelagoClient.AP.SlotID : item.Player;
            SlotName = ArchipelagoClient.AP.Session.Players.GetPlayerName(SlotID);
            Game = ArchipelagoClient.AP.Session.Players.Players[ArchipelagoClient.AP.Session.ConnectionInfo.Team][SlotID].Game;
            Classification = item.Flags;
        }
    }
}