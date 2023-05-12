using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class PlayerHealth : MonoBehaviour
{
    int health;
    int maxHealth;
    PlayerUI UI;
    PlayerLeveling leveling;
    GameObject lastHitBy;

    void Awake() {
        UI = transform.GetComponent<PlayerUI>();
        leveling = transform.GetComponent<PlayerLeveling>();
        MaxHealth = leveling.Level;
        HealFull();
    }

    public int Health 
    {
        get { return health; }
    }

    public int MaxHealth 
    {
        get { return maxHealth; }
        set { maxHealth = value * 100; }
    }

    public void HealFull() { 
        health = maxHealth;
        UI.UpdateHealthbar();
    }

    public void TakeDamage(int damage) {
        health -= damage;
        UI.UpdateHealthbar();
        if (health <= 0) die();
    }

    void die() {
        PlayerShooting[] players = FindObjectsOfType<PlayerShooting>();
        foreach (PlayerShooting shooter in players) {
            if (shooter.LastPlayerHit == gameObject) {
                shooter.SendMessage("KillReward", leveling.Level, SendMessageOptions.DontRequireReceiver);
            }
        }
        Destroy(transform.gameObject);
    }

    void OnCollisionEnter2D(Collision2D other) {
        if (other.gameObject.tag == "Bullet") {
            TakeDamage(50);
        }
    }



















    

}
