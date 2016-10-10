using System;
using UnityEngine;
using System.Collections;
using UnityEngine.Networking;

[RequireComponent(typeof(PlayerIdentity))]
public class NetworkPlayerIdentity : NetworkBehaviour
{
    private PlayerIdentity _identity;
    [HideInInspector]
    public PlayerIdentity Identity {  get { return _identity; } }

    [SyncVar] private string _hatNameSync;
    [SyncVar] private Color _colorSync;
    [SyncVar] private string _nameSync;

    public override void OnStartLocalPlayer()
    {
        if (_identity == null)
            _identity = GetComponent<PlayerIdentity>();

        // load identity from player prefs
        _identity.SetFromPlayerPrefs();

        // local authority must set its syncvars
        if (!isServer && hasAuthority)
        {
            _colorSync = _identity.Color;
            _hatNameSync = _identity.HatName;
            _nameSync = _identity.Name;
        }

        // Tell about yourself to everyone
        CmdSetIdentity(_identity.Name, _identity.Color, _identity.HatName);
    }

    void Start()
    {
        if (_identity == null)
            _identity = GetComponent<PlayerIdentity>();

        // if it is not a lovalplayer than set identity from SyncVars
        if (!isLocalPlayer)
        {
            _identity.SetColor(_colorSync);
            _identity.SetName(_nameSync);
            _identity.SetHat(_hatNameSync);
        }

        PlayersList.Players.Add(this);
    }

    
    [Command]
    void CmdSetIdentity(string playerName, Color color, string hatName)
    {
        if (isServer && hasAuthority)
        {
            _colorSync = _identity.Color;
            _hatNameSync = _identity.HatName;
            _nameSync = _identity.Name;
        }

        RpcSetIdenitity(playerName, color, hatName);
    }

    [ClientRpc]
    void RpcSetIdenitity(string playerName, Color color, string hatName)
    {
        if (!isLocalPlayer)
        {
            _identity.SetName(playerName);
            _identity.SetHat(hatName);
            _identity.SetColor(color);
        }
    }

    public void OnDestroy()
    {
        if (PlayersList.Players.Contains(this))
            PlayersList.Players.Remove(this);
    }
}
