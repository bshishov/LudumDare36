using UnityEngine;
using System.Collections;
using UnityEngine.Networking;

public class HatsProvider : NetworkBehaviour
{
    public HatsData Hats;

    public string DefaultHat;
    public Vector3 Offset = Vector3.up;
    public Vector3 Rotation = Vector3.zero;
    public string HeadObjectName = "";
    
    private Transform _headTransform;

    // sync only server => client
    [SyncVar]
    private string _hatName;

	void Start ()
	{
	    if (string.IsNullOrEmpty(HeadObjectName))
            _headTransform = gameObject.transform;
	    else
            _headTransform = gameObject.transform.FindChild(HeadObjectName);
        
        // SetHat(DefaultHat);
    }

    public override void OnStartClient()
    {
        // set hats for already existing players
        if(!string.IsNullOrEmpty(_hatName))
            SetHat(_hatName);
    }
    
    public override void OnStartLocalPlayer()
    {
        // say to all that you have a hat over here
        _hatName = PlayerPrefs.GetString("player_hat");
        CmdSetHat(_hatName);
    }

    [Command]
    void CmdSetHat(string hat)
    {
        // send to all that this guy has a hat
        RpcSetHat(hat);
    }

    [ClientRpc]
    void RpcSetHat(string hat)
    {
        SetHat(hat);
    }

    void SetHat(string hat)
    {
        Debug.LogFormat("Placing hat={0}", hat);

        if (string.IsNullOrEmpty(hat))
            return;

        var prefab = Hats.GetPrefab(hat);
        if (prefab == null)
        {
            Debug.LogWarningFormat("HAT {0} NOT FOUND", hat);
            return;
        }

        _headTransform = gameObject.transform.FindChild(HeadObjectName);
        var go = GameObject.Instantiate(prefab, Offset, Quaternion.identity) as GameObject;
        go.transform.Rotate(Rotation);
        go.transform.SetParent(_headTransform);
        go.transform.localPosition = Vector3.zero;
    }
}
