using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Microsoft.Unity.VisualStudio.Editor;
using Mirror.Discovery;
using UnityEngine.UI;

public class ServerEntry : MonoBehaviour
{
    TextMeshProUGUI  serverName, gamemode, players, ping;
    UnityEngine.UI.Image thumbnail;
    Button joinButton;

    private bool isServerInfoSet = false;
    CustomServerResponse serverInfo;

    CustomNetworkManager networkManager;

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

    public void Initialize(CustomServerResponse serverResponse)
    {
        serverInfo = serverResponse;
        isServerInfoSet = true;

        serverName.text = serverResponse.ServerName;
        gamemode.text = serverResponse.GameMode;
        players.text = serverResponse.NumPlayers.ToString() + "/Max";
        ping.text = serverResponse.Ping.ToString() + " ms";
    }

    private void ConnectToServer()
    {
        if (isServerInfoSet) 
        {
            networkManager.StartClient(serverInfo.ServerResponse.uri);
        }
    }
}
