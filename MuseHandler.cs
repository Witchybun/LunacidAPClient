using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using BepInEx.Logging;
using HarmonyLib;
using LunacidAP.Data;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.Networking;

namespace LunacidAP
{
    public class MuseHandler
    {
        private static ManualLogSource _log;
        public static Dictionary<string, AudioClip> storedSongs = new();
        public static Dictionary<string, string> randomizedSongs = new();

        public MuseHandler(ManualLogSource log)
        {
            _log = log;
            Harmony.CreateAndPatchAll(typeof(MuseHandler));
        }

        [HarmonyPatch(typeof(MUSE_scr), "NewTrack")]
        [HarmonyPrefix]
        private static bool NewTrack_ShuffleTrack(ref AudioClip track,ref string Track_Info, float vol)
        {
            if (!ArchipelagoClient.IsInGame)
            {
                return true; // Don't randomize music in non-ingame scenarios.
            }
            if (!ArchipelagoClient.AP.SlotData.CustomMusic)
            {
                return true; // Randomized music is off, don't change anything.
            }
            _log.LogInfo($"{track.name}: {Track_Info}, Volume: {vol}");
            if (!randomizedSongs.TryGetValue(track.name, out var newTrack))
            {
                _log.LogWarning("Song doesn't have a randomized track, playing it normally.");
                return true;
            }
            track = storedSongs[newTrack];
            if (LunacidMusic.FileToTitle.TryGetValue(newTrack, out var songName))
            {
                Track_Info = songName;
            }
            else
            {
                Track_Info = newTrack;
            }
            return true;
        }

        public static void InitializeTrackInfo()
        {
            var dir = Application.absoluteURL + "CustomMusic/";
            if (!Directory.Exists(dir))
            {
                Directory.CreateDirectory(dir);
            }
            var trackPath = Path.Combine(dir, "Tracks.json");
            if (!File.Exists(trackPath))
            {
                _log.LogWarning("There is no Track.json file.  Returning.");
                return;
            }
            using StreamReader reader = new StreamReader(trackPath);
            string text = reader.ReadToEnd();
            var tracks = JsonConvert.DeserializeObject<Dictionary<string, string>>(text);
            foreach (var track in tracks)
            {
                LunacidMusic.FileToTitle[track.Key] = track.Value;
            }
        }

        public static void InitializeChosenSongs(int seed)
        {
            var random = new System.Random(seed);
            var possibleSongs = storedSongs.Keys.ToList();
            var usedSongs = new List<string>();
            foreach (var file in LunacidMusic.FileToTitle)
            {  
                var chosenSong = possibleSongs[random.Next(0, possibleSongs.Count)];
                while (usedSongs.Contains(chosenSong))
                {
                    chosenSong = possibleSongs[random.Next(0, possibleSongs.Count)];
                }
                randomizedSongs[file.Key] = chosenSong;
                possibleSongs.Remove(chosenSong);
                if (!possibleSongs.Any())
                {
                    possibleSongs = storedSongs.Keys.ToList();  // Repeat the list if its too short.
                }
            }
        }
    }
}