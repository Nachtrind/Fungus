using System.Collections.Generic;
using UnityEngine;

public class SlimePath
{
    public FungusNode a;
    public FungusNode b;

    public List<Vector3> path = new List<Vector3>();
    List<Vector3> fullPath = new List<Vector3>();

    public enum BuildState { Idling, Building, Removing, Invalidated }

    public SlimePath(FungusNode a, FungusNode b, List<Vector3> path)
    {
        this.a = a;
        this.b = b;
        fullPath = path;
    }

    public void ApplySmoother(System.Func<List<Vector3>, List<Vector3>> smoother)
    {
        fullPath = smoother(fullPath);
    }

    public bool HasPart(FungusNode node)
    {
        return a == node || b == node;
    }

    public class SlimeRefreshResult
    {
        public BuildState state;
        public Vector3 point;
    }

    SlimeRefreshResult res = new SlimeRefreshResult();

    public SlimeRefreshResult Refresh()
    {
        res.state = BuildState.Idling;
        if (fullPath.Count > 0)
        {
            if (a == null || b == null) //stop building
            {
                fullPath.Clear();
            }
            else //build
            {
                path.Add(fullPath[0]);
                res.point = fullPath[0];
                fullPath.RemoveAt(0);
                res.state = BuildState.Building;
                return res;
            }
        }
        if (path.Count == 0)
        {
            res.state = BuildState.Invalidated;
            return res;
        }
        if (a == null) //shrink from start
        {
            res.point = path[0];
            res.state = BuildState.Removing;
            path.RemoveAt(0);
        }
        if (b == null) //shrink from end
        {
            res.point = path[path.Count - 1];
            res.state = BuildState.Removing;
            path.RemoveAt(path.Count - 1);
        }
        return res;
    }
}
