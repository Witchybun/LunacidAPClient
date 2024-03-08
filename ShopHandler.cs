using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Archipelago.MultiClient.Net.Enums;
using BepInEx.Logging;
using HarmonyLib;
using LunacidAP.Data;
using UnityEngine;

namespace LunacidAP
{
    public class ShopHandler
    {
        private static ArchipelagoClient _archipelago;
        private static ManualLogSource _log;
        public static void Awake(ArchipelagoClient archipelago, ManualLogSource log)
        {
            _archipelago = archipelago;
            _log = log;
            Harmony.CreateAndPatchAll(typeof(ShopHandler));
        }

        [HarmonyPatch(typeof(Shop_Inventory), "Load")]
        [HarmonyPrefix]
        private static bool Load_LoadShopItemsAnyway(Shop_Inventory __instance)
        {

            var sceneName = __instance.gameObject.scene.name;
            for (int i = 0; i < __instance.INV.Length; i++)
            {
                var objectName = __instance.INV[i].OBJ.name;
                if (LunacidLocations.ShopLocations.TryGetValue(objectName, out string locationName))
                {
                    if (sceneName == "FOREST_A1" && objectName == "OCEAN_ELIXIR_PICKUP")
                    {
                        locationName += " (Patchouli)";
                    }
                    else if (objectName == "OCEAN_ELIXIR_PICKUP")
                    {
                        locationName += " (Sheryl)";
                    }
                    _log.LogInfo($"Got location {locationName}");
                }
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
                    else
                    {
                        RemoveElement(ref __instance.INV, i);
                        i--;
                    }
                }
                else if (__instance.INV[i].type == 0 || __instance.INV[i].type == 2)
                {
                    if (_archipelago.IsLocationChecked(locationName))
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

        [HarmonyPatch(typeof(Shop_Inventory), "Buy")]
        [HarmonyPrefix]
        public static bool Buy_MakeSureAPItemsArePurchased(int which, Shop_Inventory __instance)
        {
            if (SlotData.Shopsanity == true && LunacidLocations.ShopLocations.ContainsKey(__instance.INV[which].OBJ.name) &&
            __instance.INV[which].cost <= __instance.CON.CURRENT_PL_DATA.GOLD)
            {
                var apLocation = LunacidLocations.ShopLocations[__instance.INV[which].OBJ.name];
                var sceneName = __instance.gameObject.scene.name;
                apLocation = ThePatchouliConundrum(apLocation, sceneName);
                var location = _archipelago.GetLocationIDFromName(apLocation);
                var locationInfo = _archipelago.ScoutLocation(location, false);
                var slotNameofItemOwner = _archipelago.Session.Players.GetPlayerName(locationInfo.Locations[0].Player);
                if (ConnectionData.SlotName != slotNameofItemOwner)
                {
                    var itemInfo = _archipelago.Session.Items.GetItemName(locationInfo.Locations[0].Item);
                    __instance.CON.PAPPY.POP($"Found {itemInfo} for {slotNameofItemOwner}", 1f, 0);
                }
                _archipelago.Session.Locations.CompleteLocationChecks(location);
                ConnectionData.CompletedLocations.Add(apLocation);
            }
            return true;
        }

        [HarmonyPatch(typeof(Menus), "ItemLoad")]
        [HarmonyPostfix]
        private static void ItemLoad_ChangeDisplayNameToApItem(Menus __instance)
        {
            if (__instance.sub_menu == 14)
            {
                var sceneName = __instance.gameObject.scene.name;
                var eqSelField = __instance.GetType().GetField("EQ_SEL", BindingFlags.Instance | BindingFlags.NonPublic);
                int EQ_SEL = (int)eqSelField.GetValue(__instance);
                if (!LunacidLocations.ShopLocations.TryGetValue(__instance.SHOP.INV[EQ_SEL].OBJ.name, out string apLocation))
                {
                    return;
                }
                apLocation = ThePatchouliConundrum(apLocation, sceneName);
                var location = _archipelago.GetLocationIDFromName(apLocation);
                var locationInfo = _archipelago.ScoutLocation(location, false);
                var netItem = locationInfo.Locations[0];
                var itemName = _archipelago.GetItemNameFromID(netItem.Item).Replace("Progressive ","");
                var itemNameLength = itemName.Length;
                itemName = itemName.Substring(0, Math.Min(itemNameLength, 15));
                var gameName = _archipelago.Session.Players.Players[_archipelago.Session.ConnectionInfo.Team][netItem.Player].Game;
                var progression = ArchipelagoClient.ProgressionFlagToString[GetShopProgression(location)];
                if (!ArchipelagoGames.GameToProtagonist.TryGetValue(gameName, out string protag))
                {
                    protag = "an unknown entity";
                }
                if (!ArchipelagoGames.GameToItemBlurb.TryGetValue(gameName, out string blurb))
                {
                    blurb = "an unknown land amongst the archipelagos.";
                }
                if (!ArchipelagoGames.GameToItem.TryGetValue(gameName, out string item))
                {
                    item = "Archipelago";
                }
                var totalBlurb = $"A curious object, once claimed by {protag} from {blurb}" + "  " + BlurbOnProgression(GetShopProgression(location));
                __instance.TXT[56].text = item + " Artifact";
                __instance.TXT[50].text = totalBlurb;
                __instance.TXT[51].text = "NAME: \nGAME: \nFLAG: ";
                __instance.TXT[52].text = $"{itemName.ToUpper()}\n{gameName.ToUpper()}\n{progression.ToUpper()}";
                __instance.TXT[53].text = "";
                __instance.TXT[54].text = "";
            }
        }

        private static string ThePatchouliConundrum(string apLocation, string sceneName)
        {
            if (sceneName == "FOREST_A1" && apLocation == "Buy Ocean Elixir")
                {
                    apLocation += " (Patchouli)";
                }
                else if (apLocation == "Buy Ocean Elixir")
                {
                    apLocation += " (Sheryl)";
                }
            return apLocation;
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

        private static ItemFlags GetShopProgression(long location)
        {
            return _archipelago.ScoutLocation(location, false).Locations[0].Flags;
        }
    }
}