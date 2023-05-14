using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class HostScript : MonoBehaviour
{
    NetworkManager networkManager;
    string roomName;

    private void Awake() 
    {
        DontDestroyOnLoad(this.gameObject);
        networkManager = transform.GetComponent<NetworkManager>();
    }

    public void HostGame()
    {
        // Change to the gameplay scene.
        var sceneManager = UnityEngine.SceneManagement.SceneManager;
        sceneManager.LoadScene("Scenes/GameplayScene");

        // Wait for the scene to load.
        while (sceneManager.GetActiveScene() != "GameplayScene")
        {
            // Wait...
        }

        // Start the host.
        networkManager.Singleton.StartHost(roomName);
    }
}
