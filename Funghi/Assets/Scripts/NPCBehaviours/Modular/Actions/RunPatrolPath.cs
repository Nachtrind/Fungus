namespace ModularBehaviour
{
    [ActionUsage(UsageType.AsContinuous, UsageType.AsCondition)]
    public class RunPatrolPath : AIAction
    {
        public bool ignorePathConfiguration = false;
        float currentWaitTime = 0;

        bool repeated = false; //minimize progress delay

        public override ActionResult Run(IntelligenceController controller, float deltaTime)
        {
            repeated = false;
        REPEAT:
            if (currentWaitTime > 0) { currentWaitTime = UnityEngine.Mathf.Clamp(currentWaitTime - deltaTime, 0, currentWaitTime); return ActionResult.Running; }
            PatrolPath path = null;
            if (controller.GetMemoryValue(Intelligence.PathIdentifier, out path))
            {
                int currentIndex = 0;
                if (!controller.GetMemoryValue(Intelligence.PathIndexIdentifier, out currentIndex))
                {
                    controller.SetMemoryValue(Intelligence.PathIndexIdentifier, 0);
                }
                PatrolPath.PatrolPoint currentPoint = path.points[currentIndex];
                EntityMover.MoveResult result = controller.Owner.MoveTo(currentPoint.position);
                if (result == EntityMover.MoveResult.ReachedTarget)
                {
                    if (!ignorePathConfiguration)
                    {
                        switch (currentPoint.action)
                        {
                            case PatrolPath.PatrolPointActions.Wait:
                                currentWaitTime = currentPoint.waitTime;
                                break;
                            case PatrolPath.PatrolPointActions.ChangePath:
                                if (UnityEngine.Random.Range(0, 100) > currentPoint.actionProbability) { break; }
                                PatrolPath linkedPath = currentPoint.linkedPath;
                                if (linkedPath != null)
                                {
                                    controller.LoadPath(linkedPath);
                                    controller.SetMemoryValue(Intelligence.PathIndexIdentifier, linkedPath.GetNearestPatrolPointIndex(controller.Owner.transform.position));
                                    return ActionResult.Running;
                                }
                                break;
                            case PatrolPath.PatrolPointActions.ExecuteFunction:
                                if (UnityEngine.Random.Range(0, 100) > currentPoint.actionProbability) { break; }
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
                    }
                    currentIndex++;
                    if (currentIndex >= path.points.Count)
                    {
                        currentIndex = 0;
                    }
                    controller.SetMemoryValue(Intelligence.PathIndexIdentifier, currentIndex);
                    if (!repeated)
                    {
                        repeated = true;
                        goto REPEAT;
                    }
                }
                return ActionResult.Running;
            }
            return ActionResult.Failed;
        }

        public override void DrawGUI(IntelligenceState parentState, Intelligence intelligence, CallbackCollection callbacks)
        {
#if UNITY_EDITOR
            ignorePathConfiguration = UnityEditor.EditorGUILayout.Toggle("Ignore point actions", ignorePathConfiguration);
                #endif
        }
    }
}