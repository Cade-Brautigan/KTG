using UnityEngine;
using System.Collections;
using Mirror.Discovery;
using Mirror;

public class CustomNetworkMessage : NetworkMessage
{
    ServerResponse serverResponse;
    string serverName;
    string gameMode;
    int numPlayers;
    int maxPlayers;
    int ping;

    public ServerResponse ServerResponse => serverResponse;
    public string ServerName => serverName;
    public string GameMode => gameMode;
    public int NumPlayers => numPlayers;
    public int MaxPlayers => maxPlayers;
    public int Ping => ping;

    public CustomServerResponse(ServerResponse serverResponse, string serverName, string gameMode, int numPlayers, int maxPlayers, int ping)
    {
        this.serverResponse = serverResponse;
        this.serverName = serverName;
        this.gameMode = gameMode;
        this.numPlayers = numPlayers;
        this.maxPlayers = maxPlayers;
        this.ping = ping;
    }
}


