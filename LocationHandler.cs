using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using BepInEx.Logging;
using HarmonyLib;
using LunacidAP.Data;
using UnityEngine;
using static LunacidAP.Data.LunacidLocations;

namespace LunacidAP
{
    public class LocationHandler
    {
        private static POP_text_scr _popup;
        private static ManualLogSource _log;
        private static int _currentFloor = 0;
        private static string[] _kept { get; set; }


        const BindingFlags Flags = BindingFlags.Instance | BindingFlags.NonPublic;

        public static void Awake(ManualLogSource log)
        {
            _log = log;
            Harmony.CreateAndPatchAll(typeof(LocationHandler));
        }

        [HarmonyPatch(typeof(Item_Pickup_scr), "Start")]
        [HarmonyPrefix]
        private static bool Start_MoveItemDeletionToCheckedLocations(Item_Pickup_scr __instance)
        {
            var objectName = __instance.gameObject.name;
            var sceneName = __instance.gameObject.scene.name;
            var itemType = __instance.type;
            if (objectName.Contains("(Clone)"))
            {
                return true; //This will change later for mob and shops, but good to throw these examples out.
            }
            __instance.CON = GameObject.Find("CONTROL").GetComponent<CONTROL>();
            _popup = __instance.CON.PAPPY;
            if (__instance.inChest)
            {
                __instance.GetComponent<SphereCollider>().enabled = false;
                __instance.StartCoroutine("Delay");
            }
            var apLocation = DeterminePickupLocation(sceneName, objectName, __instance.gameObject.transform.position, itemType);
            if (apLocation.APLocationID != -1 && ArchipelagoClient.AP.IsLocationChecked(apLocation))
            {
                _log.LogInfo($"Deleting {objectName} due to already collected location {apLocation.APLocationName}");
                __instance.gameObject.SetActive(false);
            }
            return false;
        }

        [HarmonyPatch(typeof(Item_Pickup_scr), "Pickup")]
        [HarmonyPrefix]
        private static bool Pickup_SendLocation(Item_Pickup_scr __instance)
        {
            // if (__instance.SAVED is not null)
            //_log.LogInfo($"This flag is in Zone {__instance.SAVED.Zone}, with Slot {__instance.SAVED.Slot}.");
            _popup = __instance.CON.PAPPY;

            return CollectLocation(__instance);
        }

        private static bool CollectLocation(Item_Pickup_scr instance)
        {
            var objectName = instance.gameObject.name;
            var sceneName = instance.gameObject.scene.name;
            var objectLocation = instance.gameObject.transform.position;
            var apLocation = DeterminePickupLocation(sceneName, objectName, objectLocation, instance.type);
            if (apLocation.APLocationName.Contains("FbA: Daedalus Knowledge"))
            {
                var actualName = TheDaedalusConundrum(instance);
                apLocation = APLocationData["ARCHIVES"].First(x => x.APLocationName == actualName);
            }
            if (apLocation.APLocationID != -1)
            {
                var item = ArchipelagoClient.AP.LocationTable[apLocation.APLocationID];
                DetermineOwnerAndDirectlyGiveIfSelf(apLocation, item);
                /*var locationID = apLocation.APLocationID;
                var locationInfo = ArchipelagoClient.AP.ScoutLocation(locationID, false);
                var itemInfo = ArchipelagoClient.AP.Session.Items.GetItemName(locationInfo.Locations[0].Item);
                var slotNameofItemOwner = ArchipelagoClient.AP.Session.Players.GetPlayerName(locationInfo.Locations[0].Player);
                ArchipelagoClient.AP.Session.Locations.CompleteLocationChecks(locationID);
                var locationInfo = ArchipelagoClient.AP.LocationTable[locationID];
                var itemInfo = locationInfo.Name;
                var slotNameofItemOwner = locationInfo.SlotName;
                ConnectionData.CompletedLocations.Add(locationID);
                if (apLocation.APLocationName == "YF: Patchouli's Reward")
                {
                    var patchouliCanopy = ArchipelagoClient.AP.GetLocationIDFromName("YF: Patchouli's Canopy Offer");
                    ArchipelagoClient.AP.Session.Locations.CompleteLocationChecks(patchouliCanopy);  // Ensures its done.
                }
                ConnectionData.CompletedLocations.Add(locationID);
                if (ConnectionData.SlotName == slotNameofItemOwner)
                {
                    instance.gameObject.SetActive(false); // Its the player's own item, offer an alternative presentation
                    return false;
                }
                else
                {
                    _popup.POP($"Found {itemInfo} for {slotNameofItemOwner}", 1f, 0);
                    instance.gameObject.SetActive(false); // Its someone else's item so it will only message once.
                    return false;
                }*/
            }

            _log.LogInfo("Found unaccounted for Location");
            _log.LogInfo($"Scene: {sceneName}, Object: {objectName}, Position: {objectLocation}");
            _log.LogInfo($"Name {instance.Name}, Alt Name {instance.Alt_Name}");
            return true;
        }

        private static LocationData DeterminePickupLocation(string sceneName, string objectName, Vector3 objectPosition, int type)
        {
            if (objectName.Contains("Clone"))
            {
                var isNameHandled = CloneHandler(sceneName, objectName, objectPosition, type, out var locationName);
                if (isNameHandled)
                {
                    return locationName;
                }
            }
            var currentLocationData = APLocationData[sceneName];
            var IsWeaponOrSpell = type == 0 || type == 1; //If its unique and there's error, its fine.
            LocationData locationOfShortestDistance = new();
            Vector3 positionOfShortestDistance = new Vector3(6969.0f, 6969.0f, 6969.0f);
            float shortestDistance = 696969f;
            foreach (var group in currentLocationData)
            {
                if (objectName == group.GameObjectName && !group.IgnoreLocationHandler)
                {
                    if (IsWeaponOrSpell || LunacidItems.UniqueItems.Contains(objectName))
                    {
                        return group; //They're unique.  The location is unimportant in this case.
                    }
                    if (Vector3.Distance(group.Position, objectPosition) < Vector3.Distance(objectPosition, positionOfShortestDistance))
                    {
                        locationOfShortestDistance = group;
                        positionOfShortestDistance = group.Position;
                        shortestDistance = Vector3.Distance(group.Position, objectPosition);
                    }
                }
            }
            if (shortestDistance > 10f)
            {
                _log.LogInfo($"Closest location for {objectName} at {objectPosition} was too far away: {locationOfShortestDistance}, {positionOfShortestDistance} with distance {shortestDistance}");
                return locationOfShortestDistance; //Failsafe for new positions
            }
            // _log.LogInfo($"Found Position for location [{locationOfShortestDistance}]");
            return locationOfShortestDistance;
        }

        public static LocationData DetermineAPLocation(GameObject gameObject, int type)
        {
            return DeterminePickupLocation(gameObject.scene.name, gameObject.name, gameObject.transform.position, type);
        }

        private static bool CloneHandler(string sceneName, string objectName, Vector3 objectPosition, int type, out LocationData locationName)
        {
            objectName = objectName.Replace("(Clone)", "");
            if (sceneName != "TOWER")
            {
                if (objectName == "BOOK_PICKUP")
                {
                    locationName = APLocationData["CAS_PITT"][0];
                    return true;
                }
                var isShopsanityLocation = ShopLocations.Any(x => x.GameObjectName == objectName);
                if (new List<string>() { "HUB_01", "FOREST_A1", "CAVE" }.Contains(sceneName) && isShopsanityLocation)
                {
                    if (!SlotData.Shopsanity)
                    {
                        locationName = new LocationData();
                        return true;
                    }
                    var nameHelper = "";
                    if (sceneName == "FOREST_A1" && objectName == "OCEAN_ELIXIR_PICKUP")
                    {
                        nameHelper = " (Patchouli)";
                    }
                    else if (objectName == "OCEAN_ELIXIR_PICKUP")
                    {
                        nameHelper = " (Sheryl)";
                    }
                    locationName = ShopLocations.FirstOrDefault(x => x.GameObjectName == objectName &&
                    x.APLocationName.Contains(nameHelper)) ?? new LocationData();
                    return true;
                }
                var isDropsanityLocation = DropLocations.Any(x => x.GameObjectName == objectName);
                if (isDropsanityLocation)
                {
                    if (!SlotData.Dropsanity)
                    {
                        locationName = new LocationData();
                        return true;
                    }
                    locationName = DropLocations.FirstOrDefault(x => x.GameObjectName == objectName) ?? new LocationData();
                    return true;
                }
                locationName = new LocationData();
                return true;
            }
            else if (sceneName == "TOWER" && Vector3.Distance(objectPosition, new Vector3(68.0f, -9.5f, -172.0f)) < 2f)
            {
                var towerLocation = string.Format("TA: Floor {0} Chest", _currentFloor.ToString());
                locationName = APLocationData["TOWER"].First(x => x.APLocationName == towerLocation);
                return true;
            }
            locationName = new LocationData();
            return false;
        }

        private static string TheDaedalusConundrum(Item_Pickup_scr instance)
        {
            _log.LogInfo($"{instance.Name}");
            if (instance.Name == "FIRE WORM")
            {
                return "FbA: Daedalus Knowledge (First)";
            }
            else if (instance.Name == "BESTIAL COMMUNION")
            {
                return "FbA: Daedalus Knowledge (Second)";
            }
            else if (instance.Name == "MOON BEAM")
            {
                return "FbA: Daedalus Knowledge (Third)";
            }
            return "";
        }

        private static bool DetermineOwnerAndDirectlyGiveIfSelf(LocationData location, ArchipelagoItem item)
        {
            if (item.SlotName == ConnectionData.SlotName) // Handle without an internet connection.
            {
                var receivedItem = new ReceivedItem(location.APLocationName, item.Name, item.SlotName, location.APLocationID, item.ID, item.SlotID);
                ConnectionData.ReceivedItems.Add(receivedItem);
                ItemHandler.GiveLunacidItem(receivedItem, true);
                var patchouliCanopy = ArchipelagoClient.AP.GetLocationIDFromName("YF: Patchouli's Canopy Offer");
                ConnectionData.CompletedLocations.Add(location.APLocationID);
                
                if (ArchipelagoClient.AP.Authenticated)
                {
                    ArchipelagoClient.AP.Session.Locations.CompleteLocationChecks(location.APLocationID);
                }
                if (location.APLocationName == "YF: Patchouli's Reward" && !ArchipelagoClient.AP.IsLocationChecked(patchouliCanopy))
                {
                    var patchouliItem = ArchipelagoClient.AP.LocationTable[patchouliCanopy];
                    var patchouliReceivedItem = new ReceivedItem("YF: Patchouli's Canopy Offer", patchouliItem.Name, patchouliItem.SlotName, patchouliCanopy, patchouliItem.ID, patchouliItem.SlotID);
                    ConnectionData.ReceivedItems.Add(patchouliReceivedItem);
                    ItemHandler.GiveLunacidItem(patchouliReceivedItem, true);
                    ConnectionData.CompletedLocations.Add(patchouliCanopy);
                    if (ArchipelagoClient.AP.Authenticated)
                    {
                        ArchipelagoClient.AP.Session.Locations.CompleteLocationChecks(patchouliCanopy);
                    }
                }
            }
            else if (ArchipelagoClient.AP.Authenticated) // If someone else's item an online, do the usual
            {
                ArchipelagoClient.AP.Session.Locations.CompleteLocationChecks(location.APLocationID);
                if (location.APLocationName == "YF: Patchouli's Reward")
                {
                    var patchouliCanopy = ArchipelagoClient.AP.GetLocationIDFromName("YF: Patchouli's Canopy Offer");
                    ArchipelagoClient.AP.Session.Locations.CompleteLocationChecks(patchouliCanopy);  // Ensures its done.
                    ConnectionData.CompletedLocations.Add(patchouliCanopy);
                }
                ConnectionData.CompletedLocations.Add(location.APLocationID);
                _popup.POP($"Found {item.Name} for {item.SlotName}", 1f, 0);

            }
            else  // Otherwise just save it for syncing later.
            {
                if (location.APLocationName == "YF: Patchouli's Reward")
                {
                    var patchouliCanopy = ArchipelagoClient.AP.GetLocationIDFromName("YF: Patchouli's Canopy Offer");
                    ConnectionData.CompletedLocations.Add(patchouliCanopy);
                }
                ConnectionData.CompletedLocations.Add(location.APLocationID);
                _popup.POP($"Found {item.Name} for {item.SlotName}", 1f, 0);
            }
            return false;
        }

        [HarmonyPatch(typeof(AREA_SAVED_ITEM), "Save")]
        [HarmonyPrefix]
        private static bool Save_LogAndDenySave(AREA_SAVED_ITEM __instance)
        {
            //  _log.LogInfo($"Data after Saving in Zone {__instance.Zone}:");
            // _log.LogInfo($"Slot {__instance.Slot} is trying to increment to {__instance.value}");
            foreach (var location in LunacidFlags.ItemToFlag)
            {
                if (__instance.Zone == location.Value[0] && __instance.Slot == location.Value[1] & __instance.value == location.Value[2])
                {
                    // _log.LogInfo($"Denying {__instance.gameObject.name} from changing data.");
                    return false;
                }
            }
            // _log.LogInfo($"Allowing {__instance.gameObject.name} to change data.");
            return true;
        }

        [HarmonyPatch(typeof(ShadowTower_CON), "Door")]
        [HarmonyPostfix]
        private static void Door_StealFloorValue(ShadowTower_CON __instance)
        {
            _currentFloor = __instance.floor * 5 + __instance.room; // Its wrong until its right basically.
        }

        [HarmonyPatch(typeof(Spawn_if_moon), "OnEnable")]
        [HarmonyPostfix]
        private static void OnEnable_AllowBrokenSword(Spawn_if_moon __instance)
        {
            if (__instance.gameObject.scene.name != "ARENA")
            {
                return;
            }
            foreach (var target in __instance.TARGETS)
            {
                if (target.name == "SW")
                {
                    target.SetActive(value: true); // always let this show up
                }
            }
        }

        [HarmonyPatch(typeof(Boss), "Start")]
        [HarmonyPostfix]
        private static void Start_SendLucidCheck(Boss __instance)
        {
            var lucidID = ArchipelagoClient.AP.GetLocationIDFromName("CF: Calamis' Weapon of Choice");
            __instance.CON ??= GameObject.Find("CONTROL").GetComponent<CONTROL>();
            _popup = __instance.CON.PAPPY;
            var locationInfo = ArchipelagoClient.AP.ScoutLocation(lucidID, false);
            var itemInfo = ArchipelagoClient.AP.Session.Items.GetItemName(locationInfo.Locations[0].Item);
            var slotNameofItemOwner = ArchipelagoClient.AP.Session.Players.GetPlayerName(locationInfo.Locations[0].Player);
            ArchipelagoClient.AP.Session.Locations.CompleteLocationChecks(lucidID);
            if (ConnectionData.SlotName != slotNameofItemOwner)
            {
                _popup.POP($"Found {itemInfo} for {slotNameofItemOwner}", 1f, 0);
            }
            return;
        }

        [HarmonyPatch(typeof(Boss), "End")]
        [HarmonyPrefix]
        private static bool End_ReturnWithoutLucid(Boss __instance)
        {
            Debug.Log("END");
            string[] KEEP = (string[])__instance.GetType().GetField("KEEP", Flags).GetValue(__instance);
            _kept = KEEP;
            __instance.CON.CURRENT_PL_DATA.WEP1 = _kept[0];
            __instance.CON.CURRENT_PL_DATA.WEP2 = _kept[1];
            __instance.CON.CURRENT_PL_DATA.ITEM1 = _kept[2];
            __instance.CON.CURRENT_PL_DATA.ITEM2 = _kept[3];
            __instance.CON.CURRENT_PL_DATA.ITEM3 = _kept[4];
            __instance.CON.CURRENT_PL_DATA.ITEM4 = _kept[5];
            __instance.CON.CURRENT_PL_DATA.ITEM5 = _kept[6];
            __instance.CON.CURRENT_PL_DATA.MAG1 = _kept[7];
            __instance.CON.CURRENT_PL_DATA.MAG2 = _kept[8];
            __instance.CON.CURRENT_PL_DATA.PLAYER_H = Mathf.Max(__instance.CON.CURRENT_PL_DATA.PLAYER_H, 20f);
            __instance.CON.PL.Poison.POISON_DUR = 0.01f;
            __instance.CON.CURRENT_PL_DATA.PLAYER_B = __instance.CON.CURRENT_PL_DATA.PLAYER_H;
            __instance.CON.EQITEMS();
            __instance.CON.EQMagic();
            return false;
        }

        [HarmonyPatch(typeof(WaitAMonth), "Start")]
        [HarmonyPrefix]
        private static bool Start_WaitInstantly(WaitAMonth __instance)
        {
            __instance.transform.GetChild(0).gameObject.SetActive(value: true);
            return false;
        }
    }
}