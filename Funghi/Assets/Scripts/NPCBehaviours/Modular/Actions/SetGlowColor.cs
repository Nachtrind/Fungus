using UnityEngine;

namespace ModularBehaviour
{
    [ActionUsage(UsageType.AsCondition, UsageType.AsOneShot)]
    public class SetGlowColor : AIAction
    {
        public Color color;
        public override ActionResult Fire(IntelligenceController controller)
        {
            var h = controller.Owner as Human;
            if (!h) return ActionResult.Failed;
            h.SetGlowColor(color);
            return ActionResult.Success;
        }

        public override void DrawGUI(IntelligenceState parentState, Intelligence intelligence, CallbackCollection callbacks)
        {
#if UNITY_EDITOR
            color = UnityEditor.EditorGUILayout.ColorField("Color:", color);
#endif
        }
    }

}