using System.Linq;
using Archipelago.Gifting.Net.Gifts;
using Archipelago.MultiClient.Net.Enums;
using BepInEx.Logging;
using System.Text.RegularExpressions;
using HarmonyLib;
using LunacidAP.Data;
using UnityEngine;
using static LunacidAP.Data.LunacidLocations;
using Archipelago.Gifting.Net.Traits;
using static LunacidAP.Data.LunacidGifts;

namespace LunacidAP
{
    public class EnemyHandler
    {
        private static ManualLogSource _log;
        private const string STARDEW_RESOURCE_PREFIX = "Resource Pack: ";
        public EnemyHandler(ManualLogSource log)
        {
            _log = log;
            Harmony.CreateAndPatchAll(typeof(EnemyHandler));
        }

        [HarmonyPatch(typeof(Loot_scr), "OnEnable")]
        [HarmonyPrefix]
        private static bool OnEnable_ModifyDrops_Prefix(Loot_scr __instance)
        {
            if (ArchipelagoClient.AP.SlotData.Dropsanity == Dropsanity.Off)
            {
                return true;
            }
            float num = 0f;
            var lOOTS = __instance.LOOTS;
            NameEveryDrop(__instance.name, lOOTS);
            var nothingChance = 0f;
            if (lOOTS.Any(x=> x.ITEM is null))
            {
                nothingChance = lOOTS.First(x => x.ITEM is null).CHANCE;
            }
            var areDropsNormalized = ArchipelagoClient.AP.SlotData.NormalizedDrops;
            _log.LogInfo($"Dealing with {__instance.name}");
            if (areDropsNormalized && lOOTS.Length - 1 > 0)
            {
                num += (float)2 * nothingChance;
            }
            else
            {
                for (int i = 0; i < lOOTS.Length; i++)
                {
                    var reward = lOOTS[i];
                    num += (float)reward.CHANCE;
                }
            }

            float num2 = UnityEngine.Random.Range(0f, num);
            int num3 = 0;
            for (int j = 0; j < lOOTS.Length; j++)
            {
                if (j == 0)
                {
                    num2 -= (float)lOOTS[j].CHANCE;
                    if (num2 <= 0f)
                    {
                        num3 = j;
                        break;
                    }
                    continue;
                }
                if (areDropsNormalized)
                {
                    num2 -= (float)nothingChance/(lOOTS.Length -1);
                    if (num2 <= 0f)
                    {
                        num3 = j;
                        break;
                    }
                }
                else
                {
                    num2 -= (float)lOOTS[j].CHANCE;
                    if (num2 <= 0f)
                    {
                        num3 = j;
                        break;
                    }
                }
            }
            if (__instance.LOOTS[num3].ITEM == null)
            {
                return false;
            }
            var location = ConstructLocation(__instance.name, __instance.LOOTS[num3].ITEM.name);
            if (location == "FAIL" || location == "SETTING_DIFFERENCE" || location == "SAFE")
            {
                DropItemOnFloor(__instance.LOOTS[num3].ITEM, __instance.gameObject.transform.position);
                return false;
            }
            var locationData = GetDropLocationData(location);
            locationData = EnemyBugFix(locationData);
            if (locationData.APLocationName == "ERROR")
            {
                _log.LogError($"Location {location} doesn't exist in Archipelago!");
                return false;
            }
            var item = ConnectionData.ScoutedLocations[locationData.APLocationID];
            var isRepeatable = item.Classification == ItemFlags.None || item.Classification.HasFlag(ItemFlags.Trap);
            _log.LogInfo($"{locationData.APLocationName}, Repeatable? {isRepeatable}");
            if (ArchipelagoClient.AP.IsLocationChecked(locationData.APLocationID))
            {
                if (item.SlotName == ConnectionData.SlotName && isRepeatable)
                {
                    ItemHandler.GiveLunacidItem(item.ID, item.Classification, item.SlotName, true); // Hey its junk.  Let them grind.  Let them suffer.
                }
                else if (!item.Classification.HasFlag(ItemFlags.Advancement)) // It will always send things right now.  Make sure it doesn't break logic mkay.
                {
                    GiftItemToOtherPlayer(item.SlotName, item.Name, item.Classification);
                }
                return false;
            }
            LocationHandler.DetermineOwnerAndDirectlyGiveIfSelf(locationData, item);
            return false;
        }

        // Attempt to fix a few locations for myself during an async.
        private static LocationData EnemyBugFix(LocationData potentialData)
        {
            if (ArchipelagoClient.AP.SlotData.ClientVersion == "0.6.0" && potentialData.APLocationID == 567)
            {
                return new LocationData( 160, "AHB: Sngula Umbra's Remains", "BOOK_PICKUP" );
            }
            else if (ArchipelagoClient.AP.SlotData.ClientVersion == "0.6.0" && potentialData.APLocationID > 563)
            {
                return new LocationData(0, "ERROR");
            }
            return potentialData;
        }

        private static void NameEveryDrop(string mobName, Loot_scr.Reward[] rewards)
        {
            foreach (var reward in rewards)
            {
                var name = "";
                if (reward.ITEM is null)
                {
                    name = "NULL";
                }
                else
                {
                    name = reward.ITEM.name;
                }
                _log.LogInfo($"{mobName} drops {name}");
            }
        }

        private static LocationData GetDropLocationData(string location)
        {
            foreach (var locationData in LunacidLocations.UniqueDropLocations)
            {
                if (locationData.APLocationName == location)
                {
                    return locationData;
                }
            }
            foreach (var locationData in LunacidLocations.OtherDropLocations)
            {
                if (locationData.APLocationName == location)
                {
                    return locationData;
                }
            }
            return new LocationData(0, "ERROR");
        }

        private static string ConstructLocation(string enemyObjectName, string itemObjectName)
        {
            if (!LunacidEnemies.NamesToGameObject.TryGetValue(enemyObjectName, out var enemyName))
            {
                _log.LogWarning($"Enemy {enemyObjectName} is not in the Dictionary.");
                return "FAIL";
            }
            if (!LunacidEnemies.ObjectToLocationSuffix.TryGetValue(itemObjectName, out var itemName))
            {
                _log.LogWarning($"Item {itemObjectName} is not in the Dictionary");
                return "FAIL";
            }
            if (ArchipelagoClient.AP.SlotData.Dropsanity == Dropsanity.Unique && !LunacidItems.Weapons.Contains(itemName) && !LunacidItems.Spells.Contains(itemName))
            {
                _log.LogWarning($"Location {enemyName}: {itemName} Drop isn't in the current game's possible locations.  Giving item as normal.");
                return "SETTING_DIFFERENCE";
            }
            var constructedLocation = $"{enemyName}: {itemName} Drop";
            return constructedLocation;

        }

        private static void GiftItemToOtherPlayer(string slotName, string itemName, ItemFlags itemClassification)
        {
            
            var giftName = FixStardewValleyResourcePacks(itemName);
            _log.LogInfo($"Sending {giftName} to {slotName}");
            var madeUpItem = new GiftItem(giftName, 1, 0);
            var color = ArchipelagoClient.FlagColor(itemClassification);
            var giftTraits = new GiftTrait[]{};
            if (itemClassification.HasFlag(ItemFlags.Trap))
            {
                giftTraits.AddToArray(new GiftTrait(GiftFlag.Trap, 1, 1)); // Stardew in particular doesn't handle direct trap names.
            }
            var packagedGift = new GiftVector(madeUpItem, giftTraits);
            ArchipelagoClient.AP.PrepareGift(packagedGift, slotName, color);
        }

        private static string FixStardewValleyResourcePacks(string itemName)
        {
            if (!itemName.Contains(STARDEW_RESOURCE_PREFIX))
            {
                return itemName;
            }
            var firstPass = itemName.Replace(STARDEW_RESOURCE_PREFIX, "");
            _log.LogInfo($"{itemName} -> {firstPass}");
            var secondPass = Regex.Replace(firstPass, "[0-9]", "");
            _log.LogInfo($"{firstPass} -> {secondPass}");
            var lastPass = secondPass.Substring(1);
            _log.LogInfo($"{secondPass} -> {lastPass}");
            return lastPass;
        }

        private static void DropItemOnFloor(GameObject loot, Vector3 position)
        {
            GameObject obj = UnityEngine.Object.Instantiate(loot, position, Quaternion.identity);
            obj.SetActive(value: false);
            obj.AddComponent<Place_on_Ground>();
            obj.GetComponent<Place_on_Ground>().LOOTED = true;
            obj.SetActive(value: true);
        }
    }
}