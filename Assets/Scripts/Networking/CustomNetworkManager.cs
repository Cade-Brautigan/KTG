using UnityEngine;
using Mirror;

public class CustomNetworkManager : NetworkManager
{
    public Transform spawnPoint;

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
    }

    public override void OnServerDisconnect(NetworkConnectionToClient conn)
    {
        // If the connection has an associated player object, destroy it
        if (conn.identity != null)
        {
            NetworkServer.Destroy(conn.identity.gameObject);
        }
    }

}