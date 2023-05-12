using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Photon.Pun;

public class PlayButton : MonoBehaviour
{

    public NetworkManager networkManager;

    private void Awake() {
        networkManager = GameObject.Find("NetworkManager").GetComponent<NetworkManager>();
    }

    public void PlayButtonPressed()
    {
        Debug.Log("Play button pressed");
        // Connect to the Photon server
        networkManager.PlayRandom();
    }

}
