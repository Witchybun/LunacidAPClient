using System.Collections.Generic;

namespace LunacidAP.Data
{
    public class LunacidFlags
    {
        public static readonly Dictionary<string, int[]> ItemToFlag = new(){
            { "VHS Tape", new int[3]{4, 16, 1} },
            { "Corrupt Key", new int[3]{4, 16, 3} },
            { "Progressive Vampiric Symbol", new int[3]{7, 6, 1} }, // Make these progressive clearly.
            { "Skull of Josiah", new int[3]{6, 15, 5} },
            { "White VHS Tape", new int[3]{4, 16, 4} },
            { "Terminus Prison Key", new int[3]{12, 2, 1} },
            { "Hammer of Cruelty", new int[3]{12, 13, 3} },
            { "Earth Talisman", new int[3]{15, 1, 1} },
            { "Water Talisman", new int[3]{14, 16, 1} },
            { "Progressive Vampiric Symbol 2", new int[3]{7, 6, 2} }, // exist just for the value lookup
            { "Progressive Vampiric Symbol 3", new int[3]{7, 6, 3} }, // exist just for the value lookup
        };
    }

}