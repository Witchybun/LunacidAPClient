using System.Collections.Generic;
using UnityEngine;

namespace LunacidAP.Data
{
    public static class LunacidLocations
    {
        public class LocationData
        {
            public long APLocationID { get; set; }
            public string APLocationName { get; set; }
            public string GameObjectName { get; set; }
            public Vector3 Position { get; set; }
            public bool IgnoreLocationHandler { get; set; }

            public LocationData(long locationID, string apLocationName, string gameObjectName = "", Vector3 position = new Vector3(), bool ignoreLocationHandler = false)
            {
                APLocationID = ArchipelagoClient.LOCATION_INIT_ID + locationID;
                APLocationName = apLocationName;
                GameObjectName = gameObjectName;
                Position = position;
                IgnoreLocationHandler = ignoreLocationHandler;
            }

            public LocationData()
            {
                APLocationID = -1;
                APLocationName = null;
                GameObjectName = null;
                Position = new Vector3();
                IgnoreLocationHandler = true;
            }
        }

        public static readonly List<LocationData> UniqueDropLocations = new(){
            new LocationData( 401, "Snail: Summon Snail Drop", "SUMMON_SNAIL" ),
            new LocationData( 402, "Mummy Knight: Rusted Sword Drop", "RUSTED_SWORD_PICKUP" ),
            new LocationData( 403, "Kodama: Summon Kodama Drop", "SUMMON_KODAMA" ),
            new LocationData( 404, "Chimera: Quick Stride Drop", "QUICK_STRIDE" ),
            new LocationData( 405, "Milk Snail: Ice Sickle Drop", "SICKLE_PICKUP" ),
            new LocationData( 406, "Skeleton: Skeleton Axe Drop", "SKELE_AXE_PICKUP" ),
            new LocationData( 407, "Skeleton: Dark Skull Drop", "DARK_SKULL_PICKUP" ),
            new LocationData( 408, "Phantom: Cursed Blade Drop", "CURSED_BLADE_PICKUP" ),
            new LocationData( 409, "Obsidian Skeleton: Obsidian Cursebrand Drop", "OBS_SWORD_PICKUP" ),
            new LocationData( 410, "Obsidian Skeleton: Obsidian Poisonguard Drop", "OBS_SHIELD_PICKUP" ),
            new LocationData( 411, "Anpu: Golden Khopesh Drop", "GOLDEN_KOPESH_PICKUP" ),
            new LocationData( 412, "Anpu: Golden Sickle Drop", "GOLDEN_SICKLE_PICKUP" ),
            new LocationData( 413, "Malformed Horse: Brittle Arming Sword Drop", "BRITTLE_PICKUP" ),
            new LocationData( 414, "Jailor: Jailor's Candle Drop", "CANDLE_WEP_PICKUP" ),
            new LocationData( 415, "Vampire Page: Lyrian Longsword Drop", "VP_SWORD_PICKUP" ),
            new LocationData( 416, "Sucsarian: Sucsarian Spear Drop", "SUS_SPEAR_PICKUP" ),
            new LocationData( 417, "Sucsarian: Sucsarian Dagger Drop", "SUS_DAGGER_PICKUP" ),
            new LocationData( 418, "Giant Skeleton: Dark Skull Drop", "DARK_SKULL_PICKUP" ),
            new LocationData( 419, "Cetea: Tornado Drop", "TORNADO_PICKUP" ),
            new LocationData( 420, "Abyssal Demon: Ocean Elixir Drop", "OCEAN_ELIXIR_PICKUP" ),
            new LocationData( 421, "Lupine Skeleton: Dark Skull Drop", "DARK_SKULL_PICKUP" ),
        };

        public static readonly List<LocationData> OtherDropLocations = new(){
            new LocationData( 451, "Snail: Small Silver Drop" ),
            new LocationData( 452, "Snail: Large Silver Drop" ),
            new LocationData( 453, "Snail: Ocean Bone Shard Drop" ),
            new LocationData( 454, "Milk Snail: Small Silver Drop" ),
            new LocationData( 455, "Milk Snail: Large Silver Drop" ),
            new LocationData( 456, "Milk Snail: Ocean Bone Shard Drop" ),
            new LocationData( 457, "Shulker: Obsidian Drop" ),
            new LocationData( 458, "Shulker: Onyx Drop" ),
            new LocationData( 459, "Mummy: Mana Vial Drop" ),
            new LocationData( 460, "Mummy: Onyx Drop" ),
            new LocationData( 461, "Mummy: Small Silver Drop" ),
            new LocationData( 462, "Mummy: Large Silver Drop" ),
            new LocationData( 463, "Necronomicon: Fire Opal Drop" ),
            new LocationData( 464, "Necronomicon: Medium Silver Drop" ),
            new LocationData( 465, "Necronomicon: Large Silver Drop" ),
            new LocationData( 466, "Necronomicon: Mana Vial Drop" ),
            new LocationData( 467, "Chimera: Light Urn Drop" ),
            new LocationData( 468, "Chimera: Holy Water Drop" ),
            new LocationData( 469, "Enlightened One: Mana Vial Drop" ),
            new LocationData( 470, "Enlightened One: Ocean Bone Shell Drop" ),
            new LocationData( 471, "Slime Skeleton: Ashes Drop" ),
            new LocationData( 472, "Skeleton: Large Silver Drop" ),
            new LocationData( 473, "Skeleton: Mana Vial Drop" ),
            new LocationData( 474, "Skeleton: Onyx Drop" ),
            new LocationData( 475, "Skeleton: Bones Drop" ),
            new LocationData( 476, "Rat King: Medium Silver Drop" ),
            new LocationData( 477, "Rat King: Lotus Seed Pod Drop" ),
            new LocationData( 478, "Rat: Small Silver Drop" ),
            new LocationData( 479, "Kodama: Small Silver Drop" ),
            new LocationData( 480, "Kodama: Medium Silver Drop" ),
            new LocationData( 481, "Kodama: Opal Drop" ),
            new LocationData( 482, "Yakul: Medium Silver Drop" ),
            new LocationData( 483, "Yakul: Fire Opal Drop" ),
            new LocationData( 484, "Yakul: Opal Drop" ),
            new LocationData( 485, "Yakul: Health Vial Drop" ),
            new LocationData( 486, "Venus: Medium Silver Drop" ),
            new LocationData( 487, "Venus: Yellow Morel Drop" ),
            new LocationData( 488, "Venus: Destroying Angel Mushroom Drop" ),
            new LocationData( 489, "Neptune: Medium Silver Drop" ),
            new LocationData( 490, "Neptune: Yellow Morel Drop" ),
            new LocationData( 491, "Neptune: Destroying Angel Mushroom Drop" ),
            new LocationData( 492, "Unilateralis: Medium Silver Drop" ),
            new LocationData( 493, "Unilateralis: Yellow Morel Drop" ),
            new LocationData( 494, "Unilateralis: Destroying Angel Mushroom Drop" ),
            new LocationData( 495, "Hemalith: Health Vial Drop" ),
            new LocationData( 496, "Hemalith: Pink Shrimp Drop" ),
            new LocationData( 497, "Hemalith: Bloodweed Drop" ),
            new LocationData( 498, "Mi-Go: Ocean Bone Shell Drop" ),
            new LocationData( 499, "Mi-Go: Medium Silver Drop" ),
            new LocationData( 500, "Mi-Go: Snowflake Obsidian" ),
            new LocationData( 501, "Mare: Medium Silver Drop" ),
            new LocationData( 502, "Mare: Obsidian Drop" ),
            new LocationData( 503, "Mare: Onyx Drop" ),
            new LocationData( 504, "Cursed Painting: Fire Opal Drop" ),
            new LocationData( 505, "Cursed Painting: Medium Silver Drop" ),
            new LocationData( 506, "Cursed Painting: Mana Vial Pickup" ),
            new LocationData( 507, "Cursed Painting: Large Silver Drop" ),
            new LocationData( 508, "Phantom: Medium Silver Drop" ),
            new LocationData( 509, "Phantom: Holy Water Drop" ),
            new LocationData( 510, "Phantom: Moonlight Vial Drop" ),
            new LocationData( 511, "Phantom: Ectoplasm Drop" ),
            new LocationData( 512, "Vampire: Large Silver Drop" ),
            new LocationData( 513, "Vampire: Vampiric Ashes Drop" ),
            new LocationData( 514, "Vampire: Cloth Bandage Drop" ),
            new LocationData( 515, "Vampire Page: Vampiric Ashes Drop" ),
            new LocationData( 516, "Vampire Page: Large Silver Drop" ),
            new LocationData( 517, "Malformed: Vampiric Ashes Drop" ),
            new LocationData( 518, "Great Bat: Health Vial Drop" ),
            new LocationData( 519, "Great Bat: Obsidian Drop" ),
            new LocationData( 520, "Great Bat: Large Silver Drop" ),
            new LocationData( 521, "Poltergeist: Large Silver Drop" ),
            new LocationData( 522, "Poltergeist: Ectoplasm Drop" ),
            new LocationData( 523, "Malformed Horse: Large Silver Drop" ),
            new LocationData( 524, "Malformed Horse: Mana Vial Drop" ),
            new LocationData( 525, "Hallowed Husk: Large Silver Drop" ),
            new LocationData( 526, "Hallowed Husk: Bones Drop" ),
            new LocationData( 527, "Hallowed Husk: Cloth Bandage Drop" ),
            new LocationData( 528, "Hallowed Husk: Light Urn Drop" ),
            new LocationData( 529, "Hallowed Husk: Fool's Gold Drop" ),
            new LocationData( 530, "Hallowed Husk: Holy Water Drop" ),
            new LocationData( 531, "Ikurr'ilb: Ikurr'ilb Root Drop" ),
            new LocationData( 532, "Ikurr'ilb: Medium Silver Drop" ),
            new LocationData( 533, "Ikurr'ilb: Snowflake Obsidian Drop" ),
            new LocationData( 534, "Mimic: Moonlight Vial Drop" ),
            new LocationData( 535, "Mimic: Obsidian Drop" ),
            new LocationData( 536, "Mimic: Fool's Gold Drop" ),
            new LocationData( 537, "Obsidian Skeleton: Large Silver Drop" ),
            new LocationData( 538, "Obsidian Skeleton: Bones Drop" ),
            new LocationData( 539, "Obsidian Skeleton: Mana Vial Drop" ),
            new LocationData( 540, "Obsidian Skeleton: Obsidian Drop" ),
            new LocationData( 541, "Anpu: Large Silver Drop" ),
            new LocationData( 542, "Anpu: Fire Opal Drop" ),
            new LocationData( 543, "Serpent: Antidote Drop" ),
            new LocationData( 544, "Serpent: Small Silver Drop" ),
            new LocationData( 545, "Embalmed: Cloth Bandage Drop" ),
            new LocationData( 546, "Embalmed: Ashes Drop" ),
            new LocationData( 547, "Embalmed: Bones Drop" ),
            new LocationData( 548, "Jailor: Large Silver Drop" ),
            new LocationData( 549, "Jailor: Spectral Candle Drop" ),
            new LocationData( 550, "Jailor: Cloth Bandage Drop" ),
            new LocationData( 551, "Jailor: Health Vial Drop" ),
            new LocationData( 552, "Jailor: Angel's Feather Drop" ),
            new LocationData( 553, "Cerritulus Lunam: Ectoplasm Drop" ),
            new LocationData( 554, "Cerritulus Lunam: Medium Silver Drop" ),
            new LocationData( 555, "Cerritulus Lunam: Snowflake Obsidian Drop" ),
            new LocationData( 556, "Giant Skeleton: Dark Urn Drop" ),
            new LocationData( 557, "Giant Skeleton: Bones Drop" ),
            new LocationData( 558, "Giant Skeleton: Mana Vial Drop" ),
            new LocationData( 559, "Giant Skeleton: Onyx Drop" ),
            new LocationData( 560, "Sucsarian: Large Silver Drop" ),
            new LocationData( 561, "Sucsarian: Obsidian Drop" ),
            new LocationData( 562, "Sucsarian: Snowflake Obsidian Drop" ),
            new LocationData( 563, "Sucsarian: Throwing Knife Drop" ),
            new LocationData( 564, "Vesta: Fairy Moss Drop" ),
            new LocationData( 565, "Vesta: Yellow Morel Drop" ),
            new LocationData( 566, "Vesta: Destroying Angel Mushroom Drop" ),
            new LocationData( 567, "Ceres: Fairy Moss Drop" ),
            new LocationData( 568, "Ceres: Yellow Morel Drop" ),
            new LocationData( 569, "Ceres: Destroying Angel Mushroom Drop" ),
            new LocationData( 570, "Gloom Wood: Fairy Moss Drop" ),
            new LocationData( 571, "Gloom Wood: Health Vial Drop" ),
            new LocationData( 572, "Gloom Wood: Mana Vial Drop" ),
            new LocationData( 573, "Cetea: Medium Silver Drop" ),
            new LocationData( 574, "Cetea: Ocean Bone Shell Drop" ),
            new LocationData( 575, "Mummy Knight: Onyx Drop" ),
            new LocationData( 576, "Mummy Knight: Large Silver Drop" ),
            new LocationData( 577, "Mummy Knight: Small Silver Drop" ),
            new LocationData( 578, "Sanguis Umbra: Black Book Drop" ),
            new LocationData( 579, "Lupine Skeleton: Medium Silver Drop" ),
            new LocationData( 580, "Lupine Skeleton: Onyx Drop" ),
            new LocationData( 581, "Lupine Skeleton: Bones Drop" ),
            new LocationData( 582, "Infested Corpse: Bones Drop" ),
            new LocationData( 583, "Infested Corpse: Antidote Drop" ),
        };

        public static readonly List<LocationData> ShopLocations = new(){
            new LocationData( 351, "Buy Rapier", "RAPIER_PICKUP" ),
            new LocationData( 352, "Buy Crossbow", "CROSSBOW_PICKUP" ),
            new LocationData( 353, "Buy Oil Lantern", "LANT_PICKUP" ),
            new LocationData( 354, "Buy Enchanted Key", "ENKEY_PICKUP" ),
            new LocationData( 355, "Buy Jotunn Slayer", "SLAYER_PICKUP" ),
            new LocationData( 356, "Buy Privateer Musket", "MUSKET_PICKUP" ),
            new LocationData( 357, "Buy Steel Needle", "STEELNEEDLE_PICKUP" ),
            new LocationData( 359, "Buy Ocean Elixir (Patchouli)", "OCEAN_ELIXIR_PICKUP" ),
        };

        public static readonly List<string> InitialVoucherItems = new(){
            "Enchanted Key", "RAPIER", "STEEL NEEDLE", "CROSSBOW"
        };

        public static readonly List<string> GoldenVoucherItems = new(){
            "Oil Lantern", "PRIVATEER MUSKET"
        };

        public static readonly List<string> DreamerVoucherItems = new(){
            "Ocean Elixir", "JOTUNN SLAYER"
        };

        public static readonly List<LocationData> QuenchLocation = new(){
            new LocationData(601, "Quench Steel Claw", "STEEL CLAW"),
            new LocationData(602, "Quench Iron Claw", "IRON CLAW"),
            new LocationData(603, "Quench Iron Club", "IRON CLUB"),
            new LocationData(604, "Quench Stone Club", "STONE CLUB"),
            new LocationData(605, "Quench Torch", "TORCH"),
            new LocationData(606, "Quench Brittle Arming Sword", "BRITTLE ARMING SWORD"),
            new LocationData(607, "Quench Broken Hilt", "BROKEN HILT"),
            new LocationData(608, "Quench Broken Lance", "BROKEN LANCE"),
            new LocationData(609, "Quench Crossbow", "CROSSBOW"),
            new LocationData(610, "Quench Elfen Sword", "ELFEN SWORD"),
            new LocationData(611, "Quench Lyrian Longsword", "LYRIAN LONGSWORD"),
            new LocationData(612, "Quench Obsidian Seal", "OBSIDIAN SEAL"),
            new LocationData(613, "Quench Obsidian Cursebrand", "OBSIDIAN CURSEBRAND"),
            new LocationData(614, "Quench Obsidian Poisonguard", "OBSIDIAN POISONGUARD"),
            new LocationData(615, "Quench Rapier", "RAPIER"),
            new LocationData(616, "Quench Replica Sword", "REPLICA SWORD"),
            new LocationData(617, "Quench Rusted Sword", "RUSTED SWORD"),
            new LocationData(618, "Quench Shadow Blade", "SHADOW BLADE"),
            new LocationData(619, "Quench Shining Blade", "SHINING BLADE"),
            new LocationData(620, "Quench Death Scythe", "DEATH SCYTHE"),
        };

        public static readonly List<LocationData> AlkiLocation = new(){
            new LocationData(651, "Alchemize Concentrated Lunacy", "Moonlight Vial"),
            new LocationData(652, "Alchemize Hostility Barrier", "Spectral Candle"),
            new LocationData(653, "Alchemize Explosives", "Bomb"),
            new LocationData(654, "Alchemize Venomous Object", "Poison Throwing Knife"),
            new LocationData(655, "Alchemize Defense Construct", "Staff of Osiris"),
            new LocationData(656, "Alchemize Concentrated Poison", "Poison Urn"),
            new LocationData(657, "Alchemize Nature", "Fairy Moss"),
            new LocationData(658, "Alchemize Antivenom", "Antidote"),
            new LocationData(659, "Alchemize Cleromancy Tool", "Survey Banner"),
            new LocationData(660, "Alchemize Healing Remedy", "Health Vial"),
            new LocationData(661, "Alchemize Water of Life", "Holy Water"),
            new LocationData(662, "Alchemize Sharp Object", "Throwing Knives"),
            new LocationData(663, "Alchemize Golden Sin of Abdul", "Limbo"),
            new LocationData(664, "Alchemize Mana Remedy", "Mana Vial"),
            new LocationData(665, "Alchemize Unstable Stone", "Crystal Shard"),
            new LocationData(666, "Alchemize Simple Life", "Wisp Heart"),
        };

        public static readonly Dictionary<string, List<LocationData>> APLocationData = new(){
            {
                "HUB_01", // Wing's Rest
                new(){
            new LocationData( 1, "WR: Rafters", "OCEAN_ELIXIR_PICKUP", new Vector3(-1.4f, 8.5f, 16.4f) ),
            new LocationData( 2, "WR: Bench", "CRYSTAL_SHARD_PICKUP", new Vector3(2.6f, 0.8f, -22.2f) ),
            new LocationData( 3, "WR: Clive's Gift", "HEALTH_VIAL_PICKUP", new Vector3(14.5f, 1.3f, -1.9f) ),
            new LocationData( 4, "WR: Demi's Victory Gift", "VHS_PICKUP", new Vector3(-4.1f, 1.3f, -11.2f) ),
            new LocationData( 5, "WR: Demi's Introduction Gift", position: new Vector3(-4.1f, 1.3f, -11.2f)),
            new LocationData( 6, "WR: Christmas Present", position: new Vector3(-8.465f, 0.557f, -5.413f)),

        }
            },
            {
                "PITT_A1", // Hollow Basin
                new(){
            new LocationData( 9, "HB: Starting Weapon", "STONE_SWORD", new Vector3(70.4f, 11.1f, -61.7f) ),
            new LocationData( 10, "HB: Rightmost Water Room (Right)", "MANA_VIAL_PICKUP", new Vector3(-66.0f, 12.9f, 6.7f) ),
            new LocationData( 11, "HB: Rightmost Water Room (Left)", "GHOST_LIGHT", new Vector3(-63.6f, 12.9f, -62.5f) ) ,
            new LocationData( 12, "HB: Leftmost Water Room", "HEALTH_VIAL_PICKUP", new Vector3(-89.6f, 11.8f, -106.0f) ),
            new LocationData( 13, "HB: Chest Near Demi", "FLAME_SPEAR", new Vector3(39.2f, 11.7f, -161.6f) ),
            new LocationData( 14, "HB: Near Enchanted Door", "HEALTH_VIAL_PICKUP", new Vector3(37.696f, 8.183f, 47.84f) ),
            new LocationData( 15, "HB: Dark Tunnel After Enchanted Door", "TORCH", new Vector3(-16.3f, 2.6f, 45.2f) ),
            new LocationData( 16, "HB: Temple Fountain", "HEALTH_VIAL_PICKUP", new Vector3(43.5f, -24.6f, 114.3f) ),
            new LocationData( 17, "HB: Temple Ritual Table", "RD_PICKUP", new Vector3(65.4f, -19.9f, 94.3f) ),
            new LocationData( 18, "HB: Temple Altar Chest", "LITHO", new Vector3(104.6f, -26.3f, 78.2f) ),
            new LocationData( 19, "HB: Temple Hidden Room Behind Pillar (Left)", "HEALTH_VIAL_PICKUP", new Vector3(76.9f, -20.9f, 14.7f)),
            new LocationData( 20, "HB: Temple Hidden Room Behind Pillar (Right)", "HEALTH_VIAL_PICKUP", new Vector3(75.4f, -20.9f, 13.5f)),
            new LocationData( 21, "HB: Temple Ritual Table After Bridge", "FLAME_FLARE", new Vector3(24.0f, -20.8f, 39.3f) ),
            new LocationData( 22, "HB: Temple Small Pillar Top", "HEALTH_VIAL_PICKUP", new Vector3(46.2f, -21.7f, 43.5f) ),
            new LocationData( 23, "HB: Temple Pillar Room Left", "CRYSTAL_SHARD_PICKUP", new Vector3(-38.5f, -26.0f, 81.4f) ),
            new LocationData( 24, "HB: Temple Pillar Room Back Left", "SHIELD_PICKUP", new Vector3(-22.5f, -26.0f, 108.5f) ),
            new LocationData( 25, "HB: Temple Pillar Room Back Right", "BLOOD_WINE_PICKUP", new Vector3(-17.4f, -24.6f, 105.9f) ),
            new LocationData( 26, "HB: Temple Pillar Room Hidden Room", "BLOOD_STRIKE", new Vector3(5.8f, -24.3f, 113.0f) ),
            new LocationData( 27, "HB: Temple Hidden Room In Sewer", "VHS_PICKUP", new Vector3(142.9f, -26.2f, 75.0f) ),
            new LocationData( 28, "HB: Temple Table in Sewer", "RUSTED_SWORD_PICKUP", new Vector3(126.2f, -23.4f, 204.3f) ),
            new LocationData( 29, "HB: Temple Sewer Puzzle", "ENKEY_PICKUP" ),
            new LocationData( 30, "HB: Temple Blood Altar", "COIN_PICKUP", new Vector3(117.6f, -23.2f, 70.0f) ),
            new LocationData( 31, "HB: Alcove on Path to Yosei Forest", "STEEL_SPEAR_PICKUP", new Vector3(49.9f, -8.5f, 12.3f) ),

                }
            },
            {
                "SEWER_A1", // The Fetid Mire
                new(){
                    new LocationData( 37, "FM: Room Left of Foyer", "ANTIDOTE_PICKUP", new Vector3(39.2f, 0.0f, -90.6f) ),
                    new LocationData( 38, "FM: Hidden Slimey Chest Near Entrance", "FLAME_SPEAR", new Vector3(12.0f, 0.7f, -158.3f) ),
                    new LocationData( 39, "FM: Hidden Upper Overlook (Left)", "CRYSTAL_SHARD_PICKUP", new Vector3(124.1f, -7.9f, -425.4f) ),
                    new LocationData( 40, "FM: Hidden Upper Overlook (Right)", "OCEAN_ELIXIR_PICKUP", new Vector3(110.1f, -8.0f, -470.0f) ),
                    new LocationData( 41, "FM: Bonenard's Trash", "ANTIDOTE_PICKUP", new Vector3(7.4f, -8.0f, -397.7f) ),
                    new LocationData( 42, "FM: Rubble Near Overlook Bridge", "ANTIDOTE_PICKUP", new Vector3(-43.4f, -8.0f, -366.6f) ),
                    new LocationData( 43, "FM: Slime Skeleton Chest", "BARRIER", new Vector3(-46.0f, 0.8f, -192.8f) ),
                    new LocationData( 44, "FM: Jellisha's Trash", "MANA_VIAL_PICKUP", new Vector3(98.9f, -28.0f, -309.5f) ),
                    new LocationData( 45, "FM: Jellisha's Quest Reward", "SLIME_ORB_PICKUP", new Vector3(83.6f, -27.0f, -305.9f) ),
                    new LocationData( 46, "FM: Path to Sanguine Sea (Left)", "AXE_PICKUP", new Vector3(116.1f, -27.5f, -533.9f)),
                    new LocationData( 47, "FM: Path to Sanguine Sea (Right)", "HEALTH_VIAL_PICKUP", new Vector3(73.9f, -26.7f, -537.1f)),
                    new LocationData( 48, "FM: Hidden Chest Near Underworks", "EARTH_ELIXIR_PICKUP", new Vector3(24.2f, -26.8f, -421.9f) ),
                    new LocationData( 49, "FM: Rubble Near Illusory Wall", "FLAME_FLARE", new Vector3(18.1f, -27.3f, -443.3f) ),
                    new LocationData( 50, "FM: Underwater Pipe", "P_THROWING_KNIFE_PICKUP", new Vector3(64.9f, -31.6f, -279.5f) ),
                    new LocationData( 51, "FM: Underworks Waterfall", "ANTIDOTE_PICKUP", new Vector3(-9.6f, -28.0f, -260.5f) ),
                    new LocationData( 52, "FM: Underworks Skeleton", "HILT_PICKUP", new Vector3(-60.4f, -28.0f, -172.0f) ),
                }
            },
            {
                "LAKE", // Sacrosanct Sea
                new(){
                    new LocationData( 58, "SS: Pillar In Front of Castle Le Fanu", "DAGGER_PICKUP", new Vector3(11.8f, 13.9f, 182.6f) ),
                    new LocationData( 59, "SS: Underblood Near Castle Le Fanu", "RAPIER_PICKUP", new Vector3(-4.2f, -9.2f, 270.0f) ),
                    new LocationData( 60, "SS: Fairy Circle", "FAIRY", new Vector3(-258.1f, 2.6f, -253.7f) ),
                    new LocationData( 61, "SS: Killing the Jotunn", "COIN_PICKUP" ), // no position, but this works
                }
            },
            {
                "HAUNT", // Accursed Tomb
                new(){
                    new LocationData( 67, "AT: Catacombs Coffins Near Stairs", "HEALTH_VIAL_PICKUP", new Vector3(12.4f, -25.7f, 54.6f) ),
                    new LocationData( 68, "AT: Catacombs Coffins With Blue Light", "COFFIN", new Vector3(84.8f, -25.5f, 6.5f) ),
                    new LocationData( 69, "AT: Corrupted Room", "VHS_PICKUP", new Vector3(171.9f, -18.5f, -47.6f) ),
                    new LocationData( 70, "AT: Gated Tomb Near Corrupted Room", "OCEAN_ELIXIR_PICKUP", new Vector3(190.8f, -10.4f, 4.9f) ),
                    new LocationData( 71, "AT: Catacombs Hidden Room", "OCEAN_ELIXIR_PICKUP", new Vector3(79.7f, -24.0f, -57.1f) ),
                    new LocationData( 72, "AT: Deep Coffin Storage", "HALBERD_PICKUP", new Vector3(68.0f, -25.7f, -30.1f) ),
                    new LocationData( 73, "AT: Red Skeleton", "MAGIC_SWORD_PICKUP", new Vector3(86.5f, -24.0f, -37.4f) ),
                    new LocationData( 74, "AT: Mausoleum Hidden Chest", "PICKUP", new Vector3(104.0f, -24.7f, 191.4f) ),
                    new LocationData( 75, "AT: Mausoleum Upper Alcove Table", "BOOK_PICKUP", new Vector3(163.8f, -17.0f, -104.7f) ),
                    new LocationData( 76, "AT: Mausoleum Maze (Early)", "HOLY_WATER_PICKUP", new Vector3(156.8f, -20.0f, -168.5f) ),
                    new LocationData( 77, "AT: Mausoleum Maze (Middle)", "HEALTH_VIAL_PICKUP", new Vector3(210.2f, -20.0f, -164.8f) ),
                    new LocationData( 78, "AT: Mausoleum Central Room (Right)", "HEALTH_VIAL_PICKUP", new Vector3(271.1f, -20.0f, -166.5f) ),
                    new LocationData( 79, "AT: Mausoleum Central Room (Left)", "HOLY_WATER_PICKUP", new Vector3(276.4f, -20.0f, -131.7f) ),
                    new LocationData( 80, "AT: Mausoleum Central Room (Back)", "EARTH_ELIXIR_PICKUP", new Vector3(293.9f, -20.0f, -146.1f) ),
                    new LocationData( 81, "AT: Mausoleum Central Room (Left Path)", "HEALTH_VIAL_PICKUP", new Vector3(261.1f, -21.7f, -53.8f) ),
                    new LocationData( 82, "AT: Mausoleum Central Room (Right Path)", "OCEAN_ELIXIR_PICKUP", new Vector3(290.4f, -21.7f, -246.5f) ),
                    new LocationData( 83, "AT: Kill Death", "COIN_PICKUP" ),
                    new LocationData( 84, "AT: Tomb With Switch", "DUNPEAL_PICKUP", new Vector3(47.9f, -7.1f, -40.8f) ),
                    new LocationData( 85, "AT: Tomb With Sitting Corpse", "BANNER_PICKUP", new Vector3(151.5f, -10.4f, 3.2f) ),
                    new LocationData( 86, "AT: Demi Chest", "LIGHTNING", new Vector3(155.3f, -3.2f, 73.0f) ),
                    new LocationData( 87, "AT: Near Light Switch", "CRYSTAL_SHARD_PICKUP", new Vector3(-51.5f, -11.9f, -106.6f) ),
                    new LocationData( 88, "AT: Hidden Room in Tomb", "EARTH_ELIXIR_PICKUP", new Vector3(-85.1f, -16.0f, -78.2f) ),
                    new LocationData( 89, "AT: Hidden Chest in Tomb", "GOLD_100", new Vector3(1.0f, -9.5f, 21.0f) ),
                }
            },
            {
                "FOREST_A1", // Yosei Forest
                new(){
                    new LocationData( 94, "YF: Barrel Group", "HEALTH_VIAL_PICKUP", new Vector3(26.2f, 36.0f, 15.5f)),
                    new LocationData( 95, "YF: Blood Pool", "BLOOD_DRAIN_PICKUP", new Vector3(-35.6f, 30.1f, -56.4f)),
                    new LocationData( 96, "YF: Branches Within Tree", "HEAL", new Vector3(-11.2f, 15.3f, 62.5f)),
                    new LocationData( 97, "YF: Chest Near Tree", "ELF_BOW_PICKUP", new Vector3(-68.2f, 0.5f, -0.2f)),
                    new LocationData( 98, "YF: Blood Plant's Insides", "HEALTH_VIAL_PICKUP", new Vector3(-20.5f, 3.8f, -34.6f)),
                    new LocationData( 99, "YF: Hanging In The Trees", "SWORD_PICKUP", new Vector3(-209.8f, 26.5f, -41.9f)),
                    new LocationData( 100, "YF: Hidden Chest", "FLAME_FLARE", new Vector3(-259.8f, -20.3f, -176.3f)),
                    new LocationData( 101, "YF: Room Defended by Blood Plant", "EARTH_STRIKE", new Vector3(-274.7f, -32.4f, -176.2f)),
                    new LocationData( 102, "YF: Patchouli's Canopy Offer", "ENKEY_PICKUP", new Vector3(-57.9f, -16.8f, -116.1f)),
                    new LocationData( 103, "YF: Patchouli's Reward", "EARTH_ELIXIR_PICKUP", new Vector3(-57.9f, -16.8f, -116.3f)),
                    new LocationData( 104, "YF: Patchouli's Yuletide Offering", "NOGG_PICKUP", new Vector3(-53.968f, -15.6588f, -116.677f))
                }
            },
            {
                "FOREST_B1", // Forest Canopy
                new(){
                    new LocationData( 109, "FC: Branch Lower Edge", "CRYSTAL_SHARD_PICKUP", new Vector3(-48.6f, -2.2f, -5.1f)),
                    new LocationData( 110, "FC: Branch Cave", "FAIRY_MOSS_PICKUP", new Vector3(-48.6f, 0.0f, -56.7f)),
                    new LocationData( 111, "FC: Chest", "DARK_SKULL_PICKUP", new Vector3(-150.4f, 15.7f, 40.4f)),
                    new LocationData( 112, "FC: Wooden Statue (Josiah)", "SKULL_PICKUP", new Vector3(-21.4f, 24.7f, 11.8f)),
                    new LocationData( 113, "FC: Wooden Statue (Sitting)", "WIND", new Vector3(-141.8f, 39.0f, -23.9f)),
                }
            },
            {
                "ARCHIVES", // Forbidden Archives
                new(){
                    new LocationData( 119, "FbA: Back Room Past Bridge", "OCEAN_ELIXIR_PICKUP", new Vector3(-99.0f, -4.0f, -51.6f) ),
                    new LocationData( 120, "FbA: Strange Corpse", "PICKUP", new Vector3(-85.6f, -2.8f, -5.8f) ),
                    new LocationData( 123, "FbA: Snail Lectern (Near)", "LIGHT_REVEAL", new Vector3(-28.0f, 9.5f, 52.0f) ),
                    new LocationData( 124, "FbA: Snail Lectern (Far)", "BLOOD_DRAIN", new Vector3(-28.0f, 9.5f, 45.0f) ),
                    new LocationData( 125, "FbA: Rug on Balcony", "MANA_VIAL_PICKUP", new Vector3(-41.4f, 8.0f, -59.3f) ),
                    new LocationData( 126, "FbA: Rooftops", "EARTH_ELIXIR_PICKUP", new Vector3(3.6f, 6.5f, -2.1f) ),
                    new LocationData( 127, "FbA: Hidden Room Upper Floor", "WOLFRAM_PICKUP", new Vector3(-112.1f, 8.5f, -22.4f) ),
                    new LocationData( 121, "FbA: Against Wall Near Trees", "L_URN_PICKUP", new Vector3(-113.8f, -20.0f, 67.1f) ),
                    new LocationData( 122, "FbA: Short Wall Near Trees", "HEALTH_VIAL_PICKUP", new Vector3(-110.9f, -20.0f, 46.0f) ),
                    new LocationData( 128, "FbA: Hidden Room Lower Floor", "CRYSTAL_SHARD_PICKUP", new Vector3(-57.9f, -19.9f, 38.5f) ),
                    new LocationData( 129, "FbA: Near Twisty Tree", "FAIRY_MOSS_PICKUP", new Vector3(-7.1f, -19.6f, 44.2f) ),
                    new LocationData( 130, "FbA: uwu", "WEPON", new Vector3(-2.1f, -18.2f, 32.1f) ),
                    new LocationData( 131, "FbA: Daedalus Knowledge (First)", "RING", new Vector3(-3.2f, -19.3f, -45.9f)),
                    new LocationData( 132, "FbA: Daedalus Knowledge (Second)", "RING", new Vector3(-3.2f, -19.3f, -45.9f)),
                    new LocationData( 133, "FbA: Daedalus Knowledge (Third)", "RING", new Vector3(-3.2f, -19.3f, -45.9f)),
                    new LocationData( 134, "FbA: Corner Near Daedalus", "HEALTH_VIAL_PICKUP", new Vector3(-43.6f, -20.0f, -29.6f))
                }
            },
            {
                "CAS_1", // Castle Le Fanu
                new(){
                    new LocationData( 140, "CLF: Outside Corner", "MANA_VIAL_PICKUP", new Vector3(-43.1f, 4.0f, -105.4f) ),
                    new LocationData( 141, "CLF: Cattle Cell (South)", "CANDLE_PICKUP", new Vector3(113.8f, -6.0f, -149.8f) ),
                    new LocationData( 142, "CLF: Cattle Cell (West)", "HEALTH_VIAI_PICKUP", new Vector3(157.0f, -6.0f, -177.1f) ),
                    new LocationData( 143, "CLF: Cattle Cell (Center)", "ICE_SWORD", new Vector3(90.8f, -5.3f, -178.7f) ),
                    new LocationData( 144, "CLF: Cattle Cell (North)", "KEY1_PICKUP", new Vector3(126.7f, -5.9f, -258.3f) ),
                    new LocationData( 145, "CLF: Hidden Cattle Cell", "WAND_PICKUP", new Vector3(113.8f, -6.0f, -286.0f) ),
                    new LocationData( 146, "CLF: Hallway Rubble Room", "L_URN_PICKUP", new Vector3(39.9f, 2.0f, -155.9f) ),
                    new LocationData( 147, "CLF: Hallway Dining Room", "BLOOD_WINE_PICKUP", new Vector3(27.6f, 3.2f, -185.8f) ),
                    new LocationData( 148, "CLF: Garrat Resting Room (Fountain)", "HOLY_WATER_PICKUP", new Vector3(-33.4f, 4.0f, -178.0f) ),
                    new LocationData( 149, "CLF: Garrat Resting Room (Wall)", "CROSSBOW_PICKUP", new Vector3(-26.9f, 4.2f, -175.9f) ),
                    new LocationData( 150, "CLF: Hallway Dead End Before Blue Doors", "KEY2_PICKUP", new Vector3(-32.1f, 2.3f, -92.5f) ),
                    new LocationData( 151, "CLF: Upper Floor Coffin Room (Small Room)", "EARTH_ELIXIR_PICKUP", new Vector3(-8.1f, 26.0f, -239.9f) ),
                    new LocationData( 152, "CLF: Upper Floor Coffin Room (Large Room)", "OCEAN_ELIXIR_PICKUP", new Vector3(-20.6f, 25.4f, -334.6f) ),
                    new LocationData( 153, "CLF: Upper Floor Coffin Room (Double)", "SWORD_PICKUP", new Vector3(-54.0f, 28.4f, -249.0f) ),
                    new LocationData( 154, "CLF:  Upper Floor Coffin Room (Hallway)", "KEY2_PICKUP", new Vector3(-2.4f, -2.8f, -202.6f) ),
                }
            },
            {
                "CAS_3", // Sealed Ballroom
                new(){
                    new LocationData( 166, "SB: Entry Small Room Lounge", "BLOOD_WINE_PICKUP", new Vector3(-65.2f, 1.0f, -19.3f) ),
                    new LocationData( 167, "SB: Entry Hidden Couch Top", "STEELNEEDLE_PICKUP", new Vector3(-31.4f, 0.8f, -33.3f) ),
                    new LocationData( 168, "SB: Entry Hidden Couch Bottom", "HEALTH_VIAL_PICKUP", new Vector3(-32.1f, 0.8f, -28.7f) ),
                    new LocationData( 169, "SB: Entry Hidden Cave in a Lounge", "CANDLE_PICKUP", new Vector3(-84.8f, 0.0f, -44.2f) ),
                    new LocationData( 170, "SB: Entry Lounge Long Table", "HEALTH_VIAL_PICKUP", new Vector3(-51.4f, 1.0f, -95.8f) ),
                    new LocationData( 171, "SB: Side Hidden Cave", "CRYSTAL_SHARD_PICKUP", new Vector3(24.6f, 0.0f, -72.2f) ),
                    new LocationData( 172, "SB: Side Chest Near Switch", "MAGIC", new Vector3(44.0f, 0.7f, 6.0f) ),
                    new LocationData( 173, "SB: Side Painting Viewing Room", "OCEAN_ELIXIR_PICKUP", new Vector3(75.0f, 1.0f, -22.0f) ),
                    new LocationData( 174, "SB: Side Hidden Casket Room", "HEALTH_VIAL_PICKUP", new Vector3(98.0f, 0.0f, -77.5f) ),
                    new LocationData( 175, "SB: Side XP Drain Party Room", "FLAIL_PICKUP", new Vector3(79.2f, 0.0f, -105.0f) ),
                }
            },
            {
                "WALL_01", // Laetus Wall
                new(){
                    new LocationData( 181, "LC: Hidden Room", "ICE_TEAR_PICKUP", new Vector3(-50.1f, 36.7f, 5.4f)),
                    new LocationData( 182, "LC: Invisible Path to Cliffside", "SWORD_PICKUP", new Vector3(8.3f, 65.0f, -62.7f))
                }
            },
            {
                "PITT_B1", // Great Well Surface
                new(){
                    new LocationData( 188, "GWS: Demi's Gift", "CRYSTAL_SHARD_PICKUP", new Vector3(-10.1f, 1.6f, -4.6f))
                }
            },
            {
                "CAS_2", // Throne Chamber
                new(){
                    new LocationData( 194, "TC: Crilall's Book Repository", "BOOK_PICKUP", new Vector3(-22.2f, 15.6f, -135.6f) ),
                }
            },
            {
                "CAVE", // Boiling Grotto
                new(){
                    new LocationData( 200, "BG: Corpse Beneath Entrance", "GOLD_5", new Vector3(-0.8f, 0.9f, -36.9f) ),
                    new LocationData( 201, "BG: Slab of a Broken Bridge", "CRYSTAL_SHARD_PICKUP", new Vector3(23.0f, 3.3f, -114.0f) ),
                    new LocationData( 202, "BG: Hidden Chest", "GOLD_25", new Vector3(-266.0f, 16.5f, 65.0f) ),
                    new LocationData( 203, "BG: Triple Hidden Chest", "ASHES", new Vector3(-266.0f, 16.4f, 105.0f) ),
                    new LocationData( 204, "BG: Lava Overseeing Dragon Switch", "ROCK_RING_PICKUP", new Vector3(-254.5f, 11.0f, -11.0f) ),
                    new LocationData( 205, "BG: Through Dragon Switch Tunnel", "MANA_VIAL_PICKUP", new Vector3(-216.0f, 10.0f, -69.7f) ),
                    new LocationData( 206, "ST: Top Right Sarcophagus", "BANDAGE_PICKUP", new Vector3(-383.1f, 12.1f, -200.2f) ),
                    new LocationData( 207, "ST: Second Floor Snake Room", "HEALTH_VIAL_PICKUP", new Vector3(-332.5f, 20.0f, -80.6f) ),
                    new LocationData( 208, "ST: Basement Snake Pit", "STAFF_PICKUP", new Vector3(-298.3f, 6.0f, -157.0f) ),
                    new LocationData( 209, "ST: Room Buried in Sand", "STAFF_PICKUP", new Vector3(-351.1f, -23.7f, -125.6f) ),
                    new LocationData( 210, "ST: Basement Stone Rubble", "MANA_VIAL_PICKUP", new Vector3(-291.9f, 4.0f, -31.4f) ),
                    new LocationData( 211, "ST: Hidden Sarcophagus", "BANDAGE_PICKUP", new Vector3(-334.6f, 12.1f, -184.3f) ),
                    new LocationData( 212, "ST: Second Floor Dead End", "HEALTH_VIAL_PICKUP", new Vector3(-371.0f, 20.0f, -194.7f) ),
                    new LocationData( 213, "ST: Lunacid Sandwich", "CLAW_PICKUP", new Vector3(-422.8f, 12.0f, -136.1f) ),
                    new LocationData( 214, "ST: Chest Near Switch", "RING", new Vector3(-301.0f, 16.7f, -70.0f) ),
                    new LocationData( 215, "ST: Chest Overlooking Crypt", "OCEAN_ELIXIR_PICKUP", new Vector3(-327.4f, 12.7f, 45.0f) ),
                    new LocationData( 216, "ST: Floor Switch Maze", "Moon_Vial_PICKUP", new Vector3(-361.6f, -20.0f, -85.2f) ),
                    new LocationData( 217, "ST: Triple Sarcophagus", "GOLD_10", new Vector3(-327.4f, 12.0f, -139.8f)),
                }
            },
            {
                "TOWER", // Tower of Abyss
                new(){
                    new LocationData( 222, "TA: Prize Beneath Tree", "BLADE_PICKUP", new Vector3(59.9f, -10.0f, -174.9f) ),
                    new LocationData( 223, "TA: Floor 5 Chest", "MANA_VIAL_PICKUP", ignoreLocationHandler: true),
                    new LocationData( 224, "TA: Floor 10 Chest", "ANTIDOTE_PICKUP", ignoreLocationHandler: true),
                    new LocationData( 225, "TA: Floor 15 Chest", "FAIRY_MOSS_PICKUP", ignoreLocationHandler: true),
                    new LocationData( 226, "TA: Floor 20 Chest", "SPECTRAL_CANDLE_PICKUP", ignoreLocationHandler: true),
                    new LocationData( 227, "TA: Floor 25 Chest", "HEALTH_VIAL_PICKUP", ignoreLocationHandler: true),
                    new LocationData( 228, "TA: Floor 30 Chest", ignoreLocationHandler: true),
                    new LocationData( 229, "TA: Floor 35 Chest", ignoreLocationHandler: true),
                    new LocationData( 230, "TA: Floor 40 Chest", "CANDLE_PICKUP", ignoreLocationHandler: true),
                    new LocationData( 231, "TA: Floor 45 Chest", "EARTH_ELIXIR_PICKUP", ignoreLocationHandler: true),
                    new LocationData( 232, "TA: Floor 50 Chest", "OCEAN_ELIXIR_PICKUP", ignoreLocationHandler: true),
                }
            },
            {
                "PRISON", // Terminus Prison
                new(){
                    new LocationData( 258, "TP: Third Floor Locked Cell Left", "BANDAGE_PICKUP", new Vector3(34.0f, 11.1f, -8.0f) ),
                    new LocationData( 259, "TP: Third Floor Locked Cell Right", "ASHES", new Vector3(-15.3f, 11.0f, -9.0f) ),
                    new LocationData( 260, "TP: Third Floor Locked Cell South", "LANCE_PICKUP", new Vector3(57.5f, 12.3f, 39.2f) ),
                    new LocationData( 261, "TP: Almost Bottomless Pit", "FLIGHT", new Vector3(-40.0f, -11.0f, 6.0f) ),
                    new LocationData( 262, "TP: Second Floor Broken Cell", "BANDAGE_PICKUP", new Vector3(0.0f, -1.0f, -8.0f) ),
                    new LocationData( 263, "TP: Second Floor Jailer's Table", "KEY_PICKUP", new Vector3(-56.8f, 1.3f, 3.9f) ),
                    new LocationData( 264, "TP: First Floor Hidden Cell", "HOLY_WATER_PICKUP", new Vector3(-32.9f, -17.0f, -75.1f) ),
                    new LocationData( 265, "TP: First Floor Hidden Debris Room", "L_URN_PICKUP", new Vector3(-10.6f, -16.8f, -107.5f) ),
                    new LocationData( 266, "TP: First Floor Remains", "CURSED_BLADE_PICKUP", new Vector3(-34.0f, -16.0f, -93.7f) ),
                    new LocationData( 267, "TP: Green Asylum Guarded Alcove (Left)", "Moon_Vial_PICKUP", new Vector3(55.4f, -48.0f, 8.4f) ),
                    new LocationData( 268, "TP: Green Asylum Guarded Alcove (Right)", "Moon_Vial_PICKUP", new Vector3(54.7f, -48.0f, 8.6f) ),
                    new LocationData( 269, "TP: Green Asylum Long Alcove", "EARTH_ELIXIR_PICKUP", new Vector3(67.6f, -54.0f, -14.2f) ),
                    new LocationData( 270, "TP: Green Asylum Bone Pit", "HEALTH_VIAL_PICKUP", new Vector3(67.6f, -48.9f, -39.3f) ),
                    new LocationData( 271, "TP: Fourth Floor Cell Hanging Remains", "HEALTH_VIAL_PICKUP", new Vector3(-39.4f, 23.0f, 27.0f) ),
                    new LocationData( 272, "TP: Fourth Floor Maledictus Secret", "BLUE_FLAME_PICKUP", new Vector3(36.0f, 23.9f, 63.9f) ),
                    new LocationData( 273, "TP: Fourth Floor Hidden Jailer Sleeping Spot", "HOLY_WATER_PICKUP", new Vector3(51.5f, 23.2f, 67.7f) ),
                    new LocationData( 274, "TP: Fourth Floor Jailer Break Room", "MANA_VIAL_PICKUP", new Vector3(74.2f, 24.0f, 95.4f) ),
                    new LocationData( 275, "TP: Etna's Resting Place Item 1", "ECTOPLASM", new Vector3(31.1f, 24.3f, -17.0f) ),
                    new LocationData( 276, "TP: Etna's Resting Place Item 2", "SNOWFLAKE_OBSIDIAN", new Vector3(31.0f, 24.3f, -16.0f) ),
                    new LocationData( 277, "TP: Etna's Resting Place Item 3", "MOONPETAL", new Vector3(31.2f, 24.3f, -14.9f) ),
                    new LocationData( 278, "TP: Fourth Floor Collapsed Tunnel", "HAMMER_PICKUP", new Vector3(41.3f, 24.3f, -37.5f) ),
                    new LocationData( 279, "TP: Egg's Resting Place", "RATTLE_PICKUP", new Vector3(72.4f, -48.0f, -19.9f)),
                }
            },
            {
                "ARENA", // Forlorn Arena
                new(){
                    new LocationData( 284, "FlA: Corpse Waiting For A Full Moon", "HERITAGE SWORD", new Vector3(-160.0f, 5.1f, 22.4f) ),
                    new LocationData( 285, "FlA: Entry Rock Parkour", "HEALTH_VIAL_PICKUP", new Vector3(-23.5f, 7.9f, 75.8f) ),
                    new LocationData( 286, "FlA: Temple of Earth Hidden Plant Haven", "SHADOW_PICKUP", new Vector3(12.6f, 6.7f, 182.5f) ),
                    new LocationData( 287, "FlA: Temple of Earth Hidden Room", "EARTH_ELIXIR_PICKUP", new Vector3(-4.2f, 12.0f, 171.1f) ),
                    new LocationData( 288, "FlA: Temple of Earth Fractured Chest", "PICKUP", new Vector3(-28.0f, -7.5f, 327.0f) ),
                    new LocationData( 289, "FlA: Temple of Earth Chest Near Switch", "TAL2_PICKUP", new Vector3(-27.0f, 8.8f, 118.0f) ),
                    new LocationData( 290, "FlA: Temple of Water Room Near Water", "FAIRY_MOSS_PICKUP", new Vector3(-169.5f, 8.0f, -117.0f) ),
                    new LocationData( 291, "FlA: Temple of Water Corner Near Water", "ANTIDOTE_PICKUP", new Vector3(-198.6f, 8.3f, -123.4f) ),
                    new LocationData( 292, "FlA: Temple of Water Collapsed End Near Balcony", "HEALTH_VIAL_PICKUP", new Vector3(-99.0f, 16.0f, -198.7f) ),
                    new LocationData( 293, "FlA: Temple of Water Hidden Basement (Left)", "ANTIDOTE_PICKUP (1)", new Vector3(-119.3f, -8.0f, -159.6f) ),
                    new LocationData( 294, "FlA: Temple of Water Hidden Basement (Right)", "ANTIDOTE_PICKUP (2)", new Vector3(-119.2f, -8.0f, -158.3f) ),
                    new LocationData( 295, "FlA: Temple of Water Hidden Laser Room", "OBS_SWORD_PICKUP", new Vector3(-116.0f, -8.0f, -137.6f) ),
                    new LocationData( 296, "FlA: Temple of Water Hidden Alcove Before Stairs", "HEALTH_VIAL_PICKUP", new Vector3(-19.4f, -8.0f, -140.3f) ),
                    new LocationData( 297, "FlA: Temple of Water Hidden Alcove (Left)", "WISP_HEART_PICKUP (1)", new Vector3(66.6f, 0.0f, -169.9f) ),
                    new LocationData( 298, "FlA: Temple of Water Hidden Alcove (Right)", "WISP_HEART_PICKUP", new Vector3(66.6f, 0.0f, -166.0f) ),
                    new LocationData( 299, "FlA: Temple of Water Hidden Alcove Before Switch", "OCEAN_ELIXIR_PICKUP", new Vector3(103.9f, 0.0f, -109.3f) ),
                    new LocationData( 300, "FlA: Temple of Water Fractured Chest", "PICKUP", new Vector3(3.2f, 24.5f, -281.0f) ),
                    new LocationData( 301, "FlA: Temple of Water Chest Near Switch", "TAL1_PICKUP", new Vector3(-42.0f, 8.8f, -121.0f) ),
                }
            },
            {
                "VOID", // Labyrinth of Ash
                new(){
                    new LocationData( 307, "LA: Entry Coffin", "MANA_VIAL_PICKUP", new Vector3(46.8f, 0.0f, 21.7f) ),
                    new LocationData( 308, "LA: Giant Remains", "HEALTH_VIAL_PICKUP", new Vector3(-74.5f, 0.0f, -37.4f) ),
                    new LocationData( 309, "LA: Behind Statue", "WISP_HEART_PICKUP", new Vector3(-88.8f, 0.2f, 62.9f) ),
                    new LocationData( 310, "LA: Rocks Near Switch", "LAVA_PICKUP", new Vector3(-34.7f, 0.3f, 2.4f) ),
                    new LocationData( 311, "LA: Forbidden Light Chest", "MAGIC", new Vector3(126.0f, 0.7f, -20.0f) ),
                    new LocationData( 312, "LA: Hidden Light Stash", "D_URN_PICKUP", new Vector3(173.6f, 0.0f, -25.1f) ),
                    new LocationData( 313, "LA: NNSNSSNSNN Lost Maze", "FANG_PICKUP", new Vector3(-7.7f, 70.2f, -22.1f) ),
                }
            },
            {
                "ARENA2", // Chamber of Fate
                new(){
                    new LocationData( 319, "CF: Calamis' Weapon of Choice", ignoreLocationHandler: true)
                }
            }

        };

        public static readonly Dictionary<string, string> sceneToArea = new(){
            {"PITT_A1", "Holow Basin"},
            {"HUB_01", "Wing's Rest"},
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
            {"CAS_2", "Throne Chamber"},
            {"CAS_3", "Sealed Ballroom"},
            {"PRISON", "Terminus Prison"},
            {"TOWER", "Tower of Abyss"},
            {"ARENA", "Forlorn Arena"},
            {"ARENA2", "Chamber of Fate"},

        };

        public static readonly List<string> LocationsThatImmediatelyReceive = new(){
            "Buy Enchanted Key", "Buy Rapier", "Buy Steel Needle", "Buy Crossbow", "Buy Oil Lantern",
            "Buy Ocean Elixir (Sheryl)", "Buy Ocean Elixir (Patchouli)", "Buy Privateer Musket", 
            "Buy Jotunn Slayer", "GWS: Demi's Gift", "FbA: Daedalus Knowledge (First)", "FbA: Daedalus Knowledge (Second)",
            "FbA: Daedalus Knowledge (Third)", "WR: Clive's Gift", "WR: Demi's Introduction Gift", "WR: Demi's Victory Gift"
        };

        public static readonly List<string> TowerLocations = new(){
            "TA: Prize Beneath Tree", "TA: Floor 5 Chest", "TA: Floor 10 Chest", "TA: Floor 15 Chest", "TA: Floor 20 Chest",
            "TA: Floor 25 Chest", "TA: Floor 30 Chest", "TA: Floor 35 Chest", "TA: Floor 40 Chest", "TA: Floor 45 Chest", "TA: Floor 50 Chest",
        };

        public static readonly List<string> CoinLocations = new(){
            "HB: Temple Blood Altar", "AT: Kill Death", "SS: Killing the Jotunn"
        };

        public static readonly List<string> DaedalusLocations = new(){
            "FbA: Daedalus Knowledge (First)", "FbA: Daedalus Knowledge (Second)", "FbA: Daedalus Knowledge (Third)"
        };

        public static readonly List<string> ChristmasLocations = new(){
            "WR: Christmas Present", "YF: Patchouli's Yuletide Offering"
        };
    }
}
