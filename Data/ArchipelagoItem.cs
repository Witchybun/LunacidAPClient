using Archipelago.MultiClient.Net.Enums;
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

        public ArchipelagoItem(ScoutedItemInfo item, bool received)
        {
            ID = item.ItemId;
            Name = item.ItemName;
            SlotID = received ? ArchipelagoClient.AP.SlotID : item.Player;
            SlotName = ArchipelagoClient.AP.Session.Players.GetPlayerName(SlotID);
            Game = item.ItemGame;
            Classification = item.Flags;
        }

        [JsonConstructor]
        public ArchipelagoItem(long id, string name, int slotID, string slotName, string game, ItemFlags classification)
        {
            ID = id;
            Name = name;
            SlotID = slotID;
            SlotName = slotName;
            Game = game;
            Classification = classification;
        }
    }
}