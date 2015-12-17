using UnityEngine;

namespace ModularBehaviour
{
    public class DebugLog : OneShotAction
    {
        public string debugString = "Fired";
        public override ActionResult Fire(IntelligenceController controller)
        {
            Debug.Log(debugString);
            return ActionResult.Finished;
        }

        public override void DrawGUI(IntelligenceState parentState, Intelligence intelligence, CallbackCollection callbacks)
        {
            GUILayout.Label("Text:");
            debugString = GUILayout.TextField(debugString);
        }
    }
}