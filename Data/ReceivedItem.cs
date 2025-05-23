using Archipelago.MultiClient.Net.Enums;
using Archipelago.MultiClient.Net.Models;
using Newtonsoft.Json;

namespace LunacidAP.Data
{ 
    public class ReceivedItem
    {
        public int Index { get;  }
        public string Game { get; }
        public string Identifier { get; }
        public string LocationName { get; }
        public string ItemName { get; }
        public string PlayerName { get; }
        public long LocationId { get; }
        public long ItemId { get; }
        public long PlayerId { get; }
        public ItemFlags Classification { get; }

        [JsonConstructor]
        public ReceivedItem(string gameName, string identifier, string locationName, string itemName, string playerName, long locationId, long itemId,
            long playerId, ItemFlags classification)
        {
            Game = gameName;
            Identifier = identifier;
            LocationName = locationName;
            ItemName = itemName;
            PlayerName = playerName;
            LocationId = locationId;
            ItemId = itemId;
            PlayerId = playerId;
            Classification = classification;
        }

        public ReceivedItem(ItemInfo item, int index)
        {
            Index = index;
            var playerName = ArchipelagoClient.AP.GetPlayerNameFromSlot(item.Player);
            Game = item.ItemGame;
            Identifier = Index + "." + item.Player.Slot + "." + item.LocationId; 
            LocationName = item.LocationName;
            ItemName = item.ItemName;
            PlayerName = playerName;
            LocationId = item.LocationId;
            ItemId = item.ItemId;
            PlayerId = item.Player.Slot;
            Classification = item.Flags;
        }
    }
}