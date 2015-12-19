using UnityEngine;

namespace ModularBehaviour
{
    public class LogDebug : TriggerAction
    {
        public string txt = "Trigger log";
        public override ActionResult Fire(IntelligenceController controller, object value = null)
        {
            Debug.Log(txt);
            return ActionResult.Finished;
        }

        public override void DrawGUI(Intelligence intelligence, CallbackCollection callbacks)
        {
#if UNITY_EDITOR
            txt = UnityEditor.EditorGUILayout.TextField("Text:", txt);
#endif
        }
    }
}