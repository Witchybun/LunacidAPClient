using System.Collections.Generic;
using System.Reflection;
using System.Runtime.CompilerServices;
using BepInEx;
using BepInEx.Logging;
using HarmonyLib;
using LunacidAP.Data;
using UnityEngine;

namespace LunacidAP
{
    public class LocationHandler
    {
        public static POP_text_scr Popup;
        private static ManualLogSource _log;

        public static void Initialize(ManualLogSource log)
        {
            _log = log;
        }

        public static void Awake()
        {
            Harmony.CreateAndPatchAll(typeof(LocationHandler));
        }

        [HarmonyPatch(typeof(Item_Pickup_scr), "Pickup")]
        [HarmonyPrefix]
        private static bool Pickup_SendLocation(Item_Pickup_scr __instance)
        {
            Popup = __instance.CON.PAPPY;
            var objectName = __instance.gameObject.name;
            var sceneName = __instance.gameObject.scene.name;
            var objectLocation = __instance.gameObject.transform.localPosition;
            if (!LunacidLocations.APLocationData.ContainsKey(sceneName))
            {
                Debug.Log($"Scene {sceneName} not implemented yet!");
            }
            var currentLocationData = LunacidLocations.APLocationData[sceneName];
            foreach (var group in currentLocationData)
            {
                Debug.Log($"{group.APLocationName}, {group.GameObjectName}, {group.Position}");
                
                
                if (group.GameObjectName == objectName && Vector3.Distance(objectLocation, group.Position) < 1f)
                {
                    Popup.POP($"Found Archipelago Item ({group.APLocationName})", 1f, 0);
                    Object.Destroy(__instance.gameObject);
                    return false;
                }
            }
            return true;
        }
    }
}