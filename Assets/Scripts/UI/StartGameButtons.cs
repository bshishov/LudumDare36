using UnityEngine;
using System.Collections;
using UnityEngine.Networking;

public class StartGameButtons : MonoBehaviour {

    NetworkManager _networkManager;

    public void Start()
    {
        _networkManager = GameObject.Find("NetworkManager").GetComponent<NetworkManager>();
    }

    public void OnHost()
    {
        //_networkManager.StartHost();
        _networkManager.StartHost();
        Application.LoadLevel("PlayerMovementTest");
    }

    public void OnConnect()
    {
        _networkManager.networkAddress = "localhost";
        _networkManager.networkPort = 7777;
        _networkManager.StartClient();
    }
}
