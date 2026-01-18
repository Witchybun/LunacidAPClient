using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using Archipelago.MultiClient.Net.Enums;
using Archipelago.MultiClient.Net.Packets;
using BepInEx.Logging;
using HarmonyLib;
using LunacidAP.Archipelago;
using LunacidAP.Data;
using Newtonsoft.Json.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngineInternal.Input;

namespace LunacidAP.Patches
{
    public class GeneralTweaks
    {
        // Should be for any fixes or changes to code that aren't necessarily AP oriented but change something.
        private static ManualLogSource _log;
        private static long LucidID = 319;
        private static string[] _kept { get; set; }
        
        public static GameObject _glowObject;
        const BindingFlags Flags = BindingFlags.Instance | BindingFlags.NonPublic;

        public GeneralTweaks(ManualLogSource log)
        {
            _log = log;
            Harmony.CreateAndPatchAll(typeof(GeneralTweaks));
        }
        
        public static void ConstructGlowObject()
        {
            var ashes = Resources.Load("ITEMS/ASHES") as GameObject;
            var glow = ashes.transform.GetChild(0).GetChild(1);
            _glowObject = glow.gameObject;
        }
        
        public static void SetParticleSystemForObject(GameObject gameObject, ArchipelagoItem item)
        {
            var flag = item.Classification;
            if (item.Classification.HasFlag(ItemFlags.Trap))
            {
                var choices = new List<ItemFlags>() {ItemFlags.Advancement | ItemFlags.NeverExclude, ItemFlags.Advancement, ItemFlags.NeverExclude, ItemFlags.None };
                flag = choices[ArchipelagoClient.AP.RandomStatic.Next(choices.Count() - 1)];
            }
            var hexColor = Colors.GetClassificationHex(flag);
            var color = Colors.HexToColorConverter(hexColor);
            var newGlow = GameObject.Instantiate(_glowObject, gameObject.transform, true);
            newGlow.name = "MW Item Glow";
            newGlow.transform.position = gameObject.transform.position;
            var main = newGlow.GetComponent<ParticleSystemRenderer>().material;
            main.color = color;
            newGlow.gameObject.SetActive(true);
        }

        [HarmonyPatch(typeof(AREA_SAVED_ITEM), "Save")]
        [HarmonyPrefix]
        private static bool Save_LogAndDenySave(AREA_SAVED_ITEM __instance)
        {
            foreach (var location in LunacidFlags.ItemToFlag)
            {
                if (__instance.Zone == location.Value.Flag[0] && __instance.Slot == location.Value.Flag[1] & __instance.value == location.Value.Flag[2])
                {
                    return false;
                }
                foreach (var maxFlag in LunacidFlags.MaximumPlotFlags)
                {
                    if (__instance.Zone == maxFlag[0] && __instance.Slot == maxFlag[1] && __instance.value > maxFlag[2])
                    {
                        _log.LogWarning($"Object {__instance.name} tried to overstep its save data.  Refusing.");
                        return false;
                    }
                }
            }
            return true;
        }

        [HarmonyPatch(typeof(Spawn_if_moon), "OnEnable")]
        [HarmonyPostfix]
        private static void OnEnable_AllowBrokenSword(Spawn_if_moon __instance)
        {
            if (__instance.gameObject.scene.name != "ARENA")
            {
                return;
            }
            foreach (var target in __instance.TARGETS)
            {
                if (target.name == "SW")
                {
                    target.SetActive(value: true); // always let this show up
                }
            }
        }

        [HarmonyPatch(typeof(Boss), "Start")]
        [HarmonyPostfix]
        private static void Start_SendLucidCheck(Boss __instance)
        {
            var locationData = new LunacidLocations.LocationData(319, "CF: Calamis' Weapon of Choice", ignoreLocationHandler: true);
            var archipelagoItem = SaveHandler.CurrentSaveData.ScoutedLocations[319];
            LocationHandler.SendLocationCoveringPatchouliCase(locationData);
            if (archipelagoItem.SlotName != SaveHandler.CurrentSaveData.SlotName)
            {
                LocationHandler.SendMessageOnPickup(archipelagoItem);
            }
        }

        [HarmonyPatch(typeof(Player_Control_scr), "Die")]
        [HarmonyPostfix]
        private static void Die_FalsifyDLTag()
        {
            if (!SaveHandler.CurrentSaveData.DeathLink) return;
            ArchipelagoClient.AP.SendDeathLink();
        }

        [HarmonyPatch(typeof(I2.Loc.LanguageSourceData), "TryGetTranslation")]
        [HarmonyPrefix]
        private static bool TryGetTranslation_AddSpecialCases(string term, out string Translation, string overrideLanguage = null, string overrideSpecialization = null, bool skipDisabled = false, bool allowCategoryMistmatch = false)
        {
            Translation = "";
            switch (term)
            {
                case "Weapons/THORN":
                    {
                        Translation = "THORN";
                        return false;
                    }
                case "Weapon Descriptions/THORN details":
                    {
                        Translation = "Sheryl the Crow's weapon, from a time before she even came to the Great Well.  It seems she has no use for it anymore.";
                        return false;
                    }
            }
            return true;
        }

        //No Sword For you Lol
        [HarmonyPatch(typeof(Boss), "Start")]
        [HarmonyPrefix]
        private static bool Start_KillPlayerNoSword(Boss __instance, ref Vector3 ___START_POS, ref string[] ___KEEP, ref Animation ___ANIM, ref Coroutine ___ROUT)
        {
            ___START_POS = __instance.transform.position;
            ___KEEP = new string[10];
            ___KEEP[0] = __instance.CON.CURRENT_PL_DATA.WEP1;
            ___KEEP[1] = __instance.CON.CURRENT_PL_DATA.WEP2;
            ___KEEP[2] = __instance.CON.CURRENT_PL_DATA.ITEM1;
            ___KEEP[3] = __instance.CON.CURRENT_PL_DATA.ITEM2;
            ___KEEP[4] = __instance.CON.CURRENT_PL_DATA.ITEM3;
            ___KEEP[5] = __instance.CON.CURRENT_PL_DATA.ITEM4;
            ___KEEP[6] = __instance.CON.CURRENT_PL_DATA.ITEM5;
            ___KEEP[7] = __instance.CON.CURRENT_PL_DATA.MAG1;
            ___KEEP[8] = __instance.CON.CURRENT_PL_DATA.MAG2;
            var calamisItem = SaveHandler.CurrentSaveData.ScoutedLocations[LucidID].Name;
            var cantFight = !ArchipelagoClient.AP.WasItemReceived("Lucid Blade") && calamisItem != "Lucid Blade";
            __instance.CON.CURRENT_PL_DATA.WEP1 = cantFight ? null : "LUCID BLADE";
            __instance.CON.CURRENT_PL_DATA.WEP2 = cantFight ? null : "LUCID BLADE";
            __instance.CON.CURRENT_PL_DATA.ITEM1 = null;
            __instance.CON.CURRENT_PL_DATA.ITEM2 = null;
            __instance.CON.CURRENT_PL_DATA.ITEM3 = null;
            __instance.CON.CURRENT_PL_DATA.ITEM4 = null;
            __instance.CON.CURRENT_PL_DATA.ITEM5 = null;
            __instance.CON.CURRENT_PL_DATA.MAG1 = null;
            __instance.CON.CURRENT_PL_DATA.MAG2 = null;
            __instance.CON.PL.GOD = !cantFight;
            __instance.CON.Current_Gameplay_State = 1;
            __instance.CON.OpenMenu();
            __instance.CON.EQITEMS();
            __instance.CON.EQMagic();
            ___ANIM = __instance.transform.GetChild(0).GetComponent<Animation>();
            
            if (!cantFight) __instance.StartCoroutine("Regen");
            ___ROUT = __instance.StartCoroutine("GO");
            return false;
        }

        [HarmonyPatch(typeof(Item_Pickup_scr), "Pickup")]
        [HarmonyPostfix]
        private static void Pickup_SendRingLinkIfOn(Item_Pickup_scr __instance)
        {
            if (!ArchipelagoClient.AP.SlotData.RingLink)
            {
                return;
            }

            if (__instance.type != 2)
            {
                return;
            }
            var amount = int.Parse(__instance.Name);
            ArchipelagoClient.AP.SendRingLinkPacket(amount);
        }

        [HarmonyPatch(typeof(Money_Damage), "OnTriggerEnter")]
        [HarmonyPostfix]
        private static void OnTriggerEnter_AlsoMakePeopleLoseMoney(Money_Damage __instance, ref int ___scaled_power)
        {
            if (!ArchipelagoClient.AP.SlotData.RingLink)
            {
                return;
            }
            if (__instance.REG_DAMAGE.power> 0)
            {
                return;
            }
            var amount = -___scaled_power;
            ArchipelagoClient.AP.SendRingLinkPacket(amount);
        }

        [HarmonyPatch(typeof(Boss), "End")]
        [HarmonyPrefix]
        private static bool End_ReturnWithoutLucid(Boss __instance, ref string[] ___KEEP)
        {
            _kept = ___KEEP;
            __instance.CON.CURRENT_PL_DATA.WEP1 = _kept[0];
            __instance.CON.CURRENT_PL_DATA.WEP2 = _kept[1];
            try
            {
                __instance.CON.CURRENT_PL_DATA.ITEM1 = _kept[2];
                __instance.CON.CURRENT_PL_DATA.ITEM2 = _kept[3];
                __instance.CON.CURRENT_PL_DATA.ITEM3 = _kept[4];
                __instance.CON.CURRENT_PL_DATA.ITEM4 = _kept[5];
                __instance.CON.CURRENT_PL_DATA.ITEM5 = _kept[6];
                __instance.CON.CURRENT_PL_DATA.MAG1 = _kept[7];
                __instance.CON.CURRENT_PL_DATA.MAG2 = _kept[8];
            }
            catch
            {
                _log.LogError($"There was an error re-adding items.  Might be vanilla bug.");
            }
            __instance.CON.CURRENT_PL_DATA.PLAYER_H = Mathf.Max(__instance.CON.CURRENT_PL_DATA.PLAYER_H, 20f);
            __instance.CON.PL.Poison.POISON_DUR = 0.01f;
            __instance.CON.CURRENT_PL_DATA.PLAYER_B = __instance.CON.CURRENT_PL_DATA.PLAYER_H;
            __instance.CON.EQITEMS();
            __instance.CON.EQMagic();
            return false;
        }

        [HarmonyPatch(typeof(WaitAMonth), "Start")]
        [HarmonyPrefix]
        private static bool Start_WaitInstantly(WaitAMonth __instance)
        {
            __instance.transform.GetChild(0).gameObject.SetActive(value: true);
            return false;
        }

        [HarmonyPatch(typeof(Wam_scr), "OnEnable")]
        [HarmonyPrefix]
        private static bool OnEnable_FixIfOwnHarmingAxe(Wam_scr __instance)
        {
            var reelWait = __instance.GetType().GetField("reel_wait", Flags);
            reelWait.SetValue(__instance, __instance.wait);
            return false;
        }

        [HarmonyPatch(typeof(Player_Poison), "Update")]
        [HarmonyPostfix]
        public static void Update_RemoveStoredXPInstead(Player_Poison __instance, ref float ___XP_DURP, ref CONTROL ___CON, ref int ___XP_DRAIN_slot)
        {
            if (!ArchipelagoClient.AP.SlotData.Levelsanity)
            {
                return;
            }
            if (___CON.CURRENT_PL_DATA.PLAYER_LVL > 99)
            {
                return; // drains should work on a level 100.
            }
            // Fix the normal experience drain.  Since you always get levels in packs of 100 we can just round back to the nearest 100.
            decimal currentExp = ___CON.CURRENT_PL_DATA.XP;
            var nearestHundred = Math.Ceiling(currentExp / 100) * 100;
            ___CON.CURRENT_PL_DATA.XP = Convert.ToInt32(nearestHundred);
            if (__instance.XP_DRAIN_DUR > 0f)
            {
                if (SaveHandler.CurrentSaveData.StoredExperience > 0)
                {
                    ___XP_DURP += Time.deltaTime * 1.2f;
                    if (___XP_DURP > 1f)
                    {
                        SaveHandler.CurrentSaveData.StoredExperience -= Mathf.RoundToInt(___XP_DURP);
                        ___XP_DURP = 0f;
                    }
                }
                else
                {
                    SaveHandler.CurrentSaveData.StoredExperience = 0;
                }
            }

        }

        [HarmonyPatch(typeof(Player_Poison), "Harm")]
        [HarmonyPrefix]
        private static bool Harm_FixSlownessBug(int type, float duration, Player_Poison __instance)
        {
            __instance.IMG.gameObject.GetComponent<AspectRatioFitter>().aspectRatio = Camera.main.aspect;
            if (type == 4)
            {
                var poisonType = __instance.GetType();
                var plField = poisonType.GetField("PL", BindingFlags.Instance | BindingFlags.NonPublic);
                GameObject PL = (GameObject)plField.GetValue(__instance);
                if (PL.GetComponent<Player_Control_scr>().CON.EQ_WEP != null)
                {
                    return true;
                }
                else
                {
                    var CON = GameObject.Find("CONTROL").GetComponent<CONTROL>();
                    if (__instance.SLOW_DUR == 0f)
                    {
                        var audioMethod = poisonType.GetMethod("Audio", BindingFlags.Instance | BindingFlags.NonPublic);
                        __instance.Active_Effects++;
                        audioMethod.Invoke(__instance, new object[] { 2 });
                        var SLOW_slotField = poisonType.GetField("SLOW_slot", BindingFlags.Instance | BindingFlags.NonPublic);
                        SLOW_slotField.SetValue(__instance, __instance.Active_Effects - 1);
                        __instance.transform.GetChild(__instance.Active_Effects - 1).GetComponent<TextMeshProUGUI>().text = "slowed";
                        __instance.transform.GetChild(__instance.Active_Effects - 1).GetComponent<TextMeshProUGUI>().color = Color.grey;
                    }
                    var steps = duration;
                    steps *= Mathf.LerpUnclamped(1.25f, 0.2f, (float)CON.CURRENT_PL_DATA.PLAYER_RES / 100f);
                    __instance.SLOW_DUR = Time.time + steps;
                }
                return false;
            }
            return true;
        }

        [HarmonyPatch(typeof(LunacyGain), "OnTriggerStay")]
        [HarmonyPrefix]
        private static bool OnTriggerStay_TryFixNull(LunacyGain __instance, Collider other)
        {
            if (__instance.CON is null)
            {
                __instance.CON = GameObject.Find("CONTROL").GetComponent<CONTROL>();
            }
            return true;
        }

        [HarmonyPatch(typeof(MoonHeal), "Start")]
        [HarmonyPrefix]
        private static bool Start_FixMoonNull(MoonHeal __instance)
        {
            if (__instance.MOON is null)
            {
                __instance.MOON = GameObject.Find("CONTROL").GetComponent<SimpleMoon>();
            }
            return true;
        }

        public static void EnsureAftermathAfterKill(Transform hubLevel)
        {
            if (!hubLevel.GetChild(7).gameObject.activeSelf && SaveHandler.CurrentSaveData.EnteredScenes.Contains("Chamber of Fate"))
            {
                hubLevel.GetChild(8).gameObject.SetActive(false);
                hubLevel.GetChild(7).gameObject.SetActive(true);
            }
        }

        [HarmonyPatch(typeof(Compass_control), "Update")]
        [HarmonyPrefix]
        private static bool Update_RotateToNearestCheck(Compass_control __instance)
        {
            if (!SaveHandler.MainRandoSettings.CompassCheck)
            {
                return true;
            }
            if (__instance.style == 0)
            {
                return false;
            }
            var playerPosition = __instance.PL.transform.position;
            var relativeVector = FindClosestItemVectorRelativeToPlayer(playerPosition);
            if (__instance.style == 1)
            {
                
                var playerRotation = __instance.PL.ROT.y;
                var rotation2d = ClosestItemAngle2D(relativeVector);
                if (rotation2d > 360f)
                {
                    __instance.MDL.transform.localRotation = Quaternion.Euler(new Vector3(0, 0, playerRotation));
                    return false;
                }
                var totalRotation2d = playerRotation - rotation2d + 180;  // Add 180 to "flip" the pointer around.
                switch (totalRotation2d)
                {
                    case > 180f:
                        totalRotation2d -= 360f;
                        break;
                    case < -180f:
                        totalRotation2d += 360f;
                        break;
                }
                __instance.MDL.transform.localRotation = Quaternion.Euler(new Vector3(0, 0, totalRotation2d));
                return false;
            }
            if (relativeVector is { x: 0, z: 0 })
            {
                __instance.MDL.transform.rotation =
                    Quaternion.LookRotation(Vector3.forward, Vector3.up);
                return false;
            }
            __instance.MDL.transform.rotation =
                Quaternion.LookRotation(new Vector3(relativeVector.x, relativeVector.y, relativeVector.z),
                    Vector3.up);
            return false;
        }

        private static Vector3 FindClosestItemVectorRelativeToPlayer(Vector3 playerPosition)
        {
            if (!LocationHandler.Pickups.Any())
            {
                return playerPosition;
            }
            var closestVector = new Vector3(99999f, 99999f, 99999f);
            var closestDistance = 999999999f;
            foreach (var pickup in LocationHandler.Pickups)
            {
                if (pickup is null)
                {
                    continue;
                }
                var distance =  Vector3.Distance(playerPosition, pickup.Position);
                if (distance > closestDistance) continue;
                closestVector = pickup.Position;
                closestDistance = distance;
            }
            if (Vector3.Distance(closestVector, new Vector3(99999f, 99999f, 99999f)) < 0.5f)
            {
                return playerPosition;
            }
            var shiftedVector = closestVector - playerPosition;
            return shiftedVector;
        }
        
        private static float ClosestItemAngle2D(Vector3 relativeVector)
        {
            var shadowVector = new Vector2(relativeVector.x, relativeVector.z);
            if (Vector2.Distance(shadowVector, new Vector2(0, 0)) < 0.1f)
            {
                return 400f;
            }

            var unitVector = shadowVector / shadowVector.magnitude;
            return Math.Sign(unitVector.x)*(float)(180 * Math.Acos(unitVector.y) / Math.PI);
        }

        [HarmonyPatch(typeof(Real_Timer), "OnEnable")]
        [HarmonyPrefix]
        private static bool OnEnable_FixRealTimer(Real_Timer __instance)
        {
            if (__instance.Begin) return true;
            if (__instance.ACT is null) return true;
            __instance.ACT.SetActive(value: true);
            return false;

        }
    }
}