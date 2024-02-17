using System.Collections.Generic;
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

        public static readonly Dictionary< string, string> DropLocations = new(){
            { "SICKLE_PICKUP", "Milk Snail: Ice Sickle Drop" },
            { "SUMMON_SNAIL", "Snail: Summon Snail" },
            { "OCEAN_ELIXIR_PICKUP", "SS: Demon's Drop" },
        };
        public static readonly Dictionary<string, string> ShopLocations = new(){
            { "ENKEY_PICKUP", "Buy Enchanted Key" },
        };

        public static readonly Dictionary<string, List<LocationData>> APLocationData = new(){
            {
                "HUB_01", // Wing's Rest
                new(){
            new LocationData( "WR: Bench", "CRYSTAL_SHARD_PICKUP", new Vector3(2.6f, 0.8f, -22.2f)),
            new LocationData( "WR: Clive's Gift", "HEALTH_VIAL_PICKUP", new Vector3(14.5f, 1.3f, -1.9f)),
            new LocationData( "WR: Demi's Gift", "EARTH_ELIXIR_PICKUP", new Vector3(-4.1f, 0.4f, -11.2f)),
            new LocationData( "WR: Rafters", "OCEAN_ELIXIR_PICKUP", new Vector3(-1.4f, 8.5f, 16.4f))

        }
            },
            {
                "PITT_A1", // Hollow Basin
                new(){
            new LocationData( "HB: Starting Weapon", "STONE_SWORD", new Vector3(70.4f, 11.1f, -61.7f) ),
            new LocationData( "HB: Rightmost Water Room (Right)", "MANA_VIAL_PICKUP", new Vector3(-66.0f, 12.9f, 6.7f) ),
            new LocationData( "HB: Rightmost Water Room (Left)", "GHOST_LIGHT", new Vector3(-63.6f, 12.9f, -62.5f) ) ,
            new LocationData( "HB: Leftmost Water Room", "HEALTH_VIAL_PICKUP", new Vector3(-89.6f, 11.8f, -106.0f) ),
            new LocationData( "HB: Chest Near Demi", "FLAME_SPEAR", new Vector3(39.2f, 11.7f, -161.6f) ),
            new LocationData( "HB: Near Enchanted Door", "HEALTH_VIAL_PICKUP", new Vector3(37.696f, 8.183f, 47.84f) ),
            new LocationData( "HB: Dark Tunnel After Enchanted Door", "TORCH", new Vector3(-16.3f, 2.6f, 45.2f) ),
            new LocationData( "HB: Temple Fountain", "HEALTH_VIAL_PICKUP", new Vector3(43.5f, -24.6f, 114.3f) ),
            new LocationData( "HB: Temple Ritual Table", "RD_PICKUP", new Vector3(65.4f, -19.9f, 94.3f) ),
            new LocationData( "HB: Temple Altar Chest", "LITHO", new Vector3(104.6f, -26.3f, 78.2f) ),
            new LocationData( "HB: Temple Hidden Room Behind Pillar (Left)", "HEALTH_VIAL_PICKUP", new Vector3(76.9f, -20.9f, 14.7f)),
            new LocationData( "HB: Temple Hidden Room Behind Pillar (Right)", "HEALTH_VIAL_PICKUP", new Vector3(75.4f, -20.9f, 13.5f)),
            new LocationData( "HB: Temple Ritual Table After Bridge", "FLAME_FLARE", new Vector3(24.0f, -20.8f, 39.3f) ),
            new LocationData( "HB: Temple Small Pillar Top", "HEALTH_VIAL_PICKUP", new Vector3(46.2f, -21.7f, 43.5f) ),
            new LocationData( "HB: Temple Pillar Room Left", "CRYSTAL_SHARD_PICKUP", new Vector3(-38.5f, -26.0f, 81.4f) ),
            new LocationData( "HB: Temple Pillar Room Back Left", "SHIELD_PICKUP", new Vector3(-22.5f, -26.0f, 108.5f) ),
            new LocationData( "HB: Temple Pillar Room Back Right", "BLOOD_WINE_PICKUP", new Vector3(-17.4f, -24.6f, 105.9f) ),
            new LocationData( "HB: Temple Pillar Room Hidden Room", "BLOOD_STRIKE", new Vector3(5.8f, -24.3f, 113.0f) ),
            new LocationData( "HB: Temple Hidden Room In Sewer", "VHS_PICKUP", new Vector3(142.9f, -26.2f, 75.0f) ),
            new LocationData( "HB: Temple Table in Sewer", "RUSTED_SWORD_PICKUP", new Vector3(126.2f, -23.4f, 204.3f) ),
            new LocationData( "HB: Temple Sewer Puzzle", "", new Vector3() ),
            new LocationData( "HB: Temple Blood Altar", "COIN_PICKUP", new Vector3(117.6f, -23.2f, 70.0f) ),
            new LocationData ( "HB: Alcove on Path to Yosei Forest", "STEEL_SPEAR_PICKUP", new Vector3(49.9f, -8.5f, 12.3f) ),

                }
            },
            {
                "PITT_B1", // Great Well Surface
                new(){
                    new LocationData( "GWS: Demi's Gift", "CRYSTAL_SHARD_PICKUP", new Vector3(-10.1f, 1.6f, -4.6f))
                }
            },
            {
                "WALL_01", // Laetus Wall
                new(){
                    new LocationData( "LC: Hidden Chest", "ICE_TEAR_PICKUP", new Vector3(-50.1f, 36.7f, 5.4f)),
                    new LocationData( "LC: Invisible Path to Cliffside", "SWORD_PICKUP", new Vector3(8.3f, 65.0f, -62.7f))
                }
            },
            {
                "ARCHIVES", // Forbidden Archives
                new(){
                    new LocationData( "FbA: Back Room Past Bridge", "OCEAN_ELIXIR_PICKUP", new Vector3(-99.0f, -4.0f, -51.6f) ),
                    new LocationData( "FbA: Strange Corpse", "PICKUP", new Vector3(-85.6f, -2.8f, -5.8f) ),
                    new LocationData( "FbA: Against Wall Near Trees", "L_URN_PICKUP", new Vector3(-113.8f, -20.0f, 67.1f) ),
                    new LocationData( "FbA: Short Wall Near Trees", "HEALTH_VIAL_PICKUP", new Vector3(-110.9f, -20.0f, 46.0f) ),
                    new LocationData( "FbA: Snail Lectern (Near)", "LIGHT_REVEAL", new Vector3(-28.0f, 9.5f, 52.0f) ),
                    new LocationData( "FbA: Snail Lectern (Far)", "BLOOD_DRAIN", new Vector3(-28.0f, 9.5f, 45.0f) ),
                    new LocationData( "FbA: Rug On Balcony", "MANA_VIAL_PICKUP", new Vector3(-41.4f, 8.0f, -59.3f) ),
                    new LocationData( "FbA: Rooftops", "EARTH_ELIXIR_PICKUP", new Vector3(3.6f, 6.5f, -2.1f) ),
                    new LocationData( "FbA: Hidden Room Upper Floor", "WOLFRAM_PICKUP", new Vector3(-112.1f, 8.5f, -22.4f) ),
                    new LocationData( "FbA: Hidden Room Lower Floor", "CRYSTAL_SHARD_PICKUP", new Vector3(-57.9f, -19.9f, 38.5f) ),
                    new LocationData( "FbA: Near Twisty Tree", "FAIRY_MOSS_PICKUP", new Vector3(-7.1f, -19.6f, 44.2f) ),
                    new LocationData( "FbA: uwu", "WEPON", new Vector3(-2.1f, -18.2f, 32.1f) ),
                    new LocationData( "FbA: Daedalus Knowledge (First)", "FIRE_WORM", new Vector3()),
                    new LocationData( "FbA: Daedalus Knowledge (Second)", "BESTIAL_COMMUNION", new Vector3()),
                    new LocationData( "FbA: Daedalus Knowledge (Third)", "MOON_BEAM", new Vector3()),
                    new LocationData( "FbA: Corner Near Daedalus", "HEALTH_VIAL_PICKUP", new Vector3(-43.6f, -20.0f, -29.6f))
                }
            },
            {
                "FOREST_A1", // Yosei Forest
                new(){
                    new LocationData( "YF: Barrel Group", "HEALTH_VIAL_PICKUP", new Vector3(26.2f, 36.0f, 15.5f)),
                    new LocationData( "YF: Blood Pool", "BLOOD_DRAIN_PICKUP", new Vector3(-35.6f, 30.1f, -56.4f)),
                    new LocationData( "YF: Banches Within Tree", "HEAL", new Vector3(-11.2f, 15.3f, 62.5f)),
                    new LocationData( "YF: Chest Near Tree", "ELF_BOW_PICKUP", new Vector3(-68.2f, 0.5f, -0.2f)),
                    new LocationData( "YF: Blood Plant's Insides", "HEALTH_VIAL_PICKUP", new Vector3(-20.5f, 3.8f, -34.6f)),
                    new LocationData( "YF: Hanging In The Trees", "SWORD_PICKUP", new Vector3(-209.8f, 26.5f, -41.9f)),
                    new LocationData( "YF: Hidden Chest", "FLAME_FLARE", new Vector3(-259.8f, -20.3f, -176.3f)),
                    new LocationData( "YF: Room Defended by Blood Plant", "EARTH_STRIKE", new Vector3(-274.7f, -32.4f, -176.2f)),
                    new LocationData( "YF: Patchouli's Canopy Offer", "ENKEY_PICKUP", new Vector3(-57.9f, -16.8f, -116.1f)),
                    new LocationData( "YF: Patchouli's Reward", "EARTH_ELIXIR_PICKUP", new Vector3()),
                }
            },
            {
                "FOREST_B1", // Forest Canopy
                new(){
                    new LocationData( "FC: Branch Lower Edge", "CRYSTAL_SHARD_PICKUP", new Vector3(-48.6f, -2.2f, -5.1f)),
                    new LocationData( "FC: Branch Cave", "FAIRY_MOSS_PICKUP", new Vector3(-48.6f, 0.0f, -56.7f)),
                    new LocationData( "FC: Chest", "DARK_SKULL_PICKUP", new Vector3(-150.4f, 15.7f, 40.4f)),
                    new LocationData( "FC: Wooden Statue (Josiah)", "SKULL_PICKUP", new Vector3(-21.4f, 24.7f, 11.8f)),
                    new LocationData( "FC: Wooden Statue (Sitting)", "WIND", new Vector3(-141.8f, 39.0f, -23.9f)),
                }
            },
            {
                "SEWER_A1", // The Fetid Mire
                new(){
                    new LocationData( "FM: Room Left of Foyer", "ANTIDOTE_PICKUP", new Vector3(39.2f, 0.0f, -90.6f) ),
                    new LocationData( "FM: Hidden Slimey Chest Near Entrance", "FLAME_SPEAR", new Vector3(12.0f, 0.7f, -158.3f) ),
                    new LocationData( "FM: Hidden Upper Overlook (Left)", "CRYSTAL_SHARD_PICKUP", new Vector3(124.1f, -7.9f, -425.4f) ),
                    new LocationData( "FM: Hidden Upper Overlook (Right)", "OCEAN_ELIXIR_PICKUP", new Vector3(110.1f, -8.0f, -470.0f) ),
                    new LocationData( "FM: Bonerard's Trash", "ANTIDOTE_PICKUP", new Vector3(7.4f, -8.0f, -397.7f) ),
                    new LocationData( "FM: Rubble Near Overlook Bridge", "ANTIDOTE_PICKUP", new Vector3(-43.4f, -8.0f, -366.6f) ),
                    new LocationData( "FM: Slime Skeleton Chest", "BARRIER", new Vector3(-46.0f, 0.8f, -192.8f) ),
                    new LocationData( "FM: Jellisha's Trash", "MANA_VIAL_PICKUP", new Vector3(98.9f, -28.0f, -309.5f) ),
                    new LocationData( "FM: Jellisha's Quest Reward", "SLIME_ORB_PICKUP", new Vector3(83.6f, -27.0f, -305.9f) ),
                    new LocationData( "FM: Path to Sanguine Sea (Left)", "AXE_PICKUP", new Vector3(116.1f, -27.5f, -533.9f)),
                    new LocationData( "FM: Path to Sanguine Sea (Right)", "HEALTH_VIAL_PICKUP", new Vector3(73.9f, -26.7f, -537.1f)),
                    new LocationData( "FM: Hidden Chest Near Underworks", "EARTH_ELIXIR_PICKUP", new Vector3(24.2f, -26.8f, 421.9f) ),
                    new LocationData( "FM: Rubble Near Illusory Wall", "FLAME_FLARE", new Vector3(18.1f, -27.3f, -443.3f) ),
                    new LocationData( "FM: Underwater Pipe", "P_THROWING_KNIFE_PICKUP", new Vector3(64.9f, -31.6f, -279.5f) ),
                    new LocationData( "FM: Underworks Waterfall", "ANTIDOTE_PICKUP", new Vector3(-9.6f, -28.0f, -260.5f) ),
                    new LocationData( "FM: Underworks Skeleton", "HILT_PICKUP", new Vector3(-60.4f, -28.0f, -172.0f) ),
                }
            },
            {
                "LAKE", // Sanguine Sea
                new(){
                    new LocationData(  "SS: Pillar In Front of Castle Le Fanu", "", new Vector3() ),
                    new LocationData( "SS: Underblood Near Castle Le Fanu", "", new Vector3() ),
                    new LocationData( "SS: Blood Island", "", new Vector3() ),
                    new LocationData( "SS: Killing the Jotunn", "", new Vector3() ),
                }
            },
            {
                "HAUNT", // Accursed Tomb
                new(){
                    new LocationData( "AT: Catacombs Coffins Near Stairs", "", new Vector3() ),
                    new LocationData( "AT: Catacombs Coffins With Blue Light", "", new Vector3() ),
                    new LocationData( "AT: Corrupted Room", "", new Vector3() ),
                    new LocationData( "AT: Sealed Coffin Room from Catacombs", "", new Vector3() ),
                    new LocationData( "AT: Catacombs Hidden Room", "", new Vector3() ),
                    new LocationData( "AT: Deep Coffin Storage", "", new Vector3() ),
                    new LocationData( "AT: Red Skeleton", "", new Vector3() ),

                    new LocationData( "AT: Mausoleum Hidden Chest", "", new Vector3() ),
                    new LocationData( "AT: Mausoleum Upper Alcove Table", "", new Vector3() ),
                    new LocationData( "AT: Mausoleum Maze (Early)", "", new Vector3() ),
                    new LocationData( "AT: Mausoleum Maze (Middle)", "", new Vector3() ),
                    new LocationData( "AT: Mausoleum Central Room (Right)", "", new Vector3() ),
                    new LocationData( "AT: Mausoleum Central Room (Left)", "", new Vector3() ),
                    new LocationData( "AT: Mausoleum Central Room (Back)", "", new Vector3() ),
                    new LocationData( "AT: Mausoleum Central Room (Left Path)", "", new Vector3() ),
                    new LocationData( "AT: Mausoleum Central Room (Right Path)", "", new Vector3() ),

                    new LocationData( "AT: Tomb With Switch", "", new Vector3() ),
                    new LocationData( "AT: Tomb With Sitting Corpse", "", new Vector3() ),
                    new LocationData( "AT: Demi Chest", "", new Vector3() ),
                    new LocationData( "AT: Near Light Switch", "", new Vector3() ),
                    new LocationData( "AT: Hidden Room in Tomb", "", new Vector3() ),
                    new LocationData( "AT: Hidden Chest in Tomb", "", new Vector3() ),
                }
            },
            {
                "CAS_1", // Castle Le Fanu
                new(){
                    new LocationData( "CLF: Outside Corner", "", new Vector3() ),
                    new LocationData( "CLF: Cattle Cell (South)", "", new Vector3() ),
                    new LocationData( "CLF: Cattle Cell (West)", "", new Vector3() ),
                    new LocationData( "CLF: Cattle Cell (Center)", "", new Vector3() ),
                    new LocationData( "CLF: Cattle Cell (North)", "", new Vector3() ),
                    new LocationData( "CLF: Hidden Cattle Cell", "", new Vector3() ),

                    new LocationData( "CLF: Hallway Rubble Room", "", new Vector3() ),
                    new LocationData( "CLF: Hallway Dining Room", "", new Vector3() ),
                    new LocationData( "CLF: Garrat Resting Room (Left)", "", new Vector3() ),
                    new LocationData( "CLF: Garrat Resting Room (Back)", "", new Vector3() ),
                    new LocationData( "CLF: Hallway Dead End Before Blue Doors", "", new Vector3() ),
                    new LocationData( "CLF: Upper Floor Coffin Room (Small Room)", "", new Vector3() ),
                    new LocationData( "CLF: Upper Floor Coffin Room (Large Room)", "", new Vector3() ),
                    new LocationData( "CLF: Upper Floor Coffin Room (Double)", "", new Vector3() ),
                    new LocationData( "CLF:  Upper Floor Coffin Room (Halllway)", "", new Vector3() ),
                }
            },
            {
                "CAS_2", // Sealed Ballroom
                new(){
                    new LocationData( "SB: Entry Small Room Lounge", "", new Vector3() ),
                    new LocationData( "SB: Entry Hidden Couch Top", "", new Vector3() ),
                    new LocationData( "SB: Entry Hidden Couch Bottom", "", new Vector3() ),
                    new LocationData( "SB: Entry Hidden Cave in a Lounge", "", new Vector3() ),
                    new LocationData( "SB: Entry Lounge Long Table", "", new Vector3() ),
                    new LocationData( "SB: Side Hidden Cave", "", new Vector3() ),
                    new LocationData( "SB: Side Chest Near Switch", "", new Vector3() ),
                    new LocationData( "SB: Side Painting Viewing Room", "", new Vector3() ),
                    new LocationData( "SB: Side Hidden Casket Room", "", new Vector3() ),
                    new LocationData( "SB: Side XP Drain Party Room", "", new Vector3() ),
                }
            },
            {
                "CAS_PITT", // A Holy Battlefield
                new(){
                    new LocationData( "AHB: Sngula Umbra's Remains", "", new Vector3() )
                }
            }

        };
    }
}
