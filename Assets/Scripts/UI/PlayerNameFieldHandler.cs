using UnityEngine;
using System.Collections;
using UnityEngine.UI;

[RequireComponent(typeof(InputField))]
public class PlayerNameFieldHandler : MonoBehaviour {

    public PlayerIdentity PlayerIdentity;

	// Use this for initialization
	void Start () {
        gameObject.GetComponent<InputField>().text = PlayerIdentity.GetName();
	}
}
