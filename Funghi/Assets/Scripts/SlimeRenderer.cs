using UnityEngine;
using UnityEngine.Rendering;
using System.Collections.Generic;

public class SlimeRenderer : MonoBehaviour
{
    List<Tile> GetSlimeTiles()
    {
        List<Tile> tiles = new List<Tile>();
        foreach (FunNode fn in FungusNetwork.Instance.nodes)
        {
            foreach (List<Tile> tl in fn.SlimePaths)
            {
                tiles.AddRange(tl);
            }
        }
        return tiles;
    }

    public Material intermediate;
    public Material postProcessor;
    Mesh plane;

    void Start()
    {
        plane = new Mesh();
        float tileSize = WorldGrid.Instance.tile_Size;
        List<Vector3> verts = new List<Vector3>
        {
            new Vector2(0,0),
            new Vector2(0,tileSize),
            new Vector2(tileSize,tileSize),
            new Vector2(tileSize,0),
        };
        plane.SetVertices(verts);
        List<int> tris = new List<int>
        {
            0,1,2,
            0,2,3
        };
        plane.SetTriangles(tris, 0);
    }

    public float downSampling = 0.5f;

    void OnRenderObject()
    {
        RenderTexture previous = RenderTexture.active;
        RenderTexture temp = RenderTexture.GetTemporary((int)(Screen.width*downSampling), (int)(Screen.height*downSampling), 0, RenderTextureFormat.ARGB32, RenderTextureReadWrite.Default, 2);
        temp.DiscardContents();
        RenderTexture.active = temp;
        intermediate.SetPass(0);
        foreach (Tile t in GetSlimeTiles())
        {
            Graphics.DrawMeshNow(plane, Matrix4x4.TRS(t.worldPosition, Quaternion.identity, Vector3.one), 0);
        }
        Graphics.Blit(temp, null, postProcessor);
        RenderTexture.active = previous;
        RenderTexture.ReleaseTemporary(temp);
    }
}
