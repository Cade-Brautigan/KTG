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
    [SerializeField] [SyncVar(hook = nameof(OnLevelChanged))] int level = 1;
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
    [Server]
    public void GainXP(int amt)
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
            movement.ServerUpdateMoveSpeed();
            movement.ServerUpdateScale();
            shooting.ServerUpdateFireRate();
            health.ServerUpdateMaxHealth();
            health.ServerHealFull();
            yield return new WaitForSeconds(levelUpTime);
        }
        isLevelingUp = false;
    }
    #endregion

    #region Callbacks
    private void OnLevelChanged(int oldValue, int newValue)
    {
        UI.UpdateLevelText();
        UI.UpdateHealthbar();
        UI.UpdateOffset();
    }
    #endregion

    #region Collisions
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!isServer) return;

        NetworkIdentity networkIdentity = other.gameObject.GetComponent<NetworkIdentity>();
        if (networkIdentity == null) return;
        uint networkid = networkIdentity.netId;

        if (other != null) 
        {
            networkid = other.gameObject.GetComponent<NetworkIdentity>().netId;
        }
        
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
            GainXP(1);
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
            GainXP(5);
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
            GainXP(10);
        }
    }
    #endregion
}
