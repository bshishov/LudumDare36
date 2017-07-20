using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Assets.Scripts.Utility;
using UnityEngine;

namespace Assets.Scripts.UI
{
    class MenuUIManager : Singleton<MenuUIManager>
    {
        public GameObject ProcessingPopup;
        public GameObject ErrorPopup;

        private List<GameObject> _panels = new List<GameObject>();
        private GameObject _activePanel;

        void Start()
        {
            foreach (Transform child in transform)
            {
                var canvasGroup = child.GetComponent<CanvasGroup>();
                if (canvasGroup != null)
                {
                    _panels.Add(canvasGroup.gameObject);
                }
            }  
        }

        public void ExitGame()
        {
            Application.Quit();
        }

        public void ShowPanel(GameObject panel)
        {
            if (panel != null)
            {
                if (_panels.Contains(panel))
                {
                    if(_activePanel != null)
                        HideCanvasGroup(_activePanel);
                    ShowCanvasGroup(panel);

                    _activePanel = panel;
                }
            }
        }

        public void HideActivePanel()
        {
            if(_activePanel != null)
                HideCanvasGroup(_activePanel);
            _activePanel = null;
        }


        public void ShowCanvasGroup(GameObject obj)
        {
            if (!obj.activeSelf)
                obj.SetActive(true);
            var canvasGroup = obj.GetComponent<CanvasGroup>();
            if (canvasGroup != null)
            {
                canvasGroup.alpha = 1f;
                canvasGroup.interactable = true;
            }
        }

        public void HideCanvasGroup(GameObject obj)
        {
            obj.SetActive(false);
            var canvasGroup = obj.GetComponent<CanvasGroup>();
            if (canvasGroup != null)
            {
                canvasGroup.alpha = 0f;
                canvasGroup.interactable = false;
            }
        }
    }
}
