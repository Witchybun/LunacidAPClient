using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Archipelago.Gifting.Net.Versioning.Gifts;
using Archipelago.Gifting.Net.Versioning.Gifts.Current;
using Archipelago.Gifting.Net.Traits;
using Archipelago.Gifting.Net.Utilities.CloseTraitParser;
using Archipelago.MultiClient.Net.Enums;
using BepInEx.Logging;
using HarmonyLib;
using LunacidAP.Archipelago;
using LunacidAP.Data;
using LunacidAP.Data.ArchipelagoGiftingCases;
using UnityEngine;
using static LunacidAP.Data.LunacidGifts;

namespace LunacidAP.Patches
{
    public class GiftHelper
    {
        private ManualLogSource _log;
        private System.Random random = new(DateTime.Now.Second);
        private ICloseTraitParser<string> CloseTraitParser {get; set;}
        public GiftHelper(ManualLogSource log)
        {
            _log = log;
            LunacidTraits.DesiredTraits = LunacidTraits.GetDesiredTraits();
        }

        

        public IEnumerator HandleIncomingGifts()
        {
            while (ArchipelagoClient.AP.allowCoroutines)
            {
                while (!ArchipelagoClient.GiftsToProcess.Any() || ArchipelagoClient.AP.Session is null)
                {
                    if (!ArchipelagoClient.AP.allowCoroutines)
                    {
                        ArchipelagoClient.GiftsToProcess.Clear();
                        yield break;
                    }
                    yield return new WaitForSeconds(1f);
                }
                while (!ArchipelagoClient.AP.IsInNormalGameState())
                {
                    yield return new WaitForSeconds(1f);
                }
                var gift = ArchipelagoClient.GiftsToProcess.Dequeue();
                ArchipelagoClient.AP.Gifting.RemoveGiftFromGiftBox(gift.ID);
                if (gift.IsRefund)
                {
                    continue; // You never send things which are Lunacid oriented anyway, so let them die.
                }
                if (gift.Traits.Any(x => x.Trait == "OthersGift"))
                {
                    GiveRandomLunacidItem(gift);
                    yield return new WaitForSeconds(1f);
                    continue;
                }
                if (IsLunacidItem(gift, out var isTrap))
                {
                    ItemHandler.GiveLunacidItem(gift, isTrap);
                    yield return new WaitForSeconds(1f);
                    continue;
                }
                HandleGiftingByTrait(gift);
                yield return new WaitForSeconds(1f);
            }
            
        }

        private void GiveRandomLunacidItem(Gift gift)
        {
            var isTrap = gift.Traits.Any(x => x.Trait == "Trap");
            var player = ArchipelagoClient.AP.Session.Players.GetPlayerName(gift.SenderSlot);
            if (isTrap)
            {
                var trapCount = LunacidItems.Traps.Count;
                var chosenTrap = LunacidItems.Traps[UnityEngine.Random.Range(0, trapCount)];
                ItemHandler.GiveLunacidItem(chosenTrap, ItemFlags.Trap, player, false, Colors.GetGiftColor(), false, true);
                return;
            }
            var giftChoices = LunacidItems.Filler.Concat(LunacidItems.Materials).ToList();
            var itemCount = giftChoices.Count;
            var chosenItem = giftChoices[UnityEngine.Random.Range(0, itemCount)];
            ItemHandler.GiveLunacidItem(chosenItem, ItemFlags.None, player, false, Colors.GetGiftColor(), false, true);

        }
        
        private static bool IsLunacidItem(Gift gift, out bool isTrap)
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
            ItemHandler.GiveLunacidItem(closestItem, itemFlag, player, overrideColor: Colors.GetGiftColor(), isGift: true);
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
            if (gift.IsRefund && gift.Traits.Any(x => x.Trait == "OthersGift"))
            {
                return "The Weight of the Dream (Nothing)";
            }
            if (CloseTraitParser is null)
            {
                InitializeTraits();
            }

            if (CloseTraitParser == null) return "The Weight of the Dream (Nothing)";
            var matches = CloseTraitParser.FindClosestAvailableGift(gift.Traits);
            if (!matches.Any())
            {
                return "Silver";
            }
            return matches.Count() == 1 ? matches[0] : matches[random.Next(matches.Count())];
        }

        public static bool GiftItemToOtherPlayer(ArchipelagoItem item)
        {
            var game = item.Game;
            var amount = 1;
            var itemName = item.Name;
            var itemClassification = item.Classification;
            if (game == "Stardew Valley")
            {
                if (!StardewValleyGifts.IsStardewItemGiftable(itemName))
                {
                    return false; // Too many fail cases because they have not allowed a player to give them an arbitrary gift.
                }
                // If it is, we might need to package the name and amounts so Stardew Valley knows what item it actually is, in-game.
                var stardewItem = StardewValleyGifts.HandleStardewValleyItemCountAndName(itemName, out var stardewAmount);
                amount = stardewAmount;
                itemName = stardewItem;
            }
            else if (itemClassification.HasFlag(ItemFlags.Advancement))
            {
                return false; //Don't resend advancement items to other games; might cause problems.
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
            return true;
        }

        public static IEnumerator GiftRandomGiftToRandomPlayer(string itemName)
        {
            Plugin.LOG.LogInfo("Starting to give random gift.");
            bool isTrap = itemName.Contains("Patchouli's Gift");
            var allPlayers = ArchipelagoClient.AP.Session.Players.AllPlayers.Select(x => x.Slot).ToList();
            allPlayers.Remove(ArchipelagoClient.AP.SlotID);
            Plugin.LOG.LogInfo($"We have {allPlayers.Count} players.");
            if (!allPlayers.Any())
            {
                yield break;
            }

            var giftablePlayers = new List<int>();
            foreach (var player in allPlayers)
            {
                var canGiftTask = ArchipelagoClient.AP.Gifting.CanGiftToPlayerAsync(player);
                yield return new WaitUntil(() => canGiftTask.IsCompleted);
                if (canGiftTask.IsFaulted)
                {
                    continue;
                }

                if (canGiftTask.Result.CanGift)
                {
                    giftablePlayers.Add(player);
                }
            }
            Plugin.LOG.LogInfo($"Do we have any players we could possibly gift to?  {giftablePlayers.Count}");
            if (!giftablePlayers.Any())
            {
                yield break;
            }
            var count = giftablePlayers.Count;
            var chosenPlayer = giftablePlayers[UnityEngine.Random.Range(0, count-1)];
            Plugin.LOG.LogInfo("Seeing if we can use OthersGift or not.");
            var acceptableTraits = ArchipelagoClient.AP.Gifting.GetAcceptedTraits(chosenPlayer, LunacidTraits.DesiredTraits); // Simple enough to just base it off our own traits, they're pretty generic.
            if (!acceptableTraits.Any())
            {
                acceptableTraits = ArchipelagoClient.AP.Gifting.GetDesiredTraits(chosenPlayer);  // If the list is empty that must mean not only do they have a desired trait list it doesn't include any of ours.
            }
            var acceptableCount = acceptableTraits.Traits.Length;
            var randomTrait = acceptableTraits.Traits[UnityEngine.Random.Range(0, acceptableCount)];
            var randomGiftTrait = new List<GiftTrait>
            {
                new("OthersGift"),
                new(randomTrait)
            };
            if (isTrap)
            {
                randomGiftTrait.Add(new GiftTrait(GiftFlag.Trap));
            }
            var finalRandomArray =  randomGiftTrait.ToArray();
            var amount = UnityEngine.Random.Range(1, 11);
            var giftItem = new GiftItem(itemName, amount, 1);
            var playerName = ArchipelagoClient.AP.Session.Players.GetPlayerName(chosenPlayer);
            ArchipelagoClient.AP.Gifting.SendGift(giftItem, finalRandomArray, playerName);
            
        }
    }
}