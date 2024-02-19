using BepInEx.Logging;
using HarmonyLib;
using LunacidAP.Data;
using UnityEngine;

namespace LunacidAP
{
    public class SwitchLocker
    {
        private static ManualLogSource _log;
        private static ArchipelagoClient _archipelago;
        private static POP_text_scr _popup;

        public static void Awake(ArchipelagoClient archipelago, ManualLogSource log)
        {
            _log = log;
            _archipelago = archipelago;
            Harmony.CreateAndPatchAll(typeof(SwitchLocker));
        }

        [HarmonyPatch(typeof(Act_Button_scr), "ACT")]
        [HarmonyPrefix]
        private static bool ACT_LockSwitchWithoutKey(Act_Button_scr __instance)
        {
            if (SlotData.Switchlock == false)
            {
                return true;
            }
            var switchPosition = __instance.gameObject.transform.position;
            var sceneName = __instance.gameObject.scene.name;
            var itemName = DetermineSwitchName(sceneName, switchPosition);
            var control = GameObject.Find("CONTROL").GetComponent<CONTROL>();
            if (control is null)
            {
                _log.LogInfo("Control is null btw");
            }
            _popup = control.PAPPY;
            foreach (var receivedItem in ConnectionData.ReceivedItems)
            {
                if (receivedItem.ItemName == itemName)
                {
                    return true; // Allow the switch to function.
                }
            }
            _popup.POP($"Requires ({itemName}).", 1f, 1);
            return false; // Don't allow the switch to function.

        }

        private static string DetermineSwitchName(string sceneName, Vector3 objectPosition)
        {
            var currentLocationData = LunacidSwitches.SwitchLocations[sceneName];
            string switchOfShortestDistance = "";
            Vector3 positionOfShortestDistance = new Vector3(6969.0f, 6969.0f, 6969.0f);
            float shortestDistance = 696969f;
            foreach (var button in currentLocationData)
            {
                if (Vector3.Distance(button.Value, objectPosition) < Vector3.Distance(objectPosition, positionOfShortestDistance))
                    {
                        switchOfShortestDistance = button.Key;
                        positionOfShortestDistance = button.Value;
                        shortestDistance = Vector3.Distance(button.Value, objectPosition);
                    }
            }
            if (shortestDistance > 10f)
            {
                _log.LogInfo($"Closest location for Switch at {objectPosition} was too far away: {switchOfShortestDistance}, {positionOfShortestDistance} with distance {shortestDistance}");
                return ""; //Failsafe for new positions
            }
            _log.LogInfo($"Found Position for location [{switchOfShortestDistance}]");
            return switchOfShortestDistance;
        }
    }
}