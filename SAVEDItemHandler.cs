using System.Collections;
using LunacidAP.Data;
using UnityEngine.Rendering;

namespace LunacidAP
{
    public class SAVEDItemHandler
    {
        public string SAVEDZoneCreator(string currentString, int position, int value)
        {
            currentString = currentString.Substring(0, position - 1) + value + currentString.Substring(position, currentString.Length - position);
            return currentString;
        }

        public void SAVEDZoneReader(int zone, string currentString)
        {
            switch(zone)
            {
                case 0: //Hollow Basin (Above Temple)
                {
                    HollowBasinUpperReader(currentString);
                }
                break;
                    
            }
        }

        public void HollowBasinUpperReader(string currentString)
        {
            if (int.Parse(currentString) == 0)  // Its untouched flags; just return the default flag state
            {
                LunacidFlags.HollowBasin = new HollowBasinUpper(){};
                return;
            }
            
        }
    }
}