using UnityEngine;

namespace LunacidAP
{
    public class FlagHandler
    {
        private static CONTROL CON;

        public static string LoadFlag(int Zone)
        {
            string current_string = "";
            CON = GameObject.Find("CONTROL").GetComponent<CONTROL>();
            switch (Zone)
            {
                case 0:
                    current_string = CON.CURRENT_PL_DATA.ZONE_0;
                    break;
                case 1:
                    current_string = CON.CURRENT_PL_DATA.ZONE_1;
                    break;
                case 2:
                    current_string = CON.CURRENT_PL_DATA.ZONE_2;
                    break;
                case 3:
                    current_string = CON.CURRENT_PL_DATA.ZONE_3;
                    break;
                case 4:
                    current_string = CON.CURRENT_PL_DATA.ZONE_4;
                    break;
                case 5:
                    current_string = CON.CURRENT_PL_DATA.ZONE_5;
                    break;
                case 6:
                    current_string = CON.CURRENT_PL_DATA.ZONE_6;
                    break;
                case 7:
                    current_string = CON.CURRENT_PL_DATA.ZONE_7;
                    break;
                case 8:
                    current_string = CON.CURRENT_PL_DATA.ZONE_8;
                    break;
                case 9:
                    current_string = CON.CURRENT_PL_DATA.ZONE_9;
                    break;
                case 10:
                    current_string = CON.CURRENT_PL_DATA.ZONE_10;
                    break;
                case 11:
                    current_string = CON.CURRENT_PL_DATA.ZONE_11;
                    break;
                case 12:
                    current_string = CON.CURRENT_PL_DATA.ZONE_12;
                    break;
                case 13:
                    current_string = CON.CURRENT_PL_DATA.ZONE_13;
                    break;
                case 14:
                    current_string = CON.CURRENT_PL_DATA.ZONE_14;
                    break;
                case 15:
                    current_string = CON.CURRENT_PL_DATA.ZONE_15;
                    break;
                case 16:
                    current_string = CON.CURRENT_PL_DATA.ZONE_16;
                    break;
            }
            return current_string;
        }

        public static void ModifyFlag(int Zone, int Slot, int Value)
        {
            var current_string = LoadFlag(Zone);
            current_string = current_string.Substring(0, Slot - 1) + Value + current_string.Substring(Slot, current_string.Length - Slot);
            switch (Zone)
            {
                case 0:
                    CON.CURRENT_PL_DATA.ZONE_0 = current_string;
                    break;
                case 1:
                    CON.CURRENT_PL_DATA.ZONE_1 = current_string;
                    break;
                case 2:
                    CON.CURRENT_PL_DATA.ZONE_2 = current_string;
                    break;
                case 3:
                    CON.CURRENT_PL_DATA.ZONE_3 = current_string;
                    break;
                case 4:
                    CON.CURRENT_PL_DATA.ZONE_4 = current_string;
                    break;
                case 5:
                    CON.CURRENT_PL_DATA.ZONE_5 = current_string;
                    break;
                case 6:
                    CON.CURRENT_PL_DATA.ZONE_6 = current_string;
                    break;
                case 7:
                    CON.CURRENT_PL_DATA.ZONE_7 = current_string;
                    break;
                case 8:
                    CON.CURRENT_PL_DATA.ZONE_8 = current_string;
                    break;
                case 9:
                    CON.CURRENT_PL_DATA.ZONE_9 = current_string;
                    break;
                case 10:
                    CON.CURRENT_PL_DATA.ZONE_10 = current_string;
                    break;
                case 11:
                    CON.CURRENT_PL_DATA.ZONE_11 = current_string;
                    break;
                case 12:
                    CON.CURRENT_PL_DATA.ZONE_12 = current_string;
                    break;
                case 13:
                    CON.CURRENT_PL_DATA.ZONE_13 = current_string;
                    break;
                case 14:
                    CON.CURRENT_PL_DATA.ZONE_14 = current_string;
                    break;
                case 15:
                    CON.CURRENT_PL_DATA.ZONE_15 = current_string;
                    break;
                case 16:
                    CON.CURRENT_PL_DATA.ZONE_16 = current_string;
                    break;
            }
        }
    }
}