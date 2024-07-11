using System.Collections.Generic;
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

        public LocationHandler(ManualLogSource log)
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
            if (objectName.Contains("(Clone)") && !IsCloneAPLocation(__instance))
            {
                return true;
            }
            var apLocation = DetermineGeneralPickupLocation(__instance);
            if (apLocation.APLocationID == -1 || Tower_IsExcludedAndIsExcludedLocation(apLocation))
            {
                return true;
            }
            if (Coin_IsExcludedAndIsExcludedLocation(apLocation))
            {
                return false;
            }
            SwapperHandler.ReplaceModelWithAppropriateItem(__instance, apLocation);
            __instance.CON = GameObject.Find("CONTROL").GetComponent<CONTROL>();
            _popup = __instance.CON.PAPPY;
            if (__instance.inChest)
            {
                __instance.GetComponent<SphereCollider>().enabled = false;
                __instance.StartCoroutine("Delay");
            }
            __instance.gameObject.SetActive(!IsLocationCollected(apLocation));
            return false;
        }

        [HarmonyPatch(typeof(Item_Pickup_scr), "Pickup")]
        [HarmonyPrefix]
        private static bool Pickup_SendLocation(Item_Pickup_scr __instance)
        {
            _popup = __instance.CON.PAPPY;
            CollectLocation(__instance, out var keepOriginalDrop);
            return keepOriginalDrop;
        }

        private static void CollectLocation(Item_Pickup_scr pickupObject, out bool keepOriginalDrop)
        {
            var objectName = pickupObject.gameObject.name;
            var sceneName = pickupObject.gameObject.scene.name;
            var objectLocation = pickupObject.gameObject.transform.position;
            var apLocation = DetermineGeneralPickupLocation(pickupObject);
            
            if (apLocation.APLocationID == -1 || Tower_IsExcludedAndIsExcludedLocation(apLocation))
            {
                keepOriginalDrop = true;
                return;
            }
            if (Coin_IsExcludedAndIsExcludedLocation(apLocation))
            {
                keepOriginalDrop = false;
                return;
            }
            keepOriginalDrop = false;
            if (apLocation.APLocationName.Contains("FbA: Daedalus Knowledge"))
            {
                var actualName = TheDaedalusConundrum(pickupObject);
                apLocation = APLocationData["ARCHIVES"].First(x => x.APLocationName == actualName);
            }
            if (ArchipelagoClient.AP.IsLocationChecked(apLocation.APLocationID))
            {
                pickupObject.gameObject.SetActive(false);
                return;
            }
            var item = ConnectionData.ScoutedLocations[apLocation.APLocationID];
            DetermineOwnerAndDirectlyGiveIfSelf(apLocation, item);
            pickupObject.gameObject.SetActive(false);

            return;
        }

        private static LocationData DetermineGeneralPickupLocation(Item_Pickup_scr pickupObject)
        {
            var objectName = pickupObject.gameObject.name;
            var sceneName = pickupObject.gameObject.scene.name;
            var objectLocation = pickupObject.transform.position;
            if (sceneName == "HUB_01" && Vector3.Distance(objectLocation, new Vector3(-4.1f, 1.3f, -11.2f)) < 5f && pickupObject.Name != "Dusty Crystal Orb")
            {
                return new LocationData(5, "WR: Demi's Introduction Gift", "", new Vector3());
            }
            if (sceneName == "ARCHIVES" && Vector3.Distance(objectLocation, new Vector3(-3.2f, -19.3f, -45.9f)) < 5f)
            {
                return new LocationData(-2, "FbA: Daedalus Knowledge", "", new Vector3());
            }
            if (objectName.Contains("(Clone)") && IsCloneAPLocation(pickupObject))
            {
                return DetermineClonePickupLocation(sceneName, objectName, objectLocation);
            }
            var type = pickupObject.type;
            return DetermineTypicalPickupLocation(sceneName, objectName, objectLocation, type);
        }

        private static LocationData DetermineTypicalPickupLocation(string sceneName, string objectName, Vector3 objectPosition, int type)
        {
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
                return locationOfShortestDistance; //Failsafe for new positions
            }
            return locationOfShortestDistance;
        }

        public static LocationData DetermineAPLocation(GameObject gameObject, int type)
        {
            return DetermineTypicalPickupLocation(gameObject.scene.name, gameObject.name, gameObject.transform.position, type);
        }

        private static LocationData DetermineClonePickupLocation(string sceneName, string objectName, Vector3 objectPosition)
        {
            var cleanedName = objectName.Replace("(Clone)", "");
            switch (sceneName)
            {
                case "TOWER":
                    {
                        if (Vector3.Distance(objectPosition, new Vector3(68.0f, -9.5f, -172.0f)) < 2f)
                        {
                            var towerLocation = string.Format("TA: Floor {0} Chest", _currentFloor.ToString());
                            return APLocationData["TOWER"].First(x => x.APLocationName == towerLocation);
                        }
                        return new LocationData();
                    }
                case "HUB_01":
                    {
                        if (!ArchipelagoClient.AP.SlotData.Shopsanity && IsShopLocation(sceneName, objectName, objectPosition))
                        {
                            return new LocationData();
                        }
                        var nameHelper = "";
                        if (cleanedName == "OCEAN_ELIXIR_PICKUP")
                        {
                            nameHelper = " (Sheryl)";
                        }
                        var foundLocation = ShopLocations.FirstOrDefault(x => x.GameObjectName == cleanedName &&
                            x.APLocationName.Contains(nameHelper)) ?? new LocationData();
                        return foundLocation;
                    }
            }
            if (ArchipelagoClient.AP.SlotData.Shopsanity && IsShopLocation(sceneName, objectName, objectPosition))
            {
                var nameHelper = "";
                if (cleanedName == "OCEAN_ELIXIR_PICKUP" && sceneName == "FOREST_A1")
                {
                    nameHelper = " (Patchouli)";
                }
                else if (cleanedName == "OCEAN_ELIXIR_PICKUP")
                {
                    nameHelper = " (Sheryl)";
                }
                var foundLocation = ShopLocations.FirstOrDefault(x => x.GameObjectName == cleanedName &&
                    x.APLocationName.Contains(nameHelper)) ?? new LocationData();
                return foundLocation;
            }
            return new LocationData();
        }


        private static string TheDaedalusConundrum(Item_Pickup_scr instance)
        {
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

        [HarmonyPatch(typeof(Load_Area), "OnTriggerEnter")]
        [HarmonyPostfix]
        private static void OnTriggerEnter_FixDaedalusTheFuckingStinky(Load_Area __instance, Collider other)
        {
            if (__instance.gameObject.name != "LOAD_DUDE")
            {
                return;
            }
            
            if (__instance.Load.name != "STAGES")
            {
                return;
            }
            if (__instance.Load.transform.GetParent().GetChild(7).GetChild(1).gameObject.activeSelf)
            {
                __instance.Load.SetActive(false);
            }
            return;
        }

        public static bool DetermineOwnerAndDirectlyGiveIfSelf(LocationData location, ArchipelagoItem item)
        {
            if (item.SlotName == ConnectionData.SlotName) // Handle without an internet connection.
            {
                var receivedItem = new ReceivedItem(item.Game, location.APLocationName, item.Name, item.SlotName, location.APLocationID, item.ID, item.SlotID, item.Classification);
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
                    var patchouliItem = ConnectionData.ScoutedLocations[patchouliCanopy];
                    var patchouliReceivedItem = new ReceivedItem(item.Game, "YF: Patchouli's Canopy Offer", patchouliItem.Name, patchouliItem.SlotName, patchouliCanopy, patchouliItem.ID, patchouliItem.SlotID, patchouliItem.Classification);
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
                var color = ArchipelagoClient.FlagColor(item.Classification);
                _popup.POP($"Found <color={color}>{item.Name}</color> for {item.SlotName}", 1f, 0);

            }
            else  // Otherwise just save it for syncing later.
            {
                if (location.APLocationName == "YF: Patchouli's Reward")
                {
                    var patchouliCanopy = ArchipelagoClient.AP.GetLocationIDFromName("YF: Patchouli's Canopy Offer");
                    ConnectionData.CompletedLocations.Add(patchouliCanopy);
                }
                ConnectionData.CompletedLocations.Add(location.APLocationID);
                var color = ArchipelagoClient.FlagColor(item.Classification);
                _popup.POP($"Found <color={color}>{item.Name}</color> for {item.SlotName}", 1f, 0);
            }
            return false;
        }

        [HarmonyPatch(typeof(AREA_SAVED_ITEM), "Save")]
        [HarmonyPrefix]
        private static bool Save_LogAndDenySave(AREA_SAVED_ITEM __instance)
        {
            foreach (var location in LunacidFlags.ItemToFlag)
            {
                if (__instance.Zone == location.Value.Flag[0] && __instance.Slot == location.Value.Flag[1] & __instance.value == location.Value.Flag[2])
                {
                    return false;
                }
                foreach (var maxFlag in LunacidFlags.MaximumPlotFlags)
                {
                    if (__instance.Zone == maxFlag[0] && __instance.Slot == maxFlag[1] && __instance.value > maxFlag[2])
                    {
                        _log.LogWarning($"Object {__instance.name} tried to overstep its save data.  Refusing.");
                        return false;
                    }
                }
            }
            return true;
        }

        [HarmonyPatch(typeof(ShadowTower_CON), "Door")]
        [HarmonyPostfix]
        private static void Door_StealFloorValue(ShadowTower_CON __instance)
        {
            _currentFloor = __instance.floor * 5 + __instance.room; // Its wrong until its right basically.
        }

        [HarmonyPatch(typeof(ShadowTower_CON), "Tower")]
        [HarmonyPostfix]
        private static void Tower_FixChestItems(ShadowTower_CON __instance)
        {
            if (__instance.LAYOUTS[__instance.floor].REWARD != null)
            {
                __instance.CHEST.SetActive(value: true);
                __instance.CHEST.transform.GetChild(0).gameObject.SetActive(value: true);
                __instance.CHEST.transform.GetChild(1).gameObject.SetActive(value: false);
                __instance.CHEST.transform.GetChild(2).gameObject.SetActive(value: true);
                GameObject REWY = (GameObject)__instance.GetType().GetField("REWY", Flags).GetValue(__instance);
                if (REWY != null)
                {
                    UnityEngine.Object.Destroy(REWY);
                }
                REWY = UnityEngine.Object.Instantiate(__instance.LAYOUTS[__instance.floor].REWARD, __instance.CHEST.transform.GetChild(1).transform.position + Vector3.up * 0.5f, Quaternion.identity, __instance.CHEST.transform.GetChild(1).transform);
                REWY.GetComponent<Item_Pickup_scr>().inChest = true;
                REWY.GetComponent<Item_Pickup_scr>().SAVED = __instance.GetComponent<AREA_SAVED_ITEM>();
            }
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
            var locationInfo = ArchipelagoClient.AP.ScoutLocation(lucidID);
            var itemInfo = locationInfo.Name;
            var slotNameofItemOwner = locationInfo.SlotName;
            ArchipelagoClient.AP.Session.Locations.CompleteLocationChecks(lucidID);
            ConnectionData.CompletedLocations.Add(lucidID);
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
            try
            {
                __instance.CON.CURRENT_PL_DATA.ITEM1 = _kept[2];
                __instance.CON.CURRENT_PL_DATA.ITEM2 = _kept[3];
                __instance.CON.CURRENT_PL_DATA.ITEM3 = _kept[4];
                __instance.CON.CURRENT_PL_DATA.ITEM4 = _kept[5];
                __instance.CON.CURRENT_PL_DATA.ITEM5 = _kept[6];
                __instance.CON.CURRENT_PL_DATA.MAG1 = _kept[7];
                __instance.CON.CURRENT_PL_DATA.MAG2 = _kept[8];
            }
            catch
            {
                _log.LogError($"There was an error re-adding items.  Might be vanilla bug.");
            }
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

        [HarmonyPatch(typeof(Wam_scr), "OnEnable")]
        [HarmonyPrefix]
        private static bool OnEnable_FixIfOwnHarmingAxe(Wam_scr __instance)
        {
            var reelWait = __instance.GetType().GetField("reel_wait", Flags);
            reelWait.SetValue(__instance, __instance.wait);
            return false;
        }

        private static bool IsLocationCollected(LocationData apLocation)
        {
            var isItemCollected = ArchipelagoClient.AP.IsLocationChecked(apLocation);
            var isItemTrulyCollected = ArchipelagoClient.AP.Session.Locations.AllLocationsChecked.Contains(apLocation.APLocationID);
            if (isItemCollected != isItemTrulyCollected)
            {
                _log.LogWarning($"Location {apLocation.APLocationName} has collect state mismatch; {isItemCollected} vs {isItemTrulyCollected}");
            }
            return isItemCollected;
        }

        private static bool IsDropLocation(string sceneName, string objectName)
        {
            var objectNameNoClone = objectName.Replace("(Clone)", "");
            if (objectNameNoClone == "OCEAN_ELIXIR_PICKUP")
            {
                var allowedScenesForElixir = new List<string>() { "LAKE", "HAUNT" };
                if (allowedScenesForElixir.Contains(sceneName))
                {
                    return true;
                }
            }
            foreach (var location in LunacidLocations.UniqueDropLocations)
            {
                if (objectNameNoClone == location.GameObjectName)
                {
                    return true;
                }
            }
            return false;
        }

        private static bool IsDropLocation(Item_Pickup_scr pickupItem)
        {
            return IsDropLocation(pickupItem.gameObject.scene.name, pickupItem.name);
        }

        private static bool IsShopLocation(string sceneName, string objectName, Vector3 position)
        {
            var shopScenes = new List<string>() { "HUB_01", "CAVE", "FOREST_A1" };
            if (!shopScenes.Contains(sceneName))
            {
                return false;
            }
            var isObjectInShopList = false;
            foreach (var location in LunacidLocations.ShopLocations)
            {
                if (objectName.Contains(location.GameObjectName))
                {
                    isObjectInShopList = true;
                    break;
                }
            }
            if (!isObjectInShopList)
            {
                return false;
            }
            switch (sceneName)
            {
                case "HUB_01":
                    {
                        if (Vector3.Distance(position, new Vector3(-8.81f, 1.737f, 5.79f)) < 10f)
                        {
                            return true;
                        }
                        return false;
                    }
                case "CAVE":
                    {
                        if (Vector3.Distance(position, new Vector3(-94.9098f, 9.637f, -176.063f)) < 10f)
                        {
                            return true;
                        }
                        return false;
                    }
                case "FOREST_A1":
                    {
                        if (Vector3.Distance(position, new Vector3(-57.9117f, -15.4167f, -115.2767f)) < 10f)
                        {
                            return true;
                        }
                        return false;
                    }
            }
            return false;
        }

        private static bool IsShopLocation(Item_Pickup_scr pickupItem)
        {
            return IsShopLocation(pickupItem.gameObject.scene.name, pickupItem.name, pickupItem.transform.position);
        }

        private static bool IsOtherCloneObjectAPRelated(Item_Pickup_scr pickupItem)
        {
            var sceneName = pickupItem.gameObject.scene.name;
            var position = pickupItem.transform.position;
            var usedName = pickupItem.Name;
            switch (sceneName)
            {
                case "CAS_PITT":
                    {
                        if (usedName.Contains("Black Book"))
                        {
                            return true;
                        }
                        return false;
                    }
                case "TOWER":
                    {
                        if (Vector3.Distance(position, new Vector3(68f, -9.5f, -172f)) < 1f)
                        {
                            return true;
                        }
                        return false;
                    }
                case "HUB_01":
                    {
                        var allowedNames = new List<string>() { "Enchanted Key", "Oil Lantern", "Ocean Elixir" };
                        if (allowedNames.Contains(usedName))
                        {
                            return true;
                        }
                        return false;
                    }
                case "CAVE":
                    {
                        var allowedNames = new List<string>() { "Oil Lantern", "Ocean Elixir" };
                        if (allowedNames.Contains(usedName))
                        {
                            return true;
                        }
                        return false;
                    }
                case "FOREST_A1":
                    {
                        var allowedNames = new List<string>() { "Ocean Elixir" };
                        if (allowedNames.Contains(usedName))
                        {
                            return true;
                        }
                        return false;
                    }
            }
            return false;
        }

        private static bool IsCloneAPLocation(Item_Pickup_scr pickupObject)
        {
            return IsDropLocation(pickupObject) || IsShopLocation(pickupObject) || IsOtherCloneObjectAPRelated(pickupObject);
        }

        private static bool Tower_IsExcludedAndIsExcludedLocation(LocationData locationData)
        {
            return ArchipelagoClient.AP.SlotData.ExcludeTower && LunacidLocations.TowerLocations.Contains(locationData.APLocationName);
        }

        private static bool Coin_IsExcludedAndIsExcludedLocation(LocationData locationData)
        {
            return ArchipelagoClient.AP.SlotData.ExcludeCoinLocations && LunacidLocations.CoinLocations.Contains(locationData.APLocationName);
        }
    }
}