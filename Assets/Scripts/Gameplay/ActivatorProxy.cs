using UnityEngine;

public class ActivatorProxy : MonoBehaviour
{
    public static string ActivateEvent = "OnActivate";
    public static string DeActivateEvent = "OnDeactivate";
    public static string ProxyActivateEvent = "OnProxyActivate";
    public static string ProxyDeActivateEvent = "OnProxyDeactivate";

    public bool IsActivated { get { return _counter >= 1; } }
    public float Delay = 0f;

    private int _counter = 0;
    private float _deactivatingTime;
    private bool _deactivationPending;

    void Update()
    {
        if (_deactivationPending && Time.time > _deactivatingTime)
        {
            gameObject.SendMessage(ProxyDeActivateEvent, SendMessageOptions.DontRequireReceiver);
            _deactivationPending = false;
            _counter = 0;
        }
    }

    void OnActivate()
    {
        if (_deactivationPending)
        {
            _deactivationPending = false;
            _counter = 0;
        }

        _counter++;
        if (_counter == 1)
        {
            gameObject.SendMessage(ProxyActivateEvent, SendMessageOptions.DontRequireReceiver);
        }
    }

    void OnDeactivate()
    {
        // if last activator
        if (_counter == 1)
        {
            _deactivationPending = true;
            _deactivatingTime = Time.time + Delay;
        }
    }
}
