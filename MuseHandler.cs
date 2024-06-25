using BepInEx.Logging;
using HarmonyLib;
using UnityEngine;

namespace LunacidAP
{
    public class MuseHandler
    {
        private static ManualLogSource _log;

        public MuseHandler(ManualLogSource log)
        {
            _log = log;
            //Harmony.CreateAndPatchAll(typeof(MuseHandler));
        }
        
        [HarmonyPatch(typeof(MUSE_scr), "NewTrack")]
        [HarmonyPrefix]
        private static bool NewTrack_ShuffleTrack(AudioClip track, string Track_Info, float vol)
        {
            _log.LogInfo($"{track.name}: {Track_Info}");
            return true;
        }
    }
}