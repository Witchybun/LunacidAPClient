using System.Collections.Generic;
using UnityEngine;

namespace LunacidAP.Data
{
    public class LunacidSwitches
    {

        private static readonly Dictionary<string, Vector3> BasinSwitches = new(){
            { "Hollow Basin Switch", new Vector3(59.3f, 12.2f, -117.6f) },
            { "Temple of Silence Switch", new Vector3(61.2f, -25.3f, 67.1f)}
        };

        private static readonly Dictionary<string, Vector3> MireSwitches = new(){
            { "Fetid Mire Switch", new Vector3(3.8f, 2.1f, -149.2f) },
        };

        private static readonly Dictionary<string, Vector3> LibrarySwitches = new(){
            { "Forbidden Archives Shortcut Switch", new Vector3(-45.0f, -1.8f, 21.1f) },
            { "Forbidden Archives Elevator Switch (1F - 2F) [Top]", new Vector3(-85.0f, -2.9f, 11.0f) },
            { "Forbidden Archives Elevator Switch (1F - 2F) [Bottom]", new Vector3(-90.9f, -18.0f, 17.0f) },
            { "Forbidden Archives Elevator Switch (2F - 3F) [Top]", new Vector3(-61.0f, 10.0f, 15.5f) },
            { "Forbidden Archives Elevator Switch (2F - 3F) [Bottom]", new Vector3(-61.0f, -2.0f, 15.5f) },
            { "Forbidden Archives Elevator Switch (1F - 3F) [Top]", new Vector3(-93.5f, 10.0f, -13.0f) },
            { "Forbidden Archives Elevator Switch (1F - 3F) [Bottom]", new Vector3(-93.5f, -18.0f, -13.0f) },
        };

        private static readonly Dictionary<string, Vector3> TombSwitches = new(){
            { "Accursed Tomb Main Switch", new Vector3(72.2f, -6.0f, 56.2f) },
            { "Accursed Tomb Crypt Switch", new Vector3() },
            { "Accursed Tomb Light Switch (Near Save Crystal)", new Vector3(0.5f, -8.9f, 73.0f) },
            { "Accursed Tomb Light Switch (Near Demi)", new Vector3(136.0f, -6.9f, 112.5f) },
            { "Accursed Tomb Light Switch (In Noble Crypt)", new Vector3(-56.0f, -13.9f, -122.5f) },
            { "Accursed Tomb Lightning Gate (To Mausoleum)", new Vector3() },
            { "Accursed Tomb Lightning Gate (To Jusztina)", new Vector3() },
        };

        private static readonly Dictionary<string, Vector3> BallroomSwitches = new(){
            { "Sealed Ballroom Switch", new Vector3() },
        };  

        private static readonly Dictionary<string, Vector3> GrottoSwitches = new(){
            { "Boiling Grotto Fire Switch (North)", new Vector3() },
            { "Boiling Grotto Fire Switch (South)", new Vector3() },
            { "Sand Temple Main Switch", new Vector3() },
            { "Sand Temple Back Switch", new Vector3() },
        };

        private static readonly Dictionary<string, Vector3> PrisonSwitches = new(){
            { "Terminus Prison Back Alley Switch", new Vector3() },
            { "Forlorn Arena Gate Switch", new Vector3() },
        };

        private static readonly Dictionary<string, Vector3> ArenaSwitches = new(){
            { "Temple of Water Switch", new Vector3() },
            { "Temple of Earth Switch", new Vector3() },
        };

        private static readonly Dictionary<string, Vector3> AshSwitches = new(){
            { "Labyrinth of Ash Switch", new Vector3() },
        };
        public static readonly Dictionary<string, Dictionary<string, Vector3>> SwitchLocations = new(){
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