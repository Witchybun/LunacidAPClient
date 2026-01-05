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
    private bool _isRatTrapped;

    public static void AddTrap(string name)
    {
        TrapCounts[name]++;
    }
    
    public IEnumerator BleedPlayerWhenPossible()
    {
        while (ArchipelagoClient.AP.allowCoroutines)
        {
            while (TrapCounts["Bleed Trap"] <= 0)
            {
                if (!ArchipelagoClient.AP.allowCoroutines)
                {
                    yield break;
                }
                yield return null;
            }

            if (!ArchipelagoClient.AP.allowCoroutines)
            {
                TrapCounts["Bleed Trap"] = 0;
                yield break;
            }
            while (!ArchipelagoClient.AP.IsInNormalGameState())
            {
                yield return null;
            }
            var con = GameObject.Find("CONTROL").GetComponent<CONTROL>();
            while (con.PL.Poison.BLEED_DUR > 0)
            {
                con = GameObject.Find("CONTROL").GetComponent<CONTROL>();
                yield return new WaitForSeconds(1f);
            }
            con = GameObject.Find("CONTROL").GetComponent<CONTROL>();
            con.PL.Poison.Harm(0, 10.0f);
            TrapCounts["Bleed Trap"] -= 1;
            yield return new WaitForSeconds(5f);
        }
    }

    public IEnumerator PoisonPlayerWhenPossible()
    {
        while (ArchipelagoClient.AP.allowCoroutines)
        {
            while (TrapCounts["Poison Trap"] <= 0)
            {
                if (!ArchipelagoClient.AP.allowCoroutines)
                {
                    yield break;
                }
                yield return null;
            }
            if (!ArchipelagoClient.AP.allowCoroutines)
            {
                TrapCounts["Poison Trap"] = 0;
                yield break;
            }
            while (!ArchipelagoClient.AP.IsInNormalGameState())
            {
                yield return null;
            }
            var con = GameObject.Find("CONTROL").GetComponent<CONTROL>();
            while (con.PL.Poison.POISON_DUR > 0)
            {
                con = GameObject.Find("CONTROL").GetComponent<CONTROL>();
                yield return new WaitForSeconds(1f);
            }
            con = GameObject.Find("CONTROL").GetComponent<CONTROL>();
            con.PL.Poison.Harm(1, 10.0f);
            TrapCounts["Poison Trap"] -= 1;
            yield return new WaitForSeconds(5f);
        }
    }

    public IEnumerator CursePlayerWhenPossible()
    {
        while (ArchipelagoClient.AP.allowCoroutines)
        {
            while (TrapCounts["Curse Trap"] <= 0)
            {
                if (!ArchipelagoClient.AP.allowCoroutines)
                {
                    yield break;
                }
                yield return null;
            }
            if (!ArchipelagoClient.AP.allowCoroutines)
            {
                TrapCounts["Curse Trap"] = 0;
                yield break;
            }
            while (!ArchipelagoClient.AP.IsInNormalGameState())
            {
                yield return null;
            }
            var con = GameObject.Find("CONTROL").GetComponent<CONTROL>();
            while (con.PL.Poison.CURSE_DUR > 0)
            {
                con = GameObject.Find("CONTROL").GetComponent<CONTROL>();
                yield return new WaitForSeconds(1f);
            }
            con = GameObject.Find("CONTROL").GetComponent<CONTROL>();
            con.PL.Poison.Harm(2, 10.0f);
            TrapCounts["Curse Trap"] -= 1;
            yield return new WaitForSeconds(5f);
        }
    }
    
    public IEnumerator SlowPlayerWhenPossible()
    {
        while (ArchipelagoClient.AP.allowCoroutines)
        {
            while (TrapCounts["Slowness Trap"] <= 0)
            {
                if (!ArchipelagoClient.AP.allowCoroutines)
                {
                    yield break;
                }
                yield return null;
            }
            if (!ArchipelagoClient.AP.allowCoroutines)
            {
                TrapCounts["Slowness Trap"] = 0;
                yield break;
            }
            while (!ArchipelagoClient.AP.IsInNormalGameState())
            {
                yield return null;
            }
            var con = GameObject.Find("CONTROL").GetComponent<CONTROL>();
            while (con.PL.Poison.SLOW_DUR > 0)
            {
                con = GameObject.Find("CONTROL").GetComponent<CONTROL>();
                yield return new WaitForSeconds(1f);
            }
            con = GameObject.Find("CONTROL").GetComponent<CONTROL>();
            con.PL.Poison.Harm(4, 10.0f);
            TrapCounts["Slowness Trap"] -= 1;
            yield return new WaitForSeconds(5f);
        }
    }
    
    public IEnumerator BlindPlayerWhenPossible()
    {
        while (ArchipelagoClient.AP.allowCoroutines)
        {
            
            while (TrapCounts["Blindness Trap"] <= 0)
            {
                if (!ArchipelagoClient.AP.allowCoroutines)
                {
                    yield break;
                }
                yield return null;
            }
            if (!ArchipelagoClient.AP.allowCoroutines)
            {
                TrapCounts["Blindness Trap"] = 0;
                yield break;
            }
            while (!ArchipelagoClient.AP.IsInNormalGameState())
            {
                yield return null;
            }
            var con = GameObject.Find("CONTROL").GetComponent<CONTROL>();
            while (con.PL.Poison.BLIND_DUR > 0)
            {
                con = GameObject.Find("CONTROL").GetComponent<CONTROL>();
                yield return new WaitForSeconds(1f);
            }
            con = GameObject.Find("CONTROL").GetComponent<CONTROL>();
            con.PL.Poison.Harm(5, 10.0f);
            TrapCounts["Blindness Trap"] -= 1;
            yield return new WaitForSeconds(5f);
        }
    }
    
    public IEnumerator DrainManaOfPlayerWhenPossible()
    {
        while (ArchipelagoClient.AP.allowCoroutines)
        {
            while (TrapCounts["Mana Drain Trap"] <= 0)
            {
                if (!ArchipelagoClient.AP.allowCoroutines)
                {
                    yield break;
                }
                yield return null;
            }
            if (!ArchipelagoClient.AP.allowCoroutines)
            {
                TrapCounts["Mana Drain Trap"] = 0;
                yield break;
            }
            while (!ArchipelagoClient.AP.IsInNormalGameState())
            {
                yield return null;
            }
            var con = GameObject.Find("CONTROL").GetComponent<CONTROL>();
            while (con.PL.Poison.DRAIN_DUR > 0)
            {
                con = GameObject.Find("CONTROL").GetComponent<CONTROL>();
                yield return new WaitForSeconds(1f);
            }
            con = GameObject.Find("CONTROL").GetComponent<CONTROL>();
            con.PL.Poison.Harm(6, 10.0f);
            TrapCounts["Mana Drain Trap"] -= 1;
            yield return new WaitForSeconds(5f);
        }
    }
    
    public IEnumerator DrainXPOfPlayerWhenPossible()
    {
        while (ArchipelagoClient.AP.allowCoroutines)
        {
            while (TrapCounts["XP Drain Trap"] <= 0)
            {
                if (!ArchipelagoClient.AP.allowCoroutines)
                {
                    yield break;
                }
                yield return null;
            }
            if (!ArchipelagoClient.AP.allowCoroutines)
            {
                TrapCounts["XP Drain Trap"] = 0;
                yield break;
            }
            while (!ArchipelagoClient.AP.IsInNormalGameState())
            {
                yield return null;
            }
            var con = GameObject.Find("CONTROL").GetComponent<CONTROL>();
            while (con.PL.Poison.XP_DRAIN_DUR > 0)
            {
                con = GameObject.Find("CONTROL").GetComponent<CONTROL>();
                yield return new WaitForSeconds(1f);
            }
            con = GameObject.Find("CONTROL").GetComponent<CONTROL>();
            con.PL.Poison.Harm(7, 10.0f);
            TrapCounts["XP Drain Trap"] -= 1;
            yield return new WaitForSeconds(5f);
        }
    }
    
    public IEnumerator DropRatsWhenPossible()
    {
        while (ArchipelagoClient.AP.allowCoroutines)
        {
            
            while (TrapCounts["Rat Gang"] <= 0)
            {
                if (!ArchipelagoClient.AP.allowCoroutines)
                {
                    yield break;
                }
                yield return null;
            }
            while (_isRatTrapped)
            {
                if (!ArchipelagoClient.AP.allowCoroutines)
                {
                    yield break;
                }
                yield return null;
            }
            while (EnemyHandler.RatPrefab is null)
            {
                if (!ArchipelagoClient.AP.allowCoroutines)
                {
                    yield break;
                }
                yield return null;
            }
            if (!ArchipelagoClient.AP.allowCoroutines)
            {
                TrapCounts["Rat Gang"] = 0;
                yield break;
            }
            while (!ArchipelagoClient.AP.IsInNormalGameState())
            {
                yield return null;
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
            TrapCounts["Rat Gang"] = Math.Max(0,  TrapCounts["Rat Gang"] - 1);
            yield return new WaitForSeconds(15f);
            _isRatTrapped = false;
        }
    }

    public IEnumerator GoDateDeathWhenNotDoingSoAlready()
    {
        while (ArchipelagoClient.AP.allowCoroutines)
        {
            
            while (TrapCounts["Date With Death Trap"] <= 0)
            {
                if (!ArchipelagoClient.AP.allowCoroutines)
                {
                    yield break;
                }
                yield return null;
            }
            if (!ArchipelagoClient.AP.allowCoroutines)
            {
                TrapCounts["Date With Death Trap"] = 0;
                yield break;
            }
            while (!ArchipelagoClient.AP.IsInNormalGameState())
            {
                yield return null;
            }
            var con = GameObject.Find("CONTROL").GetComponent<CONTROL>();
            while (!DoesPlayerHaveSpiritWarp(con))  // The player should be able to actually escape.
            {
                yield return new WaitForSeconds(3f);
                con = GameObject.Find("CONTROL").GetComponent<CONTROL>();
            }
            while (SceneManager.GetActiveScene().name == "DETHLAND")
            {
                yield return null;
            }
            con = GameObject.Find("CONTROL").GetComponent<CONTROL>();
            TrapCounts["Date With Death Trap"] -= 1;
            var component = con.MENU.ITEMS[20].GetComponent<GOTO_LEVEL>();
            component.LVL = "DETHLAND";
            component.POS = new Vector3(0f, 1f, 0f);
            component.ROT = 0f;
            component.gameObject.SetActive(value: true);
            yield return new WaitForSeconds(15f);
        }
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