using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using Archipelago.MultiClient.Net;
using BepInEx;
using BepInEx.Logging;
using HarmonyLib;
using LunacidAP.Data;
using UnityEngine;

namespace LunacidAP
{
    public class LocationHandler
    {
        private static POP_text_scr _popup;
        private static ArchipelagoClient _archipelago;
        private static ManualLogSource _log;

        public static void Awake(ArchipelagoClient archipelago, ManualLogSource log)
        {
            _log = log;
            _archipelago = archipelago;
            Harmony.CreateAndPatchAll(typeof(LocationHandler));
        }

        [HarmonyPatch(typeof(Item_Pickup_scr), "Start")]
        [HarmonyPrefix]
        private static bool Start_MoveItemDeletionToCheckedLocations(Item_Pickup_scr __instance)
        {
            var objectName = __instance.gameObject.name;
            if (objectName.Contains("(Clone)"))
            {
                return true; //This will change later for mob and shops, but good to throw these examples out.
            }
            __instance.CON = GameObject.Find("CONTROL").GetComponent<CONTROL>();
            _popup = __instance.CON.PAPPY;
            if (__instance.inChest)
            {
                __instance.GetComponent<SphereCollider>().enabled = false;
                __instance.StartCoroutine("Delay");
            }

            var apLocation = DetermineAPLocation(__instance.gameObject.scene.name, __instance.Name, __instance.gameObject.transform.position, __instance.type);
            _log.LogInfo($"This location with {objectName} at {__instance.gameObject.transform.position} gives {apLocation}.");
            if (apLocation != "" && _archipelago.IsLocationChecked(apLocation))
            {
                Object.Destroy(__instance.gameObject);
            }
            
            /* The rest of this is vanilla code; for future use as drops needs to be handled differently
            switch (__instance.type)
            {
                case 0:
                    {
                        string[] sPELLS = __instance.CON.CURRENT_PL_DATA.WEPS;
                        foreach (string check in sPELLS)
                        {
                            if (!StaticFuncs.IS_NULL(check))
                            {
                                if (StaticFuncs.REMOVE_NUMS(check).ToUpper().Replace(" ", "") == __instance.Name.ToUpper().Replace(" ", ""))
                                {
                                    Object.Destroy(__instance.gameObject);
                                }
                                else if (StaticFuncs.REMOVE_NUMS(check).ToUpper().Replace(" ", "") == __instance.Alt_Name.ToUpper().Replace(" ", ""))
                                {
                                    Object.Destroy(__instance.gameObject);
                                }
                            }
                        }
                        break;
                    }
                case 1:
                    {
                        string[] sPELLS = __instance.CON.CURRENT_PL_DATA.SPELLS;
                        for (int i = 0; i < sPELLS.Length; i++)
                        {
                            if (sPELLS[i] == __instance.Name)
                            {
                                Object.Destroy(__instance.gameObject);
                            }
                        }
                        break;
                    }
                case 4:
                    {
                        StreamReader streamReader = new StreamReader(Application.dataPath + "/Resources/TXT/" + PlayerPrefs.GetString("LANG", "ENG") + "/MATERIALS.txt");
                        string text = streamReader.ReadToEnd();
                        streamReader.Close();
                        string[] array = text.Split("|"[0]);
                        __instance.Name = array[int.Parse(__instance.Alt_Name)];
                        __instance.Name = __instance.Name.Replace("\n", "");
                        break;
                    }
                case 2:
                case 3:
                    break;
            }
            */
            return false;
        }

        [HarmonyPatch(typeof(Item_Pickup_scr), "Pickup")]
        [HarmonyPrefix]
        private static bool Pickup_SendLocation(Item_Pickup_scr __instance)
        {
            if (__instance.SAVED is not null)
                _log.LogInfo($"This flag is in Zone {__instance.SAVED.Zone}, with Slot {__instance.SAVED.Slot}.");
            _popup = __instance.CON.PAPPY;

            return CollectLocation(__instance);
        }

        private static bool CollectLocation(Item_Pickup_scr instance)
        {
            var objectName = instance.gameObject.name;
            if (objectName.Contains("(Clone)"))
            {
                return true; //This will change later for mob and shops, but good to throw these examples out.
            }
            var sceneName = instance.gameObject.scene.name;
            var objectLocation = instance.gameObject.transform.localPosition;
            if (!LunacidLocations.APLocationData.ContainsKey(sceneName))
            {
                _log.LogInfo($"Scene {sceneName} not implemented yet!");
            }
            if (!ArchipelagoClient.Authenticated)
            {
                _popup.POP($"Restart the game and login first!", 1f, 0);
                return false;
            }
            var apLocation = DetermineAPLocation(sceneName, objectName, objectLocation, instance.type);
            if (apLocation != "")
            {
                var locationID = _archipelago.GetLocationIDFromName(apLocation);
                var locationInfo = _archipelago.ScoutLocation(locationID, false);
                var slotNameofItemOwner = _archipelago.Session.Players.GetPlayerName(locationInfo.Locations[0].Player);
                _archipelago.Session.Locations.CompleteLocationChecks(locationID);
                ConnectionData.CompletedLocations.Add(apLocation);
                if (ConnectionData.SlotName == slotNameofItemOwner)
                {
                    Object.Destroy(instance.gameObject); //It was already collected and this exists in error, remove it.
                    return false;
                }
                else
                {
                    _popup.POP($"Found Archipelago Item ({apLocation})", 1f, 0);
                    Object.Destroy(instance.gameObject);
                    return false;
                }
            }
            
            _log.LogInfo("Found unaccounted for Location");
            _log.LogInfo($"Scene: {sceneName}, Object: {objectName}, Position: {objectLocation}");
            _log.LogInfo($"Name {instance.Name}, Alt Name {instance.Alt_Name}");
            return true;
        }

        private static string DetermineAPLocation(string sceneName, string objectName, Vector3 objectPosition, int type)
        {
            var currentLocationData = LunacidLocations.APLocationData[sceneName];
            var IsWeaponOrSpell = type == 0 || type == 1; //If its unique and there's error, its fine.
            foreach (var group in currentLocationData)
            {
                if (Vector3.Distance(objectPosition, group.Position) < 2f)
                {
                    return group.APLocationName; // The position is almost always useful.
                }
                else if (!group.GameObjectName.Contains("(Clone)") && IsWeaponOrSpell && group.GameObjectName == objectName)
                {
                    return group.APLocationName;  // Worst case, attempt to pull it by its name.
                }
            }
            return "";
        }

        /*[HarmonyPatch(typeof(AREA_SAVED_ITEM), "Load")]
        [HarmonyPostfix]
        private static void Load_PrintData(AREA_SAVED_ITEM __instance)
        {
            _log.LogInfo($"Loaded Data in Zone {__instance.Zone}:");
            _log.LogInfo($"{__instance.current_string}");
        }*/

        [HarmonyPatch(typeof(AREA_SAVED_ITEM), "Save")]
        [HarmonyPostfix]
        private static void Save_PrintData(AREA_SAVED_ITEM __instance)
        {
            _log.LogInfo($"Data after Saving in Zone {__instance.Zone}:");
            _log.LogInfo($"{__instance.current_string}");
        }
    }
}