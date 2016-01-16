using System.Collections.Generic;
using UnityEngine;
using Pathfinding;

public class SlimeHandler: MonoBehaviour
{

    public float slimeUpdateInterval = 0.5f;
    public bool smooth = false;
    float lastUpdate;

    public float slimeSize = 0.3f;

    List<SlimePath> slimePaths = new List<SlimePath>();

    GameWorld world;

    [SerializeField]
    SimpleSmoothModifier pathSmoother;

    void Awake()
    {
        world = GameWorld.Instance;
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

    public void ClearAllConnections()
    {
        slimePaths.Clear();
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

