using System.Collections.Generic;
using UnityEngine;

namespace LunacidAP.Data
{
    public class LunacidSwitches
    {

        private static readonly Dictionary<string, List<Vector3>> BasinSwitches = new(){
            { "Hollow Basin Switch Key", new List<Vector3>(){new Vector3(59.3f, 12.2f, -117.6f)} },
            { "Temple of Silence Switch Key", new List<Vector3>(){new Vector3(61.2f, -25.3f, 67.1f)}}
        };

        private static readonly Dictionary<string, List<Vector3>> MireSwitches = new(){
            { "Fetid Mire Switch Key", new List<Vector3>(){new Vector3(3.8f, 2.1f, -149.2f)} },
        };

        private static readonly Dictionary<string, List<Vector3>> LibrarySwitches = new(){
            { "Forbidden Archives Shortcut Switch Key", new List<Vector3>(){new Vector3(-45.0f, -1.8f, 21.1f)} },
            { "Forbidden Archives Elevator Switch Keyring", new List<Vector3>(){new Vector3(-85.0f, -2.9f, 11.0f),
                new Vector3(-90.9f, -18.0f, 17.0f), new Vector3(-61.0f, 10.0f, 15.5f), new Vector3(-61.0f, -2.0f, 15.5f), 
                new Vector3(-93.5f, 10.0f, -13.0f), new Vector3(-93.5f, -18.0f, -13.0f)} },
        };

        private static readonly Dictionary<string, List<Vector3>> TombSwitches = new(){
            { "Accursed Tomb Switch Keyring", new List<Vector3>(){new Vector3(72.2f, -6.0f, 56.2f), new Vector3(45.0f, -8.5f, -32.1f)} },
            { "Prometheus Fire Switch Keyring", new List<Vector3>(){new Vector3(0.5f, -8.9f, 73.0f), new Vector3(136.0f, -6.9f, 112.5f),
                new Vector3(-56.0f, -13.9f, -122.5f)}},
        };

        private static readonly Dictionary<string, List<Vector3>> BallroomSwitches = new(){
            { "Sealed Ballroom Switch Key", new List<Vector3>(){new Vector3(24.0f, 2.0f, -7.1f)} },
        };  

        private static readonly Dictionary<string, List<Vector3>> GrottoSwitches = new(){
            { "Grotto Fire Switch Keyring", new List<Vector3>(){new Vector3(-260.5f, 6.2f, -141.0f), new Vector3(-251.5f, 15.2f, -73f)} },
            { "Sand Temple Switches Keyring", new List<Vector3>(){new Vector3(-301.0f, 18.0f, -76.9f), new Vector3(-380.0f, 14.0f, -72.9f)} },
        };

        private static readonly Dictionary<string, List<Vector3>> PrisonSwitches = new(){
            { "Terminus Prison Back Alley Switch Key", new List<Vector3>(){new Vector3(68.0f, 14.0f, 6.9f)} },
            { "Forlorn Arena Gate Switch Key", new List<Vector3>(){new Vector3(104.0f, 25.1f, 18.0f)} },
        };

        private static readonly Dictionary<string, List<Vector3>> ArenaSwitches = new(){
            { "Temple of Water Switch Key", new List<Vector3>(){new Vector3(-50.9f, 10.0f, -115.0f)} },
            { "Temple of Earth Switch Key", new List<Vector3>(){new Vector3(-66.9f, 10.0f, 121.0f)} },
        };

        private static readonly Dictionary<string, List<Vector3>> AshSwitches = new(){
            { "Labyrinth of Ash Switch Key", new List<Vector3>(){new Vector3(-27.0f, 2.0f, -2.9f)} },
        };
        public static readonly Dictionary<string, Dictionary<string, List<Vector3>>> SwitchLocations = new(){
            { "PITT_A1",  BasinSwitches },
            { "SEWER_A1", MireSwitches },
            { "ARCHIVES", LibrarySwitches },
            { "HAUNT", TombSwitches },
            { "CAS_3", BallroomSwitches },
            { "CAVE", GrottoSwitches },
            { "PRISON", PrisonSwitches },
            { "VOID", AshSwitches },
            { "ARENA", ArenaSwitches },
        };
    }
}