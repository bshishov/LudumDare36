using UnityEngine;

using System.Linq;

[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(MeshRenderer))]
public class CustomSpriteRenderer : MonoBehaviour
{
    public Sprite Sprite;

    void Start()
    {
        Rebuild();
    }

    void Update()
    {

    }

    [ContextMenu("Rebuild")]
    void Rebuild()
    {
        transform.localScale = new Vector3(Sprite.bounds.extents.x, Sprite.bounds.extents.y, 0);

        var uv = new Vector2[4];
        uv[2] = new Vector2(Sprite.textureRect.xMax / Sprite.texture.width, Sprite.textureRect.yMin / Sprite.texture.height);
        uv[0] = new Vector2(Sprite.textureRect.xMin / Sprite.texture.width, Sprite.textureRect.yMin / Sprite.texture.height);
        uv[1] = new Vector2(Sprite.textureRect.xMax / Sprite.texture.width, Sprite.textureRect.yMax / Sprite.texture.height);
        uv[3] = new Vector2(Sprite.textureRect.xMin / Sprite.texture.width, Sprite.textureRect.yMax / Sprite.texture.height);
        GetComponent<MeshFilter>().mesh.uv = uv;

        var meshRenderer = GetComponent<MeshRenderer>();
        meshRenderer.material.SetTexture("_MainTex", Sprite.texture);
    }

    public void SetSprite(Sprite sprite)
    {
        this.Sprite = sprite;
        Rebuild();
    }

    public void Destroy()
    {
        Destroy(gameObject);
    }
}