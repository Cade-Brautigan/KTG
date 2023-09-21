using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class MainMenu : MonoBehaviour
{
    // Networking

    // Main Menu
    GameObject mainMenu;
    Button playButton;
    TMP_InputField nicknameInput;
    
    string defaultName = "Player";

    private void Awake() 
    {

        // Main Menu
        mainMenu = transform.Find("MainMenu").gameObject;
        playButton = mainMenu.transform.Find("PlayButton").GetComponent<Button>();
        nicknameInput = mainMenu.transform.Find("NicknameInput").GetComponent<TMP_InputField>();

        playButton.onClick.AddListener(PlayButtonPressed);
        
    }

    public void PlayButtonPressed()
    {
        Debug.Log("Play button pressed");
    }

}
