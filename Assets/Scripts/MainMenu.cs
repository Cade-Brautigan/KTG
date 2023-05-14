using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class MainMenu : MonoBehaviour
{
    // Networking
    HostScript hostScript;

    // Main Menu
    GameObject mainMenu;
    Button playButton;
    Button localMultiplayerButton;
    TMP_InputField nicknameInput;

    // Local Multiplayer Submenu
    GameObject localMultiplayerSubmenu;
    TMP_InputField roomNameInput;
    Button hostButton;
    Button joinButton;
    Button localMPBackButton;
    
    string defaultName = "Player";

    private void Awake() 
    {
        // Networking
        hostScript = transform.Find("Networking").GetComponent<HostScript>();

        // Main Menu
        mainMenu = transform.Find("MainMenu").gameObject;
        playButton = mainMenu.transform.Find("PlayButton").GetComponent<Button>();
        localMultiplayerButton = mainMenu.transform.Find("LocalMultiplayerButton").GetComponent<Button>();
        nicknameInput = mainMenu.transform.Find("NicknameInput").GetComponent<TMP_InputField>();

        playButton.onClick.AddListener(PlayButtonPressed);
        localMultiplayerButton.onClick.AddListener(LocalMultiplayerButtonPressed);

        // Local Multiplayer Submenu
        localMultiplayerSubmenu = transform.Find("LocalMultiplayerSubmenu").gameObject;
        roomNameInput = localMultiplayerSubmenu.transform.Find("RoomNameInput").GetComponent<TMP_InputField>();
        hostButton = localMultiplayerSubmenu.transform.Find("HostButton").GetComponent<Button>();
        joinButton = localMultiplayerSubmenu.transform.Find("JoinButton").GetComponent<Button>();
        localMPBackButton = localMultiplayerSubmenu.transform.Find("LocalMPBackButton").GetComponent<Button>();

        localMPBackButton.onClick.AddListener(ReturnToMainMenu);
        
    }

    public void LocalMultiplayerButtonPressed() 
    {
        mainMenu.SetActive(false);
        localMultiplayerSubmenu.SetActive(true);
    }

    public void ReturnToMainMenu() 
    {
        localMultiplayerSubmenu.SetActive(false);
        mainMenu.SetActive(true);
    }

    public void PlayButtonPressed()
    {
        Debug.Log("Play button pressed");
    }

    public void HostButtonPressed()
    {
        Debug.Log("Host button pressed");
        hostScript.HostGame();
    }




    /*
    void Start() {

        inputField = this.GetComponent<TMP_InputField>();

        if (inputField != null && PlayerPrefs.HasKey(playerNamePrefKey)) {
            defaultName = PlayerPrefs.GetString(playerNamePrefKey);
            inputField.text = defaultName;
        }

        SetPlayerName();
    }

    /// <summary>
    /// Sets the name of the player, and save it in the PlayerPrefs for future sessions.
    /// </summary>
    public void SetPlayerName() {

        string name = inputField.text;

        if (string.IsNullOrEmpty(name)) {
            return;
        }

        PhotonNetwork.NickName = name; // Sets the nickname
        PlayerPrefs.SetString(playerNamePrefKey, name); // Save for future sessions
    }
    */

}
