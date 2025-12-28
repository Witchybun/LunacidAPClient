using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using Archipelago.MultiClient.Net.Enums;
using BepInEx;
using BepInEx.Logging;
using LunacidAP.Data;
using Newtonsoft.Json;
using UnityEngine;

namespace LunacidAP
{
    public class Colors
    {

        private static readonly Dictionary<string, string> ArchipelagoColors = new()
        {
            {"Progression", "#A335EE"},
            {"Useful", "#0070DD"},
            {"Filler", "#1EFF00"},
            {"Trap", "#FF0000"},
            {"Cheat", "#FF0000"},
            {"Gift", "#FF8DA1"},
        };

        private static readonly Dictionary<string, string> MultiworldGGColors = new()
        {
            {"Progression", "#FFC500"},
            {"Useful", "#6D8BE8"},
            {"Filler", "#00EEEE"},
            {"Trap", "#FA8072"},
            {"Cheat", "#FF0000"},
            {"Gift", "#FF8DA1"},
        };

        private static Dictionary<string, string> CustomColors = new()
        {
            {"Progression", "#A335EE"},
            {"Useful", "#0070DD"},
            {"Filler", "#1EFF00"},
            {"Trap", "#FF0000"},
            {"Cheat", "#FF0000"},
            {"Gift", "#FF8DA1"},
        };

        private static readonly Dictionary<Plugin.RandoSettings.Colors, Dictionary<string, string>> ColorLookup = new();

        private static ManualLogSource _log;

        public Colors(ManualLogSource log)
        {
            _log = log;
        }

        public static string GetGiftColor()
        {;
            return ColorLookup[Plugin.randoSettings.ItemColors]["Gift"];
        }

        public static string GetCheatColor()
        {
            return ColorLookup[Plugin.randoSettings.ItemColors]["Cheat"];
        }

        public static void GrabCustomColors()
        {
            var mainDir = Path.Combine(Path.Combine(Paths.PluginPath, "LunacidAP"), "CustomColor.json");
            if (!File.Exists(mainDir))
            {
                var serializedDefault = JsonConvert.SerializeObject(ArchipelagoColors);
                File.WriteAllText(mainDir, serializedDefault);
                CustomColors = ArchipelagoColors;
            }
            else
            {
                using var colorReader = new StreamReader(mainDir);
                var colorText = colorReader.ReadToEnd();
                var customColors = JsonConvert.DeserializeObject<Dictionary<string, string>>(colorText);
                var finalColors = new Dictionary<string, string>();
                foreach (var kvp in customColors)
                {
                    if (!IsColorInRightFormat(kvp.Value))
                    {
                        finalColors[kvp.Key] =  ArchipelagoColors[kvp.Key];
                    }
                    else
                    {
                        finalColors[kvp.Key] = kvp.Value;
                    }
                }
                CustomColors = finalColors;
                colorReader.Close();
            }
            
            ColorLookup[Plugin.RandoSettings.Colors.Archipelago] = ArchipelagoColors;
            ColorLookup[Plugin.RandoSettings.Colors.Multiworldgg] = MultiworldGGColors;
            ColorLookup[Plugin.RandoSettings.Colors.Custom] = CustomColors;
        }

        public static string GetClassificationHex(ItemFlags itemFlags)
        {
            if (itemFlags.HasFlag(ItemFlags.Advancement))
            {
                return ColorLookup[Plugin.randoSettings.ItemColors]["Progression"];
            }
            else if (itemFlags.HasFlag(ItemFlags.NeverExclude))
            {
                return ColorLookup[Plugin.randoSettings.ItemColors]["Useful"];
            }
            else if (itemFlags.HasFlag(ItemFlags.Trap))
            {
                return ColorLookup[Plugin.randoSettings.ItemColors]["Trap"];
            }
            return ColorLookup[Plugin.randoSettings.ItemColors]["Filler"];
        }

        public static Color HexToColorConverter(string hex)
        {
            if (hex.IndexOf('#') != -1)
                hex = hex.Replace("#", "");
            int r, g, b = 0;
            r = int.Parse(hex.Substring(0, 2), NumberStyles.AllowHexSpecifier);
            g = int.Parse(hex.Substring(2, 2), NumberStyles.AllowHexSpecifier);
            b = int.Parse(hex.Substring(4, 2), NumberStyles.AllowHexSpecifier);
            return new Color(r/255f, g/255f, b/255f, 1f);
        }

        private static bool IsColorInRightFormat(string color)
        {
            var firstChar = color[0];
            var theRest = color.Substring(1);
            if (firstChar != '#')
            {
                return false;
            }
            if (!int.TryParse(theRest, out var _))
            {
                return false;
            }
            try
            {
                _ = HexToColorConverter(color);
            }
            catch
            {
                return false;
            }
            return true;
        }
    }
}