using UnityEngine;
using System.Collections;
using UnityEditor;

public class Menu
{

    const string BaseMenu = "Fungus";

    [MenuItem(BaseMenu+"/Create Enemy Spawner")]
    static void CreateEnemySpawner()
    {
        GameObject go = new GameObject("SpawnPoint (Enemy)");
        go.transform.position = GetPositionFromView();
        go.AddComponent<EnemySpawner>();
    }

    static Vector3 GetPositionFromView()
    {
        Camera cam = EditorWindow.GetWindow<SceneView>().camera;
        if (!cam) { return Vector3.zero; }
        Vector3 p = cam.ViewportToWorldPoint(Vector2.one * 0.5f);
        p.y = 0;
        return p;
    }

    [MenuItem(BaseMenu+"/Create Path")]
    static void CreatePath()
    {
        GameObject go = new GameObject("Path");
        go.transform.position = GetPositionFromView();
        go.AddComponent<PatrolPath>();
    }

    [MenuItem(BaseMenu+"/Create Enemy World Exit")]
    static void CreateEnemyExit()
    {
        GameObject go = new GameObject("EnemyExit");
        go.transform.position = GetPositionFromView();
        go.AddComponent<EnemyExitWorldTrigger>();
    }
}
