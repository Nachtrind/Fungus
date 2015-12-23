namespace ModularBehaviour
{
    public class PlaySound : OneShotAction
    {
        public SoundSet.ClipType type;
        public override ActionResult Fire(IntelligenceController controller)
        {
            if (controller.Owner.PlaySound(type))
            {
                return ActionResult.Finished;
            }
            return ActionResult.Failed;
        }

        public override void DrawGUI(IntelligenceState parentState, Intelligence intelligence, CallbackCollection callbacks)
        {
#if UNITY_EDITOR
            UnityEditor.EditorGUILayout.BeginHorizontal();
            UnityEngine.GUILayout.Label("Type:");
            type = (SoundSet.ClipType)UnityEditor.EditorGUILayout.EnumPopup(type);
            UnityEditor.EditorGUILayout.EndHorizontal();
#endif
        }
    }
}