using ModularBehaviour;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(Intelligence))]
public class IntelligenceEditor: Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        if (GUILayout.Button("Edit"))
        {
            IntelligenceEditorWindow iew = EditorWindow.GetWindow<IntelligenceEditorWindow>();
            iew.target = target as Intelligence;
        }
    }
}
