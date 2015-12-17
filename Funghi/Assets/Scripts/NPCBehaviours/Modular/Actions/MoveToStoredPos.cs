using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace ModularBehaviour
{
    public class MoveToStoredPos : AIAction
    {
        public string storeName = "";
        public override ActionResult Run(IntelligenceController controller, float deltaTime)
        {
            Vector3 target = controller.Owner.transform.position;
            if (controller.Storage.TryGetParameter(storeName, out target))
            {
                Human.MoveResult res = controller.Owner.MoveTo(target);
                if (res == Human.MoveResult.ReachedTarget)
                {
                    return ActionResult.Finished;
                }
                if (res == Human.MoveResult.TargetNotReachable)
                {
                    return ActionResult.Failed;
                }
            }
            else
            {
                Entity t = controller.Owner;
                if (controller.Storage.TryGetParameter(storeName, out t))
                {
                    Human.MoveResult res = controller.Owner.MoveTo(t.transform.position);
                    if (res == Human.MoveResult.ReachedTarget)
                    {
                        return ActionResult.Finished;
                    }
                    if (res == Human.MoveResult.TargetNotReachable)
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