using System;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace ModularBehaviour
{

    [ActionUsage(UsageType.AsContinuous, UsageType.AsCondition)]
    public class Attack : AIAction
    {
        public string entityVarName = "target";
        public float interval = 0.5f;
        public int damagePerAttack = 20;

        float lastAttack = 0;
        public override ActionResult Run(IntelligenceController controller, float deltaTime)
        {
            if (Time.time - lastAttack > interval)
            {
                Entity target;
                if (controller.GetMemoryValue(entityVarName, out target))
                {
                    if (!target.isAttackable) { return ActionResult.Failed; }
                    target.Damage(controller.Owner, (int)(damagePerAttack * deltaTime));
                    controller.Owner.TriggerAnimator("Attack");
                    return ActionResult.Success;
                }
                lastAttack = Time.time;
            }
            return ActionResult.Running;
        }

        public override void DrawGUI(IntelligenceState parentState, Intelligence intelligence, CallbackCollection callbacks)
        {
#if UNITY_EDITOR
            EditorGUILayout.BeginHorizontal();
            GUILayout.Label("Target var name:");
            entityVarName = EditorGUILayout.TextField(entityVarName);
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.BeginHorizontal();
            GUILayout.Label("Interval:");
            interval = EditorGUILayout.Slider(interval, 0.1f, 2f);
            EditorGUILayout.EndHorizontal();
            damagePerAttack = EditorGUILayout.IntField(new GUIContent("Damage:", "per interval"), damagePerAttack);
#endif
        }
    }
}