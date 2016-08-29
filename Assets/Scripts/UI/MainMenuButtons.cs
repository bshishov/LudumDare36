using UnityEngine;
using System.Collections;

public class MainMenuButtons : MonoBehaviour { 

    public void OnPlay()
    {
        Application.LoadLevel("NetworkConnection");
    }

    public void OnExit()
    {
        Application.Quit();
    }
}
