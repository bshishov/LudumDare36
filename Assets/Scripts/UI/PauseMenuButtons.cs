using UnityEngine;
using Assets.Scripts;

public class PauseMenuButtons : MonoBehaviour {
    
    public void OnContinue()
    {
        gameObject.SetActive(false);
    }

    public void OnExit()
    {
        Application.LoadLevel("MainMenu");
        var networkManager = GameObject.Find("NetworkManager").GetComponent<CustomNetworkManager>();
        switch (networkManager.NetworkingMode)
        {
            case CustomNetworkManager.NetworkingModes.Host:
                networkManager.StopHost();
                break;
            case CustomNetworkManager.NetworkingModes.Client:
                networkManager.StopClient();
                break;
        }
        networkManager.NetworkingMode = CustomNetworkManager.NetworkingModes.None;
    }
}
