using UnityEngine;
using Mirror;
using UnityEngine.SceneManagement;
using Mirror.Discovery;

public class CustomNetworkManager : NetworkManager
{
    GameObject spawnPoint;
    CustomNetworkDiscovery networkDiscovery;
    bool hosting;

    public override void Start()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
        networkDiscovery = transform.GetComponent<CustomNetworkDiscovery>();

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
        Debug.Log("OnServerAddPlayer called");
        
        GameObject player = Instantiate(playerPrefab, spawnPoint.transform.position, spawnPoint.transform.rotation);

        if (player != null)
        {
            Debug.Log("Player object is not null");
        }
        else
        {
            Debug.Log("Player object is null");
        }

        NetworkServer.AddPlayerForConnection(conn, player); // Associate the GameObject with a network connection
    }

    public override void OnServerDisconnect(NetworkConnectionToClient conn)
    {
        if (conn.identity != null)
        {
            NetworkServer.Destroy(conn.identity.gameObject);
        }
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.name == "GameplayScene")
        {
            spawnPoint = Instantiate(new GameObject("SpawnPoint"));
            spawnPoint.transform.position = Vector3.zero;
            if (hosting) 
            {
                StartHost();
                networkDiscovery.AdvertiseServer();
            } 
            else 
            {
                Debug.Log("Client starting");
                StartClient(); // After this is called, OnServerAddPLayer is called on the server
            }
        }
    }

    public void StartLANHost()
    {
        hosting = true;
        SceneManager.LoadScene("GameplayScene");
    }

    public void ConnectToLANServer(DiscoveryResponse serverData)
    {
        networkDiscovery.StopSearchForServers();
        hosting = false;
        networkAddress = serverData.ip;
        SceneManager.LoadScene("GameplayScene");
    }

}