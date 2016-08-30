using UnityEngine;

using System.Linq;

[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(MeshRenderer))]
public class CustomSpriteRenderer : MonoBehaviour
{
    public enum CustomSpriteRendererType
    {
        BuildMeshFromSprite,
        UseQuad
    }

    public Sprite Sprite;
    //public CustomSpriteRendererType Type = CustomSpriteRendererType.UseQuad;
    public bool BuildOnStart = true;

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
        var uv = new Vector2[4];
        uv[1] = new Vector2(Sprite.textureRect.xMin / Sprite.texture.width, Sprite.textureRect.yMin / Sprite.texture.height);
        uv[3] = new Vector2(Sprite.textureRect.xMax / Sprite.texture.width, Sprite.textureRect.yMin / Sprite.texture.height);
        uv[2] = new Vector2(Sprite.textureRect.xMin / Sprite.texture.width, Sprite.textureRect.yMax / Sprite.texture.height);
        uv[0] = new Vector2(Sprite.textureRect.xMax / Sprite.texture.width, Sprite.textureRect.yMax / Sprite.texture.height);
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