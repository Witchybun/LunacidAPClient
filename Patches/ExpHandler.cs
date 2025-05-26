using System;
using System.Runtime.InteropServices.ComTypes;
using BepInEx.Logging;
using HarmonyLib;
using LunacidAP.Data;
using TMPro;
using UnityEngine;
using static LunacidAP.Data.LunacidLocations;

namespace LunacidAP
{
    public class ExpHandler
    {
        private static ManualLogSource _log;
        private static POP_text_scr _popup;
        public ExpHandler(ManualLogSource log)
        {
            _log = log;
            Harmony.CreateAndPatchAll(typeof(ExpHandler));
        }

        [HarmonyPatch(typeof(CONTROL), "GiveXP")]
        [HarmonyPrefix]
        private static bool GiveXP_MultiplyIncomingXP(CONTROL __instance, int LEVELED_XP)
        {
            var has_bangle = ArchipelagoClient.AP.WasItemReceived("Lucky Bangle") ? 3 : 1;
            var givenXP = LEVELED_XP;
            var multiplier = Math.Min(12, has_bangle * ArchipelagoClient.AP.SlotData.ExperienceMultiplier);
            givenXP = Mathf.RoundToInt(givenXP * multiplier);
            if ((__instance.CURRENT_PL_DATA.PLAYER_LVL < 100 || ConnectionData.StoredLevel < 100) && ArchipelagoClient.AP.SlotData.Levelsanity)
            {
                StoreXP(givenXP, __instance.ST, __instance.CURRENT_PL_DATA.PLAYER_L, __instance.GetComponent<SimpleMoon>().MOON_MULT);
                SendLevelLocations();
                return false;
            }
            if (__instance.ST)
            {
                givenXP = Mathf.RoundToInt((float)givenXP * 0.3f);
            }
            float num = (float)__instance.CURRENT_PL_DATA.PLAYER_LVL + (float)__instance.CURRENT_PL_DATA.XP / 100f;
            float mOON_MULT = __instance.GetComponent<SimpleMoon>().MOON_MULT;
            givenXP = Mathf.RoundToInt((float)givenXP * Mathf.Lerp(1f, 2f, mOON_MULT / 10f * (__instance.CURRENT_PL_DATA.PLAYER_L / 50f)));
            givenXP = Math.Min(100, givenXP); // place a limiter on experience gain.
            if (num > 50f)
            {
                __instance.CURRENT_PL_DATA.XP += Math.Min(100, Mathf.RoundToInt(35f * Mathf.Pow((float)givenXP / num, 1.25f) / Mathf.Pow(num, 0.1f)));
            }
            else
            {
                __instance.CURRENT_PL_DATA.XP += Math.Min(100, Mathf.RoundToInt(35f * Mathf.Pow((float)givenXP / num, 1.25f)));
            }
            return false;
        }

        [HarmonyPatch(typeof(CONTROL), "OnSwap")]
        [HarmonyPostfix]
        private static void OnSwap_ModifyWeaponGrowth(CONTROL __instance)
        {
            if (__instance.EQ_WEP is null)
            {
                return;
            }
            if (__instance.EQ_WEP.WEP_XP > 99f)
            {
                __instance.EQ_WEP.WEP_XP = 99f;
            }
            var additionalExp = ArchipelagoClient.AP.SlotData.WExperienceMultiplier;
            __instance.EQ_WEP.WEP_GROWTH *= additionalExp;
        }

        public static void StoreXP(int LEVELED_XP, bool ST, float PLAYER_L, float MOON_MULT)
        {
            if (ST)
            {
                LEVELED_XP = Mathf.RoundToInt((float)LEVELED_XP * 0.3f);
            }
            float num = ConnectionData.StoredLevel + (float)ConnectionData.StoredExperience / 100f;
            float mOON_MULT = MOON_MULT;
            LEVELED_XP = Mathf.RoundToInt((float)LEVELED_XP * Mathf.Lerp(1f, 2f, mOON_MULT / 10f * (PLAYER_L / 50f)));
            if (num > 50f)
            {
                ConnectionData.StoredExperience += Math.Min(100, Mathf.RoundToInt(35f * Mathf.Pow((float)LEVELED_XP / num, 1.25f) / Mathf.Pow(num, 0.1f)));
            }
            else
            {
                ConnectionData.StoredExperience += Math.Min(100, Mathf.RoundToInt(35f * Mathf.Pow((float)LEVELED_XP / num, 1.25f)));
            }
        }

        public static void SendLevelLocations()
        {
            if (ConnectionData.StoredLevel >= 100)
            {
                return;
            }
            _popup ??= GameObject.Find("CONTROL").GetComponent<CONTROL>().PAPPY;
            int num = Mathf.FloorToInt(ConnectionData.StoredExperience / 100f);
            while (num >= 1)
            {
                ConnectionData.StoredLevel += 1;
                var location = new LocationData(800 + ConnectionData.StoredLevel, $"Reach Level {ConnectionData.StoredLevel}");
                var item = ConnectionData.ScoutedLocations[location.APLocationID];
                LocationHandler.DetermineOwnerAndDirectlyGiveIfSelf(location, item);
                if (item.SlotName != ConnectionData.SlotName)
                {
                    LocationHandler.SendLevelMessageOnLevelUp(item);
                }
                num -= 1;
                ConnectionData.StoredExperience -= 100;
            }

        }
    }
}