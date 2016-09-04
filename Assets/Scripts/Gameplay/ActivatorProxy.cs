using UnityEngine;
using UnityEngine.Networking;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class ActivatorProxy : NetworkBehaviour
{
    public static string ActivateEvent = "OnActivate";
    public static string DeActivateEvent = "OnDeactivate";
    public static string ProxyActivateEvent = "OnProxyActivate";
    public static string ProxyDeActivateEvent = "OnProxyDeactivate";

    public bool IsActivated { get; private set; }

    public bool HoldActivation = true;
    public bool Inverted = false;
    public float TimeBeforeActivationMessage;
    public float TimeBeforeDeactivationMessage;

#if UNITY_EDITOR
    [SerializeField][ShowOnly]
#endif
    private int _counter = 0;
    private float _deactivatingTime;
    private bool _deactivationPending;
    private bool _activationPending;
    private float _activationTime;

    void Start()
    {
        if (Inverted)
        {
            RpcDeactivate();
        }
    }

    void Update()
    {
        if (isServer)
        {
            if (_deactivationPending && Time.time > _deactivatingTime)
            {
                RpcDeactivate();
                _deactivationPending = false;
            }

            if (_activationPending && Time.time > _activationTime)
            {
                RpcActivate();
                _activationPending = false;
            }
        }
    }

    // Coming from SendMessage
    void OnActivate()
    {
        if (isServer)
        {
            if (HoldActivation)
            {
                _counter++;

                if (_deactivationPending)
                    _deactivationPending = false;

                if (_counter == 1)
                {
                    if (!_activationPending)
                    {
                        _activationPending = true;
                        _activationTime = Time.time + TimeBeforeActivationMessage;
                    }
                }
            }
            else if (!_activationPending && !_deactivationPending)
            {
                _activationPending = true;
                _activationTime = Time.time + TimeBeforeActivationMessage;
                _deactivationPending = true;
                _deactivatingTime = Mathf.Max(_activationTime, Time.time) + TimeBeforeDeactivationMessage;
            }
        }
    }

    // Coming from SendMessage
    void OnDeactivate()
    {
        if (isServer && HoldActivation)
        {
            _counter--;
            if (_counter < 0)
                _counter = 0;

            if (_counter == 0)
            {
                _deactivationPending = true;
                _deactivatingTime = Mathf.Max(_activationTime, Time.time) + TimeBeforeDeactivationMessage;
            }
        }
    }

    [ClientRpc]
    void RpcActivate()
    {
        if (!Inverted)
        {
            IsActivated = true;
            gameObject.SendMessage(ProxyActivateEvent, SendMessageOptions.DontRequireReceiver);
        }
        else
        {
            IsActivated = false;
            gameObject.SendMessage(ProxyDeActivateEvent, SendMessageOptions.DontRequireReceiver);
        }
    }

    [ClientRpc]
    void RpcDeactivate()
    {
        if (!Inverted)
        {
            IsActivated = false;
            gameObject.SendMessage(ProxyDeActivateEvent, SendMessageOptions.DontRequireReceiver);
        }
        else
        {
            IsActivated = true;
            gameObject.SendMessage(ProxyActivateEvent, SendMessageOptions.DontRequireReceiver);
        }
    }
}
