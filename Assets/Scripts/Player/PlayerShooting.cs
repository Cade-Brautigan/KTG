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

    public GameObject LastPlayerHit
    {
        get { return lastPlayerHit; }
    }
    #endregion

    #region Start & Update
    void Awake()
    {
        firepoint = transform.Find("Firepoint").transform;
        leveling = transform.GetComponent<PlayerLeveling>();
    }

    void Start()
    {
        if (!isLocalPlayer) return;

        CmdUpdateFireRate();
    }
    #endregion

    #region Shooting System
    //public void Shoot()
    //{
    //    if (!isLocalPlayer) return;


    //}

    [Command]
    public void CmdShoot()
    {
        //if (!isLocalPlayer) return;

        if (Time.time > fireRate + lastShot)
        {
            GameObject bullet = Instantiate(bulletPrefab, firepoint.position, firepoint.rotation);
            bullet.GetComponent<BulletController>().shooter = this;
            Rigidbody2D bulletBody = bullet.GetComponent<Rigidbody2D>();
            bulletBody.velocity = firepoint.right * bulletForce;

            // Spawn the bullet for all the clients
            NetworkServer.Spawn(bullet);

            lastShot = Time.time;
        }
    }

    [Command]
    public void CmdUpdateFireRate()
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
        leveling.CmdGainXP(otherPlayerLevel);
    }
    #endregion
}
