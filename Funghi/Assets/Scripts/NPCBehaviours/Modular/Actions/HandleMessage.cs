using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace ModularBehaviour
{
    public class HandleMessage : TriggerAction
    {
        public NotificationType handledType = NotificationType.PoliceAlarm;
        public bool savePosition = true;
        public string varName = "AlarmPosition";
        public bool saveSender = false;
        public string senderVarName = "AlarmSender";
        public TriggerAction action;
        public override ActionResult Fire(IntelligenceController controller, object value = null)
        {
            Message m = value as Message;
            if (m == null) { return ActionResult.Failed; }
            if (m.type != handledType) { return ActionResult.Failed; }
            if (savePosition)
            {
                controller.SetMemoryValue(varName, m.position);
            }
            if (saveSender)
            {
                controller.SetMemoryValue(senderVarName, m.sender);
            }
            if (action)
            {
                action.Fire(controller, value);
            }
            return ActionResult.Success;
        }

        public override void DrawGUI(Intelligence intelligence, CallbackCollection callbacks)
        {
#if UNITY_EDITOR
            handledType = (NotificationType)EditorGUILayout.EnumPopup("Type:", handledType);
            savePosition = EditorGUILayout.Toggle("Save position", savePosition);
            if (savePosition)
            {
                EditorGUILayout.BeginHorizontal();
                GUILayout.Label("var name:", GUILayout.Width(60));
                varName = EditorGUILayout.TextField(varName);
                EditorGUILayout.EndHorizontal();
            }
            saveSender = EditorGUILayout.Toggle("Save sender", saveSender);
            if (saveSender)
            {
                EditorGUILayout.BeginHorizontal();
                GUILayout.Label("var name:", GUILayout.Width(80));
                senderVarName = EditorGUILayout.TextField(senderVarName);
                EditorGUILayout.EndHorizontal();
            }
            GUILayout.Label("Then:");
            if (action == null)
            {
                System.Type res = callbacks.TriggerActionPopup();
                if (res != null)
                {
                    action = CreateInstance(res) as TriggerAction;
                    action.name = res.Name;
                    callbacks.AddAsset(action);
                }
            }
            else
            {
                if (action is HandleMessage) { callbacks.RemoveAsset(action); return; }
                EditorGUILayout.BeginVertical(EditorStyles.helpBox);
                EditorGUILayout.BeginHorizontal();
                GUILayout.Label(action.name);
                if (GUILayout.Button("x", EditorStyles.miniButton, GUILayout.Width(20)))
                {
                    callbacks.RemoveAsset(action);
                }
                EditorGUILayout.EndHorizontal();
                action.DrawGUI(intelligence, callbacks);
                EditorGUILayout.EndVertical();
            }
#endif
        }

        public override void OnDelete(CallbackCollection callbacks)
        {
            if (action) { action.OnDelete(callbacks); callbacks.RemoveAsset(action); }
        }
    }
}