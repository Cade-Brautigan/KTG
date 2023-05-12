using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerShooting : MonoBehaviour
{

    float bulletForce = 10f;
    float fireRate = 0.5f;
    float lastShot = 0f;
    [SerializeField] GameObject bulletPrefab;
    Transform firepoint;
    GameObject lastPlayerHit;
    PlayerLeveling playerLeveling;

    void Awake() {
        firepoint = transform.Find("Firepoint").transform;
        playerLeveling = transform.GetComponent<PlayerLeveling>();
    }

    /*
    1. Bullet is created and referenced by this instance
    2. BulletController's PlayerShooting component, sourceWeapon, is set to this PlayerShooting instance
    3. If the bullet hits a player, that player's GameObject is stored here
    */
    public void Shoot() {
        if (Time.time > fireRate + lastShot) {
            GameObject bullet = Instantiate(bulletPrefab, firepoint.position, firepoint.rotation);
            bullet.GetComponent<BulletController>().shooter = this;
            Rigidbody2D bulletBody = bullet.GetComponent<Rigidbody2D>();
            bulletBody.AddForce(firepoint.right * bulletForce, ForceMode2D.Impulse);
            lastShot = Time.time;
            Debug.Log("shot");
        }
    }

    public float FireRate
    {
        get { return fireRate; }
        set { fireRate = 1f - (0.15f * (value - 1)); }
    }

    public GameObject LastPlayerHit {
        get { return lastPlayerHit; }
    }

    public void OnPlayerHit(GameObject other) {
        lastPlayerHit = other;
    }

    void KillReward(int otherPlayerLevel) {
        playerLeveling.GainXP(otherPlayerLevel);
    }

}
