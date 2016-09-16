using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class MainMenuButtons : MonoBehaviour {

    public GameObject ErrorPopup;
    public GameObject ProcessingPopup;

    public void OnPlay()
    {
        SceneManager.LoadScene("NetworkConnection");
    }

    public void OnExit()
    {
        Application.Quit();
    }
}
