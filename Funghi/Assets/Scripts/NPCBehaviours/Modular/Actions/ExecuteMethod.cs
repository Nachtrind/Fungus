namespace ModularBehaviour
{
    public class ExecuteMethod : OneShotAction
    {
        public string methodName = "Trigger";
        public override ActionResult Fire(IntelligenceController controller)
        {
            if (methodName.Length > 1)
            {
                controller.Owner.BroadcastMessage(methodName);
                return ActionResult.Finished;
            }
            return ActionResult.Failed;
        }

        public override void DrawGUI(IntelligenceState parentState, Intelligence intelligence, CallbackCollection callbacks)
        {
#if UNITY_EDITOR
            UnityEditor.EditorGUILayout.BeginHorizontal();
            UnityEngine.GUILayout.Label("Method name:");
            methodName = UnityEditor.EditorGUILayout.TextField(methodName);
            UnityEditor.EditorGUILayout.EndHorizontal();
#endif
        }
    }
}