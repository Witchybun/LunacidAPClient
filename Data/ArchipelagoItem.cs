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
        public ItemFlags Classification;

        public ArchipelagoItem(NetworkItem item, bool received)
        {
            
            Plugin.LOG.LogInfo("Making item");
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
            Classification = item.Flags;
        }
    }
}