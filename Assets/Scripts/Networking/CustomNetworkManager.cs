using UnityEngine;
using Mirror;
using UnityEngine.SceneManagement;
using Mirror.Discovery;

public class CustomNetworkManager : NetworkManager
{
    GameObject spawnPoint;
    CustomNetworkDiscovery networkDiscovery;
    ConnectionType connectionType;

    public enum ConnectionType
    {
        HOST,
        CLIENT,
        SERVER
    }

    public override void Start()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
        networkDiscovery = transform.GetComponent<CustomNetworkDiscovery>();

        string[] args = System.Environment.GetCommandLineArgs();
        for (int i = 0; i < args.Length; i++)
        {
            if (args[i] == "-launch-as-server")
            {
                connectionType = ConnectionType.SERVER;
                SceneManager.LoadScene("GameplayScene");
            }
        }
    }

    public override void OnServerAddPlayer(NetworkConnectionToClient conn)
    {
        Debug.Log("OnServerAddPlayer called");
        GameObject player = Instantiate(playerPrefab, spawnPoint.transform.position, spawnPoint.transform.rotation);
        NetworkServer.AddPlayerForConnection(conn, player); // Associate the GameObject with a network connection
    }

    public override void OnServerDisconnect(NetworkConnectionToClient conn)
    {
        if (conn.identity != null)
        {
            NetworkServer.Destroy(conn.identity.gameObject);
        }
    }

    public override void OnClientDisconnect()
    {
        SceneManager.LoadScene("MainMenu");
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.name == "GameplayScene")
        {
            spawnPoint = Instantiate(new GameObject("SpawnPoint"));
            spawnPoint.transform.position = Vector3.zero;
            if (connectionType == ConnectionType.HOST) 
            {
                Debug.Log("Host starting");
                StartHost();
                networkDiscovery.AdvertiseServer();
            } 
            else if (connectionType == ConnectionType.CLIENT)
            {
                Debug.Log("Client starting");
                StartClient(); // After this is called, OnServerAddPLayer is called on the server
            }
            else if (connectionType == ConnectionType.SERVER)
            {
                Debug.Log("Server Starting");
                StartServer();
            }
        }
    }

    public void StartLANHost()
    {
        connectionType = ConnectionType.HOST;
        SceneManager.LoadScene("GameplayScene");
    }

    public void ConnectToLANServer(DiscoveryResponse serverData)
    {
        networkDiscovery.StopSearchForServers();
        connectionType = ConnectionType.CLIENT;
        networkAddress = serverData.ip;
        SceneManager.LoadScene("GameplayScene");
    }

}