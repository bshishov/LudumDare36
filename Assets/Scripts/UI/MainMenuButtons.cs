using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class MainMenuButtons : MonoBehaviour {

    public GameObject ErrorPopup;
    public GameObject ProcessingPopup;

    public SettingsMenuPanel SettingsPanel;

    public void Start()
    {
        if (SettingsPanel)
            SettingsPanel.InitializeSettings();
    }

    public void OnPlay()
    {
        SceneManager.LoadScene("NetworkConnection");
    }

    public void OnExit()
    {
        Application.Quit();
    }
}
