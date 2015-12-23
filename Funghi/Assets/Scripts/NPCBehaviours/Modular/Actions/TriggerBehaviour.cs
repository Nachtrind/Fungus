namespace ModularBehaviour
{
    public class TriggerBehaviour : AIAction
    {
        public enum ParameterType { None, Self, VarValue, String }
        public string targetName = "target";
        public string triggerName = "Trigger";
        public ParameterType paramType = ParameterType.None;
        public string paramTypeValue = "target";
        public override ActionResult Run(IntelligenceController controller, float deltaTime)
        {
            Entity e;
            if (controller.GetMemoryValue(targetName, out e))
            {
                switch (paramType)
                {
                    case ParameterType.None:
                        if (e.TriggerBehaviour(triggerName))
                        {
                            return ActionResult.Finished;
                        }
                        return ActionResult.Failed;
                    case ParameterType.Self:
                        if (e.TriggerBehaviour(triggerName, controller.Owner))
                        {
                            return ActionResult.Finished;
                        }
                        return ActionResult.Failed;
                    case ParameterType.VarValue:
                        object value;
                        if (controller.GetMemoryValue(paramTypeValue, out value))
                        {
                            if (e.TriggerBehaviour(triggerName, value))
                            {
                                return ActionResult.Finished;
                            }
                        }
                        return ActionResult.Failed;
                    case ParameterType.String:
                        if (e.TriggerBehaviour(triggerName, paramTypeValue))
                        {
                            return ActionResult.Finished;
                        }
                        return ActionResult.Failed;
                }
            }
            return ActionResult.Failed;
        }

        public override void DrawGUI(IntelligenceState parentState, Intelligence intelligence, CallbackCollection callbacks)
        {
#if UNITY_EDITOR
            targetName = UnityEditor.EditorGUILayout.TextField("Target var:", targetName);
            triggerName = UnityEditor.EditorGUILayout.TextField("Trigger name:", triggerName);
            paramType = (ParameterType)UnityEditor.EditorGUILayout.EnumPopup("Parameter:", paramType);
            if (paramType == ParameterType.VarValue)
            {
                paramTypeValue = UnityEditor.EditorGUILayout.TextField("Var name:", paramTypeValue);
            }
            if (paramType == ParameterType.String)
            {
                paramTypeValue = UnityEditor.EditorGUILayout.TextField("Value:", paramTypeValue);
            }
#endif
        }
    }
}