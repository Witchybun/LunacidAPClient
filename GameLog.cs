using System.Collections.Generic;
using System.Linq;
using BepInEx.Logging;
using HarmonyLib;
using UnityEngine;
using LunacidAP.APGUI;

namespace LunacidAP
{
    public static class GameLog
    {
        public static bool isHidden = true;

        private static Rect hideShowbuttonRect;
        public static bool DisableArchipelagoLogin;

        private static ManualLogSource _log;
        private static ArchipelagoClient _archipelago;

        public static void Awake(ArchipelagoClient archipelago, ManualLogSource log)
        {
            _archipelago = archipelago;
            _log = log;
            UpdateWindow();
        }

        public static void OnGUI()
        {

            if (GUI.Button(hideShowbuttonRect, isHidden ? "Show" : "Hide"))
            {
                isHidden = isHidden ? false : true;
                UpdateWindow();
            }
        }

        public static void UpdateWindow()
        {
            int width = (int)((float)Screen.width*0.4f);
            int buttonWidth = (int)((float)Screen.width*0.035f);
            int buttonHeight = (int)((float)Screen.height*0.03f);

            hideShowbuttonRect = new Rect(100 + (width / 2) + (buttonWidth / 3), Screen.height*0.004f, buttonWidth, buttonHeight);
                        
            ArchipelagoLoginGUI.UpdateGUI();
        }

        /*[HarmonyPatch(typeof(FixedAspectRatioManager), "LateUpdate")]
        [HarmonyPrefix]
        private static void LateUpdate(ref float ___m_screenWidth, ref float ___m_screenHeight)
        {
            if ((float)Screen.width != ___m_screenWidth || (float)Screen.height != ___m_screenHeight)
            {
                UpdateWindow();
            }
        }

        [HarmonyPatch(typeof(T17StandaloneInputModule), nameof(T17StandaloneInputModule.Process))]
        [HarmonyPrefix]
        private static bool Process()
        {
            return isHidden;
        }*/
    }
}
