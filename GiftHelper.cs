using System;
using System.Collections.Generic;
using System.Linq;
using Archipelago.Gifting.Net.Gifts.Versions.Current;
using Archipelago.MultiClient.Net.Enums;
using BepInEx.Logging;
using LunacidAP.Data;
using static LunacidAP.Data.LunacidGifts;

namespace LunacidAP
{
    public class GiftHelper
    {
        private ManualLogSource _log;
        private System.Random random = new(DateTime.Now.Second);
        public bool HandlingGifts = false;
        public GiftHelper(ManualLogSource log)
        {
            _log = log;
        }

        public void HandleIncomingGifts()
        {
            var giftBoxCheck = ArchipelagoClient.AP.Gifting.CheckGiftBox();
            if (!giftBoxCheck.Any())
            {
                return;
            }
            var gifts = ArchipelagoClient.AP.Gifting.GetAllGiftsAndEmptyGiftbox();
            foreach (var giftPair in gifts)
            {
                var gift = giftPair.Value;
                if (gift.IsRefund || WasGiftReceivedBefore(gift))
                {
                    continue; // You never send things which are Lunacid oriented anyway, so let them die.
                }
                if (IsLunacidItem(gift, out var isTrap))
                {
                    ItemHandler.GiveLunacidItem(gift, isTrap);
                    continue;
                }
                HandleGiftingByTrait(gift);
                var receivedGift = new ReceivedGift(gift.ItemName, gift.ID);
                ConnectionData.ReceivedGifts.Add(receivedGift);

            }
            return;
        }

        private bool WasGiftReceivedBefore(Gift gift)
        {
            foreach (var receivedGift in ConnectionData.ReceivedGifts)
            {
                if (receivedGift.ID == gift.ID)
                {
                    return true;
                }
            }
            return false;
        }

        private bool IsLunacidItem(Gift gift, out bool isTrap)
        {
            if (LunacidItems.Items.Contains(gift.ItemName) || LunacidItems.Materials.Contains(gift.ItemName))
            {
                isTrap = false;
                return true;
            }
            if (LunacidItems.Traps.Contains(gift.ItemName))
            {
                isTrap = true;
                return true;
            }
            isTrap = false;
            return false;
        }

        private void HandleGiftingByTrait(Gift gift)
        {
            var closestItem = ClosestLunacidItem(gift);
            var itemFlag = LunacidItems.Traps.Contains(closestItem) ? ItemFlags.Trap : ItemFlags.None;
            var player = ArchipelagoClient.AP.GetPlayerNameFromSlot(gift.SenderSlot);
            ItemHandler.GiveLunacidItem(closestItem, itemFlag, player, false);
        }

        private string ClosestLunacidItem(Gift gift)
        {
            List<string> chosenItem = new() { };
            double closenessRating = 0;
            var giftTraits = gift.Traits.ToList();
            foreach (var kvp in LunacidTraits.LunacidItemTraits)
            {
                var item = kvp.Key;
                var traits = kvp.Value;
                var intersection = giftTraits.Intersect(traits).ToList();
                var totalMatches = intersection.Count();
                var errors = 0.4f * traits.Where(x => giftTraits.Contains(x)).Count() + 0.1f * giftTraits.Where(x => traits.Contains(x)).Count();
                if (closenessRating < totalMatches - errors)
                {
                    closenessRating = totalMatches - errors;
                    chosenItem = new() { item };
                    _log.LogInfo($"Item: {item}, Total Matches: {totalMatches}, Errors: {errors}");
                }
                else if (closenessRating == totalMatches - errors)
                {
                    chosenItem.Add(item);
                }
            }
            if (chosenItem.Any())
            {
                return "Silver";
            }
            else if (chosenItem.Count() == 1)
            {
                return chosenItem[0];
            }
            else
            {
                return chosenItem[random.Next(chosenItem.Count())];
            }
        }
    }
}