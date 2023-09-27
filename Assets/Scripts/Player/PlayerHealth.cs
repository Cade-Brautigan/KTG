using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Mirror;
using UnityEngine.Networking.Types;

public class PlayerHealth : NetworkBehaviour
{
    #region Data
    [SerializeField] [SyncVar(hook = nameof(OnHealthChanged))] int health;
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

        CmdRequestUpdateMaxHealth();
        CmdRequestHealFull();
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
    public void CmdRequestUpdateMaxHealth()
    {
        ServerUpdateMaxHealth();
    }

    [Server]
    public void ServerUpdateMaxHealth()
    {
        if (leveling != null)
        {
            maxHealth = leveling.Level * 100;
        }
    }

    [Command]
    public void CmdRequestHealFull() 
    {
        ServerHealFull();
    }

    [Server]
    public void ServerHealFull()
    { 
        health = maxHealth;
    }

    [Server]
    public void ServerTakeDamage(int damage)
    {
        health -= damage;
        if (health <= 0)
        {
            ServerDie();
        }
    }

    [Server]
    private void ServerDie()
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
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!isServer) return;

        if (other.gameObject.tag == "Bullet")
        {
            uint networkid = other.gameObject.GetComponent<NetworkIdentity>().netId;
            ServerBulletCollision(networkid);
        }
    }


    [Server]
    private void ServerBulletCollision(uint networkid)
    {
        GameObject bullet = NetworkServer.spawned[networkid].gameObject;
        if (bullet != null)
        {
            // TODO: Server-side hit validation (maybe put on bullet?)
            NetworkServer.Destroy(bullet);
            ServerTakeDamage(50);
        }
    }
    #endregion

}
