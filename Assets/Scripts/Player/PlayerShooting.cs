using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using Newtonsoft.Json.Linq;

public class PlayerShooting : NetworkBehaviour
{
    #region Data
    [SyncVar] float bulletForce = 10f;
    [SyncVar] float fireRate = 0.5f;
    [SyncVar] bool canShoot = true;
    float lastShot = 0f;

    [SerializeField] GameObject bulletPrefab;
    Transform firepoint;
    GameObject lastPlayerHit;
    PlayerHealth lastPlayerHitHealth;

    AudioSource audioSource;
    [SerializeField] AudioClip gunshotSound;
    
    PlayerLeveling leveling;
    PlayerHealth health;
    #endregion

    #region Start & Update
    void Awake()
    {
        firepoint = transform.Find("Firepoint").transform;
        audioSource = GetComponent<AudioSource>();
        leveling = transform.GetComponent<PlayerLeveling>();
    }

    void Start()
    {
        if (!isLocalPlayer) return;

        CmdRequestUpdateFireRate();
    }

    void Update() 
    {
        if (!isServer) return;

        CheckLastHitPlayer();
    }
    #endregion

    #region Shooting System
    public void Shoot()
    {
        if (!isLocalPlayer) return;

        if ((Time.time > fireRate + lastShot) && canShoot)
        {
            if (gunshotSound != null) 
            {
                audioSource.PlayOneShot(gunshotSound);
            }

            if (!isServer)
            {
                lastShot = Time.time; // Do host lastShot update in Cmd
            }
            
            CmdShoot();
        }
    }

    [Command]
    public void CmdShoot()
    {
        if ((Time.time > fireRate + lastShot) && canShoot)
        {
            GameObject bullet = Instantiate(bulletPrefab, firepoint.position, firepoint.rotation);
            bullet.GetComponent<BulletController>().shooter = this;
            Rigidbody2D bulletBody = bullet.GetComponent<Rigidbody2D>();
            bulletBody.velocity = firepoint.right * bulletForce;
            NetworkServer.Spawn(bullet); // Spawn the bullet for all the clients
            lastShot = Time.time;
        }
    }

    [Server]
    public void ServerDisableShooting()
    {
        firepoint.GetComponent<SpriteRenderer>().enabled = false;
        canShoot = false;
    }

    [Server]
    public void ServerEnableShooting()
    {
        firepoint.GetComponent<SpriteRenderer>().enabled = true;
        canShoot = true;
    }

    [Command]
    public void CmdRequestUpdateFireRate()
    {
        ServerUpdateFireRate();
    }

    [Server]
    public void ServerUpdateFireRate()
    {
        if (leveling != null)
        {
            fireRate = 1f - (0.15f * (leveling.Level - 1));
        }
    }

    [Server]
    public void ServerOnPlayerHit(GameObject other)
    {
        lastPlayerHit = other;
        lastPlayerHitHealth = other.GetComponent<PlayerHealth>();
    }

    [Server]
    void CheckLastHitPlayer()
    {
        if (lastPlayerHitHealth != null && lastPlayerHitHealth.Health <= 0)
        {
            int lastPlayerHitLevel = lastPlayerHit.GetComponent<PlayerLeveling>().Level;
            leveling.ServerGainXP(lastPlayerHitLevel);

            lastPlayerHit = null;
            lastPlayerHitHealth = null;
        }
    }
    #endregion
}