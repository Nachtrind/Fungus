using UnityEngine;

namespace ModularBehaviour
{
    [ActionUsage(UsageType.AsOneShot, UsageType.AsContinuous, UsageType.AsCondition)]
    public class PlaySound : AIAction
    {
        public SoundSet.ClipType type;
        public float interval = 0.5f;
        public override ActionResult Fire(IntelligenceController controller)
        {
            if (controller.Owner.PlaySound(type))
            {
                return ActionResult.Success;
            }
            return ActionResult.Failed;
        }

        float lastRun = 0;
        public override ActionResult Run(IntelligenceController controller, float deltaTime)
        {
            if (Time.time-lastRun >= interval)
            {
                lastRun = Time.time;
                if (controller.Owner.PlaySound(type))
                {
                    return ActionResult.Success;
                }
                return ActionResult.Failed;
            }
            return ActionResult.Running;
        }

        public override void DrawGUI(IntelligenceState parentState, Intelligence intelligence, CallbackCollection callbacks)
        {
#if UNITY_EDITOR
            UnityEditor.EditorGUILayout.BeginHorizontal();
            GUILayout.Label("Type:");
            type = (SoundSet.ClipType)UnityEditor.EditorGUILayout.EnumPopup(type);
            UnityEditor.EditorGUILayout.EndHorizontal();
            UnityEditor.EditorGUILayout.BeginHorizontal();
            GUILayout.Label("Interval (if continuous):");
            interval = UnityEditor.EditorGUILayout.FloatField(interval);
            UnityEditor.EditorGUILayout.EndHorizontal();
#endif
        }
    }
}