using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Archipelago.MultiClient.Net.Enums;
using BepInEx.Logging;
using LunacidAP.Data;
using UnityEngine;

namespace LunacidAP
{
    public class Colors
    {
        public const string PROGUSEFUL_COLOR_DEFAULT = "FF8000";
        public const string PROGRESSION_COLOR_DEFAULT = "#A335EE";
        public const string USEFUL_COLOR_DEFAULT = "#0070DD";
        public const string FILLER_COLOR_DEFAULT = "#1EFF00";
        public const string TRAP_COLOR_DEFAULT = "#FF0000";
        public const string CHEAT_COLOR_DEFAULT = "#FF0000";
        public const string GIFT_COLOR_DEFAULT = "#FF8DA1";

        private static ManualLogSource _log;

        public Colors(ManualLogSource log)
        {
            _log = log;
        }

        public static string GetGiftColor()
        {
            var isGiftAdded = ConnectionData.ItemColors.TryGetValue("Gift", out var gift);
            return isGiftAdded ? gift : GIFT_COLOR_DEFAULT;
        }

        public static string GetCheatColor()
        {
            var isCheatAdded = ConnectionData.ItemColors.TryGetValue("Cheat", out var cheat);
            return isCheatAdded ? cheat : CHEAT_COLOR_DEFAULT;
        }

        public static string GetClassificationHex(ItemFlags itemFlags)
        {
            var colors = new List<string>();
            var isProgUsefulAdded = ConnectionData.ItemColors.TryGetValue("ProgUseful", out var proguseful);
            var isProgressionAdded = ConnectionData.ItemColors.TryGetValue("Progression", out var progression);
            var isUsefulAdded = ConnectionData.ItemColors.TryGetValue("Unique", out var useful);
            var isTrapAdded = ConnectionData.ItemColors.TryGetValue("Trap", out var trap);
            var isFillerAdded = ConnectionData.ItemColors.TryGetValue("Filler", out var filler);
            if (itemFlags.HasFlag(ItemFlags.Advancement | ItemFlags.NeverExclude))
            {
                return isProgUsefulAdded ? proguseful : PROGUSEFUL_COLOR_DEFAULT;
            }
            else if (itemFlags.HasFlag(ItemFlags.Advancement))
            {
                return isProgressionAdded ? progression : PROGRESSION_COLOR_DEFAULT;
            }
            else if (itemFlags.HasFlag(ItemFlags.NeverExclude))
            {
                return isUsefulAdded ? useful : USEFUL_COLOR_DEFAULT;
            }
            else if (itemFlags.HasFlag(ItemFlags.Trap))
            {
                return isTrapAdded ? trap : TRAP_COLOR_DEFAULT;
            }
            return isFillerAdded ? filler : FILLER_COLOR_DEFAULT;
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

        private static string RGBToHexConverter(int[] rgb)
        {
            string first, second, third = "";
            first = rgb[0].ToString("X");
            if (first.Length == 1)
            {
                first = "0" + first;
            }
            second = rgb[1].ToString("X");
            if (second.Length == 1)
            {
                second = "0" + second;
            }
            third = rgb[2].ToString("X");
            if (third.Length == 1)
            {
                third = "0" + third;
            }
            var finalString = "#" + first + second + third;
            return finalString;
        }

    }
}