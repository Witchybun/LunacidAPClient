using System.Collections.Generic;
using BepInEx.Logging;
using LunacidAP.Data;
using UnityEngine;
using static LunacidAP.Data.LunacidLocations;

namespace LunacidAP
{
    public class SwapperHandler
    {
        private static ManualLogSource _log;
        private static readonly List<string> ModelNames = new()
        {
            "MDL", "Health_Vial_Model", "Ring", "Bag", "Club_Torch", "Sword_Heritage", "RUSTED SWORD", "VHS", "mdl", "Sword_Brittle1", "Candel",
            "Bow_Elf", "KEY_MDL", "Sickle", "Axe_Anu", "Staff_Osiris"
        };

        public SwapperHandler(ManualLogSource log)
        {
            _log = log;
        }

        public static void ReplaceModelWithAppropriateItem(Item_Pickup_scr pickupObject, LocationData locationData)
        {
            if (LunacidLocations.LocationsThatImmediatelyReceive.Contains(locationData.APLocationName) || locationData.APLocationName.Contains("Daedalus Knowledge"))
            {
                return; // No need to swap models for locations you get dropped on top of you.
            }
            var archipelagoItem = ConnectionData.ScoutedLocations[locationData.APLocationID];
            var locationItem = archipelagoItem.Name;
            if (ConnectionData.ScoutedLocations[locationData.APLocationID].Game != "Lunacid")
            {
                try
                {
                    var substitutedItem = ArchipelagoGames.KeywordToItem(archipelagoItem);
                    if (substitutedItem == "NULL")
                    {
                        HideItemModel(pickupObject);
                        return;
                    }
                    locationItem = substitutedItem;
                }
                catch
                {
                    _log.LogError($"Failed to replace non-Lunacid Item at {locationData.APLocationName}");
                    return;
                }
            }
            if (locationItem.Contains("Silver"))
            {
                try
                {
                    ReplaceModelWithSilver(pickupObject);
                }
                catch
                {
                    _log.LogError($"Failed to replace Money Item at {locationData.APLocationName}");
                }
                return;
            }
            if (LunacidItems.Materials.Contains(locationItem))
            {
                try
                {
                    ReplaceModelWithBag(pickupObject);
                }
                catch
                {
                    _log.LogError($"Failed to replace Material Item at {locationData.APLocationName}");
                }
                return;
            }
            if (LunacidItems.Items.Contains(locationItem))
            {
                if (LunacidItems.Vouchers.Contains(locationItem) || locationItem == "Deep Knowledge" || locationItem == "Angel Feather")
                {
                    locationItem = "Cloth Bandage";
                }
                try
                {
                    ReplaceItemGeneric(pickupObject, locationItem, "ITEMS/");
                }
                catch
                {
                    _log.LogError($"Failed to replace Item at {locationData.APLocationName}");
                }
            }
            if (LunacidItems.Weapons.Contains(locationItem))
            {
                try
                {
                    locationItem = locationItem.ToUpper();
                    ReplaceItemGeneric(pickupObject, locationItem, "WEPS/");
                }
                catch
                {
                    _log.LogError($"Failed to replace Weapon at {locationData.APLocationName}");
                }
            }
            if (LunacidItems.Spells.Contains(locationItem))
            {
                try
                {
                    locationItem = locationItem.ToUpper();
                    ReplaceItemGeneric(pickupObject, locationItem, "MAGIC/");
                }
                catch
                {
                    _log.LogError($"Failed to replace Spell at {locationData.APLocationName}");
                }
            }
            if (LunacidItems.Keys.Contains(locationItem) || LunacidItems.Switches.Contains(locationItem))
            {
                try
                {
                    ReplaceModelWithKey(pickupObject);
                }
                catch
                {
                    _log.LogError($"Failed to replace Switch or Key at {locationData.APLocationName}");
                }
            }
            if (archipelagoItem.Classification.HasFlag(Archipelago.MultiClient.Net.Enums.ItemFlags.Trap))
            {
                try
                {

                    ReplaceModelWithTrickyThing(pickupObject);
                }
                catch
                {
                    _log.LogError($"Failed to replace Trap at {locationData.APLocationName}");
                }
            }

        }

        private static void ReplaceItemGeneric(Item_Pickup_scr pickupObject, string locationItem, string resourceInfo)
        {
            GameObject resourceReference;
            Transform modelReference;
            resourceReference = GameObject.Instantiate(Resources.Load(resourceInfo + locationItem)) as GameObject;
            if (resourceInfo == "WEPS/")
            {
                modelReference = resourceReference.transform.Find("MDL");
            }
            else if (resourceInfo == "MAGIC/")
            {
                GameObject.Destroy(resourceReference);
                resourceReference = GameObject.Instantiate(Resources.Load("ITEMS/TORNADO_PICKUP")) as GameObject;
                modelReference = resourceReference.transform.Find("Ring");
            }
            else if (resourceInfo == "ITEMS/")
            {
                if (LunacidItems.ItemToPickup.TryGetValue(locationItem, out var itemPickupName))
                {
                    GameObject.Destroy(resourceReference);
                    resourceReference = GameObject.Instantiate(Resources.Load(resourceInfo + itemPickupName)) as GameObject;
                    modelReference = resourceReference.transform.GetChild(1);
                }
                else
                {
                    HideItemModel(pickupObject);
                    GameObject.Destroy(resourceReference);
                    return;
                }
            }
            else
            {
                GameObject.Destroy(resourceReference);
                ReplaceModelWithBag(pickupObject);
                return;
            }
            if (modelReference is null)
            {
                if (CanHandleSpecialCase(pickupObject, locationItem, resourceReference))
                {
                    return;
                }
                _log.LogWarning($"Item for {locationItem} has no MDL.  Bagging it with the resource applied.");
                foreach (Transform child in resourceReference.transform)
                {
                    _log.LogWarning($"{child.name}");
                }
                ReplaceModelWithBag(pickupObject);
                resourceReference.transform.SetParent(pickupObject.transform);
                return;
            }
            modelReference = GameObject.Instantiate(modelReference);
            modelReference.transform.SetParent(pickupObject.transform);
            modelReference.transform.SetPositionAndRotation(pickupObject.transform.position, pickupObject.transform.rotation);
            HideItemModel(pickupObject);
            modelReference.gameObject.SetActive(true);
            GameObject.Destroy(resourceReference);
        }

        private static bool CanHandleSpecialCase(Item_Pickup_scr pickupObject, string locationItem, GameObject foundObject)
        {
            if (locationItem == "MARAUDER BLACK FLAIL")
            {
                var realFoundObject = GameObject.Instantiate(foundObject.transform.GetChild(2));
                realFoundObject.transform.SetParent(pickupObject.transform);
                realFoundObject.transform.SetPositionAndRotation(pickupObject.transform.position, pickupObject.transform.rotation);
                HideItemModel(pickupObject);
                realFoundObject.gameObject.SetActive(true);
                GameObject.Destroy(foundObject);
                HideTheFuckingHandBro(foundObject);
                return true;
            }
            try
            {
                GameObject.Destroy(foundObject.GetComponent<Weapon_scr>());
                GameObject.Destroy(foundObject.GetComponent<AudioSource>());
                foundObject.transform.SetParent(pickupObject.transform);
                foundObject.transform.SetPositionAndRotation(pickupObject.transform.position, pickupObject.transform.rotation);
                HideItemModel(pickupObject);
                foundObject.SetActive(true);
                HideTheFuckingHandBro(foundObject);
                return true;
            }
            catch
            {
                return false;
            }
        }

        private static void HideTheFuckingHandBro(GameObject weapon)
        {
            foreach (Transform child in weapon.transform)
            {
                if (child.name.Contains("HAND") || child.name.Contains("Hand"))
                {
                    child.gameObject.SetActive(false);
                    return;
                }
            }

        }


        private static void ReplaceModelWithBag(Item_Pickup_scr pickupObject)
        {
            var resourceReference = GameObject.Instantiate(Resources.Load("ITEMS/ASHES")) as GameObject;
            var modelReference = resourceReference.transform.GetChild(1);
            modelReference.transform.position = pickupObject.transform.position;
            modelReference.transform.SetParent(pickupObject.transform);
            modelReference.gameObject.SetActive(true);
            HideItemModel(pickupObject);
            GameObject.Destroy(resourceReference);
        }

        private static void ReplaceModelWithSilver(Item_Pickup_scr pickupObject)
        {
            var resourceReference = GameObject.Instantiate(Resources.Load("ITEMS/GOLD_10")) as GameObject;
            var modelReference = resourceReference.transform.GetChild(1);
            modelReference.transform.position = pickupObject.transform.position;
            modelReference.transform.SetParent(pickupObject.transform);
            modelReference.gameObject.SetActive(true);
            HideItemModel(pickupObject);
            GameObject.Destroy(resourceReference);
        }

        private static void ReplaceModelWithKey(Item_Pickup_scr pickupObject)
        {
            var resourceReference = GameObject.Instantiate(Resources.Load("ITEMS/ENKEY_PICKUP")) as GameObject;
            var modelReference = resourceReference.transform.GetChild(1);
            modelReference.transform.position = pickupObject.transform.position;
            modelReference.transform.SetParent(pickupObject.transform);
            modelReference.gameObject.SetActive(true);
            HideItemModel(pickupObject);
            GameObject.Destroy(resourceReference);
        }

        private static void ReplaceModelWithTrickyThing(Item_Pickup_scr pickupObject)
        {
            var trickyThings = new List<string>() { "Lucid Blade", "Enchanted Key", "Water Talisman", "Flame Flare", "Shining Blade", "Bomb", "Moonlight"};
            var random = ArchipelagoClient.AP.RandomStatic.Next(0, 3);
            var chosenTrick = trickyThings[random];
            var type = "";
            switch (chosenTrick)
            {
                case "Lucid Blade":
                    {
                        type = "WEPS/";
                        break;
                    }
                case "Enchanted Key":
                    {
                        type = "ITEMS/";
                        break;
                    }
                case "Water Talisman":
                    {
                        type = "ITEMS/";
                        break;
                    }
                case "Flame Flare":
                    {
                        type = "MAGIC/";
                        break;
                    }
                case "Shining Blade":
                    {
                        type = "WEPS/";
                        break;
                    }
                case "Bomb":
                    {
                        type = "ITEMS/";
                        break;
                    }
                case "Moonlight":
                    {
                        type = "WEPS/";
                        break;
                    }
            }
            ReplaceItemGeneric(pickupObject, chosenTrick, type);
        }

        private static Transform FindModelCandidate(Item_Pickup_scr pickupObject)
        {
            var childNames = "";
            foreach (Transform child in pickupObject.transform)
            {
                var name = child.gameObject.name;
                childNames += name;
                foreach (var matchString in ModelNames)
                {
                    if (matchString.Contains(name))
                    {
                        return child;
                    }
                }
            }
            _log.LogWarning($"The object {pickupObject.name} didn't have a model to hide.  Children: {childNames}");
            return pickupObject.transform;
        }

        private static void HideItemModel(Item_Pickup_scr pickupObject)
        {
            var model = FindModelCandidate(pickupObject);
            if (model.name == pickupObject.name)
            {
                return;
            }
            model.gameObject.SetActive(false);
        }
    }
}