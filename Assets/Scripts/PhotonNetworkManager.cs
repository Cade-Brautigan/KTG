/*
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using UnityEngine.SceneManagement;
using Photon.Realtime;


public class NetworkManager : MonoBehaviourPunCallbacks
{

    public GameObject playerPrefab;
    string gameVersion = "1";
    byte maxPlayersPerRoom = 10;

    private void Awake()
    {
        DontDestroyOnLoad(this.gameObject);
        PhotonNetwork.AutomaticallySyncScene = true;
        PhotonNetwork.GameVersion = gameVersion;
        PhotonNetwork.SendRate = 60; // Set the send rate to 60 updates per second
        PhotonNetwork.SerializationRate = 60; // Set the serialization rate to 60 updates per second
        PhotonNetwork.ConnectUsingSettings();
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    // Activated by play button
    public void PlayRandom() 
    {
        if (!PhotonNetwork.IsConnectedAndReady) {
            PhotonNetwork.ConnectUsingSettings();
        } else {
            PhotonNetwork.JoinRandomRoom();
        }
    }

    // Delegate of SceneManager.sceneLoaded
    void OnSceneLoaded(Scene scene, LoadSceneMode mode) 
    {
        Debug.Log("Scene loaded: " + scene.name); 
        // MOVE THIS TO A GAMEOBJECT WITHIN GAMEPLAYSCENE THAT HANDLES SPAWNING
        if (SceneManager.GetSceneByName("GameplayScene").isLoaded) {
            PhotonNetwork.Instantiate(playerPrefab.name, RandomPointFromCenter(5f), Quaternion.identity);
        }
    }

    #region PUN CALLBACKS

    public override void OnJoinedRoom() 
    {
        Debug.Log("Joined room: " + PhotonNetwork.CurrentRoom.Name);
        SceneManager.LoadScene("Scenes/GameplayScene");
    }

    public override void OnConnectedToMaster() {
        Debug.Log("Connected to Photon server");
    }

    public override void OnJoinRandomFailed(short returnCode, string message) {
        Debug.Log("No suitable room found, creating new one");
        PhotonNetwork.CreateRoom(null, new RoomOptions { MaxPlayers = maxPlayersPerRoom });
    }

    public override void OnDisconnected(DisconnectCause cause) {
        Debug.LogWarningFormat("Disconnected from the server: {0}", cause);
    }

    public override void OnPlayerEnteredRoom(Player newPlayer) {
        Debug.Log(newPlayer.NickName + " has joined the game!");
    }

    public override void OnPlayerLeftRoom(Player otherPlayer) {
        Debug.Log(otherPlayer.NickName + " has left the game.");
    }

    #endregion

    private Vector3 RandomPointFromCenter(float radius) {
        float angle = Random.Range(0f, Mathf.PI * 2); // Random angle in radians
        float randomRadius = Mathf.Sqrt(Random.Range(0f, 1f)) * radius;
        float x = randomRadius * Mathf.Cos(angle);
        float y = randomRadius * Mathf.Sin(angle);
        return new Vector3(x, y, 0f);
    }

}
*/
