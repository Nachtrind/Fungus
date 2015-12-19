using UnityEngine;
using System.Collections.Generic;
using Pathfinding;

public class PatrolPath : MonoBehaviour
{
    public enum PatrolPointActions { Continue, Wait, ChangePath, ExecuteFunction }

    [System.Serializable]
	public class PatrolPoint
    {
        public enum FunctionTarget { NPC, PatrolPath }

        public Vector3 position;
        public PatrolPointActions action;
        public float waitTime = 0;
        public PatrolPath linkedPath;
        public int changeLikelyness = 100;
        public string functionName = "";
        public FunctionTarget target;
    }

    public int GetNearestPatrolPointIndex(Vector3 position)
    {
        if (points.Count == 0) { throw new System.Exception("Path has no points"); }
        int nearest = 0;
        float nearestDist = AstarMath.SqrMagnitudeXZ(position, points[0].position);
        for (int i = 1; i < points.Count; i++)
        {
            float newnearDist = AstarMath.SqrMagnitudeXZ(position, points[i].position);
            if (newnearDist < nearestDist)
            {
                nearest = i;
                nearestDist = newnearDist;
            }
        }
        return nearest;
    }

    public List<PatrolPoint> points = new List<PatrolPoint>();
}
