using System.Collections.Generic;
using UnityEngine;

namespace LunacidAP.Data
{
    public class WarpDestinations
    {
        public class WarpData
        {
            public string Scene;
            public Vector3 Position;
            public float Rotation;
            public string ParentScene;

            public WarpData(string scene, Vector3 position, float rotation, string parentScene = "")
            {
                Scene = scene;
                Position = position;
                Rotation = rotation;
                ParentScene = parentScene;
            }
        }

        public static readonly Dictionary<string, WarpData> EntranceToWhereItShouldLead = new(){
            {"Sewers Door (The Fetid Mire Side)", new WarpData("PITT_A1", new Vector3(163f, -25.7f, 239f), 180)},
            {"Sewers Door (Hollow Basin Side)", new WarpData("SEWER_A1", new Vector3(0.1f, 1.25f, 12.5f), 180)},
            {"Rickety Bridge Door (Hollow Basin Side)", new WarpData("FOREST_A1", new Vector3(35f, 37f, -14f), 0)},
            {"Rickety Bridge Door (Yosei Forest Side)", new WarpData("PITT_A1", new Vector3(42.5f, -9.5f, -35.6f), 90)},
            {"Broken Steps Door (Hollow Basin Side)", new WarpData("ARCHIVES", new Vector3(54f, 4.5f, -36f), 270)},
            {"Broken Steps Door (Forbidden Archives Side)", new WarpData("PITT_A1", new Vector3(62.5f, 21.6f, 7.45f), 90)},
            {"Library Exit Door (Forbidden Archives Side)", new WarpData("WALL_01", new Vector3(-2f, 0f, 10f), 180)},
            {"Library Exit Door (Laetus Chasm Side)", new WarpData("ARCHIVES", new Vector3(-31f, 8f, 81f), 270)},
            {"Surface Door (Laetus Chasm Side)", new WarpData("PITT_B1", new Vector3(-24f, -4f, 100f), 180)},
            {"Surface Door (Great Well Surface Side)", new WarpData("WALL_01", new Vector3(-106f, 34f, -38f), 90)},
            {"Treetop Door (Yosei Forest Side)", new WarpData("FOREST_B1", new Vector3(5f, 1f, 0f), 270)},
            {"Treetop Door (Forest Canopy Path)", new WarpData("FOREST_A1", new Vector3(-144.7f, 49.4f, -18f), 302)},
            {"Sewers Sea Door (The Fetid Mire Side)", new WarpData("LAKE", new Vector3(264f, 10.5f, -77.25f), 270)},
            {"Sewers Sea Door (Sanguine Sea Side)", new WarpData("SEWER_A1", new Vector3(102f, -28f, -546.75f), 0)},
            {"Accursed Door (Sanguine Sea Side)", new WarpData("HAUNT", new Vector3(76f, -7f, 103f), 180)},
            {"Accursed Door (Accursed Tomb Side)", new WarpData("LAKE", new Vector3(-170f, 9f, -71f), 90)},
            {"Tomb Secret Door (Accursed Tomb Side)", new WarpData("FOREST_A1", new Vector3(74f, -50f, -153f), 270)},
            {"Tomb Secret Door (Yosei Forest Side)", new WarpData("HAUNT", new Vector3(3f, 2f, 1f), 90)},
            {"Castle Doors (Sanguine Sea Side)", new WarpData("CAS_1", new Vector3(0.1f, 0.1f, -2f), 180)},
            {"Castle Doors (Castle Le Fanu Side)", new WarpData("LAKE", new Vector3(0.01f, 6f, 269f), 180)},
            {"Light Accursed Door (Castle Le Fanu Side)", new WarpData("CAS_3", new Vector3(0.1f, 0f, 31f), 180)},
            {"Light Accursed Door (Sealed Ballroom Side)", new WarpData("CAS_1", new Vector3(51f, 24f, -252f), 270)},
            {"Burning Hot Door (Castle Le Fanu Side)", new WarpData("CAVE", new Vector3(-20f, 16f, -1f), 90)},
            {"Burning Hot Door (Boiling Grotto Side)", new WarpData("CAS_1", new Vector3(40f, -8f, -229f), 0)},
            // {"Tower of Abyss Entrance to Boiling Grotto", new WarpData("CAVE", new Vector3(-72.505f, 9f, -142.2955f), 215)},
            {"Prison Main Door (Throne Chamber Side)", new WarpData("PRISON", new Vector3(56f, 18f, -66f), 0)},
            {"Prison Main Door (Terminus Prison Side)", new WarpData("CAS_2", new Vector3(66f, 7f, -135f), 270)},
            {"Secondary Door (Terminus Prison Side)", new WarpData("ARENA", new Vector3(64f, 4f, 0f), 270)},
            {"Secondary Door (Forlorn Arena Side)", new WarpData("PRISON", new Vector3(-8f, -16f, 60f), 180)},
            {"Forbidden Door (Terminus Prison Side)", new WarpData("VOID", new Vector3(46f, 16f, 68f), 270)},
            {"Forbidden Door (Labyrinth of Ash Side)", new WarpData("PRISON", new Vector3(156f, -52f, -58f), 270)},
            {"Jump from Castle Le Fanu Walls", new WarpData("CAS_PITT", new Vector3(1f, 16f, 5f), 0)},
            {"Climb Rope Out Of Battlefield", new WarpData("CAS_1", new Vector3(84f, 5f, -87f), 270)},
            {"Queen's Throne Door (Castle Le Fanu Side)", new WarpData("CAS_2", new Vector3(0.1f, 7f, -21f), 180)},
            {"Queen's Throne Door (Throne Chamber Side)", new WarpData("CAS_1", new Vector3(-40f, 24f, -178f), 0)},
            {"Hollow Basin Ceiling", new WarpData("PITT_B1", new Vector3(-3.7f, 0.73f, 19f), 180)},
            {"Surface Floor Holes", new WarpData("PITT_A1", new Vector3(70.36f, 37.5f, -16.5f), 0)},
        };

        public static readonly Dictionary<string, WarpData> EntranceItsActualWarpPosition = new(){
            {"Sewers Door (Hollow Basin Side)", new WarpData("PITT_A1", new Vector3(163f, -25.7f, 239f), 180)},
            {"Sewers Door (The Fetid Mire Side)", new WarpData("SEWER_A1", new Vector3(0.1f, 1.25f, 12.5f), 180)},
            {"Rickety Bridge Door (Yosei Forest Side)", new WarpData("FOREST_A1", new Vector3(35f, 37f, -14f), 0)},
            {"Rickety Bridge Door (Hollow Basin Side)", new WarpData("PITT_A1", new Vector3(42.5f, -9.5f, -35.6f), 90)},
            {"Broken Steps Door (Forbidden Archives Side)", new WarpData("ARCHIVES", new Vector3(54f, 4.5f, -36f), 270)},
            {"Broken Steps Door (Hollow Basin Side)", new WarpData("PITT_A1", new Vector3(62.5f, 21.6f, 7.45f), 90)},
            {"Library Exit Door (Laetus Chasm Side)", new WarpData("WALL_01", new Vector3(-2f, 0f, 10f), 180)},
            {"Library Exit Door (Forbidden Archives Side)", new WarpData("ARCHIVES", new Vector3(-31f, 8f, 81f), 270)},
            {"Surface Door (Great Well Surface Side)", new WarpData("PITT_B1", new Vector3(-24f, -4f, 100f), 180)},
            {"Surface Door (Laetus Chasm Side)", new WarpData("WALL_01", new Vector3(-106f, 34f, -38f), 90)},
            {"Treetop Door (Forest Canopy Path)", new WarpData("FOREST_B1", new Vector3(5f, 1f, 0f), 270)},
            {"Treetop Door (Yosei Forest Side)", new WarpData("FOREST_A1", new Vector3(-144.7f, 49.4f, -18f), 302)},
            {"Sewers Sea Door (Sanguine Sea Side)", new WarpData("LAKE", new Vector3(264f, 10.5f, -77.25f), 270)},
            {"Sewers Sea Door (The Fetid Mire Side)", new WarpData("SEWER_A1", new Vector3(102f, -28f, -546.75f), 0)},
            {"Accursed Door (Accursed Tomb Side)", new WarpData("HAUNT", new Vector3(76f, -7f, 103f), 180)},
            {"Accursed Door (Sanguine Sea Side)", new WarpData("LAKE", new Vector3(-170f, 9f, -71f), 90)},
            {"Tomb Secret Door (Yosei Forest Side)", new WarpData("FOREST_A1", new Vector3(74f, -50f, -153f), 270)},
            {"Tomb Secret Door (Accursed Tomb Side)", new WarpData("HAUNT", new Vector3(3f, 2f, 1f), 90)},
            {"Castle Doors (Castle Le Fanu Side)", new WarpData("CAS_1", new Vector3(0.1f, 0.1f, -2f), 180)},
            {"Castle Doors (Sanguine Sea Side)", new WarpData("LAKE", new Vector3(0.01f, 6f, 269f), 180)},
            {"Light Accursed Door (Sealed Ballroom Side)", new WarpData("CAS_3", new Vector3(0.1f, 0f, 31f), 180)},
            {"Light Accursed Door (Castle Le Fanu Side)", new WarpData("CAS_1", new Vector3(51f, 24f, -252f), 270)},
            {"Burning Hot Door (Boiling Grotto Side)", new WarpData("CAVE", new Vector3(-20f, 16f, -1f), 90)},
            {"Burning Hot Door (Castle Le Fanu Side)", new WarpData("CAS_1", new Vector3(40f, -8f, -229f), 0)},
            // {"Tower of Abyss Entrance to Boiling Grotto", new WarpData("CAVE", new Vector3(-72.505f, 9f, -142.2955f), 215)},
            {"Prison Main Door (Terminus Prison Side)", new WarpData("PRISON", new Vector3(56f, 18f, -66f), 0)},
            {"Prison Main Door (Throne Chamber Side)", new WarpData("CAS_2", new Vector3(66f, 7f, -135f), 270)},
            {"Secondary Door (Forlorn Arena Side)", new WarpData("ARENA", new Vector3(64f, 4f, 0f), 270)},
            {"Secondary Door (Terminus Prison Side)", new WarpData("PRISON", new Vector3(-8f, -16f, 60f), 180)},
            {"Forbidden Door (Labyrinth of Ash Side)", new WarpData("VOID", new Vector3(46f, 16f, 68f), 270)},
            {"Forbidden Door (Terminus Prison Side)", new WarpData("PRISON", new Vector3(156f, -52f, -58f), 270)},
            {"Climb Rope Out Of Battlefield", new WarpData("CAS_PITT", new Vector3(1f, 16f, 5f), 0)},
            {"Jump from Castle Le Fanu Walls", new WarpData("CAS_1", new Vector3(84f, 5f, -87f), 270)},
            {"Queen's Throne Door (Throne Chamber Side)", new WarpData("CAS_2", new Vector3(0.1f, 7f, -21f), 180)},
            {"Queen's Throne Door (Castle Le Fanu Side)", new WarpData("CAS_1", new Vector3(-40f, 24f, -178f), 0)},
            {"Surface Floor Holes", new WarpData("PITT_B1", new Vector3(-3.7f, 0.73f, 19f), 180, "PITT_A1")},
            {"Hollow Basin Ceiling", new WarpData("PITT_A1", new Vector3(70.36f, 37.5f, -16.5f), 0)},
        };

        // Fix warps to be either more consistent or not kill you.
        public static readonly Dictionary<WarpData, WarpData> WarpFixes = new(){
            {new WarpData("PITT_A1", new Vector3(70.36f, 37.5f, -16.5f), 0), new WarpData("PITT_A1", new Vector3(35.404f, 15f, -15.1075f), 98)},
            {new WarpData("CAS_PITT", new Vector3(1f, 16f, 5f), 90), new WarpData("CAS_PITT", new Vector3(149f, 1f, -79f), 0)},
            {new WarpData("ARENA", new Vector3(-178.0f, 4.0f, 0.0f), 90), new WarpData("ARENA", new Vector3(-145.021f, 5f, 0.0392f), 90)}
        };

        public static readonly Dictionary<int, string> StartingAreaDataToStartingSubstring = new()
        {
            {0, "1000000000000001"}, // Hollow Basin
            {1, "1010000000000000"}, // Mire
            {2, "1100000000000000"}, // Forest
            {3, "1000000100000000"}, // Archives
            {4, "1001000000000000"}, // Tomb
            {5, "1000100000000000"}, // Castle
            {6, "1000010000000000"}, // Grotto
            {7, "1000001000000000"}, // Prison
            {8, "1000000010000000"}, // Arena
            {9, "1000000001000000"}, // Ash
        };

        public static readonly Dictionary<string, string> DefaultEntrances = new()
        {
            {"Sewers Door (Hollow Basin Side)", "Sewers Door (The Fetid Mire Side)"},
            {"Sewers Door (The Fetid Mire Side)", "Sewers Door (Hollow Basin Side)"},
            {"Rickety Bridge Door (Yosei Forest Side)", "Rickety Bridge Door (Hollow Basin Side)"},
            {"Rickety Bridge Door (Hollow Basin Side)", "Rickety Bridge Door (Yosei Forest Side)"},
            {"Broken Steps Door (Forbidden Archives Side)", "Broken Steps Door (Hollow Basin Side)"},
            {"Broken Steps Door (Hollow Basin Side)", "Broken Steps Door (Forbidden Archives Side)"},
            {"Library Exit Door (Laetus Chasm Side)", "Library Exit Door (Forbidden Archives Side)"},
            {"Library Exit Door (Forbidden Archives Side)", "Library Exit Door (Laetus Chasm Side)"},
            {"Surface Door (Great Well Surface Side)", "Surface Door (Laetus Chasm Side)"},
            {"Surface Door (Laetus Chasm Side)", "Surface Door (Great Well Surface Side)"},
            {"Treetop Door (Forest Canopy Path)", "Treetop Door (Yosei Forest Side)"},
            {"Treetop Door (Yosei Forest Side)", "Treetop Door (Forest Canopy Path)"},
            {"Sewers Sea Door (Sanguine Sea Side)", "Sewers Sea Door (The Fetid Mire Side)"},
            {"Sewers Sea Door (The Fetid Mire Side)", "Sewers Sea Door (Sanguine Sea Side)"},
            {"Accursed Door (Accursed Tomb Side)", "Accursed Door (Sanguine Sea Side)"},
            {"Accursed Door (Sanguine Sea Side)", "Accursed Door (Accursed Tomb Side)"},
            {"Tomb Secret Door (Yosei Forest Side)", "Tomb Secret Door (Accursed Tomb Side)"},
            {"Tomb Secret Door (Accursed Tomb Side)", "Tomb Secret Door (Yosei Forest Side)"},
            {"Castle Doors (Castle Le Fanu Side)", "Castle Doors (Sanguine Sea Side)"},
            {"Castle Doors (Sanguine Sea Side)", "Castle Doors (Castle Le Fanu Side)"},
            {"Light Accursed Door (Sealed Ballroom Side)", "Light Accursed Door (Castle Le Fanu Side)"},
            {"Light Accursed Door (Castle Le Fanu Side)", "Light Accursed Door (Sealed Ballroom Side)"},
            {"Burning Hot Door (Boiling Grotto Side)", "Burning Hot Door (Castle Le Fanu Side)"},
            {"Burning Hot Door (Castle Le Fanu Side)", "Burning Hot Door (Boiling Grotto Side)"},
            {"Prison Main Door (Terminus Prison Side)", "Prison Main Door (Throne Chamber Side)"},
            {"Prison Main Door (Throne Chamber Side)", "Prison Main Door (Terminus Prison Side)"},
            {"Secondary Door (Forlorn Arena Side)", "Secondary Door (Terminus Prison Side)"},
            {"Secondary Door (Terminus Prison Side)", "Secondary Door (Forlorn Arena Side)"},
            {"Forbidden Door (Labyrinth of Ash Side)", "Forbidden Door (Terminus Prison Side)"},
            {"Forbidden Door (Terminus Prison Side)", "Forbidden Door (Labyrinth of Ash Side)"},
            {"Climb Rope Out Of Battlefield", "Jump from Castle Le Fanu Walls"},
            {"Jump from Castle Le Fanu Walls", "Climb Rope Out Of Battlefield"},
            {"Queen's Throne Door (Throne Chamber Side)", "Queen's Throne Door (Castle Le Fanu Side)"},
            {"Queen's Throne Door (Castle Le Fanu Side)", "Queen's Throne Door (Throne Chamber Side)"},
            {"Surface Floor Holes", "Hollow Basin Ceiling"},
            {"Hollow Basin Ceiling", "Surface Floor Holes"},
        };
    }
}