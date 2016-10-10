using UnityEngine;

[CreateAssetMenu(fileName = "Emotion Data")]
public class BuffData : ScriptableObject
{
    public bool CanWalk = true;
    public bool CanCast = true;
    public bool CanRotate = true;
    public bool InverseMovement = false;
    public float Duration = 1f;
    public GameObject EffectPrefab;
    public bool Stacks = false;
}
