using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using Photon.Pun;

public class PlayerUI : MonoBehaviour
{
    string playerName = "Cade";
    float offset = 0.5f;
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
        UpdateNameText();
        UpdateLevelText();
        playerUI.position = playerUI.parent.position + Vector3.up * offset;
        playerUI.rotation = Quaternion.LookRotation(Vector3.forward, Vector3.up);
    }

    void Update()
    {
        playerUI.position = playerUI.parent.position + Vector3.up * offset;
        playerUI.rotation = Quaternion.LookRotation(Vector3.forward, Vector3.up);
    }

    public float Offset
    {
        get { return offset; }
        set { offset = 0.5f + (0.1f * (value - 1)); }
    }

    public string PlayerName 
    { 
        get { return playerName; }
        set { playerName = value.ToString(); UpdateNameText(); }
    }

    public void UpdateNameText() {
        nameUI.text = PhotonNetwork.NickName;
    }

    public void UpdateLevelText() {
        levelUI.text = leveling.Level.ToString();
    }

    public void UpdateHealthbar() {
        if (health != null) {
            healthbar.fillAmount = (float)(health.Health) / (float)(health.MaxHealth);
        }
    }


}
