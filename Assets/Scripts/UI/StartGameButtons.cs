using UnityEngine;
using System;
using System.Collections;
using UnityEngine.Networking;
using UnityEngine.UI;
using Assets.Scripts;
using Assets.Scripts.UI;

public class StartGameButtons : MonoBehaviour {

    public GameObject ConnectionProcessingPopup;
    public GameObject LevelsDropDown;

    private GameObject _processingPopup;

    CustomNetworkManager _networkManager;

    private int _defaultPort = 15678;
    private string[] levelNames = { "level1", "PlayerMovementTest", "buttons_test",  };

    public void Start()
    {
        _networkManager = GameObject.Find("NetworkManager").GetComponent<CustomNetworkManager>();
        _networkManager.networkPort = _defaultPort;
        _networkManager.offlineScene = "MainMenu";
        GameObject.Find("IpToConnect").GetComponent<InputField>().text = PlayerPrefs.GetString("connection_address", _networkManager.networkAddress);

        GameObject.Find(PlayerPrefs.GetString("player_hat")).GetComponent<Toggle>().isOn = true;
    }

    public void OnHost()
    {
        var levelIndex = LevelsDropDown.GetComponent<Dropdown>().value;

        _networkManager.networkAddress = "localhost";
        GetPort(GameObject.Find("ServerPort/Text"));
        _networkManager.onlineScene = levelNames[levelIndex];
        _networkManager.NetworkingMode = CustomNetworkManager.NetworkingModes.Host;
        _networkManager.StartHost();

        PlayerPrefs.SetInt("connection_port", _networkManager.networkPort);
    }

    public void OnConnect()
    {
        _networkManager.networkAddress = GameObject.Find("IpToConnect").GetComponent<InputField>().text;
        if (_networkManager.networkAddress == "")
            _networkManager.networkAddress = "localhost";
        GetPort(GameObject.Find("ClientPort/Text"));
        _networkManager.StartClient();
        _networkManager.NetworkingMode = CustomNetworkManager.NetworkingModes.Client;

        PlayerPrefs.SetString("connection_address", _networkManager.networkAddress);
        PlayerPrefs.SetInt("connection_port", _networkManager.networkPort);

        _processingPopup = GameObject.Find("Menu").GetComponent<NetworkControllerPopups>().ProcessingPopup;
        _processingPopup.SetActive(true);
    }

    public void OnBack()
    {
        _networkManager.networkAddress = GameObject.Find("IpToConnect").GetComponent<InputField>().text;
        if (_networkManager.networkAddress == "")
            _networkManager.networkAddress = "localhost";
        GetPort(GameObject.Find("ClientPort/Text"));
        PlayerPrefs.SetString("connection_address", _networkManager.networkAddress);
        PlayerPrefs.SetInt("connection_port", _networkManager.networkPort);
    }

    public void GetPort(GameObject target)
    {
        var portText = target.GetComponent<Text>().text;
        if (portText == "")
            _networkManager.networkPort = _defaultPort;
        else
            _networkManager.networkPort = Convert.ToInt32(portText);
    }

    public void SetHat(string hatName)
    {
        PlayerPrefs.SetString("player_hat", hatName);
    }
}
