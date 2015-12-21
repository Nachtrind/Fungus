using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace ModularBehaviour
{
    public class WanderAround : AIAction
    {
        public string centerPositionVar = "Node";
        public float minRadius = 0.25f;
        public float maxRadius = 0.5f;
        public int waitChance = 50;
        public float randomWaitTimeMin = 0f;
        public float randomWaitTimeMax = 0.5f;

        Vector3 targetPos;
        float currentWait = 0;
        public override ActionResult Run(IntelligenceController controller, float deltaTime)
        {
            if (currentWait > 0) { currentWait = Mathf.Clamp(currentWait - deltaTime, 0, currentWait); return ActionResult.Running; }
            Vector3 centerPosVar;
            if (!controller.Storage.TryGetParameter(centerPositionVar, out centerPosVar))
            {
                Entity e;
                if (!controller.Storage.TryGetParameter(centerPositionVar, out e))
                {
                    return ActionResult.Failed;
                }
                centerPosVar = e.transform.position;
            }
            Entity.MoveResult res = controller.Owner.MoveTo(centerPosVar + targetPos);
            if (res == Entity.MoveResult.ReachedTarget || res == Entity.MoveResult.TargetNotReachable)
            {
                Vector2 rndPos = Random.insideUnitCircle;
                targetPos = new Vector3(Mathf.Clamp(rndPos.x, minRadius, maxRadius), 0, Mathf.Clamp(rndPos.y, minRadius, maxRadius));
                if (Random.Range(0, 100) > waitChance)
                {
                    currentWait = Random.Range(randomWaitTimeMin, randomWaitTimeMax);
                }
            }
            return ActionResult.Running;
        }

        public override void DrawGUI(IntelligenceState parentState, Intelligence intelligence, CallbackCollection callbacks)
        {
#if UNITY_EDITOR
            centerPositionVar = EditorGUILayout.TextField("Center var name", centerPositionVar);
            EditorGUILayout.BeginHorizontal();
            GUILayout.Label("Radius:", GUILayout.ExpandWidth(false));
            EditorGUILayout.MinMaxSlider(ref minRadius, ref maxRadius, 0f, 2f);
            minRadius = Mathf.Round(minRadius * 10) * 0.1f;
            maxRadius = Mathf.Round(maxRadius * 10) * 0.1f;
            EditorGUILayout.EndHorizontal();
            GUILayout.Label(string.Format("{0} - {1} units distance to center", minRadius, maxRadius));
            waitChance = EditorGUILayout.IntField("Wait probability:", waitChance);
            if (waitChance > 0)
            {
                EditorGUILayout.BeginHorizontal();
                GUILayout.Label("Waiting:", GUILayout.ExpandWidth(false));
                EditorGUILayout.MinMaxSlider(ref randomWaitTimeMin, ref randomWaitTimeMax, 0, 5f);
                randomWaitTimeMin = Mathf.Round(randomWaitTimeMin * 10) * 0.1f;
                randomWaitTimeMax = Mathf.Round(randomWaitTimeMax * 10) * 0.1f;
                EditorGUILayout.EndHorizontal();
                GUILayout.Label(string.Format("{0} - {1} seconds", randomWaitTimeMin, randomWaitTimeMax));
            }
#endif
        }
    }
}