using UnityEngine;
using BepInEx.Logging;

namespace LunacidAP.APGUI
{
    public static class ArchipelagoLoginGUI
    {
        public static bool ContinueWithoutArchipelago = false;

        public static bool Unlocked = false;
        private static bool ReachedTitleScreen = false;
        private static string StatusText = "";

        private static string serverUrl = "archipelago.gg";
        private static string userName = "";
        private static string password = "";

        private static Rect bgRect = new Rect(5, 5, 350, 143);
        private static Rect bgRectMini = new Rect(5, 5, 250, 45);
        private static Rect ScreenRect = new Rect(0, 0, Screen.width, Screen.height);

        private static ArchipelagoClient _archipelago;
        private static ManualLogSource _log;

        public static void Awake(ArchipelagoClient archipelago, ManualLogSource log)
        {
            _archipelago = archipelago;
            _log = log;
        }

        public static void Update()
        {
            if (GameLog.DisableArchipelagoLogin)
            {
                ContinueWithoutArchipelago = true;
            }

            if (ArchipelagoClient.Authenticated || ContinueWithoutArchipelago)
            {
                Unlocked = true;
            }

            if (!Unlocked && ReachedTitleScreen && GameLog.isHidden)
            {
                GameLog.isHidden = false;
                GameLog.UpdateWindow();
            }
            else if (Unlocked)
            {
                GameLog.isHidden = true;
                GameLog.UpdateWindow();
            }
        }

        // TODO: call on client connect/disconnect
        public static void UpdateGUI()
        {
            StatusText = "Archipelago Status: ";
            if (ContinueWithoutArchipelago)
            {
                StatusText += "Skipped";
            }
            else
            {
                StatusText += $"Connected ";
            }
        }

        public static void OnGUI()
        {
            if (GameLog.isHidden || GameLog.DisableArchipelagoLogin)
            {
                return;
            }

            if (ArchipelagoClient.Authenticated || ContinueWithoutArchipelago)
            {
                GUI.Box(bgRectMini, "");
                GUI.Box(bgRectMini, "");
                GUI.Label(new Rect(16, 16, 300, 20), StatusText);
                return;
            }

            if (!Unlocked)
            {
                GUI.Box(ScreenRect, ""); // Darken Screen
            }
            else
            {
                GUI.Box(bgRect, "");
            }

            GUI.Box(bgRect, "");

            GUI.Label(new Rect(16, 16, 300, 20), "Archipelago Status: Not Connected");
            GUI.Label(new Rect(16, 36, 150, 20), "Host: ");
            GUI.Label(new Rect(16, 56, 150, 20), "Player Name: ");
            GUI.Label(new Rect(16, 76, 150, 20), "Password: ");

            serverUrl =
                GUI.TextField(new Rect(150 + 16 + 8, 36, 150, 20), serverUrl);
            userName =
                GUI.TextField(new Rect(150 + 16 + 8, 56, 150, 20), userName);
            password =
                GUI.TextField(new Rect(150 + 16 + 8, 76, 150, 20), password);

            if (GUI.Button(new Rect(17, 110, 100, 30), "Connect"))
                {
                    _archipelago.Connect(userName, serverUrl, password, out var isSuccessful);
                    var isVerified = _archipelago.VerifySeed();
                    if (isSuccessful && isVerified)
                    {
                        _archipelago.ReceiveAllItems();
                        _archipelago.CollectLocationsFromSave();
                    }
                }

            if (!ContinueWithoutArchipelago && GUI.Button(new Rect(145, 110, 195, 30), "Continue Without Archipelago"))
            {
                ContinueWithoutArchipelago = true;               
                GameLog.isHidden = true;
            }
        }
    }
}
