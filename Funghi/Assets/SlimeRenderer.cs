using UnityEngine;
using System.Collections.Generic;
using System.Threading;

public class SlimeRenderer : MonoBehaviour
{
    Thread worker;
    private volatile bool isDone = true;
    private volatile bool pendingRefresh = false;

    [Range(0,1f)]
    public float smoothingStrength = 0.75f;
    private float oldSmooth = 1f;

    public float depth = -0.2f;
    private float oldDepth = 0f;

    MeshFilter mf;
    MeshRenderer mr;

    void Start()
    {
        if (!mf) { mf = GetComponent<MeshFilter>(); }
        if (!mf) { mf = gameObject.AddComponent<MeshFilter>(); }
        if (!mr) { mr = GetComponent<MeshRenderer>(); }
        if (!mr) { mr = gameObject.AddComponent<MeshRenderer>(); }
    }

    void Update()
    {
        if (smoothingStrength != oldSmooth)
        {
            Refresh();
            oldSmooth = smoothingStrength;
        }
        if (depth != oldDepth)
        {
            Refresh();
            oldDepth = depth;
        }
        if (isDone && generatedTris.Count > 0)
        {
            Mesh m = new Mesh();
            m.name = "Slime";
            m.SetVertices(generatedVerts);
            m.SetTriangles(generatedTris, 0);
            m.RecalculateBounds();
            generatedTris.Clear();
            generatedVerts.Clear();
            mf.sharedMesh = m;
        }
        if (pendingRefresh && isDone)
        {
            PrepareRebuild();
            pendingRefresh = false;
        }
    }

    #region internal
    private volatile float tileSize = 1f;
    private volatile List<int> generatedTris = new List<int>();
    private volatile List<Vector3> generatedVerts = new List<Vector3>();
    #endregion

    public void Refresh()
    {
        pendingRefresh = true;
    }

    private void PrepareRebuild()
    {
        isDone = false;
        List<Vector3> tileCenters = new List<Vector3>();
        foreach (FunNode fn in FungusNetwork.Instance.nodes)
        {
            foreach (List<Tile> tl in fn.SlimePaths)
            {
                for (int i = 0; i < tl.Count; i++)
                {
                    tileCenters.Add(tl[i].worldPosition);
                }
            }
        }
        tileSize = WorldGrid.Instance.tile_Size;
        worker = new Thread(RebuildMesh);
        worker.Start(tileCenters);
    }

    public void RebuildMesh(object threadObject)
    {
        List<Vector3> tileCenters = threadObject as List<Vector3>;
        List<Vector3> verts = new List<Vector3>();
        List<int> tris = new List<int>();
        for (int i = 0; i < tileCenters.Count; i++)
        {
            Vector3 center = tileCenters[i];
            center.z = depth;
            Vector3 upCenter = center + Vector3.up * tileSize * 0.5f;
            Vector3 botCenter = center + Vector3.down * tileSize * 0.5f;
            Vector3 leftCenter = center + Vector3.left * tileSize * 0.5f;
            Vector3 rightCenter = center + Vector3.right * tileSize * 0.5f;
            Vector3 upLeft = upCenter + Vector3.left * tileSize * 0.5f;
            Vector3 upRight = upLeft + Vector3.right * tileSize;
            Vector3 botLeft = botCenter + Vector3.left * tileSize * 0.5f;
            Vector3 botRight = botLeft + Vector3.right * tileSize;
            int centerIndex = verts.Count;
            verts.Add(center);
            int upLeftIndex = verts.IndexOf(upLeft);
            if (upLeftIndex == -1)
            {
                verts.Add(upLeft);//+1
                upLeftIndex = verts.Count - 1;
            }
            int upCenterIndex = verts.IndexOf(upCenter);
            if (upCenterIndex == -1)
            {
                verts.Add(upCenter);//+2
                upCenterIndex = verts.Count - 1;
            }
            int upRightIndex = verts.IndexOf(upRight);
            if (upRightIndex == -1)
            {
                verts.Add(upRight);//+3
                upRightIndex = verts.Count - 1;
            }
            int rightCenterIndex = verts.IndexOf(rightCenter);
            if (rightCenterIndex == -1)
            {
                verts.Add(rightCenter);//+4
                rightCenterIndex = verts.Count - 1;
            }
            int botRightIndex = verts.IndexOf(botRight);
            if (botRightIndex == -1)
            {
                verts.Add(botRight);//+5
                botRightIndex = verts.Count - 1;
            }
            int botCenterIndex = verts.IndexOf(botCenter);
            if (botCenterIndex == -1)
            {
                verts.Add(botCenter);//+6
                botCenterIndex = verts.Count - 1;
            }
            int botLeftIndex = verts.IndexOf(botLeft);
            if (botLeftIndex == -1)
            {
                verts.Add(botLeft);//+7
                botLeftIndex = verts.Count - 1;
            }
            int leftCenterIndex = verts.IndexOf(leftCenter);
            if (leftCenterIndex == -1)
            {
                verts.Add(leftCenter);//+8
                leftCenterIndex = verts.Count - 1;
            }
            tris.Add(centerIndex); tris.Add(upLeftIndex); tris.Add(upCenterIndex);
            tris.Add(centerIndex); tris.Add(upCenterIndex); tris.Add(upRightIndex);
            tris.Add(centerIndex); tris.Add(upRightIndex); tris.Add(rightCenterIndex);
            tris.Add(centerIndex); tris.Add(rightCenterIndex); tris.Add(botRightIndex);
            tris.Add(centerIndex); tris.Add(botRightIndex); tris.Add(botCenterIndex);
            tris.Add(centerIndex); tris.Add(botCenterIndex); tris.Add(botLeftIndex);
            tris.Add(centerIndex); tris.Add(botLeftIndex); tris.Add(leftCenterIndex);
            tris.Add(centerIndex); tris.Add(leftCenterIndex); tris.Add(upLeftIndex);
        }
        generatedVerts = Smooth(verts, tris);
        generatedTris = tris;
        isDone = true;
    }

    List<Vector3> Smooth(List<Vector3> vertices, List<int> tris)
    {
        List<Vector3> shrunk = new List<Vector3>(vertices);
        for (int i = 0; i < shrunk.Count; i++)
        {
            List<Vector3> neighbors = FindNeighborTris(vertices, tris, i);
            if (neighbors.Count > 4) { continue; }
            Vector3 shrunkCenter = Vector3.zero;
            for (int n = 0; n < neighbors.Count; n++)
            {
                shrunkCenter += neighbors[n];
            }
            if (neighbors.Count > 1)
            {
                shrunkCenter /= neighbors.Count;
            }
            shrunk[i] = Vector3.Lerp(shrunk[i], shrunkCenter, smoothingStrength);
        }
        return shrunk;
    }

    List<Vector3> FindNeighborTris(List<Vector3> verts, List<int> tris, int index)
    {
        HashSet<Vector3> neighbors = new HashSet<Vector3>();
        for (int i = 0; i < tris.Count/3; i++)
        {
            for (int t = 0; t < 3; t++)
            {
                int current = tris[(i * 3) + t];
                if (current == index)
                {
                    for (int ti = 0; ti < 3; ti++)
                    {
                        int vIndex = tris[i * 3 + ti];
                        if (vIndex == index)
                        {
                            continue;
                        }
                        neighbors.Add(verts[vIndex]);
                    }
                    break;
                }
            }
        }
        return new List<Vector3>(neighbors);
    }

}
