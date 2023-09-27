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
    ServerResponse serverInfo;

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

    public void Initialize(ServerResponse serverResponse)
    {
        // Assuming serverResponse has custom attributes like numberOfPlayers and pingValue
        // serverName.text = serverResponse.serverName; // set the server name
        // gamemode.text = serverResponse.gameMode; // set the game mode
        // players.text = serverResponse.numberOfPlayers.ToString() + "/Max"; // set the number of players
        // ping.text = serverResponse.pingValue.ToString() + " ms"; // set the ping


    }

    private void ConnectToServer()
    {
        if (isServerInfoSet) 
        {
            networkManager.StartClient(serverInfo.uri);
        }
    }
}
