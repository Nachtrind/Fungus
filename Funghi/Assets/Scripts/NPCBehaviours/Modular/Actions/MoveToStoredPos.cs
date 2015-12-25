using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace ModularBehaviour
{
    [ActionUsage(UsageType.AsCondition, UsageType.AsContinuous)]
    public class MoveToStoredPos : AIAction
    {
        public string storeName = "";
        public override ActionResult Run(IntelligenceController controller, float deltaTime)
        {
            Vector3 target = controller.Owner.transform.position;
            if (controller.GetMemoryValue(storeName, out target))
            {
                EntityMover.MoveResult res = controller.Owner.MoveTo(target);
                if (res == EntityMover.MoveResult.ReachedTarget)
                {
                    return ActionResult.Success;
                }
                if (res == EntityMover.MoveResult.TargetNotReachable)
                {
                    return ActionResult.Failed;
                }
            }
            else
            {
                Entity t = controller.Owner;
                if (controller.GetMemoryValue(storeName, out t))
                {
                    EntityMover.MoveResult res = controller.Owner.MoveTo(t.transform.position);
                    if (res == EntityMover.MoveResult.ReachedTarget)
                    {
                        return ActionResult.Success;
                    }
                    if (res == EntityMover.MoveResult.TargetNotReachable)
                    {
                        return ActionResult.Failed;
                    }
                }
            }
            return ActionResult.Running;
        }

        public override void DrawGUI(IntelligenceState parentState, Intelligence intelligence, CallbackCollection callbacks)
        {
#if UNITY_EDITOR
            EditorGUILayout.BeginHorizontal();
            GUILayout.Label("Var Name:", GUILayout.Width(64));
            storeName = EditorGUILayout.TextField(storeName);
            EditorGUILayout.EndHorizontal();
                #endif
        }
    }

}