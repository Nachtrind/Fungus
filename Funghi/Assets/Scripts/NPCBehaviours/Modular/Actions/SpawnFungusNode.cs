using UnityEngine;

namespace ModularBehaviour
{
    public class SpawnFungusNode : OneShotAction
    {
        public enum PositionSource { Self, VarValue }
        public PositionSource posVarSource = PositionSource.Self;
        public string posVarName = "target";
        public bool saveAsVar = false;
        public string saveVarName = "SpawnedNode";
        public override ActionResult Fire(IntelligenceController controller)
        {
            Vector3 pos;
            if (posVarSource == PositionSource.Self)
            {
                pos = controller.Owner.transform.position;
            }
            else
            {
                if (posVarName.Length < 1) { return ActionResult.Failed; }
                if (!controller.GetMemoryValue(posVarName, out pos))
                {
                    Entity e;
                    if (!controller.GetMemoryValue(posVarName, out e))
                    {
                        return ActionResult.Failed;
                    }
                    pos = e.transform.position;
                }
            }
            FungusNode node = GameWorld.Instance.SpawnFungusNode(pos);
            if (saveAsVar)
            {
                if (saveVarName.Length > 0)
                {
                    controller.SetMemoryValue(saveVarName, node);
                    return ActionResult.Finished;
                }
                return ActionResult.Failed;
            }
            return ActionResult.Finished;
        }

        public override void DrawGUI(IntelligenceState parentState, Intelligence intelligence, CallbackCollection callbacks)
        {
#if UNITY_EDITOR
            posVarSource = (PositionSource)UnityEditor.EditorGUILayout.EnumPopup("Position source:", posVarSource);
            if (posVarSource == PositionSource.VarValue)
            {
                UnityEditor.EditorGUILayout.BeginHorizontal();
                GUILayout.Label("Postion var name:");
                posVarName = UnityEditor.EditorGUILayout.TextField(posVarName);
                UnityEditor.EditorGUILayout.EndHorizontal();
            }
            saveAsVar = UnityEditor.EditorGUILayout.Toggle("Save as var:", saveAsVar);
            if (saveAsVar)
            {
                UnityEditor.EditorGUILayout.BeginHorizontal();
                GUILayout.Label("Save var name:");
                saveVarName = UnityEditor.EditorGUILayout.TextField(saveVarName);
                UnityEditor.EditorGUILayout.EndHorizontal();
            }
#endif
        }
    }
}