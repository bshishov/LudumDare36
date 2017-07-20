using System;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.UI
{
    public class ConnectGameButtons : MonoBehaviour
    {

        public GameObject LevelsDropDown;

        public Text IpTextBox;
        public Text PortTextBox;
        public Text PasswordTextBox;
        
        private CustomNetworkManager _networkManager;

        public void Start()
        {
            _networkManager = FindObjectOfType<CustomNetworkManager>();
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
            _networkManager.NetworkingMode = CustomNetworkManager.NetworkingModes.Client;
            _networkManager.StartClient();

            PlayerPrefs.SetString("connection_address", _networkManager.networkAddress);
            PlayerPrefs.SetInt("connection_port", _networkManager.networkPort);

            MenuUIManager.Instance.ProcessingPopup.SetActive(true);
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
}
