using UnityEngine;
using System.Collections;
using UnityEditor;

public class Menu
{

    const string BaseMenu = "Fungus";

    [MenuItem(BaseMenu + "/Create Spawner")]
    static void CreateEnemySpawner()
    {
        GameObject go = new GameObject("SpawnPoint");
        go.transform.position = GetPositionFromView();
        go.AddComponent<EntitySpawner>();
        Selection.activeGameObject = go;
    }

    static Vector3 GetPositionFromView()
    {
        Camera cam = EditorWindow.GetWindow<SceneView>().camera;
        if (!cam) { return Vector3.zero; }
        Vector3 p = cam.ViewportToWorldPoint(Vector2.one * 0.5f);
        p.y = 0;
        return p;
    }

    [MenuItem(BaseMenu + "/Create Path")]
    static void CreatePath()
    {
        GameObject go = new GameObject("Path");
        go.transform.position = GetPositionFromView();
        go.AddComponent<PatrolPath>();
        Selection.activeGameObject = go;
    }

    [MenuItem(BaseMenu + "/Create Entity Trigger")]
    static void CreateEntityTrigger()
    {
        GameObject go = new GameObject("EntityTrigger");
        go.transform.position = GetPositionFromView();
        go.AddComponent<EntityTrigger>();
        Selection.activeGameObject = go;
    }

    [MenuItem(BaseMenu + "/Create PoliceStation")]
    static void CreatePoliceStation()
    {
        GameObject go = new GameObject("PoliceStation");
        go.transform.position = GetPositionFromView();
        go.AddComponent<PoliceStation>();
        Selection.activeGameObject = go;
    }

    [MenuItem(BaseMenu + "/Settings/Open")]
    static void OpenSettings()
    {
        if (StandardGameSettings.Get)
        {
            Selection.activeObject = StandardGameSettings.Get;
        }
        else
        {
            Debug.LogWarning("Not found!");
        }
    }
}
