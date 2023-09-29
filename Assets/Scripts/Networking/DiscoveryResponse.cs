using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public struct DiscoveryResponse : NetworkMessage
{
    public string serverName;
    public string gameMode;
    public string mapName;
    public int numPlayers;
    public int maxPlayers;
    public string ip;
    public int ping;
}