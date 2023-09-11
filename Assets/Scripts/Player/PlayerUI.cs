using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using Mirror;
using Newtonsoft.Json.Linq;

public class PlayerUI : NetworkBehaviour
{
    [SyncVar] string playerName = "Cade";
    [SyncVar] float offset = 0.5f;

    RectTransform playerUI;
    TextMeshPro nameUI;
    TextMeshPro levelUI;
    Image healthbar;

    PlayerLeveling leveling;
    PlayerHealth health;

    void Awake() 
    {
        playerUI = transform.Find("PlayerUI").GetComponent<RectTransform>();
        nameUI = playerUI.Find("Player Name").GetComponent<TextMeshPro>();
        levelUI = playerUI.Find("Player Level").GetComponent<TextMeshPro>();
        healthbar = playerUI.Find("Player Health").GetComponent<Image>();

        leveling = transform.GetComponent<PlayerLeveling>();
        health = transform.GetComponent<PlayerHealth>();
    }

    void Start()  
    {
        playerUI.position = playerUI.parent.position + Vector3.up * offset;
        playerUI.rotation = Quaternion.LookRotation(Vector3.forward, Vector3.up);

        if (isLocalPlayer)
        {
            CmdUpdateNameText();
            CmdUpdateLevelText();
        }
    }

    void Update()
    {
        if (!isLocalPlayer) return;
        playerUI.position = playerUI.parent.position + Vector3.up * offset;
        playerUI.rotation = Quaternion.LookRotation(Vector3.forward, Vector3.up);
    }

    [Command]
    public void CmdUpdateNameText()
    {
        nameUI.text = playerName;
        RpcUpdateNameText();
    }

    [ClientRpc]
    public void RpcUpdateNameText()
    {
        if (isLocalPlayer) return;
        nameUI.text = playerName;
    }

    [Command]
    public void CmdUpdateLevelText()
    {
        if (leveling != null)
        {
            levelUI.text = leveling.Level.ToString();
            RpcUpdateLevelText();
        }
    }

    [ClientRpc]
    public void RpcUpdateLevelText()
    {
        if (isLocalPlayer) return;
        if (leveling != null)
        { 
            levelUI.text = leveling.Level.ToString();
        }
    }

    [Command]
    public void CmdUpdateHealthbar()
    {
        if (health != null)
        { 
            healthbar.fillAmount = (float)(health.Health) / (float)(health.MaxHealth);
        }
    }

    [Command]
    public void CmdUpdateOffset()
    {
        if (leveling != null)
        {
            offset = 0.5f + (0.1f * (leveling.Level - 1));
        }
    }

}
