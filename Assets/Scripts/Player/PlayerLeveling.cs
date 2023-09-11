using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class PlayerLeveling : NetworkBehaviour
{
    /*
    Under the philosophy that:
    Objects can only GET certain values from other objects.
    Objects can only SET the internal values of other objects indirectly through functions.
    */
    [SyncVar(hook = nameof(OnLevelChanged))] int level = 1;
    [SyncVar] int xp = 0;
    [SyncVar] float levelUpTime = 1f;
    [SyncVar] bool isLevelingUp = false;

    public int Level => level;
    public int XP => xp;

    PlayerHealth health;
    PlayerShooting shooting;
    PlayerMovement movement;
    PlayerUI UI;

    void Awake()
    {
        health = transform.GetComponent<PlayerHealth>();
        shooting = transform.GetComponent<PlayerShooting>();
        movement = transform.GetComponent<PlayerMovement>();
        UI = transform.GetComponent<PlayerUI>();
    }

    [Command]
    public void CmdGainXP(int amt)
    {
        xp += amt;
        if (xp >= level) StartCoroutine(LevelUp());
    }

    [Server]
    IEnumerator LevelUp()
    {
        if (isLevelingUp) {yield break;}
        isLevelingUp = true;
        while (xp >= level)
        {
            xp -= level;
            level += 1;
            health.CmdHealFull();
            yield return new WaitForSeconds(levelUpTime);
        }
        isLevelingUp = false;
    }

    void OnLevelChanged(int oldValue, int newValue)
    {
        if (!isLocalPlayer) return;
        health.CmdUpdateMaxHealth();
        movement.CmdUpdateMoveSpeed();
        movement.CmdUpdateScale();
        shooting.CmdUpdateFireRate();
        UI.CmdUpdateHealthbar();
        UI.CmdUpdateOffset();
    }

    void OnCollisionEnter2D(Collision2D other) {
        /*
        XP_Orb orb = other.gameObject.GetComponent<XP_Orb>();
        if (orb != null) {
            int xpValue = orb.GetXPValue();
            Destroy(other.gameObject);
            CmdGainXP(xpValue);
        }
        */
        if (other.gameObject.CompareTag("1XP Orb")) {
            Destroy(other.gameObject);
            CmdGainXP(1);
        }

        if (other.gameObject.CompareTag("5XP Orb")) {
            Destroy(other.gameObject);
            CmdGainXP(5);
        }

        if (other.gameObject.CompareTag("10XP Orb")) {
            Destroy(other.gameObject);
            CmdGainXP(10);
        }

    }
}
