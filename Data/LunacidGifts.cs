using System.Collections.Generic;
using Archipelago.Gifting.Net.Giftboxes;
using Archipelago.Gifting.Net.Traits;

namespace LunacidAP.Data
{
    public class LunacidGifts
    {
        public static readonly Dictionary<string, string[]> ItemToGiftTraits = new(){
            {"Health Vial", new string[]{GiftFlag.Heal, GiftFlag.Consumable, GiftFlag.Drink, GiftFlag.Life}}
        }; 
    }
}