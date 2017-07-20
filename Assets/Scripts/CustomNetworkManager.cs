using Assets.Scripts.UI;
using UnityEngine;
using UnityEngine.Networking;

namespace Assets.Scripts
{
    public class CustomNetworkManager : NetworkManager
    {
        public enum NetworkingModes
        {
            None, Host, Server, Client
        }

        public NetworkingModes NetworkingMode = NetworkingModes.None;

        public void Start()
        {
        }

        public static int DefaultPort = 15678;

        public override void OnClientError(NetworkConnection conn, int errorCode)
        {
            base.OnClientError(conn, errorCode);
            Debug.Log("CLIENT ERROR");
            StopClient();
            NetworkingMode = NetworkingModes.None;


            MenuUIManager.Instance.ErrorPopup.SetActive(true);
            MenuUIManager.Instance.ProcessingPopup.SetActive(false);
        }

        public override void OnClientDisconnect(NetworkConnection conn)
        {
            Debug.Log("CLIENT DISCONNECTED");
            StopClient();
            NetworkingMode = NetworkingModes.None;

            MenuUIManager.Instance.ErrorPopup.SetActive(true);
            MenuUIManager.Instance.ProcessingPopup.SetActive(false);
        }

        /*
    public override void OnClientConnect(NetworkConnection conn)
    {
        //ClientScene.AddPlayer(conn, 0);
        //ClientScene.AddPlayer(0);
    }*/

        /*
    public override void OnServerAddPlayer(NetworkConnection conn, short playerControllerId)
    {
        Debug.Log("OnServerAddPlayer");
        var player = (GameObject)Instantiate(playerPrefab, Vector3.zero, Quaternion.identity);
        NetworkServer.AddPlayerForConnection(conn, player, playerControllerId);
    }*/
    }
}
