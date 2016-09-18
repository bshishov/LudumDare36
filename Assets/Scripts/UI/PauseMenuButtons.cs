using UnityEngine;
using Assets.Scripts;
using UnityEngine.SceneManagement;

public class PauseMenuButtons : MonoBehaviour {

    public GameObject PlayButton;
    public GameObject GameOverPanel;

    public void OnContinue()
    {
    }

    public void OnReplay()
    {
        var networkManager = GameObject.Find("NetworkManager").GetComponent<CustomNetworkManager>();
        networkManager.ServerChangeScene(SceneManager.GetActiveScene().name);
    }

    public void OnExit()
    {
        SceneManager.LoadScene("MainMenu");
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
