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

    void Start ()
	{
	    var meshFilter = GetComponent<MeshFilter>();
	    if (meshFilter.mesh != null)
	    {
            Debug.Log("NO MESH FOUND");

            if (BuildOnStart)
	        {
                Debug.Log("REBUILD MESH");
                Rebuild();
	        }
	    }
	}
	
	void Update ()
    {
	
	}

    [ContextMenu("Rebuild")]
    void Rebuild()
    {
        var meshFilter = GetComponent<MeshFilter>();
        var mesh = new Mesh();
        mesh.vertices = Sprite.vertices.Select(v => new Vector3(v.x, v.y, 0)).ToArray();
        mesh.triangles = Sprite.triangles.Select(v => (int)v).ToArray();
        mesh.uv = Sprite.uv;
        meshFilter.mesh = mesh;

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
