using UnityEngine;
using System.Collections;
using Assets.Scripts;

public class PauseMenuController : MonoBehaviour {
    
    public GameObject PauseCanvas;
    public GameObject ScoreCanvas;

    [HideInInspector]
    public bool GameOver = false;

    private bool _allowInteraction = true;
    private float _lastInteraction = 0f;
    private float _interactionTimeout = 0.2f;

    // Update is called once per frame
    void Update () {
        if (_allowInteraction && !GameOver)
        {
            if (Input.GetButton("Cancel"))
            {
                _allowInteraction = false;
                _lastInteraction = Time.time;
                PauseCanvas.SetActive(!PauseCanvas.activeSelf);
            }
        }

        if (Time.time - _lastInteraction >= _interactionTimeout)
            _allowInteraction = true;

        if (Input.GetKeyDown(KeyCode.Tab) || Input.GetKeyDown(KeyCode.Q))
        {
            ScoreCanvas.SetActive(true);
        }

        if (Input.GetKeyUp(KeyCode.Tab) || Input.GetKeyUp(KeyCode.Q))
        {
            ScoreCanvas.SetActive(false);
        }
    }
}
