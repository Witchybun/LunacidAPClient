using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using BepInEx.Logging;
using HarmonyLib;
using LunacidAP.Data;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace LunacidAP
{
    public class ItemHandler
    {
        private static CONTROL Control;
        private static POP_text_scr Popup;
        private static ManualLogSource _log;

        public static void Awake(ManualLogSource log)
        {
            _log = log;
            Harmony.CreateAndPatchAll(typeof(ItemHandler));
        }
        public static void GiveLunacidItem(long itemID, string player = "", bool self = false)
        {
            string Name = ArchipelagoClient.AP.Session.Items.GetItemName(itemID);
            if (LunacidFlags.ItemToFlag.Keys.Contains(Name))
            {
                ApplyFlag(Name);
                if (Name == "Progressive Vampiric Symbol")
                {
                    Name = ProgressiveSymbolHandler(Name);
                }
            }
            var type = IdentifyItemGetType(Name);
            if (type == 1 || type == 2)
            {
                Name = Name.ToUpper();
            }
            switch (type)
            {
                case 1:
                    {
                        GiveWeapon(Name, player, self);
                        return;
                    }
                case 2:
                    {
                        GiveSpell(Name, player, self);
                        return;
                    }
                case 0:
                    {
                        GiveSilver(Name, player, self);
                        return;
                    }
                case 4:
                    {
                        GiveItem(Name, player, self);
                        return;
                    }
                case 3:
                    {
                        GiveMaterial(Name, player, self);
                        return;
                    }
            }
            if (Name.Contains(" Switch") || Name.Contains("Lightning Gate"))
            {
                GiveSwitch(Name, player, self);
                return;
            }
            if (Name.Contains(" Trap"))
            {
                GiveTrap(Name, player, self);
                return;
            }
            _log.LogError($"Supplied item {Name} was not caught by any of the given cases");
        }

        public static void GiveLunacidItem(ReceivedItem receivedItem, bool self)
        {
            GiveLunacidItem(receivedItem.ItemId, receivedItem.PlayerName, self);
            ConnectionData.ReceivedItems.Add(receivedItem);
        }

        private static void PopupCommand(int sprite, string Name, string player, bool self)
        {
            Control = GameObject.Find("CONTROL").GetComponent<CONTROL>();
            Popup = Control.PAPPY;
            if (self)
                Popup?.POP(Name + $"<sprite={sprite}> ACQUIRED", 1f, 0);
            else
                Popup?.POP(Name + $"<sprite={sprite}> RECEIVED FROM {player}", 1f, 0);
        }

        private static void GiveWeapon(string Name, string player = "", bool self = false)
        {
            try
            {
                PopupCommand(1, Name, player, self);
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
                _log.LogInfo($"Method {nameof(GiveWeapon)} failed.");
                _log.LogInfo($"Reason: {ex.Message}");
            }



        }

        private static void GiveSpell(string Name, string player = "", bool self = false)
        {
            try
            {
                PopupCommand(2, Name, player, self);
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

            var currencyAmount = Name.Split('(', ')')[1];
            var currencyMultiplier = SlotData.Fillerbundle;
            currencyAmount = (int.Parse(currencyAmount) * currencyMultiplier).ToString();
            PopupCommand(0, currencyAmount, player, self);
            Control.CURRENT_PL_DATA.GOLD += int.Parse(currencyAmount);
        }

        private static void GiveItem(string Name, string player = "", bool self = false)
        {
            PopupCommand(4, Name, player, self);
            var bundleSize = SlotData.Fillerbundle;
            if (Name == "Strange Coin")
            {
                bundleSize = SlotData.Coinbundle;
            }
            if (LunacidItems.OneCountItems.Contains(Name))
            {
                bundleSize = 1;
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

        private static void GiveMaterial(string Name, string player = "", bool self = false)
        {
            if (!LunacidItems.MaterialNames.Keys.Contains(Name))
            {
                _log.LogInfo($"MaterialNames has no definition for {Name}; cannot give it!");
                return;
            }
            var bundleSize = SlotData.Fillerbundle;
            if (LunacidItems.OneCountItems.Contains(Name))
            {
                bundleSize = 1;
            }
            var actualName = LunacidItems.MaterialNames[Name].ToString();
            PopupCommand(3, Name, player, self);
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

        private static void GiveSwitch(string Name, string player = "", bool self = false)
        {
            PopupCommand(4, Name, player, self);
        }

        private static void GiveTrap(string Name, string player = "", bool self = false)
        {
            PopupCommand(4, Name, player, self);
            Control.PL.Poison.Harm(LunacidItems.TrapToHarm[Name], 10.0f);
        }

        // This is a temporary patch to fix a vanilla bug.
        [HarmonyPatch(typeof(Player_Poison), "Harm")]
        [HarmonyPrefix]
        private static bool Harm_FixSlownessBug(int type, float duration, Player_Poison __instance)
        {
            __instance.IMG.gameObject.GetComponent<AspectRatioFitter>().aspectRatio = Camera.main.aspect;
            if (type == 4)
            {
                var CON = GameObject.Find("CONTROL").GetComponent<CONTROL>();
                var poisonType = __instance.GetType();
                var plField = poisonType.GetField("PL", BindingFlags.Instance | BindingFlags.NonPublic);
                GameObject PL = (GameObject)plField.GetValue(__instance);
                if (PL.GetComponent<Player_Control_scr>().CON.EQ_WEP != null)
                {
                    if (PL.GetComponent<Player_Control_scr>().CON.EQ_WEP.special != 18) return false;
                    return true;
                }
                else
                {
                    if (__instance.SLOW_DUR == 0f)
                    {
                        var audioMethod = poisonType.GetMethod("Audio", BindingFlags.Instance | BindingFlags.NonPublic);
                        __instance.Active_Effects++;
                        audioMethod.Invoke(__instance, new object[]{2});
                        var SLOW_slotField = poisonType.GetField("SLOW_slot", BindingFlags.Instance | BindingFlags.NonPublic);
                        SLOW_slotField.SetValue(__instance, __instance.Active_Effects - 1);
                        __instance.transform.GetChild(__instance.Active_Effects - 1).GetComponent<TextMeshProUGUI>().text = "slowed";
                        __instance.transform.GetChild(__instance.Active_Effects - 1).GetComponent<TextMeshProUGUI>().color = Color.grey;
                    }
                    var steps = duration;
                    steps *= Mathf.LerpUnclamped(1.25f, 0.2f, (float)CON.CURRENT_PL_DATA.PLAYER_RES / 100f);
                    __instance.SLOW_DUR = Time.time + steps;
                }
                return false;
            }
            return true;
        }

        private static string ValueSuffix(int value)
        {
            if (value > 9)
            {
                return value.ToString();
            }
            return "0" + value.ToString();
        }

        private static void ApplyFlag(string Name)
        {
            var flagData = LunacidFlags.ItemToFlag[Name];
            if (Name == "Progressive Vampiric Symbol")
            {
                var receivedCount = ArchipelagoClient.AP.Session.Items.AllItemsReceived.Count(x => ArchipelagoClient.AP.Session.Items.GetItemName(x.Item) == "Progressive Vampiric Symbol");
                FlagHandler.ModifyFlag(flagData[0], flagData[1], Math.Min(3, receivedCount));
                return;
            }
            if (!FlagHandler.DoesPlayerHaveItem(Name))
            {
                FlagHandler.ModifyFlag(flagData[0], flagData[1], flagData[2]);
            }
        }

        private static string ProgressiveSymbolHandler(string Name)
        {
            try
            {
                if (ConnectionData.Symbols > 2)
                {
                    return "";
                }
                switch (ConnectionData.Symbols)
                {
                    case 0:
                        {
                            ConnectionData.Symbols += 1;
                            return "Vampiric Symbol (W)";
                        }
                    case 1:
                        {
                            ConnectionData.Symbols += 1;
                            return "Vampiric Symbol (A)";
                        }
                    case 2:
                        {
                            ConnectionData.Symbols += 1;
                            return "Vampiric Symbol (E)";
                        }
                }
                return "";
            }
            catch (Exception ex)
            {
                if (Control is null)
                {
                    _log.LogError("CON is null.");
                }
                if (Control.CURRENT_PL_DATA.ITEMS is null)
                {
                    _log.LogError("Items list is null.");
                }
                _log.LogError($"{ex.Message}");
                return "";
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
            else if (name.Contains("Silver ("))
            {
                return 0;
            }
            else if (name.Contains(" Switch"))
            {
                return -1;
            }
            return -2;
        }
    }
}