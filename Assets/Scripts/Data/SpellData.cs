using System;
using UnityEngine;

namespace Assets.Scripts.Data
{
    [CreateAssetMenu(fileName = "Spell")]
    public class SpellData : ScriptableObject
    {
        public float CoolDown = 1f;
        public Sprite Icon;
        public string Description;
        public SpellHandler Handler;
    }
}