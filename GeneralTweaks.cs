using System.Reflection;
using BepInEx.Logging;
using HarmonyLib;
using LunacidAP.Data;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace LunacidAP
{
    public class GeneralTweaks
    {
        // Should be for any fixes or changes to code that aren't necessarily AP oriented but change something.
        private static ManualLogSource _log;
        private static string[] _kept { get; set; }
        const BindingFlags Flags = BindingFlags.Instance | BindingFlags.NonPublic;

        public GeneralTweaks(ManualLogSource log)
        {
            _log = log;
            Harmony.CreateAndPatchAll(typeof(GeneralTweaks));
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
            var lucidID = ArchipelagoClient.AP.GetLocationIDFromName("CF: Calamis' Weapon of Choice");
            __instance.CON ??= GameObject.Find("CONTROL").GetComponent<CONTROL>();
            var popup = __instance.CON.PAPPY;
            var locationInfo = ArchipelagoClient.AP.ScoutLocation(lucidID);
            var itemInfo = locationInfo.Name;
            var slotNameofItemOwner = locationInfo.SlotName;
            ArchipelagoClient.AP.Session.Locations.CompleteLocationChecks(lucidID);
            ConnectionData.CompletedLocations.Add(lucidID);
            if (ConnectionData.SlotName != slotNameofItemOwner)
            {
                popup.POP($"Found {itemInfo} for {slotNameofItemOwner}", 1f, 0);
            }
            return;
        }

        [HarmonyPatch(typeof(Boss), "End")]
        [HarmonyPrefix]
        private static bool End_ReturnWithoutLucid(Boss __instance)
        {
            string[] KEEP = (string[])__instance.GetType().GetField("KEEP", Flags).GetValue(__instance);
            _kept = KEEP;
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
            if (!hubLevel.GetChild(7).gameObject.activeSelf && ConnectionData.EnteredScenes.Contains("Chamber of Fate"))
            {
                hubLevel.GetChild(8).gameObject.SetActive(false);
                hubLevel.GetChild(7).gameObject.SetActive(true);
            }
        }
    }
}