using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Assets.Scripts.Utility;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Assets.Scripts
{
    class GameplayManager : Singleton<GameplayManager>
    {
        void Start()
        {
            var networkManager = GameObject.FindObjectOfType<CustomNetworkManager>();
            if (networkManager == null)
                CreateDummyGame();
        }

        void CreateDummyGame()
        {
            
        }

        public void RestartCurrentGame()
        {
            var networkManager = GameObject.FindObjectOfType<CustomNetworkManager>();
            if (networkManager != null)
            {
                networkManager.ServerChangeScene(SceneManager.GetActiveScene().name);
            }
        }

        public void ExitCurrentGame()
        {
            SceneManager.LoadScene("MainMenu");
            var networkManager = GameObject.FindObjectOfType<CustomNetworkManager>();
            if (networkManager != null)
            {
                switch (networkManager.NetworkingMode)
                {
                    case CustomNetworkManager.NetworkingModes.Host:
                        networkManager.StopHost();
                        break;
                    case CustomNetworkManager.NetworkingModes.Client:
                        networkManager.StopClient();
                        break;
                }
                networkManager.NetworkingMode = CustomNetworkManager.NetworkingModes.None;
            }
        }
    }
}
