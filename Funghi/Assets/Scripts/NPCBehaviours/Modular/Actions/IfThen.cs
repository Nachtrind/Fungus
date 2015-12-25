using System;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace ModularBehaviour
{
    public class IfThen : TriggerAction
    {
        public TriggerAction ifAction;
        public TriggerAction thenAction;
        public bool invert = false;
        public override ActionResult Fire(IntelligenceController controller, object value = null)
        {
            if (ifAction == null || thenAction == null) { return ActionResult.Failed; }
            ActionResult res = ifAction.Fire(controller, value);
            switch (res)
            {
                default:
                    return ActionResult.Running;
                case ActionResult.Success:
                    if (invert)
                    {
                        return ActionResult.Failed;
                    }
                    return thenAction.Fire(controller, value);
                case ActionResult.Failed:
                    if (invert)
                    {
                        return thenAction.Fire(controller, value);
                    }
                    return ActionResult.Failed;
            }
        }

        public override void DrawGUI(Intelligence intelligence, CallbackCollection callbacks)
        {
#if UNITY_EDITOR
            invert = EditorGUILayout.Toggle("Invert:", invert);
            GUILayout.Label(string.Format("If{0}:", invert ? " not" : ""));
            if (ifAction == null)
            {
                Type res = callbacks.TriggerActionPopup();
                if (res != null)
                {
                    
                    ifAction = CreateInstance(res) as TriggerAction;
                    ifAction.name = res.Name;
                    callbacks.AddAsset(ifAction);
                }
            }
            else
            {
                EditorGUILayout.BeginVertical(EditorStyles.helpBox);
                EditorGUILayout.BeginHorizontal();
                GUILayout.Label(ifAction.name);
                if (GUILayout.Button("x", EditorStyles.miniButton, GUILayout.Width(20)))
                {
                    callbacks.RemoveAsset(ifAction);
                }
                EditorGUILayout.EndHorizontal();
                ifAction.DrawGUI(intelligence, callbacks);
                EditorGUILayout.EndVertical();
            }
            GUILayout.Label("Then:");
            if (thenAction == null)
            {
                Type res = callbacks.TriggerActionPopup();
                if (res != null)
                {
                    thenAction = CreateInstance(res) as TriggerAction;
                    thenAction.name = res.Name;
                    callbacks.AddAsset(thenAction);
                }
            }
            else
            {
                EditorGUILayout.BeginVertical(EditorStyles.helpBox);
                EditorGUILayout.BeginHorizontal();
                GUILayout.Label(thenAction.name);
                if (GUILayout.Button("x", EditorStyles.miniButton, GUILayout.Width(20)))
                {
                    callbacks.RemoveAsset(thenAction);
                }
                EditorGUILayout.EndHorizontal();
                thenAction.DrawGUI(intelligence, callbacks);
                EditorGUILayout.EndVertical();
            }
#endif
            }

        public override void OnDelete(CallbackCollection callbacks)
        {
            if (ifAction != null)
            {
                ifAction.OnDelete(callbacks);
                callbacks.RemoveAsset(ifAction);
            }
            if (thenAction != null)
            {
                thenAction.OnDelete(callbacks);
                callbacks.RemoveAsset(thenAction);
            }
        }
    }
}
