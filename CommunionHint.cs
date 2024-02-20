using System;
using System.Collections.Generic;
using System.Linq;
using BepInEx.Logging;
using HarmonyLib;
using UnityEngine;

namespace LunacidAP
{
    public class CommunionHint
    {
        private static ArchipelagoClient _archipelago;
        private static ManualLogSource _log;
        private static POP_text_scr PAPPY;

        private static CONTROL CON;

        private static readonly char[] alphabet = new[] { 'a', 'b', 'c', 'd', 'e', 'f', 'g', 'h', 'i', 'j', 'k', 'l', 'm', 'n', 'o', 'p', 'q', 'r', 's', 't', 'u', 'v', 'w', 'x', 'y', 'z' };
        private static Dictionary<string, string> CreatureToHint = new(){
            { "ABYSSALDEMON", "Upon {0} lies {1}.  And you will die before reaching it!" },
            { "CENTAUR", "Dead flesh.  Cannot reach {0}.  For {1} is salvation." },
            { "CETEA", "The curse of finding {1} has yet to affect you, for you merely walk to {0}." },
            { "CHIMERA", "Fool!  You seek {1} at {0}, and yet will only find death." },
            { "CURSEDPAINTING", "Help.  Release us upon {0}.  Our new vessal shall be {1}." },
            { "DEVILSLIME", "No absorb into?  Give {1} from {0}.  Will absorb in place." },
            { "ENLIGHTENEDONE", "To shed the physical, obtain {1} upon {0}.  Only then." },
            { "GREATBAT", "Give {1} to replace my wings.  Master will never let me go to {0}." },
            { "HEMALITH", "Drink drip drop.  You drink {1}.  I try at {0}.  Cannot satisfy." },
            { "IKURR'ILB", "You dare speak of {1} in my tongue.  As if you come from {0}..." },
            { "JOTUNN", "Little one speaks of {1}, encased within {0}.  I would not worry.  Do well with your time little one." },
            { "LUNAGA", "Saw {1} at {0}.  Not as bright as blue orb in sky.  Must return." },
            { "LUPINE", "A waste to speak of {0}.  I feed upon corpses, not {1}." },
        };

        private static Dictionary<string, long> CreatureToPossibleLocations;

        private void Awake(ArchipelagoClient archipelago, ManualLogSource log)
        {
            _archipelago = archipelago;
            _log = log;
        }

        private static string Encrypt(string phrase)
        {
            char[] encrypted = new char[phrase.Length];
            var decrypted = phrase.ToCharArray();
            for (int i = 0; i < phrase.Length; i++)
            {
                var letter = decrypted[i];
                int index = Array.IndexOf(alphabet, letter);
                int newIndex = (7 + index) % 26;
                char newLetter = alphabet[newIndex];
                encrypted[i] = newLetter;
            }
            var newPhrase = String.Join("", encrypted);
            return newPhrase;
        }

        [HarmonyPatch(typeof(Analyz_NPC), "OnTriggerEnter")]
        [HarmonyPrefix]
        private static bool OnTriggerEnter_HintSystemForCommunion(Analyz_NPC __instance, Collider other)
        {
            if (!__instance.COM)
            {
                return true; // just let the original code handle it.
            }
            CON = GameObject.Find("CONTROL").GetComponent<CONTROL>();
            PAPPY = CON.PAPPY;
            __instance.NPC_NAME = other.GetComponent<OBJ_HEALTH>().MOM.gameObject.name.ToUpper().Replace(" ", "");
            string text = "";
            if (!CreatureToHint.Keys.Contains(__instance.NPC_NAME))
            {
                return true; // For now just use the old code.
            }
            var location = _archipelago.Session.Locations.GetLocationNameFromId(CreatureToPossibleLocations[__instance.NPC_NAME]);
            var item = _archipelago.ScoutLocation(CreatureToPossibleLocations[__instance.NPC_NAME], false);
            text = Encrypt(string.Format(CreatureToHint[__instance.NPC_NAME], location, item));
            PAPPY.POP(text, 1f, 13);
            Debug.Log(__instance.NPC_NAME);
            Debug.Log(text);
            __instance.transform.GetChild(0).gameObject.SetActive(value: true);
            UnityEngine.Object.Destroy(__instance.gameObject);
            return false;
        }

        private static void DetermineHints(int seed)
        {
            if (CreatureToPossibleLocations is not null)
            {
                return;
            }
            var random = new System.Random(seed);
            var locationList = _archipelago.Session.Locations.AllLocations;
            var locationCount = locationList.Count();
            foreach (var creature in CreatureToHint)
            {
                var chosenLocationPosition = random.Next(0, locationCount - 1);
                var chosenLocation = locationList[chosenLocationPosition];
                CreatureToPossibleLocations[creature.Key] = chosenLocation;
            }
        }
    }
}