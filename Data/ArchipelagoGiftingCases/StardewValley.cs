using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;

namespace LunacidAP.Data.ArchipelagoGiftingCases
{
    public class StardewValleyGifts
    {
        private const string STARDEW_RESOURCE_PREFIX = "Resource Pack: ";
        private static readonly List<string> NonResourceWhitelist = new()
        {
            "Joja Cola", "Magic Rock Candy", "Golden Egg", "Taro Tuber", "Coffee Bean", "Tulib Bulb", "Mushroom Tree Seed", "Galaxy Soul", "Fairy Dust", 
            "Prize Ticket", "Mystery Box", "Tent Kit", "Dish O' The Sea", "Seamfoam Pudding", "Trap Bobber", "Treasure Chest", "Hero Elixir", "Aegis Elixir", 
            "Haste Elixir", "Lightning Elixir", "Armor Elixir", "Gravity Elixir", "Barbarian Elixir"
        };

        private static readonly List<string> SeedSuffices = new()
        {
            "Seeds", "Starter", "Sapling"
        };

        public static bool IsStardewItemGiftable(string itemName)
        {
            if (itemName.Contains(STARDEW_RESOURCE_PREFIX))
            {
                return true;
            }
            else if (NonResourceWhitelist.Contains(itemName))
            {
                return true;
            }
            else if (SeedSuffices.Contains(itemName.Split(' ').Last()))
            {
                return true;
            }
            return false;

        }

        public static string HandleStardewValleyItemCountAndName(string itemName, out int amount)
        {
            var actualName = itemName;
            amount = 1;
            if (itemName.Contains(STARDEW_RESOURCE_PREFIX))
            {
                var noResourcePrefixName = itemName.Replace(STARDEW_RESOURCE_PREFIX, "");
                var itemNameArray = noResourcePrefixName.Split(' ');
                amount = Math.Max(int.Parse(itemNameArray[0]) / 4, 1); // Its farmable, lets not give the other player too much.
                actualName = string.Join(" ", itemNameArray.Skip(1));
            }
            return actualName;
        }
    }
}