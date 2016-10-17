using System;
using System.Linq;
using UnityEngine;

namespace Assets.Scripts.Data
{
    [CreateAssetMenu(fileName = "Emotion Data")]
    public class EmotionData : ScriptableObject
    {
        [Serializable]
        public struct Emoticon
        {
            public EmoticonType Type;
            public Sprite Sprite;
        }

        public enum EmoticonType : int
        {
            Kappa,
            Leonidas,
            WTF,
            Fuck,
            Angry,
            Awkward,
            Cute,
            Facepalm,
            Love,
            NoComments,
            Oops,
            Question,
            Zzzz
        }

        [SerializeField]
        public Emoticon[] Emoticons;

        public Sprite GetEmoticonSprite(EmotionData.EmoticonType emoticon)
        {
            return Emoticons.FirstOrDefault(e => e.Type == emoticon).Sprite;
        }
    }
}
