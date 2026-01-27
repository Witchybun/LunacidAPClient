using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Archipelago.MultiClient.Net.Enums;
using BepInEx;
using Newtonsoft.Json;

namespace LunacidAP.Data
{
    public static class ArchipelagoGames
    {
        public class APGameData
        {
            public readonly string Game;

            public readonly string Blurb;

            public Dictionary<string, string> Items;

            public APGameData(string game, string blurb, Dictionary<string, string> items)
            {
                Game = game;
                Blurb = blurb;
                Items = items;
            }
        }

        public static readonly Dictionary<string, APGameData> GameData = new()
        {
            {"Generic", new APGameData("Generic", "A mysterious item from another time and place.", new())}
        };

        public static void ConstructData()
        {
            var text = Path.Combine(Path.Combine(Paths.PluginPath, "LunacidAP"), "GamesData");
            if (!Directory.Exists(text))
            {
                Directory.CreateDirectory(text);
            }
            var dataFiles = Directory.GetFiles(text, "*.json");
            foreach (var file in dataFiles)
            {
                try
                {
                    var apGameData = JsonConvert.DeserializeObject<APGameData>(File.ReadAllText(file));
                    GameData[apGameData.Game] = apGameData;
                }
                catch (Exception ex)
                {
                    Plugin.LOG.LogError($"Could not parse data for {file}");
                    Plugin.LOG.LogError(ex.Message);
                }
            }
        }
        
        public static void ConstructNewGameData(string game)
        {
            var text = Path.Combine(Path.Combine(Paths.PluginPath, "LunacidAP"), "GamesData");
            if (!Directory.Exists(text))
            {
                Directory.CreateDirectory(text);
            }
            var text2 = Path.Combine(text, $"{game}.json");
            if (!File.Exists(text2))
            {
                return;
            }
            var apGameData = new APGameData(game, "An item from an unknown original, not yet documented", new Dictionary<string, string>());
            var text3 = JsonConvert.SerializeObject(apGameData);
            File.WriteAllText(text2, text3);
            Plugin.LOG.LogInfo($"Created new data entry for {game}.");
            return;
        }
        
        public static string KeywordToItem(ArchipelagoItem archipelagoItem)
        {
            var itemName = archipelagoItem.Name;
            var random = new Random(itemName.GetHashCode());
            if (itemName.Contains("Key") || itemName.Contains("Red key") || itemName.Contains("Blue key") || itemName.Contains("Yellow key") ||
            itemName.Contains("Green key"))
            {
                return "Enchanted Key";
            }
            else if (itemName.Contains("Emerald") || itemName.Contains("Diamond") || itemName.Contains("Sapphire"))
            {
                return "Wisp Heart";
            }
            else if (itemName.Contains("Ruby") || itemName.Contains("Crystal"))
            {
                return "Crystal Shard";
            }
            else if (itemName.Contains("Feather"))
            {
                return "Angel Feather";
            }
            else if (itemName.Contains("Rupees") || itemName.Contains("Money") || itemName.Contains("Soul") || 
                    itemName.Contains("Geo") || itemName.Contains("Bones") || itemName.Contains("Coin"))
            {
                return "Silver";
            }
            else if (itemName.Contains("Fishing Rod"))
            {
                return "Fishing Spear";
            }
            else if (itemName.Contains("Sword") || itemName.Contains("Dagger") || itemName.Contains("Blade") || itemName.Contains("Knife"))
            {
                return LunacidItems.Swords[random.Next(0, LunacidItems.Swords.Count)];
            }
            else if (itemName.Contains("Spear") || itemName.Contains("Lance") || itemName.Contains("Greatlance"))
            {
                return LunacidItems.Spears[random.Next(0, LunacidItems.Spears.Count)];
            }
            else if (itemName.Contains("Axe") || itemName.Contains("Greataxe"))
            {
                return LunacidItems.Axes[random.Next(0, LunacidItems.Axes.Count)];
            }
            else if (itemName.Contains("Bow") || itemName.Contains("Crossbow") || 
            itemName.Contains("Gun") || itemName.Contains("Launcher") || itemName.Contains("Shotgun") || itemName.Contains("Chaingun")
            )
            {
                return LunacidItems.Bows[random.Next(0, LunacidItems.Axes.Count)];
            }
            else if (itemName.Contains("Bomb") || itemName.Contains("Grenade") || itemName.Contains("Rocket"))
            {
                return "Bomb";
            }
            else if (itemName.Contains("Glove") || itemName.Contains("Strength") || itemName.Contains("Bracelet"))
            {
                return LunacidItems.Gloves[random.Next(0, LunacidItems.Gloves.Count)];
            }
            else if (itemName.Contains("Shield") || itemName.Contains("Aegis") || itemName.Contains("Parma"))
            {
                return LunacidItems.Shields[random.Next(0, LunacidItems.Shields.Count)];
            }
            else if (itemName.Contains("Staff") || itemName.Contains("Rod") || itemName.Contains("Wand"))
            {
                return new List<string>(){"Twisted Staff", "Wand of Power"}[random.Next(2)];
            }
            else if (itemName.Contains("Magic Meter") || itemName.Contains("Heart Container") || itemName.Contains("Energy Tank"))
            {
                return "Earth Elixir";
            }
            else if (itemName.Contains("Magic") || itemName.Contains("Spell") || itemName.Contains("Orb") || itemName.Contains("TM") ||
            itemName.Contains("HM"))
            {
                return "Flame Flare";
            }
            else if (itemName.Contains("Song") || itemName.Contains("Book") || itemName.Contains("Tome"))
            {
                return "Black Book";
            }
            else if (itemName.Contains("Bottle") || itemName.Contains("Potion") || itemName.Contains("Flask"))
            {
                return "Holy Water";
            }
            else if (archipelagoItem.Classification == ItemFlags.None)
            {
                return "Ashes";
            }
            
            return "NULL";

        }
    }
}