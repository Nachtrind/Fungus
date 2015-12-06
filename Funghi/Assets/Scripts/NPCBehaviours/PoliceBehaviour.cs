using UnityEngine;
using System.Collections.Generic;
using NPCBehaviours;
using System;
using Pathfinding;

public class PoliceBehaviour : NPCBehaviour
{

    #region Settings
    const float enemyScanTimeout = 3f;

    const float alarmRadius = 2f;
    const float attackRadius = 0.5f;

    const int damagerPerSecond = 20;
    const float sightRadius = 2f;
    #endregion

    int currentTargetIndex = 0;

    public enum NPCStates { Idle, Patrolling, Alarmed, LookingForTarget, ApproachingTarget, ReturningToPatrol, InAttackRange }

    NPCStates state = NPCStates.Idle;
    float currentDelay = 0f;
    Vector3 alarmedPosition;
    Vector3 lastPatrollingPosition;
    bool pathingReversed = false;

    IBehaviourControllable owner;
    FungusNode target;
    float lookingForEnemyStarted;

    public override void Evaluate(IBehaviourControllable owner, float deltaTime)
    {
        this.owner = owner;
        switch (state)
        {
            case NPCStates.Idle:
                OnIdle(deltaTime);
                break;
            case NPCStates.Patrolling:
                OnMoving(deltaTime);
                break;
            case NPCStates.Alarmed:
                OnAlarmed(deltaTime);
                break;
            case NPCStates.LookingForTarget:
                OnLookingForTarget(deltaTime);
                break;
            case NPCStates.ApproachingTarget:
                OnApproaching(deltaTime);
                break;
            case NPCStates.ReturningToPatrol:
                OnReturningToPatrol(deltaTime);
                break;
            case NPCStates.InAttackRange:
                OnInRange(deltaTime);
                break;
        }
    }

    void GotoState(NPCStates newState)
    {
        if (newState == state) { return; }
        switch (newState)
        {
            case NPCStates.Idle:
                OnEnterIdle(state);
                break;
            case NPCStates.Patrolling:
                OnEnterMoving(state);
                break;
            case NPCStates.Alarmed:
                OnEnterAlarmed(state);
                break;
            case NPCStates.LookingForTarget:
                OnEnterLookingForTarget(state);
                break;
            case NPCStates.ApproachingTarget:
                OnEnterApproaching(state);
                break;
            case NPCStates.ReturningToPatrol:
                OnEnterReturningToPatrol(state);
                break;
            case NPCStates.InAttackRange:
                OnEnterInRange(state);
                break;
            
        }
        state = newState;
    }

    public override void OnReceivedBroadcastMessage(Message message)
    {
        if (message.type == NotificationType.PoliceAlarm)
        {
            alarmedPosition = message.position;
            GotoState(NPCStates.Alarmed);
        }
    }

    #region Transitions
    void OnEnterIdle(NPCStates oldState)
    {

    }

    void OnEnterMoving(NPCStates oldState)
    {

    }

    void OnEnterAlarmed(NPCStates oldState)
    {
        lastPatrollingPosition = owner.entity.transform.position;
    }

    void OnEnterLookingForTarget(NPCStates oldState)
    {
        lookingForEnemyStarted = GameWorld.LevelTime;
    }

    void OnEnterReturningToPatrol(NPCStates oldState)
    {

    }

    void OnEnterApproaching(NPCStates oldState)
    {

    }

    void OnEnterInRange(NPCStates oldState)
    {

    }
    #endregion

    #region StateEvaluators
    void OnIdle(float deltaTime)
    {
        if (currentDelay > 0)
        {
            currentDelay = Mathf.Clamp(currentDelay - deltaTime, 0, currentDelay);
        }
        else
        {
            GotoState(NPCStates.Patrolling);
            currentTargetIndex++;
            return;
        }
        if (FindNode())
        {
            AlarmOthers();
            GotoState(NPCStates.ApproachingTarget);
            return;
        }
    }

    void OnMoving(float deltaTime)
    {
        if (FindNode())
        {
            AlarmOthers();
            GotoState(NPCStates.ApproachingTarget);
            return;
        }
        if (path == null || path.points.Count == 0 || (currentTargetIndex == path.points.Count - 1 & path.circularPath == false))
        {
            GotoState(NPCStates.Idle);
            return;
        }
        if (pathingReversed == false)
        {
            if (currentTargetIndex >= path.points.Count)
            {
                if (!path.circularPath)
                {
                    pathingReversed = true;
                }
                else
                {
                    currentTargetIndex = 0;
                    pathingReversed = false;
                }
            }
        }
        else
        {
            if (currentTargetIndex <=0)
            {
                currentTargetIndex = 0;
                pathingReversed = false;
            }
        }
        if (owner.MoveTo(path.points[currentTargetIndex].position) == MoveResult.ReachedTarget)
        {
            PatrolPath.PatrolPoint pp = path.points[currentTargetIndex];
            if (pp.action == PatrolPath.PatrolPointActions.Continue)
            {
                if (pathingReversed)
                {
                    currentTargetIndex--;
                }
                else
                {
                    currentTargetIndex++;
                }
            }
            else if (pp.action == PatrolPath.PatrolPointActions.Wait)
            {
                GotoState(NPCStates.Idle);
                currentDelay = pp.waitTime;
                return;
            }
            else if (pp.action == PatrolPath.PatrolPointActions.ChangePath)
            {
                path = pp.linkedPath;
                currentTargetIndex = 0;
                pathingReversed = false;
                return;
            }
            else if (pp.action == PatrolPath.PatrolPointActions.ExecuteFunction)
            {
                if (pp.target == PatrolPath.PatrolPoint.FunctionTarget.NPC)
                {
                    if (pp.functionName.Length > 1)
                    {
                        owner.entity.BroadcastMessage(pp.functionName, SendMessageOptions.RequireReceiver);
                    }
                }
                else
                {
                    if (pp.functionName.Length > 1)
                    {
                        path.BroadcastMessage(pp.functionName, SendMessageOptions.RequireReceiver);
                    }
                }
            }
        }
    }

    void OnAlarmed(float deltaTime)
    {
        if (owner.MoveTo(alarmedPosition) == MoveResult.ReachedTarget)
        {
            GotoState(NPCStates.LookingForTarget);
            return;
        }
    }

    void OnLookingForTarget(float deltaTime)
    {
        if (GameWorld.LevelTime-lookingForEnemyStarted > enemyScanTimeout)
        {
            GotoState(NPCStates.ReturningToPatrol);
            return;
        }
        if (FindNode())
        {
            GotoState(NPCStates.ApproachingTarget);
            return;
        }
    }

    void OnApproaching(float deltaTime)
    {
        if (!target) { GotoState(NPCStates.LookingForTarget); return; }
        if (owner.MoveTo(target.transform.position) == MoveResult.ReachedTarget)
        {
            GotoState(NPCStates.InAttackRange);
            return;
        }
    }

    void OnReturningToPatrol(float deltaTime)
    {
        if (owner.MoveTo(lastPatrollingPosition) == MoveResult.ReachedTarget)
        {
            GotoState(NPCStates.Patrolling);
            return;
        }
    }

    void OnInRange(float deltaTime)
    {
        if (!target) { GotoState(NPCStates.LookingForTarget); return; }
        if (!IsInRange(target, attackRadius)){ GotoState(NPCStates.ApproachingTarget); return; }
        Attack(deltaTime);
    }
    #endregion

    #region Actions
    void AlarmOthers()
    {
        Message m = new Message(owner.entity, NotificationType.PoliceAlarm, owner.entity.transform.position);
        GameWorld.Instance.BroadcastToEnimies(m, m.position);
    }

    void Attack(float deltaTime)
    {
        owner.Attack(target, (int)(damagerPerSecond * deltaTime));
    }
    #endregion

    bool IsInRange(Entity e, float range)
    {
        return AstarMath.SqrMagnitudeXZ(owner.entity.transform.position, e.transform.position) <= range * range;
    }

    bool FindNode()
    {
        List<FungusNode> nodes = GameWorld.Instance.GetFungusNodes(owner.entity.transform.position, sightRadius);
        for (int i = 0; i < nodes.Count; i++)
        {
            if (GameWorld.Instance.HasLineOfSight(owner.entity, nodes[i]))
            {
                target = nodes[i];
                return true;
            }
        }
        return false;
    }

    void OnDrawGizmos()
    {
        if (owner == null) { return; }
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(owner.entity.transform.position, attackRadius);
    }
}
