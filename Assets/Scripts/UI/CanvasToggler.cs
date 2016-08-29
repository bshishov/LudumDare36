using UnityEngine;
using System.Collections;

public class CanvasToggler : MonoBehaviour {

    public void ShowPanel(GameObject panelToShow)
    {
        panelToShow.SetActive(true);
    }

    public void HidePanel(GameObject panelToHide)
    {
        panelToHide.SetActive(false);
    }
}
