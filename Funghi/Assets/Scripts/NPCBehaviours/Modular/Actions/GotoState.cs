using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace ModularBehaviour
{
    public class GotoState : AIAction
    {
        public IntelligenceState newState;
        public override ActionResult Run(IntelligenceController controller, float deltaTime)
        {
            if (newState)
            {
                controller.ChangeState(newState);
                return ActionResult.SkipNext;
            }
            return ActionResult.Failed;
        }

        public override void DrawGUI(IntelligenceState parentState, Intelligence intelligence, CallbackCollection callbacks)
        {
#if UNITY_EDITOR
            int currentState = 0;
            List<string> stateNames = new List<string>();
            stateNames.Add("Select");
            for (int i = 0; i < intelligence.states.Count; i++)
            {
                string newStateName = intelligence.states[i].name;
                if (newState != null)
                {
                    if (newState.name.Equals(newStateName, System.StringComparison.OrdinalIgnoreCase))
                    {
                        currentState = i + 1;
                    }
                }
                stateNames.Add(newStateName);
            }
            EditorGUILayout.BeginHorizontal();
            GUILayout.Label("New State: ");
            int selectedState = EditorGUILayout.Popup(currentState, stateNames.ToArray());
            EditorGUILayout.EndHorizontal();
            if (selectedState > 0)
            {
                if (!parentState.name.Equals(intelligence.states[selectedState - 1].name))
                {
                    newState = intelligence.states[selectedState - 1];
                }
                else
                {
                    Debug.LogWarning("transitioning to self is useless.");
                }
            }
#endif
        }
    }
}