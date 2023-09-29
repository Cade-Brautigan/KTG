using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror.Discovery;
using Mirror;
using System.Net;
using System;

public class CustomNetworkDiscovery : NetworkDiscoveryBase<DiscoveryRequest, DiscoveryResponse>
{
    public event Action<DiscoveryResponse> OnServerFoundEvent;

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
            serverName = "Your Custom Server Name",
            gameMode = "Kill To Grow",
            mapName = "Expanse",
            numPlayers = NetworkServer.connections.Count,
            maxPlayers = 16,
            ip = endpoint.Address.ToString()
        };
    }
    #endregion

    #region Client
    /// <summary>
    /// Create a message that will be broadcasted on the network to discover servers
    /// </summary>
    /// <remarks>
    /// Override if you wish to include additional data in the discovery message
    /// such as desired game mode, language, difficulty, etc... </remarks>
    /// <returns>An instance of ServerRequest with data to be broadcasted</returns>
    protected override DiscoveryRequest GetRequest()
    {
        Debug.Log("GetRequest called");
        return new DiscoveryRequest();
    }

    /// <summary>
    /// Process the answer from a server
    /// </summary>
    /// <remarks>
    /// A client receives a reply from a server, this method processes the
    /// reply and raises an event
    /// </remarks>
    /// <param name="response">Response that came from the server</param>
    /// <param name="endpoint">Address of the server that replied</param>
    protected override void ProcessResponse(DiscoveryResponse response, IPEndPoint endpoint) 
    {
        Debug.Log("ProcessResponse called");
        OnServerFoundEvent?.Invoke(response);
    }

    public void SearchForServers() 
    {
        base.StartDiscovery();
        Debug.Log("Discovery started");
    }

    public void StopSearchForServers()
    {
        base.StopDiscovery();
        Debug.Log("Discovery stopped");
    }
    #endregion

}
