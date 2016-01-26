namespace ModularBehaviour
{
    [ActionUsage(UsageType.AsCondition, UsageType.AsOneShot)]
    public class TriggerAnimator : AIAction
    {
        public string triggerID = "";
        public override ActionResult Fire(IntelligenceController controller)
        {
            controller.Owner.TriggerAnimator(triggerID);
            return ActionResult.Success;
        }

        public override ActionResult Run(IntelligenceController controller, float deltaTime)
        {
            return Fire(controller);
        }

        public override void DrawGUI(IntelligenceState parentState, Intelligence intelligence, CallbackCollection callbacks)
        {
#if UNITY_EDITOR
            triggerID = UnityEditor.EditorGUILayout.TextField("TriggerID:", triggerID);
#endif
        }
    }
}