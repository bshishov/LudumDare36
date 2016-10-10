using UnityEngine;

[CreateAssetMenu(fileName = "Emotion Data")]
public class SpellData : ScriptableObject
{
    public float CoolDown = 1f;
    public GameObject Prefab;
    public Sprite Icon;
    public string Description;
    public bool IsProjectile = true;
}
