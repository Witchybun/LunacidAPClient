using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Archipelago.MultiClient.Net.Enums;
using LunacidAP.Data;

namespace LunacidAP
{
    public class Colors
    {
        public const string PROGRESSIVE_COLOR_ORIGINAL = "#AF99EF";
        public const string USEFUL_COLOR_ORIGINAL = "#6D8BE8";
        public const string FILLER_COLOR_ORIGINAL = "#00EEEE";
        public const string TRAP_COLOR_ORIGINAL = "#FA8072";
        public const string CHEAT_COLOR_ORIGINAL = "FF0000";
        public const string GIFT_COLOR_ORIGINAL = "#9DAE11";

        public const string PROGRESSION_COLOR_DEFAULT = "#FFA500";
        public const string USEFUL_COLOR_DEFAULT = "#0000FF";
        public const string FILLER_COLOR_DEFAULT = "#000000";
        public const string TRAP_COLOR_DEFAULT = "#FF1493";
        public const string GIFT_COLOR_DEFAULT = "#00CED1";
        public const string CHEAT_COLOR_DEFAULT = "#00FF00";

        public static string DetermineItemColor(ItemFlags itemFlags)
        {
            var colors = new List<string>();
            if (itemFlags.HasFlag(ItemFlags.Advancement))
            {
                colors.Add(PROGRESSION_COLOR_DEFAULT);
            }
            if (itemFlags.HasFlag(ItemFlags.NeverExclude))
            {
                colors.Add(USEFUL_COLOR_DEFAULT);
            }
            if (itemFlags.HasFlag(ItemFlags.Trap))
            {
                colors.Add(TRAP_COLOR_DEFAULT);
            }
            if (!colors.Any())
            {
                return "#000000"; // Its filler
            }
            return ColorMixer(colors);
        }

        public static string GetGiftColor()
        {
            return GIFT_COLOR_DEFAULT;
        }

        private static string ColorMixer(List<string> colors)
        {
            var colorRGBs = new List<int[]>();
            foreach (var color in colors)
            {
                colorRGBs.Add(HexToRGBConverter(color));
            }
            int avgR = 0;
            int avgG = 0;
            int avgB = 0;
            for (var i = 0; i < colors.Count; i++)
            {
                avgR += colorRGBs[i][0];
            }
            for (var j = 0; j < colors.Count; j++)
            {
                avgG += colorRGBs[j][1];
            }
            for (var k = 0; k < colors.Count; k++)
            {
                avgB += colorRGBs[k][2];
            }
            avgR /= 3;
            avgG /= 3;
            avgB /= 3;
            var averageRGB = new int[]{avgR, avgG, avgB};
            return RGBToHexConverter(averageRGB);
        }

        private static int[] HexToRGBConverter(string hex)
        {
            if (hex.IndexOf('#') != -1)                
                hex = hex.Replace("#", "");            
            int r,g,b = 0;             
            r = int.Parse(hex.Substring(0, 2), NumberStyles.AllowHexSpecifier);             
            g = int.Parse(hex.Substring(2, 2), NumberStyles.AllowHexSpecifier);             
            b = int.Parse(hex.Substring(4, 2), NumberStyles.AllowHexSpecifier);            
            return new int[]{r, g, b};
        }

        private static string RGBToHexConverter(int[] rgb)
        {
            string first, second, third = "";
            first = rgb[0].ToString("X");
            second = rgb[1].ToString("X");
            third = rgb[2].ToString("X");
            var finalString = "#" + first + second + third;
            return finalString;
        }

    }
}