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

        [HarmonyPatch(typeof(Weapon_scr), "Attack")]
        [HarmonyPostfix]
        private static void Attack_TryToBuffExp(Weapon_scr __instance)
        {
            var additionalExp = SlotData.WExperienceMultiplier - 1; // the "damage" was already done, so this is the remainder
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
    }
}