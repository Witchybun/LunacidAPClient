using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace LunacidAP.Patches;

public class InitialLoadGrabber
{
    private static bool finishedCollectionState;
    public static Dictionary<string, GameObject> GrabbedObjects = new();
    public static Dictionary<string, GameObject> GrabbedEnemy = new ();
    
    public static bool SceneIterator(string sceneName)
    {
        if (finishedCollectionState)
        {
            return false;
        }

        switch (sceneName)
        {
            case "MainMenu":
                SceneManager.LoadScene("SEWER_A1");
                break;
            case "SEWER_A1":
            {
                var weedPlant = GameObject.Find("SEWER/Live_Props/LOTUS/Lotus3/Weed_Plant1");
                if (weedPlant != null)
                {
                    GrabbedObjects["Grass"] = weedPlant;
                }

                var barrel = GameObject.Find("SEWER/Live_Props/Barrel");
                if (barrel != null)
                {
                    Object.Destroy(barrel.GetComponent<OBJ_HEALTH>());
                    GrabbedObjects["Barrel"] = barrel;
                }

                var door = GameObject.Find("SEWER/Live_Props/DOOR/DOOR_WAIT/Stone_Door");
                if (door != null)
                {
                    GrabbedObjects["Door"] = door;
                }

                var slime = GameObject.Find("SEWER").transform.GetChild(6).GetChild(0).gameObject;
                if (slime != null)
                {
                    GrabbedEnemy["Slime"] = slime;
                }

                var rat = GameObject.Find("SEWER").transform.GetChild(6).GetChild(22).gameObject;
                if (rat != null)
                {
                    GrabbedEnemy["Rat"] = rat;
                }

                var stool = GameObject.Find("SEWER/Props/Stool");
                if (stool != null)
                {
                    GrabbedObjects["Stool"] = stool;
                }

                var rug = GameObject.Find("SEWER/Props/Rug (2)");
                if (rug != null)
                {
                    GrabbedObjects["Rug"] = rug;
                }

                SceneManager.LoadScene("FOREST_A1");
                break;
            }
            case "FOREST_A1":
            {
                var lunaga = GameObject.Find("FOREST_A1").transform.GetChild(37).GetChild(3).gameObject;
                if (lunaga != null)
                {
                    GrabbedEnemy["Lunaga"] = lunaga;
                }

                var skull = GameObject.Find("FOREST_A1/Live_Props/clutter/SKULL2");
                if (skull != null)
                {
                    GrabbedObjects["Skull"] = skull;
                }

                var cauldron = GameObject.Find("FOREST_A1/Live_Props/clutter/Cauldron");
                if (cauldron != null)
                {
                    GrabbedObjects["Cauldron"] = cauldron;
                }

                var gaMangetsu = GameObject.Find("FOREST_A1/NPC/MOON/").transform.GetChild(0).gameObject;
                if (gaMangetsu != null)
                {
                    gaMangetsu.SetActive(true);
                    GrabbedEnemy["Ga-Mangetsu"] = gaMangetsu;
                }

                SceneManager.LoadScene("ARCHIVES");
                break;
            }
            case "ARCHIVES":
            {
                var bookStack = GameObject.Find("ARCHIVES/Props/books").transform.GetChild(43).gameObject;
                Object.Destroy(bookStack.GetComponent<OBJ_HEALTH>());
                GrabbedObjects["Pile of Books"] = bookStack;
                var book = GameObject.Find("ARCHIVES/Props/books").transform.GetChild(45).gameObject;
                Object.Destroy(book.GetComponent<OBJ_HEALTH>());
                GrabbedObjects["Book"] = book;
                var candel = GameObject.Find("ARCHIVES/Live_Props/Candel4");
                Object.Destroy(candel.GetComponent<OBJ_HEALTH>());
                GrabbedObjects["Candle"] = candel;
                var lever = GameObject.Find("ARCHIVES/Live_Props/ELE1/LEVER/Lever");
                if (lever != null)
                {
                    Object.Destroy(lever.transform.GetChild(2));
                    GrabbedObjects["Lever"] = lever;
                }

                SceneManager.LoadScene("CAS_1");
                break;
            }
            case "CAS_1":
            {
                var chairs = GameObject.Find("CAS_1/Live_Props/Chairs");
                if (chairs != null)
                {
                    var chair = chairs.transform.GetChild(1).gameObject;
                    GrabbedObjects["Chair"] = chair;
                }
                finishedCollectionState = true;
                SceneManager.LoadScene("MainMenu");
                break;
            }
        }

        return true;
    }
}