using System;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace ModularBehaviour
{
    public class Attack : AIAction
    {
        public string entityVarName = "target";
        public int damagePerSecond = 20;
        public override ActionResult Run(IntelligenceController controller, float deltaTime)
        {
            Entity target;
            if (controller.Storage.TryGetParameter(entityVarName, out target))
            {
                if (!target.isAttackable) { return ActionResult.Failed; }
                target.Damage(controller.Owner, (int)(damagePerSecond * deltaTime));
                if (target.IsDead) { return ActionResult.Finished; }
                return ActionResult.Running;
            }
            return ActionResult.Finished;
        }

        public override void DrawGUI(IntelligenceState parentState, Intelligence intelligence, CallbackCollection callbacks)
        {
#if UNITY_EDITOR
            EditorGUILayout.BeginHorizontal();
            GUILayout.Label("Target var name:");
            entityVarName = EditorGUILayout.TextField(entityVarName);
            EditorGUILayout.EndHorizontal();
            damagePerSecond = EditorGUILayout.IntField(new GUIContent("Damage:", "per second"), damagePerSecond);
#endif
        }
    }
}