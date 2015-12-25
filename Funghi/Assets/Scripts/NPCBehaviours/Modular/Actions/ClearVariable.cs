using System;
using UnityEngine;

namespace ModularBehaviour
{
    [ActionUsage(UsageType.AsOneShot, UsageType.AsCondition)]
    public class ClearVariable : AIAction
    {
        public string varName = "target";
        public override ActionResult Fire(IntelligenceController controller)
        {
            controller.SetMemoryValue(varName, null);
            return ActionResult.Success;
        }

        public override ActionResult Run(IntelligenceController controller, float deltaTime)
        {
            return Fire(controller);
        }

        public override void DrawGUI(IntelligenceState parentState, Intelligence intelligence, CallbackCollection callbacks)
        {
#if UNITY_EDITOR
            UnityEditor.EditorGUILayout.BeginHorizontal();
            GUILayout.Label("Var name:");
            varName = UnityEditor.EditorGUILayout.TextField(varName);
            UnityEditor.EditorGUILayout.EndHorizontal();
#endif
        }
    }
}