namespace ModularBehaviour
{
    public class ExecuteMethod : OneShotAction
    {
        public enum ParameterType { None, Self, VarValue, String }
        public string methodName = "Trigger";
        public bool useTargetVar = false;
        public string targetVarName = "target";
        public ParameterType paramType = ParameterType.None;
        public string paramTypeValue = "target";
        public override ActionResult Fire(IntelligenceController controller)
        {
            if (methodName.Length > 1)
            {
                Entity e;
                if (useTargetVar)
                {
                    controller.GetMemoryValue(targetVarName, out e);
                }
                else
                {
                    e = controller.Owner;
                }
                if (!e) { return ActionResult.Failed; }
                switch (paramType)
                {
                    case ParameterType.None:
                        e.BroadcastMessage(methodName);
                        return ActionResult.Finished;
                    case ParameterType.Self:
                        e.BroadcastMessage(methodName, controller.Owner);
                        return ActionResult.Finished;
                    case ParameterType.VarValue:
                        object value;
                        if (controller.GetMemoryValue(paramTypeValue, out value))
                        {
                            e.BroadcastMessage(methodName, value);
                            return ActionResult.Finished;
                        }
                        return ActionResult.Failed;
                    case ParameterType.String:
                        e.BroadcastMessage(methodName, paramTypeValue);
                        return ActionResult.Finished;
                }
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
            useTargetVar = UnityEditor.EditorGUILayout.Toggle("Use other target", useTargetVar);
            if (useTargetVar)
            {
                UnityEditor.EditorGUILayout.BeginHorizontal();
                UnityEngine.GUILayout.Label("Target var name:");
                targetVarName = UnityEditor.EditorGUILayout.TextField(targetVarName);
                UnityEditor.EditorGUILayout.EndHorizontal();
            }
            paramType = (ParameterType)UnityEditor.EditorGUILayout.EnumPopup("Parameter:", paramType);
            if (paramType == ParameterType.VarValue)
            {
                UnityEditor.EditorGUILayout.BeginHorizontal();
                UnityEngine.GUILayout.Label("Var name:");
                paramTypeValue = UnityEditor.EditorGUILayout.TextField(paramTypeValue);
                UnityEditor.EditorGUILayout.EndHorizontal();
            }
            if (paramType == ParameterType.String)
            {
                UnityEditor.EditorGUILayout.BeginHorizontal();
                UnityEngine.GUILayout.Label("Value:");
                paramTypeValue = UnityEditor.EditorGUILayout.TextField(paramTypeValue);
                UnityEditor.EditorGUILayout.EndHorizontal();
            }
#endif
        }
    }
}