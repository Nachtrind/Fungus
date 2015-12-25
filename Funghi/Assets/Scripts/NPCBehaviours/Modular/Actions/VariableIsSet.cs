using System;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace ModularBehaviour
{
    [ActionUsage(UsageType.AsCondition)]
    public class VariableIsSet : AIAction
    {
        public string varName = "target";
        public override ActionResult Run(IntelligenceController controller, float deltaTime)
        {
            return Fire(controller);
        }

        public override ActionResult Fire(IntelligenceController controller)
        {
            object o;
            if (controller.GetMemoryValue(varName, out o))
            {
                return ActionResult.Success;
            }
            return ActionResult.Failed;
        }

        public override void DrawGUI(IntelligenceState parentState, Intelligence intelligence, CallbackCollection callbacks)
        {
#if UNITY_EDITOR
            EditorGUILayout.BeginHorizontal();
            GUILayout.Label("var name: ");
            varName = EditorGUILayout.TextField(varName);
            EditorGUILayout.EndHorizontal();
#endif
        }
    }
}