using UnityEngine;
using System.Collections.Generic;
using Pathfinding;
using UnityEngine.Serialization;

public class PatrolPath : MonoBehaviour
{
    public enum PatrolPointActions { Continue, Wait, ChangePath, ExecuteFunction }
    public bool drawCircle = true;

    [System.Serializable]
	public class PatrolPoint
    {
        public enum FunctionTarget { NPC, PatrolPath }

        public Vector3 position;
        public PatrolPointActions action;
        public float waitTime = 0;
        public PatrolPath linkedPath;
        [FormerlySerializedAs("changeLikelyness")]
        public int actionProbability = 100;
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

    public Color gizmoDrawColor = Color.yellow;
    void OnDrawGizmos()
    {
        if (points.Count <= 1) { return; }
        Gizmos.color = gizmoDrawColor;
        for (int i = 1; i < points.Count; i++)
        {
            Gizmos.DrawLine(points[i].position, points[i - 1].position);
            if (points[i].action == PatrolPointActions.ChangePath && points[i].linkedPath)
            {
                int index = points[i].linkedPath.GetNearestPatrolPointIndex(points[i].position);
                if (index >= 0)
                {
                    Gizmos.color = Color.gray;
                    Gizmos.DrawLine(points[i].position, points[i].linkedPath.points[index].position);
                    Gizmos.color = gizmoDrawColor;
                }
            }
        }
        if (points.Count > 2 && drawCircle)
        {
            Gizmos.DrawLine(points[points.Count - 1].position, points[0].position);
        }
    }

    public List<PatrolPoint> points = new List<PatrolPoint>();
}
