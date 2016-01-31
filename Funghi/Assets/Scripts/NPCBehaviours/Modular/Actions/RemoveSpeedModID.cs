namespace ModularBehaviour
{
    [ActionUsage(UsageType.AsCondition, UsageType.AsOneShot)]
    public class RemoveSpeedModID : AIAction
    {
        public string varName = "SpeedMod";
        public override ActionResult Fire(IntelligenceController controller)
        {
            int id;
            if (controller.GetMemoryValue(varName, out id))
            {
                controller.Owner.RevokeSpeedMod(id);
                return ActionResult.Success;
            }
            return ActionResult.Failed;
        }

        public override void DrawGUI(IntelligenceState parentState, Intelligence intelligence, CallbackCollection callbacks)
        {
#if UNITY_EDITOR
            varName = UnityEditor.EditorGUILayout.TextField("var:", varName);
#endif
        }
    }
}