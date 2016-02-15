using System;
using UnityEngine;

namespace ModularBehaviour{
	[ActionUsage(UsageType.AsCondition, UsageType.AsOneShot)]
	public class DebugVariable: AIAction
	{
		public string varName = "target";
		public override ActionResult Fire (IntelligenceController controller)
		{
			object var;
			if (controller.GetMemoryValue (varName, out var)) {
				Debug.Log (var.ToString ());
			} 
			return ActionResult.Success;
		}

		public override void DrawGUI (IntelligenceState parentState, Intelligence intelligence, CallbackCollection callbacks)
		{
			#if UNITY_EDITOR
			varName = UnityEditor.EditorGUILayout.TextField("var name", varName);
			#endif
		}
	}
}
