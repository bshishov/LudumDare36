using UnityEngine;
using UnityEngine.Networking;

public class ActivatorProxy : NetworkBehaviour
{
    public static string ActivateEvent = "OnActivate";
    public static string DeActivateEvent = "OnDeactivate";
    public static string ProxyActivateEvent = "OnProxyActivate";
    public static string ProxyDeActivateEvent = "OnProxyDeactivate";

    public bool IsActivated { get; private set; }
    public float Delay = 0f;

    [SerializeField]
    private int _counter = 0;
    private float _deactivatingTime;
    private bool _deactivationPending;

    void Update()
    {
        if (isServer)
        {
            if (_deactivationPending && Time.time > _deactivatingTime)
            {
                RpcDeactivate();
                _deactivationPending = false;
                _counter = 0;
            }
        }
    }

    // Coming from SendMessage
    void OnActivate()
    {
        if (isServer)
        {
            if (_deactivationPending)
            {
                _deactivationPending = false;
                _counter = 0;
            }

            _counter++;
            if (_counter == 1)
            {
                RpcActivate();
            }
        }
    }

    // Coming from SendMessage
    void OnDeactivate()
    {
        if (isServer)
        {
            // if last activator
            if (_counter == 1)
            {
                _deactivationPending = true;
                _deactivatingTime = Time.time + Delay;
            }
            else
            {
                _counter--;
            }
        }
    }
    
    [ClientRpc]
    void RpcActivate()
    {
        IsActivated = true;
        gameObject.SendMessage(ProxyActivateEvent, SendMessageOptions.DontRequireReceiver);
    }

    [ClientRpc]
    void RpcDeactivate()
    {
        IsActivated = false;
        gameObject.SendMessage(ProxyDeActivateEvent, SendMessageOptions.DontRequireReceiver);
    }
}
