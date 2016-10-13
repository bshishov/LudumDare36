using UnityEngine;
using System;
using System.Threading;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.Networking;
using UnityEngine.UI;
using Assets.Scripts;
using Assets.Scripts.UI;
using System.Net.Sockets;
using System.Net;
using Open.Nat;

public class HostGameButtons : MonoBehaviour {
    
    public GameObject LevelPacksDropDown;
    public GameObject LevelsGrid;
    public ToggleGroup LevelsGridToggle;
    public GameObject LevelsItemTemplate;
    public LevelPack[] LevelPacks;
    
    private CustomNetworkManager _networkManager;

    private static uint _defaultMaxPlayers = 20;
    private static uint _maxPlayers = _defaultMaxPlayers;

    public void Start()
    {
        _networkManager = GameObject.Find("NetworkManager").GetComponent<CustomNetworkManager>();
        _networkManager.networkPort = CustomNetworkManager.DefaultPort;
        _networkManager.offlineScene = "MainMenu";

        var dropDown = LevelPacksDropDown.GetComponent<Dropdown>();
        var options = new List<Dropdown.OptionData>();

        foreach (var levelPack in LevelPacks)
        {
            options.Add(new Dropdown.OptionData(string.Format("{0}", levelPack.Name)));
        }
        OnLevelPackChanged(0);

        dropDown.AddOptions(options);

        var discoverer = new NatDiscoverer();
        var cts = new CancellationTokenSource();
        var device = discoverer.DiscoverDeviceAsync(PortMapper.Upnp, cts);
        device.Wait();
        device.Result.CreatePortMapAsync(new Mapping(Protocol.Tcp, 15678, 15678, "GDGP"));
    }

    public string GetLocalIPAddress()
    {
        var host = Dns.GetHostEntry(Dns.GetHostName());
        foreach (var ip in host.AddressList)
        {
            if (ip.AddressFamily == AddressFamily.InterNetwork)
            {
                return ip.ToString();
            }
        }
        throw new Exception("Local IP Address Not Found!");
    }

    public void OnHost()
    {
        _networkManager.networkAddress = "localhost";
        SetPortToNetworkManager(GameObject.Find("ServerPort/Text"));
        SetMatchSizeToNetworkManager(GameObject.Find("MaximumPlayers/Text"));

        var activeToggle = LevelsGridToggle.ActiveToggles().First();
        if (activeToggle == null)
            return;
        var selectedName = activeToggle.gameObject.name;
        if (string.IsNullOrEmpty(selectedName))
        {
            Debug.LogErrorFormat("No such level for dropdown value = {0}", selectedName);
            return;
        }
        _networkManager.matchSize = _maxPlayers;

        _networkManager.onlineScene = selectedName;
        _networkManager.NetworkingMode = CustomNetworkManager.NetworkingModes.Host;
        _networkManager.StartHost();

        PlayerPrefs.SetInt("connection_port", _networkManager.networkPort);
    }

    public void OnChangeMaxPlayers(string newValue)
    {
        uint.TryParse(newValue, out _maxPlayers);
        _maxPlayers = Math.Min(_maxPlayers, _defaultMaxPlayers);
        _maxPlayers = Math.Max(_maxPlayers, 1);
    }

    public void OnLevelPackChanged(int index)
    {
        var children = new List<GameObject>();
        foreach (Transform child in LevelsGrid.transform) children.Add(child.gameObject);
        children.ForEach(child => Destroy(child));

        foreach (var level in LevelPacks[index].Levels)
        {
            var newItem = Instantiate(LevelsItemTemplate, LevelsGrid.transform) as GameObject;
            newItem.transform.FindChild("Image").GetComponentInChildren<Image>().sprite = level.Thumbnail;
            newItem.transform.FindChild("Text").GetComponentInChildren<Text>().text = level.Name;
            newItem.name = level.SceneName;
            newItem.transform.localScale = Vector3.one;

            var newItemToggle = newItem.GetComponent<Toggle>();
            LevelsGridToggle.RegisterToggle(newItemToggle);
        }

        var cellHeightLevelsGrid = LevelsGrid.GetComponent<GridLayoutGroup>().cellSize.y;
        var rowsNumber = Math.Floor(LevelPacks[index].Levels.Length / 5f) + 1;
        var rect = LevelsGrid.GetComponent<RectTransform>().rect;
        rect.size = new Vector2(rect.size.x, (float)rowsNumber * cellHeightLevelsGrid);
        // TODO: size is not changing
    }

    public void SetPortToNetworkManager(GameObject target)
    {
        var portText = target.GetComponent<Text>().text;
        if (portText == "")
            _networkManager.networkPort = CustomNetworkManager.DefaultPort;
        else
            _networkManager.networkPort = Convert.ToInt32(portText);
    }

    public void SetMatchSizeToNetworkManager(GameObject target)
    {
        var matchSize = target.GetComponent<Text>().text;
        if (matchSize == "")
            _networkManager.matchSize = _defaultMaxPlayers;
        else
            _networkManager.matchSize = Convert.ToUInt32(matchSize);
    }
}
