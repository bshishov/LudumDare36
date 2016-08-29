using UnityEngine;
using System.Collections;

public class HatsProvider : MonoBehaviour
{
    public HatsData Hats;

    public string DefaultHat;
    public Vector3 Offset = Vector3.up;
    public Vector3 Rotation = Vector3.zero;
    public string HeadObjectName = "";

    private Transform _headTransform;

	void Start ()
	{
	    if (string.IsNullOrEmpty(HeadObjectName))
            _headTransform = gameObject.transform;
	    else
            _headTransform = gameObject.transform.FindChild(HeadObjectName);
        
        SetHat(DefaultHat);
	}

    public void SetHat(string hat)
    {
        if (string.IsNullOrEmpty(DefaultHat))
            return;

        var prefab = Hats.GetPrefab(hat);
        if (prefab == null)
        {
            Debug.LogWarningFormat("HAT {0} NOT FOUND", hat);
            return;
        }

        var go = GameObject.Instantiate(prefab, Offset, Quaternion.identity) as GameObject;
        go.transform.Rotate(Rotation);
        go.transform.SetParent(_headTransform);
    }
}
