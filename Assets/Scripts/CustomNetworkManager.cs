using Assets.Scripts.UI;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;

public class CustomNetworkManager : NetworkManager
{
    public enum NetworkingModes
    {
        None, Host, Server, Client
    }

    public NetworkingModes NetworkingMode = NetworkingModes.None;

    private GameObject _errorPopup;
    private GameObject _processingPopup;

    public static int DefaultPort = 15678;

    public override void OnClientError(NetworkConnection conn, int errorCode)
    {
        base.OnClientError(conn, errorCode);
        Debug.Log("CLIENT ERROR");
        StopClient();
        NetworkingMode = NetworkingModes.None;

        _errorPopup = GameObject.Find("Menu").GetComponent<NetworkControllerPopups>().ErrorPopup;
        _processingPopup = GameObject.Find("Menu").GetComponent<NetworkControllerPopups>().ProcessingPopup;
        _errorPopup.SetActive(true);
        _processingPopup.SetActive(false);
    }

    public override void OnClientDisconnect(NetworkConnection conn)
    {
        Debug.Log("CLIENT DISCONNECTED");
        StopClient();
        NetworkingMode = NetworkingModes.None;

        _errorPopup = GameObject.Find("Menu").GetComponent<NetworkControllerPopups>().ErrorPopup;
        _processingPopup = GameObject.Find("Menu").GetComponent<NetworkControllerPopups>().ProcessingPopup;
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
