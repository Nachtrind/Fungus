using System;
using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Tutorials;

[CustomEditor(typeof(Tutorial))]
public class TutorialEditor : Editor
{
    void OnEnable()
    {
        _cachedActionNames.Clear();
        _cachedActions.Clear();
        _cachedActions = Assembly.GetAssembly(typeof (Tutorial)).GetTypes().Where(type => type.IsSubclassOf(typeof (TutorialAction)) & !type.IsAbstract).ToList();
        _cachedActionNames.Add("Select");
        for (var i = 0; i < _cachedActions.Count; i++)
        {
            _cachedActionNames.Add(_cachedActions[i].Name);
        }
    }

    readonly List<string> _cachedActionNames = new List<string>();
    List<Type> _cachedActions = new List<Type>();

    readonly Dictionary<TutorialAction, Editor> _cachedEditors = new Dictionary<TutorialAction, Editor>();

    bool HasSameTypeActions(Tutorial tut, TutorialAction ac)
    {
        for (var i =0; i < tut.EventActions.Count; i++)
        {
            if (tut.EventActions[i].Event == ac.Event & tut.EventActions[i] != ac)
            {
                return true;
            }
        }
        return false;
    }

    bool cleanup = false;
    public override void OnInspectorGUI()
    {
        var tut = target as Tutorial;
        tut.UIAnchor = EditorGUILayout.ObjectField("UI Anchor", tut.UIAnchor, typeof (RectTransform), true) as RectTransform;
        var selected = EditorGUILayout.Popup("Actions", 0, _cachedActionNames.ToArray());
        cleanup = EditorGUILayout.Toggle("remove duplicate hooks", cleanup);
        if (selected > 0)
        {
            tut.EventActions.Add(CreateInstance(_cachedActions[selected-1]) as TutorialAction);
        }
        for (var i = tut.EventActions.Count; i-- > 0;)
        {
            var delete = false;
            if (!_cachedEditors.ContainsKey(tut.EventActions[i]))
            {
                var e = CreateEditor(tut.EventActions[i]);
                _cachedEditors.Add(tut.EventActions[i], e);
            }
            EditorGUILayout.BeginVertical(EditorStyles.helpBox);
            EditorGUILayout.BeginHorizontal();
            GUILayout.Label(tut.EventActions[i].GetType().Name);
            if (GUILayout.Button("x", EditorStyles.miniButton, GUILayout.Width(22)))
            {
                delete = true;
            }
            EditorGUILayout.EndHorizontal();
            _cachedEditors[tut.EventActions[i]].DrawDefaultInspector();
            EditorGUILayout.EndVertical();
            if (delete || (cleanup && HasSameTypeActions(tut, tut.EventActions[i])))
            {
                _cachedEditors.Remove(tut.EventActions[i]);
                DestroyImmediate(tut.EventActions[i]);
                tut.EventActions.RemoveAt(i);
            }
        }
    }
}
