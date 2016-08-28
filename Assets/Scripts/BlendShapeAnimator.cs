using UnityEngine;
using System.Collections;

public class BlendShapeAnimator : MonoBehaviour
{
    public float Speed = 1f;
    public int StartFrame = 0;
    public int EndFrame = 2;

    private SkinnedMeshRenderer _skinnedMeshRenderer;
    private Mesh _skinnedMesh;
    private int _count;

    private int _indexOne;
    private int _indexTwo;
    private float _state = 0f;

    void Awake()
    {
        _indexOne = StartFrame;
        _indexTwo = StartFrame + 1;
        _skinnedMeshRenderer = GetComponent<SkinnedMeshRenderer>();
        _skinnedMesh = GetComponent<SkinnedMeshRenderer>().sharedMesh;
        _skinnedMeshRenderer.SetBlendShapeWeight(_indexOne, 100);
    }

    void Start()
    {
        _count = EndFrame - StartFrame;
    }

    void Update()
    {
        _state += Time.deltaTime * Speed;

        _skinnedMeshRenderer.SetBlendShapeWeight(_indexOne, 100f - 100f * _state);
        _skinnedMeshRenderer.SetBlendShapeWeight(_indexTwo, 100f * _state);

        if (_state > 1f)
        {
            _skinnedMeshRenderer.SetBlendShapeWeight(_indexOne, 0f);
            _skinnedMeshRenderer.SetBlendShapeWeight(_indexTwo, 100f);
            _state = 0f;

            _indexOne++;
            _indexTwo++;

            if (_indexOne >= EndFrame)
                _indexOne = StartFrame;

            if (_indexTwo >= EndFrame)
                _indexTwo = StartFrame;
        }
    }
}
