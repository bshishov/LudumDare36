using UnityEngine;

public class Emotion : MonoBehaviour
{
    public static string SetEmoticonMessageKey = "SetEmoticon";
    public EmotionData EmotionData;

    private CustomSpriteRenderer _emoticonRenderer;
	
	void Start ()
	{
	    //_emoticonRenderer = GetComponentInChildren<CustomSpriteRenderer>();
    }

    void AfterStart()
    {
        //_emoticonRenderer = GetComponentInChildren<CustomSpriteRenderer>();
    }

    public void Destroy()
    {
        GameObject.Destroy(gameObject);
    }

    public void SetEmoticon(EmotionData.EmoticonType emoticon)
    {
        var renderer = gameObject.transform.GetChild(0).GetComponent<CustomSpriteRenderer>();
        renderer.SetSprite(EmotionData.GetEmoticonSprite(emoticon));
    }
}
