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
        private const string VERSION = "client_version";
        private const string CLASS_KEY = "starting_class";
        private const string ER_KEY = "entrance_randomization";
        private const string EXP_KEY = "experience";
        private const string WEXP_KEY = "weapon_experience";
        private const string SWITCH_KEY = "switch_locks";
        private const string DOOR_KEY = "door_locks";
        private const string COIN_KEY = "required_strange_coin";
        private const string ENDING_KEY = "ending";
        private const string SHOP_KEY = "shopsanity";
        private const string DROP_KEY = "dropsanity";
        private const string QUENCH_KEY = "quenchsanity";
        private const string ETNAS_KEY = "etnas_pupil";
        private const string NORM_DROP_KEY = "normalized_drops";
        private const string DL_KEY = "death_link";
        private const string RANDOM_ELE_KEY = "random_elements";
        private const string ELE_DICT_KEY = "elements";
        private const string WALL_KEY = "secret_door_lock";
        private const string ENT_KEY = "entrances";
        private const string REMOVAL_KEY = "remove_locations";
        private const string IS_CHRISTMAS = "is_christmas";
        private const string CUSTOM_NAME_KEY = "created_class_name";
        private const string CUSTOM_DESC_KEY = "created_class_description";
        private const string CUSTOM_CLASS_KEY = "created_class_stats";
        private const string CUSTOM_COLORS_KEY = "item_colors";
        private Dictionary<string, object> _slotDataFields;
        public int Seed {get; private set;}
        public Goal Ending {get; private set;}
        public int StartingClass {get; private set;}
        public bool EntranceRandomizer {get; private set;}
        public string ClientVersion {get; private set;}
        public Dropsanity Dropsanity {get; private set;}
        public bool Quenchsanity {get; private set;}
        public bool EtnasPupil {get; private set;}
        public bool NormalizedDrops {get; private set;}
        public bool Shopsanity {get; private set;}
        public bool Switchlock {get; private set;}
        public bool Doorlock {get; private set;}
        public float ExperienceMultiplier {get; private set;}
        public float WExperienceMultiplier {get; private set;}
        public int RequiredCoins {get; private set;}
        public bool DeathLink {get; private set;}
        public bool RandomElements {get; private set;}
        public bool FalseWalls {get; private set;}
        public List<string> RemovedLocations {get; private set;}
        public bool IsChristmas {get; private set;}
        public string CustomName {get; private set;}
        public string CustomDescription {get; private set;}
        public Dictionary<string, int> CustomStats {get; private set;}
        public Dictionary<string, string> ItemColors {get; private set;}

        public SlotData(Dictionary<string, object> slotDataFields, ManualLogSource log)
        {
            _log = log;
            _slotDataFields = slotDataFields;
            Ending = GetSlotSetting(ENDING_KEY, Goal.AnyEnding);
            StartingClass = GetSlotSetting(CLASS_KEY, 0);
            EntranceRandomizer = GetSlotSetting(ER_KEY, false);
            Seed = GetSlotSetting(SEED_KEY, 0);
            ClientVersion = GetSlotSetting(VERSION, "0.0.0");
            Dropsanity = GetSlotSetting(DROP_KEY, Dropsanity.Off);
            Quenchsanity = GetSlotSetting(QUENCH_KEY, false);
            EtnasPupil = GetSlotSetting(ETNAS_KEY, false);
            NormalizedDrops = GetSlotSetting(NORM_DROP_KEY, false);
            Shopsanity = GetSlotSetting(SHOP_KEY, false);
            Switchlock = GetSlotSetting(SWITCH_KEY, false);
            Doorlock = GetSlotSetting(DOOR_KEY, false);
            ExperienceMultiplier = (float) GetSlotSetting(EXP_KEY, 100)/100;
            WExperienceMultiplier = (float) GetSlotSetting(WEXP_KEY, 100)/100;
            RequiredCoins = GetSlotSetting(COIN_KEY, 30);
            DeathLink = GetSlotSetting(DL_KEY, false);
            RandomElements = GetSlotSetting(RANDOM_ELE_KEY, false);
            var elementsData = GetSlotSetting(ELE_DICT_KEY, "");
            FalseWalls = GetSlotSetting(WALL_KEY, false);
            var removedList = GetSlotSetting(REMOVAL_KEY, "");
            RemovedLocations = JsonConvert.DeserializeObject<List<string>>(removedList);
            IsChristmas = GetSlotSetting(IS_CHRISTMAS, false);
            CustomName = GetSlotSetting(CUSTOM_NAME_KEY, "");
            CustomDescription = GetSlotSetting(CUSTOM_DESC_KEY, "");
            var customStats = GetSlotSetting(CUSTOM_CLASS_KEY, "");
            CustomStats = JsonConvert.DeserializeObject<Dictionary<string, int>>(customStats);
            var itemColors = GetSlotSetting(CUSTOM_COLORS_KEY, "");
            ItemColors = JsonConvert.DeserializeObject<Dictionary<string, string>>(itemColors);
            
            foreach (var data in JsonConvert.DeserializeObject<Dictionary<string, string>>(elementsData))
            {
                var newKey = data.Key.ToUpper().Replace("'", "");
                if (!ConnectionData.Elements.ContainsKey(newKey))
                {
                    ConnectionData.Elements.Add(newKey, data.Value);
                }
            }
            var entranceData = GetSlotSetting(ENT_KEY, "");
            foreach (var data in JsonConvert.DeserializeObject<Dictionary<string, string>>(entranceData))
            {
                if (!ConnectionData.Entrances.ContainsKey(data.Key))
                {
                    ConnectionData.Entrances.Add(data.Key, data.Value);
                }
            }
            if (!ConnectionData.ItemColors.Any())
            {
                ConnectionData.ItemColors = ItemColors;
            }
            
        }

        private Goal GetSlotSetting(string key, Goal defaultValue)
        {
            return (Goal)(_slotDataFields.ContainsKey(key) ? Enum.Parse(typeof(Goal), _slotDataFields[key].ToString()) : GetSlotDefaultValue(key, defaultValue));
        }

        private Dropsanity GetSlotSetting(string key, Dropsanity defaultValue)
        {
            return (Dropsanity)(_slotDataFields.ContainsKey(key) ? Enum.Parse(typeof(Dropsanity), _slotDataFields[key].ToString()) : GetSlotDefaultValue(key, defaultValue));
        }

        private string GetSlotSetting(string key, string defaultValue)
        {
            return _slotDataFields.ContainsKey(key) ? _slotDataFields[key].ToString() : GetSlotDefaultValue(key, defaultValue);
        }

        public int GetSlotSetting(string key, int defaultValue)
        {
            return _slotDataFields.ContainsKey(key) ? (int)(long)_slotDataFields[key] : GetSlotDefaultValue(key, defaultValue);
        }

        public bool GetSlotSetting(string key, bool defaultValue)
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
    }

    public enum Goal
    {
        AnyEnding = 0,
        EndingA = 1,
        EndingB = 2,
        EndingCD = 3,
        EndingE = 4,
    }

    public enum Dropsanity
    {
        Off = 0,
        Unique = 1,
        Randomized = 2,
    }
}