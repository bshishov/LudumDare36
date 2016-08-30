using UnityEngine;
using System.Collections;
using UnityEngine.Networking;

public class EmotionHotkeys : NetworkBehaviour
{
    public GameObject EmotionBalloonPrefab;
    public float Cooldown = 2f;
    public Vector3 Offset = new Vector3(0, 1f, 0);
    public Vector3 Scale = Vector3.one;

    private float _lastTimeSpawn;

    void Start ()
    {
	}
	
	void Update ()
    {
	    if (isLocalPlayer)
	    {
	        if (Input.GetKeyDown(KeyCode.Alpha1))
                CmdSpawnEmotion((int)EmotionData.EmoticonType.Kappa);

	        if (Input.GetKeyDown(KeyCode.Alpha2))
                CmdSpawnEmotion((int)EmotionData.EmoticonType.Leonidas);

	        if (Input.GetKeyDown(KeyCode.Alpha3))
                CmdSpawnEmotion((int)EmotionData.EmoticonType.WTF);

            if (Input.GetKeyDown(KeyCode.Alpha4))
                CmdSpawnEmotion((int)EmotionData.EmoticonType.Fuck);

            if (Input.GetKeyDown(KeyCode.Alpha5))
                CmdSpawnEmotion((int)EmotionData.EmoticonType.NoComments);

            if (Input.GetKeyDown(KeyCode.Alpha6))
                CmdSpawnEmotion((int)EmotionData.EmoticonType.Love);

            if (Input.GetKeyDown(KeyCode.Alpha7))
                CmdSpawnEmotion((int)EmotionData.EmoticonType.Oops);

            if (Input.GetKeyDown(KeyCode.Alpha8))
                CmdSpawnEmotion((int)EmotionData.EmoticonType.Awkward);

            if (Input.GetKeyDown(KeyCode.Alpha9))
                CmdSpawnEmotion((int)EmotionData.EmoticonType.Question);

            if (Input.GetKeyDown(KeyCode.Alpha0))
                CmdSpawnEmotion((int)EmotionData.EmoticonType.Angry);
        }
    }

    [Command]
    void CmdSpawnEmotion(int type)
    {
        RpcSpawnEmotion(type);
    }

    [ClientRpc]
    void RpcSpawnEmotion(int rawType)
    {
        var type = (EmotionData.EmoticonType) rawType;

        if(Time.time < _lastTimeSpawn + Cooldown)
            return;

        var empty = new GameObject();
        empty.transform.SetParent(transform);
        empty.transform.localScale = Scale;
        var balloon = GameObject.Instantiate(EmotionBalloonPrefab, transform.position + Offset, Quaternion.identity, empty.transform) as GameObject;
        balloon.SendMessage(Emotion.SetEmoticonMessageKey, type);
        _lastTimeSpawn = Time.time;
    }
}
