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
        public int UniqueId { get; }

        public ReceivedItem(string locationName, string itemName, string playerName, long locationId, long itemId,
            long playerId, int uniqueId)
        {
            LocationName = locationName;
            ItemName = itemName;
            PlayerName = playerName;
            LocationId = locationId;
            ItemId = itemId;
            PlayerId = playerId;
            UniqueId = uniqueId;
        }
    }
}