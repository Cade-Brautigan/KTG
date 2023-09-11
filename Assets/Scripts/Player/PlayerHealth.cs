using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Mirror;

public class PlayerHealth : NetworkBehaviour
{
    [SyncVar(hook = nameof(OnHealthChanged))] int health;
    [SyncVar] int maxHealth;

    public int Health => health;
    public int MaxHealth => maxHealth;

    GameObject lastHitBy;
    PlayerUI UI;
    PlayerLeveling leveling;

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

    public void OnHealthChanged(int oldValue, int newValue)
    {
        if (!isLocalPlayer) return;
        UI.CmdUpdateHealthbar();
    }

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

    [Command]
    public void CmdTakeDamage(int damage)
    {
        health -= damage;
        if (health <= 0)
        {
            Die();
        }
    }

    [Server]
    void Die()
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

    void OnCollisionEnter2D(Collision2D other)
    {
        if (other.gameObject.tag == "Bullet")
        { 
            CmdTakeDamage(50);
        }
    }

}
