using System.Runtime.InteropServices;
using BepInEx.Logging;
using HarmonyLib;
using LunacidAP.Data;
using UnityEngine;

namespace LunacidAP
{
    public class LocationHandler
    {
        private static POP_text_scr _popup;
        private static ArchipelagoClient _archipelago;
        private static ManualLogSource _log;
        private static int _currentFloor = 0;

        public static void Awake(ArchipelagoClient archipelago, ManualLogSource log)
        {
            _log = log;
            _archipelago = archipelago;
            Harmony.CreateAndPatchAll(typeof(LocationHandler));
        }

        [HarmonyPatch(typeof(Item_Pickup_scr), "Start")]
        [HarmonyPrefix]
        private static bool Start_MoveItemDeletionToCheckedLocations(Item_Pickup_scr __instance)
        {
            var objectName = __instance.gameObject.name;
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
            var apLocation = DetermineAPLocation(__instance.gameObject.scene.name, __instance.gameObject.name, __instance.gameObject.transform.position, __instance.type);
            if (apLocation != "" && _archipelago.IsLocationChecked(apLocation))
            {
                _log.LogInfo($"Deleting {objectName} due to already collected location {apLocation}");
                __instance.gameObject.SetActive(false);
            }
            return false;
        }

        [HarmonyPatch(typeof(Item_Pickup_scr), "Pickup")]
        [HarmonyPrefix]
        private static bool Pickup_SendLocation(Item_Pickup_scr __instance)
        {
            if (__instance.SAVED is not null)
                _log.LogInfo($"This flag is in Zone {__instance.SAVED.Zone}, with Slot {__instance.SAVED.Slot}.");
            _popup = __instance.CON.PAPPY;

            return CollectLocation(__instance);
        }

        private static bool CollectLocation(Item_Pickup_scr instance)
        {
            var objectName = instance.gameObject.name;
            var sceneName = instance.gameObject.scene.name;
            var objectLocation = instance.gameObject.transform.position;
            if (!LunacidLocations.APLocationData.ContainsKey(sceneName))
            {
                _log.LogInfo($"Scene {sceneName} not implemented yet!");
            }
            if (!ArchipelagoClient.Authenticated)
            {
                _popup.POP($"Restart the game and login first!", 1f, 0);
                return false;
            }
            var apLocation = DetermineAPLocation(sceneName, objectName, objectLocation, instance.type);
            if (apLocation != "")
            {
                var locationID = _archipelago.GetLocationIDFromName(apLocation);
                var locationInfo = _archipelago.ScoutLocation(locationID, false);
                var slotNameofItemOwner = _archipelago.Session.Players.GetPlayerName(locationInfo.Locations[0].Player);
                _archipelago.Session.Locations.CompleteLocationChecks(locationID);
                ConnectionData.CompletedLocations.Add(apLocation);
                if (ConnectionData.SlotName == slotNameofItemOwner)
                {
                    _log.LogInfo($"New location from {objectName} collected: {apLocation}");
                    instance.gameObject.SetActive(false); // Its the player's own item, offer an alternative presentation
                    return false;
                }
                else
                {
                    _popup.POP($"Found Archipelago Item ({apLocation})", 1f, 0);
                    instance.gameObject.SetActive(false); // Its someone else's item so it will only message once.
                    return false;
                }
            }
            
            _log.LogInfo("Found unaccounted for Location");
            _log.LogInfo($"Scene: {sceneName}, Object: {objectName}, Position: {objectLocation}");
            _log.LogInfo($"Name {instance.Name}, Alt Name {instance.Alt_Name}");
            return true;
        }

        private static string DetermineAPLocation(string sceneName, string objectName, Vector3 objectPosition, int type)
        {
            if (objectName.Contains("Clone"))
            {
                var isNameHandled = CloneHandler(sceneName, objectName, objectPosition, out var locationName);
                if (isNameHandled)
                {
                    _log.LogInfo($"Found Position for location [{locationName}]");
                    return locationName;
                }
            }
            var currentLocationData = LunacidLocations.APLocationData[sceneName];
            var IsWeaponOrSpell = type == 0 || type == 1; //If its unique and there's error, its fine.
            string locationOfShortestDistance = "";
            Vector3 positionOfShortestDistance = new Vector3(6969.0f, 6969.0f, 6969.0f);
            float shortestDistance = 696969f;
            foreach (var group in currentLocationData)
            {
                if (objectName == group.GameObjectName)
                {
                    if (IsWeaponOrSpell || LunacidItems.UniqueItems.Contains(objectName))
                    {
                        return group.APLocationName; //They're unique.  The location is unimportant in this case.
                    }
                    if (Vector3.Distance(group.Position, objectPosition) < Vector3.Distance(objectPosition, positionOfShortestDistance))
                    {
                        locationOfShortestDistance = group.APLocationName;
                        positionOfShortestDistance = group.Position;
                        shortestDistance = Vector3.Distance(group.Position, objectPosition);
                    }
                }
            }
            if (shortestDistance > 10f)
            {
                _log.LogInfo($"Closest location for {objectName} at {objectPosition} was too far away: {locationOfShortestDistance}, {positionOfShortestDistance} with distance {shortestDistance}");
                return ""; //Failsafe for new positions
            }
            _log.LogInfo($"Found Position for location [{locationOfShortestDistance}]");
            return locationOfShortestDistance;
        }

        private static bool CloneHandler(string sceneName, string objectName, Vector3 objectPosition, out string locationName)
        {
            objectName = objectName.Replace("(Clone)", "");
            if (sceneName != "TOWER")
            {
                if (LunacidLocations.DropLocations.ContainsKey(objectName))
                {
                    locationName = LunacidLocations.DropLocations[objectName];
                    return true;
                }
                if (LunacidLocations.ShopLocations.ContainsKey(objectName))
                {
                    locationName = LunacidLocations.ShopLocations[objectName];
                    if (sceneName == "FOREST_A1" && objectName == "OCEAN_ELIXIR_PICKUP")
                    {
                        locationName += " (Patchouli)";
                    }
                    else
                    {
                        locationName += " (Sheryl)";
                    }
                    return true;
                }
                locationName = "";
                return true;
            }
            if (objectName == "CANDLE_PICKUP")
            {
                if (_currentFloor == 20)
                {
                    locationName = "TA: Floor 20 Chest";
                }
                else
                {
                    locationName = "TA: Floor 40 Chest";
                }
                return true;
            }
            if (Vector3.Distance(new Vector3(60.2f, -10.0f, -168.7f), objectPosition) < 1f)
            {
                locationName = $"TA: Floor {_currentFloor} Health Refill";
                return true;
            }
            if (Vector3.Distance(new Vector3(59.9f, -10.0f, -174.9f), objectPosition) < 1f)
            {
                locationName = $"TA: Floor {_currentFloor} Crystal Refill";
                return true;
            }
            locationName = "";
            return false;
        }

        [HarmonyPatch(typeof(AREA_SAVED_ITEM), "Save")]
        [HarmonyPrefix]
        private static bool Save_LogAndDenySave(AREA_SAVED_ITEM __instance)
        {
            _log.LogInfo($"Data after Saving in Zone {__instance.Zone}:");
            _log.LogInfo($"Slot {__instance.Slot} is trying to increment to {__instance.value}");
            foreach (var location in LunacidFlags.ItemToFlag)
            {
                if (__instance.Zone == location.Value[0] && __instance.Slot == location.Value[1])
                {
                    _log.LogInfo($"Denying {__instance.gameObject.name} from changing data.");
                    return false;
                }
            }
            _log.LogInfo($"Allowing {__instance.gameObject.name} from changing data.");
            return true;
        }

        [HarmonyPatch(typeof(ShadowTower_CON), "Door")]
        [HarmonyPostfix]
        private static void Door_StealFloorValue(ShadowTower_CON __instance)
        {
            _currentFloor = __instance.floor;
        }
    }
}