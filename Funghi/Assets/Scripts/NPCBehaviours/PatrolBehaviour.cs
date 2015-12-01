using UnityEngine;
using System.Collections;
using NPCBehaviours;
using System;
using Pathfinding;

public class PatrolBehaviour : NPCBehaviour
{

    public PatrolPath path;
    public int currentTargetIndex = 0;

    public enum PatrolStates { Moving, WaitingforDelay, RandomPatroling }

    public PatrolStates state;
    float currentDelay = 0f;

    public override void Evaluate(Enemy owner, float deltaTime)
    {
        if (path == null || path.points.Count == 0 || (currentTargetIndex == path.points.Count-1 & path.circularPath == false))
        {
            return;
        }
        if (currentTargetIndex >= path.points.Count)
        {
            currentTargetIndex = 0;
        }
        switch (state)
        {
            case PatrolStates.Moving:
                if (owner.PathTo(path.points[currentTargetIndex].position))
                {
                    if (path.points[currentTargetIndex].action == PatrolPath.PatrolPointActions.Continue)
                    {
                        currentTargetIndex++;
                    }else if (path.points[currentTargetIndex].action == PatrolPath.PatrolPointActions.Wait)
                    {
                        state = PatrolStates.WaitingforDelay;
                        currentDelay = path.points[currentTargetIndex].waitTime;
                    }
                }
                break;
            case PatrolStates.WaitingforDelay:
                if (currentDelay > 0)
                {
                    currentDelay = Mathf.Clamp(currentDelay - deltaTime, 0, currentDelay);
                }
                else
                {
                    state = PatrolStates.Moving;
                    currentTargetIndex++;
                }
                break;
        }
    }
}
