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
    [SyncVar(hook = nameof(OnNameChanged))] string playerName = "Cade";
    [SyncVar] float offset = 0.5f;

    RectTransform playerUI;
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

        UpdateNameText();
        UpdateLevelText();
    }

    void Update()
    {
        if (!isLocalPlayer) return;

        playerUI.position = playerUI.parent.position + Vector3.up * offset;
        playerUI.rotation = Quaternion.LookRotation(Vector3.forward, Vector3.up);
    }
    #endregion

    #region Callbacks
    private void OnNameChanged(string oldValue, string newValue)
    {
        UpdateNameText();
    }
    #endregion

    #region UI Updates
    public void UpdateNameText()
    {
        nameUI.text = playerName;
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
    #endregion

}
