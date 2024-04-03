using System.Collections.Generic;
using UnityEngine;

namespace LunacidAP.Data
{
    public class LunacidDoors
    {

        public static readonly Dictionary<string, string> EntranceToDoorKey = new(){
            {"Hollow Basin to Forbidden Archives 2F", "Broken Steps Door Key"},
            {"Temple of Silence Interior to The Fetid Mire", "Sewers Door Key"},
            {"Temple of Silence Interior to Yosei Forest", "Lower Rickety Bridge Door Key"},
            {"Yosei Forest to Forest Canopy", "Treetop Door Key"},
            {"Yosei Forest Lower to Accursed Tomb", "Tomb Secret Door Key"},
            {"Forbidden Archives 3F to Laetus Chasm", "Library Exit Door Key"},
            {"Laetus Chasm to Great Well Surface", "Surface Door Key"},
            {"The Fetid Mire to The Sanguine Sea", "Sewers Sea Door Key"},
            {"The Sanguine Sea to Accursed Tomb", "Accursed Door Key"},
            {"The Sanguine Sea to Castle Le Fanu", "Castle Doors Key"},
            {"Castle Le Fanu Blue Area to Sealed Ballroom", "Light Accursed Door Key"},
            {"Throne Chamber to Terminus Prison", "Prison Main Door Key"},
            {"Castle Le Fanu White Area to Throne Chamber", "Queen's Throne Door Key"},
            {"Terminus Prison Dark Areas to Forlorn Arena", "Secondary Lock Key"},
            {"Terminus Prison Dark Areas to Labyrinth of Ash", "Forbidden Door Key"},
            {"Castle Le Fanu Blue Area to Boiling Grotto", "Burning Hot Key"},
            {"Forlorn Arena to Chamber of Fate", "Sucsarian Key"},
            {"Chamber of Fate to Grave of the Sleeper", "Dreamer Key"},
        };

        private static readonly Dictionary<string, Vector3> BasinEntranceToDoor = new(){
            {"Hollow Basin to Forbidden Archives 2F", new Vector3(61.75f, 22.25f, 7.5f)},
            {"Temple of Silence Interior to The Fetid Mire", new Vector3(163.63f, -24.718f, 241.3f)},
            {"Temple of Silence Interior to Yosei Forest", new Vector3(40.2617f, -9.218f, -35.7f)}
        };

        private static readonly Dictionary<string, Vector3> ArchivesEntranceToDoor = new(){
            {"Forbidden Archives 2F to Hollow Basin", new Vector3(55.25f, 5.25f, -36f)},
            {"Forbidden Archives 3F to Laetus Chasm", new Vector3(-28.75f, 9.25f, 80f)}
        };

        private static readonly Dictionary<string, Vector3> ChasmEntranceToDoor = new(){
            {"Laetus Chasm to Forbidden Archives 3F", new Vector3(-2f, 1.5f, 11.25f)},
            {"Laetus Chasm to Great Well Surface", new Vector3(-107.25f, 35.25f, -38f)}
        };

        private static readonly Dictionary<string, Vector3> SurfaceEntranceToDoor = new(){
            {"Great Well Surface to Laetus Chasm", new Vector3(-24f, -2.75f, 101.25f)}
        };

        private static readonly Dictionary<string, Vector3> ForestEntranceToDoor = new(){
            {"Yosei Forest to Temple of Silence Interior", new Vector3(35f, 37.5f, -15f)},
            {"Yosei Forest to Forest Canopy", new Vector3(-142.8437f, 50.69f, -19.8452f)},
            {"Yosei Forest Lower to Accursed Tomb", new Vector3(76.25f, -49f, -153.5f)}
        };

        private static readonly Dictionary<string, Vector3> CanopyEntranceToDoor = new(){
            {"Forest Canopy to Yosei Forest", new Vector3(7.25f, 1.25f, 0f)}
        };

        private static readonly Dictionary<string, Vector3> TombEntranceToDoor = new(){
            {"Accursed Tomb to The Sanguine Sea", new Vector3(76f, -6.75f, 104.25f)},
            {"Accursed Tomb to Yosei Forest Lower", new Vector3(0.75f, 1.25f, 1f)}
        };

        private static readonly Dictionary<string, Vector3> MireEntranceToDoor = new(){
            {"The Fetid Mire to Temple of Silence Interior", new Vector3(0f, 1.5f, 14.5f)},
            {"The Fetid Mire to The Sanguine Sea", new Vector3(102f, -26.75f, -547.25f)}
        };

        private static readonly Dictionary<string, Vector3> SeaEntranceToDoor = new(){
            {"The Sanguine Sea to The Fetid Mire", new Vector3(266.25f, 11.75f, -77.25f)},
            {"The Sanguine Sea to Castle Le Fanu", new Vector3(1f, 7.25f, 271.5f)},
            {"The Sanguine Sea to Accursed Tomb", new Vector3(-171.25f, 8.5f, -71f)}
        };

        private static readonly Dictionary<string, Vector3> CastleEntranceToDoor = new(){
            {"Castle Le Fanu to The Sanguine Sea", new Vector3(1f, 1.25f, 1.25f)},
            {"Castle Le Fanu Blue Area to Boiling Grotto", new Vector3(39.8991f, -6.75f, -230.25f)},
            {"Castle Le Fanu White Area to Throne Chamber", new Vector3(-40f, 25.25f, -179.25f)},
            {"Castle Le Fanu Blue Area to Sealed Ballroom", new Vector3(53.25f, 25.25f, -252f)}
        };

        private static readonly Dictionary<string, Vector3> BallroomEntranceToDoor = new(){
            {"Sealed Ballroom to Castle Le Fanu Blue Area", new Vector3(0f, 1.25f, 33.25f)},
        };

        private static readonly Dictionary<string, Vector3> ThroneEntranceToDoor = new(){
            {"Throne Chamber to Castle Le Fanu White Area", new Vector3(0f, 8.25f, -19.75f)},
            {"Throne Chamber to Terminus Prison", new Vector3(67.25f, 8.25f, -135f)}
        };

        private static readonly Dictionary<string, Vector3> GrottoEntranceToDoor = new(){
            {"Boiling Grotto to Castle Le Fanu Blue Area", new Vector3(-21.25f, 17.25f, -1f)}
        };

        private static readonly Dictionary<string, Vector3> PrisonEntranceToDoor = new(){
            {"Terminus Prison to Throne Chamber", new Vector3(56f, 19.25f, -67.25f)},
            {"Terminus Prison Dark Areas to Labyrinth of Ash", new Vector3(157.25f, -50.75f, -58f)},
            {"Terminus Prison Dark Areas to Forlorn Arena", new Vector3(-8f, -14f, 62f)}
        };

        private static readonly Dictionary<string, Vector3> ArenaEntranceToDoor = new(){
            {"Forlorn Arena to Terminus Prison Dark Areas", new Vector3(66f, 6f, 0f)},
            {"Forlorn Arena to Chamber of Fate", new Vector3(-179.25f, 5.39f, 0f)}
        };

        private static readonly Dictionary<string, Vector3> AshEntranceToDoor = new(){
            {"Labyrinth of Ash to Terminus Prison Dark Areas", new Vector3(47.25f, 17.25f, 68f)}
        };

        private static readonly Dictionary<string, Vector3> FateEntranceToDoor = new(){
            {"Chamber of Fate to Forlorn Arena", new Vector3(14f, 5.25f, -20.75f)},
            {"Chamber of Fate to Grave of the Sleeper", new Vector3(14f, 5.25f, -211.25f)}
        };

        private static readonly Dictionary<string, Vector3> SleeperEntranceToDoor = new(){
            {"Grave of the Sleeper to Chamber of Fate", new Vector3(-28.25f, 20.25f, -81.68f)}
        };

        public static readonly Dictionary<string, Dictionary<string, Vector3>> EntranceToDoor = new(){
            {"PITT_A1", BasinEntranceToDoor},
            {"FOREST_A1", ForestEntranceToDoor},
            {"FOREST_B1", CanopyEntranceToDoor},
            {"SEWERS_A1", MireEntranceToDoor},
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

        public static readonly List<Vector3> BallroomDoors = new(){
            new Vector3(-48f, 1.9f, 6f), new Vector3(-48f, 1.9f, -70f), new Vector3(-54f, 1.9f, -44f), new Vector3(-54f, 1.9f, -24f),
            new Vector3(44f, 1.9f, -4f), new Vector3(80f, 1.9f, -34f), new Vector3(80f, 1.9f, -90f), new Vector3(-24f, 1.9f, -42f)
        };

        public static Vector3 AbyssDoor = new Vector3(-119.25f, -26.75f, 96f);
        
    }
}