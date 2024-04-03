using Archipelago.MultiClient.Net.Enums;
using Archipelago.MultiClient.Net.Models;
using Newtonsoft.Json;

namespace LunacidAP.Data
{ 
    public class ReceivedItem
    {
        public string LocationName { get; }
        public string ItemName { get; }
        public string PlayerName { get; }
        public long LocationId { get; }
        public long ItemId { get; }
        public long PlayerId { get; }
        public ItemFlags Classification { get; }

        [JsonConstructor]
        public ReceivedItem(string locationName, string itemName, string playerName, long locationId, long itemId,
            long playerId, ItemFlags classification)
        {
            LocationName = locationName;
            ItemName = itemName;
            PlayerName = playerName;
            LocationId = locationId;
            ItemId = itemId;
            PlayerId = playerId;
            Classification = classification;
        }

        public ReceivedItem(NetworkItem item)
        {
            var locationName = ArchipelagoClient.AP.GetLocationNameFromID(item.Location);
            var itemName = ArchipelagoClient.AP.GetItemNameFromID(item.Item);
            var playerName = ArchipelagoClient.AP.GetPlayerNameFromSlot(item.Player);
            LocationName = locationName;
            ItemName = itemName;
            PlayerName = playerName;
            LocationId = item.Location;
            ItemId = item.Item;
            PlayerId = item.Player;
            Classification = item.Flags;
        }
    }
}