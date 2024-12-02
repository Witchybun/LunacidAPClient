using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using BepInEx.Logging;
using HarmonyLib;
using LunacidAP.Data;
using UnityEngine;
using static LunacidAP.Data.LunacidEnemies;
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
        private static string magicCast = "MAGIC/CAST/";
        private static Transform EnemyCombatant { get; set; }
        public EnemyHandler(ManualLogSource log)
        {
            _log = log;
            Harmony.CreateAndPatchAll(typeof(EnemyHandler));
        }

        [HarmonyPatch(typeof(Loot_scr), "OnEnable")]
        [HarmonyPrefix]
        private static bool OnEnable_ModifyDrops_Prefix(Loot_scr __instance)
        {
            if ((ArchipelagoClient.AP.SlotData.Dropsanity == Dropsanity.Off && __instance.name == "SANGUIS UMBRA") || __instance.name == "CENTAUR" || __instance.gameObject.scene.name == "TOWER")
            {
                return false; // Third book is always in the pool, they drop nothing.  And Centaurs has only one drop which is null so don't even bother.
                // Also tower has no drops, so why run this.
            }
            if (ArchipelagoClient.AP.SlotData.Dropsanity == Dropsanity.Off)
            {
                return true;
            }
            if (ArchipelagoClient.AP.SlotData.Dropsanity == Dropsanity.Randomized)
            {
                AddFeatherToJailor(__instance);
            }
            var dropBoosts = ConnectionData.ReceivedItems.Where(x => x.Value.ItemName == "Text on Great Well Resourcefulness").Count();
            float nothingWeightScalar = (float)Math.Max(0.75, 0.25*dropBoosts);
            var lOOTS = __instance.LOOTS;
            //NameEveryDrop(__instance.name, lOOTS);
            var nothingWeight = 0f;
            var areDropsNormalized = ArchipelagoClient.AP.SlotData.NormalizedDrops;
            var normalizedNonemptyWeight = 0;
            for (int i = 0; i < lOOTS.Length; i++)
            {
                if (lOOTS[i].ITEM is null)
                {
                    nothingWeight = lOOTS[i].CHANCE;
                    continue;
                }
                normalizedNonemptyWeight += lOOTS[i].CHANCE;
            }
            var totalWeight = nothingWeight + normalizedNonemptyWeight;
            nothingWeight *= nothingWeightScalar;
            var otherDropScalar = (float)((1-nothingWeightScalar)*totalWeight/(totalWeight - nothingWeight) + nothingWeightScalar);
            normalizedNonemptyWeight = (int)(float)((totalWeight - nothingWeightScalar * nothingWeight) / (lOOTS.Length - 1));
            _log.LogInfo($"{nothingWeightScalar} with nothing weight {nothingWeight} vs {otherDropScalar} with {lOOTS.Length - 1} {normalizedNonemptyWeight}s.");
            int num3 = DetermineDrop(nothingWeight, normalizedNonemptyWeight, totalWeight, otherDropScalar, lOOTS, areDropsNormalized);
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

        private static void AddFeatherToJailor(Loot_scr lootInfo)
        {
            if (lootInfo.name != "JAILOR")
            {
                return;
            }
            var newLootData = new Loot_scr.Reward();
            newLootData.ITEM = new GameObject();
            newLootData.ITEM.name = "ANGEL_PICKUP";
            newLootData.CHANCE = 1;
            var loot = lootInfo.LOOTS.ToList();
            loot.Add(newLootData);
            lootInfo.LOOTS = loot.ToArray();
        }

        private static int DetermineDrop(float nothingChance, float normalizedWeight, float totalWeight, float dropScalar, Loot_scr.Reward[] possibleLoot, bool areDropsNormalized)
        {
            if (possibleLoot.Length == 1)
            {
                return 0; // It only has one drop.  Don't even calculate anything.
            }
            float num2 = UnityEngine.Random.Range(0f, totalWeight);
            int num3 = 0;
            for (int j = 0; j < possibleLoot.Length; j++)
            {
                if (possibleLoot[j].ITEM is null)
                {
                    num2 -= nothingChance;
                    if (num2 <= 0f)
                    {
                        num3 = j;
                        break;
                    }
                    else
                    {
                        continue;
                    }
                }
                if (areDropsNormalized)
                {
                    num2 -= normalizedWeight;
                    if (num2 <= 0f)
                    {
                        num3 = j;
                        break;
                    }
                }
                else
                {
                    num2 -= (float)possibleLoot[j].CHANCE * dropScalar;
                    if (num2 <= 0f)
                    {
                        num3 = j;
                        break;
                    }
                }
            }
            return num3;
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
            if (itemName == "Angelic Feather")
            {
                return "Item Blessed By the Angels";
            }
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
            if (!LunacidEnemies.BaseEnemyPositionData.TryGetValue(__instance.gameObject.scene.name, out var enemyPositionData))
            {
                return true;
            }
            var random = new System.Random(ConnectionData.Seed);
            EnemyPrefabKeys = EnemyPrefabs.Keys.ToList();
            foreach (var data in enemyPositionData)
            {
                var gameObjectWithChildren = GameObject.Find(LunacidEnemies.SceneToWorldObjectsName[scene]).transform;
                if (!ConnectionData.RandomEnemyData.TryGetValue(scene, out var enemyData))
                {
                    _log.LogInfo($"Scene {scene} is not in the enemy data dictionary.");
                    return true;
                }
                foreach (var childIndex in data.childPath)
                {
                    gameObjectWithChildren = gameObjectWithChildren.GetChild(childIndex);
                }
                foreach (var affectedChild in data.affectedChildren)
                {
                    var chosenEnemy = TryGetEnemy(data.groupName, affectedChild, enemyData, random);
                    var givenChild = gameObjectWithChildren.GetChild(affectedChild);
                    var isBlessed = false;
                    if (data.groupName == "MainPrison")
                    {
                        if (affectedChild == 14)
                        {
                            isBlessed = true;
                        }
                    }
                    SwapEnemy(givenChild, scene, chosenEnemy, isBlessed);
                }
            }
            return true;
        }

        private static string TryGetEnemy(string groupName, int child, List<RandomizedEnemyData> enemyData, System.Random random)
        {
            foreach (var enemy in enemyData)
            {
                if (enemy.enemyName == "Lunaga")
                {
                    continue; // remove later; they weren't in the list oops.
                }
                if (enemy.groupName == groupName && enemy.affectedChild == child)
                {
                    if (LunacidEnemies.APWorldNameToGameName.TryGetValue(enemy.enemyName, out var gameName))
                    {
                        return gameName;
                    }
                    var capitalizedName = enemy.enemyName.ToUpper();
                    if (enemy.enemyName == "Rat")
                    {
                        return "RAT2";
                    }
                    if (EnemyPrefabKeys.Contains(capitalizedName))
                    {
                        if (capitalizedName == "MUMMY")
                        {
                            capitalizedName = new List<string>() { "MUMMY", "MUMMY_CRAWLING" }[random.Next(2)];
                        }
                        else if (capitalizedName == "VENUS")
                        {
                            capitalizedName = new List<string>() { "VENUS", "VENUS_HIDE" }[random.Next(2)];
                        }

                        return capitalizedName;
                    }
                }
            }
            _log.LogError($"Could not find suitable enemy for {groupName}, {child}, returning NULL and keeping normal placement.");
            return "NULL";
        }

        public static void SwapEnemy(Transform child, string scene, string chosenEnemyName, bool isBlessed)
        {
            if (chosenEnemyName == "NULL")
            {
                return;
            }
            if (!EnemyPrefabs.TryGetValue(chosenEnemyName, out var chosenEnemy))
            {
                _log.LogError($"Could not find {chosenEnemyName} in prefab list!  Returning normal enemy.");
                return;
            }
            var position = child.position;
            var rotation = child.rotation;
            var newEnemy = GameObject.Instantiate(chosenEnemy, position, rotation, parent: child.parent);
            newEnemy.transform.localScale = child.localScale;
            newEnemy.name = newEnemy.name.Replace("(Clone)","");
            newEnemy.name = LunacidEnemies.CleanupName.Keys.Contains(newEnemy.name) ? LunacidEnemies.CleanupName[newEnemy.name] : newEnemy.name;
            var ai = newEnemy.GetComponent<AI_simple>();
            if (ai is not null)
            {
                var childai = child.GetComponent<AI_simple>();
                ai.BAR = childai.BAR;
                ai.health = Math.Max(ai.health, ai.health_max) * LunacidEnemies.SceneToAverageLevel[scene] / ai.Level;
                ai.health_max = ai.health;
                ai.Level = childai.Level; // keep the exp amounts of the old enemy.
            }
            if (isBlessed)
            {
                var angelFeather = child.GetComponent<Loot_scr>().LOOTS[6];
                var loots = newEnemy.GetComponent<Loot_scr>().LOOTS.ToList();
                loots.Add(angelFeather);
                newEnemy.GetComponent<Loot_scr>().LOOTS = loots.ToArray();
            }
            child.gameObject.SetActive(false);
        }

        [HarmonyPatch(typeof(Damage_Trigger), "Hurt")]
        [HarmonyPrefix]
        public static void Alter_Enemy_Damage_if_Random(Damage_Trigger __instance)
        {
            if (!ArchipelagoClient.AP.SlotData.EnemyRandomization)
            {
                return;
            }
            if (!__instance.EffectPlayer || !__instance.OnlyPL)
            {
                return;
            }
            if (!LunacidEnemies.SceneToWorldObjectsName.Keys.ToList().Contains(__instance.gameObject.scene.name))
            {
                return;
            }
            var enemy = __instance.transform;
            var enemySpell = enemy.name.Replace("(Clone)","");
            if (LunacidEnemies.EnemySpells.Contains(enemySpell))
            {
                enemy = EnemyCombatant;
            }
            else
            {
                while (!EnemyPrefabKeys.Contains(enemy.name))
                {
                    if (LunacidEnemies.WorldObjects.Contains(enemy.name))
                    {
                        _log.LogWarning($"Could not find enemy!");
                        return;
                    }
                    _log.LogInfo($"{enemy.name} wasn't the parent, continuing.");
                    enemy = enemy.parent;
                }
            }
            if (EnemyCombatant is null)
            {
                return;  // Its likely a spellcast which didn't hit the player.  Its fine.
            }
            var scene = __instance.gameObject.scene.name;
            var enemyLevel = enemy.GetComponent<AI_simple>().Level;
            __instance.power = Math.Min(150, Math.Max(10, __instance.power * (float)(LunacidEnemies.SceneToAverageLevel[scene] / enemyLevel)));
        }

        [HarmonyPatch(typeof(Spawn_on_enable), "OnEnable")]
        [HarmonyPostfix]
        private static void OnEnable_CollectSpellData(Spawn_on_enable __instance)
        {
            _log.LogInfo($"Spell Item: {__instance.item}");
            if (__instance.item == "MAGIC/CAST/SILK_SPIT_CAST")
            {
                return; //Its a Lunaga, they aren't shuffled.
            }
            GameObject castOverride = new();
            var castOverrideField = __instance.GetType().GetField("OVERRIDE", BindingFlags.Instance | BindingFlags.NonPublic);
            if (castOverrideField is not null)
            {
                castOverride = (GameObject) castOverrideField.GetValue(__instance);
            }
            var overrideName = "";
            if (castOverride.name != "" && __instance.item == "")
            {
                overrideName = castOverride.name.Replace("Clone", "");
            }
            if (overrideName == "")
            {
                return;
            }
            var castItemNoParents = __instance.item.Replace(magicCast, "");
            try
            {
                if (LunacidEnemies.EnemySpells.Contains(castItemNoParents) || LunacidEnemies.EnemySpells.Contains(overrideName))
                {
                    var enemy = __instance.transform;
                    var enemyName = LunacidEnemies.ModdedNameToPrefabName.Keys.Contains(enemy.name) ? LunacidEnemies.ModdedNameToPrefabName[enemy.name] : enemy.name;
                    while (!EnemyPrefabKeys.Contains(enemyName))
                    {
                        if (LunacidEnemies.WorldObjects.Contains(enemy.name))
                        {
                            _log.LogWarning($"Could not find enemy!");
                            return;
                        }
                        _log.LogInfo($"{enemy.name} wasn't the parent, continuing.");
                        enemy = enemy.parent;
                        enemyName = LunacidEnemies.ModdedNameToPrefabName.Keys.Contains(enemy.name) ? LunacidEnemies.ModdedNameToPrefabName[enemy.name] : enemy.name;
                    }
                    EnemyCombatant = enemy;
                }
            }
            catch
            {
                _log.LogWarning($"Could not find spell data for {__instance.name}");
            }
        }
    }
}