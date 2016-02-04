using UnityEngine;

namespace ModularBehaviour
{
    public class ChangeGlowColor : TriggerAction
    {
        public Color color;
        public override ActionResult Fire(IntelligenceController controller, object value = null)
        {
            var h = controller.Owner as Human;
            if (!h) return ActionResult.Failed;
            h.SetGlowColor(color);
            return ActionResult.Success;
        }

        public override void DrawGUI(Intelligence intelligence, CallbackCollection callbacks)
        {
#if UNITY_EDITOR
            color = UnityEditor.EditorGUILayout.ColorField("Color:", color);
#endif
        }
    }
}