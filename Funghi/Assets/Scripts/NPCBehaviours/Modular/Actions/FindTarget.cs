using System;
using UnityEngine;
using Pathfinding;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace ModularBehaviour
{
    public class FindTarget : AIAction
    {
        public enum TargetType { Human, Fungus, PoliceStation }
        public TargetType Type = TargetType.Human;
        public float range = 2f;
        public bool checkLOS = true;
        public bool saveVar = true;
        public string storageVar = "target";
        public override ActionResult Run(IntelligenceController controller, float deltaTime)
        {
            Entity t = null;
            switch (Type)
            {
                case TargetType.Human:
                    t = GameWorld.Instance.GetNearestHuman(controller.Owner.transform.position);
                    break;
                case TargetType.Fungus:
                    t = GameWorld.Instance.GetNearestFungusNode(controller.Owner.transform.position);
                    break;
                case TargetType.PoliceStation:
                    t = GameWorld.Instance.GetNearestPoliceStation(controller.Owner.transform.position);
                    break;
            }
            
            if (t && Mathf.Sqrt(AstarMath.SqrMagnitudeXZ(controller.Owner.transform.position, t.transform.position)) <= range)
            {
                if (checkLOS && !GameWorld.Instance.HasLineOfSight(controller.Owner, t))
                {
                    return ActionResult.Running;
                }
                if (saveVar)
                {
                    controller.Storage.SetParameter(storageVar, t);
                }
                return ActionResult.Finished;
            }
            return ActionResult.Running;
        }

        public override void DrawGUI(IntelligenceState parentState, Intelligence intelligence, CallbackCollection callbacks)
        {
#if UNITY_EDITOR
            Type = (TargetType)EditorGUILayout.EnumPopup("Type:", Type);
            range = EditorGUILayout.FloatField("Range:", range);
            checkLOS = EditorGUILayout.Toggle("Check LOS:", checkLOS);
            saveVar = EditorGUILayout.Toggle("Save", saveVar);
            if (saveVar)
            {
                EditorGUILayout.BeginHorizontal();
                GUILayout.Label("Var name:");
                storageVar = EditorGUILayout.TextField(storageVar);
                EditorGUILayout.EndHorizontal();
            }
#endif
        }
    }
}