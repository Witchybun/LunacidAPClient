using Archipelago.MultiClient.Net.Enums;
using BepInEx.Logging;
using HarmonyLib;
using UnityEngine;

namespace LunacidAP
{
    public class LivingGateHandler
    {
        private static ManualLogSource _log;
        public LivingGateHandler(ManualLogSource log)
        {
            _log = log;
            Harmony.CreateAndPatchAll(typeof(LivingGateHandler));
        }

        [HarmonyPatch(typeof(KeyTrigger), "OnTriggerEnter")]
        [HarmonyPrefix]
        private static bool OnTriggerEnter_UpdateChoosenGate(KeyTrigger __instance, Collider other)
        {
            if (other.gameObject.name == __instance.LOOKING_FOR && other.gameObject.name == "LV_GATE")
            {
                switch (other.gameObject.scene.name)
                {
                    case "PITT_A1":
                        ArchipelagoClient.AP.Session.DataStorage[Scope.Slot, "LV_GATE_BASIN"] = true;
                        break;

                    case "FOREST_A1":
                        ArchipelagoClient.AP.Session.DataStorage[Scope.Slot, "LV_GATE_FOREST"] = true;
                        break;
                }
            }

            return true;
        }
    }
}