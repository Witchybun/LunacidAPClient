using System;
using System.Collections.Generic;
using System.Linq;
using Archipelago.MultiClient.Net.Enums;
using BepInEx.Logging;
using HarmonyLib;
using LunacidAP.Data;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace LunacidAP
{
    public class TeleportHandler
    {
        private static ManualLogSource _log;
        public TeleportHandler(ManualLogSource log)
        {
            _log = log;
            Harmony.CreateAndPatchAll(typeof(TeleportHandler));
        }

        [HarmonyPatch(typeof(Act_Button_scr), "ACT")]
        [HarmonyPrefix]
        private static void ACT_SaveHollowBasinInformation(Act_Button_scr __instance)
        {
            if (ArchipelagoClient.AP.SlotData.StartingArea == 0)
            {
                return;
            }
            var parent = __instance.transform.parent;
            var name = parent.name;
            var scene = parent.gameObject.scene.name;
            if (name == "CRYSTAL_STATION" && scene == "PITT_A1")
            {
                if (FlagHandler.LoadFlag(2).Substring(15, 1) != "1")
                {
                    FlagHandler.ModifyFlag(2, 16, 1);
                }
            }
        }

        [HarmonyPatch(typeof(Warp_Load), "OnEnable")]
        [HarmonyPrefix]
        private static void OnEnable_FixHollowBasinButton(Warp_Load __instance)
        {
            string zONE_ = __instance.CON.CURRENT_PL_DATA.ZONE_2;
            int startingArea = ArchipelagoClient.AP.SlotData.StartingArea;
            if (__instance.gameObject.scene.name == "HUB_01" && zONE_ == "1000000000000000")
            {
                __instance.CON.CURRENT_PL_DATA.ZONE_2 = WarpDestinations.StartingAreaDataToStartingSubstring[startingArea];
                zONE_ = WarpDestinations.StartingAreaDataToStartingSubstring[startingArea];
                ItemHandler.GiveLunacidItem("Spirit Warp", ItemFlags.Advancement, "The Crystal", false);
            }
            __instance.MAP_IMG[0].color = new Color(0.5f, 0.5f, 0.5f, 1);
            __instance.STATIC_IMG[0].color = new Color(0.5608f, 0.6f, 0.698f, 0.8f);

            var num = int.Parse(zONE_.Substring(15, 1));  // Store Hollow Basin data here.
            __instance.BUTTONS[0].interactable = num == 1;
            __instance.MAP_IMG[0].color = Color.Lerp(Color.gray, Color.white, num);
            __instance.STATIC_IMG[0].color = Color.Lerp(__instance.COL[1], __instance.COL[0], num);
        }

        [HarmonyPatch(typeof(GOTO_LEVEL), "OnEnable")]
        [HarmonyPrefix]
        private static bool OnEnable_AlterWarpBasedOnER(GOTO_LEVEL __instance)
        {
            var currentWarp = new WarpDestinations.WarpData(__instance.LVL, __instance.POS, __instance.ROT);
            var eredWarp = HandleEntranceRandomizer(currentWarp);
            var finalWarp = FixWarps(eredWarp);

            _log.LogInfo($"{__instance.gameObject.scene.name} to {finalWarp.Scene}");
            if (__instance.gameObject.scene.name == "HUB_01" && finalWarp.Scene == "PITT_B1")
            {
                // Stop the player from leaving if they haven't checked the crystal yet.  
                if (FlagHandler.LoadFlag(2).Substring(0, 1) != "1")
                {
                    var control = GameObject.Find("CONTROL").GetComponent<CONTROL>();
                    control.PAPPY.POP("The crystal beckons...", 1f, 1);
                    return false;
                }
            }
            else
            {
                UpdateTraversedEntrancesAP(currentWarp);
            }
            __instance.LVL = finalWarp.Scene;
            __instance.POS = finalWarp.Position;
            __instance.ROT = finalWarp.Rotation;

            return true;
        }

        private static WarpDestinations.WarpData FixWarps(WarpDestinations.WarpData warpData)
        {
            foreach (var warp in WarpDestinations.WarpFixes)
            {
                if (AreTwoWarpsIdentical(warpData, warp.Key))
                {
                    return warp.Value;
                }
            }
            return warpData;
        }

        private static WarpDestinations.WarpData HandleEntranceRandomizer(WarpDestinations.WarpData warpData)
        {
            if (!ArchipelagoClient.AP.Authenticated)
            {
                return warpData;
            }
            if (!ArchipelagoClient.AP.SlotData.EntranceRandomizer)
            {
                return warpData;
            }
            var entrance = DetermineEntrance(warpData);
            if (entrance == "NULL" || entrance == "")
            {
                return warpData;
            }
            if (ConnectionData.Entrances.TryGetValue(entrance, out var newEntrance))
            {
                if (SceneManager.GetActiveScene().name == "HUB_01")
                {
                    return warpData;
                }
                return WarpDestinations.EntranceItsActualWarpPosition[newEntrance];
            }
            _log.LogError($"Could not find warp for {entrance}!");
            return warpData;
        }

        private static bool AreTwoWarpsIdentical(WarpDestinations.WarpData unknownWarp, WarpDestinations.WarpData warpReference)
        {
            if (unknownWarp.Scene == warpReference.Scene && Vector3.Distance(unknownWarp.Position, warpReference.Position) < 1f)
            {
                if (warpReference.ParentScene != "" && SceneManager.GetActiveScene().name != warpReference.ParentScene)
                {
                    return false;
                }
                return true;
            }
            return false;
        }

        private static string DetermineEntrance(WarpDestinations.WarpData unknownWarp)
        {
            try
            {
                var knownWarp = WarpDestinations.EntranceToWhereItShouldLead.FirstOrDefault(x => AreTwoWarpsIdentical(unknownWarp, x.Value));
                if (knownWarp.Key == null)
                {
                    return "NULL";
                }
                return knownWarp.Key;

            }
            catch
            {
                _log.LogError("Could not find entrance for given warp.");
                return "NULL";
            }
        }

        private static void UpdateTraversedEntrancesAP(WarpDestinations.WarpData from)
        {
            var fromString = DetermineEntrance(from);

            if (fromString == "NULL")
            {
                return;
            }

            _log.LogInfo("from: " + fromString);

            if (!ConnectionData.Entrances.TryGetValue(fromString, out var toString))
            {
                _log.LogError("to:   unknown");
                return;
            }
            _log.LogInfo("to:   " + toString);

            if (toString == "NULL" || fromString == toString)
            {
                return;
            }

            if (ConnectionData.TraversedEntrances.ContainsKey(fromString)
                || ConnectionData.TraversedEntrances.ContainsKey(toString))
            {
                return;
            }

            ConnectionData.TraversedEntrances.Add(fromString, toString);
            ConnectionData.TraversedEntrances.Add(toString, fromString);

            ArchipelagoClient.AP.Session.DataStorage[Scope.Slot, "TraversedEntrances"] = ConnectionData.TraversedEntrances.ToArray();
        }
    }
}