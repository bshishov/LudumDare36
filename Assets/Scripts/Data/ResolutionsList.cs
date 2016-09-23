using UnityEngine;
using System.Collections;
using System;

[CreateAssetMenu(fileName = "Resolutions")]
public class ResolutionsList : ScriptableObject {

    [Serializable]
    public struct Resolution
    {
        public int Width;
        public int Height;
    }

    public Resolution[] Resolutions;
}
