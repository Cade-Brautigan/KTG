using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Net;

public class ServerEntry : MonoBehaviour
{
    CustomNetworkManager networkManager;

    TextMeshProUGUI  serverName, gamemode, players, ping;
    UnityEngine.UI.Image thumbnail;
    Button joinButton;

    DiscoveryResponse serverData;
    IPEndPoint endpoint;
    bool isServerDataSet = false;

    private void Awake()
    {
        networkManager = GameObject.Find("NetworkManager").GetComponent<CustomNetworkManager>();

        serverName = transform.Find("Name").GetComponent<TextMeshProUGUI>();
        gamemode = transform.Find("Gamemode").GetComponent<TextMeshProUGUI>();
        players = transform.Find("Players").GetComponent<TextMeshProUGUI>();
        ping = transform.Find("Ping").GetComponent<TextMeshProUGUI>();
        thumbnail = transform.Find("Thumbnail").GetComponent<UnityEngine.UI.Image>();

        joinButton = transform.GetComponent<Button>();
        joinButton.onClick.AddListener(() => networkManager.ConnectToLANServer(serverData.ip));
    }

    public void Initialize(DiscoveryResponse serverData, IPEndPoint endpoint)
    {
        this.serverData = serverData;
        this.endpoint = endpoint;
        isServerDataSet = true;

        serverName.text = endpoint.Address + ":" + endpoint.Port; //serverData.serverName;
        gamemode.text = serverData.gameMode;
        players.text = serverData.numPlayers.ToString() + "/" + serverData.maxPlayers.ToString();
        ping.text = serverData.ping.ToString() + " ms";
    }

}