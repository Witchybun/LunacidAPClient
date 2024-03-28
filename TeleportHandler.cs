using System;
using System.Linq;
using BepInEx.Logging;
using HarmonyLib;
using LunacidAP.Data;
using UnityEngine;

namespace LunacidAP
{
    public class TeleportHandler
    {
        private static ManualLogSource _log;

        public static void Awake(ManualLogSource log)
        {
            _log = log;
            Harmony.CreateAndPatchAll(typeof(TeleportHandler));
        }

        [HarmonyPatch(typeof(GOTO_LEVEL), "OnEnable")]
        [HarmonyPrefix]
        private static bool OnEnable_AlterWarpBasedOnER(GOTO_LEVEL __instance)
        {
            var currentWarp = new WarpDestinations.WarpData(__instance.LVL, __instance.POS, __instance.ROT);



            var finalWarp = FixWarps(currentWarp);
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
            // Add ER option check here.
            var entrance = DetermineEntrance(warpData);
            if (entrance == "NULL")
            {
                return warpData;
            }
            
            string newEntrance = entrance;  // Add method to check entrance dictionary here.
            return WarpDestinations.EntranceToWarp[newEntrance];
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
                return WarpDestinations.EntranceToWarp.FirstOrDefault(x => AreTwoWarpsIdentical(unknownWarp, x.Value)).Key;

            }
            catch
            {
                _log.LogError("Could not find entrance for given warp.");
                return "NULL";
            }
        }
    }
}