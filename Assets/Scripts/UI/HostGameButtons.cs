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
#if UNITY_STANDALONE
using Open.Nat;
#endif

public class HostGameButtons : MonoBehaviour {
    
    public GameObject LevelPacksDropDown;
    public GameObject LevelsGrid;
    public ToggleGroup LevelsGridToggle;
    public GameObject LevelsItemTemplate;
    public LevelPack[] LevelPacks;
    
    public CustomNetworkManager NetworkManager;
    public Text PortTextBox;
    public Text MaxPlayersTextBox;

    private static uint _defaultMaxPlayers = 20;
    private static uint _maxPlayers = _defaultMaxPlayers;

    public void Start()
    {
        NetworkManager.networkPort = CustomNetworkManager.DefaultPort;
        NetworkManager.offlineScene = "MainMenu";

        var dropDown = LevelPacksDropDown.GetComponent<Dropdown>();
        var options = new List<Dropdown.OptionData>();

        foreach (var levelPack in LevelPacks)
        {
            options.Add(new Dropdown.OptionData(string.Format("{0}", levelPack.Name)));
        }
        OnLevelPackChanged(0);

        dropDown.AddOptions(options);

#if UNITY_STANDALONE
        try
        {
            var discoverer = new NatDiscoverer();
            var cts = new CancellationTokenSource();
            var device = discoverer.DiscoverDeviceAsync(PortMapper.Upnp, cts);
            device.Wait();
            device.Result.CreatePortMapAsync(new Mapping(Protocol.Tcp, 15678, 15678, "GDGP"));
        }
        catch
        {
            Debug.LogWarning("UPnP port opening failed");
        }
#endif
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
        NetworkManager.networkAddress = "localhost";
        SetPortToNetworkManager(PortTextBox);
        SetMatchSizeToNetworkManager(MaxPlayersTextBox);

        var activeToggle = LevelsGridToggle.ActiveToggles().First();
        if (activeToggle == null)
            return;
        var selectedName = activeToggle.gameObject.name;
        if (string.IsNullOrEmpty(selectedName))
        {
            Debug.LogErrorFormat("No such level for dropdown value = {0}", selectedName);
            return;
        }
        NetworkManager.matchSize = _maxPlayers;

        NetworkManager.onlineScene = selectedName;
        NetworkManager.NetworkingMode = CustomNetworkManager.NetworkingModes.Host;
        NetworkManager.StartHost();

        PlayerPrefs.SetInt("connection_port", NetworkManager.networkPort);
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
            var info = newItem.GetComponentInChildren<LevelItemTemplateInfo>();
            info.Image.sprite = level.Thumbnail;
            info.Text.text = level.Name;
            newItem.name = level.SceneName;
            newItem.transform.localScale = Vector3.one;

            var newItemToggle = newItem.GetComponent<Toggle>();
            LevelsGridToggle.RegisterToggle(newItemToggle);
        }

        var cellHeightLevelsGrid = LevelsGrid.GetComponent<GridLayoutGroup>().cellSize.y;
        var rowsNumber = Math.Floor(LevelPacks[index].Levels.Length / 5f) + 1;
        var rect = LevelsGrid.GetComponent<RectTransform>().rect;
        rect.size = new Vector2(rect.size.x, (float)rowsNumber * cellHeightLevelsGrid);
        // TODO: size is not changing ?
    }

    public void SetPortToNetworkManager(Text target)
    {
        var portText = target.text;
        if (portText == "")
            NetworkManager.networkPort = CustomNetworkManager.DefaultPort;
        else
            NetworkManager.networkPort = Convert.ToInt32(portText);
    }

    public void SetMatchSizeToNetworkManager(Text target)
    {
        var matchSize = target.text;
        if (matchSize == "")
            NetworkManager.matchSize = _defaultMaxPlayers;
        else
            NetworkManager.matchSize = Convert.ToUInt32(matchSize);
    }
}
