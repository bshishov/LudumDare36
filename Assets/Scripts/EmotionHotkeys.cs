using UnityEngine;
using System.Collections;

public class EmotionTest : MonoBehaviour
{
    public struct EmotionHotkey
    {
        public KeyCode KeyCode;
        public EmotionData.EmoticonType Type;
    }

    public GameObject EmotionBalloonPrefab;
    
    public 

    void Start ()
    {
	
	}
	
	void Update ()
    {
	    if (Input.GetKeyDown(KeyCode.Alpha1))
	    {
	        var balloon = GameObject.Instantiate(EmotionBalloonPrefab, transform.position, Quaternion.identity) as GameObject;
	        balloon.SendMessage(Emotion.SetEmoticonMessageKey, EmotionData.EmoticonType.Kappa);
	    }

        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            var balloon = GameObject.Instantiate(EmotionBalloonPrefab, transform.position, Quaternion.identity) as GameObject;
            balloon.SendMessage(Emotion.SetEmoticonMessageKey, EmotionData.EmoticonType.Leonidas);
        }
    }
}
