using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Mirror.Discovery;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    CustomNetworkManager networkManager;
    CustomNetworkDiscovery networkDiscovery;

    GameObject mainMenu;
    Button quickplayButton, lanButton;
    TMP_InputField nicknameInput;

    GameObject lanServerBrowser;
    Button lanBackButton, lanHostButton;
    Transform lanScrollViewContent;
    [SerializeField] GameObject serverEntryPrefab;

    private void Awake() 
    {
        networkManager = GameObject.Find("NetworkManager").GetComponent<CustomNetworkManager>();
        networkDiscovery = GameObject.Find("NetworkManager").GetComponent<CustomNetworkDiscovery>();
        networkDiscovery.OnServerFoundEvent += OnServerFound;

        mainMenu = transform.Find("MainMenu").gameObject;
        quickplayButton = mainMenu.transform.Find("QuickplayButton").GetComponent<Button>();
        lanButton = mainMenu.transform.Find("LANButton").GetComponent<Button>();
        nicknameInput = mainMenu.transform.Find("NicknameInput").GetComponent<TMP_InputField>();
        quickplayButton.onClick.AddListener(QuickplayButtonPressed);
        nicknameInput.text = PlayerPrefs.GetString("PlayerName", "Player");
        nicknameInput.onDeselect.AddListener(ChangeNameTo);
        nicknameInput.onSubmit.AddListener(ChangeNameTo);
        lanButton.onClick.AddListener(LANButtonPressed);

        lanServerBrowser = transform.Find("LANServerBrowser").gameObject;
        lanBackButton = lanServerBrowser.transform.Find("BackButton").GetComponent<Button>();
        lanHostButton = lanServerBrowser.transform.Find("HostButton").GetComponent<Button>();
        lanScrollViewContent = lanServerBrowser.transform.Find("ScrollView/Viewport/Content");
        lanBackButton.onClick.AddListener(LANBackButtonPressed);
        lanHostButton.onClick.AddListener(LANHostButtonPressed);
    }

    #region Main Menu
    private void QuickplayButtonPressed()
    {
    }

    private void LANButtonPressed()
    {
        mainMenu.SetActive(false);
        lanServerBrowser.SetActive(true);
        networkDiscovery.SearchForServers();
    }

    private void ChangeNameTo(string name) 
    {
        if (name.Length != 0 && name.Length < 20)
        {
            Debug.Log("Player name changed to " + name);
            PlayerPrefs.SetString("PlayerName", name);
        }
        else
        {
            nicknameInput.text = PlayerPrefs.GetString("PlayerName", "Player");
        }
    }
    #endregion

    #region LAN Server Browser
    private void LANBackButtonPressed()
    {
        lanServerBrowser.SetActive(false);
        mainMenu.SetActive(true);
        networkDiscovery.StopSearchForServers();
    }

    private void LANHostButtonPressed()
    {
        networkManager.StartLANHost();
        networkDiscovery.StopSearchForServers();
    }

    private void OnServerFound(DiscoveryResponse serverData)
    {
        Debug.Log("Server Found!");
        GameObject newEntry = Instantiate(serverEntryPrefab, lanScrollViewContent);
        ServerEntry serverEntry = newEntry.GetComponent<ServerEntry>();

        if (serverEntry != null)
        {
            serverEntry.Initialize(serverData);
        }
    }

    public void ClearServerEntries()
    {
        foreach (Transform child in lanScrollViewContent)
        {
            Destroy(child.gameObject);
        }
    }

    #endregion

}
