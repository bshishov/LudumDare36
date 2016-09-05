using System;
using UnityEngine;
using System.Collections;
using UnityEngine.Networking;

public class PlayerIdentity : NetworkBehaviour
{
    public string Name { get { return _name; } }
    public string HatName { get { return _hatName; } }
    public Color Color { get { return _color; } }

    public HatsData Hats;
    public Vector3 Offset = Vector3.up;
    public Vector3 Rotation = Vector3.zero;
    public string HeadObjectName = "";

    private Transform _headTransform;

    // sync only server => client
    [SyncVar] private string _hatName;
    [SyncVar] private Color _color;
    [SyncVar] private string _name;

    private readonly string _defaultHat = String.Empty;
    private Color _defaultColor = Color.white;
    private readonly string _defaultName = "Unnamed";

    void Start ()
	{
	    if (string.IsNullOrEmpty(HeadObjectName))
            _headTransform = gameObject.transform;
	    else
            _headTransform = gameObject.transform.FindChild(HeadObjectName);

        ApplyIdentity();
    }

    public override void OnStartClient()
    {
    }
    
    public override void OnStartLocalPlayer()
    {
        // say to all that you have a hat over here
        _hatName = PlayerPrefs.GetString("player_hat", _defaultHat);
        _color = new Color(
            PlayerPrefs.GetFloat("player_color_r", _defaultColor.r), 
            PlayerPrefs.GetFloat("player_color_g", _defaultColor.g), 
            PlayerPrefs.GetFloat("player_color_b", _defaultColor.b));
        _name = PlayerPrefs.GetString("player_name", _defaultName);
        CmdSetIdentity(_name, _color, _hatName);
    }

    [Command]
    void CmdSetIdentity(string name, Color color, string hat)
    {
        // send to all that this guy has a hat
        RpcSetIdentity(name, color, hat);
    }

    [ClientRpc]
    void RpcSetIdentity(string name, Color color, string hat)
    {
        if (!isLocalPlayer)
        {
            _name = name;
            _color = color;
            _hatName = hat;
            ApplyIdentity();
        }
    }

    void ApplyIdentity()
    {
        Debug.LogFormat("Applying identity name={0} hat={1} color={2}", _name, _hatName, _color);

        var meshRenderer = GetComponentInChildren<SkinnedMeshRenderer>();
        if (meshRenderer != null)
        {
            meshRenderer.material.color = _color;
        }

        if (!string.IsNullOrEmpty(_hatName))
        {
            var hat = Hats.GetHat(_hatName);
            if (hat.Prefab == null)
            {
                Debug.LogWarningFormat("HAT {0} NOT FOUND", _hatName);
                return;
            }

            _headTransform = gameObject.transform.FindChild(HeadObjectName);
            var go = GameObject.Instantiate(hat.Prefab, Vector3.zero, Quaternion.identity) as GameObject;
            go.transform.Rotate(Rotation + hat.Prefab.transform.localEulerAngles);
            go.transform.SetParent(_headTransform);
            go.transform.localPosition = Offset + hat.Prefab.transform.position;
        }
    }
}
