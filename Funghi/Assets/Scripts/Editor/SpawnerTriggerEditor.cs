using UnityEngine;
using System.Collections;
using UnityEditor;

[CustomEditor(typeof(SpawnerTriggerCollection))]
public class SpawnerTriggerEditor : Editor
{
    void OnSceneGUI()
    {
        SpawnerTriggerCollection spc = target as SpawnerTriggerCollection;
        for (int i = 0; i < spc.triggers.Count; i++)
        {
            spc.triggers[i].position = Handles.PositionHandle(spc.triggers[i].position, Quaternion.identity);
        }
    }
}
