using System.Collections.Generic;
using UnityEngine;
using Pathfinding;

public class SlimeHandler: MonoBehaviour
{

    public float slimeUpdateInterval = 0.5f;
    public bool smooth = false;
    float lastUpdate;

    public float slimeSize = 0.3f;

    const float downSampling = 0.25f;

    List<SlimePath> slimePaths = new List<SlimePath>();

    GameWorld world;

    [SerializeField]
    SimpleSmoothModifier pathSmoother;

    void Awake()
    {
        world = GameWorld.Instance;
        rt = new RenderTexture(Mathf.ClosestPowerOfTwo((int)(Screen.width * downSampling)), Mathf.ClosestPowerOfTwo((int)(Screen.height * downSampling)), 0);
        whiteMaterial.color = Color.green;
    }

    void OnApplicationQuit()
    {
        groundMaterial.mainTexture = null;
    }

    public List<Vector3> Smooth(List<Vector3> vectorPath, SimpleSmoothModifier.SmoothType smoothType)
    {
        switch (smoothType)
        {
            default:
                return pathSmoother.SmoothSimple(vectorPath);
            case SimpleSmoothModifier.SmoothType.Bezier:
                return pathSmoother.SmoothBezier(vectorPath);
            case SimpleSmoothModifier.SmoothType.OffsetSimple:
                return pathSmoother.SmoothOffsetSimple(vectorPath);
            case SimpleSmoothModifier.SmoothType.CurvedNonuniform:
                return pathSmoother.CurvedNonuniform(vectorPath);
        }
    }

    public List<Vector3> SmoothLikeSlime(List<Vector3> vectorPath)
    {
        if (!smooth) { return vectorPath; }
        switch (pathSmoother.smoothType)
        {
            default:
                return pathSmoother.SmoothSimple(vectorPath);
            case SimpleSmoothModifier.SmoothType.Bezier:
                return pathSmoother.SmoothBezier(vectorPath);
            case SimpleSmoothModifier.SmoothType.OffsetSimple:
                return pathSmoother.SmoothOffsetSimple(vectorPath);
            case SimpleSmoothModifier.SmoothType.CurvedNonuniform:
                return pathSmoother.CurvedNonuniform(vectorPath);
        }
    }

    public void AddConnection(SlimePath path)
    {
        if (smooth)
        {
            switch (pathSmoother.smoothType)
            {
                default:
                    path.ApplySmoother(pathSmoother.SmoothSimple);
                    break;
                case SimpleSmoothModifier.SmoothType.Bezier:
                    path.ApplySmoother(pathSmoother.SmoothBezier);
                    break;
                case SimpleSmoothModifier.SmoothType.OffsetSimple:
                    path.ApplySmoother(pathSmoother.SmoothOffsetSimple);
                    break;
                case SimpleSmoothModifier.SmoothType.CurvedNonuniform:
                    path.ApplySmoother(pathSmoother.CurvedNonuniform);
                    break;
            }
        }
        slimePaths.Add(path);
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        for (int i = 0; i < slimePaths.Count; i++)
        {
            for (int p = 1; p < slimePaths[i].path.Count; p++)
            {
                Gizmos.DrawLine(slimePaths[i].path[p], slimePaths[i].path[p - 1]);
            }
        }
    }

    #region Rendering

    public Material groundMaterial;
    public Material whiteMaterial;

    RenderTexture rt;

    void OnRenderObject()
    {
        RenderTexture previousRT = RenderTexture.active;
        RenderTexture.active = rt;
        GridGraph gg = AstarPath.active.astarData.gridGraph;
        whiteMaterial.SetPass(0);
        GL.Clear(true, true, Color.red);
        GL.PushMatrix();
        GL.LoadProjectionMatrix(Camera.main.projectionMatrix);
        GL.LoadIdentity();
        GL.MultMatrix(Camera.main.worldToCameraMatrix);
        GL.Begin(GL.QUADS);
        GL.Color(Color.green);
        for (int i = 0; i < gg.nodes.Length; i++)
        {
            if (gg.nodes[i].Tag != GameWorld.slimeTag || !gg.nodes[i].Walkable)
            {
                continue;
            }
            Vector3 bottomLeft = (Vector3)gg.nodes[i].position;
            bottomLeft.x -= (gg.nodeSize * 0.5f);
            bottomLeft.z -= (gg.nodeSize * 0.5f);
            GL.Vertex(bottomLeft);
            GL.Vertex3(bottomLeft.x, 0, bottomLeft.z + gg.nodeSize);
            GL.Vertex3(bottomLeft.x + gg.nodeSize, 0, bottomLeft.z + gg.nodeSize);
            GL.Vertex3(bottomLeft.x+ gg.nodeSize, 0, bottomLeft.z);
        }
        GL.End();
        GL.PopMatrix();
        RenderTexture.active = previousRT;
        groundMaterial.mainTexture = rt;
    }
    #endregion

    public void UpdateSlimeConnections()
    {
        if (Time.time - lastUpdate < slimeUpdateInterval)
        {
            return;
        }
        for (int i = slimePaths.Count; i-- > 0;)
        {
            SlimePath.SlimeRefreshResult result = slimePaths[i].Refresh();
            switch (result.state)
            {
                default:
                    for (int v = 0; v < slimePaths[i].path.Count; v++)
                    {
                        world.SetPositionIsSlime(slimePaths[i].path[v], slimeSize, true);
                    }
                    break;
                //case SlimePath.BuildState.Building:
                //    world.SetPositionIsSlime(result.point, slimeSize, true);
                //    break;
                case SlimePath.BuildState.Removing:
                    world.SetPositionIsSlime(result.point, slimeSize, false);
                    break;
                case SlimePath.BuildState.Invalidated:
                    CheckRemoveDisconnected(slimePaths[i].a);
                    CheckRemoveDisconnected(slimePaths[i].b);
                    slimePaths.RemoveAt(i);
                    continue;
            }
        }
        lastUpdate = Time.time;
    }

    void CheckRemoveDisconnected(FungusNode node)
    {
        if (node == null) { return; }
        if (!node.IsConnected)
        {
            GameWorld.Instance.OnNodeWasDisconnected(node);
        }
    }
}

