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
    public LevelPack[] LevelPacks;

    private List<LevelPack.LevelData> _levels = new List<LevelPack.LevelData>();
    private GameObject _processingPopup;
    private CustomNetworkManager _networkManager;

    private static int _defaultPort = 15678;

    public void Start()
    {
        _networkManager = GameObject.Find("NetworkManager").GetComponent<CustomNetworkManager>();
        _networkManager.networkPort = _defaultPort;
        _networkManager.offlineScene = "MainMenu";
        GameObject.Find("IpToConnect").GetComponent<InputField>().text = PlayerPrefs.GetString("connection_address", _networkManager.networkAddress);

        GameObject.Find(PlayerPrefs.GetString("player_hat")).GetComponent<Toggle>().isOn = true;

        var dropDown = LevelsDropDown.GetComponent<Dropdown>();
        var options = new List<Dropdown.OptionData>();

        foreach (var levelPack in LevelPacks)
        {
            foreach (var level in levelPack.Levels)
            {
                _levels.Add(level);
                options.Add(new Dropdown.OptionData(string.Format("[{0}] {1}", levelPack.Name, level.Name), level.Thumbnail));
            }
        }

        dropDown.AddOptions(options);
    }

    public void OnHost()
    {
        var dropdownValue = LevelsDropDown.GetComponent<Dropdown>().value;

        _networkManager.networkAddress = "localhost";
        GetPort(GameObject.Find("ServerPort/Text"));

        var sceneName = _levels[dropdownValue].SceneName;

        if (string.IsNullOrEmpty(sceneName))
            Debug.LogErrorFormat("No such level for dropdown value = {0}", dropdownValue);

        _networkManager.onlineScene = sceneName;
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
