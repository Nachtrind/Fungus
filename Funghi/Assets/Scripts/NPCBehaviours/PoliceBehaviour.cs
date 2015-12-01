using UnityEngine;
using System.Collections.Generic;
using NPCBehaviours;
using System;
using Pathfinding;

public class PoliceBehaviour : NPCBehaviour
{

    public PatrolPath path;
    public int currentTargetIndex = 0;

    public float alarmRadius = 2f;
    public float enemyScanTimeout = 3f;

    public float attackRadius = 0.2f;
    public int damagerPerSecond = 20;
    public float sightRadius = 2f;

    public enum NPCStates { Idle, Patrolling, Alarmed, LookingForTarget, ApproachingTarget, ReturningToPatrol, InAttackRange }

    public NPCStates state = NPCStates.Idle;
    float currentDelay = 0f;
    Vector3 alarmedPosition;
    Vector3 lastPatrollingPosition;

    public FungusNode target;
    float lookingForEnemyStarted;

    public override void Evaluate(float deltaTime)
    {
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

    public override void OnAlarm(Enemy alarmSource)
    {
        alarmedPosition = alarmSource.transform.position;
        GotoState(NPCStates.Alarmed);
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
        lastPatrollingPosition = owner.transform.position;
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
        if (currentTargetIndex >= path.points.Count)
        {
            currentTargetIndex = 0;
        }
        if (owner.PathTo(path.points[currentTargetIndex].position))
        {
            if (path.points[currentTargetIndex].action == PatrolPath.PatrolPointActions.Continue)
            {
                currentTargetIndex++;
            }
            else if (path.points[currentTargetIndex].action == PatrolPath.PatrolPointActions.Wait)
            {
                GotoState(NPCStates.Idle);
                currentDelay = path.points[currentTargetIndex].waitTime;
                return;
            }
        }
    }

    void OnAlarmed(float deltaTime)
    {
        if (owner.PathTo(alarmedPosition))
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
        if (owner.PathTo(target.transform.position))
        {
            GotoState(NPCStates.InAttackRange);
            return;
        }
    }

    void OnReturningToPatrol(float deltaTime)
    {
        if (owner.PathTo(lastPatrollingPosition))
        {
            GotoState(NPCStates.Patrolling);
            return;
        }
    }

    void OnInRange(float deltaTime)
    {
        if (!target) { GotoState(NPCStates.LookingForTarget); return; }
        if (!IsInRange(target, attackRadius)){ GotoState(NPCStates.ApproachingTarget); return; }
        Attack();
    }
    #endregion

    #region Actions
    void AlarmOthers()
    {
        List<Enemy> others = GameWorld.Instance.GetEnemies(owner.transform.position, alarmRadius);
        for (int i = 0; i < others.Count; i++)
        {
            others[i].Alarm(owner);
        }
    }

    void Attack()
    {
        target.Damage(owner, (int)(damagerPerSecond * GameWorld.TickInterval));
    }
    #endregion

    bool IsInRange(Entity e, float range)
    {
        return AstarMath.SqrMagnitudeXZ(owner.transform.position, e.transform.position) <= range * range;
    }

    bool FindNode()
    {
        List<FungusNode> nodes = GameWorld.Instance.GetFungusNodes(owner.transform.position, sightRadius);
        for (int i = 0; i < nodes.Count; i++)
        {
            if (GameWorld.Instance.HasLineOfSight(owner, nodes[i]))
            {
                target = nodes[i];
                return true;
            }
        }
        return false;
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRadius);
    }
}
