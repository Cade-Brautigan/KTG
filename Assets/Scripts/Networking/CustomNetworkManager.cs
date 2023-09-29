using UnityEngine;
using Mirror;
using UnityEngine.SceneManagement;

public class CustomNetworkManager : NetworkManager
{
    GameObject spawnPoint;
    int clientCount = 0;

    public override void Start()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;

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
        GameObject player = Instantiate(playerPrefab, spawnPoint.transform.position, spawnPoint.transform.rotation);
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

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        spawnPoint = Instantiate(new GameObject("SpawnPoint"));
        spawnPoint.transform.position = Vector3.zero;
    }

}