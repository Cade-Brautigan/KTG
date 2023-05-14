using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DebugConsole : MonoBehaviour
{
    private TextMeshProUGUI consoleUI;

    void Start()
    {
        //DontDestroyOnLoad(this.gameObject);
        consoleUI = GetComponent<TextMeshProUGUI>();
    }

    // Called after Start
    private void OnEnable()
    {
        Application.logMessageReceived += HandleLog;
    }

    private void OnDisable()
    {
        Application.logMessageReceived -= HandleLog;
    }

    void HandleLog(string logString, string stackTrace, LogType type)
    {
        if (consoleUI != null) {
            consoleUI.text += "\n" + logString;
        }
    }
}
