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
            Debug.Log("CLIENT STOPPED");
            StopClient();
            ErrorPopup.SetActive(true);
            ProcessingPopup.SetActive(false);
        }
    }
}
