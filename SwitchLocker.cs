using System.Linq;
using BepInEx.Logging;
using HarmonyLib;
using LunacidAP.Data;
using UnityEngine;

namespace LunacidAP
{
    public class SwitchLocker
    {
        private static ManualLogSource _log;
        private static POP_text_scr _popup;

        public SwitchLocker(ManualLogSource log)
        {
            _log = log;
            Harmony.CreateAndPatchAll(typeof(SwitchLocker));
        }

        [HarmonyPatch(typeof(Act_Button_scr), "ACT")]
        [HarmonyPrefix]
        private static bool ACT_LockSwitchWithoutKey(Act_Button_scr __instance)
        {
            if (ArchipelagoClient.AP.SlotData.Switchlock == false)
            {
                return true;
            }
            var switchPosition = __instance.gameObject.transform.position;
            var sceneName = __instance.gameObject.scene.name;
            if (!LunacidSwitches.SwitchLocations.Keys.Contains(sceneName))
            {
                return true; // Location isn't relevant to switches
            }
            var itemName = DetermineSwitchName(sceneName, switchPosition);
            if (itemName == "")
            {
                return true; // Its not any relevant switch, so it shouldn't stop the player.
            }
            var control = GameObject.Find("CONTROL").GetComponent<CONTROL>();
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
                foreach (var keySpot in button.Value)
                {
                    if (Vector3.Distance(keySpot, objectPosition) < Vector3.Distance(objectPosition, positionOfShortestDistance))
                    {
                        switchOfShortestDistance = button.Key;
                        positionOfShortestDistance = keySpot;
                        shortestDistance = Vector3.Distance(keySpot, objectPosition);
                    }
                }
                
            }
            if (shortestDistance > 4f)
            {
                return ""; //Failsafe for new positions
            }
            return switchOfShortestDistance;
        }

        [HarmonyPatch(typeof(False_Wall_scr), "ACT")]
        [HarmonyPrefix]
        private static bool ACT_BlockIfNoOrb()
        {
            if (ArchipelagoClient.AP.SlotData.FalseWalls == true && !ArchipelagoClient.AP.WasItemReceived("Dusty Crystal Orb"))
            {
                var control =  GameObject.Find("CONTROL").GetComponent<CONTROL>();
                var popup = control.PAPPY;
                popup.POP("Cannot open without the Dusty Crystal Orb.", 1f, 1);
                return false;
            }
            return true;
        }

    }
}