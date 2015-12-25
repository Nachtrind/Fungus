using System;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace ModularBehaviour
{
    [ActionUsage(UsageType.AsContinuous, UsageType.AsOneShot, UsageType.AsCondition)]
    public class Conditional : AIAction
    {
        public bool invert = false;
        public AIAction ifAction;
        public AIAction thenAction;
        public override ActionResult Run(IntelligenceController controller, float deltaTime)
        {
            if (ifAction == null || thenAction == null) { return ActionResult.Failed; }
            ActionResult res = ifAction.Run(controller, deltaTime);
            switch (res)
            {
                default:
                    return ActionResult.Running;
                case ActionResult.Success:
                    if (invert)
                    {
                        return ActionResult.Failed;
                    }
                    return thenAction.Run(controller, deltaTime);
                case ActionResult.Failed:
                    if (invert)
                    {
                        return thenAction.Run(controller, deltaTime);
                    }
                    return ActionResult.Failed;
            }
        }

        public override ActionResult Fire(IntelligenceController controller)
        {
            if (ifAction == null || thenAction == null) { return ActionResult.Failed; }
            ActionResult res = ifAction.Fire(controller);
            switch (res)
            {
                default:
                    return ActionResult.Running;
                case ActionResult.Success:
                    if (invert)
                    {
                        return ActionResult.Failed;
                    }
                    return thenAction.Fire(controller);
                case ActionResult.Failed:
                    if (invert)
                    {
                        return thenAction.Fire(controller);
                    }
                    return ActionResult.Failed;
            }
        }

        public override void DrawGUI(IntelligenceState parentState, Intelligence intelligence, CallbackCollection callbacks)
        {
#if UNITY_EDITOR
            invert = EditorGUILayout.Toggle("Invert:", invert);
            GUILayout.Label(string.Format("If{0}:", invert ? " not" : ""));
            if (ifAction == null)
            {
                Type res = callbacks.ConditionalActionPopup();
                if (res != null)
                {
                    
                    ifAction = CreateInstance(res) as AIAction;
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
                ifAction.DrawGUI(parentState, intelligence, callbacks);
                EditorGUILayout.EndVertical();
            }
            GUILayout.Label("Then:");
            if (thenAction == null)
            {
                Type res = callbacks.ContinuousActionPopup();
                if (res != null)
                {
                    thenAction = CreateInstance(res) as AIAction;
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
                thenAction.DrawGUI(parentState, intelligence, callbacks);
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
