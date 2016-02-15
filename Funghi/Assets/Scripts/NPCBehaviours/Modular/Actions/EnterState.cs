using UnityEngine;
using System.Collections.Generic;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace ModularBehaviour
{
	public class EnterState : TriggerAction
	{
		public IntelligenceState newState;

		public override ActionResult Fire (IntelligenceController controller, object value = null)
		{
			if (newState) {
				if (controller.IsActiveState (newState.name)) {
					return ActionResult.Success;
				}
				controller.ChangeState (newState);
				return ActionResult.Success;
			}
			return ActionResult.Failed;
		}

		public override void DrawGUI (Intelligence intelligence, CallbackCollection callbacks)
		{
#if UNITY_EDITOR
			int currentState = 0;
			List<string> stateNames = new List<string> ();
			stateNames.Add ("Select");
			for (int i = 0; i < intelligence.states.Count; i++) {
				string newStateName = intelligence.states [i].name;
				if (newState != null) {
					if (newState.name.Equals (newStateName, System.StringComparison.OrdinalIgnoreCase)) {
						currentState = i + 1;
					}
				}
				stateNames.Add (newStateName);
			}
			EditorGUILayout.BeginHorizontal ();
			GUILayout.Label ("New State: ");
			int selectedState = EditorGUILayout.Popup (currentState, stateNames.ToArray ());
			EditorGUILayout.EndHorizontal ();
			if (selectedState > 0) {
				newState = intelligence.states [selectedState - 1];
			}
#endif
		}

		void SetState (System.Func<IntelligenceState, IntelligenceState> GetState)
		{
			newState = GetState (newState);
		}

		public override void DeepClone (List<System.Action<System.Func<IntelligenceState, IntelligenceState>>> stateCloneCallbacks)
		{
			stateCloneCallbacks.Add (SetState);
		}
	}

}