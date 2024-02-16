using System.Collections.Generic;
using BepInEx;
using UnityEngine;

namespace LunacidAP.Data
{
    public static class LunacidLocations
    {
        public class LocationData
        {
            public string APLocationName { get; set; }
            public string GameObjectName { get; set; }
            public Vector3 Position { get; set; }

            public LocationData(string apLocationName, string gameObjectName, Vector3 position)
            {
                APLocationName = apLocationName;
                GameObjectName = gameObjectName;
                Position = position;
            }
        }

        public static readonly Dictionary<string, string> AbbreviationToScene = new(){
            { "WR", "HUB_01" },
            { "HB", "PITT_A1" },
            { "FM", "SEWER_A1" },
            { "SS", "LAKE" },
            { "AT", "HAUNT" },
            { "YF", "FOREST_A1" },
            { "FC", "FOREST_B1" },
            { "FbA", "ARCHIVES" },
            { "CLF", "CAS_1" },
            { "SB", "CAS_3" },
            { "LC", "WALL_01" },
            { "GWS", "PITT_B1" },
            { "TC", "CAS_2" },
            { "AHB", "CAS_PITT" },
            { "BG", "CAVE" },
            { "ST", "CAVE" },
            { "TA", "TOWER" },
            { "TP", "PRISON" },
            { "FlA", "ARENA" },
            { "LA", "VOID" }
        };

        public static readonly Dictionary<string, List<LocationData>> APLocationData = new(){
            {
                "HUB_01",
                new(){
            new LocationData( "WR: Bench", "CRYSTAL_SHARD_PICKUP", new Vector3(2.6f, 0.8f, -22.2f)),
            new LocationData( "WR: Clive's Gift", "HEALTH_VIAL_PICKUP", new Vector3(0.0f, 0.0f, 0.0f)),
            //new LocationData( "Buy Enchanted Key", "ENKEY_PICKUP(Clone)", new Vector3(-7.0f, 1.0f, 2.9f)),
            new LocationData( "WR: Rafters", "OCEAN_ELIXIR_PICKUP", new Vector3(-1.4f, 8.5f, 16.4f))

        }
            },
            {
                "PITT_A1",
                new(){
            new LocationData( "HB: Starting Weapon", "STONE_SWORD", new Vector3(70.4f, 11.1f, -61.7f) ),
            new LocationData( "HB: Rightmost Water Room (Right)", "MANA_VIAL_PICKUP", new Vector3(-66.0f, 12.9f, 6.7f) ),
            new LocationData( "HB: Rightmost Water Room (Left)", "GHOST_LIGHT", new Vector3(-63.6f, 12.9f, -62.5f) ) ,
            new LocationData( "HB: Leftmost Water Room", "HEALTH_VIAL_PICKUP", new Vector3(-89.6f, 11.8f, -106.0f) ),
            new LocationData( "HB: Chest Near Demi", "FLAME_SPEAR", new Vector3(0.0f, 0.7f, 0.0f) ),
            new LocationData( "HB: Near Enchanted Door", "HEALTH_VIAL_PICKUP", new Vector3(37.696f, 8.183f, 47.84f) ),
            new LocationData( "HB: Dark Tunnel After Enchanted Door", "TORCH", new Vector3(-16.3f, 2.6f, 45.2f) ),
            new LocationData( "HB: Temple Fountain", "HEALTH_VIAL_PICKUP", new Vector3(43.5f, -24.6f, 114.3f) ),
            new LocationData( "HB: Temple Ritual Table", "RD_PICKUP", new Vector3(65.4f, -19.9f, 94.3f) ),
            new LocationData( "HB: Temple Altar Chest", "LITHO", new Vector3(104.6f, -26.3f, 78.2f) ),
            new LocationData( "HB: Temple Hidden Room Behind Pillar (Left)", "", new Vector3()),
            new LocationData( "HB: Temple Hidden Room behind Pillar (Right)", "", new Vector3()),
            new LocationData( "HB: Temple Ritual Table After Bridge", "FLAME_FLARE", new Vector3(24.0f, -20.8f, 39.3f) ),
            new LocationData( "HB: Temple Small Pillar Top", "HEALTH_VIAL_PICKUP", new Vector3(46.2f, -21.7f, 43.5f) ),
            new LocationData( "HB: Temple Pillar Room Left", "CRYSTAL_SHARD_PICKUP", new Vector3(-38.5f, -26.0f, 81.4f) ),
            new LocationData( "HB: Temple Pillar Room Back Left", "SHIELD_PICKUP", new Vector3(-22.5f, -26.0f, 108.5f) ),
            new LocationData( "HB: Temple Pillar Room Back Right", "BLOOD_WINE_PICKUP", new Vector3(-17.4f, -24.6f, 105.9f) ),
            new LocationData( "HB: Temple Pillar Room Hidden Room", "BLOOD_STRIKE", new Vector3(5.8f, -24.3f, 113.0f) ),
            new LocationData( "HB: Temple Hidden Room In Sewer", "VHS_PICKUP", new Vector3(0.0f, 0.0f, 0.0f) ),
            new LocationData( "HB: Temple Table in Sewer", "RUSTED_SWORD_PICKUP", new Vector3(126.2f, -23.4f, 204.3f) ),
            new LocationData( "HB: Temple Sewer Puzzle", "VHS_PICKUP", new Vector3(142.9f, -26.2f, 75.0f) ),
            new LocationData( "HB: Temple Blood Altar", "", new Vector3(0.0f, 0.0f, 0.0f) ),

                }
            }
        };
    }
}
