using UnityEngine;
using UnityEditor;
using System.Collections;
using Tutorials;

[CustomEditor(typeof(Tutorial))]
public class TutorialEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
    }
}
