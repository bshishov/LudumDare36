using System.Collections.Generic;
using Assets.Scripts.Gameplay.Player;
using UnityEngine;
using UnityEngine.UI;

public class PlayersList : MonoBehaviour {

    public static List<NetworkPlayerIdentity> Players = new List<NetworkPlayerIdentity>();

    public GameObject ItemTemplate;

    public void Update()
    {
        foreach (var player in Players)
        {
            var row = gameObject.transform.Find(player.Identity.Name);
            if (row == null)
            {
                var newItem = Instantiate(ItemTemplate, gameObject.transform) as GameObject;
                var info = newItem.GetComponent<ScoresItemTemplateInfo>();
                info.NameText.text = player.Identity.Name;
                info.DeathsText.text = 0.ToString();
                newItem.name = player.Identity.Name;
            }
            else
            {
                row.GetComponent<ScoresItemTemplateInfo>().DeathsText.text = player.gameObject.GetComponent<PlayerState>().DeathsCount.ToString();
            }
        }
    }
}
