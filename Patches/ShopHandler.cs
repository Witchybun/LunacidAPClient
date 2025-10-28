using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Archipelago.MultiClient.Net.Enums;
using BepInEx.Logging;
using HarmonyLib;
using LunacidAP.Data;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using static LunacidAP.Data.LunacidLocations;

namespace LunacidAP
{
    public class ShopHandler
    {
        private static ManualLogSource _log;
        public ShopHandler(ManualLogSource log)
        {
            _log = log;
            Harmony.CreateAndPatchAll(typeof(ShopHandler));
        }

        public static void EnsureEnchantedKey()
        {
            if (!ArchipelagoClient.AP.SlotData.Shopsanity || ArchipelagoClient.AP.IsLocationChecked("Buy Enchanted Key"))
            {
                return;
            }
            var sceneName = SceneManager.GetActiveScene().name;
            if (sceneName == "FOREST_A1" || sceneName == "CAVE")
            {
                return;
            }
            if (sceneName == "HUB_01")
            {
                var sheryls = GameObject.Find("LEVEL").transform.GetChild(8).GetChild(0);
                if (sheryls.GetChild(0).gameObject.activeSelf)
                {

                    return;
                }
                var keyItem = sheryls.GetChild(0).GetChild(4).GetChild(0).GetComponent<Shop_Inventory>().INV[0];
                if (sheryls.GetChild(1).gameObject.activeSelf)
                {
                    var shop = sheryls.GetChild(1).GetChild(4);

                    if (shop.gameObject.activeSelf && shop.GetChild(0).GetComponent<Shop_Inventory>().INV[0].item != "Enchanted Key")
                    {


                        Shop_Inventory.INV_ITEMS[] oldShopItems = shop.GetChild(0).GetComponent<Shop_Inventory>().INV;
                        var newShopItems = new List<Shop_Inventory.INV_ITEMS>
                        {
                            keyItem
                        };
                        for (var i = 0; i < oldShopItems.Length; i++)
                        {

                            newShopItems.Add(oldShopItems[i]);
                        }

                        shop.GetChild(0).GetComponent<Shop_Inventory>().INV = newShopItems.ToArray();
                    }
                    return;
                }
                if (sheryls.GetChild(2).gameObject.activeSelf)
                {
                    var shop = sheryls.GetChild(2).GetChild(4);
                    if (shop.gameObject.activeSelf && shop.GetChild(0).GetComponent<Shop_Inventory>().INV[0].item != "Enchanted Key")
                    {
                        Shop_Inventory.INV_ITEMS[] oldShopItems = shop.GetChild(0).GetComponent<Shop_Inventory>().INV;
                        var newShopItems = new List<Shop_Inventory.INV_ITEMS>
                        {
                            keyItem
                        };


                        for (var i = 0; i < oldShopItems.Length; i++)
                        {

                            newShopItems.Add(oldShopItems[i]);
                        }

                        shop.GetChild(0).GetComponent<Shop_Inventory>().INV = newShopItems.ToArray();
                    }
                    return;
                }
                _log.LogError("Could not place the Enchanted Key.");
            }
        }

        [HarmonyPatch(typeof(Shop_Inventory), "Load")]
        [HarmonyPrefix]
        private static bool Load_LoadShopItemsAnyway(Shop_Inventory __instance)
        {

            if (!ArchipelagoClient.AP.SlotData.Shopsanity)
            {
                return true;
            }
            var sceneName = __instance.gameObject.scene.name;
            for (int i = 0; i < __instance.INV.Length; i++)
            {
                var objectName = __instance.INV[i].OBJ.name;
                var shopLocation = DetermineShopLocation(sceneName, objectName);
                if (__instance.INV[i].saved_slot != -1)
                {
                    __instance.INV[i].count = int.Parse(__instance.CON.CURRENT_PL_DATA.ZONE_1.Substring(__instance.INV[i].saved_slot - 1, 1));
                }
                if (__instance.INV[i].count < 1)
                {
                    if (__instance.INV[i].Model != null)
                    {
                        __instance.INV[i].Model.SetActive(value: false);
                    }
                    if (objectName == "HEALTH_VIAL_PICKUP")
                    {
                        for (int j = 0; j < 128; j++)
                        {
                            if (__instance.CON.CURRENT_PL_DATA.ITEMS[j].Contains("Health Vial"))
                            {
                                int.TryParse(__instance.CON.CURRENT_PL_DATA.ITEMS[j].Substring(__instance.CON.CURRENT_PL_DATA.ITEMS[j].Length - 2, 2), out int result);
                                result *= result;
                                j = 999;
                                __instance.INV[i].cost = Mathf.RoundToInt(Mathf.LerpUnclamped(15f, 30f, (float)result / 70f));
                            }
                        }
                    }
                    else if (objectName == "MANA_VIAL_PICKUP")
                    {
                        for (int k = 0; k < 128; k++)
                        {
                            if (__instance.CON.CURRENT_PL_DATA.ITEMS[k].Contains("Mana Vial"))
                            {
                                int.TryParse(__instance.CON.CURRENT_PL_DATA.ITEMS[k].Substring(__instance.CON.CURRENT_PL_DATA.ITEMS[k].Length - 2, 2), out int result2);
                                result2 *= result2;
                                k = 999;
                                __instance.INV[i].cost = Mathf.RoundToInt(Mathf.LerpUnclamped(10f, 20f, (float)result2 / 70f));
                            }
                        }
                    }
                    else if (objectName == "ENKEY_PICKUP")
                    {
                        if (ArchipelagoClient.AP.IsLocationChecked(shopLocation))
                        {
                            RemoveElement(ref __instance.INV, i);
                            i--;
                        }
                    }
                    else
                    {
                        RemoveElement(ref __instance.INV, i);
                        i--;
                    }
                }
                else if (__instance.INV[i].type == 0 || __instance.INV[i].type == 2)
                {
                    if (ArchipelagoClient.AP.IsLocationChecked(shopLocation))
                    {
                        RemoveElement(ref __instance.INV, i);
                        i--;
                    }
                }
            }
            if (__instance.INV.Length < 1)
            {
                __instance.CON.SendMessage("OnINV");
                __instance.CON.PAPPY.POP("NO STOCK LEFT", 1f, 1);
            }
            else
            {
                __instance.MENU.SendMessage("LoadText", 11);
            }
            return false;
        }

        private static void RemoveElement<T>(ref T[] arr, int index)
        {
            for (int i = index; i < arr.Length - 1; i++)
            {
                arr[i] = arr[i + 1];
            }
            Array.Resize(ref arr, arr.Length - 1);
        }

        [HarmonyPatch(typeof(Menus), "LoadText")]
        [HarmonyPrefix]
        private static bool LoadText_ChangeShopTextIfShopsanity_Prefix(Menus __instance, int text2load)
        {
            if (ArchipelagoClient.ScenesNotInGame.Contains(__instance.gameObject.scene.name))
            {
                return true; // This runs on a lot of stuff; avoid a case where it would try to run on scenes where you can't have initialized AP to begin with.
            }
            if (text2load != 11)
            {
                return true;
            }
            __instance.TXT[55].text = __instance.CON.CURRENT_PL_DATA.GOLD.ToString();
            for (int n = 1; n < __instance.ITEMS[25].transform.parent.childCount; n++)
            {
                UnityEngine.Object.Destroy(__instance.ITEMS[25].transform.parent.GetChild(n).gameObject);
            }
            EventSystem.current.SetSelectedGameObject(__instance.ITEMS[25]);
            var sceneName = __instance.gameObject.scene.name;
            int lengthOfArray = 0;
            for (int i = 0; i < __instance.SHOP.INV.Length; i++)
            {
                if (__instance.SHOP.INV[i].count < 1 && __instance.SHOP.INV[i].OBJ.name != "HEALTH_VIAL_PICKUP" && __instance.SHOP.INV[i].OBJ.name != "MANA_VIAL_PICKUP")
                {
                    i = 999;
                }
                else
                {
                    lengthOfArray++;
                }
            }
            var updatedName = WasGeneralShopDisplayNameUpdated(__instance.ITEMS[25], __instance.SHOP.INV[0], sceneName, out var newName) ? newName : StaticFuncs.REMOVE_NUMS(__instance.SHOP.INV[0].item) + " - <sprite=0>" + __instance.SHOP.INV[0].cost;
            __instance.ITEMS[25].transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = updatedName;
            for (int j = 1; j < lengthOfArray; j++)
            {
                GameObject obj = UnityEngine.Object.Instantiate(__instance.ITEMS[25], __instance.ITEMS[25].transform.parent);
                obj.name = "WEP" + j;
                _ = WasGeneralShopDisplayNameUpdated(obj, __instance.SHOP.INV[j], sceneName, out var newjthName);
                obj.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = newjthName;
            }
            return false;
        }

        private static bool WasGeneralShopDisplayNameUpdated(GameObject menuUI, Shop_Inventory.INV_ITEMS shopItem, string sceneName, out string newName)
        {
            var inventoryText = shopItem.OBJ.name;
            foreach (var location in LunacidLocations.ShopLocations)
            {
                if (inventoryText == location.GameObjectName)
                {
                    var itemName = shopItem.item;
                    if (ConnectionData.ScoutedLocations.TryGetValue(location.APLocationID, out var item))
                    {
                        if (item.Classification.HasFlag(ItemFlags.Trap))
                        {
                            var random = new System.Random(ConnectionData.Seed + (int)location.APLocationID);
                            var scouts = ConnectionData.ScoutedLocations.Values.ToList();
                            var scoutedLocationsAvoidingTrapsObject = scouts.Where(x => !x.Classification.HasFlag(ItemFlags.Trap));
                            try
                            {
                                var scoutedLocationsAvoidingTraps = scoutedLocationsAvoidingTrapsObject.ToList();
                                item = scoutedLocationsAvoidingTraps[random.Next(scoutedLocationsAvoidingTraps.Count()) - 1];
                            }
                            catch
                            {
                                _log.LogError("Something went wrong with trying to find a suitable trap replacement.  Telling the truth.");
                            }
                            
                        }
                        itemName = item.Name;
                    }
                    var itemNameCondensed = CommunionHint.GetSuitableStringLength(itemName, 22);
                    if (itemNameCondensed != itemName)
                    {
                        itemNameCondensed += "...";
                    }
                    if (item is not null)
                    {
                        var color = Colors.GetClassificationHex(item.Classification);
                        itemNameCondensed = $"<color={color}>{itemNameCondensed}</color>";
                    }
                    var cost = " - <sprite=0>" + shopItem.cost.ToString();
                    if (DetermineItemCost(shopItem, sceneName) == 0)
                    {
                        cost = " - FREE!!";
                    }
                    newName = itemNameCondensed + cost;
                    return true;
                }
            }
            newName = shopItem.item + " - <sprite=0>" + shopItem.cost;
            return false;
        }

        private static int DetermineItemCost(Shop_Inventory.INV_ITEMS shopItem, string sceneName)
        {
            if (sceneName == "FOREST_A1")
            {
                if (shopItem.item == "Ocean Elixir" && ArchipelagoClient.AP.WasItemReceived("Patchouli's Drink Voucher"))
                {
                    return 0;
                }
            }
            else
            {
                if (LunacidLocations.InitialVoucherItems.Contains(shopItem.item) && ArchipelagoClient.AP.WasItemReceived("Sheryl's Initial Offerings Voucher"))
                {
                    return 0;
                }
                if (LunacidLocations.GoldenVoucherItems.Contains(shopItem.item) && ArchipelagoClient.AP.WasItemReceived("Sheryl's Golden Armor Voucher"))
                {
                    return 0;
                }
                if (LunacidLocations.DreamerVoucherItems.Contains(shopItem.item) && ArchipelagoClient.AP.WasItemReceived("Sheryl's Dreamer Voucher"))
                {
                    return 0;
                }
            }
            return shopItem.cost;
        }

        [HarmonyPatch(typeof(Shop_Inventory), "Buy")]
        [HarmonyPrefix]
        public static bool Buy_MakeSureAPItemsArePurchased(int which, Shop_Inventory __instance)
        {
            var sceneName = __instance.gameObject.scene.name;
            __instance.INV[which].cost = DetermineItemCost(__instance.INV[which], sceneName);
            var objectName = __instance.INV[which].OBJ.name;

            if (ShopLocations.All(x => x.GameObjectName == __instance.INV[which].OBJ.name) ||
                __instance.INV[which].cost > __instance.CON.CURRENT_PL_DATA.GOLD)
            {
                return true;
            }
            
            if (!ArchipelagoClient.AP.SlotData.Shopsanity)
            {
                ConnectionData.BoughtItems.Add(objectName);
                ArchipelagoClient.AP.Session.DataStorage[Scope.Slot, "BoughtItems"] = ConnectionData.BoughtItems.ToArray();
                
                return true;
            }
            
            var apLocation = DetermineShopLocation(sceneName, objectName);
            var location = +apLocation.APLocationID;
            var locationInfo = ArchipelagoClient.AP.ScoutLocation(location);
            var slotNameofItemOwner = locationInfo.SlotName;
            if (ConnectionData.SlotName != slotNameofItemOwner)
            {
                var itemName = locationInfo.Name;
                __instance.CON.PAPPY.POP($"Found {itemName} for {slotNameofItemOwner}", 1f, 0);
            }
            ArchipelagoClient.AP.Session.Locations.CompleteLocationChecks(location);
            ConnectionData.CompletedLocations.Add(location);
            return true;
        }

        [HarmonyPatch(typeof(Menus), "ItemLoad")]
        [HarmonyPostfix]
        private static void ItemLoad_ChangeDisplayNameToApItem_Postfix(Menus __instance)
        {
            if (!ArchipelagoClient.AP.SlotData.Shopsanity)
            {
                return;
            }
            if (__instance.sub_menu == 14)
            {
                var sceneName = __instance.gameObject.scene.name;
                var eqSelField = __instance.GetType().GetField("EQ_SEL", BindingFlags.Instance | BindingFlags.NonPublic);
                int EQ_SEL = (int)eqSelField.GetValue(__instance);
                var objectName = __instance.SHOP.INV[EQ_SEL].OBJ.name;
                if (!ShopLocations.Any(x => x.GameObjectName == objectName))
                {
                    return;
                }
                var apLocation = DetermineShopLocation(sceneName, objectName);
                var locationInfo = ArchipelagoClient.AP.ScoutLocation(apLocation.APLocationID);
                if (locationInfo.Classification.HasFlag(ItemFlags.Trap))
                {
                    var random = new System.Random(ConnectionData.Seed + (int)apLocation.APLocationID);
                    try
                    {
                        var scoutedLocationsAvoidingTraps = ConnectionData.ScoutedLocations.Values.ToList().Where(x => !x.Classification.HasFlag(ItemFlags.Trap)).ToList();
                        locationInfo = scoutedLocationsAvoidingTraps[random.Next(scoutedLocationsAvoidingTraps.Count)];
                    }
                    catch
                    {
                        _log.LogError("Something went wrong with trying to find a suitable trap replacement.  Telling the truth.");
                    }
                    
                }
                var slotName = locationInfo.SlotName;
                var itemName = locationInfo.Name;
                var itemNameLength = itemName.Length;
                itemName = itemName.Substring(0, Math.Min(itemNameLength, 25));
                var gameName = locationInfo.Game;
                var gameNameLength = gameName.Length;
                gameName = gameName.Substring(0, Math.Min(gameNameLength, 25));
                if (!ArchipelagoGames.GameToProtagonist.TryGetValue(gameName, out string protag))
                {
                    protag = "an unknown entity";
                }
                if (!ArchipelagoGames.GameToItemBlurb.TryGetValue(gameName, out string blurb))
                {
                    blurb = "an unknown land amongst the archipelagos.";
                }
                var totalBlurb = $"<align=center>SLOT NAME: {slotName.ToUpper()}</align>\n\nA curious object, once claimed by {protag} from {blurb}" + "  " + BlurbOnProgression(locationInfo.Classification);
                __instance.TXT[56].text = itemName.ToUpper();
                __instance.TXT[50].text = totalBlurb;
                __instance.ITEMS[26].SetActive(value: false);
                __instance.TXT[51].text = "";
                __instance.TXT[52].text = "";
                __instance.TXT[53].text = "";
                __instance.TXT[54].text = "";
            }
        }

        private static LocationData DetermineShopLocation(string sceneName, string objectName)
        {
            if (new List<string>() { "HUB_01", "FOREST_A1", "CAVE" }.Contains(sceneName))
            {
                var nameHelper = "";
                if (sceneName == "FOREST_A1" && objectName == "OCEAN_ELIXIR_PICKUP")
                {
                    nameHelper = " (Patchouli)";
                }
                else if (objectName == "OCEAN_ELIXIR_PICKUP")
                {
                    nameHelper = " (Sheryl)";
                }
                return ShopLocations.FirstOrDefault(x => x.GameObjectName == objectName &&
                x.APLocationName.Contains(nameHelper)) ?? new LocationData();
            }
            return new LocationData();
        }

        private static string BlurbOnProgression(ItemFlags flag)
        {
            if (flag.HasFlag(ItemFlags.Advancement))
            {
                return "It was a long sought after object, adored by many.";
            }
            if (flag.HasFlag(ItemFlags.NeverExclude))
            {
                return "It was perhaps one of a kind, but not unique enough to be written down in legend.";
            }
            if (flag.HasFlag(ItemFlags.None))
            {
                return "It was one among many.  Despite this, perhaps one of notable taste would still enjoy it.";
            }
            if (flag.HasFlag(ItemFlags.Trap))
            {
                return "It tempted many in its time, cursing all those would eventually carry it.";
            }
            return "It was an item found in a lost archipelago.  Never to be noted.";
        }
    }
}