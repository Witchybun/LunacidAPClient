using BepInEx.Logging;
using HarmonyLib;
using LunacidAP.Data;
using UnityEngine;

namespace LunacidAP
{
    public class DoorHandler
    {

        private static ManualLogSource _log;
        public DoorHandler(ManualLogSource log)
        {
            _log = log;
            Harmony.CreateAndPatchAll(typeof(DoorHandler));
        }
        [HarmonyPatch(typeof(Door_scr), "ACT")]
        [HarmonyPrefix]
        private static bool ACT_LockDoors(Door_scr __instance)
        {
            if (!ArchipelagoClient.AP.SlotData.Doorlock)
            {
                return true;
            }
            var sceneName = __instance.gameObject.scene.name;
            var doorPOS = __instance.transform.position;
            var entrance = "";
            if (!LunacidDoors.EntranceToDoor.TryGetValue(sceneName, out var Data))
            {
                return true;
            }
            foreach (var doorData in Data)
            {
                if (AreDoorsIdentical(doorData.Value, doorPOS))
                {
                    entrance = doorData.Key;
                    break;
                }
            }
            if (entrance == "")
            {
                _log.LogError($"Could not find relevant door!");
                return false;
            }
            if (ArchipelagoClient.AP.SlotData.EntranceRandomizer)
            {
                var reversedEntrance = TeleportHandler.ReverseEntrance(entrance);
                if (!LunacidDoors.EntranceToDoorKey.ContainsKey(entrance))
                {
                    if (!ConnectionData.Entrances.TryGetValue(entrance, out var _))
                    {
                        _log.LogWarning($"Tried to handle {entrance} and failed.");
                        return true;
                    }
                    entrance = TeleportHandler.ReverseEntrance(ConnectionData.Entrances[entrance]);
                }
            }
            if (!LunacidDoors.EntranceToDoorKey.TryGetValue(entrance, out var key))
            {
                return true;
            }
            if (!ArchipelagoClient.AP.WasItemReceived(key))
            {
                var control = GameObject.Find("CONTROL").GetComponent<CONTROL>();
                var popup = control.PAPPY;
                popup.POP($"Locked.  Requires {key}.", 1f, 1);
                return false;
            }
            return true;
        }

        [HarmonyPatch(typeof(Act_Button_scr), "ACT")]
        [HarmonyPrefix]
        private static bool ACT_StopOpenIfNoKey(Act_Button_scr __instance)
        {
            if (!ArchipelagoClient.AP.SlotData.Doorlock)
            {
                return true;
            }
            var sceneName = __instance.gameObject.scene.name;
            if (sceneName != "CAS_3" || sceneName != "VOID")
            {
                return true;
            }
            switch (sceneName)
            {
                case "CAS_3":
                    {
                        if (LunacidDoors.BallroomDoors.Contains(__instance.transform.position) && !ArchipelagoClient.AP.WasItemReceived("Ballroom Side Rooms Keyring"))
                        {
                            var control = GameObject.Find("CONTROL").GetComponent<CONTROL>();
                            var popup = control.PAPPY;
                            popup.POP("The door is locked...", 1f, 1);
                            return false;
                        }
                        break;
                    }
                case "VOID":
                    {
                        if (LunacidDoors.VoidDoors.Contains(__instance.transform.position) && !ArchipelagoClient.AP.WasItemReceived("Ashen Doors Keyring"))
                        {
                            var control = GameObject.Find("CONTROL").GetComponent<CONTROL>();
                            var popup = control.PAPPY;
                            popup.POP("The door is locked...", 1f, 1);
                            return false;
                        }
                        break;
                    }
            }

            return true;
        }

        [HarmonyPatch(typeof(ST_DOOR), "ACT")]
        [HarmonyPrefix]
        private static bool ACT_StopOpenIfNoKeyTower(ST_DOOR __instance)
        {
            if (!ArchipelagoClient.AP.SlotData.Doorlock)
            {
                return true;
            }
            if (!ArchipelagoClient.AP.WasItemReceived("Tower of Abyss Keyring"))
            {
                var control = GameObject.Find("CONTROL").GetComponent<CONTROL>();
                var popup = control.PAPPY;
                popup.POP("The door is locked...", 1f, 1);
                return false;
            }
            return true;
        }
        private static bool AreDoorsIdentical(Vector3 firstDoor, Vector3 secondDoor)
        {
            return Vector3.Distance(firstDoor, secondDoor) < 1f;
        }
    }
}