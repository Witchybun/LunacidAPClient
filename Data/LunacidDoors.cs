using System.Collections.Generic;
using UnityEngine;

namespace LunacidAP.Data
{
    public class LunacidDoors
    {

        public static readonly Dictionary<string, string> EntranceToDoorKey = new(){
            {"Broken Steps Door (Hollow Basin Side)", "Broken Steps Door Key"},
            {"Broken Steps Door (Forbidden Archives Side)", "Broken Steps Door Key"},
            {"Sewers Door (The Fetid Mire Side)", "Sewers Door Key"},
            {"Sewers Door (Hollow Basin Side)", "Sewers Door Key"},
            {"Rickety Bridge Door (Hollow Basin Side)", "Lower Rickety Bridge Door Key"},
            {"Rickety Bridge Door (Yosei Forest Side)", "Lower Rickety Bridge Door Key"},
            {"Treetop Door (Yosei Forest Side)", "Treetop Door Key"},
            {"Treetop Door (Forest Canopy Path)", "Treetop Door Key"},
            {"Tomb Secret Door (Yosei Forest Side)", "Tomb Secret Door Key"},
            {"Tomb Secret Door (Accursed Tomb Side)", "Tomb Secret Door Key"},
            {"Library Exit Door (Forbidden Archives Side)", "Library Exit Door Key"},
            {"Library Exit Door (Laetus Chasm Side)", "Library Exit Door Key"},
            {"Surface Door (Laetus Chasm Side)", "Surface Door Key"},
            {"Surface Door (Great Well Surface Side)", "Surface Door Key"},
            {"Sewers Sea Door (The Fetid Mire Side)", "Sewers Sea Door Key"},
            {"Sewers Sea Door (Sanguine Sea Side)", "Sewers Sea Door Key"},
            {"Accursed Door (Sanguine Sea Side)", "Accursed Door Key"},
            {"Accursed Door (Accursed Tomb Side)", "Accursed Door Key"},
            {"Castle Doors (Sanguine Sea Side)", "Castle Doors Key"},
            {"Castle Doors (Castle Le Fanu Side)", "Castle Doors Key"},
            {"Light Accursed Door (Castle Le Fanu Side)", "Light Accursed Door Key"},
            {"Light Accursed Door (Sealed Ballroom Side)", "Light Accursed Door Key"},
            {"Prison Main Door (Throne Chamber Side)", "Prison Main Door Key"},
            {"Prison Main Door (Terminus Prison Side)", "Prison Main Door Key"},
            {"Queen's Throne Door (Castle Le Fanu Side)", "Queen's Throne Door Key"},
            {"Queen's Throne Door (Throne Chamber Side)", "Queen's Throne Door Key"},
            {"Secondary Door (Terminus Prison Side)", "Secondary Lock Key"},
            {"Secondary Door (Forlorn Arena)", "Secondary Lock Key"},
            {"Forbidden Door (Terminus Prison Side)", "Forbidden Door Key"},
            {"Forbidden Door (Labyrinth of Ash Side)", "Forbidden Door Key"},
            {"Burning Hot Door (Castle Le Fanu Side)", "Burning Hot Key"},
            {"Burning Hot Door (Boiling Grotto Side)", "Burning Hot Key"},
            {"Sucsarian Door (Forlorn Arena Side)", "Sucsarian Key"},
            {"Sucsarian Door (Chamber of Fate Side)", "Sucsarian Key"},
            {"Dreamer Door (Chamber of Fate Side)", "Dreamer Key"},
            {"Dreamer Door (Grave of the Sleeper Side)", "Dreamer Key"},
        };

        private static readonly Dictionary<string, Vector3> BasinEntranceToDoor = new(){
            {"Broken Steps Door (Hollow Basin Side)", new Vector3(61.75f, 22.25f, 7.5f)},
            {"Sewers Door (Hollow Basin Side)", new Vector3(163.63f, -24.718f, 241.3f)},
            {"Rickety Bridge Door (Hollow Basin Side)", new Vector3(40.2617f, -9.218f, -35.7f)}
        };

        private static readonly Dictionary<string, Vector3> ArchivesEntranceToDoor = new(){
            {"Broken Steps Door (Forbidden Archives Side)", new Vector3(55.25f, 5.25f, -36f)},
            {"Library Exit Door (Forbidden Archives Side)", new Vector3(-28.75f, 9.25f, 80f)}
        };

        private static readonly Dictionary<string, Vector3> ChasmEntranceToDoor = new(){
            {"Library Exit Door (Laetus Chasm Side)", new Vector3(-2f, 1.5f, 11.25f)},
            {"Surface Door (Laetus Chasm Side)", new Vector3(-107.25f, 35.25f, -38f)}
        };

        private static readonly Dictionary<string, Vector3> SurfaceEntranceToDoor = new(){
            {"Surface Door (Great Well Surface Side)", new Vector3(-24f, -2.75f, 101.25f)}
        };

        private static readonly Dictionary<string, Vector3> ForestEntranceToDoor = new(){
            {"Rickety Bridge Door (Yosei Forest Side)", new Vector3(35f, 37.5f, -15f)},
            {"Treetop Door (Yosei Forest Side)", new Vector3(-142.8437f, 50.69f, -19.8452f)},
            {"Tomb Secret Door (Yosei Forest Side)", new Vector3(76.25f, -49f, -153.5f)}
        };

        private static readonly Dictionary<string, Vector3> CanopyEntranceToDoor = new(){
            {"Treetop Door (Forest Canopy Path)", new Vector3(7.25f, 1.25f, 0f)}
        };

        private static readonly Dictionary<string, Vector3> TombEntranceToDoor = new(){
            {"Accursed Door (Accursed Tomb Side)", new Vector3(76f, -6.75f, 104.25f)},
            {"Tomb Secret Door (Accursed Tomb Side)", new Vector3(0.75f, 1.25f, 1f)}
        };

        private static readonly Dictionary<string, Vector3> MireEntranceToDoor = new(){
            {"Sewers Door (The Fetid Mire Side)", new Vector3(0f, 1.5f, 14.5f)},
            {"Sewers Sea Door (The Fetid Mire Side)", new Vector3(102f, -26.75f, -547.25f)}
        };

        private static readonly Dictionary<string, Vector3> SeaEntranceToDoor = new(){
            {"Sewers Sea Door (Sanguine Sea Side)", new Vector3(266.25f, 11.75f, -77.25f)},
            {"Castle Doors (Sanguine Sea Side)", new Vector3(1f, 7.25f, 271.5f)},
            {"Accursed Door (Sanguine Sea Side)", new Vector3(-171.25f, 8.5f, -71f)}
        };

        private static readonly Dictionary<string, Vector3> CastleEntranceToDoor = new(){
            {"Castle Doors (Castle Le Fanu Side)", new Vector3(1f, 1.25f, 1.25f)},
            {"Burning Hot Door (Castle Le Fanu Side)", new Vector3(39.8991f, -6.75f, -230.25f)},
            {"Queen's Throne Door (Castle Le Fanu Side)", new Vector3(-40f, 25.25f, -179.25f)},
            {"Light Accursed Door (Castle Le Fanu Side)", new Vector3(53.25f, 25.25f, -252f)}
        };

        private static readonly Dictionary<string, Vector3> BallroomEntranceToDoor = new(){
            {"Light Accursed Door (Sealed Ballroom Side)", new Vector3(0f, 1.25f, 33.25f)},
        };

        private static readonly Dictionary<string, Vector3> ThroneEntranceToDoor = new(){
            {"Queen's Throne Door (Throne Chamber Side)", new Vector3(0f, 8.25f, -19.75f)},
            {"Prison Main Door (Throne Chamber Side)", new Vector3(67.25f, 8.25f, -135f)}
        };

        private static readonly Dictionary<string, Vector3> GrottoEntranceToDoor = new(){
            {"Burning Hot Door (Boiling Grotto Side)", new Vector3(-21.25f, 17.25f, -1f)}
        };

        private static readonly Dictionary<string, Vector3> PrisonEntranceToDoor = new(){
            {"Prison Main Door (Terminus Prison Side)", new Vector3(56f, 19.25f, -67.25f)},
            {"Forbidden Door (Terminus Prison Side)", new Vector3(157.25f, -50.75f, -58f)},
            {"Secondary Door (Terminus Prison Side)", new Vector3(-8f, -14f, 62f)}
        };

        private static readonly Dictionary<string, Vector3> ArenaEntranceToDoor = new(){
            {"Secondary Door (Forlorn Arena)", new Vector3(66f, 6f, 0f)},
            {"Sucsarian Door (Forlorn Arena Side)", new Vector3(-179.25f, 5.39f, 0f)}
        };

        private static readonly Dictionary<string, Vector3> AshEntranceToDoor = new(){
            {"Forbidden Door (Labyrinth of Ash Side)", new Vector3(47.25f, 17.25f, 68f)}
        };

        private static readonly Dictionary<string, Vector3> FateEntranceToDoor = new(){
            {"Sucsarian Door (Chamber of Fate Side)", new Vector3(14f, 5.25f, -20.75f)},
            {"Dreamer Door (Chamber of Fate Side)", new Vector3(14f, 5.25f, -211.25f)}
        };

        private static readonly Dictionary<string, Vector3> SleeperEntranceToDoor = new(){
            {"Dreamer Door (Grave of the Sleeper Side)", new Vector3(-28.25f, 20.25f, -81.68f)}
        };

        public static readonly Dictionary<string, Dictionary<string, Vector3>> EntranceToDoor = new(){
            {"PITT_A1", BasinEntranceToDoor},
            {"FOREST_A1", ForestEntranceToDoor},
            {"FOREST_B1", CanopyEntranceToDoor},
            {"SEWER_A1", MireEntranceToDoor},
            {"ARCHIVES", ArchivesEntranceToDoor},
            {"WALL_01", ChasmEntranceToDoor},
            {"PITT_B1", SurfaceEntranceToDoor},
            {"HAUNT", TombEntranceToDoor},
            {"LAKE", SeaEntranceToDoor},
            {"CAS_1", CastleEntranceToDoor},
            {"CAVE", GrottoEntranceToDoor},
            {"VOID", AshEntranceToDoor},
            {"CAS_2", ThroneEntranceToDoor},
            {"CAS_3", BallroomEntranceToDoor},
            {"PRISON", PrisonEntranceToDoor},
            {"ARENA", ArenaEntranceToDoor},
            {"ARENA2", FateEntranceToDoor},
        };

            public static readonly Dictionary<string, string> SceneToDisplayName = new(){
            {"HUB_01", "Wing's Rest"},
            {"PITT_A1", "Hollow Basin"},
            {"FOREST_A1", "Yosei Forest"},
            {"FOREST_B1", "Forest Canopy"},
            {"SEWER_A1", "The Fetid Mire"},
            {"ARCHIVES", "Forbidden Archives"},
            {"WALL_01", "Laetus Chasm"},
            {"PITT_B1", "Great Well Surface"},
            {"HAUNT", "Accursed Tomb"},
            {"LAKE", "The Sanguine Sea"},
            {"CAS_1", "Castle Le Fanu"},
            {"CAVE", "Boiling Grotto"},
            {"VOID", "Labyrinth of Ash"},
            {"TOWER", "Tower of Abyss"},
            {"CAS_2", "A Holy Battlefield"},
            {"CAS_3", "Sealed Ballroom"},
            {"PRISON", "Terminus Prison"},
            {"ARENA", "Forlorn Arena"},
            {"ARENA2", "Chamber of Fate"},
            {"END_TOWN", "Grave of the Sleeper"}
        };

        public static readonly List<Vector3> BallroomDoors = new(){
            new Vector3(-48f, 1.9f, 6f), new Vector3(-48f, 1.9f, -70f), new Vector3(-54f, 1.9f, -44f), new Vector3(-54f, 1.9f, -24f),
            new Vector3(44f, 1.9f, -4f), new Vector3(80f, 1.9f, -34f), new Vector3(80f, 1.9f, -90f), new Vector3(-24f, 1.9f, -42f)
        };

        public static readonly List<Vector3> VoidDoors = new(){
            new Vector3(-76f, 1.9f, -120f), new Vector3(-76f, 1.9f, -52f), new Vector3(-108f, 1.9f, -80f), new Vector3(-108f, 1.9f, 8f),
            new Vector3(-68f, 1.9f, 68f), new Vector3(-68f, 1.9f, 44f), new Vector3(-68f, 1.9f, 12f), new Vector3(96f, 1.9f, -76f),
            new Vector3(224f, 1.9f, -10f)
        };

        public static Vector3 AbyssDoor = new(-119.25f, -26.75f, 96f);
        
    }
}