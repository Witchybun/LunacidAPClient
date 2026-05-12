using System;
using System.Collections;
using System.Collections.Generic;
using LunacidAP.Archipelago;
using LunacidAP.Data;
using UnityEngine;
using UnityEngine.SceneManagement;
using Object = UnityEngine.Object;
using Random = System.Random;

namespace LunacidAP.Patches;

public class TrapHandler
{
    private int _bleedTimer;
    private int _poisonTimer;
    private int _curseTimer;
    private int _slowTimer;
    private int _blindTimer;
    private int _manaTimer;
    private int _xpTimer;
    private int _ratTimer;
    private int _dateTimer;
    
    private static readonly Dictionary<string, int> TrapCounts = new()
    {
        { "Bleed Trap", 0 },
        { "Poison Trap", 0 },
        { "Curse Trap", 0 },
        { "Slowness Trap", 0 },
        { "Blindness Trap", 0 },
        { "Mana Drain Trap", 0 },
        { "XP Drain Trap", 0 }, 
        {"Rat Gang", 0},
        {"Date With Death Trap", 0},
        
    };

    public static void AddTrap(string name)
    {
        TrapCounts[name]++;
    }

    public void StatusPlayerWhenPossible()
    {
        foreach (var type in LunacidItems.Traps)
        {
            StatusPlayerWhenPossible(type);
        }
    }

    public void StatusPlayerWhenPossible(string type)
    {
        if (TrapCounts[type] <= 0)
        {
            return;
        }
        if (!ArchipelagoClient.AP.IsInNormalGameState())
        {
            return;
        }
        var con = GameObject.Find("CONTROL").GetComponent<CONTROL>();
        if (!IsStatusNotApplied(type, con.PL.Poison))
        {
            return;
        }
        if (!IsTimerReady(type))
        {
            return;
        }

        try
        {
            if (type != "Rat Gang" && type != "Date With Death Trap")
            {
                var harmId = GetHarmType(type);
                con.PL.Poison.Harm(harmId, 10.0f);
            }
            else
            {
                if (type == "Rat Gang")
                {
                    // This can likely fail because the rats prefab might not be loaded.
                    if (!DidDropRats())
                    {
                        return;
                    }
                }
                else
                {
                    // This waits until the player has Spirit Warp.
                    if (!CouldSendPlayerToDETHLAND())
                    {
                        return;
                    }
                }
            }
            TrapCounts[type] -= 1;
        }
        catch (Exception e)
        {
            Plugin.LOG.LogError($"Could not give trap of type {type}");
        }
    }

    private bool IsTimerReady(string type)
    {
        switch (type)
        {
            case "Bleed Trap":
            {
                if (_bleedTimer > 0)
                {
                    _bleedTimer--;
                    return false;
                }

                _bleedTimer = 60;
                return true;
            }
            case "Poison Trap":
            {
                if (_poisonTimer > 0)
                {
                    _poisonTimer--;
                    return false;
                }

                _poisonTimer = 60;
                return true;
            }
            case "Curse Trap":
            {
                if (_curseTimer > 0)
                {
                    _curseTimer--;
                    return false;
                }
                _curseTimer = 60;
                return true;
            }
            case "Slowness Trap":
            {
                if (_slowTimer > 0)
                {
                    _slowTimer--;
                    return false;
                }

                _slowTimer = 60;
                return true;
            }
            case "Blindness Trap":
            {
                if (_blindTimer > 0)
                {
                    _blindTimer--;
                    return false;
                }

                _blindTimer = 60;
                return true;
            }
            case "Mana Drain Trap":
            {
                if (_manaTimer > 0)
                {
                    _manaTimer--;
                    return false;
                }

                _manaTimer = 60;
                return true;
            }
            case "XP Drain Trap":
            {
                if (_xpTimer > 0)
                {
                    _xpTimer--;
                    return false;
                }

                _xpTimer = 60;
                return true;
            }
            case "Rat Gang":
            {
                if (_ratTimer > 0)
                {
                    _ratTimer--;
                    return false;
                }

                _ratTimer = 300;
                return true;
            }
            case "Date With Death Trap":
            {
                if (_dateTimer > 0)
                {
                    _dateTimer--;
                    return false;
                }

                _dateTimer = 600;
                return true;
            }
        }
        return false;
    }

    private bool IsStatusNotApplied(string type, Player_Poison poison)
    {
        switch (type)
        {
            case "Bleed Trap":
            {
                return !(poison.BLEED_DUR > 0);
            }
            case "Poison Trap":
            {
                return !(poison.POISON_DUR > 0);
            }
            case "Curse Trap":
            {
                return !(poison.CURSE_DUR > 0);
            }
            case "Slowness Trap":
            {
                return !(poison.SLOW_DUR > 0);
            }
            case "Blindness Trap":
            {
                return !(poison.BLIND_DUR > 0);
            }
            case "Mana Drain Trap":
            {
                return !(poison.DRAIN_DUR > 0);
            }
            case "XP Drain Trap":
            {
                return !(poison.XP_DRAIN_DUR > 0);
            }
            case "Rat Gang":
            {
                return true;
            }
            case "Date With Death Trap":
            {
                if (SceneManager.GetActiveScene().name == "DETHLAND")
                {
                    return false;
                }
                return true;
            }
        }
        return false;
    }

    private int GetHarmType(string type)
    {
        switch (type)
        {
            case "Bleed Trap":
                return 0;
            case "Poison Trap":
                return 1;
            case "Curse Trap":
                return 2;
            case "Slowness Trap":
                return 4;
            case "Blindness Trap":
                return 5;
            case "Mana Drain Trap":
                return 6;
            case "XP Drain Trap":
                return 7;
        }

        return 0;
    }
    
    private bool DidDropRats()
    {
        if (EnemyHandler.RatPrefab is null)
        {
            return false;
        }
        var con = GameObject.Find("CONTROL").GetComponent<CONTROL>();
        var plPosition = con.PL.gameObject.transform.position;
        var ratBase = EnemyHandler.RatPrefab;
        Object.Instantiate(ratBase, plPosition, Quaternion.identity);
        Object.Instantiate(ratBase, plPosition + new Vector3(0, 0, 2), Quaternion.identity);
        Object.Instantiate(ratBase, plPosition + new Vector3(2, 0, 0), Quaternion.identity);
        Object.Instantiate(ratBase, plPosition + new Vector3(-2, 0, 0), Quaternion.identity);
        Object.Instantiate(ratBase, plPosition + new Vector3(0, 0, -2), Quaternion.identity);
        AudioSource.PlayClipAtPoint(MuseHandler.storedSounds["rats"], plPosition);
        return true;
    }

    private bool CouldSendPlayerToDETHLAND()
    {
        var con = GameObject.Find("CONTROL").GetComponent<CONTROL>();
        if (!DoesPlayerHaveSpiritWarp(con))  // The player should be able to actually escape.
        {
            return false;
        }
        var component = con.MENU.ITEMS[20].GetComponent<GOTO_LEVEL>();
        component.LVL = "DETHLAND";
        component.POS = new Vector3(0f, 1f, 0f);
        component.ROT = 0f;
        component.gameObject.SetActive(value: true);
        return true;
    }
    
    private static bool DoesPlayerHaveSpiritWarp(CONTROL con)
    {
        var playerInventory = con.CURRENT_PL_DATA.SPELLS;
        if (playerInventory is null)
        {
            return false;
        }
        for (var i = 0; i < 128; i++)
        {
            if (playerInventory[i] == "" || playerInventory[i] == null)
            {
                return false;
            }
            if (StaticFuncs.REMOVE_NUMS(playerInventory[i]) == "SPIRIT WARP")
            {
                return true;
            }

        }
        return false;
    }
}