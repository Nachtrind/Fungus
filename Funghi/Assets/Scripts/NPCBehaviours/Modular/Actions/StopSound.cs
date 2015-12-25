namespace ModularBehaviour
{
    [ActionUsage(UsageType.AsCondition, UsageType.AsOneShot)]
    public class StopSound : AIAction
    {
        public SoundSet.ClipType type;
        public override ActionResult Fire(IntelligenceController controller)
        {
            if (controller.Owner.StopSound(type))
            {
                return ActionResult.Success;
            }
            return ActionResult.Failed;
        }

        public override ActionResult Run(IntelligenceController controller, float deltaTime)
        {
            return Fire(controller);
        }

        public override void DrawGUI(IntelligenceState parentState, Intelligence intelligence, CallbackCollection callbacks)
        {
#if UNITY_EDITOR
            UnityEditor.EditorGUILayout.BeginHorizontal();
            UnityEngine.GUILayout.Label(new UnityEngine.GUIContent("Type:", "Only if this type is currently playing as continuous it will be stopped"));
            type = (SoundSet.ClipType)UnityEditor.EditorGUILayout.EnumPopup(type);
            UnityEditor.EditorGUILayout.EndHorizontal();
#endif
        }
    }
}