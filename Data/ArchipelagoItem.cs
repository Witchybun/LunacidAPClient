using Archipelago.MultiClient.Net.Enums;
using Archipelago.MultiClient.Net.MessageLog.Messages;
using Archipelago.MultiClient.Net.Models;
using Newtonsoft.Json;

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
        public bool Collected;
        public ArchipelagoItem(ScoutedItemInfo item, bool received)
        {
            ID = item.ItemId;
            Name = item.ItemName;
            SlotID = item.Player;
            SlotName = ArchipelagoClient.AP.Session.Players.GetPlayerName(SlotID);
            Game = item.ItemGame;
            Classification = item.Flags;
            Collected = received;
        }

        [JsonConstructor]
        public ArchipelagoItem(long id, string name, int slotID, string slotName, string game, ItemFlags classification, bool collected)
        {
            ID = id;
            Name = name;
            SlotID = slotID;
            SlotName = slotName;
            Game = game;
            Classification = classification;
            Collected = collected;
        }
    }
}