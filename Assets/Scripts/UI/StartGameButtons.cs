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
    
    private GameObject _processingPopup;
    private CustomNetworkManager _networkManager;

    public void Start()
    {
        _networkManager = GameObject.Find("NetworkManager").GetComponent<CustomNetworkManager>();
        _networkManager.networkPort = CustomNetworkManager.DefaultPort;
        _networkManager.offlineScene = "MainMenu";
        GameObject.Find("IpToConnect").GetComponent<InputField>().text = PlayerPrefs.GetString("connection_address", _networkManager.networkAddress);
    }

    public void OnConnect()
    {
        _networkManager.networkAddress = GameObject.Find("IpToConnect").GetComponent<InputField>().text;
        if (_networkManager.networkAddress == "")
            _networkManager.networkAddress = "localhost";
        SetPortToNetworkManager(GameObject.Find("ClientPort/Text"));
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
        SetPortToNetworkManager(GameObject.Find("ClientPort/Text"));
        PlayerPrefs.SetString("connection_address", _networkManager.networkAddress);
        PlayerPrefs.SetInt("connection_port", _networkManager.networkPort);
    }

    public void SetPortToNetworkManager(GameObject target)
    {
        var portText = target.GetComponent<Text>().text;
        if (portText == "")
            _networkManager.networkPort = CustomNetworkManager.DefaultPort;
        else
            _networkManager.networkPort = Convert.ToInt32(portText);
    }
}
