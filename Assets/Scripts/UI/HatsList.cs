using Assets.Scripts.Gameplay.Player;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.UI
{
    public class HatsList : MonoBehaviour
    {
        public HatsData Hats;
        public GameObject HatThumbnailPrefab;
        public GameObject Dummy;
	
        void Start ()
        {
            foreach (var hat in Hats.Hats)
            {
                var hatThumb = GameObject.Instantiate(HatThumbnailPrefab, transform) as GameObject;
                hatThumb.GetComponent<Image>().sprite = hat.Icon;
                var hat1 = hat;
                hatThumb.GetComponent<Toggle>().onValueChanged.AddListener((toggled) =>
                {
                    this.ValueChanged(hat1.Name);
                });
            }
        }

        void ValueChanged(string hatName)
        {
            if (Dummy != null)
            {
                var identity = Dummy.GetComponent<PlayerIdentity>();
                if (identity != null)
                {
                    identity.SetHat(hatName);
                    identity.SaveToPlayerPrefs();
                }
            }
        }
    }
}
