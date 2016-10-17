using System;
using UnityEngine;

namespace Assets.Scripts.Gameplay.Player
{
    public class PlayerIdentity : MonoBehaviour
    {
        public event Action<string> NameChanged;
        public string Name { get; private set; }
        public string HatName { get; private set; }
        public Color Color { get; private set; }

        public HatsData Hats;
        public Vector3 Offset = Vector3.up;
        public Vector3 Rotation = Vector3.zero;
        public string HeadObjectName = "";

        private Transform _headTransform;
        private readonly string _defaultHat = String.Empty;
        private Color _defaultColor = Color.white;
        private readonly string _defaultName = "Unnamed";
        private GameObject _hatObject;

        void Awake()
        {
            HatName = _defaultHat;
            Name = _defaultName;
            Color = _defaultColor;
        }

        void Start()
        {
            if (string.IsNullOrEmpty(HeadObjectName))
                _headTransform = gameObject.transform;
            else
                _headTransform = gameObject.transform.FindChild(HeadObjectName);

            if (gameObject.name == "Dummy")
            {
                SetFromPlayerPrefs();
            }
        }

        public void SetHat(string hatName)
        {
            if (!string.IsNullOrEmpty(hatName))
            {
                var hat = Hats.GetHat(hatName);
                if (hat.Prefab == null)
                {
                    Debug.LogWarningFormat("HAT {0} NOT FOUND", hatName);
                    return;
                }

                HatName = hatName;

                _headTransform = gameObject.transform.FindChild(HeadObjectName);

                if(_hatObject != null)
                    Destroy(_hatObject);

                _hatObject = GameObject.Instantiate(hat.Prefab, Vector3.zero, Quaternion.identity) as GameObject;
                _hatObject.transform.SetParent(_headTransform);
                _hatObject.transform.Rotate(_headTransform.rotation.eulerAngles + Rotation + hat.Prefab.transform.localEulerAngles);
                _hatObject.transform.localPosition = Offset + hat.Prefab.transform.position;
            }
        }

        public void SetColor(Color color)
        {
            Color = color;
            var meshRenderer = GetComponentInChildren<SkinnedMeshRenderer>();
            if (meshRenderer != null)
            {
                meshRenderer.material.color = color;
            }
        }

        public void SetColor(string colorName)
        {
            switch (colorName.ToLowerInvariant())
            {
                case "white":
                    SetColor(Color.white);
                    break;
                case "blue":
                    SetColor(Color.blue);
                    break;
                case "red":
                    SetColor(Color.red);
                    break;
                case "black":
                    SetColor(Color.black);
                    break;
                case "random":
                    SetColor(UnityEngine.Random.ColorHSV());
                    break;
            }
        }

        public void SetName(string playerName)
        {
            if (string.IsNullOrEmpty(playerName))
            {
                Debug.LogWarning("Trying to set an empty name");
                return;
            }

            if(playerName.Length > 15)
            {
                Debug.LogWarning("Name is too long");
                return;
            }

            Name = playerName;

            if (NameChanged != null)
                NameChanged(Name);
        }

        public void SetFromPlayerPrefs()
        {
            SetHat(PlayerPrefs.GetString("player_hat", _defaultHat));
            SetColor(new Color(
                PlayerPrefs.GetFloat("player_color_r", _defaultColor.r),
                PlayerPrefs.GetFloat("player_color_g", _defaultColor.g),
                PlayerPrefs.GetFloat("player_color_b", _defaultColor.b))
                );
            SetName(PlayerPrefs.GetString("player_name", _defaultName));
        }

        public void SaveToPlayerPrefs()
        {
            PlayerPrefs.SetString("player_hat", HatName);
            PlayerPrefs.SetString("player_name", Name);
            PlayerPrefs.SetFloat("player_color_r", Color.r);
            PlayerPrefs.SetFloat("player_color_g", Color.g);
            PlayerPrefs.SetFloat("player_color_b", Color.b);
        }
    }
}
