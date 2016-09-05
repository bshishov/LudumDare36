using System;
using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

[CreateAssetMenu(fileName = "Level Pack")]
public class LevelPack : ScriptableObject
{
    public string Name;

    [Serializable]
    public struct LevelData
    {
        public Sprite Thumbnail;
        public string Name;
        public string SceneName;
        public string Author;
    }

    public LevelData[] Levels;
}
