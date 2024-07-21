using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Archipelago.Gifting.Net.Gifts.Versions.Current;
using Archipelago.MultiClient.Net.Enums;
using BepInEx.Logging;
using HarmonyLib;
using LunacidAP.Data;
using UnityEngine;
using UnityEngine.EventSystems;

namespace LunacidAP
{
    public class ItemHandler
    {
        private static CONTROL Control;
        private static POP_text_scr Popup;
        private static LogicHelper _lunacidLogic;
        private static ManualLogSource _log;

        public ItemHandler(LogicHelper lunacidLogic, ManualLogSource log)
        {
            _log = log;
            _lunacidLogic = lunacidLogic;
            Harmony.CreateAndPatchAll(typeof(ItemHandler));
        }

        [HarmonyPatch(typeof(Menus), "ItemLoad")]
        [HarmonyPrefix]
        private static bool ItemLoad_CreateFakeKeys(Menus __instance)
        {
            if (__instance.sub_menu != 6)
            {
                return true;
            }
            var eqSelField = __instance.GetType().GetField("EQ_SEL", BindingFlags.Instance | BindingFlags.NonPublic);
            int EQ_SEL = (int)eqSelField.GetValue(__instance);
            __instance.TXT[9].GetComponent<Animation>().Play();
            __instance.TXT[10].GetComponent<Animation>().Play();
            __instance.TXT[11].GetComponent<Animation>().Play();
            int num = EQ_SEL = int.Parse(EventSystem.current.currentSelectedGameObject.name.Substring(3));
            if (StaticFuncs.IS_NULL(__instance.CON.CURRENT_PL_DATA.ITEMS[num]))
            {
                return true;
            }
            var keyItems = new List<string>() { "Great Well Doors Keyring", "Great Well Switches Keyring", "Orb of a Lost Archipelago" };
            if (!keyItems.Contains(StaticFuncs.REMOVE_NUMS(__instance.CON.CURRENT_PL_DATA.ITEMS[num])))
            {
                return true;
            }
            int num3 = int.Parse(__instance.CON.CURRENT_PL_DATA.ITEMS[num].Substring(__instance.CON.CURRENT_PL_DATA.ITEMS[num].Length - 2, 2));
            string itemName = StaticFuncs.REMOVE_NUMS(__instance.CON.CURRENT_PL_DATA.ITEMS[num]);
            if (itemName == "Great Well Doors Keyring")
            {
                CreateDoorKeyInInventory(__instance, num3);
            }
            else if (itemName == "Great Well Switches Keyring")
            {
                CreateSwitchKeyInInventory(__instance, num3);
            }
            else if (itemName == "Orb of a Lost Archipelago")
            {
                CreateOrbInInventory(__instance, num3);
            }
            return false;
        }

        public static void GiveLunacidItem(string itemName, ItemFlags itemFlag, string player = "", bool self = false, string overrideColor = "")
        {
            
            if (!FlagHandler.DoesPlayerHaveItem("Orb of a Lost Archipelago"))
            {
                Control = GameObject.Find("CONTROL").GetComponent<CONTROL>();
                ApplyItemToInventory("Orb of a Lost Archipelago");
            }
            string color = Colors.DetermineItemColor(itemFlag);
            if (overrideColor != "")
            {
                color = overrideColor;
            }
            itemName = ProgressiveSymbolHandler(itemName);
            if (LunacidFlags.ItemToFlag.Keys.Contains(itemName))
            {
                FlagHandler.HandleFlagGivenItemName(itemName);
            }
            var type = IdentifyItemGetType(itemName);
            if (type == 1 || type == 2)
            {
                itemName = itemName.ToUpper();
            }if (LunacidItems.FakeItems.Contains(itemName))
            {
                PopupCommand(0, itemName, color, player, self);
                return;
            }
            switch (type)
            {
                case 1:
                    {
                        GiveWeapon(itemName, color, player, self);
                        return;
                    }
                case 2:
                    {
                        GiveSpell(itemName, color, player, self);
                        return;
                    }
                case 0:
                    {
                        GiveSilver(itemName, player, self);
                        return;
                    }
                case 4:
                    {
                        GiveItem(itemName, color, player, self);
                        return;
                    }
                case 3:
                    {
                        GiveMaterial(itemName, color, player, self);
                        return;
                    }
                case -1:
                    {
                        GiveSwitch(itemName, color, player, self);
                        return;
                    }
                case -2:
                    {
                        GiveDoor(itemName, color, player, self);
                        return;
                    }
            }
            if (itemName.Contains(" Trap"))
            {
                GiveTrap(itemName, color, player, self);
                return;
            }
            _log.LogError($"Supplied item {itemName} was not caught by any of the given cases");
        }

        public static void GiveLunacidItem(long itemID, ItemFlags itemFlag, string player = "", bool self = false)
        {
            string Name = ArchipelagoClient.AP.Session.Items.GetItemName(itemID);
            GiveLunacidItem(Name, itemFlag, player, self);
        }

        public static void GiveLunacidItem(ReceivedItem receivedItem, bool self, bool isCheated)
        {
            if (isCheated)
            {
                GiveLunacidItem(receivedItem.ItemName, receivedItem.Classification, receivedItem.PlayerName, self, Colors.GetCheatColor());
            }
            else
            {
                GiveLunacidItem(receivedItem.ItemId, receivedItem.Classification, receivedItem.PlayerName, self);
            }
            ConnectionData.ReceivedItems.Add(receivedItem);
        }

        public static void GiveLunacidItem(Gift gift, bool isTrap)
        {
            var playerName = ArchipelagoClient.AP.GetPlayerNameFromSlot(gift.SenderSlot);
            if (isTrap)
            {
                GiveLunacidItem(gift.ItemName, ItemFlags.Trap, playerName, false);
                return;
            }
            GiveLunacidItem(gift.ItemName, ItemFlags.None, playerName, false);
        }

        private static void PopupCommand(int sprite, string Name, string color, string player, bool self)
        {
            Control = GameObject.Find("CONTROL").GetComponent<CONTROL>();
            Popup = Control.PAPPY;
            if (self)
                Popup?.POP($"<color={color}>{Name}</color> <sprite={sprite}> ACQUIRED", 1f, 0);
            else
                Popup?.POP($"<color={color}>{Name}</color> <sprite={sprite}> RECEIVED FROM {player}", 1f, 0);
        }

        private static void GiveWeapon(string Name, string color, string player = "", bool self = false)
        {
            try
            {
                PopupCommand(1, Name, color, player, self);
                Name = Name.Replace("JAILOR'S", "JAILORS");
                for (int j = 0; j < 128; j++)
                {
                    if (Control.CURRENT_PL_DATA.WEPS[j] == "" || Control.CURRENT_PL_DATA.WEPS[j] == null || StaticFuncs.REMOVE_NUMS(Control.CURRENT_PL_DATA.WEPS[j]) == Name)
                    {
                        Control.CURRENT_PL_DATA.WEPS[j] = Name;
                        j = 999;
                    }
                }
            }
            catch (Exception ex)
            {
                _log.LogError($"Method {nameof(GiveWeapon)} failed.");
                _log.LogError($"Reason: {ex.Message}");
            }



        }

        private static void GiveSpell(string Name, string color, string player = "", bool self = false)
        {
            try
            {
                PopupCommand(2, Name, color, player, self);
                for (int k = 0; k < 128; k++)
                {
                    if (Control.CURRENT_PL_DATA.SPELLS[k] == "" || Control.CURRENT_PL_DATA.SPELLS[k] == null || StaticFuncs.REMOVE_NUMS(Control.CURRENT_PL_DATA.SPELLS[k]) == Name)
                    {
                        Control.CURRENT_PL_DATA.SPELLS[k] = Name;
                        k = 999;
                    }
                }

            }
            catch (Exception ex)
            {
                _log.LogInfo($"Failure in {nameof(GiveSpell)}.");
                _log.LogInfo($"Reason: {ex.Message}");
            }

        }

        private static void GiveSilver(string Name, string player = "", bool self = false)
        {
            var currencyMultiplier = DetermineRandomAmount(Name);
            var currencyAmount = (10 * currencyMultiplier).ToString();
            PopupCommand(0, currencyAmount, "white", player, self);
            Control.CURRENT_PL_DATA.GOLD += int.Parse(currencyAmount);
        }

        private static void GiveItem(string Name, string color, string player = "", bool self = false)
        {
            PopupCommand(4, Name, color, player, self);
            ApplyItemToInventory(Name);
        }

        private static void ApplyItemToInventory(string Name)
        {
            if (ArchipelagoClient.AP is null)
            {
                _log.LogError("Archipelago is null!");
            }
            if (ArchipelagoClient.AP.SlotData is null)
            {
                _log.LogError("SlotData is null!");
            }
            var bundleSize = DetermineRandomAmount(Name);
            if (LunacidItems.OneCountItems.Contains(Name))
            {
                bundleSize = 1;
            }
            if (Name == "Shrimp")
            {
                Name = "Pink Shrimp";
            }
            if (Name == "Poison Throwing Knife" || Name == "Throwing Knife" || Name == "Survey Banner")
            {
                bundleSize = Math.Min(10, bundleSize * 4); // Game normally boosts these anyway
            }
            if (Name == "Poison Urn" || Name == "Staff of Osiris")
            {
                bundleSize = Math.Min(10, bundleSize * 2); //Same deal, but not as much
            }
            Useable_Item[] eQ_ITEMS = Control.EQ_ITEMS;
            foreach (Useable_Item useable_Item in eQ_ITEMS)
            {
                if (!(useable_Item.ITEM_NAME == Name.Replace(" ", "_")))
                {
                    continue;
                }
                if (useable_Item.Count > 100 - bundleSize)
                {
                    return;
                }
                useable_Item.Count += 1 + bundleSize;
                useable_Item.TakeOne();
                useable_Item.CON.ITM_SLOT--;
                useable_Item.CON.SendMessage("OnNextITEM");
                return;
            }
            for (int m = 0; m < 128; m++)
            {
                if (Control.CURRENT_PL_DATA.ITEMS[m] == "" || Control.CURRENT_PL_DATA.ITEMS[m] == null)
                {
                    Control.CURRENT_PL_DATA.ITEMS[m] = Name + ValueSuffix(bundleSize);
                    m = 999;
                }
                else
                {
                    if (!(StaticFuncs.REMOVE_NUMS(Control.CURRENT_PL_DATA.ITEMS[m]) == Name))
                    {
                        continue;
                    }
                    int num2 = int.Parse(Control.CURRENT_PL_DATA.ITEMS[m].Substring(Control.CURRENT_PL_DATA.ITEMS[m].Length - 2, 2));
                    num2 += bundleSize;
                    if (num2 > 100 - bundleSize)
                    {
                        m = 999;
                        continue;
                    }
                    if (num2 < 10)
                    {
                        Control.CURRENT_PL_DATA.ITEMS[m] = Name + "0" + num2;
                    }
                    else
                    {
                        Control.CURRENT_PL_DATA.ITEMS[m] = Name + num2;
                    }
                    m = 999;
                }
            }
        }

        private static void GiveMaterial(string Name, string color, string player = "", bool self = false)
        {
            if (!LunacidItems.MaterialNames.Keys.Contains(Name))
            {
                return;
            }
            var bundleSize = DetermineRandomAmount(Name);
            if (LunacidItems.OneCountItems.Contains(Name))
            {
                bundleSize = 1;
            }
            var actualName = LunacidItems.MaterialNames[Name].ToString();
            PopupCommand(3, Name, color, player, self);
            for (int i = 0; i < 128; i++)
            {
                if (Control.CURRENT_PL_DATA.MATER[i] == "" || Control.CURRENT_PL_DATA.MATER[i] == null)
                {
                    Control.CURRENT_PL_DATA.MATER[i] = actualName + ValueSuffix(bundleSize);  // Used Alt_Name previously
                    i = 999;
                }
                else
                {
                    if (!(Control.CURRENT_PL_DATA.MATER[i].Substring(0, Control.CURRENT_PL_DATA.MATER[i].Length - 2) == actualName))  // Used Alt_Name previously
                    {
                        continue;
                    }
                    int num = int.Parse(Control.CURRENT_PL_DATA.MATER[i].Substring(Control.CURRENT_PL_DATA.MATER[i].Length - 2, 2));
                    num += bundleSize;
                    if (num > 100 - bundleSize)
                    {
                        i = 999;
                        continue;
                    }
                    if (num < 10)
                    {
                        Control.CURRENT_PL_DATA.MATER[i] = actualName + "0" + num;  // Used Alt_Name previously
                    }
                    else
                    {
                        Control.CURRENT_PL_DATA.MATER[i] = actualName + num;  // Used Alt_Name previously
                    }
                    i = 999;
                }
            }
        }

        private static void GiveSwitch(string Name, string color, string player = "", bool self = false)
        {
            PopupCommand(4, Name, color, player, self);
            if (!FlagHandler.DoesPlayerHaveItem("Great Well Switches Keyring"))
            {
                ApplyItemToInventory("Great Well Switches Keyring");
            }
        }

        private static int DetermineRandomAmount(string Name)
        {
            var random = new System.Random(Name.GetHashCode() + DateTime.Now.Day + DateTime.Now.Hour + DateTime.Now.Minute);
            var chosenValue = random.Next(0, 100);
            if (chosenValue < 30)
            {
                return 1;
            }
            else if (chosenValue < 60)
            {
                return 2;
            }
            else if (chosenValue < 80)
            {
                return 3;
            }
            else if (chosenValue < 90)
            {
                return 4;
            }
            return 5;
        }

        private static void GiveDoor(string Name, string color, string player = "", bool self = false)
        {
            PopupCommand(4, Name, color, player, self);
            if (!FlagHandler.DoesPlayerHaveItem("Great Well Doors Keyring"))
            {
                ApplyItemToInventory("Great Well Doors Keyring");
            }
        }

        private static void GiveTrap(string Name, string color, string player = "", bool self = false)
        {
            PopupCommand(4, Name, color, player, self);
            Control.PL.Poison.Harm(LunacidItems.TrapToHarm[Name], 10.0f);
        }

        private static void CreateDoorKeyInInventory(Menus menu, int itemCount)
        {
            GameObject obj = UnityEngine.Object.Instantiate(Resources.Load("ITEMS/" + "Terminus Prison Key")) as GameObject;
            Useable_Item component6 = obj.GetComponent<Useable_Item>();
            var description = "<size=80%>A ring with the following keys on it:</size>\n";
            var rightKeys = new List<string>() { };
            var leftKeys = new List<string>() { };
            var rightSide = "";
            var leftSide = "";
            foreach (var key in LunacidItems.Keys)
            {
                if (ArchipelagoClient.AP.WasItemReceived(key))
                {
                    rightKeys.Add(key);
                }
            }
            if (rightKeys.Count > 9)
            {
                for (var i = 0; i < 10; i++)
                {
                    leftKeys.Add(rightKeys[0]);
                    rightKeys.RemoveAt(0);
                }
            }
            foreach (var key in rightKeys)
            {
                rightSide += $"<size=60%><align=center>{key}</align></size>\n";
            }
            if (leftKeys.Count > 0)
            {
                foreach (var key in leftKeys)
                {
                    leftSide += $"<size=40%><align=center>{key.ToUpper()}</align></size>\n";
                }
            }
            menu.TXT[32].text = "Great Well Doors Keyring";
            menu.TXT[33].text = itemCount.ToString() + $"\n<size=35%>\n</size>{leftSide}";
            menu.TXT[34].text = description + rightSide;
            menu.TXT[32].transform.GetChild(0).gameObject.SetActive(leftKeys.Count == 0);
            menu.TXT[32].transform.GetChild(1).gameObject.SetActive(leftKeys.Count == 0);
            if (leftKeys.Count == 0)
            {
                menu.TXT[32].transform.GetChild(1).GetComponent<UnityEngine.UI.Image>().sprite = component6.SPR;
            }
            UnityEngine.Object.Destroy(obj);
            var doubleClick = menu.GetType().GetMethod("DoubleClick", BindingFlags.Instance | BindingFlags.NonPublic);
            if ((bool)doubleClick.Invoke(menu, null))
            {
                var click = menu.GetType().GetMethod("Click", BindingFlags.Instance | BindingFlags.NonPublic);
                click.Invoke(menu, new object[1] { 24 });
            }
        }

        private static void CreateSwitchKeyInInventory(Menus menu, int itemCount)
        {
            GameObject obj = UnityEngine.Object.Instantiate(Resources.Load("ITEMS/" + "Terminus Prison Key")) as GameObject;
            Useable_Item component6 = obj.GetComponent<Useable_Item>();
            var description = "<size=80%>A ring with the following keys on it:</size>\n";
            foreach (var key in LunacidItems.Switches)
            {
                if (ArchipelagoClient.AP.WasItemReceived(key))
                {
                    description += $"<size=60%><align=center> {key} </align></size>\n";
                }
            }
            menu.TXT[32].text = "Great Well Switches Keyring";
            menu.TXT[33].text = itemCount.ToString();
            menu.TXT[34].text = description;
            menu.TXT[32].transform.GetChild(0).gameObject.SetActive(value: true);
            menu.TXT[32].transform.GetChild(1).gameObject.SetActive(value: true);
            menu.TXT[32].transform.GetChild(1).GetComponent<UnityEngine.UI.Image>().sprite = component6.SPR;
            UnityEngine.Object.Destroy(obj);
            var doubleClick = menu.GetType().GetMethod("DoubleClick", BindingFlags.Instance | BindingFlags.NonPublic);
            if ((bool)doubleClick.Invoke(menu, null))
            {
                var click = menu.GetType().GetMethod("Click", BindingFlags.Instance | BindingFlags.NonPublic);
                click.Invoke(menu, new object[1] { 24 });
            }
        }

        private static void CreateOrbInInventory(Menus menu, int itemCount)
        {
            GameObject obj = UnityEngine.Object.Instantiate(Resources.Load("ITEMS/" + "Dusty Crystal Orb")) as GameObject;
            var sceneName = menu.gameObject.scene.name;
            Useable_Item component6 = obj.GetComponent<Useable_Item>();
            var description = "<size=80%>An orb with several words floating within, as semblances of colorful smoke.</size>\n";
            var rightsideLocations = new List<string>() { };
            if (LunacidLocations.APLocationData.Keys.Contains(sceneName))
            {
                foreach (var locationData in LunacidLocations.APLocationData[sceneName])
                {
                    if (!ArchipelagoClient.AP.IsLocationChecked(locationData.APLocationID))
                    {
                        rightsideLocations.Add(locationData.APLocationName);
                    }
                }
            }
            if (ArchipelagoClient.AP.SlotData.Dropsanity != Dropsanity.Off)
            {
                foreach (var locationData in LunacidLocations.UniqueDropLocations)
                {
                    var enemyName = locationData.APLocationName.Split(':')[0];
                    _log.LogInfo($"Looking at {enemyName}");
                    if (LunacidEnemies.EnemyToScenes[enemyName].Contains(sceneName))
                    {
                        if (!ArchipelagoClient.AP.IsLocationChecked(locationData.APLocationID))
                        {
                            rightsideLocations.Add(locationData.APLocationName);
                        }
                    }
                }
            }
            if (ArchipelagoClient.AP.SlotData.Dropsanity == Dropsanity.Randomized)
            {
                foreach (var locationData in LunacidLocations.OtherDropLocations)
                {
                    var enemyName = locationData.APLocationName.Split(':')[0];
                    if (!LunacidEnemies.EnemyToScenes.TryGetValue(enemyName, out var sceneList))
                    {
                        _log.LogWarning($"Couldn't find {locationData.APLocationName} in multiworld, continuing.");
                        continue;
                    }
                    if (sceneList.Contains(sceneName))
                    {
                        if (!ArchipelagoClient.AP.IsLocationChecked(locationData.APLocationID))
                        {
                            rightsideLocations.Add(locationData.APLocationName);
                        }
                    }
                }
            }
            var shopScenes = new List<string>() { "HUB_01", "FOREST_A1" };
            if (ArchipelagoClient.AP.SlotData.Shopsanity && shopScenes.Contains(sceneName))
            {
                if (sceneName == "FOREST_A1")
                {
                    var locationID = ArchipelagoClient.AP.GetLocationIDFromName("Buy Ocean Elixir (Patchouli)");
                    if (!ArchipelagoClient.AP.IsLocationChecked(locationID))
                    {
                        rightsideLocations.Add("Buy Ocean Elixir (Patchouli)");
                    }
                }
                else
                {
                    foreach (var locationData in LunacidLocations.ShopLocations)
                    {
                        if (locationData.APLocationName != "Buy Ocean Elixir (Patchouli)" &&
                        !ArchipelagoClient.AP.IsLocationChecked(locationData.APLocationID))
                        {
                            rightsideLocations.Add(locationData.APLocationName);
                        }
                    }
                }

            }
            var random = new System.Random(DateTime.Today.Day + DateTime.Today.Hour + DateTime.Today.Minute);
            rightsideLocations = rightsideLocations.OrderBy(_ => random.Next()).ToList();
            var leftsideLocations = new List<string>() { };
            var wasListTooBig = false;

            if (rightsideLocations.Count() == 0)
            {
                description = "<size=80%>A multi-colored orb, clear as glass.  Once, words danced upon its surface, but now nothing can be seen...</size>";
            }
            else if (rightsideLocations.Count > 14)
            {
                if (rightsideLocations.Count > 25)
                {
                    wasListTooBig = true;
                    while(rightsideLocations.Count> 25)
                    {
                        rightsideLocations.RemoveAt(0);
                    }
                }
                while (rightsideLocations.Count > 14)
                {
                    leftsideLocations.Add(rightsideLocations[0]);
                    rightsideLocations.RemoveAt(0);
                }
            }
            var leftsideText = "";
            var rightsideText = "";
            foreach (var location in rightsideLocations)
            {
                rightsideText += $"<size=55%><align=center><color={_lunacidLogic.ColorLogicLocation(location, sceneName)}>{location}</color></align></size>\n";
            }
            if (leftsideLocations.Count > 0)
            {
                foreach (var location in leftsideLocations)
                {
                    leftsideText += $"<size=37%><align=center><color={_lunacidLogic.ColorLogicLocation(location, sceneName)}>{location.ToUpper()}</color></align></size>\n";
                }
            }
            if (wasListTooBig)
            {
                leftsideText += $"<size=50%>THE OTHER WORDS BLEND TOGETHER...</size>\n";
            }
            var inventoryItem = LunacidLocations.sceneToArea.Keys.Contains(sceneName) ? LunacidLocations.sceneToArea[sceneName] : "a Lost Archipelago";
            menu.TXT[32].text = $"Orb of {inventoryItem}";
            menu.TXT[33].text = itemCount.ToString() + "\n<size=35%>\n</size>" + leftsideText;
            menu.TXT[34].text = description + rightsideText;
            menu.TXT[32].transform.GetChild(0).gameObject.SetActive(leftsideLocations.Count == 0);
            menu.TXT[32].transform.GetChild(1).gameObject.SetActive(leftsideLocations.Count == 0);
            if (leftsideLocations.Count == 0)
            {
                menu.TXT[32].transform.GetChild(1).GetComponent<UnityEngine.UI.Image>().sprite = component6.SPR;
            }
            UnityEngine.Object.Destroy(obj);
            var doubleClick = menu.GetType().GetMethod("DoubleClick", BindingFlags.Instance | BindingFlags.NonPublic);
            if ((bool)doubleClick.Invoke(menu, null))
            {
                var click = menu.GetType().GetMethod("Click", BindingFlags.Instance | BindingFlags.NonPublic);
                click.Invoke(menu, new object[1] { 24 });
            }
        }

        private static string ValueSuffix(int value)
        {
            if (value > 9)
            {
                return value.ToString();
            }
            return "0" + value.ToString();
        }

        private static string ProgressiveSymbolHandler(string Name)
        {
            try
            {
                if (Name != "Progressive Vampiric Symbol")
                {
                    return Name;
                }
                if (!FlagHandler.DoesPlayerHaveItem("Vampiric Symbol (W)"))
                {
                    return "Vampiric Symbol (W)";

                }
                else if (!FlagHandler.DoesPlayerHaveItem("Vampiric Symbol (A)"))
                {
                    return "Vampiric Symbol (A)";
                }
                else if (!FlagHandler.DoesPlayerHaveItem("Vampiric Symbol (E)"))
                {
                    return "Vampiric Symbol (E)";
                }
                return Name;
            }
            catch (Exception ex)
            {
                _log.LogError($"{ex.Message}");
                return Name;
            }
        }

        public static int IdentifyItemGetType(string name)
        {
            if (LunacidItems.Items.Contains(name))
            {
                return 4;
            }
            else if (LunacidItems.Materials.Contains(name))
            {
                return 3;
            }
            else if (LunacidItems.Weapons.Contains(name))
            {
                return 1;
            }
            else if (LunacidItems.Spells.Contains(name))
            {
                return 2;
            }
            else if (name == "Silver" || name == "Silver (10)")
            {
                return 0;
            }
            else if (LunacidItems.Switches.Contains(name))
            {
                return -1;
            }
            else if (LunacidItems.Keys.Contains(name))
            {
                return -2;
            }
            return -3;
        }
    }
}