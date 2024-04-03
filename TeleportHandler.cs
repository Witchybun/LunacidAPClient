using System;
using System.Collections;
using System.Linq;
using System.Runtime.InteropServices;
using BepInEx.Logging;
using HarmonyLib;
using LunacidAP.Data;
using UnityEngine;

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

        [HarmonyPatch(typeof(GOTO_LEVEL), "OnEnable")]
        [HarmonyPrefix]
        private static bool OnEnable_AlterWarpBasedOnER(GOTO_LEVEL __instance)
        {
            var currentWarp = new WarpDestinations.WarpData(__instance.LVL, __instance.POS, __instance.ROT);

            var eredWarp = HandleEntranceRandomizer(currentWarp);

            var finalWarp = FixWarps(eredWarp);


            __instance.LVL = finalWarp.Scene;
            __instance.POS = finalWarp.Position;
            __instance.ROT = finalWarp.Rotation;


            _log.LogInfo($"Warping to {__instance.LVL}");
            _log.LogInfo($"[{__instance.POS.x}, {__instance.POS.y}, {__instance.POS.z}]");
            _log.LogInfo($"Rotation {__instance.ROT}");

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
            if (!ArchipelagoClient.AP.SlotData.EntranceRandomizer)
            {
                return warpData;
            }
            _log.LogInfo($"Received warpdata to {warpData.Scene}");
            var entrance = DetermineEntrance(warpData);
            if (entrance == "NULL" || entrance == "")
            {
                return warpData;
            }
            _log.LogInfo($"Handling {entrance}");
            if (ConnectionData.Entrances.TryGetValue(entrance, out var newEntrance))
            {
                return WarpDestinations.EntranceToWarp[newEntrance];
            }
            else if (ConnectionData.Entrances.TryGetValue(ReverseEntrance(entrance), out var newEntranceReversed))
            {
                return WarpDestinations.EntranceToWarp[newEntranceReversed];
            }
            _log.LogError($"Could not find warp for {entrance}!");
            return warpData;
        }

        private static void WarpToStartingSpotAfterIntro()
        {
            /*var warpData = WarpDestinations.StartingArea[];
            var startingWarp = new GOTO_LEVEL{
                LVL = 
            }*/
        }

        private static bool AreTwoWarpsIdentical(WarpDestinations.WarpData unknownWarp, WarpDestinations.WarpData warpReference)
        {
            if (unknownWarp.Scene == warpReference.Scene && Vector3.Distance(unknownWarp.Position, warpReference.Position) < 1f)
            {
                return true;
            }
            return false;
        }

        private static string DetermineEntrance(WarpDestinations.WarpData unknownWarp)
        {
            try
            {
                _log.LogInfo($"Looking for {unknownWarp.Scene}, [{unknownWarp.Position.x}, {unknownWarp.Position.y}, {unknownWarp.Position.z}], {unknownWarp.Rotation}");
                var knownWarp = WarpDestinations.EntranceToWarp.FirstOrDefault(x => AreTwoWarpsIdentical(unknownWarp, x.Value));
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

        public static string ReverseEntrance(string entrance)
        {
            var entranceArray = entrance.Split(new string[] { " to " }, StringSplitOptions.None);
            var reversedEntrance = entranceArray[1] + " to " + entranceArray[0];
            return reversedEntrance;
        }
    }
}