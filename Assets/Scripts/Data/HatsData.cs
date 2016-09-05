using System;
using UnityEngine;
using System.Collections;
using System.Linq;

[CreateAssetMenu(fileName = "Hats Data")]
public class HatsData : ScriptableObject
{
    [Serializable]
    public struct Hat
    {
        public string Name;
        public GameObject Prefab;
        public Sprite Icon;
    }

    [SerializeField]
    public Hat[] Hats;

    public GameObject GetPrefab(string name)
    {
        return Hats.FirstOrDefault(h => h.Name == name).Prefab;
    }

    public Hat GetHat(string name)
    {
        return Hats.FirstOrDefault(h => h.Name == name);
    }
}
