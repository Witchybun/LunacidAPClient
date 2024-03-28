using System.Collections.Generic;

namespace LunacidAP.Data
{
    public class LunacidFlags
    {
        public static readonly Dictionary<string, LunacidItemFlags> ItemToFlag = new(){
            {"VHS Tape", new LunacidItemFlags(new int[3]{4, 16, 1}, "PITT_A1")},
            { "Vampiric Symbol (W)", new LunacidItemFlags(new int[3]{7, 6, 1}, "CAS_1") },
            { "Vampiric Symbol (A)", new LunacidItemFlags(new int[3]{7, 6, 2}, "CAS_1") },
            { "Vampiric Symbol (E)", new LunacidItemFlags(new int[3]{7, 6, 3}, "CAS_1") },
            { "Skull of Josiah", new LunacidItemFlags(new int[3]{6, 15, 5}, "FOREST_B1") },
            { "White VHS Tape", new LunacidItemFlags(new int[3]{4, 16, 4}, "HAUNT") },
            { "Terminus Prison Key", new LunacidItemFlags(new int[3]{12, 2, 1}, "PRISON") },
            { "Hammer of Cruelty", new LunacidItemFlags(new int[3]{12, 13, 3}, "PRISON") },
            { "Earth Talisman", new LunacidItemFlags(new int[3]{15, 1, 1}, "ARENA") },
            { "Water Talisman", new LunacidItemFlags(new int[3]{14, 16, 1}, "ARENA") },
        };

        public class LunacidItemFlags
        {
            public int[] Flag;
            public string Scene;

            public LunacidItemFlags(int[] flag, string scene)
            {
                Flag = flag;
                Scene = scene;
            }
        }
    }

}