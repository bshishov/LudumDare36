using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayersList : MonoBehaviour {

    public static List<NetworkPlayerIdentity> Players = new List<NetworkPlayerIdentity>();

    public GameObject ItemTemplate;

    public void Update()
    {
        foreach (var player in Players)
        {
            var row = gameObject.transform.FindChild(player.Identity.Name);
            if (row == null)
            {
                var newItem = Instantiate(ItemTemplate, gameObject.transform) as GameObject;
                newItem.transform.FindChild("Name").gameObject.GetComponent<Text>().text = player.Identity.Name;
                newItem.transform.FindChild("DeathsNumber").GetComponent<Text>().text = 0.ToString();
                newItem.name = player.Identity.Name;
            }
            else
            {
                row.FindChild("DeathsNumber").GetComponent<Text>().text = player.gameObject.GetComponent<PlayerState>().DeathsCount.ToString();
            }
        }
    }
}
