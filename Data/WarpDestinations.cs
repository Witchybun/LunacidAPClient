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

            public WarpData(string scene, Vector3 position, float rotation)
            {
                Scene = scene;
                Position = position;
                Rotation = rotation;
            }
        }

        public static readonly Dictionary<string, WarpData> EntranceToWarp = new(){
            {"The Fetid Mire to Temple of Silence Interior", new WarpData("PITT_A1", new Vector3(163f, -25.7f, 239f), 180)},
            {"Temple of Silence Interior to The Fetid Mire", new WarpData("SEWER_A1", new Vector3(0.1f, 1.25f, 12.5f), 180)},
            {"Temple of Silence Interior to Yosei Forest", new WarpData("FOREST_A1", new Vector3(35f, 37f, -14f), 0)},
            {"Yosei Forest to Temple of Silence Interior", new WarpData("PITT_A1", new Vector3(42.5f, -9.5f, -35.6f), 90)},
            {"Hollow Basin to Forbidden Archives 2F", new WarpData("ARCHIVES", new Vector3(54f, 4.5f, -36f), 270)},
            {"Forbidden Archives 2F to Hollow Basin", new WarpData("PITT_A1", new Vector3(62.5f, 21.6f, 7.45f), 90)},
            {"Forbidden Archives 3F to Laetus Chasm", new WarpData("WALL_01", new Vector3(-2f, 0f, 10f), 180)},
            {"Laetus Chasm to Forbidden Archives 3F", new WarpData("ARCHIVES", new Vector3(-31f, 8f, 81f), 270)},
            {"Laetus Chasm to Great Well Surface", new WarpData("PITT_B1", new Vector3(-24f, -4f, 100f), 180)},
            {"Great Well Surface to Laetus Chasm", new WarpData("WALL_01", new Vector3(-106f, 34f, -38f), 90)},
            {"Yosei Forest to Forest Canopy", new WarpData("FOREST_B1", new Vector3(5f, 1f, 0f), 270)},
            {"Forest Canopy to Yosei Forest", new WarpData("FOREST_A1", new Vector3(-144.7f, 49.4f, -18f), 302)},
            {"The Fetid Mire to The Sanguine Sea", new WarpData("LAKE", new Vector3(264f, 10.5f, -77.25f), 270)},
            {"The Sanguine Sea to The Fetid Mire", new WarpData("SEWER_A1", new Vector3(102f, -28f, -546.75f), 0)},
            {"The Sanguine Sea to Accursed Tomb Lobby", new WarpData("HAUNT", new Vector3(76f, -7f, 103f), 180)},
            {"Accursed Tomb Lobby to The Sanguine Sea", new WarpData("LAKE", new Vector3(-170f, 9f, -71f), 90)},
            {"Accursed Tomb Upper Lobby to Yosei Forest Lower", new WarpData("FOREST_A1", new Vector3(74f, -50f, -153f), 270)},
            {"Yosei Forest Lower to Accursed Tomb Upper Lobby", new WarpData("HAUNT", new Vector3(3f, 2f, 1f), 90)},
            {"The Sanguine Sea to Castle Le Fanu", new WarpData("CAS_1", new Vector3(0.1f, 0.1f, -2f), 180)},
            {"Castle Le Fanu to The Sanguine Sea", new WarpData("LAKE", new Vector3(0.01f, 6f, 269f), 180)},
            {"Castle Le Fanu Blue Area to Sealed Ballroom", new WarpData("CAS_3", new Vector3(0.1f, 0f, 31f), 180)},
            {"Sealed Ballroom to Castle Le Fanu Blue Area", new WarpData("CAS_1", new Vector3(51f, 24f, -252f), 270)},
            {"Castle Le Fanu Blue Area to Boiling Grotto", new WarpData("CAVE", new Vector3(-20f, 16f, -1f), 90)},
            {"Boiling Grotto to Castle Le Fanu Blue Area", new WarpData("CAS_1", new Vector3(40f, -8f, -229f), 0)},
            {"Boiling Grotto to Tower of Abyss Entrance", new WarpData("TOWER", new Vector3(88f, -31f, 104f), 180)},
            {"Tower of Abyss Entrance to Boiling Grotto", new WarpData("CAVE", new Vector3(-72.505f, 9f, -142.2955f), 215)},
            {"Throne Chamber to Terminus Prison", new WarpData("PRISON", new Vector3(56f, 18f, -66f), 0)},
            {"Terminus Prison to Throne Chamber", new WarpData("CAS_2", new Vector3(66f, 7f, -135f), 270)},
            {"Terminus Prison Dark Areas to Forlorn Arena", new WarpData("ARENA", new Vector3(64f, 4f, 0f), 270)},
            {"Forlorn Arena to Terminus Prison Dark Areas", new WarpData("PRISON", new Vector3(-8f, -16f, 60f), 180)},
            {"Terminus Prison Dark Areas to Labyrinth of Ash Entrance", new WarpData("VOID", new Vector3(46f, 16f, 68f), 270)},
            {"Labyrinth of Ash Entrance to Terminus Prison Dark Areas", new WarpData("PRISON", new Vector3(156f, -52f, -58f), 270)},
            {"Castle Le Fanu to A Holy Battleground", new WarpData("CAS_PITT", new Vector3(1f, 16f, 5f), 0)},
            {"A Holy Battleground to Castle Le Fanu", new WarpData("CAS_1", new Vector3(84f, 5f, -87f), 270)},
            {"Castle Le Fanu White Area to Throne Chamber", new WarpData("CAS_2", new Vector3(0.1f, 7f, -21f), 180)},
            {"Throne Chamber to Castle Le Fanu White Area", new WarpData("CAS_1", new Vector3(-40f, 24f, -178f), 0)},
            {"Hollow Basin to Great Well Surface", new WarpData("PITT_B1", new Vector3(-3.7f, 0.73f, 19f), 180)},
            {"Great Well Surface to Hollow Basin", new WarpData("PITT_A1", new Vector3(70.36f, 37.5f, -16.5f), 0)},
        };

        // Fix warps to be either more consistent or not kill you.
        public static readonly Dictionary<WarpData, WarpData> WarpFixes = new(){
            {new WarpData("PITT_A1", new Vector3(70.36f, 37.5f, -16.5f), 0), new WarpData("PITT_A1", new Vector3(35.404f, 15f, -15.1075f), 98)},
            {new WarpData("CAS_PITT", new Vector3(1f, 16f, 5f), 90), new WarpData("CAS_PITT", new Vector3(149f, 1f, -79f), 0)},
            {new WarpData("ARENA", new Vector3(-178.0f, 4.0f, 0.0f), 90), new WarpData("ARENA", new Vector3(-145.021f, 5f, 0.0392f), 90)}
        };

        public static readonly Dictionary<string, WarpData> StartingArea = new(){
            {"Yosei Forest", new WarpData("FOREST_A1", new Vector3(37.4649f, 36.9636f, -0.3514f), 270)},
            {"The Fetid Mire", new WarpData("SEWERS", new Vector3(-55.7227f, 1f, -131.2003f), 0)},
            {"Forbidden Archives", new WarpData("ARCHIVES", new Vector3(-107.0403f, -3f, -46.6349f), 180)},
            {"Laetus Chasm", new WarpData("WALL_01", new Vector3(-57.1614f, 37.0943f, -49.7487f), 340)},
            {"Castle Le Fanu", new WarpData("CAS_1", new Vector3(-51.7807f, 5f, -76.4138f), 40)},
            {"Boiling Grotto", new WarpData("CAVE", new Vector3(-106.1413f, 9f, -133.6253f), 180)},
        };
    }

    
}