using System;
using System.Collections.Generic;
using System.Linq;
using BepInEx.Logging;
using LunacidAP.Data;
using Newtonsoft.Json;

namespace LunacidAP
{
    public class SlotData
    {
        private ManualLogSource _log;
        private const string SEED_KEY = "seed";
        private const string EXP_KEY = "experience";
        private const string WEXP_KEY = "weapon_experience";
        private const string SWITCH_KEY = "switch_locks";
        private const string COIN_KEY = "strange_coin_bundle";
        private const string FILLER_KEY = "filler_bundle";
        private const string ENDING_KEY = "ending";
        private const string SHOP_KEY = "shopsanity";
        private const string DROP_KEY = "dropsanity";
        private const string DL_KEY = "death_link";
        private const string RANDOM_ELE_KEY = "random_elements";
        private const string ELE_DICT_KEY = "elements";
        private const string WALL_KEY = "secret_door_lock";
        private Dictionary<string, object> _slotDataFields;
        public static int Seed {get; private set;}
        public static Goal Ending {get; private set;}
        public static bool Dropsanity {get; private set;}
        public static bool Shopsanity {get; private set;}
        public static bool Switchlock {get; private set;}
        public static float ExperienceMultiplier {get; private set;}
        public static float WExperienceMultiplier {get; private set;}
        public static int Coinbundle {get; private set;}
        public static int Fillerbundle {get; private set;}
        public static bool DeathLink {get; private set;}
        public static bool RandomElements {get; private set;}
        public static bool FalseWalls {get; private set;}

        public SlotData(Dictionary<string, object> slotDataFields, ManualLogSource log)
        {
            _log = log;
            _slotDataFields = slotDataFields;
            Ending = GetSlotSetting(ENDING_KEY, Goal.AnyEnding);
            Seed = GetSlotSetting(SEED_KEY, 0);
            Dropsanity = GetSlotSetting(DROP_KEY, false);
            Shopsanity = GetSlotSetting(SHOP_KEY, false);
            Switchlock = GetSlotSetting(SWITCH_KEY, false);
            ExperienceMultiplier = (float) GetSlotSetting(EXP_KEY, 100)/100;
            WExperienceMultiplier = (float) GetSlotSetting(WEXP_KEY, 100)/100;
            Coinbundle = ParseCoinBundle(GetSlotSetting(COIN_KEY, StrangeCoin.Ten));
            Fillerbundle = GetSlotSetting(FILLER_KEY, 1);
            DeathLink = GetSlotSetting(DL_KEY, false);
            RandomElements = GetSlotSetting(RANDOM_ELE_KEY, false);
            var elementsData = GetSlotSetting(ELE_DICT_KEY, "");
            FalseWalls = GetSlotSetting(WALL_KEY, false);
            foreach (var data in JsonConvert.DeserializeObject<Dictionary<string, string>>(elementsData))
            {
                var newKey = data.Key.ToUpper().Replace("'", "");
                if (!ConnectionData.Elements.ContainsKey(newKey))
                {
                    ConnectionData.Elements.Add(newKey, data.Value);
                }
            }
        }

        private Goal GetSlotSetting(string key, Goal defaultValue)
        {
            return (Goal)(_slotDataFields.ContainsKey(key) ? Enum.Parse(typeof(Goal), _slotDataFields[key].ToString()) : GetSlotDefaultValue(key, defaultValue));
        }

        private StrangeCoin GetSlotSetting(string key, StrangeCoin defaultValue)
        {
            return (StrangeCoin)(_slotDataFields.ContainsKey(key) ? Enum.Parse(typeof(StrangeCoin), _slotDataFields[key].ToString()) : GetSlotDefaultValue(key, defaultValue));
        }

        private string GetSlotSetting(string key, string defaultValue)
        {
            return _slotDataFields.ContainsKey(key) ? _slotDataFields[key].ToString() : GetSlotDefaultValue(key, defaultValue);
        }

        public int GetSlotSetting(string key, int defaultValue)
        {
            return _slotDataFields.ContainsKey(key) ? (int)(long)_slotDataFields[key] : GetSlotDefaultValue(key, defaultValue);
        }

        private bool GetSlotSetting(string key, bool defaultValue)
        {
            if (_slotDataFields.ContainsKey(key) && _slotDataFields[key] != null && _slotDataFields[key] is bool boolValue)
            {
                return boolValue;
            }
            if (_slotDataFields[key] is string strValue && bool.TryParse(strValue, out var parsedValue))
            {
                return parsedValue;
            }
            if (_slotDataFields[key] is int intValue)
            {
                return intValue != 0;
            }
            if (_slotDataFields[key] is long longValue)
            {
                return longValue != 0;
            }
            if (_slotDataFields[key] is short shortValue)
            {
                return shortValue != 0;
            }

            return GetSlotDefaultValue(key, defaultValue);
        }

        private T GetSlotDefaultValue<T>(string key, T defaultValue)
        {
            _log.LogWarning($"SlotData did not contain expected key: \"{key}\"");
            return defaultValue;
        }

        private int ParseCoinBundle(StrangeCoin strangeCoin)
        {
            switch (strangeCoin)
            {
                case StrangeCoin.One:
                {
                    return 1;
                }
                case StrangeCoin.Two:
                {
                    return 2;
                }
                case StrangeCoin.Three:
                {
                    return 3;
                }
                case StrangeCoin.Five:
                {
                    return 5;
                }
                case StrangeCoin.Six:
                {
                    return 6;
                }
                case StrangeCoin.Ten:
                {
                    return 10;
                }
                case StrangeCoin.Fifteen:
                {
                    return 15;
                }
                case StrangeCoin.Thirty:
                {
                    return 30;
                }
            }
            return 10;
        }
    }

    public enum Goal
    {
        AnyEnding = 0,
        EndingA = 1,
        EndingB = 2,
        EndingCD = 3,
        EndingE = 4,
    }

    public enum StrangeCoin
    {
        One = 0,
        Two = 1,
        Three = 2,
        Five = 3,
        Six = 4,
        Ten = 5,
        Fifteen = 6,
        Thirty = 7,
    }
}