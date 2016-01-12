using System;
using UnityEngine;
using System.Collections.Generic;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace ModularBehaviour
{
    [ActionUsage(UsageType.AsContinuous, UsageType.AsOneShot, UsageType.AsCondition)]
    public class Conditional : AIAction
    {
        public AIAction ifAction;
        public AIAction thenAction;
        public AIAction elseAction;
        public override ActionResult Run(IntelligenceController controller, float deltaTime)
        {
            if (ifAction == null) { return ActionResult.Failed; }
            ActionResult res = ifAction.Run(controller, deltaTime);
            switch (res)
            {
                default:
                    return ActionResult.Running;
                case ActionResult.Success:
                    if (thenAction)
                    {
                        return thenAction.Run(controller, deltaTime);
                        //return ActionResult.Success;
                    }
                    return ActionResult.Failed;
                case ActionResult.Failed:
                    if (elseAction)
                    {
                        return elseAction.Run(controller, deltaTime);
                        //return ActionResult.Success;
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
                    if (thenAction)
                    {
                        return thenAction.Fire(controller);
                    }
                    return ActionResult.Failed;
                case ActionResult.Failed:
                    if (elseAction)
                    {
                        return elseAction.Fire(controller);
                    }
                    return ActionResult.Failed;
            }
        }

        public override void DrawGUI(IntelligenceState parentState, Intelligence intelligence, CallbackCollection callbacks)
        {
#if UNITY_EDITOR
            GUILayout.Label("If:");
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
            GUILayout.Label("Else:");
            if (elseAction == null)
            {
                Type res = callbacks.ContinuousActionPopup();
                if (res != null)
                {
                    elseAction = CreateInstance(res) as AIAction;
                    elseAction.name = res.Name;
                    callbacks.AddAsset(elseAction);
                }
            }
            else
            {
                EditorGUILayout.BeginVertical(EditorStyles.helpBox);
                EditorGUILayout.BeginHorizontal();
                GUILayout.Label(elseAction.name);
                if (GUILayout.Button("x", EditorStyles.miniButton, GUILayout.Width(20)))
                {
                    callbacks.RemoveAsset(elseAction);
                }
                EditorGUILayout.EndHorizontal();
                elseAction.DrawGUI(parentState, intelligence, callbacks);
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
            if (elseAction)
            {
                elseAction.OnDelete(callbacks);
                callbacks.RemoveAsset(elseAction);
            }
        }

        public override void DeepClone(List<Action<Func<IntelligenceState, IntelligenceState>>> stateCloneCallbacks)
        {
            if (ifAction)
            {
                ifAction = Instantiate(ifAction);
                ifAction.DeepClone(stateCloneCallbacks);
            }
            if (thenAction)
            {
                thenAction = Instantiate(thenAction);
                thenAction.DeepClone(stateCloneCallbacks);
            }
            if (elseAction)
            {
                elseAction = Instantiate(elseAction);
                elseAction.DeepClone(stateCloneCallbacks);
            }
        }
    }
}
