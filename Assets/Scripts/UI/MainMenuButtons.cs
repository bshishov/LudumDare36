using UnityEngine;
using System.Collections;

public class MainMenuButtons : MonoBehaviour {

    public GameObject ErrorPopup;
    public GameObject ProcessingPopup;

    public void OnPlay()
    {
        Application.LoadLevel("NetworkConnection");
    }

    public void OnExit()
    {
        Application.Quit();
    }
}
