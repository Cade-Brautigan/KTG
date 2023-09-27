using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEditor.SearchService;
using Mirror.Discovery;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    CustomNetworkManager networkManager;
    NetworkDiscovery networkDiscovery;

    GameObject mainMenu;
    Button quickplayButton, lanButton;
    TMP_InputField nicknameInput;
    string defaultName = "Player";

    GameObject lanServerBrowser;
    Button lanBackButton, lanHostButton;
    Transform lanScrollViewContent;
    [SerializeField] GameObject serverEntryPrefab; // Prefab for the server entry.
    Transform tablePanel; // Parent container for the server entries.

    private void Awake() 
    {
        networkManager = GameObject.Find("NetworkManager").GetComponent<CustomNetworkManager>();
        networkDiscovery = GameObject.Find("NetworkManager").GetComponent<NetworkDiscovery>();
        networkDiscovery.OnServerFound.AddListener(OnServerFound);

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

    private void QuickplayButtonPressed()
    {
        Debug.Log("Quickplay button pressed");
    }

    private void LANButtonPressed()
    {
        Debug.Log("LAN button pressed");
        mainMenu.SetActive(false);
        lanServerBrowser.SetActive(true);
        networkDiscovery.StartDiscovery();
    }

    private void LANBackButtonPressed()
    {
        lanServerBrowser.SetActive(false);
        mainMenu.SetActive(true);
    }

    private void LANHostButtonPressed()
    {
        //SceneManager.SetActiveScene("GameplayScene");
    }

    private void OnServerFound(ServerResponse serverResponse)
    {
        GameObject newEntry = Instantiate(serverEntryPrefab, lanScrollViewContent);
        ServerEntry serverEntryScript = newEntry.GetComponent<ServerEntry>();
        
        if (serverEntryScript != null)
        {
            serverEntryScript.Initialize(serverResponse);
        }
    }

}
