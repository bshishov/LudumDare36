using UnityEngine;
using UnityEngine.Networking;

namespace Assets.Scripts
{
    public class CustomNetworkManager : NetworkManager
    {
        public GameObject ProcessingPopup;
        public GameObject ErrorPopup;

        public override void OnClientError(NetworkConnection conn, int errorCode)
        {
            base.OnClientError(conn, errorCode);
            Debug.Log("CLIENT ERROR");
            StopClient();
            ErrorPopup.SetActive(true);
            ProcessingPopup.SetActive(false);
        }

        public override void OnClientDisconnect(NetworkConnection conn)
        {
            Debug.Log("CLIENT DISCONNECTED");
            StopClient();
            ErrorPopup.SetActive(true);
            ProcessingPopup.SetActive(false);
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
