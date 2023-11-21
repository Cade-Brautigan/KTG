using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using Newtonsoft.Json.Linq;

public class PlayerShooting : NetworkBehaviour
{
    #region Data
    // Internal Data
    [SyncVar] float bulletForce = 10f;
    [SyncVar] float fireRate = 0.5f;
    [SyncVar] bool canShoot = true;
    float lastShot = 0f;
    Joystick shootingJoystick;
    Vector2 rotation = Vector2.zero; // Stores joystick values between Update and FixedUpdate
    [SyncVar] Vector2 networkedRotation = Vector2.zero;
    GameObject lastPlayerHit;
    PlayerHealth lastPlayerHitHealth;

    public Vector2 Rotation => rotation;
    public Vector2 NetworkedRotation => networkedRotation;
    
    Transform firepoint;
    [SyncVar] float firepointRadius = 0.5f; // TODO get firepoint radius directly from firepoint somehow

    // Visual
    [SerializeField] GameObject bulletPrefab;
    
    // Sister Components
    PlayerLeveling leveling;
    PlayerAudio playerAudio;
    #endregion

    #region Start & Update
    void Awake()
    {
        firepoint = transform.Find("Firepoint").transform;
        
        leveling = transform.GetComponent<PlayerLeveling>();
        playerAudio = transform.GetComponent<PlayerAudio>();
    }

    void Start()
    {
        if (!isLocalPlayer) return;

        shootingJoystick = GameObject.Find("Canvas/RightJoystick").GetComponent<Joystick>();

        CmdRequestUpdateFireRate();
    }

    void Update() 
    {
        if (shootingJoystick)
        {
            rotation = shootingJoystick.Direction;
            CmdUpdateNetworkedRotation(rotation);
        }

        if (isServer) 
        {
            CheckLastHitPlayer();
        }
    }

    void FixedUpdate()
    {
        Rotate(rotation);

        if (rotation != Vector2.zero)
        {
            Shoot();
        }
    }
    #endregion

    #region Shooting System
    public void Shoot()
    {
        if (!isLocalPlayer) return;

        if ((Time.time > fireRate + lastShot) && canShoot)
        {
            playerAudio.PlayGunshotSound();

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
            bullet.GetComponent<BulletController>().shooterGameObject = gameObject;
            Rigidbody2D bulletBody = bullet.GetComponent<Rigidbody2D>();
            bulletBody.velocity = firepoint.right * bulletForce;
            NetworkServer.Spawn(bullet); // Spawn the bullet for all the clients
            lastShot = Time.time;
            RpcShoot();
        }
    }

    [ClientRpc]
    private void RpcShoot() 
    {
        if (isLocalPlayer) return;

        playerAudio.PlayGunshotSound();
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
            if (leveling.Level < 20)
            {
                fireRate = 1f - (0.045f * (leveling.Level - 1));
            }
            else
            {
                fireRate = 0.1f;
            }
            
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

    #region Rotation
    private void Rotate(Vector2 rotation)
    {
        if (!isLocalPlayer) return;
        
        float radians = Mathf.Atan2(rotation.y, rotation.x);
        float degrees = radians * Mathf.Rad2Deg;

        float xPos = Mathf.Cos(radians) * firepointRadius;
        float yPos = Mathf.Sin(radians) * firepointRadius;

        Vector3 relativePos = new Vector3(xPos, yPos, 0);

        if (rotation != Vector2.zero)
        {
            firepoint.position = transform.position + relativePos;
            firepoint.eulerAngles = new Vector3(0f, 0f, degrees);
        }
        CmdRotate(rotation);
    }

    [Command]
    private void CmdRotate(Vector2 rotation)
    {
        if (!isLocalPlayer)
        {
            float radians = Mathf.Atan2(rotation.y, rotation.x);
            float degrees = radians * Mathf.Rad2Deg;
            
            float xPos = Mathf.Cos(radians) * firepointRadius;
            float yPos = Mathf.Sin(radians) * firepointRadius;

            Vector3 relativePos = new Vector3(xPos, yPos, 0);

            if (rotation != Vector2.zero)
            {
                firepoint.position = transform.position + relativePos;
                firepoint.eulerAngles = new Vector3(0f, 0f, degrees);
            }
        }
        RpcRotate(rotation);
    }

    [ClientRpc]
    private void RpcRotate(Vector2 rotation)
    {
        if (isLocalPlayer) return;

        float radians = Mathf.Atan2(rotation.y, rotation.x);
        float degrees = radians * Mathf.Rad2Deg;
        
        float xPos = Mathf.Cos(radians) * firepointRadius;
        float yPos = Mathf.Sin(radians) * firepointRadius;

        Vector3 relativePos = new Vector3(xPos, yPos, 0);

        if (rotation != Vector2.zero)
        {
            firepoint.position = transform.position + relativePos;
            firepoint.eulerAngles = new Vector3(0f, 0f, degrees);
        }
    }

    [Command]
    private void CmdUpdateNetworkedRotation(Vector2 rotation)
    {
        networkedRotation = rotation;
    }
    #endregion
}