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
            var givenXP = LEVELED_XP;
            givenXP = Mathf.RoundToInt(givenXP * ArchipelagoClient.AP.SlotData.ExperienceMultiplier);
            if ((__instance.CURRENT_PL_DATA.PLAYER_LVL < 100 || ConnectionData.StoredLevel < 100) && ArchipelagoClient.AP.SlotData.Levelsanity)
            {
                _log.LogInfo(givenXP);
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
            if (num > 50f)
            {
                __instance.CURRENT_PL_DATA.XP += Mathf.RoundToInt(35f * Mathf.Pow((float)givenXP / num, 1.25f) / Mathf.Pow(num, 0.1f));
            }
            else
            {
                __instance.CURRENT_PL_DATA.XP += Mathf.RoundToInt(35f * Mathf.Pow((float)givenXP / num, 1.25f));
            }
            return false;
        }

        [HarmonyPatch(typeof(Weapon_scr), "Attack")]
        [HarmonyPostfix]
        private static void Attack_TryToBuffExp(Weapon_scr __instance)
        {
            var additionalExp = ArchipelagoClient.AP.SlotData.WExperienceMultiplier - 1; // the "damage" was already done, so this is the remainder
            if (__instance.type == 0)
            {
                if (__instance.special == 6)
                {
                    __instance.WEP_XP += 1f * additionalExp;
                }
                int num = __instance.WEP_ELEMENT;
                if (num > 7)
                {
                    switch (num)
                    {
                        case 8:
                            num = 5;
                            break;
                        case 9:
                            num = 0;
                            break;
                        case 10:
                            num = 2;
                            break;
                        case 11:
                            num = 5;
                            break;
                    }
                }
                if (__instance.gameObject.GetComponent<OBJ_HEALTH>() != null && __instance.gameObject.GetComponent<OBJ_HEALTH>().type < 4 && __instance.WEP_XP > -1f && __instance.WEP_XP < 99f)
                {
                    __instance.WEP_XP += (__instance.WEP_XP += __instance.WEP_GROWTH * __instance.gameObject.GetComponent<OBJ_HEALTH>().Damage_Mult[num] * (__instance.USED_WEP_DAMAGE / __instance.WEP_DAMAGE)) * additionalExp;
                }
            }
            if (__instance.type == 1)
            {
                if (__instance.WEP_XP > -1f && __instance.WEP_XP < 99f)
                {
                    __instance.WEP_XP += __instance.WEP_GROWTH * (__instance.USED_WEP_DAMAGE / __instance.WEP_DAMAGE) * additionalExp;
                }
            }
            if (__instance.WEP_XP > 99f)
            {
                __instance.WEP_XP = 99f;
            }

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
                ConnectionData.StoredExperience += Mathf.RoundToInt(35f * Mathf.Pow((float)LEVELED_XP / num, 1.25f) / Mathf.Pow(num, 0.1f));
            }
            else
            {
                ConnectionData.StoredExperience += Mathf.RoundToInt(35f * Mathf.Pow((float)LEVELED_XP / num, 1.25f));
            }
        }

        public static void SendLevelLocations()
        {
            if (ConnectionData.StoredLevel >= 100)
            {
                return;
            }
            _popup ??= GameObject.Find("CONTROL").GetComponent<CONTROL>().PAPPY;
            _log.LogInfo("Calculating locations to send.");
            int num = Mathf.FloorToInt(ConnectionData.StoredExperience / 100f);
            _log.LogInfo($"We have {num} levels to handle because we have {ConnectionData.StoredExperience}");
            while (num >= 1)
            {
                ConnectionData.StoredLevel += 1;
                var location = new LocationData(800 + ConnectionData.StoredLevel, $"Reach Level {ConnectionData.StoredLevel}");
                var item = ConnectionData.ScoutedLocations[location.APLocationID];
                LocationHandler.DetermineOwnerAndDirectlyGiveIfSelf(location, item);
                if (item.SlotName != ConnectionData.SlotName)
                {
                    var color = Colors.DetermineItemColor(item.Classification);
                    _popup.POP($"Level Up!  Found <color={color}>{item.Name}</color> for {item.SlotName}", 1f, 0);
                }
                num -= 1;
                ConnectionData.StoredExperience -= 100;
            }

        }
    }
}