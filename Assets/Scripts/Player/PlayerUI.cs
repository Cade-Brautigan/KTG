using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using Mirror;
using Newtonsoft.Json.Linq;

public class PlayerUI : NetworkBehaviour
{
    #region Data
    [SyncVar] float offset = 0.5f;
    [SyncVar(hook = nameof(OnNameChanged))] string playerName;

    public string PlayerName => playerName;

    RectTransform playerUI;
    RectTransform panel;
    TextMeshPro nameUI;
    TextMeshPro levelUI;
    Image healthbar;

    PlayerLeveling leveling;
    PlayerHealth health;
    #endregion

    #region Start & Update
    void Awake() 
    {
        playerUI = transform.Find("PlayerUI").GetComponent<RectTransform>();
        panel = playerUI.Find("Panel").GetComponent<RectTransform>();
        nameUI = panel.Find("Player Name").GetComponent<TextMeshPro>();
        levelUI = panel.Find("Player Level").GetComponent<TextMeshPro>();
        healthbar = playerUI.Find("Player Health").GetComponent<Image>();

        leveling = transform.GetComponent<PlayerLeveling>();
        health = transform.GetComponent<PlayerHealth>();
    }

    void Start()  
    {
        playerUI.position = playerUI.parent.position + Vector3.up * offset;
        playerUI.rotation = Quaternion.LookRotation(Vector3.forward, Vector3.up);

        Canvas canvas = playerUI.GetComponent<Canvas>();
        if (canvas != null)
        {
            // Force UI to overlap other player sprites
            canvas.sortingLayerName = "UI";
        }

        UpdateLevelText();

        if (isLocalPlayer)
        {
            string name = PlayerPrefs.GetString("PlayerName", "Player");
            CmdChangeNameTo(name);
        }
    }

    void Update()
    {
        if (!isLocalPlayer) return;

        playerUI.position = playerUI.parent.position + Vector3.up * offset;
        playerUI.rotation = Quaternion.LookRotation(Vector3.forward, Vector3.up);
    }
    #endregion

    #region UI Updates
    [Command]
    private void CmdChangeNameTo(string name)
    {
        playerName = name;
    }

    private void OnNameChanged(string oldValue, string newValue)
    {
        nameUI.text = newValue;
        gameObject.name = "Player (" + newValue + ")";
        // Force recaclulation of horizontal layout group spacing
        LayoutRebuilder.ForceRebuildLayoutImmediate(panel);
    }

    public void UpdateLevelText()
    {
        if (leveling != null)
        {
            levelUI.text = leveling.Level.ToString();
        }
    }

    public void UpdateHealthbar()
    {
        if (health != null)
        {
            healthbar.fillAmount = (float)(health.Health) / (float)(health.MaxHealth);
        }
    }

    public void UpdateOffset()
    {
        if (leveling != null)
        {
            offset = 0.5f + (0.1f * (leveling.Level - 1));
        }
    }

    public void DisableUI()
    {
        nameUI.enabled = false;
        levelUI.enabled = false;
        healthbar.enabled = false;
    }

    public void EnableUI()
    {
        nameUI.enabled = true;
        levelUI.enabled = true;
        healthbar.enabled = true;
    }
    #endregion

}
