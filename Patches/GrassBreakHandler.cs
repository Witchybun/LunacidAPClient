using System.Collections.Generic;
using System.Linq;
using Archipelago.MultiClient.Net.Enums;
using BepInEx.Logging;
using HarmonyLib;
using LunacidAP.Archipelago;
using LunacidAP.Data;
using UnityEngine;

namespace LunacidAP.Patches
{
    public class GrassBreakHandler
    {
        private static ManualLogSource _log;
        public GrassBreakHandler(ManualLogSource log)
        {
            _log = log;
            Harmony.CreateAndPatchAll(typeof(GrassBreakHandler));
        }

        [HarmonyPatch(typeof(OBJ_HEALTH), "Die")]
        [HarmonyPrefix]
        private static bool Die_AlsoDropGrassShit(OBJ_HEALTH __instance)
        {
            var archipelagoInfo = __instance.gameObject.GetComponent<ArchipelagoPickup>();
            if (archipelagoInfo is null)
            {
                return true;
            }
            if (archipelagoInfo.LocationData is null)
            {
                return true;
            }
            LocationHandler.Pickups.Remove(archipelagoInfo);
            // Its a grass/pot.  Drop the item on the floor.
            if (__instance.type == 2 && __instance.attack_controlled != -1)
            {
                __instance.MOM.GetComponent<AI_simple>().ABH[__instance.attack_controlled].Chance.x = 0f;
            }
            if (__instance.type == 7)
            {
                __instance.MOM.SetActive(value: true);
            }
            if (__instance.dest_type == 0 && __instance.DED != "")
            {
                var dedThingObject = Resources.Load(__instance.DED) as GameObject;
                if (!archipelagoInfo.ArchipelagoItem.Collected)
                {
                    var loot = Resources.Load("ITEMS/ASHES") as GameObject;
                    GameObject.Destroy(dedThingObject.GetComponent<Loot_scr>());
                    GameObject.Instantiate(dedThingObject, __instance.transform.position, __instance.transform.rotation);
                    var finalPosition = FixPositionInEdgeCase(archipelagoInfo.LocationData.APLocationName, __instance.transform.position);
                    EnemyHandler.DropItemOnFloor(loot, finalPosition, archipelagoInfo);
                }
                else
                {

                    GameObject.Instantiate(dedThingObject, __instance.transform.position, __instance.transform.rotation);
                }
            }
            if (__instance.type == 3)
            {
                Object.Destroy(__instance.MOM);
            }
            switch (__instance.dest_type)
            {
                case 0:
                    UnityEngine.Object.Destroy(__instance.gameObject);
                    break;
                case 1:
                    __instance.transform.localScale = Vector3.zero;
                    __instance.gameObject.SetActive(value: false);
                    break;
                case 2:
                    UnityEngine.Object.Instantiate(Resources.Load(__instance.DED), __instance.transform.position, __instance.transform.rotation,
                    __instance.transform.parent);
                    __instance.transform.localScale = Vector3.zero;
                    __instance.gameObject.SetActive(value: false);
                    break;
            }
            return false;
        }

        private static Vector3 FixPositionInEdgeCase(string locationName, Vector3 originalPosition)
        {
            if (locationName == "YF: Mushroom 46 - (Yosei Forest Lower Path Secret)")
            {
                return new Vector3(-260.4886f, -20.0825f, -167.9378f);
            }
            else if (locationName == "FlA: Fiddlehead 15 - (Temple of Earth)")
            {
                return new Vector3(46.4224f, 13.02f, 201.6953f);
            }
            return originalPosition;
        }

        public static void AddArchipelagoData(string sceneName)
        {
            if (!ArchipelagoClient.AP.Authenticated)
            {
                return;
            }
            AddArchipelagoDataForGrass(sceneName);
            AddArchipelagoDataForBreakables(sceneName);
        }

        private static void AddArchipelagoDataForGrass(string sceneName)
        {
            if (!ArchipelagoClient.AP.SlotData.GrassSanity)
            {
                return;
            }
            if (!LunacidLocations.GrassLocations.TryGetValue(sceneName, out var locations))
            {
                return;
            }

            foreach (var group in locations)
            {
                var gameObject = FindClosestObjectsToPoint(group.Position);
                if (gameObject is null)
                {
                    _log.LogError($"Couldn't find a position for {group.APLocationName}.");
                    continue;
                }
                var item = SaveHandler.CurrentSaveData.ScoutedLocations[group.APLocationID];
                
                if (gameObject.GetComponent<ArchipelagoPickup>() is null)
                {
                    gameObject.AddComponent<ArchipelagoPickup>();
                    gameObject.GetComponent<ArchipelagoPickup>().LocationData = group;
                    gameObject.GetComponent<ArchipelagoPickup>().ArchipelagoItem = item;
                    gameObject.GetComponent<ArchipelagoPickup>().Collected = item.Collected;
                    gameObject.GetComponent<ArchipelagoPickup>().Position =  group.Position;
                }

                if (item.Collected) continue;
                GeneralTweaks.SetParticleSystemForObject(gameObject, item);
                LocationHandler.Pickups.Add(gameObject.GetComponent<ArchipelagoPickup>());


            }
        }

        private static void AddArchipelagoDataForBreakables(string sceneName)
        {
            if (!ArchipelagoClient.AP.SlotData.Breakables)
            {
                return;
            }
            if (!LunacidLocations.BreakLocations.TryGetValue(sceneName, out var locations))
            {
                return;
            }
            foreach (var group in locations)
            {
                var gameObject = FindClosestObjectsToPoint(group.Position);
                if (gameObject is null)
                {
                    _log.LogError($"Couldn't find a position for {group.APLocationName}.");
                    continue;
                }
                if (sceneName == "SEWER_A1")
                {
                    if (group.APLocationName == "FM: Vase 16 - (The Fetid Mire)" || group.APLocationName == "FM: Vase 17 - (The Fetid Mire)")
                    {
                        gameObject.transform.position += new Vector3(-3f, 0f, 0f); // Move the pots out of the wall.
                    }
                }
                var item = SaveHandler.CurrentSaveData.ScoutedLocations[group.APLocationID];
                if (gameObject.GetComponent<ArchipelagoPickup>() is null)
                {
                    gameObject.AddComponent<ArchipelagoPickup>();
                    gameObject.GetComponent<ArchipelagoPickup>().LocationData = group;
                    gameObject.GetComponent<ArchipelagoPickup>().ArchipelagoItem = item;
                    gameObject.GetComponent<ArchipelagoPickup>().Collected = item.Collected;
                    gameObject.GetComponent<ArchipelagoPickup>().Position =  group.Position;
                    
                }
                if (item.Collected) continue;
                GeneralTweaks.SetParticleSystemForObject(gameObject, item);
            }
        }

        private static GameObject FindClosestObjectsToPoint(Vector3 position)
        {
            var allObjects = Physics.OverlapSphere(position, 1f);
            Vector3 positionOfShortestDistance = new Vector3(6969.0f, 6969.0f, 6969.0f);
            float shortestDistance = 696969f;
            GameObject objectOfShortestDistance = new GameObject();
            foreach (var collider in allObjects)
            {
                if (Vector3.Distance(collider.transform.position, position) < Vector3.Distance(position, positionOfShortestDistance))
                {
                    objectOfShortestDistance = collider.gameObject;
                    positionOfShortestDistance = collider.transform.position;
                    shortestDistance = Vector3.Distance(collider.transform.position, position);
                }
            }
            if (shortestDistance > 5f)
            {
                return null;
            }
            return objectOfShortestDistance;
        }
    }
}