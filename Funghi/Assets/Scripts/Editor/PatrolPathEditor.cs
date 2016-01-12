using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(PatrolPath))]
public class PatrolPathEditor : Editor
{
    Vector2 scrollPos;
    public enum GizmoColor { Set, White, Blue, Red, Yellow, Green, Grey }
    public override void OnInspectorGUI()
    {
        PatrolPath pp = target as PatrolPath;
        //pp.transform.position = new Vector3(pp.transform.position.x, 0, pp.transform.position.z);
        GizmoColor gc = (GizmoColor)EditorGUILayout.EnumPopup("Gizmo color:", GizmoColor.Set);
        pp.drawCircle = EditorGUILayout.Toggle("Draw circle:", pp.drawCircle);
        if (gc > 0)
        {
            switch (gc)
            {
                case GizmoColor.White:
                    pp.gizmoDrawColor = Color.white;
                    break;
                case GizmoColor.Blue:
                    pp.gizmoDrawColor = Color.blue;
                    break;
                case GizmoColor.Red:
                    pp.gizmoDrawColor = Color.red;
                    break;
                case GizmoColor.Yellow:
                    pp.gizmoDrawColor = Color.yellow;
                    break;
                case GizmoColor.Green:
                    pp.gizmoDrawColor = Color.green;
                    break;
                case GizmoColor.Grey:
                    pp.gizmoDrawColor = Color.grey;
                    break;
            }
            SceneView.RepaintAll();
        }
        EditorGUILayout.BeginVertical(EditorStyles.textField);
        EditorGUILayout.BeginHorizontal();
        if (GUILayout.Button("Add Point", EditorStyles.miniButtonLeft))
        {
            var p = new PatrolPath.PatrolPoint();
            //if (pp.points.Count > 1)
            //{
            //    p.position = Vector3.Lerp(pp.points[pp.points.Count - 1].position, pp.points[0].position, 0.5f);
            //    scrollPos.y = float.PositiveInfinity;
            //}
            //else
            //{
                //p.position = pp.transform.position + Vector3.right;
            //}
            if (pp.points.Count > 0)
            {
                p.position = pp.points[pp.points.Count - 1].position + (Vector3.forward+Vector3.right) * 0.25f;
            }
            else
            {
                p.position = pp.transform.position + (Vector3.forward + Vector3.right) * 0.25f;
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
                    pp.points[i].actionProbability = EditorGUILayout.IntSlider(pp.points[i].actionProbability, 0, 100);
                    GUILayout.EndHorizontal();
                    break;
                case PatrolPath.PatrolPointActions.ExecuteFunction:
                    GUILayout.BeginHorizontal();
                    GUILayout.Label("Function Name:");
                    pp.points[i].functionName = EditorGUILayout.TextField(pp.points[i].functionName);
                    GUILayout.EndHorizontal();
                    pp.points[i].actionProbability = EditorGUILayout.IntSlider(pp.points[i].actionProbability, 0, 100);
                    pp.points[i].target = (PatrolPath.PatrolPoint.FunctionTarget)EditorGUILayout.EnumPopup("Function Target:", pp.points[i].target);
                    break;
            }
            EditorGUILayout.EndVertical();
            if (remove) { pp.points.RemoveAt(i); i -= 1; SceneView.RepaintAll(); }
        }
        EditorGUILayout.EndScrollView();
        EditorGUILayout.EndVertical();
    }

    GUIStyle labelStyle;
    public void OnSceneGUI()
    {
        PatrolPath pp = target as PatrolPath;
        if (labelStyle == null) { labelStyle = new GUIStyle(GUI.skin.label); labelStyle.normal.textColor = Color.white; }
        for (int i = pp.points.Count; i-- > 0;)
        {
            pp.points[i].position = Handles.PositionHandle(pp.points[i].position, Quaternion.identity);
            Handles.Label(pp.points[i].position, i.ToString(), labelStyle);
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