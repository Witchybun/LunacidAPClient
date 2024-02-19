using HarmonyLib;
using UnityEngine;

namespace LunacidAP
{
    public class ExpHandler
    {
        public static void Awake()
        {
            Harmony.CreateAndPatchAll(typeof(ExpHandler));
        }
        
        [HarmonyPatch(typeof(CONTROL), "GiveXP")]
        [HarmonyPrefix]
        private static bool GiveXP_MultiplyIncomingXP(CONTROL __instance, int LEVELED_XP)
        {
            var givenXP = LEVELED_XP;
            givenXP = Mathf.RoundToInt(givenXP * SlotData.ExperienceMultiplier);
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
    }
}