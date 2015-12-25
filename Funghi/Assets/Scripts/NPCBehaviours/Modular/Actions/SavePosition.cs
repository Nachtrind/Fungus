namespace ModularBehaviour
{
    [ActionUsage(UsageType.AsOneShot, UsageType.AsCondition)]
    public class SavePosition : AIAction
    {
        public enum TargetType { Self, TargetVar }
        public TargetType target = TargetType.Self;
        public string targetVarName = "target";
        public string varName = "SavedPosition";
        public override ActionResult Fire(IntelligenceController controller)
        {
            if (varName.Length < 1) { return ActionResult.Failed; }
            if (target == TargetType.Self)
            {
                controller.SetMemoryValue(varName, controller.Owner.transform.position);
                return ActionResult.Success;
            }
            Entity e;
            if (controller.GetMemoryValue(targetVarName, out e))
            {
                controller.SetMemoryValue(varName, e.transform.position);
                return ActionResult.Success;
            }
            return ActionResult.Failed;
        }

        public override ActionResult Run(IntelligenceController controller, float deltaTime)
        {
            return Fire(controller);
        }

        public override void DrawGUI(IntelligenceState parentState, Intelligence intelligence, CallbackCollection callbacks)
        {
#if UNITY_EDITOR
            target = (TargetType)UnityEditor.EditorGUILayout.EnumPopup("Target:", target);
            if (target == TargetType.TargetVar)
            {
                UnityEditor.EditorGUILayout.BeginHorizontal();
                UnityEngine.GUILayout.Label("Target var name:");
                targetVarName = UnityEditor.EditorGUILayout.TextField(targetVarName);
                UnityEditor.EditorGUILayout.EndHorizontal();
            }
            UnityEditor.EditorGUILayout.BeginHorizontal();
            UnityEngine.GUILayout.Label("Save var name:");
            varName = UnityEditor.EditorGUILayout.TextField(varName);
            UnityEditor.EditorGUILayout.EndHorizontal();
#endif
        }
    }
}