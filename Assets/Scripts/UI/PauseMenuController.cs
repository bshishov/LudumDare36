using UnityEngine;
using System.Collections;
using Assets.Scripts;

public class PauseMenuController : MonoBehaviour {
    
    public GameObject PauseCanvas;

    private bool _allowInteraction = true;
    private float _lastInteraction = 0f;
    private float _interactionTimeout = 0.2f;

    // Update is called once per frame
    void Update () {
        if (_allowInteraction)
        {
            if (Input.GetButton("Cancel"))
            {
                _allowInteraction = false;
                _lastInteraction = Time.time;
                PauseCanvas.SetActive(!PauseCanvas.active);
            }
        }

        if (Time.time - _lastInteraction >= _interactionTimeout)
            _allowInteraction = true;
    }
}
