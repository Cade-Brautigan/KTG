using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror.Discovery;
using Mirror;
using System.Net;
using System;
using UnityEngine.SceneManagement;

public struct DiscoveryRequest : NetworkMessage
{
    // Add public fields (not properties) for whatever information you want
    // sent by clients in their broadcast messages that servers will use.
}

public struct DiscoveryResponse : NetworkMessage
{
    public string serverName;
    public string gameMode;
    public string mapName;
    public int numPlayers;
    public int maxPlayers;
    public string ip;
    public int port;
    public int ping;
}

public class CustomNetworkDiscovery : NetworkDiscoveryBase<DiscoveryRequest, DiscoveryResponse>
{
    public event Action<DiscoveryResponse> OnServerFoundEvent;
    MainMenu mainMenu;

    private void Awake()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.name == "MainMenu")
        {
            mainMenu = GameObject.Find("MainMenuCanvas").GetComponent<MainMenu>();
            Debug.Log("Main menu assigned");
        }
    }

    #region Server
    /// <summary>
    /// Reply to the client to inform it of this server
    /// </summary>
    /// <remarks>
    /// Override if you wish to ignore server requests based on
    /// custom criteria such as language, full server game mode or difficulty
    /// </remarks>
    /// <param name="request">Request coming from client</param>
    /// <param name="endpoint">Address of the client that sent the request</param>
    protected override void ProcessClientRequest(DiscoveryRequest request, IPEndPoint endpoint)
    {
        Debug.Log("ProcessClientRequest called");
        base.ProcessClientRequest(request, endpoint);
    }
    
    /// <summary>
    /// Process the request from a client
    /// </summary>
    /// <remarks>
    /// Override if you wish to provide more information to the clients
    /// such as the name of the host player
    /// </remarks>
    /// <param name="request">Request coming from client</param>
    /// <param name="endpoint">Address of the client that sent the request</param>
    /// <returns>A message containing information about this server</returns>
    protected override DiscoveryResponse ProcessRequest(DiscoveryRequest request, IPEndPoint endpoint) 
    {
        Debug.Log("ProcessRequest called");
        return new DiscoveryResponse
        {
            serverName = "Custom Server Name",
            gameMode = "Kill To Grow",
            mapName = "Expanse",
            numPlayers = NetworkServer.connections.Count,
            maxPlayers = 16,
            ip = NetworkManager.singleton.networkAddress,
            port = 7777,
        };
    }
    #endregion

    #region Client
    // Create a message that will be broadcasted on the network to discover servers
    protected override DiscoveryRequest GetRequest()
    {
        if (mainMenu != null)
        {
            mainMenu.ClearServerEntries();
            Debug.Log("server entries cleared");
        }
        Debug.Log("GetRequest called");
        return new DiscoveryRequest();
    }

    // Process the answer from a server
    protected override void ProcessResponse(DiscoveryResponse response, IPEndPoint endpoint) 
    {
        Debug.Log("ProcessResponse called");
        OnServerFoundEvent?.Invoke(response);
    }

    public void StartSearchForServers() 
    {
        StartDiscovery();
        Debug.Log("Searching for servers");
    }

    public void StopSearchForServers()
    {
        StopDiscovery();
        Debug.Log("Discovery stopped");
    }
    #endregion
}
