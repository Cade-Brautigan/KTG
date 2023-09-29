using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class ServerEntry : MonoBehaviour
{
    CustomNetworkManager networkManager;

    TextMeshProUGUI  serverName, gamemode, players, ping;
    UnityEngine.UI.Image thumbnail;
    Button joinButton;

    DiscoveryResponse serverData;
    bool isServerDataSet = false;

    private void Awake()
    {
        serverName = transform.Find("Name").GetComponent<TextMeshProUGUI>();
        gamemode = transform.Find("Gamemode").GetComponent<TextMeshProUGUI>();
        players = transform.Find("Players").GetComponent<TextMeshProUGUI>();
        ping = transform.Find("Ping").GetComponent<TextMeshProUGUI>();

        thumbnail = transform.Find("Thumbail").GetComponent<UnityEngine.UI.Image>();

        joinButton = transform.GetComponent<Button>();
        joinButton.onClick.AddListener(ConnectToServer);
    }

    public void Initialize(DiscoveryResponse serverData)
    {
        this.serverData = serverData;
        isServerDataSet = true;

        serverName.text = serverData.serverName;
        gamemode.text = serverData.gameMode;
        players.text = serverData.numPlayers.ToString() + "/" + serverData.maxPlayers.ToString();
        ping.text = serverData.ping.ToString() + " ms";
    }

    private void ConnectToServer()
    {
        if (isServerDataSet) 
        {
            //networkManager.StartClient(serverData.ip);
        }
    }
}
