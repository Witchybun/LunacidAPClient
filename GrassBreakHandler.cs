using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Security.Cryptography.X509Certificates;
using Archipelago.MultiClient.Net.Enums;
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
        private static GameObject _glowObject;
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
            _log.LogInfo($"Name: {__instance.name}, Type: {__instance.type}, DED: {__instance.DED}");
            _log.LogInfo($"Drop is {archipelagoInfo.LocationData} with item {archipelagoInfo.ArchipelagoItem.Name}.  Was it collected?  {archipelagoInfo.ArchipelagoItem.Collected}");
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
                    loot.AddComponent<ArchipelagoPickup>();
                    loot.GetComponent<ArchipelagoPickup>().LocationData = archipelagoInfo.LocationData;
                    loot.GetComponent<ArchipelagoPickup>().ArchipelagoItem = archipelagoInfo.ArchipelagoItem;
                    loot.GetComponent<ArchipelagoPickup>().Collected = archipelagoInfo.ArchipelagoItem.Collected;
                    EnemyHandler.DropItemOnFloor(loot, __instance.transform.position, archipelagoInfo);
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
                    Object.Destroy(__instance.gameObject);
                    break;
                case 1:
                    __instance.transform.localScale = Vector3.zero;
                    __instance.gameObject.SetActive(value: false);
                    break;
                case 2:
                    Object.Instantiate(Resources.Load(__instance.DED), __instance.transform.position, __instance.transform.rotation,
                    __instance.transform.parent);
                    __instance.transform.localScale = Vector3.zero;
                    __instance.gameObject.SetActive(value: false);
                    break;
            }
            return false;
        }

        public static void AddArchipelagoData(string sceneName)
        {
            if (!ArchipelagoClient.AP.Authenticated)
            {
                return;
            }
            ConstructGlowObject();
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
                if (!item.Collected)
                {
                    SetParticleSystemForObject(gameObject, item);
                }


            }
        }

        private static void ConstructGlowObject()
        {
            var ashes = GameObject.Instantiate(Resources.Load("ITEMS/ASHES") as GameObject);
            var glow = ashes.transform.GetChild(0).GetChild(1);
            _glowObject = glow.gameObject;
            GameObject.Destroy(ashes);
        }

        private static void SetParticleSystemForObject(GameObject gameObject, ArchipelagoItem item)
        {
            var usedClassification = item.Classification.HasFlag(ItemFlags.Trap) ? ItemFlags.Advancement : item.Classification;
            var colors = Colors.AllColorsToMix(usedClassification);
            if (!colors.Any())
            {
                var isFillerAdded = ConnectionData.ItemColors.TryGetValue("Filler", out var filler);
                var fillerColor = isFillerAdded ? filler : Colors.FILLER_COLOR_DEFAULT; // Its filler
                colors.Add(fillerColor);
            }
            var progressionColorInt = Colors.ColorMixer(colors);
            var color = new Color(progressionColorInt[0] / 255f, progressionColorInt[1] / 255f, progressionColorInt[2] / 255f, 1f);
            _log.LogInfo($"{gameObject} is given color {color.r}, {color.g}, {color.b}");
            var newGlow = GameObject.Instantiate(_glowObject);
            newGlow.transform.parent = gameObject.transform;
            newGlow.transform.position = gameObject.transform.position;
            var main = newGlow.GetComponent<ParticleSystemRenderer>().material;
            main.color = color;
            newGlow.gameObject.SetActive(true);
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
                var item = ConnectionData.ScoutedLocations[group.APLocationID];
                gameObject.AddComponent<ArchipelagoPickup>();
                gameObject.GetComponent<ArchipelagoPickup>().LocationData = group;
                gameObject.GetComponent<ArchipelagoPickup>().ArchipelagoItem = item;
                gameObject.GetComponent<ArchipelagoPickup>().Collected = item.Collected;
                if (!item.Collected)
                {
                    SetParticleSystemForObject(gameObject, item);
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