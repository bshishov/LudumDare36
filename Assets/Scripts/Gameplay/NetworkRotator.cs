using System;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

[NetworkSettings(sendInterval = 60f)]
public class NetworkRotator : NetworkBehaviour
{
    public Vector3 Speed;
    public Space RotationSpace = Space.World;

    // MAKE SURE THAT SEND RATE IS HIGH ENOUGH
    [SyncVar] private Quaternion _syncRotation;

    void Start ()
    {
	    if(isClient)
        {
            gameObject.transform.localRotation = _syncRotation;
        }
	}
	
	void Update ()
    {
        gameObject.transform.Rotate(Speed * Time.deltaTime, RotationSpace);

	    if (isServer && hasAuthority)
            _syncRotation = gameObject.transform.localRotation;
    }
}
