using NodeAbilities;
using System.Collections.Generic;
using UnityEngine;
using Pathfinding;

public class FungusNode : Entity
{
    public float size = 0.3f;

    protected override void Initialize()
    {
        world.SetPositionIsSlime(transform.position, size, true);
        CreateConnections();
    }

    protected override void Cleanup()
    {
        for (int i = 0; i < nodeConnections.Count; i++)
        {
            if (nodeConnections[i] != null)
            {
                nodeConnections[i].DisconnectionFrom(this);
            }
        }
        world.SetPositionIsSlime(transform.position, size, false);
    }

    protected override void Tick(float deltaTime)
    {
        for (int i = pendingPaths.Count; i-- > 0;)
        {
            if (!pendingPaths[i].IsValid)
            {
                pendingPaths.RemoveAt(i);
                continue;
            }
            if (pendingPaths[i].IsDone)
            {
                world.OnConnectionInitiated(this, pendingPaths[i].other, pendingPaths[i].path.vectorPath);
                pendingPaths.RemoveAt(i);
            }
        }
    }

    Color gizmoRadiusColor = new Color(1,1,1,0.25f);
    Color gizmoRadiusAbilityColor = new Color(1, 0, 0, 0.25f);
    void OnDrawGizmos()
    {
        Gizmos.color = gizmoRadiusColor; 
        Gizmos.DrawWireSphere(transform.position, GameWorld.nodeConnectionDistance);
        if (ability != null)
        {
            Gizmos.color = gizmoRadiusAbilityColor;
            Gizmos.DrawWireSphere(transform.position, ability.radius);
        }
    }

    #region Abilities

    [Header("Node specialization")]
    [SerializeField]
    NodeAbility ability;

    public void ExecuteAbility()
    {
        if (ability)
        {
            ability.Execute(this);
        }
    }

    public bool IsSpecialized
    {
        get { return ability != null; }
    }

    public void Specialize(NodeAbility newAbility)
    {
        ability = newAbility;
    }
    #endregion

    #region Network

    public bool IsConnected
    {
        get { return nodeConnections.Count > 0; }
    }

    class PathRequest
    {
        public ABPath path;
        public FungusNode other;
        public PathRequest(FungusNode self, FungusNode other)
        {
            this.other = other;
            path = ABPath.Construct(self.transform.position, other.transform.position);
            AstarPath.StartPath(path);
        }
        public bool IsDone
        {
            get { return path.IsDone(); }
        }

        public bool IsValid
        {
            get { return other != null; }
        }
    }

    List<PathRequest> pendingPaths = new List<PathRequest>();
    List<FungusNode> nodeConnections = new List<FungusNode>();

    public void ConnectTo(FungusNode other)
    {
        if (!nodeConnections.Contains(other))
        {
            nodeConnections.Add(other);
            pendingPaths.Add(new PathRequest(this, other));
        }
    }

    public void DisconnectionFrom(FungusNode other)
    {
        nodeConnections.Remove(other);
    }

    void CreateConnections()
    {
        List<FungusNode> neighbors = world.GetFungusNodes(transform.position, GameWorld.nodeConnectionDistance);
        for (int i = 0; i < neighbors.Count; i++)
        {
            if (neighbors[i] == this) { continue; }
            if (!nodeConnections.Contains(neighbors[i]))
            {
                nodeConnections.Add(neighbors[i]);
                neighbors[i].ConnectTo(this);
            }
        }
    }
    #endregion

    #region State
    public override void Damage(Entity attacker, int amount)
    {
        SubtractHealth(amount);
        if (IsDead)
        {
            world.OnFungusNodeWasDestroyed(this);
        }
    }
    #endregion

}