using System.Collections.Generic;

namespace LunacidAP.Data
{
    public class LunacidFlags
    {
        public static readonly Dictionary<string, int[]> ItemToFlag = new(){
            { "VHS Tape", new int[3]{4, 16, 1} },
            { "Skull of Josiah", new int[3]{6, 15, 5} }, // Check to see if this flag is based on the Patchouli quest; revert back if needed.
            { "Progressive Vampiric Symbol", new int[3]{7, 6, 1} }, // Make these progressive clearly.
            { "White VHS Tape", new int[3]{0, 0, 0} },
            { "Terminus Prison Key", new int[3]{12, 2, 1} },
            { "Hammer of Cruelty", new int[3]{12, 13, 3} },
            { "Earth Talisman", new int[3]{15, 1, 1} },
            { "Water Talisman", new int[3]{14, 16, 1} },
        };
    }

}