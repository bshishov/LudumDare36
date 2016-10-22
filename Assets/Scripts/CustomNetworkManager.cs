using Assets.Scripts.UI;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(NetworkControllerPopups))]
public class CustomNetworkManager : NetworkManager
{
    public enum NetworkingModes
    {
        None, Host, Server, Client
    }

    public NetworkingModes NetworkingMode = NetworkingModes.None;

    public NetworkControllerPopups PopupsController;
    private GameObject _errorPopup;
    private GameObject _processingPopup;

    public void Start()
    {
        _errorPopup = GetComponent<NetworkControllerPopups>().ErrorPopup;
        _processingPopup = GetComponent<NetworkControllerPopups>().ProcessingPopup;
    }

    public static int DefaultPort = 15678;

    public override void OnClientError(NetworkConnection conn, int errorCode)
    {
        base.OnClientError(conn, errorCode);
        Debug.Log("CLIENT ERROR");
        StopClient();
        NetworkingMode = NetworkingModes.None;

        _errorPopup.SetActive(true);
        _processingPopup.SetActive(false);
    }

    public override void OnClientDisconnect(NetworkConnection conn)
    {
        Debug.Log("CLIENT DISCONNECTED");
        StopClient();
        NetworkingMode = NetworkingModes.None;
        
        _errorPopup.SetActive(true);
        _processingPopup.SetActive(false);
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
