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
            if (apLocation.APLocationID == -1)
            {
                return true;
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
            var keepOriginalDrop = CollectLocation(__instance);
            return keepOriginalDrop;
        }

        private static bool CollectLocation(Item_Pickup_scr pickupObject)
        {
            var objectName = pickupObject.gameObject.name;
            var sceneName = pickupObject.gameObject.scene.name;
            var objectLocation = pickupObject.gameObject.transform.position;
            var apLocation = DetermineGeneralPickupLocation(pickupObject);

            if (apLocation.APLocationID == -1)
            {
                return true;
            }
            if (apLocation.APLocationName.Contains("FbA: Daedalus Knowledge"))
            {
                var actualName = TheDaedalusConundrum(pickupObject);
                apLocation = APLocationData["ARCHIVES"].First(x => x.APLocationName == actualName);
            }
            if (ArchipelagoClient.AP.IsLocationChecked(apLocation.APLocationID))
            {
                pickupObject.gameObject.SetActive(false);
                return false;
            }
            var item = ConnectionData.ScoutedLocations[apLocation.APLocationID];
            DetermineOwnerAndDirectlyGiveIfSelf(apLocation, item);
            if (item.SlotName != ConnectionData.SlotName)
            {
                var color = Colors.DetermineItemColor(item.Classification);
                _popup.POP($"Found <color={color}>{item.Name}</color> for {item.SlotName}", 1f, 0);
            }
            pickupObject.gameObject.SetActive(false);

            return false;
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
            if (sceneName == "TOWER")
            {
                if (Vector3.Distance(objectLocation, new Vector3(60.22f, -10f, -168.66f)) < 2f)
                {
                    var item1Location = string.Format("TA: Floor {0} Item 1", TheTowerDNumberDilemma(pickupObject).ToString());
                    return APLocationData["TOWER"].First(x => x.APLocationName == item1Location);
                }
                else if (Vector3.Distance(objectLocation, new Vector3(59.878f, -10f, -174.863f)) < 2f)
                {
                    var item2Location = string.Format("TA: Floor {0} Item 2", TheTowerDNumberDilemma(pickupObject).ToString());
                    return APLocationData["TOWER"].First(x => x.APLocationName == item2Location);
                }
            }
            if (objectName.Contains("(Clone)") && IsCloneAPLocation(pickupObject))
            {
                return DetermineClonePickupLocation(sceneName, objectName, objectLocation);
            }
            var type = pickupObject.type;
            return DetermineTypicalPickupLocation(sceneName, objectName, objectLocation, type);
        }

        private static int TheTowerDNumberDilemma(Item_Pickup_scr pickupObject)
        {
            var trueParent = pickupObject.transform.GetParent().GetParent();
            if (trueParent.name[0] != 'D')
            {
                return -1;
            }
            var numberInName = trueParent.name.Replace("D","");
            return 5 * int.Parse(numberInName);
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
            _popup = _popup = GameObject.Find("CONTROL").GetComponent<CONTROL>().PAPPY; // Though done before, other calls might not catch it.
            if (item.SlotName == ConnectionData.SlotName) // Handle without an internet connection.
            {
                var receivedItem = new ReceivedItem(item.Game, location.APLocationName, item.Name, item.SlotName, location.APLocationID, item.ID, item.SlotID, item.Classification);
                ConnectionData.ReceivedItems.Add(receivedItem);
                ItemHandler.GiveLunacidItem(receivedItem, true, false);
                var patchouliCanopy = LunacidLocations.APLocationData["FOREST_A1"].First(x => x.APLocationName == "YF: Patchouli's Canopy Offer").APLocationID;
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
                    ItemHandler.GiveLunacidItem(patchouliReceivedItem, true, false);
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

            }
            else  // Otherwise just save it for syncing later.
            {
                if (location.APLocationName == "YF: Patchouli's Reward")
                {
                    var patchouliCanopy = ArchipelagoClient.AP.GetLocationIDFromName("YF: Patchouli's Canopy Offer");
                    ConnectionData.CompletedLocations.Add(patchouliCanopy);
                }
                ConnectionData.CompletedLocations.Add(location.APLocationID);
            }
            return false;
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

        [HarmonyPatch(typeof(Item_Adjust), "OnEnable")]
        [HarmonyPrefix]
        private static bool OnEnable_AllowItemIfNotCollected(Item_Adjust __instance)
        {
            if (__instance.gameObject.scene.name != "TOWER")
            {
                return true;
            }
            if (__instance.ACT.name != "KILL_HEALTH" && __instance.ACT.name != "KILL_SHARD")
            {
                return true;
            }
            var locationName = "";
            if (__instance.ACT.name == "KILL_HEALTH")
            {
                locationName = string.Format("TA: Floor {0} Item 1", _currentFloor);
            }
            else if (__instance.ACT.name == "KILL_SHARD")
            {
                locationName = string.Format("TA: Floor {0} Item 2", _currentFloor);
            }
            if (ArchipelagoClient.AP.IsLocationChecked(locationName) && __instance.ACT is not null)
            {
                __instance.ACT.SetActive(true);
            }
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

        [HarmonyPatch(typeof(CRIMPUS), "OnEnable")]
        [HarmonyPrefix]
        private static bool OnEnable_ItsCRIMPUS(CRIMPUS __instance)
        {
            if (ArchipelagoClient.AP.SlotData.IsChristmas)
            {
                __instance.transform.GetChild(0).gameObject.SetActive(value: true);
                if (__instance.gameObject.scene.name == "FOREST_A1")
                {
                    //Make sure the eggnog is out frfr ong
                    __instance.transform.GetChild(0).GetChild(13).gameObject.SetActive(value: true);
                }
                return false;
            }
            return true;
        }

        private static bool IsCloneAPLocation(Item_Pickup_scr pickupObject)
        {
            return IsDropLocation(pickupObject) || IsShopLocation(pickupObject) || IsOtherCloneObjectAPRelated(pickupObject);
        }
    }
}