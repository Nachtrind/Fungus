using UnityEngine;

namespace ModularBehaviour
{
    [ActionUsage(UsageType.AsOneShot, UsageType.AsContinuous, UsageType.AsCondition)]
    public class DebugLog : AIAction
    {
        public string debugString = "Fired";
        public override ActionResult Fire(IntelligenceController controller)
        {
            Debug.Log(debugString);
            return ActionResult.Success;
        }

        public override ActionResult Run(IntelligenceController controller, float deltaTime)
        {
            return Fire(controller);
        }

        public override void DrawGUI(IntelligenceState parentState, Intelligence intelligence, CallbackCollection callbacks)
        {
            GUILayout.Label("Text:");
            debugString = GUILayout.TextField(debugString);
        }
    }
}