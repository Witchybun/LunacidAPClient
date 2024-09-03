using System.Collections.Generic;
using System.Linq;
using BepInEx.Logging;
using HarmonyLib;
using LunacidAP.Data;
using UnityEngine;
using static LunacidAP.Data.LunacidLocations;

namespace LunacidAP
{
    public class EnemyHandler
    {
        private static ManualLogSource _log;
        private static Dictionary<string, GameObject> EnemyPrefabs = new();
        private static List<string> EnemyPrefabKeys = new();
        private static List<string> barredEnemies = new(){
            "MILK SNAIL_2", "RAT", "SKELETON_HURT", "LVG_OBJ", "Starved VP_Hurt", "ANKOU", "HOMUNCULUS", "SPECTRE", "ABYSSAL DEMON"
        };
        public EnemyHandler(ManualLogSource log)
        {
            _log = log;
            Harmony.CreateAndPatchAll(typeof(EnemyHandler));
        }

        [HarmonyPatch(typeof(Loot_scr), "OnEnable")]
        [HarmonyPrefix]
        private static bool OnEnable_ModifyDrops_Prefix(Loot_scr __instance)
        {
            if (ArchipelagoClient.AP.SlotData.Dropsanity == Dropsanity.Off)
            {
                return true;
            }
            float num = 0f;
            var lOOTS = __instance.LOOTS;
            //NameEveryDrop(__instance.name, lOOTS);
            var nothingChance = 0f;
            if (lOOTS.Any(x => x.ITEM is null))
            {
                nothingChance = lOOTS.First(x => x.ITEM is null).CHANCE;
            }
            var areDropsNormalized = ArchipelagoClient.AP.SlotData.NormalizedDrops;
            //_log.LogInfo($"Dealing with {__instance.name}");
            if (areDropsNormalized && lOOTS.Length - 1 > 0)
            {
                num += (float)2 * nothingChance;
            }
            else
            {
                for (int i = 0; i < lOOTS.Length; i++)
                {
                    var reward = lOOTS[i];
                    num += (float)reward.CHANCE;
                }
            }

            float num2 = UnityEngine.Random.Range(0f, num);
            int num3 = 0;
            for (int j = 0; j < lOOTS.Length; j++)
            {
                if (j == 0)
                {
                    num2 -= (float)lOOTS[j].CHANCE;
                    if (num2 <= 0f)
                    {
                        num3 = j;
                        break;
                    }
                    continue;
                }
                if (areDropsNormalized)
                {
                    num2 -= (float)nothingChance / (lOOTS.Length - 1);
                    if (num2 <= 0f)
                    {
                        num3 = j;
                        break;
                    }
                }
                else
                {
                    num2 -= (float)lOOTS[j].CHANCE;
                    if (num2 <= 0f)
                    {
                        num3 = j;
                        break;
                    }
                }
            }
            if (__instance.LOOTS[num3].ITEM == null)
            {
                return false;
            }
            var location = ConstructLocation(__instance.name, __instance.LOOTS[num3].ITEM.name);
            if (location == "FAIL" || location == "SETTING_DIFFERENCE" || location == "SAFE")
            {
                DropItemOnFloor(__instance.LOOTS[num3].ITEM, __instance.gameObject.transform.position);
                return false;
            }
            var locationData = GetDropLocationData(location);
            if (locationData.APLocationName == "ERROR")
            {
                _log.LogError($"Location {location} doesn't exist in Archipelago!");
                return false;
            }
            var itemName = ArchipelagoClient.AP.SendLocationGivenLocationDataSendingGift(locationData);
            return false;
        }

        private static void NameEveryDrop(string mobName, Loot_scr.Reward[] rewards)
        {
            foreach (var reward in rewards)
            {
                var name = "";
                if (reward.ITEM is null)
                {
                    name = "NULL";
                }
                else
                {
                    name = reward.ITEM.name;
                }
                _log.LogInfo($"{mobName} drops {name}");
            }
        }

        private static LocationData GetDropLocationData(string location)
        {
            foreach (var locationData in LunacidLocations.UniqueDropLocations)
            {
                if (locationData.APLocationName == location)
                {
                    return locationData;
                }
            }
            foreach (var locationData in LunacidLocations.OtherDropLocations)
            {
                if (locationData.APLocationName == location)
                {
                    return locationData;
                }
            }
            return new LocationData(0, "ERROR");
        }

        private static string ConstructLocation(string enemyObjectName, string itemObjectName)
        {
            if (!LunacidEnemies.NamesToGameObject.TryGetValue(enemyObjectName, out var enemyName))
            {
                _log.LogWarning($"Enemy {enemyObjectName} is not in the Dictionary.");
                return "FAIL";
            }
            if (!LunacidEnemies.ObjectToLocationSuffix.TryGetValue(itemObjectName, out var itemName))
            {
                _log.LogWarning($"Item {itemObjectName} is not in the Dictionary");
                return "FAIL";
            }
            if (ArchipelagoClient.AP.SlotData.Dropsanity == Dropsanity.Unique && !LunacidItems.Weapons.Contains(itemName) && !LunacidItems.Spells.Contains(itemName) && !LunacidItems.UniqueDrop.Contains(itemName))
            {
                _log.LogWarning($"Location {enemyName}: {itemName} Drop isn't in the current game's possible locations.  Giving item as normal.");
                return "SETTING_DIFFERENCE";
            }
            var constructedLocation = $"{enemyName}: {itemName} Drop";
            return constructedLocation;

        }

        private static void DropItemOnFloor(GameObject loot, Vector3 position)
        {
            GameObject obj = UnityEngine.Object.Instantiate(loot, position, Quaternion.identity);
            obj.SetActive(value: false);
            obj.AddComponent<Place_on_Ground>();
            obj.GetComponent<Place_on_Ground>().LOOTED = true;
            obj.SetActive(value: true);
        }

        [HarmonyPatch(typeof(NPC_SYS), "Start")]
        [HarmonyPrefix]
        private static bool NPC_SYS_InitializeEnemyPlacement(NPC_SYS __instance)
        {
            _log.LogInfo("Starting Enemy Shuffle...");
            if (__instance.FABS[0].name != "ABYSSAL DEMON")
            {
                return true;
            }
            if (!EnemyPrefabs.Any())
            {
                foreach (var item in __instance.FABS)
                {
                    if (barredEnemies.Contains(item.name))
                    {
                        continue;
                    }
                    EnemyPrefabs[item.name] = item;
                }
            }
            var scene = __instance.gameObject.scene.name;
            if (!LunacidEnemies.AllEnemyPositionData.TryGetValue(__instance.gameObject.scene.name, out var enemyPositionData))
            {
                return true;
            }
            var random = new System.Random(ConnectionData.Seed);
            EnemyPrefabKeys = EnemyPrefabs.Keys.ToList();
            foreach (var data in enemyPositionData)
            {
                var gameObjectWithChildren = GameObject.Find(LunacidEnemies.SceneToWorldObjectsName[scene]).transform;
                foreach (var childIndex in data.childPath)
                {
                    gameObjectWithChildren = gameObjectWithChildren.GetChild(childIndex);
                }
                foreach (var affectedChild in data.affectedChildren)
                {
                    var givenChild = gameObjectWithChildren.GetChild(affectedChild);
                    SwapEnemy(random, givenChild);
                }
            }
            return true;
        }

        public static void SwapEnemy(System.Random random, Transform child)
        {
            var chosenEnemyName = EnemyPrefabKeys[random.Next(0, EnemyPrefabKeys.Count)];
                var chosenEnemy = EnemyPrefabs[chosenEnemyName];
                var position = child.position;
                var rotation = child.rotation;
                var newEnemy = GameObject.Instantiate(chosenEnemy, position, rotation, parent: child.parent);
                newEnemy.transform.localScale = child.localScale;
                newEnemy.name = chosenEnemyName;
                var ai = newEnemy.GetComponent<AI_simple>();
                if (ai is not null)
                {
                    ai.BAR = child.GetComponent<AI_simple>().BAR;
                    ai.Level = child.GetComponent<AI_simple>().Level;
                }
                child.gameObject.SetActive(false);
        }
    }
}