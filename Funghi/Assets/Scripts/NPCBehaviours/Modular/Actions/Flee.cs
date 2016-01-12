using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;

namespace ModularBehaviour
{
    [ActionUsage(UsageType.AsCondition, UsageType.AsContinuous)]
    public class Flee : AIAction
    {
        public float desiredDistance = 1f;
        public string fleeFrom = "target";
        public override ActionResult Run(IntelligenceController controller, float deltaTime)
        {
            Vector3 fleeSource;
            if (!controller.GetMemoryValue(fleeFrom, out fleeSource))
            {
                Entity e;
                if (!controller.GetMemoryValue(fleeFrom, out e))
                {
                    return ActionResult.Failed;
                }
                fleeSource = e.transform.position;
            }
            Vector3 fleeDirection = controller.Owner.transform.position - fleeSource;
            EntityMover.MoveResult res = controller.Owner.MoveTo(controller.Owner.transform.position + fleeDirection);
            if (res == EntityMover.MoveResult.ReachedTarget || res == EntityMover.MoveResult.TargetNotReachable)
            {
                return ActionResult.Success;
            }
            if (Vector3.SqrMagnitude(controller.Owner.transform.position - fleeSource) >= desiredDistance * desiredDistance)
            {
                return ActionResult.Success;
            }
            return ActionResult.Running;
        }

        public override void DrawGUI(IntelligenceState parentState, Intelligence intelligence, CallbackCollection callbacks)
        {
#if UNITY_EDITOR
            desiredDistance = UnityEditor.EditorGUILayout.FloatField("distance:", desiredDistance);
            fleeFrom = UnityEditor.EditorGUILayout.TextField("from:", fleeFrom);
#endif
        }
    }
}