namespace ModularBehaviour
{
    [ActionUsage(UsageType.AsCondition, UsageType.AsOneShot, UsageType.AsContinuous)]
    public class ChangeBehaviour : AIAction
    {
        public Intelligence newBehaviour;
        public override ActionResult Fire(IntelligenceController controller)
        {

            if (newBehaviour && controller.Owner.SetBehaviour(newBehaviour))
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
            newBehaviour = UnityEditor.EditorGUILayout.ObjectField("Behaviour:", newBehaviour, typeof(Intelligence), false) as Intelligence;
#endif
        }
    }
}