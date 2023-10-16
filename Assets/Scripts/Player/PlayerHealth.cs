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
    PlayerMovement movement;
    PlayerShooting shooting;
    #endregion

    #region Start & Update
    void Awake()
    {
        UI = transform.GetComponent<PlayerUI>();
        leveling = transform.GetComponent<PlayerLeveling>();
        movement = transform.GetComponent<PlayerMovement>();
        shooting = transform.GetComponent<PlayerShooting>();
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
    #endregion

    #region Death & Respawn
    [Server]
    private void ServerDie()
    {
        movement.ServerDisableMovement();
        shooting.ServerDisableShooting();
        RpcDie();

        StartCoroutine(ServerRespawn());
    }

    [ClientRpc]
    private void RpcDie()
    {
        UI.DisableUI();
        GetComponent<SpriteRenderer>().enabled = false;
        GetComponent<Collider2D>().enabled = false;
    }

    [Server]
    private IEnumerator ServerRespawn()
    {
        yield return new WaitForSeconds(3f);

        leveling.ServerResetLevel();
        ServerHealFull();

        GameObject spawnPoint = GameObject.Find("SpawnPoint");
        transform.position = spawnPoint.transform.position;

        movement.ServerEnableMovement();
        shooting.ServerEnableShooting();

        RpcRespawn();
    }

    [ClientRpc]
    private void RpcRespawn()
    {
        UI.EnableUI();
        GetComponent<SpriteRenderer>().enabled = true;
        GetComponent<Collider2D>().enabled = true;
    }
    #endregion

}
