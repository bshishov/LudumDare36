using System.Collections.Generic;
using Assets.Scripts.Utility;
using UnityEngine;

namespace Assets.Scripts.UI
{
    public class GameplayUIManager : Singleton<GameplayUIManager>
    {
        public enum UIPanel
        {
            None,
            Pause,
            GameOver,
            Scores
        }
        
        private UIPanel _currentPanel;
        private readonly Dictionary<UIPanel, GameObject> _panels = 
            new Dictionary<UIPanel, GameObject>();

        void Start()
        {
            _panels.Add(UIPanel.Pause, transform.Find("Canvas/PausePanel").gameObject);
            _panels.Add(UIPanel.GameOver, transform.Find("Canvas/GameOverPanel").gameObject);
            _panels.Add(UIPanel.Scores, transform.Find("Canvas/ScoresPanel").gameObject);
        }
        
        void Update ()
        {
            
            if (Input.GetButtonDown("Cancel"))
            {
                if (_currentPanel == UIPanel.None)
                {
                    ShowPanel(UIPanel.Pause);
                }
                else if(_currentPanel == UIPanel.Pause)
                {
                    HidePanel(UIPanel.Pause);
                }
            }

            if (Input.GetKeyDown(KeyCode.Tab) || Input.GetKeyDown(KeyCode.Q))
                ShowPanel(UIPanel.Scores);

            if (Input.GetKeyUp(KeyCode.Tab) || Input.GetKeyUp(KeyCode.Q))
                HidePanel(UIPanel.Scores);
        }

        public void Unpause()
        {
            if (_currentPanel == UIPanel.Pause)
            {
                HidePanel(UIPanel.Pause);
            }
        }

        public void ShowPanel(UIPanel panel)
        {
            if(_currentPanel == panel)
                return;

            if (_panels.ContainsKey(panel))
            {
                if(_currentPanel != UIPanel.None)
                    HidePanel(_currentPanel);

                ShowCanvasGroup(_panels[panel]);
                _currentPanel = panel;
            }
        }

        public void HidePanel(UIPanel panel)
        {
            if(_currentPanel != panel)
                return;

            if (_panels.ContainsKey(panel))
            {
                HideCanvasGroup(_panels[panel]);
                _currentPanel = UIPanel.None;
            }
        }

        public void ShowCanvasGroup(GameObject obj)
        {
            if(!obj.activeSelf)
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
            var canvasGroup = obj.GetComponent<CanvasGroup>();
            if (canvasGroup != null)
            {
                canvasGroup.alpha = 0f;
                canvasGroup.interactable = false;
            }
        }
    }
}
