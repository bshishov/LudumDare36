using UnityEngine;
using System.Collections;
using UnityEngine.Networking;
using UnityEngine.UI;

public class StartGameButtons : MonoBehaviour {

    NetworkManager _networkManager;

    public void Start()
    {
        _networkManager = GameObject.Find("NetworkManager").GetComponent<NetworkManager>();
    }

    public void OnHost()
    {
        _networkManager.networkPort = 15678;
        _networkManager.StartHost();
        Application.LoadLevel("PlayerMovementTest");
    }

    public void OnConnect()
    {
        _networkManager.networkAddress = GameObject.Find("IpToConnect/Text").GetComponent<Text>().text;
        if (_networkManager.networkAddress == "")
            _networkManager.networkAddress = "localhost";
        _networkManager.networkPort = 15678;
        _networkManager.StartClient();
    }
}
