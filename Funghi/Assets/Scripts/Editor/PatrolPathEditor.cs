using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(PatrolPath))]
public class PatrolPathEditor : Editor
{
    Vector2 scrollPos;
    public override void OnInspectorGUI()
    {
        PatrolPath pp = target as PatrolPath;
        EditorGUILayout.BeginVertical(EditorStyles.textField);
        EditorGUILayout.BeginHorizontal();
        if (GUILayout.Button("Add Point", EditorStyles.miniButtonLeft))
        {
            var p = new PatrolPath.PatrolPoint();
            if (pp.points.Count > 0)
            {
                p.position = pp.points[pp.points.Count - 1].position + Vector3.right;
                scrollPos.y = float.PositiveInfinity;
            }
            else
            {
                p.position = pp.transform.position + Vector3.right;
            }
            pp.points.Add(p);
            SceneView.RepaintAll();
        }
        if (GUILayout.Button("Clear all", EditorStyles.miniButtonRight))
        {
            pp.points.Clear();
            SceneView.RepaintAll();
        }
        EditorGUILayout.EndHorizontal();
        scrollPos = EditorGUILayout.BeginScrollView(scrollPos, GUILayout.Height(Mathf.Min(pp.points.Count * 50, 300)));
        for (int i = 0; i < pp.points.Count; i++)
        {
            bool remove = false;
            EditorGUILayout.BeginVertical(EditorStyles.helpBox);
            EditorGUILayout.BeginHorizontal();
            GUILayout.Label(i.ToString(), EditorStyles.helpBox);
            if (GUILayout.Button("x", GUILayout.Width(20))) { remove = true; }
            EditorGUILayout.EndHorizontal();
            pp.points[i].action = (PatrolPath.PatrolPointActions)EditorGUILayout.EnumPopup("Action:", pp.points[i].action);
            switch (pp.points[i].action)
            {
                case PatrolPath.PatrolPointActions.Continue:
                    break;
                case PatrolPath.PatrolPointActions.Wait:
                    pp.points[i].waitTime = EditorGUILayout.FloatField("WaitTime:", pp.points[i].waitTime);
                    break;
                case PatrolPath.PatrolPointActions.ChangePath:
                    pp.points[i].linkedPath = EditorGUILayout.ObjectField("New Path:", pp.points[i].linkedPath, typeof(PatrolPath), true) as PatrolPath;
                    GUILayout.BeginHorizontal();
                    GUILayout.Label("Probability:");
                    pp.points[i].changeLikelyness = EditorGUILayout.IntSlider(pp.points[i].changeLikelyness, 0, 100);
                    GUILayout.EndHorizontal();
                    break;
                case PatrolPath.PatrolPointActions.ExecuteFunction:
                    GUILayout.BeginHorizontal();
                    GUILayout.Label("Function Name:");
                    pp.points[i].functionName = EditorGUILayout.TextField(pp.points[i].functionName);
                    GUILayout.EndHorizontal();
                    pp.points[i].target = (PatrolPath.PatrolPoint.FunctionTarget)EditorGUILayout.EnumPopup("Function Target:", pp.points[i].target);
                    break;
            }
            EditorGUILayout.EndVertical();
            if (remove) { pp.points.RemoveAt(i); i -= 1; }
        }
        EditorGUILayout.EndScrollView();
        EditorGUILayout.EndVertical();
    }

    public void OnSceneGUI()
    {
        PatrolPath pp = target as PatrolPath;
        for (int i = pp.points.Count; i-- > 0;)
        {
            pp.points[i].position = Handles.PositionHandle(pp.points[i].position, Quaternion.identity);
            Handles.color = Color.white;
            Handles.Label(pp.points[i].position, i.ToString());
            Handles.color = Color.yellow;
            if (i > 0)
            {
                Handles.DrawLine(pp.points[i].position, pp.points[i - 1].position);
            }
        }
        if (pp.points.Count > 2)
        {
            Handles.DrawLine(pp.points[pp.points.Count - 1].position, pp.points[0].position);
        }
        if (pp.transform.hasChanged)
        {
            MoveNodesOnTransformChange(pp);
            pp.transform.hasChanged = false;
        }
    }

    Vector3 lastPosition = Vector3.zero;
    void MoveNodesOnTransformChange(PatrolPath pp)
    {
        Vector3 delta = pp.transform.position - lastPosition;
        for (int i = 0; i < pp.points.Count; i++)
        {
            pp.points[i].position = pp.points[i].position + delta;
        }
        lastPosition = pp.transform.position;
    }

    void OnEnable()
    {
        //Tools.hidden = true;
        PatrolPath pp = target as PatrolPath;
        lastPosition = pp.transform.position;
    }

    void OnDisable()
    {
        //Tools.hidden = false;
    }
}