using UnityEngine;
using LunacidAP.Archipelago;
using static LunacidAP.Data.LunacidLocations;
using LunacidAP.Patches;

namespace LunacidAP.Data
{
    public class ArchipelagoPickup: MonoBehaviour
    {
        public LocationData LocationData;
        public ArchipelagoItem ArchipelagoItem;
        public bool Collected;
        public bool CanBeRepeated;
        public Vector3 Position;

        public ArchipelagoPickup(LocationData locationData, ArchipelagoItem archipelagoItem, bool collected, bool canBeRepeated, Vector3 position)
        {
            LocationData = locationData;
            ArchipelagoItem = archipelagoItem;
            Collected = collected;
            CanBeRepeated = canBeRepeated;
            Position = position;
        }

        public void SendLocation()
        {
            if (!Collected)
            {
                LocationHandler.SendLocationCoveringPatchouliCase(LocationData);
                if (ArchipelagoItem.SlotName != ConnectionData.SlotName)
                {
                    LocationHandler.SendMessageOnPickup(ArchipelagoItem);
                }

            }
            else if (CanBeRepeated)
            {
                ArchipelagoClient.AP.SendLocationGivenLocationDataSendingGift(LocationData);
                if (ArchipelagoItem.SlotName != ConnectionData.SlotName)
                {
                    LocationHandler.SendMessageOnPickup(ArchipelagoItem);
                }
            }
        }
    }
}