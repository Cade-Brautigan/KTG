using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using UnityEngine.Networking.Types;

public class PlayerLeveling : NetworkBehaviour
{
    #region Data
    /*
    Under the philosophy that:
    Objects can only GET certain values from other objects.
    Objects can only SET the internal values of other objects indirectly through functions.
    */
    [SyncVar(hook = nameof(OnLevelChanged))] int level = 1;
    [SyncVar] int xp = 0;
    [SyncVar] float levelUpTime = 1f;
    [SyncVar] bool isLevelingUp = false;

    public int Level => level;
    public int XP => xp;

    PlayerHealth health;
    PlayerShooting shooting;
    PlayerMovement movement;
    PlayerUI UI;
    #endregion

    #region Start & Update
    private void Awake()
    {
        health = transform.GetComponent<PlayerHealth>();
        shooting = transform.GetComponent<PlayerShooting>();
        movement = transform.GetComponent<PlayerMovement>();
        UI = transform.GetComponent<PlayerUI>();
    }
    #endregion

    #region Level System
    [Command]
    public void CmdGainXP(int amt)
    {
        xp += amt;
        if (xp >= level) StartCoroutine(LevelUp());
    }

    [Server]
    private IEnumerator LevelUp()
    {
        if (isLevelingUp) { yield break; }
        isLevelingUp = true;
        while (xp >= level)
        {
            xp -= level;
            level += 1;
            movement.CmdUpdateMoveSpeed();
            movement.CmdUpdateScale();
            shooting.CmdUpdateFireRate();
            health.CmdUpdateMaxHealth();
            health.CmdHealFull();
            yield return new WaitForSeconds(levelUpTime);
        }
        isLevelingUp = false;
    }
    #endregion

    #region Callbacks
    private void OnLevelChanged(int oldValue, int newValue)
    {
        UI.UpdateHealthbar();
        UI.UpdateOffset();
    }
    #endregion

    #region Collisions
    private void OnCollisionEnter2D(Collision2D other)
    {
        if (!isLocalPlayer) return;

        uint networkid = other.gameObject.GetComponent<NetworkIdentity>().netId;
        if (other.gameObject.CompareTag("1XP Orb"))
        {
            CmdSmallXpOrbPickup(networkid);
        }

        if (other.gameObject.CompareTag("5XP Orb"))
        {
            CmdMediumXpOrbPickup(networkid);
        }

        if (other.gameObject.CompareTag("10XP Orb"))
        {
            CmdLargeXpOrbPickup(networkid);
        }
    }

    [Command]
    private void CmdSmallXpOrbPickup(uint networkid)
    {
        GameObject orb = NetworkServer.spawned[networkid].gameObject;
        if (orb != null)
        {
            // TODO: Check if this is a valid pickup
            NetworkServer.Destroy(orb);
            CmdGainXP(1);
        }
    }

    [Command]
    private void CmdMediumXpOrbPickup(uint networkid)
    {
        GameObject orb = NetworkServer.spawned[networkid].gameObject;
        if (orb != null)
        {
            // TODO: Check if this is a valid pickup
            NetworkServer.Destroy(orb);
            CmdGainXP(5);
        }
    }

    [Command]
    private void CmdLargeXpOrbPickup(uint networkid)
    {
        GameObject orb = NetworkServer.spawned[networkid].gameObject;
        if (orb != null)
        {
            // TODO: Check if this is a valid pickup
            NetworkServer.Destroy(orb);
            CmdGainXP(10);
        }
    }
    #endregion
}
