using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerLeveling : MonoBehaviour
{
    /*
    Under the philosophy that:
    Can only GET values from other objects, not SET
    Objects only SET their own values within functions
    */
    [SerializeField] int level = 1;
    int xp = 0;
    float levelUpTime = 1f;
    bool isLevelingUp = false;
    PlayerHealth health;
    PlayerShooting shooting;
    PlayerMovement movement;
    PlayerUI UI;

    void Awake() {
        health = transform.GetComponent<PlayerHealth>();
        shooting = transform.GetComponent<PlayerShooting>();
        movement = transform.GetComponent<PlayerMovement>();
        UI = transform.GetComponent<PlayerUI>();
    }

    public int Level => level;

    public int XP => xp;

    public void GainXP(int amt) {
        xp += amt;
        if (xp >= level) StartCoroutine(levelUp());
    }

    IEnumerator levelUp() {
        if (isLevelingUp) {yield break;}
        isLevelingUp = true;
        while (xp >= level) {
            xp -= level;
            level += 1;

            health.MaxHealth = level;
            health.HealFull(); // also updates health UI

            movement.MoveSpeed = level;
            movement.UpdateScale();

            shooting.FireRate = level;

            UI.Offset = level;
            UI.UpdateLevelText();

            yield return new WaitForSeconds(levelUpTime);
        }
        isLevelingUp = false;
    }

    void OnCollisionEnter2D(Collision2D other) {
        /*
        if (other.gameObject.tag == "XP Orb") {
            GainXP( some access of the gameobjects XPamt attribute. Maybe use deconstructor method? );
            Destroy(other.gameObject)
        }
        */
        if (other.gameObject.CompareTag("1XP Orb")) {
            Destroy(other.gameObject);
            GainXP(1);
        }

        if (other.gameObject.CompareTag("5XP Orb")) {
            Destroy(other.gameObject);
            GainXP(5);
        }

        if (other.gameObject.CompareTag("10XP Orb")) {
            Destroy(other.gameObject);
            GainXP(10);
        }

    }
}
