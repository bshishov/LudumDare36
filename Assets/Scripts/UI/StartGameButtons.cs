using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.Networking;
using UnityEngine.UI;
using Assets.Scripts;
using Assets.Scripts.UI;

public class StartGameButtons : MonoBehaviour {

    public GameObject ConnectionProcessingPopup;
    public GameObject LevelsDropDown;

    public CustomNetworkManager NetworkManager;
    public Text IpTextBox;
    public Text PortTextBox;
    public Text PasswordTextBox;

    private GameObject _processingPopup;
    private CustomNetworkManager _networkManager;

    public void Start()
    {
        _networkManager = GameObject.Find("NetworkManager").GetComponent<CustomNetworkManager>();
        _networkManager.networkPort = CustomNetworkManager.DefaultPort;
        _networkManager.offlineScene = "MainMenu";
        IpTextBox.text = PlayerPrefs.GetString("connection_address", _networkManager.networkAddress);
    }

    public void OnConnect()
    {
        _networkManager.networkAddress = IpTextBox.text;
        if (_networkManager.networkAddress == "")
            _networkManager.networkAddress = "localhost";
        SetPortToNetworkManager(PortTextBox);
        _networkManager.StartClient();
        _networkManager.NetworkingMode = CustomNetworkManager.NetworkingModes.Client;

        PlayerPrefs.SetString("connection_address", _networkManager.networkAddress);
        PlayerPrefs.SetInt("connection_port", _networkManager.networkPort);

        _processingPopup = NetworkManager.GetComponent<NetworkControllerPopups>().ProcessingPopup;
        _processingPopup.SetActive(true);
    }

    public void OnBack()
    {
        _networkManager.networkAddress = IpTextBox.text;
        if (_networkManager.networkAddress == "")
            _networkManager.networkAddress = "localhost";
        SetPortToNetworkManager(PortTextBox);
        PlayerPrefs.SetString("connection_address", _networkManager.networkAddress);
        PlayerPrefs.SetInt("connection_port", _networkManager.networkPort);
    }

    public void SetPortToNetworkManager(Text target)
    {
        var portText = target.text;
        if (portText == "")
            _networkManager.networkPort = CustomNetworkManager.DefaultPort;
        else
            _networkManager.networkPort = Convert.ToInt32(portText);
    }
}
