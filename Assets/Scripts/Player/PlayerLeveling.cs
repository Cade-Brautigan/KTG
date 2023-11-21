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
    Objects can only GET certain values from other objects through getter functions.
    Objects can only SET the internal values of other objects indirectly through functions.
    */
    [SerializeField] [SyncVar(hook = nameof(OnLevelChanged))] int level = 1;
    [SyncVar] int xp = 0;
    [SyncVar] bool isLevelingUp = false;

    public int Level => level;
    public int XP => xp;

    PlayerHealth health;
    PlayerShooting shooting;
    PlayerMovement movement;
    PlayerUI UI;
    PlayerAudio playerAudio;
    #endregion

    #region Start & Update
    private void Awake()
    {
        health = transform.GetComponent<PlayerHealth>();
        shooting = transform.GetComponent<PlayerShooting>();
        movement = transform.GetComponent<PlayerMovement>();
        UI = transform.GetComponent<PlayerUI>();
        playerAudio = transform.GetComponent<PlayerAudio>();
    }
    #endregion

    #region Level System
    [Server]
    public void ServerGainXP(int amt)
    {
        xp += amt;
        RpcGainXP();
        if (xp >= level) StartCoroutine(ServerLevelUp());
    }

    [ClientRpc]
    private void RpcGainXP()
    {
        playerAudio.PlayXpGainSound();
    }

    [Server]
    private IEnumerator ServerLevelUp()
    {
        if (isLevelingUp) { yield break; }
        isLevelingUp = true;
        while (xp >= level)
        {
            xp -= level;
            level += 1;
            ServerUpdateStats();
            health.ServerHealFull();
            RpcLevelUp();
            yield return new WaitForSeconds(1f);
        }
        isLevelingUp = false;
    }

    [ClientRpc]
    private void RpcLevelUp()
    {
        playerAudio.PlayLevelUpSound();
    }

    [Server]
    public void ServerResetLevel()
    {
        level = 1;
        xp = 0;
        ServerUpdateStats();
    }

    [Server]
    private void ServerUpdateStats()
    {
        movement.ServerUpdateMoveSpeed();
        movement.ServerUpdateScale();
        shooting.ServerUpdateFireRate();
        health.ServerUpdateMaxHealth();
    }
    #endregion

    #region Callbacks
    private void OnLevelChanged(int oldValue, int newValue)
    {
        UI.UpdateLevelText();
        UI.UpdateHealthbar();
        UI.UpdateOffset();
        playerAudio.UpdateAudio();
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
            ServerSmallXpOrbPickup(networkid);
        }

        if (other.gameObject.CompareTag("5XP Orb"))
        {
            ServerMediumXpOrbPickup(networkid);
        }

        if (other.gameObject.CompareTag("10XP Orb"))
        {
            ServerLargeXpOrbPickup(networkid);
        }
    }

    [Server]
    private void ServerSmallXpOrbPickup(uint networkid)
    {
        GameObject orb = NetworkServer.spawned[networkid].gameObject;
        if (orb != null)
        {
            // TODO: Check if this is a valid pickup
            NetworkServer.Destroy(orb);
            ServerGainXP(1);
        }
    }

    [Server]
    private void ServerMediumXpOrbPickup(uint networkid)
    {
        GameObject orb = NetworkServer.spawned[networkid].gameObject;
        if (orb != null)
        {
            // TODO: Check if this is a valid pickup
            NetworkServer.Destroy(orb);
            ServerGainXP(5);
        }
    }

    [Server]
    private void ServerLargeXpOrbPickup(uint networkid)
    {
        GameObject orb = NetworkServer.spawned[networkid].gameObject;
        if (orb != null)
        {
            // TODO: Check if this is a valid pickup
            NetworkServer.Destroy(orb);
            ServerGainXP(10);
        }
    }
    #endregion
}
