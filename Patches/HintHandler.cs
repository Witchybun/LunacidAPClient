using System;
using System.Collections.Generic;
using System.Linq;
using Archipelago.MultiClient.Net.Enums;
using BepInEx.Logging;
using HarmonyLib;
using LunacidAP.Data;
using UnityEngine;

namespace LunacidAP
{
    public class CommunionHint
    {
        private static ManualLogSource _log;
        private static POP_text_scr PAPPY;

        private static CONTROL CON;

        private static readonly char[] alphabet = new[] { 'a', 'b', 'c', 'd', 'e', 'f', 'g', 'h', 'i', 'j', 'k', 'l', 'm', 'n', 'o', 'p', 'q', 'r', 's', 't', 'u', 'v', 'w', 'x', 'y', 'z' };
        private static readonly char[] symbols = new[] { '.', ':', '(', ')', '?', '!', '[', ']', ' ', '{', '}', '#', '$' };
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
            { "KODAMA", "Sweet smelling {1}, snuggled with at {0}.  Bun bun!"},
            { "MALEDICTUS", "I bear this curse alone.  The knowledge of {1} at {0}.  Begone." },
            { "MALFORMED", "{1} is not blood, unlike you.  Why bother to reach {0}?" },
            { "MALFORMEDHORSE", "ACCURSED {1}.  HELP IT ESCAPE {0}.  HELLLLLLLLPPPPPPPPPPPP!"},
            { "MARE", "Ahahaha, {1}?  It delights me that it wishes to find {0} for it!" },
            { "MI-GO", "Sour dried {1} within the sins of your eyes black future with {0} descended upon with teeth below amber crooked limbs thrown apart." },
            { "MILKSNAIL", "Why worry for {1}?  Slow and soak deeply at {0}.  Join us. " },
            { "NECRONOMICON", "{1} is foolish to speak of.  Why speak at all?  Go to {0}, if you wish to speak of anything at all." },
            { "NEPTUNE", "The water laps at the feet of {1}, in the depths of {0}.  Part such waters with thy m o v e m e n t." },
            { "RAT", "Shush don't step on me.  I smell cheese at {0}.  Going.  Not cheese but {1}?  Nooo..." },
            { "SNAIL", "Curious drippy {1}.  Your struggle to reach {0} for it running too and fro you must go." },
            { "SLIME", "Solid creature leave alone.  Wish to know {0}?  If I tell you it be {1}, will you go?  Please?" },
            { "SKELETON", "Go to {0}, pick up {1}.  Use it to join us.  Soon." },
            { "SLIMESKELETON", "Give us flesh to add, bones to add, {1} to add.  Collect by {0}.  Give to us." },
            { "SHULKER", "Corpse good food.  {1}, not good food.  Dropped down at {0}.  Waste." },
            { "VENUS", "Feel the earth below {1} at {0}.  Connected to the entire multiworld." },
            { "YAKUL", "This forest is sacred.  Leave, go to {0}, get {1}, and leave.  Trespass no longer." },
            { "UNILATERALIS", "Let time take {1}.  Let it age away as it rests at {0}.  Let it go." },
            { "TILLANDSIA", "The wind blows so well at {0}.  Not even {1} stops it blowing past me.  It feels like home..." },
        };

        private static Dictionary<string, string> CreatureHints { get; set; }

        public static void Awake(ManualLogSource log)
        {
            _log = log;
            Harmony.CreateAndPatchAll(typeof(CommunionHint));
        }

        private static string Encrypt(string phrase)
        {
            char[] encrypted = new char[phrase.Length];
            var decrypted = phrase.ToCharArray();
            for (int i = 0; i < phrase.Length; i++)
            {
                var letter = decrypted[i];
                if (char.IsPunctuation(letter) || char.IsSeparator(letter) || char.IsNumber(letter))
                {
                    encrypted[i] = letter;
                    continue;
                }
                bool isCapitalized = char.IsUpper(letter);
                int index = Array.IndexOf(alphabet, char.ToLower(letter));
                int newIndex = (7 + index) % 26;
                char newLetter = isCapitalized ? char.ToUpper(alphabet[newIndex]) : alphabet[newIndex];
                encrypted[i] = newLetter;
            }
            var newPhrase = string.Join("", encrypted);
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
            if (other.gameObject.GetComponent<OBJ_HEALTH>() == null || other.gameObject.GetComponent<OBJ_HEALTH>().MOM == null || !(other.gameObject.GetComponent<OBJ_HEALTH>().MOM.GetComponent<AI_simple>() != null))
            {
                return false;
            }
            __instance.NPC_NAME = other.GetComponent<OBJ_HEALTH>().MOM.gameObject.name.ToUpper().Replace(" ", "");
            if (!CreatureToHint.Keys.Contains(__instance.NPC_NAME))
            {
                return true; // For now just use the old code.
            }
            try
            {
                CON = GameObject.Find("CONTROL").GetComponent<CONTROL>();
                PAPPY = CON.PAPPY;
                var text = ConnectionData.CommunionHints[__instance.NPC_NAME];
                /*if (!hintData.AlreadyHinted) // Lets turn off automatic hinting for now, for flair's sake.
                {
                    ArchipelagoClient.AP.ScoutLocation(locationID, isProgression);
                }*/
                if (ArchipelagoClient.AP.WasItemReceived("Bestial Mastery"))
                {
                    PAPPY.POP(text, 5f, 16);
                }
                else
                {
                    text = Encrypt(text);
                    PAPPY.POP(text, 1f, 13);
                }
                __instance.transform.GetChild(0).gameObject.SetActive(value: true);
                UnityEngine.Object.Destroy(__instance.gameObject);
                return false;
            }
            catch (Exception ex)
            {
                if (other.GetComponent<OBJ_HEALTH>() is null)
                {
                    _log.LogError($"OBJ_HEALTH is null");
                }
                if (other.GetComponent<OBJ_HEALTH>().MOM is null)
                {
                    _log.LogError("MOM is null");
                }
                if (__instance.transform.GetChild(0) is null)
                {
                    _log.LogError("Child is null");
                }
                if (__instance.transform.GetChild(0).gameObject is null)
                {
                    _log.LogError("Child GameObject is null");
                }
                _log.LogError(ex.Message);
                return true;
            }
        }

        public static string GetSuitableStringLength(string phrase, int size)
        {
            var length = phrase.Length;
            if (length > size)
            {
                if (phrase.Any(x => Char.IsWhiteSpace(x)))
                {
                    var phraseArray = phrase.Split(' ');
                    var shortestPhrase = "";
                    foreach (var word in phraseArray)
                    {
                        var previousPhrase = shortestPhrase;
                        shortestPhrase += word + " ";

                        if (shortestPhrase.Length > size)
                        {
                            shortestPhrase = previousPhrase;
                            break;
                        }
                    }
                    if (shortestPhrase != "")
                    {
                        return shortestPhrase + "...";
                    }
                }
                return phrase.Substring(0, size);
            }
            return phrase;
        }

        public static void DetermineHints(int seed)
        {
            if (ConnectionData.CommunionHints is not null && ConnectionData.CommunionHints.Count() > 1)
            {
                return; // already genned probably
            }
            CreatureHints = new() { };
            var dictionaryToManipulate = ArchipelagoClient.AP.SlotData.ImportantItemLocations;
            dictionaryToManipulate.Remove("Lucid Blade");
            var random = new System.Random(seed);
            dictionaryToManipulate.OrderBy(x => random.Next()).ToDictionary(item => item.Key, item => item.Value);
            var unusedCreatures = CreatureToHint.Keys.ToList();
            foreach (var item in dictionaryToManipulate)
            {
                var locations = item.Value;
                foreach (var location in locations)
                {
                    if (!unusedCreatures.Any())
                    {
                        ConnectionData.CommunionHints = CreatureHints;
                        return;
                    }
                    var chosenCreature = unusedCreatures[random.Next(0, unusedCreatures.Count() - 1)];
                    unusedCreatures.Remove(chosenCreature);
                    var message = string.Format(CreatureToHint[chosenCreature], location, item.Key);
                    CreatureHints[chosenCreature] = message;
                }
            }
            if (unusedCreatures.Any())
            {
                GenerateFillerHints(unusedCreatures, random);
            }
            ConnectionData.CommunionHints = CreatureHints;
        }


        private static void GenerateFillerHints(List<string> unusedCreatures, System.Random random)
        {

            var locationList = ConnectionData.ScoutedLocations.Keys.ToList();
            var locationCount = locationList.Count();
            var chosenLocations = new List<long>() { };
            foreach (var creature in unusedCreatures)
            {
                var chosenLocation = locationList[random.Next(0, locationCount - 1)];
                while (chosenLocations.Contains(chosenLocation))
                {
                    chosenLocation = locationList[random.Next(0, locationCount - 1)];
                }
                var locationInfo = ConnectionData.ScoutedLocations[chosenLocation];
                CreatureHints[creature] = string.Format(CreatureToHint[creature], ArchipelagoClient.AP.GetLocationNameFromID(chosenLocation), locationInfo.Name);
            }
        }

        [HarmonyPatch(typeof(CutScene_Dialog), "LOAD")]
        [HarmonyPostfix]
        private static void LOAD_MakeGarratHintForYou(CutScene_Dialog __instance)
        {
            if (__instance.npc_name != "GARRAT THE HOUND")
            {
                return;
            }
            var playerName = __instance.CON.CURRENT_PL_DATA.PLAYER_NAME;
            var bladeLocation = ArchipelagoClient.AP.SlotData.ImportantItemLocations["Lucid Blade"];
            if (__instance.gameObject.name == "Gar_DIALOG_ALT")
            {
                __instance.LINES[0].value = $"You're alive, {playerName}?   Without {bladeLocation[0]} you have no chance of victory.";
            }
            else
            {
                __instance.LINES[0].value = $"{playerName}.  Without {bladeLocation[0]} you have no chance of victory.";
            }
            return;
        }
    }


}