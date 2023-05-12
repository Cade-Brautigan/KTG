using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using TMPro;

/// <summary>
/// Player name input field. Let the user input his name, will appear above the player in the game.
/// </summary>
public class NicknameInput : MonoBehaviour 
{

    // Store the PlayerPref Key to avoid typos
    const string playerNamePrefKey = "PlayerName";
    string defaultName = "Player";
    TMP_InputField inputField;

    /// <summary>
    /// MonoBehaviour method called on GameObject by Unity during initialization phase.
    /// </summary>
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

}
