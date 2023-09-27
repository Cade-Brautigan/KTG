using UnityEngine;
using Mirror;

public class CustomNetworkManager : NetworkManager
{
    public Transform spawnPoint;
    private int clientCount = 0;

    public override void Start()
    {
        string[] args = System.Environment.GetCommandLineArgs();
        for (int i = 0; i < args.Length; i++)
        {
            if (args[i] == "-launch-as-server")
            {
                Debug.Log("Starting server");
                StartServer();
            }
        }
    }

    public override void OnServerAddPlayer(NetworkConnectionToClient conn)
    {
        GameObject player = Instantiate(playerPrefab, spawnPoint.position, spawnPoint.rotation);
        NetworkServer.AddPlayerForConnection(conn, player); // Associate the GameObject with a network connection

        if (conn == NetworkServer.localConnection)
        {
            player.name = "Host";
        }
        else
        {
            clientCount++;
            player.name = "Client " + clientCount;
        }
    }

    public override void OnServerDisconnect(NetworkConnectionToClient conn)
    {
        if (conn.identity != null)
        {
            if (conn != NetworkServer.localConnection)
            {
                clientCount--;
            }
            NetworkServer.Destroy(conn.identity.gameObject);
        }
    }

}