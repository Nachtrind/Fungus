namespace ModularBehaviour
{
    public class RunPatrolPath : AIAction
    {
        public bool ignorePathConfiguration = false;
        float currentWaitTime = 0;
        public override ActionResult Run(IntelligenceController controller, float deltaTime)
        {
            if (currentWaitTime > 0) { currentWaitTime = UnityEngine.Mathf.Clamp(currentWaitTime - deltaTime, 0, currentWaitTime); return ActionResult.Running; }
            PatrolPath path = null;
            if (controller.Storage.TryGetParameter(Intelligence.PathIdentifier, out path))
            {
                int currentIndex = 0;
                if (!controller.Storage.TryGetParameter(Intelligence.PathIndexIdentifier, out currentIndex))
                {
                    controller.Storage.SetParameter(Intelligence.PathIndexIdentifier, 0);
                }
                PatrolPath.PatrolPoint currentPoint = path.points[currentIndex];
                Entity.MoveResult result = controller.Owner.MoveTo(currentPoint.position);
                if (result == Entity.MoveResult.ReachedTarget)
                {
                    switch (currentPoint.action)
                    {
                        case PatrolPath.PatrolPointActions.Wait:
                            currentWaitTime = currentPoint.waitTime;
                            break;
                        case PatrolPath.PatrolPointActions.ChangePath:
                            PatrolPath linkedPath = currentPoint.linkedPath;
                            if (linkedPath != null)
                            {
                                controller.LoadPath(linkedPath);
                                controller.Storage.SetParameter(Intelligence.PathIndexIdentifier, linkedPath.GetNearestPatrolPointIndex(controller.Owner.transform.position));
                                return ActionResult.Running;
                            }
                            break;
                        case PatrolPath.PatrolPointActions.ExecuteFunction:
                            switch (currentPoint.target)
                            {
                                case PatrolPath.PatrolPoint.FunctionTarget.NPC:
                                    controller.Owner.BroadcastMessage(currentPoint.functionName, UnityEngine.SendMessageOptions.RequireReceiver);
                                    break;
                                case PatrolPath.PatrolPoint.FunctionTarget.PatrolPath:
                                    path.BroadcastMessage(currentPoint.functionName, UnityEngine.SendMessageOptions.RequireReceiver);
                                    break;
                            }
                            break;
                    }
                    currentIndex++;
                    if (currentIndex >= path.points.Count)
                    {
                        currentIndex = 0;
                    }
                    controller.Storage.SetParameter(Intelligence.PathIndexIdentifier, currentIndex);
                }
                return ActionResult.Running;
            }
            return ActionResult.Failed;
        }

        public override void DrawGUI(IntelligenceState parentState, Intelligence intelligence, CallbackCollection callbacks)
        {
#if UNITY_EDITOR
            ignorePathConfiguration = UnityEditor.EditorGUILayout.Toggle("Ignore point-settings", ignorePathConfiguration);
                #endif
        }
    }
}