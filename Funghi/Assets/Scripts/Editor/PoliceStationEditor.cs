using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(PoliceStation))]
public class PoliceStationEditor: Editor
{
    public override void OnInspectorGUI()
    {
        EditorGUILayout.HelpBox("Releases police units when notified by citizens or other officers. Spawner controls release.\nThis represents the point where citizens run to.", MessageType.Info);
        if (Application.isPlaying)
        {
            if (GUILayout.Button("Test"))
            {
                FungusNode nearest = GameWorld.Instance.GetNearestFungusNode((target as PoliceStation).transform.position);
                if (nearest)
                {
                    (target as PoliceStation).ReceiveBroadcast(new Message(null, NotificationType.PoliceAlarm, nearest.transform.position));
                }
            }
        }
    }
}
