using System.Collections.Generic;

namespace LunacidAP.Data
{
    public static class LunacidEnemies
    {
        public static readonly List<string> EnemyNames = new(){
            "Snail", "Milk Snail", "Shulker", "Mummy", "Mummy Knight", "Necronomicon", "Enlightened One", "Chimera", "Rat", "Hemalith",
            "Great Bat", "Rat King", "Rat Queen", "Slime Skeleton", "Skeleton", "Slime", "Devil Slime", "Lunaga", "Kodama", "Yakul",
            "Venus", "Neptune", "Unilateralis", "Tillandsia", "Mimic", "Mare", "Mi-Go", "Phantom", "Cursed Painting", "Malformed", "Poltergeist",
            "Giant Bat", "Vampire Page", "Vampire", "Malformed Horse", "Hallowed Husk", "Ikurr'ilb", "Obsidian Skeleton", "Serpent",
            "Anpu", "Embalmed", "Jailor", "Lupine Skeleton", "Giant Skeleton", "Sucsarian", "Ceres", "Vesta", "Gloom Wood",
            "Cetea", "Abyssal Demon", "Cerritulus Lunam",
        };

        public static readonly Dictionary<string, string> NamesToGameObject = new(){
            // Some of these are clones; should be accounted for.
            {"MUMMY KNIGHT", "Mummy Knight"},
            {"MUMMY", "Mummy"},
            {"MILK_SNAIL_DED(Clone)", "Milk Snail"},
            {"MILK_SNAIL_DED2(Clone)", "Milk Snail"},
            {"SNAIL_DED(Clone)", "Snail"},
            {"SHULKER_DED(Clone)", "Shulker"},
            {"RAT_DED1(Clone)", "Rat"}, // Monitor this; seems to end with a different number sometimes?
            {"RAT_DED2(Clone)", "Rat"},
            {"RAT KING", "Rat King"},
            {"SLIME SKELETON", "Slime Skeleton"},
            {"RABBIT_DED(Clone)", "Kodama"},
            {"RABBIT_alt2_DED(Clone)", "Kodama"},
            {"ELK_DED(Clone)", "Yakul"}, //Another one which ends in a random number sometimes.
            {"ELK_DED2(Clone)", "Yakul"},
            {"ELK_DED3(Clone)", "Yakul"},
            {"VENUS", "Venus"},
            {"ABYSSAL DEMON", "Abyssal Demon"},
            {"NEPTUNE", "Neptune"},
            {"UNILAT_DED(Clone)", "Unilateralis"},
            {"HEMALITH", "Hemalith"},
            {"MI-GO", "Mi-Go"},
            {"MARE_DIE(Clone)", "Mare"},
            {"CURSED PAINTING", "Cursed Painting"},
            {"Cursed Painting", "Cursed Painting"},
            {"Necronomicon", "Necronomicon"},
            {"CHIMERA", "Chimera"},
            {"NERV_DED(Clone)", "Enlightened One"},
            {"PHANTOM", "Phantom"},
            {"OBSIDIAN SKELETON", "Obsidian Skeleton"},
            {"Starved Vampire Thrall", "Vampire"},
            {"Vampire Page", "Vampire Page"},
            {"MALFORMED", "Malformed"},
            {"Great Bat", "Great Bat"},
            {"Poltergeist", "Poltergeist"},
            {"MALFORMED HORSE", "Malformed Horse"},
            {"HALLOWED HUSK", "Hallowed Husk"},
            {"IKURR'ILB", "Ikurr'ilb"},
            {"SKELETON", "Skeleton"},
            {"MIMIC", "Mimic"},
            {"Anpu", "Anpu"},
            {"Serpent", "Serpent"},
            {"Embalmed", "Embalmed"},
            {"JAILOR", "Jailor"},
            {"CERRITULUS LUNAM", "Cerritulus Lunam"},
            {"LUPINE SKELETON", "Lupine Skeleton"},
            {"INFESTED CORPSE", "Infested Corpse"},
            {"GIANT SKELETON", "Giant Skeleton"},
            {"CERES", "Ceres"},
            {"CETEA", "Cetea"},
            {"SUCSARIAN", "Sucsarian"},
            {"VESTA", "Vesta"},
            {"GLOOM WOOD", "Gloom Wood"},
            {"SANGUIS UMBRA", "Sanguis Umbra"},
        };

        public static readonly List<string> NotEnemyWhitelist = new(){
            "Broke_Pot(Clone)"
        };

        public static readonly Dictionary<string, string> ObjectToLocationSuffix = new(){
            {"OCEAN_ELIXIR_PICKUP", "Ocean Elixir"},
            {"SUS_DAGGER_PICKUP", "Sucsarian Dagger"},
            {"SUS_SPEAR_PICKUP", "Sucsarian Spear"},
            {"SUMMON_SNAIL", "Summon Snail"},
            {"SICKLE_PICKUP", "Ice Sickle"},
            {"TORNADO_PICKUP", "Tornado"},
            {"CANDLE_WEP_PICKUP", "Jailor's Candle"},
            {"RUSTED_SWORD_PICKUP", "Rusted Sword"},
            {"GOLDEN_SICKLE_PICKUP", "Golden Sickle"},
            {"GOLDEN_KHOPESH_PICKUP", "Golden Khopesh"},
            {"BRITTLE_PICKUP", "Brittle Arming Sword"},
            {"BOOK_PICKUP", "Black Book"},
            {"OBS_SHIELD_PICKUP", "Obsidian Cursebrand"},
            {"OBS_SWORD_PICKUP", "Obsidian Poisonguard"},
            {"CURSED_BLADE_PICKUP", "Cursed Blade"},
            {"VP_SWORD_PICKUP", "Lyrian Longsword"},
            {"QUICK_STRIDE", "Quick Stride"},
            {"SUMMON_KODAMA", "Summon Kodama"},
            {"SKELE_AXE_PICKUP", "Skeleton Axe"},
            {"DARK_SKULL_PICKUP", "Dark Skull"},
            {"SHRIMP_PICKUP", "Pink Shrimp"},
            {"FAIRY_MOSS_PICKUP", "Fairy Moss"},
            {"THROWING_KNIFE_PICKUP", "Throwing Knife"},
            {"ANGEL_PICKUP", "Angel Feather"},
            {"CANDLE_PICKUP", "Spectral Candle"},
            {"HEALTH_VIAL_PICKUP", "Health Vial"},
            {"MANA_VIAL_PICKUP", "Mana Vial"},
            {"Moon_Vial_PICKUP", "Moonlight Vial"},
            {"L_URN_PICKUP", "Light Urn"},
            {"HOLY_WATER_PICKUP", "Holy Water"},
            {"BANDAGE_PICKUP", "Cloth Bandage"},
            {"Cloth Bandage", "Cloth Bandage"},
            {"ANTIDOTE_PICKUP", "Antidote"},
            {"D_URN_PICKUP", "Dark Urn"},
            {"BLOODWEED", "Bloodweed"},
            {"ASHES", "Ashes"},
            {"FOOLS_GOLD", "Fool's Gold"},
            {"IKURR'ILB ROOT", "Ikurr'ilb Root"},
            {"ECTOPLASM", "Ectoplasm"},
            {"OCEAN_BONE_SHELL", "Ocean Bone Shell"},
            {"OCEAN_BONE_SHARD", "Ocean Bone Shard"},
            {"SNOWFLAKE_OBSIDIAN", "Snowflake Obsidian"},
            {"OBSIDIAN", "Obsidian"},
            {"YELLOW_MOREL", "Yellow Morel"},
            {"LOTUS_SEED_POD", "Lotus Seed Pod"},
            {"DEST_ANGEL", "Destroying Angel Mushroom"},
            {"FIRE_OPAL", "Fire Opal"},
            {"BONES", "Bones"},
            {"OPAL", "Opal"},
            {"ONYX", "Onyx"},
            {"VAMPIRIC ASHES", "Vampiric Ashes"},
            {"GOLDENESS", "Fool's Gold"},
            {"GOLD_5", "Small Silver"},
            {"GOLD_10", "Medium Silver"},
            {"GOLD_25", "Large Silver"},
        };

        public static readonly Dictionary<string, List<string>> EnemyToScenes = new(){
            {"Snail", new List<string>(){"PITT_A1"}},
            {"Milk Snail", new List<string>(){"PITT_A1", "ARCHIVES"}},
            {"Shulker", new List<string>(){"PITT_A1"}},
            {"Mummy", new List<string>(){"PITT_A1"}},
            {"Mummy Knight", new List<string>(){"PITT_A1"}},
            {"Necronomicon", new List<string>(){"ARCHIVES"}},
            {"Enlightened One", new List<string>(){"ARCHIVES"}},
            {"Chimera", new List<string>(){"ARCHIVES"}},
            {"Rat", new List<string>(){"SEWER_A1", "PRISON"}},
            {"Hemalith", new List<string>(){"LAKE"}},
            {"Great Bat", new List<string>(){"CAS_1"}},
            {"Rat King", new List<string>(){"SEWER_A1", "HAUNT", "PRISON"}},
            {"Rat Queen", new List<string>(){"SEWER_A1", "PRISON"}},
            {"Slime Skeleton", new List<string>(){"SEWER_A1"}},
            {"Skeleton", new List<string>(){"SEWER_A1", "CAVE", "HAUNT", "PRISON"}},
            {"Slime", new List<string>(){"SEWER_A1"}},
            {"Devil Slime", new List<string>(){"SEWER_A1"}},
            {"Lunaga", new List<string>(){"FOREST_A1"}},
            {"Kodama", new List<string>(){"FOREST_A1"}},
            {"Yakul", new List<string>(){"FOREST_A1"}},
            {"Venus", new List<string>(){"FOREST_A1", "FOREST_B1", "ARENA"}},
            {"Neptune", new List<string>(){"FOREST_A1", "FOREST_B1"}},
            {"Unilateralis", new List<string>(){"FOREST_B1"}},
            {"Tillandsia", new List<string>(){"FOREST_B1"}},
            {"Mimic", new List<string>(){"CAVE"}},
            {"Mare", new List<string>(){"HAUNT"}},
            {"Mi-Go", new List<string>(){"HAUNT"}},
            {"Phantom", new List<string>(){"CAS_1", "CAS_3", "HAUNT"}},
            {"Cursed Painting", new List<string>(){"HAUNT", "CAS_1"}},
            {"Malformed", new List<string>(){"HAUNT", "CAS_1"}},
            {"Poltergeist", new List<string>(){"CAS_1"}},
            {"Giant Bat", new List<string>(){"CAS_1"}},
            {"Vampire Page", new List<string>(){"CAS_1"}},
            {"Vampire", new List<string>(){"CAS_1"}},
            {"Malformed Horse", new List<string>(){"CAS_3"}},
            {"Hallowed Husk", new List<string>(){"CAS_3"}},
            {"Ikurr'ilb", new List<string>(){"CAVE"}},
            {"Obsidian Skeleton", new List<string>(){"CAVE", "PRISON", "VOID"}},
            {"Serpent", new List<string>(){"CAVE"}},
            {"Anpu", new List<string>(){"CAVE"}},
            {"Embalmed", new List<string>(){"CAVE"}},
            {"Jailor", new List<string>(){"PRISON"}},
            {"Lupine Skeleton", new List<string>(){"PRISON"}},
            {"Giant Skeleton", new List<string>(){"PRISON"}},
            {"Sucsarian", new List<string>(){"ARENA"}},
            {"Ceres", new List<string>(){"ARENA"}},
            {"Vesta", new List<string>(){"ARENA"}},
            {"Gloom Wood", new List<string>(){"ARENA"}},
            {"Cetea", new List<string>(){"VOID"}},
            {"Abyssal Demon", new List<string>(){"LAKE", "HAUNT"}},
            {"Cerritulus Lunam", new List<string>(){"PRISON"}},
            {"Sanguis Umbra", new List<string>(){"CAS_PITT"}},
            {"Infested Corpse", new List<string>(){"PRISON"}},
        };

        public static Dictionary<string, string> APWorldNameToGameName = new(){
            {"Milk Snail", "MILK_SNAIL"},
            {"Mummy Knight", "MUMMY_KNIGHT"},
            {"Necronomicon", "Necronomicon"},
            {"Cursed Painting", "Cursed Painting"},
            {"Vampire", "Starved Vampire Thrall"},
            {"Vampire Page", "Vampire Page"},
            {"Great Bat", "Great Bat"},
            {"Serpent", "Serpent"},
            {"Anpu", "Anpu"},
            {"Anpu (Sword)", "Anpu_Sword"},
            {"Skeleton (Weapon)", "SKELETON_WEAPON"},
            {"Sucsarian (Dagger)", "SUCSARIAN"},
            {"Sucsarian (Spear)", "SUCSARIAN_SPEAR"},
            {"Embalmed", "MUMMY_BANDAGED"},
            {"Poltergeist", "Poltergeist"}
        };

        public static Dictionary<string, string> ModdedNameToPrefabName = new(){
            {"MILK SNAIL", "MILK_SNAIL"},
            {"MUMMY KNIGHT", "MUMMY_KNIGHT"},
            {"Embalmed", "MUMMY_BANDAGED"}
        };

        public class EnemyLocationData
        {
            public string groupName;
            public int[] childPath;
            public int[] affectedChildren;

            public EnemyLocationData(string name, int[] path, int[] affected)
            {
                groupName = name;
                childPath = path;
                affectedChildren = affected;
            }
        }

        public class RandomizedEnemyData
        {
            public string groupName;
            public int affectedChild;
            public string enemyName;

            public RandomizedEnemyData(string name, int affected, string enemy)
            {
                groupName = name;
                affectedChild = affected;
                enemyName = enemy;
            }
        }

        public static readonly Dictionary<string, string> SceneToWorldObjectsName = new(){
            {"PITT_A1", "THE_PIT_A1"},
            {"FOREST_A1", "FOREST_A1"},
            {"SEWER_A1", "SEWER"},
            {"FOREST_B1", "FOREST_B1"},
            {"LAKE", "BLOOD_LAKE"},
            {"ARCHIVES", "ARCHIVES"},
            {"HAUNT", "HAUNT"},
            {"CAS_1", "CAS_1"},
            {"CAVE", "CAVE"},
            {"CAS_3", "CAS_3"},
            {"PRISON", "PRISON"},
            {"ARENA", "ARENA"},
            {"VOID", "VOID_WORLD"},
        };

        public static readonly List<string> WorldObjects = new(){
            "THE_PIT_A1", "FOREST_A1", "SEWER", "FOREST_B1", "BLOOD_LAKE", "ARCHIVES", "HAUNT",
            "CAS_1", "CAVE", "CAS_3", "PRISON", "ARENA", "VOID_WORLD"
        };

        public static readonly List<string> EnemySpells = new(){
            "LAVA_CAST", "POI_SPORE_CAST", "DARK_WAVE_CAST", "WIND_NPC_CAST", "ACID_SPIT_CAST",
            "DARK_SPHERE_CAST", "NPC_GHOST_FLARE_CAST", "SUS_DAGGER_CAST", "LASER_BEAM_CAST"
        };
        public static readonly Dictionary<string, int> SceneToAverageLevel = new(){
            {"PITT_A1", 9},
            {"FOREST_A1", 11},
            {"SEWER_A1", 9},
            {"FOREST_B1", 19},
            {"LAKE", 18},
            {"ARCHIVES", 19},
            {"HAUNT", 14},
            {"CAS_1", 20},
            {"CAVE", 23},
            {"CAS_3", 20},
            {"PRISON", 22},
            {"ARENA", 32},
            {"VOID", 37},
        };

        public static readonly Dictionary<string, string> CleanupName = new(){
            {"VENUS_HIDE", "VENUS"},
            {"SKELETON_WEAPON", "SKELETON"},
            {"MUMMY_CRAWLING", "MUMMY"},
            {"MUMMY_KNIGHT", "MUMMY KNIGHT"},
            {"Anpu_Sword", "Anpu"},
            {"MILK_SNAIL", "MILK SNAIL"},
            {"RAT2", "RAT"}
        };

        public static readonly Dictionary<string, List<EnemyLocationData>> BaseEnemyPositionData = new(){
            {"PITT_A1", new(){
            new EnemyLocationData("SmallGroupPittA1", new int[]{7, 2}, new int[]{0, 1, 2}),
            new EnemyLocationData("StartSnailPittA1", new int[]{7, 4, 0}, new int[]{0, 1, 2, 3, 4}),
            new EnemyLocationData("MainPittA1",  new int[]{7}, new int[]{6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16, 17, 18}),
            new EnemyLocationData("HiddenPittA1",  new int[]{7, 3}, new int[]{0, 1, 2, 3, 4, 5, 6, 7, 8, 9}),
            new EnemyLocationData("CrawlingPittA1",  new int[]{7, 3, 10}, new int[]{0, 1, 2, 3, 4, 5, 6})
            }},
            {"FOREST_A1", new(){
            new EnemyLocationData("LowerFrontForestA1", new int[]{7, 0}, new int[]{0, 1, 2, 3, 4}),
            new EnemyLocationData("LowerBackYakulForestA1", new int[]{7, 1}, new int[]{0, 1}),
            new EnemyLocationData("LowerBackBunForestA1", new int[]{7, 2}, new int[]{0, 1, 2, 3, 4, 5}),
            new EnemyLocationData("MainForestA1", new int[]{7}, new int[]{10, 11, 12, 13, 14, 15, 16, 17, 18, 19, 20, 21, 22, 23})
            }},
            {"SEWER_A1", new(){
            // new EnemyPositionData("RatNestSewerA1", new int[]{6, 11}, new int[]{12, 13, 14, 15}),  They spawn late or something...?
            new EnemyLocationData("MainSewerA1", new int[]{6}, new int[]{8, 9, 10, 12, 13, 14, 15, 17, 18, 19,
            22, 23, 24, 25, 26, 27, 28, 29, 30, 31, 36, 37})
            }},
            {"FOREST_B1", new(){
            new EnemyLocationData("NestForestB1", new int[]{7, 14}, new int[]{0, 1, 2}),
            new EnemyLocationData("MainForestB1", new int[]{7}, new int[]{2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 15, 16, 17, 18, 19})
            }},
            {"LAKE", new(){
            new EnemyLocationData("DryLake", new int[]{4, 5}, new int[]{3, 4, 5, 6, 7, 8, 9, 10, 11}),
            new EnemyLocationData("DryNest1Lake", new int[]{4, 5, 12}, new int[]{0, 1, 2}),
            new EnemyLocationData("DryNest2Lake", new int[]{4, 5, 13}, new int[]{0, 1}),
            new EnemyLocationData("WetLake", new int[]{4, 6}, new int[]{0, 1, 2, 3, 4, 5, 6, 7, 8}),
            new EnemyLocationData("WetNest1Lake", new int[]{4, 6, 9}, new int[]{0, 1, 2}),
            new EnemyLocationData("WetNest2Lake", new int[]{4, 6, 10}, new int[]{0, 1}),
            new EnemyLocationData("MainLake", new int[]{4}, new int[]{1, 2, 3, 4, 7, 8, 9, 10, 11, 12, 13, 14})
            }},
            {"ARCHIVES", new(){
            new EnemyLocationData("MainArchives", new int[]{5}, new int[]{1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 32}),
            new EnemyLocationData("ChimeraTopArchives", new int[]{5, 17}, new int[]{0}),
            new EnemyLocationData("ChimeraLowerArchives", new int[]{5, 18}, new int[]{0}),
            new EnemyLocationData("SnailNest1Archives", new int[]{5, 20}, new int[]{0, 1, 2, 3, 4}),
            new EnemyLocationData("SnailNest2Archives", new int[]{5, 21}, new int[]{0, 1}),
            new EnemyLocationData("SnailNest3Archives", new int[]{5, 22}, new int[]{0, 1}),
            new EnemyLocationData("NerveNest1Archives", new int[]{5, 23}, new int[]{0, 1}),
            new EnemyLocationData("NerveNest2Archives", new int[]{5, 24}, new int[]{0}),
            new EnemyLocationData("RandomNest1Archives", new int[]{5, 25}, new int[]{0, 1, 2}),
            new EnemyLocationData("NerveNest3Archives", new int[]{5, 31}, new int[]{0, 1}),
            new EnemyLocationData("RandomNest2Archives", new int[]{5, 34}, new int[]{0, 1})
            }},
            {"HAUNT", new(){
            new EnemyLocationData("JumpscareHaunt", new int[]{7, 56}, new int[]{0}),
            new EnemyLocationData("NestHaunt", new int[]{7, 27}, new int[]{0, 1, 2}),
            new EnemyLocationData("MainHaunt", new int[]{7}, new int[]{2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16, 17, 18, 19, 20, 21,
            22, 23, 24, 25, 26, 28, 29, 30, 31, 32, 33, 34, 35, 36, 37, 39, 40, 41, 42, 43, 44, 45, 46, 47, 48, 49, 50, 51, })
            }},
            {"CAS_1", new(){
            new EnemyLocationData("RedAreaCas1", new int[]{2, 7}, new int[]{0, 1, 2, 3, 4, 5, 6, 8, 9, 10, 11, 12, 13, 14, 15, 16}),
            new EnemyLocationData("RedAreaNestCas1", new int[]{2, 7, 7}, new int[]{0, 1}),
            new EnemyLocationData("EntryCas1", new int[]{2, 8}, new int[]{0, 1, 2, 3}),
            new EnemyLocationData("InteriorPhantomCas1", new int[]{2, 9}, new int[]{0, 1, 2, 3}),
            new EnemyLocationData("UndergroundNest1Cas1", new int[]{2, 10, 0}, new int[]{0, 1, 2}),
            new EnemyLocationData("UndergroundNest2Cas1", new int[]{2, 10, 1}, new int[]{0, 1}),
            new EnemyLocationData("InteriorOtherCas1", new int[]{2, 11}, new int[]{0, 1, 2, 3, 4, 5}),
            new EnemyLocationData("UpstairsCas1", new int[]{2, 13}, new int[]{0, 1, 2, 3, 4, 6, 7, 8}),
            new EnemyLocationData("UpstairsNestCas1", new int[]{2, 13, 5}, new int[]{0, 1, 2}),
            }},
            {"CAVE", new(){
            new EnemyLocationData("SnakeDen1Cave", new int[]{2, 10}, new int[]{0, 1, 2, 3}),
            new EnemyLocationData("SnakeDen2Cave", new int[]{2, 11}, new int[]{0, 1, 2, 3, 4, 5, 6}),
            new EnemyLocationData("AnpuSpawnCave", new int[]{2, 17}, new int[]{0}),
            new EnemyLocationData("MimicSpawnCave", new int[]{2, 21}, new int[]{0}),
            new EnemyLocationData("MummyUpCave", new int[]{2, 31}, new int[]{1}),
            new EnemyLocationData("MainCave", new int[]{2}, new int[]{0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 13, 14, 15, 16, 18, 20, 23, 24, 25,
            26, 27, 28, 29, 33, 34, 35})
            }},
            {"CAS_3", new(){
            new EnemyLocationData("HallowNest1Cas3", new int[]{2, 0}, new int[]{0, 1, 2, 3}),
            new EnemyLocationData("HallowNest2Cas3", new int[]{2, 1}, new int[]{0, 1}),
            new EnemyLocationData("HallowNest3Cas3", new int[]{2, 2}, new int[]{0, 1, 2}),
            new EnemyLocationData("HorseJumpscareCas3", new int[]{2, 14}, new int[]{0}),
            new EnemyLocationData("MainCas3", new int[]{2}, new int[]{3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13})
            }},
            {"PRISON", new(){
            new EnemyLocationData("MainPrison", new int[]{7}, new int[]{5, 8, 9, 11, 12, 13, 15, 16, 17, 18, 19, 20, 21, 22, 23, 24, 27, 29,
            30, 31, 32, 33, 34, 35, 37, 38, 39, 40, 41, 42, 43, 44, 45, 46, 47, 49, 50, 51, }),
            new EnemyLocationData("CellPrison", new int[]{7, 6}, new int[]{0, 1}),
            new EnemyLocationData("AliveCorpsesPrison", new int[]{7, 10, 0}, new int[]{0, 1, 2}),
            new EnemyLocationData("NoAggroCorpsesPrison", new int[]{7, 10, 1}, new int[]{0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15,
            16, 17, 18, 19, 20}),
            new EnemyLocationData("SkelNest1Prison", new int[]{7, 25}, new int[]{0, 1}),
            new EnemyLocationData("SkelNest2Prison", new int[]{7, 26}, new int[]{0, 1}),
            new EnemyLocationData("DogPackPrison", new int[]{7, 36}, new int[]{9, 10}),
            new EnemyLocationData("SkelSpawnPrison", new int[]{7, 48}, new int[]{0}),
            new EnemyLocationData("RatNestPrison", new int[]{7, 52}, new int[]{0, 1, 2, 3})
            }},
            {"ARENA", new(){
            new EnemyLocationData("MainArena", new int[]{2}, new int[]{0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16, 17, 18, 19, 20,
            21, 22, 23, 24, 25, 26, 27, 28, 29, 30, 31, 32, 33, 34, 35, 36, 37, 38, 39, 40, 41, 42, 43, 44, 45, 46, 47, 48, 49, 50, 51, 52, 53,
            54, 55, 56, 57, 58, 59, 60, 61, 62, 63})
            }},
            {"VOID", new(){
            new EnemyLocationData("RayNestVoid", new int[]{8, 9}, new int[]{0, 1}),
            new EnemyLocationData("MainVoid", new int[]{8}, new int[]{0, 1, 2, 3, 4, 6, 7, 8, 10, 11, 12, 13, 14, 15, 16, 17, 18, 19, 20, 21,
            22, 23, 24, 25})
            }}
        };
    }
}