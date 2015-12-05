using UnityEngine;
using System.Collections.Generic;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class PatrolPath : MonoBehaviour
{
    public enum PatrolPointActions { Continue, Wait, PatrolRandomArea }

    public bool circularPath = true;

    [System.Serializable]
	public class PatrolPoint
    {
        public Vector3 position;
        public PatrolPointActions action;
        public float waitTime;
        public float areaRadius;
    }

    public List<PatrolPoint> points = new List<PatrolPoint>();
}

#if UNITY_EDITOR
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
                p.position = pp.transform.position+ Vector3.right;
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
        pp.circularPath = EditorGUILayout.Toggle("Loop: ", pp.circularPath);
        scrollPos = EditorGUILayout.BeginScrollView(scrollPos, GUILayout.Height(Mathf.Min(pp.points.Count * 50, 200)));
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
                case PatrolPath.PatrolPointActions.PatrolRandomArea:
                    pp.points[i].waitTime = EditorGUILayout.FloatField("RandomPatrolDuration:", pp.points[i].waitTime);
                    pp.points[i].areaRadius = EditorGUILayout.FloatField("RandomPatrolRadius:", pp.points[i].areaRadius);
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
        if (pp.circularPath && pp.points.Count > 1)
        {
            Handles.DrawLine(pp.points[pp.points.Count-1].position, pp.points[0].position);
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
#endif
