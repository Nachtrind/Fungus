using System.Collections.Generic;
using ModularBehaviour;
using UnityEngine;
using Pathfinding;

public class SlimeHandler: MonoBehaviour
{

    public const int slimeTag = 0x1;
    public float slimeUpdateInterval = 0.5f;
    public bool smooth = false;
    float lastUpdate;

    [Tooltip("performancewise only works when growing (not on existing)")]
    public bool killHumans = true;

    public bool citizensOnly = true;

    List<SlimePath> slimePaths = new List<SlimePath>();
    StandardGameSettings sgs;

    [SerializeField]
    SimpleSmoothModifier pathSmoother;

    void Awake()
    {
        sgs = StandardGameSettings.Get;
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
                    if (killHumans)
                    {
                        KillHumansInSlimeRadius(slimePaths[i].a??slimePaths[i].b, result.point);
                        //Debug.Log(result.state);
                        //Debug.DrawRay(result.point, Vector3.up, Color.red, 2f);
                    }
                    for (int v = 0; v < slimePaths[i].path.Count; v++)
                    {
                        pendingAdds.Add(GetUpdateObject(slimePaths[i].path[v], sgs.connectionSlimeWidth, true));
                    }
                    break;
                case SlimePath.BuildState.Removing:
                    pendingRemoves.Add(GetUpdateObject(result.point, sgs.connectionSlimeWidth, false));
                    break;
                case SlimePath.BuildState.Invalidated:
                    CheckRemoveDisconnected(slimePaths[i].a);
                    CheckRemoveDisconnected(slimePaths[i].b);
                    slimePaths.RemoveAt(i);
                    continue;
            }
        }
        for (int i = 0; i < pendingRemoves.Count; i++)
        {
            AstarPath.active.UpdateGraphs(pendingRemoves[i]);
        }
        pendingRemoves.Clear();
        for (int i = 0; i < pendingAdds.Count; i++)
        {
            AstarPath.active.UpdateGraphs(pendingAdds[i]);
        }
        pendingAdds.Clear();
        lastUpdate = Time.time;
    }

    List<GraphUpdateObject> pendingAdds = new List<GraphUpdateObject>();
    List<GraphUpdateObject> pendingRemoves = new List<GraphUpdateObject>();

    void KillHumansInSlimeRadius(FungusNode growSource, Vector3 point)
    {
        var humans = citizensOnly
            ? GameWorld.Instance.GetHumans(point, StandardGameSettings.Get.connectionSlimeWidth*0.75f, IntelligenceType.Citizen)
            : GameWorld.Instance.GetHumans(point, StandardGameSettings.Get.connectionSlimeWidth*0.75f, IntelligenceType.Human);
        for (var i = 0; i < humans.Count; i++)
        {
            humans[i].Kill(growSource);
        }
    }

    public void QueueSlimeUpdate(Vector3 point, float size, bool state)
    {
        if (state)
        {
            pendingAdds.Add(GetUpdateObject(point, size, true));
        }
        else
        {
            pendingRemoves.Add(GetUpdateObject(point, size, false));
        }
        
    }

    GraphUpdateObject GetUpdateObject(Vector3 point, float radius, bool state)
    {
        GraphUpdateObject guo = new GraphUpdateObject(new Bounds(point, Vector3.one * radius));
        guo.modifyTag = true;
        guo.updatePhysics = false;
        guo.modifyWalkability = false;
        guo.setTag = state ? slimeTag : 0;
        return guo;
    }

    public bool GetPositionIsSlime(Vector3 point, float toleranceRadius)
    {
        point.y = 0;
        NNConstraint slimeConstraint = new NNConstraint();
        slimeConstraint.constrainTags = true;
        slimeConstraint.tags = ~slimeTag;
        NNInfo nn = AstarPath.active.GetNearest(point, slimeConstraint);
        if (AstarMath.SqrMagnitudeXZ(nn.clampedPosition, point) <= toleranceRadius * toleranceRadius)
        {
            Debug.DrawLine(nn.clampedPosition, point);
            return true;
        }
        return false;
    }

    void CheckRemoveDisconnected(FungusNode node)
    {
        if (node == null) { return; }
        pendingAdds.Add(GetUpdateObject(node.transform.position, sgs.nodeSlimeExtend, true));
        if (!node.IsConnected)
        {
            GameWorld.Instance.OnNodeWasDisconnected(node);
        }
    }
}

