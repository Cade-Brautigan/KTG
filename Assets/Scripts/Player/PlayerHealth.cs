using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Mirror;
using UnityEngine.Networking.Types;

public class PlayerHealth : NetworkBehaviour
{
    #region Data
    [SyncVar(hook = nameof(OnHealthChanged))] int health;
    [SyncVar] int maxHealth;

    public int Health => health;
    public int MaxHealth => maxHealth;

    GameObject lastHitBy;
    PlayerUI UI;
    PlayerLeveling leveling;
    #endregion

    #region Start & Update
    void Awake()
    {
        UI = transform.GetComponent<PlayerUI>();
        leveling = transform.GetComponent<PlayerLeveling>();
    }

    void Start()
    {
        if (!isLocalPlayer) return;

        CmdUpdateMaxHealth();
        CmdHealFull();
    }
    #endregion

    #region Callbacks
    public void OnHealthChanged(int oldValue, int newValue)
    {
        UI.UpdateHealthbar();
    }
    #endregion

    #region Health System
    [Command]
    public void CmdUpdateMaxHealth()
    {
        if (leveling != null)
        {
            maxHealth = leveling.Level * 100;
        }
    }

    [Command]
    public void CmdHealFull()
    { 
        health = maxHealth;
    }

    [Server]
    public void TakeDamage(int damage)
    {
        health -= damage;
        if (health <= 0)
        {
            Die();
        }
    }

    [Server]
    private void Die()
    {
        PlayerShooting[] players = FindObjectsOfType<PlayerShooting>();
        foreach (PlayerShooting shooter in players)
        {
            if (shooter.LastPlayerHit == gameObject)
            { 
                shooter.SendMessage("KillReward", leveling.Level, SendMessageOptions.DontRequireReceiver);
            }
        }
        Destroy(transform.gameObject);
    }
    #endregion

    #region Collisions
    private void OnCollisionEnter2D(Collision2D other)
    {
        if (!isLocalPlayer) return;

        uint networkid = other.gameObject.GetComponent<NetworkIdentity>().netId;
        if (other.gameObject.tag == "Bullet")
        {
            CmdBulletCollision(networkid);
        }
    }

    [Command]
    private void CmdBulletCollision(uint networkid)
    {
        GameObject bullet = NetworkServer.spawned[networkid].gameObject;
        if (bullet != null)
        {
            // TODO: Server-side hit validation (maybe put on bullet?)
            NetworkServer.Destroy(bullet);
            TakeDamage(50);
        }
    }
    #endregion

}
