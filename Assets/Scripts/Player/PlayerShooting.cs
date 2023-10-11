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
    [SyncVar] float lastShot = 0f;

    [SerializeField] GameObject bulletPrefab;
    Transform firepoint;
    GameObject lastPlayerHit;
    PlayerLeveling leveling;
    [SerializeField] AudioClip gunshotSound;
    AudioSource audioSource;

    public GameObject LastPlayerHit
    {
        get { return lastPlayerHit; }
    }
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
    #endregion

    #region Shooting System
    public void Shoot()
    {
        if (!isLocalPlayer) return;

        if (Time.time > fireRate + lastShot)
        {
            if (gunshotSound != null) 
            {
                audioSource.PlayOneShot(gunshotSound);
            }
            CmdShoot();
        }
    }

    [Command]
    public void CmdShoot()
    {
        if (Time.time > fireRate + lastShot)
        {
            GameObject bullet = Instantiate(bulletPrefab, firepoint.position, firepoint.rotation);
            bullet.GetComponent<BulletController>().shooter = this;
            Rigidbody2D bulletBody = bullet.GetComponent<Rigidbody2D>();
            bulletBody.velocity = firepoint.right * bulletForce;
            NetworkServer.Spawn(bullet); // Spawn the bullet for all the clients
            lastShot = Time.time;
            RpcShoot();
        }
    }

    [ClientRpc]
    public void RpcShoot()
    {
        if (isLocalPlayer) return;

        if (gunshotSound != null)
        {
            audioSource.PlayOneShot(gunshotSound);
        }
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

    public void OnPlayerHit(GameObject other)
    {
        lastPlayerHit = other;
    }

    void KillReward(int otherPlayerLevel)
    {
        // leveling.CmdGainXP(otherPlayerLevel);
    }
    #endregion
}