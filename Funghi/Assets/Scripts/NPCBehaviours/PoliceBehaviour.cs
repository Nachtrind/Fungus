using NPCBehaviours;
using Pathfinding;
using System.Collections.Generic;
using UnityEngine;

public class PoliceBehaviour : NPCBehaviour
{

    #region Settings
    [Range(0, 10f)]
    public float enemyRescanTimeout = 3f;

    [Range(0, 5f)]
    public float alarmRadius = 2f;
    const float attackRadius = 0.5f;

    [Range(0, 100)]
    public int damagerPerSecond = 20;
    [Range(0, 5f)]
    public float sightRadius = 2f;
    #endregion

    int currentTargetIndex = 0;

    public enum NPCStates { Idle, Patroling, Alarmed, Waiting, LookingForTarget, ApproachingTarget, ReturningToPatrol, InAttackRange }

    [SerializeField, ReadOnlyInInspector]
    NPCStates state = NPCStates.Idle;
    float currentDelay = 0f;
    Vector3 alarmedPosition;
    Vector3 lastPatrolingPosition;

    Human owner;
    FungusNode target;
    float lookingForEnemyStarted;
    NPCStates delayReturnState = NPCStates.Idle;

    #region Gizmos/Debug
    Color gizmoColor = new Color(0, 0, 1f, 0.25f);
    public override void DrawGizmos(Human owner)
    {
        Gizmos.color = gizmoColor;
        Gizmos.DrawWireSphere(owner.transform.position, sightRadius);
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(owner.transform.position, attackRadius);
    }
#if UNITY_EDITOR
    public override void DrawDebugInfos(Human owner)
    {
        Vector2 unitPos = Camera.main.WorldToScreenPoint(owner.transform.position);
        unitPos.y = Screen.height - unitPos.y;
        GUILayout.BeginArea(new Rect(unitPos, new Vector2(150, 50)), GUI.skin.box);
        GUILayout.Label("State: " + state.ToString());
        GUILayout.Label("Target: " + (target != null ? target.name : "none"));
        GUILayout.EndArea();
    }
#endif
    #endregion

    public override void Evaluate(Human owner, float deltaTime)
    {
        this.owner = owner;
        switch (state)
        {
            case NPCStates.Idle:
                OnIdle(deltaTime);
                break;
            case NPCStates.Patroling:
                OnPatroling(deltaTime);
                break;
            case NPCStates.Waiting:
                OnWaiting(deltaTime);
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
            case NPCStates.Patroling:
                OnEnterPatroling(state);
                break;
            case NPCStates.Alarmed:
                OnEnterAlarmed(state);
                break;
            case NPCStates.Waiting:
                OnEnterWaiting(state);
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

    void OnEnterPatroling(NPCStates oldState)
    {

    }

    void OnEnterWaiting(NPCStates oldState)
    {
        delayReturnState = oldState;
        owner.StopMovement();
    }

    void OnEnterAlarmed(NPCStates oldState)
    {
        lastPatrolingPosition = owner.transform.position;
    }

    void OnEnterLookingForTarget(NPCStates oldState)
    {
        lookingForEnemyStarted = GameWorld.LevelTime;
    }

    void OnEnterReturningToPatrol(NPCStates oldState)
    {
        target = null;
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
            return;
        }
        if (target == null)
        {
            if (FindNode())
            {
                AlarmOthers();
                GotoState(NPCStates.ApproachingTarget);
                return;
            }
            if (path != null)
            {
                GotoState(NPCStates.Patroling);
                return;
            }
        }
    }

    void OnPatroling(float deltaTime)
    {
        if (FindNode())
        {
            AlarmOthers();
            GotoState(NPCStates.ApproachingTarget);
            return;
        } 
        if (owner.MoveTo(path.points[currentTargetIndex].position) == Human.MoveResult.ReachedTarget)
        {
            PatrolPath.PatrolPoint pp = path.points[currentTargetIndex];
            if (pp.action == PatrolPath.PatrolPointActions.Wait)
            {
                currentDelay = pp.waitTime;
                GotoState(NPCStates.Waiting);
            }
            if (pp.action == PatrolPath.PatrolPointActions.ChangePath)
            {
                path = pp.linkedPath;
                currentTargetIndex = path.GetNearestPatrolPointIndex(owner.transform.position);
            }
            if (pp.action == PatrolPath.PatrolPointActions.ExecuteFunction)
            {
                if (pp.target == PatrolPath.PatrolPoint.FunctionTarget.NPC)
                {
                    if (pp.functionName.Length > 1)
                    {
                        owner.BroadcastMessage(pp.functionName, SendMessageOptions.RequireReceiver);
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
            currentTargetIndex++;
            if (currentTargetIndex >= path.points.Count)
            {
                currentTargetIndex = 0;
            }
        }
    }

    void OnWaiting(float deltaTime)
    {
        if (FindNode())
        {
            currentDelay = 0;
        }
        if (currentDelay > 0)
        {
            currentDelay = Mathf.Clamp(currentDelay - deltaTime, 0, currentDelay);
            return;
        }
        GotoState(delayReturnState);
    }

    void OnAlarmed(float deltaTime)
    {
        if (owner.MoveTo(alarmedPosition) == Human.MoveResult.ReachedTarget)
        {
            GotoState(NPCStates.LookingForTarget);
            return;
        }
    }

    void OnLookingForTarget(float deltaTime)
    {
        if (GameWorld.LevelTime - lookingForEnemyStarted > enemyRescanTimeout)
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
        if (owner.MoveTo(target.transform.position) == Human.MoveResult.ReachedTarget)
        {
            GotoState(NPCStates.InAttackRange);
            return;
        }
    }

    void OnReturningToPatrol(float deltaTime)
    {
        if (owner.MoveTo(lastPatrolingPosition) == Human.MoveResult.ReachedTarget)
        {
            GotoState(NPCStates.Patroling);
            return;
        }
    }

    void OnInRange(float deltaTime)
    {
        if (!target) { GotoState(NPCStates.LookingForTarget); return; }
        if (!IsInRange(target, attackRadius)) { GotoState(NPCStates.ApproachingTarget); return; }
        Attack(deltaTime);
    }
    #endregion

    #region Actions
    void AlarmOthers()
    {
        Message m = new Message(owner, NotificationType.PoliceAlarm, owner.transform.position);
        GameWorld.Instance.BroadcastToEnimies(m, m.position);
    }

    void Attack(float deltaTime)
    {
        owner.Attack(target, (int)(damagerPerSecond * deltaTime));
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
    #endregion

    bool IsInRange(Entity e, float range)
    {
        return AstarMath.SqrMagnitudeXZ(owner.transform.position, e.transform.position) <= range * range;
    }

}
