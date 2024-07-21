using System;
using System.Collections;
using System.Linq;
using System.Threading.Tasks;
using Archipelago.Gifting.Net.Gifts;
using Archipelago.Gifting.Net.Gifts.Versions.Current;
using Archipelago.Gifting.Net.Traits;
using Archipelago.Gifting.Net.Utilities.CloseTraitParser;
using Archipelago.MultiClient.Net.Enums;
using BepInEx.Logging;
using HarmonyLib;
using LunacidAP.Data;
using LunacidAP.Data.ArchipelagoGiftingCases;
using UnityEngine;
using static LunacidAP.Data.LunacidGifts;

namespace LunacidAP
{
    public class GiftHelper
    {
        private ManualLogSource _log;
        private System.Random random = new(DateTime.Now.Second);
        public bool HandlingGifts = false;
        private ICloseTraitParser<string> CloseTraitParser {get; set;}
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
            ItemHandler.GiveLunacidItem(closestItem, itemFlag, player, false, overrideColor: Colors.GIFT_COLOR_DEFAULT);
        }

        public void InitializeTraits()
        {
            
            CloseTraitParser = new BKTreeCloseTraitParser<string>();
            foreach (var lunacidGiftItem in LunacidTraits.LunacidItemTraits)
            {
                CloseTraitParser.RegisterAvailableGift(lunacidGiftItem.Key, lunacidGiftItem.Value);
            }
        }

        private string ClosestLunacidItem(Gift gift)
        {
            if (CloseTraitParser is null)
            {
                InitializeTraits();
            }
            var matches = CloseTraitParser.FindClosestAvailableGift(gift.Traits);
            if (!matches.Any())
            {
                return "Silver";
            }
            else if (matches.Count() == 1)
            {
                return matches[0];
            }
            else
            {
                return matches[random.Next(matches.Count())];
            }
        }

        public static void GiftItemToOtherPlayer(ArchipelagoItem item)
        {
            var game = item.Game;
            var amount = 1;
            var itemName = item.Name;
            var itemClassification = item.Classification;
            if (game == "Stardew Valley")
            {
                if (!StardewValleyGifts.IsStardewItemGiftable(itemName))
                {
                    return; // Too many fail cases because they have not allowed a player to give them an arbitrary gift.
                }
                // If it is, we might need to package the name and amounts so Stardew Valley knows what item it actually is, in-game.
                var stardewItem = StardewValleyGifts.HandleStardewValleyItemCountAndName(itemName, out var stardewAmount);
                amount = stardewAmount;
                itemName = stardewItem;
            }
            else if (itemClassification.HasFlag(ItemFlags.Advancement))
            {
                return; //Don't resend advancement items to other games; might cause problems.
            }
            var slotName = item.SlotName;
            var madeUpItem = new GiftItem(itemName, amount, 0);
            var giftTraits = new GiftTrait[]{};
            if (itemClassification.HasFlag(ItemFlags.Trap))
            {
                giftTraits.AddToArray(new GiftTrait(GiftFlag.Trap, 1, 1)); // Stardew in particular doesn't handle direct trap names.
            }
            var packagedGift = new GiftVector(madeUpItem, giftTraits);
            ArchipelagoClient.AP.PrepareGift(packagedGift, slotName);
        }
    }
}