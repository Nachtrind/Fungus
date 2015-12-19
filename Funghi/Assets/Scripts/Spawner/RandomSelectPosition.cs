using UnityEngine;
using System.Collections.Generic;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Spawner.Modules
{
    [ModuleDescription("Selects a random position from the ones provided")]
    public class RandomSelectPosition : PositionModule
    {
        public List<Vector3> positions = new List<Vector3>();
        public override void Apply(Entity e, ModuleWorker worker)
        {
            if (positions.Count > 0)
            {
                e.transform.position = positions[Random.Range(0, positions.Count - 1)];
            }
            worker.ProcessNext(e);
        }

        Color gizmoColor = new Color(0, 1f, 1f, 0.33f);
        public override void DrawGizmos(Vector3 center)
        {
            Gizmos.color = gizmoColor;
            for (int i = 0; i < positions.Count; i++)
            {
                Gizmos.DrawLine(center, positions[i]);
                Gizmos.DrawRay(positions[i], Vector3.up*0.5f);
            }
        }
#if UNITY_EDITOR
        public override void DrawHandles()
        {
            for (int i = 0; i < positions.Count; i++)
            {
                positions[i] = Handles.PositionHandle(positions[i], Quaternion.identity);
            }
        }
#endif
    }
}