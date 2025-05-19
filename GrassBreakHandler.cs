using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Security.Cryptography.X509Certificates;
using BepInEx.Logging;
using HarmonyLib;
using LunacidAP.Data;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.SceneManagement;
using static LunacidAP.Data.LunacidLocations;

namespace LunacidAP
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
        private static void Die_AlsoDropGrassShit(OBJ_HEALTH __instance)
        {
            var archipelagoInfo = __instance.gameObject.GetComponent<ArchipelagoPickup>();
            if (archipelagoInfo is null)
            {
                return;
            }
            archipelagoInfo.SendLocation();
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

        public static void AddArchipelagoDataForGrass(string sceneName)
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
                var item = ConnectionData.ScoutedLocations[group.APLocationID];
                gameObject.AddComponent<ArchipelagoPickup>();
                gameObject.GetComponent<ArchipelagoPickup>().LocationData = group;
                gameObject.GetComponent<ArchipelagoPickup>().ArchipelagoItem = item;
                gameObject.GetComponent<ArchipelagoPickup>().Collected = item.Collected;
            }
        }

        public static void AddArchipelagoDataForBreakables(string sceneName)
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
                try
                {
                    var item = ConnectionData.ScoutedLocations[group.APLocationID];
                    gameObject.AddComponent<ArchipelagoPickup>();
                    gameObject.GetComponent<ArchipelagoPickup>().LocationData = group;
                    gameObject.GetComponent<ArchipelagoPickup>().ArchipelagoItem = item;
                    gameObject.GetComponent<ArchipelagoPickup>().Collected = item.Collected;
                }
                catch
                {
                    gameObject.AddComponent<ArchipelagoPickup>();
                    _log.LogError("There's no actual location for this");
                }
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
                _log.LogInfo("Found nothing.");
                return null;
            }
            _log.LogInfo($"Found something: {objectOfShortestDistance.name} at {objectOfShortestDistance.transform.position.x}, {objectOfShortestDistance.transform.position.y}, {objectOfShortestDistance.transform.position.z}");
            return objectOfShortestDistance;
        }
    }
}