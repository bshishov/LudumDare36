using System.Linq;
using Assets.Scripts.Data;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.Gameplay.Player
{
    [RequireComponent(typeof(PlayerIdentity))]
    public class PlayerUI : MonoBehaviour
    {
        public GameObject PlayerUIPrefab;
        public GameObject EmotionUI;
        public bool IsUIActive;
        public Vector3 Offset = Vector3.zero;
        public EmotionData EmotionsPack;

        private GameObject _uiObject;
        private Text _playerNameText;
        private PlayerIdentity _identity;
        	
        void Start ()
        {
            _identity = GetComponent<PlayerIdentity>();
            _identity.NameChanged += name =>
            {
                if (_playerNameText != null)
                    _playerNameText.text = name;
            };

            var container = GameObject.Find("GameCanvas");
            if (container != null)
            {
                if (PlayerUIPrefab != null)
                {
                    _uiObject = GameObject.Instantiate(PlayerUIPrefab, container.transform) as GameObject;
                    _playerNameText = _uiObject.GetComponentInChildren<Text>();
                    _playerNameText.text = _identity.Name;
                }
                else
                {
                    Debug.LogWarning("Player's UI prefab is not set");
                }
            }
            else
            {
                Debug.LogWarning("Can't find main UI container");
            }
        }	
	
        void Update ()
        {
            if (_uiObject != null && IsUIActive)
            {
                _uiObject.transform.position = Camera.main.WorldToScreenPoint(transform.position + Offset);
            }
        }

        void OnDestroy()
        {
            if(_uiObject != null)
                Destroy(_uiObject);
        }

        public void ShowEmotion(EmotionData.EmoticonType emotionType)
        {
            if (IsUIActive && EmotionUI != null && _uiObject != null)
            {
                var instance = GameObject.Instantiate(EmotionUI, Vector3.zero, Quaternion.identity) as GameObject;
                instance.transform.SetParent(_uiObject.transform);
                instance.transform.localPosition = Vector3.zero;
                instance.GetComponentsInChildren<Image>().Last().sprite = EmotionsPack.GetEmoticonSprite(emotionType);
            }
        }
    }
}
