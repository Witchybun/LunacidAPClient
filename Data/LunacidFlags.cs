using System.Collections.Generic;

namespace LunacidAP.Data
{
    public class LunacidFlags
    {
        public static HollowBasinUpper HollowBasin;

        public static bool IsSAVEDDataSame(int[] knownData, int[] offeredData)
        {
            for (var i = 0; i<= 1; i++)  //Structure is: 0: Zone, 1: Position in data "string" from left to right
            {
                if (knownData[i] != offeredData[i])
                {
                    return false;
                }
            }
            return true;
        }
    }
    

    public class HollowBasinUpper
    {
        public readonly int Zone = 0;
        public bool WoodenBarricadeDestroyed { get; set; } = false;
        public bool HealthVialNearDoor { get; set; } = false;
        public bool PoolRightRight { get; set; } = false;
        public bool PoolLeft { get; set; } = false;
        public bool DemiChest { get; set; } = false;
        public bool SwitchNearDemi { get; set; } = false;
        public bool TorchNearTemple { get; set; } = false;
        public bool DidDemiLeave { get; set; } = false;
        public bool LadderFallen { get; set; } = false;

        public static readonly Dictionary<string, int[]> LocationToSAVED = new(){
            { nameof(WoodenBarricadeDestroyed), new int[3]{ 0, 2, 1 } },
            { nameof(HealthVialNearDoor), new int[3]{ 0, 3, 1 }},
            { nameof(PoolRightRight), new int[3]{ 0, 4, 1 }},
            { nameof(PoolLeft), new int[3]{ 0, 5, 1 }},
            { nameof(DemiChest), new int[3]{ 0, 2, 2 }}, // This means the barricade and chest are linked.
            { nameof(DidDemiLeave), new int[3]{ 0, 7, 1 }},



        };
    }

    public class WingsRest
    {
        public readonly int Zone = 3;
        public bool Bench {get; set;} = false;
        public bool Rafter {get; set;} = false;
        public bool ClivesPotion {get; set;} = false;

        public static readonly Dictionary<string, int[]> LocationToSAVED = new(){
            { nameof(Bench), new int[3]{ 3, 11, 1}},
            { nameof(ClivesPotion), new int[3]{ 3, 10, 1}},
            { nameof(Rafter), new int[3]{16, 5, 1}},
        };
    }

    public class HollowBasinLower
    {
        public bool FountainBottle { get; set; } = false;
        public bool SacrificialDagger { get; set; } = false;
        public bool ChestNearAltar { get; set; } = false;
        public bool SwitchToTemple { get; set; } = false;
        public bool PillarRoomLeft { get; set; } = false;
        public bool PillarRoomBackLeft { get; set; } = false;
    }


}