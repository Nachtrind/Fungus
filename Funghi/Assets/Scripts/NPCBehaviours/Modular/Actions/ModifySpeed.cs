namespace ModularBehaviour
{
    [ActionUsage(UsageType.AsCondition, UsageType.AsOneShot, UsageType.AsContinuous)]
    public class ModifySpeed : AIAction
    {
        public EntityMover.SpeedModType type = EntityMover.SpeedModType.NoSpeed;
        public float timeout = -1f;
        public bool saveAsVar = false;
        public string varName = "SpeedMod";
        public override ActionResult Fire(IntelligenceController controller)
        {
            var modID = controller.Owner.ApplySpeedMod(type, controller.Owner, timeout);
            if (saveAsVar)
            {
                controller.SetMemoryValue(varName, modID);
            }
            return ActionResult.Success;
        }

        public override ActionResult Run(IntelligenceController controller, float deltaTime)
        {
            return Fire(controller);
        }

        public override void DrawGUI(IntelligenceState parentState, Intelligence intelligence, CallbackCollection callbacks)
        {
#if UNITY_EDITOR
            type = (EntityMover.SpeedModType)UnityEditor.EditorGUILayout.EnumPopup("Type", type);
            timeout = UnityEditor.EditorGUILayout.FloatField("Timeout:", timeout);
            saveAsVar = UnityEditor.EditorGUILayout.Toggle("Save as var", saveAsVar);
            if (saveAsVar)
            {
                varName = UnityEditor.EditorGUILayout.TextField("var:", varName);
            }
#endif
        }
    }
}