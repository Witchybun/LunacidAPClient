using System.Collections.Generic;
using System.Linq;
using Archipelago.MultiClient.Net.Enums;
using Archipelago.MultiClient.Net.Models;
using BepInEx.Logging;
using HarmonyLib;
using LunacidAP.Archipelago;
using LunacidAP.Data;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace LunacidAP.Patches
{
    public class ReadDialogueHelper
    {
        private static ManualLogSource _log;
        public static void Awake(ManualLogSource log)
        {
            _log = log;
            Harmony.CreateAndPatchAll(typeof(ReadDialogueHelper));
        }

        [HarmonyPatch(typeof(LoreBlock), "ACT")]
        [HarmonyPrefix]
        private static bool ACT_ExchangeLoreForHelp(LoreBlock __instance)
        {
            var scene = SceneManager.GetActiveScene().name;
            switch (scene)
            {
                case "HUB_01":
                    {
                        switch (__instance.which)
                        {
                            case 75:
                                {
                                    __instance.test_string = DetermineRequestedHints();
                                    break;
                                }
                        }
                        return true;
                    }
            }
            return true;
        }
        private static string DetermineRequestedHints()
        {
            var hintString = "<size=100%>Clint's Hintbook \n (Be sure to pick up anything in here!)</size> \n \n";
            var hints = ArchipelagoClient.AP.Session.DataStorage.GetHints(ArchipelagoClient.AP.SlotID);
            var goodHints = new List<Hint>() { };
            foreach (var hint in hints)
            {
                if (!hint.Found && hint.ItemFlags.HasFlag(ItemFlags.Advancement) && hint.FindingPlayer == ArchipelagoClient.AP.SlotID)
                {
                    goodHints.Add(hint);
                }
            }
            if (goodHints.Count == 0)
            {
                return hintString + "<size=50%>Nothing to do; good job!</size>";
            }
            foreach (var hint in goodHints)
            {
                var item = SaveHandler.CurrentSaveData.ScoutedLocations[hint.LocationId];
                var locationName = ArchipelagoClient.AP.GetLocationNameFromID(hint.LocationId);
                hintString += $"<size=50%>{item.SlotName} wants {item.Name} at {locationName}</size>\n";
            }
            return hintString;
        }

        public static void AssignPickupsForLoreInScene(string sceneName)
        {
            if (!ArchipelagoClient.AP.Authenticated) return;
            if (!ArchipelagoClient.AP.SlotData.Bookworm) return;
            if (Object.FindObjectsOfType(typeof(LoreBlock), true) is not LoreBlock[] loreSpots)
            {
                _log.LogWarning("Current scene has no LoreBlock objects.  Error?");
                return;
            }
            if (!LunacidLocations.LoreLocations.TryGetValue(sceneName, out var locations))
            {
                _log.LogWarning($"Scene {sceneName} has no books documented.  Error?");
                return;
            }

            if (sceneName == "SEWER_A1")   // ???
            {
                
            }
            
            foreach (var lore in loreSpots)
            {
                var position = lore.transform.position;
                LunacidLocations.LocationData locationOfShortestDistance = new();
                var positionOfShortestDistance = new Vector3(6969.0f, 6969.0f, 6969.0f);
                var shortestDistance = 696969f;
                
                foreach (var group in locations)
                {
                    if (Vector3.Distance(group.Position, position) < Vector3.Distance(position, positionOfShortestDistance))
                    {
                        locationOfShortestDistance = group;
                        positionOfShortestDistance = group.Position;
                        shortestDistance = Vector3.Distance(group.Position, position);
                    }
                }
                if (shortestDistance > 2f)
                {
                    _log.LogWarning($"We went through every documented spot.  {lore.transform.parent.name} isn't in there.");
                    continue;
                }
                _log.LogInfo($"Found {lore.transform.parent.name} at {position}");
                var item = SaveHandler.CurrentSaveData.ScoutedLocations[locationOfShortestDistance.APLocationID];
                lore.gameObject.AddComponent<ArchipelagoPickup>();
                lore.gameObject.GetComponent<ArchipelagoPickup>().LocationData = locationOfShortestDistance;
                lore.gameObject.GetComponent<ArchipelagoPickup>().ArchipelagoItem = item;
                lore.gameObject.GetComponent<ArchipelagoPickup>().Collected = item.Collected;
                lore.gameObject.GetComponent<ArchipelagoPickup>().Position = lore.transform.position;
                if (!item.Collected)
                {
                    LocationHandler.Pickups.Add(lore.gameObject.GetComponent<ArchipelagoPickup>());
                    GeneralTweaks.SetParticleSystemForObject(lore.gameObject, item);
                }
            }
        }
    }
}