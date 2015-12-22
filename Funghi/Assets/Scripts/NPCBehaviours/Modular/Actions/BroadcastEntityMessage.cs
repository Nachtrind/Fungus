using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace ModularBehaviour
{
    public class BroadcastEntityMessage : OneShotAction
    {
        public enum MessageTarget { Humans, Fungi, PoliceStations }
        public MessageTarget target = MessageTarget.Humans;
        public NotificationType type = NotificationType.FungusInSight;
        public string positionVariableName = "target";
        public float radius = 2f;
        public override ActionResult Fire(IntelligenceController controller)
        {
            Message m = new Message(controller.Owner, type, Vector3.zero);
            if (positionVariableName.Length > 0)
            {
                if (!controller.GetMemoryValue(positionVariableName, out m.position))
                {
                    Entity e = null;
                    if (!controller.GetMemoryValue(positionVariableName, out e))
                    {
                        return ActionResult.Failed;
                    }
                    m.position = e.transform.position;
                }
            }
            switch (target)
            {
                case MessageTarget.Humans:
                    GameWorld.Instance.BroadcastToHumans(m, controller.Owner.transform.position, radius);
                    break;
                case MessageTarget.Fungi:
                    GameWorld.Instance.BroadcastToNodes(m, controller.Owner.transform.position, radius);
                    break;
                case MessageTarget.PoliceStations:
                    GameWorld.Instance.BroadcastToPoliceStations(m, controller.Owner.transform.position, radius);
                    break;
            }
            return ActionResult.Finished;
        }

        public override void DrawGUI(IntelligenceState parentState, Intelligence intelligence, CallbackCollection callbacks)
        {
#if UNITY_EDITOR
            target = (MessageTarget)EditorGUILayout.EnumPopup("Target:",target);
            type = (NotificationType)EditorGUILayout.EnumPopup("Type:", type);
            EditorGUILayout.BeginHorizontal();
            GUILayout.Label("Position var:", GUILayout.Width(74));
            positionVariableName = EditorGUILayout.TextField(positionVariableName);
            EditorGUILayout.EndHorizontal();
            radius = EditorGUILayout.FloatField("Radius:", radius);
#endif
        }
    }
}