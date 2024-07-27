using BepInEx.Logging;
using HarmonyLib;
using LunacidAP.Data;
using UnityEngine;

namespace LunacidAP
{
    public class AlkiHandler
    {
        private static ManualLogSource _log;

        public AlkiHandler(ManualLogSource log)
        {
            _log = log;
            Harmony.CreateAndPatchAll(typeof(AlkiHandler));
        }

        [HarmonyPatch(typeof(Alki), "FORGE")]
        [HarmonyPrefix]
        private static bool FORGE_ForgeACheck(Alki __instance)
        {
            __instance.CON ??= GameObject.Find("CONTROL").GetComponent<CONTROL>();
            var num = FindMater(__instance, out var flag);
            if (flag)
            {
                var locationData = new LunacidLocations.LocationData(-1, "ERROR", "ERROR");
                foreach (var location in LunacidLocations.AlkiLocation)
                {
                    if (location.GameObjectName == __instance.Recipes[num].name)
                    {
                        locationData = location;
                        break;
                    }
                }
                if (locationData.APLocationName == "ERROR")
                {
                    _log.LogError($"Could not find recipe location for {__instance.Recipes[num].name}!");
                    return flag;
                }
                var item = ArchipelagoClient.AP.SendLocationGivenLocationDataSendingGift(locationData);
                if (item == "ALREADY_ACQUIRED")
                {
                    __instance.has_made = "Already Checked!";
                    __instance.Reset();
                    return flag;
                }
                __instance.CON.RemoveMatter(__instance.current_1.ToString());
                __instance.CON.RemoveMatter(__instance.current_2.ToString());
                __instance.CON.RemoveMatter(__instance.current_3.ToString());
                __instance.has_made = item + " Created!";
                __instance.Recipes[num].unlocked = 1;
                int num2 = num + 1;
                string zONE_ = __instance.CON.CURRENT_PL_DATA.ZONE_8;
                zONE_ = zONE_.Substring(0, num2 - 1) + "1" + zONE_.Substring(num2, zONE_.Length - num2);
                __instance.CON.CURRENT_PL_DATA.ZONE_8 = zONE_;
                if (zONE_ == "1111111111111111")
                {
                    __instance.transform.GetChild(0).gameObject.SetActive(value: true);
                }
                __instance.Reset();
            }
            return flag;
        }

        private static int FindMater(Alki alki, out bool wasFound)
        {
            var num = -1;
            wasFound = false;
            for (int i = 0; i < alki.Recipes.Length; i++)
            {
                if (alki.current_1 == alki.Recipes[i].need_1)
                {
                    if (alki.current_2 == alki.Recipes[i].need_2)
                    {
                        if (alki.current_3 == alki.Recipes[i].need_3)
                        {
                            num = i;
                            wasFound = true;
                            i = 999;
                        }
                    }
                    else if (alki.current_3 == alki.Recipes[i].need_2 && alki.current_2 == alki.Recipes[i].need_3)
                    {
                        num = i;
                        wasFound = true;
                        i = 999;
                    }
                }
                else if (alki.current_2 == alki.Recipes[i].need_1)
                {
                    if (alki.current_1 == alki.Recipes[i].need_2)
                    {
                        if (alki.current_3 == alki.Recipes[i].need_3)
                        {
                            num = i;
                            wasFound = true;
                            i = 999;
                        }
                    }
                    else if (alki.current_3 == alki.Recipes[i].need_2 && alki.current_1 == alki.Recipes[i].need_3)
                    {
                        num = i;
                        wasFound = true;
                        i = 999;
                    }
                }
                else
                {
                    if (alki.current_3 != alki.Recipes[i].need_1)
                    {
                        continue;
                    }
                    if (alki.current_2 == alki.Recipes[i].need_2)
                    {
                        if (alki.current_1 == alki.Recipes[i].need_3)
                        {
                            num = i;
                            wasFound = true;
                            i = 999;
                        }
                    }
                    else if (alki.current_3 == alki.Recipes[i].need_2 && alki.current_2 == alki.Recipes[i].need_3)
                    {
                        num = i;
                        wasFound = true;
                        i = 999;
                    }
                }
            }
            return num;
        }
    }
}