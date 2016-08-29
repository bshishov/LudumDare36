using UnityEngine;
using System.Collections;
using Assets.Scripts;

public class PauseMenuButtons : MonoBehaviour {
    
    public void OnContinue()
    {
        gameObject.SetActive(false);
    }

    public void OnExit()
    {
        Application.LoadLevel("MainMenu");
        GameObject.Find("NetworkManager").GetComponent<CustomNetworkManager>().StopHost();
    }
}
