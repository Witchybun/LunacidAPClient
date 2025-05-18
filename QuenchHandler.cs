using BepInEx.Logging;
using HarmonyLib;
using LunacidAP.Data;
using UnityEngine;

namespace LunacidAP
{
    public class QuenchHandler
    {
        private static ManualLogSource _log;

        public QuenchHandler(ManualLogSource log)
        {
            _log = log;
            Harmony.CreateAndPatchAll(typeof(QuenchHandler));
        }

        [HarmonyPatch(typeof(Menus), "Click")]
        [HarmonyPrefix]
        private static bool Click_SendWeaponCheck(Menus __instance, int which)
        {
            if (!ArchipelagoClient.AP.Authenticated)
            {
                return true;
            }
            __instance.CON ??= GameObject.Find("CONTROL").GetComponent<CONTROL>();
            if (!ArchipelagoClient.AP.SlotData.Quenchsanity)
            {
                return true;
            }
            if (which != 28 || __instance.current_query != 7)
            {
                return true;
            }
            var weaponToCheck = StaticFuncs.REMOVE_NUMS(__instance.CON.EQ_WEP.name);
            var locationData = new LunacidLocations.LocationData(-1, "ERROR", "ERROR");
            foreach (var location in LunacidLocations.QuenchLocation)
            {
                if (location.GameObjectName == weaponToCheck)
                {
                    locationData = location;
                    break;
                }
            }
            if (locationData.APLocationName == "ERROR")
            {
                _log.LogError($"Could not find recipe location for {weaponToCheck}!");
                return false;
            }
            var item = ConnectionData.ScoutedLocations[locationData.APLocationID];
            if (!ArchipelagoClient.AP.IsLocationChecked(locationData.APLocationID))
            {
                LocationHandler.DetermineOwnerAndDirectlyGiveIfSelf(locationData, item);
            }
            __instance.CON.EQ_WEP.cooling = 0f;
            __instance.CON.PAPPY.POP(item.Name + " CREATED", 1f, 7);
            if (weaponToCheck == "DEATH SCYTHE" || weaponToCheck == "BRITTLE ARMING SWORD")
            {
                __instance.CON.RemoveItem(__instance.CON.EQ_WEP.name, __instance.CON.EQ_WEP.UPGRADE);
            }
            __instance.CON.SendMessage("OnINV");
            return false;
        }
    }
}