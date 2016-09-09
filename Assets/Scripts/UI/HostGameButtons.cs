using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.Networking;
using UnityEngine.UI;
using Assets.Scripts;
using Assets.Scripts.UI;

public class HostGameButtons : MonoBehaviour {
    
    public GameObject LevelPacksDropDown;
    public GameObject LevelsGrid;
    public ToggleGroup LevelsGridToggle;
    public GameObject LevelsItemTemplate;
    public LevelPack[] LevelPacks;

    private List<LevelPack.LevelData> _levels = new List<LevelPack.LevelData>();
    private CustomNetworkManager _networkManager;

    private static int _defaultPort = 15678;
    private static int _defaultMaxPlayers = 20;
    private static int _maxPlayers = _defaultMaxPlayers;

    public void Start()
    {
        _networkManager = GameObject.Find("NetworkManager").GetComponent<CustomNetworkManager>();
        _networkManager.networkPort = _defaultPort;
        _networkManager.offlineScene = "MainMenu";

        var dropDown = LevelPacksDropDown.GetComponent<Dropdown>();
        var options = new List<Dropdown.OptionData>();

        //foreach (var levelPack in LevelPacks)
        //{
        //    foreach (var level in levelPack.Levels)
        //    {
        //        _levels.Add(level);
        //        options.Add(new Dropdown.OptionData(string.Format("[{0}] {1}", levelPack.Name, level.Name), level.Thumbnail));
        //    }
        //}

        foreach (var levelPack in LevelPacks)
        {
            options.Add(new Dropdown.OptionData(string.Format("{0}", levelPack.Name)));
        }
        OnLevelPackChanged(0);

        dropDown.AddOptions(options);
    }

    public void OnHost()
    {
        _networkManager.networkAddress = "localhost";
        GetPort(GameObject.Find("ServerPort/Text"));

        var selectedName = LevelsGridToggle.ActiveToggles().First().gameObject.name;
        if (string.IsNullOrEmpty(selectedName))
            Debug.LogErrorFormat("No such level for dropdown value = {0}", selectedName);

        _networkManager.onlineScene = selectedName;
        _networkManager.NetworkingMode = CustomNetworkManager.NetworkingModes.Host;
        _networkManager.StartHost();

        PlayerPrefs.SetInt("connection_port", _networkManager.networkPort);
    }

    public void OnChangeMaxPlayers(string newValue)
    {
        int.TryParse(newValue, out _maxPlayers);
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
            var newItem = Instantiate(LevelsItemTemplate) as GameObject;
            newItem.transform.FindChild("Image").GetComponentInChildren<Image>().sprite = level.Thumbnail;
            newItem.transform.FindChild("Text").GetComponentInChildren<Text>().text = level.Name;
            newItem.name = level.SceneName;
            newItem.transform.SetParent(LevelsGrid.transform);
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

    public void GetPort(GameObject target)
    {
        var portText = target.GetComponent<Text>().text;
        if (portText == "")
            _networkManager.networkPort = _defaultPort;
        else
            _networkManager.networkPort = Convert.ToInt32(portText);
    }
}
