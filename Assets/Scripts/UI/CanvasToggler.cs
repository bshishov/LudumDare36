using UnityEngine;
using System.Collections;

public class CanvasToggler : MonoBehaviour {

    private Animator _animator;

    public void Start()
    {
        _animator = gameObject.GetComponent<Animator>();
    }

    public void ShowPanel(GameObject panelToShow)
    {
        panelToShow.SetActive(true);
    }

    public void HidePanel(GameObject panelToHide)
    {
        panelToHide.SetActive(false);
    }
}
