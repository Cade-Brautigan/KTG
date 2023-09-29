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
    string defaultName = "Player";

    GameObject lanServerBrowser;
    Button lanBackButton, lanHostButton;
    Transform lanScrollViewContent;
    [SerializeField] GameObject serverEntryPrefab; // Prefab for the server entry.

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
        SceneManager.sceneLoaded += OnSceneLoaded;
        SceneManager.LoadScene("GameplayScene");
    }

    private void OnServerFound(DiscoveryResponse serverData)
    {
        GameObject newEntry = Instantiate(serverEntryPrefab, lanScrollViewContent);
        ServerEntry serverEntry = newEntry.GetComponent<ServerEntry>();

        if (serverEntry != null)
        {
            serverEntry.Initialize(serverData);
        }
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.name == "GameplayScene")
        {
            networkManager.StartHost();
            SceneManager.sceneLoaded -= OnSceneLoaded; // Important to unsubscribe
        }
    }
    #endregion

}
