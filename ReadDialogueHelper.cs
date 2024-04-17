using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Archipelago.MultiClient.Net.Enums;
using Archipelago.MultiClient.Net.Models;
using BepInEx.Logging;
using HarmonyLib;
using LunacidAP.Data;
using UnityEngine;
using UnityEngine.Networking.Match;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements.Experimental;

namespace LunacidAP
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
            if (goodHints.Count() == 0)
            {
                return hintString + "<size=50%>Nothing to do; good job!</size>";
            }
            foreach (var hint in goodHints)
            {
                var item = ConnectionData.ScoutedLocations[hint.LocationId];
                var locationName = ArchipelagoClient.AP.GetLocationNameFromID(hint.LocationId);
                hintString += $"<size=50%>{item.SlotName} wants {item.Name} at {locationName}</size>\n";
            }
            return hintString;
        }

        [HarmonyPatch(typeof(Dialog), "Bye")]
        [HarmonyPrefix]
        private static bool Bye_MoveStateAhead(Dialog __instance)
        {
            var sceneName = __instance.gameObject.scene.name;
            _log.LogInfo($"{__instance.npc_name}");

            return true;
        }
    }
}