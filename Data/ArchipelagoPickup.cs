using System;
using UnityEngine;
using static LunacidAP.Data.LunacidLocations;

namespace LunacidAP.Data
{
    public class ArchipelagoPickup: MonoBehaviour
    {
        public LocationData LocationData;
        public ArchipelagoItem ArchipelagoItem;
        public bool Collected;
        public bool CanBeRepeated;

        public ArchipelagoPickup(LocationData locationData, ArchipelagoItem archipelagoItem, bool collected, bool canBeRepeated)
        {
            LocationData = locationData;
            ArchipelagoItem = archipelagoItem;
            Collected = collected;
            CanBeRepeated = canBeRepeated;
        }

        public void SendLocation()
        {
            if (!Collected)
            {
                LocationHandler.DetermineOwnerAndDirectlyGiveIfSelf(LocationData, ArchipelagoItem);
            }
            else if (CanBeRepeated)
            {
                ArchipelagoClient.AP.SendLocationGivenLocationDataSendingGift(LocationData);
            }
        }
    }
}