using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Archipelago.Gifting.Net.Gifts;
using Archipelago.Gifting.Net.Gifts.Versions.Current;
using Archipelago.MultiClient.Net.Enums;
using BepInEx.Logging;
using LunacidAP.Data;
using UnityEngine;
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

        

        public IEnumerator HandleIncomingGifts()
        {
            if (ArchipelagoClient.AP.Session is null || !ArchipelagoClient.AP.IsInNormalGameState())
            {
                yield return new WaitForSeconds(10f);
            }
            var giftsTask = Task.Run(ArchipelagoClient.AP.Gifting.GetAllGiftsAndEmptyGiftboxAsync);
            yield return new WaitUntil(() => giftsTask.IsCompleted);
            if (giftsTask.IsFaulted)
            {
                _log.LogError(giftsTask.Exception.GetBaseException().Message);
                yield break;
            }
            var gifts = giftsTask.Result;
            foreach (var giftPair in gifts)
            {
                var gift = giftPair.Value;
                if (gift.IsRefund || WasGiftReceivedBefore(gift))
                {
                    continue; // You never send things which are Lunacid oriented anyway, so let them die.
                }
                var receivedGift = new ReceivedGift(gift.ItemName, gift.ID);
                ConnectionData.ReceivedGifts.Add(receivedGift);
                if (IsLunacidItem(gift, out var isTrap))
                {
                    ItemHandler.GiveLunacidItem(gift, isTrap);
                    continue;
                }
                HandleGiftingByTrait(gift);

            }
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
            if (LunacidItems.Filler.Contains(gift.ItemName) || LunacidItems.Materials.Contains(gift.ItemName))
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
            ItemHandler.GiveLunacidItem(closestItem, itemFlag, player, false, overrideColor: ArchipelagoClient.GIFT_COLOR);
        }

        private string ClosestLunacidItem(Gift gift)
        {
            var giftVector = new LunacidGifts.GiftVector(gift);
            List<string> chosenItem = new() { };
            double closenessRating = 10000000;
            var giftTraits = gift.Traits.ToList();
            foreach (var kvp in LunacidGifts.LunacidItemsToGifts)
            {
                var distance = kvp.Value.TraitDistance(giftVector);
                if (distance < closenessRating)
                {
                    chosenItem = new(){kvp.Key};
                    closenessRating = distance;
                    _log.LogInfo($"New shortest distance found!  {kvp.Key}: {distance}");
                }
                else if (distance == closenessRating)
                {
                    chosenItem.Add(kvp.Key);
                }
            }
            if (!chosenItem.Any())
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