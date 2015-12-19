using UnityEngine;

public class FungusCore : Entity
{

    public float groundCheckInterval = 0.33f;

    protected override void OnStart()
    {
        GameInput.RegisterCoreMoveCallback(TryMoveTo);
    }
    protected override void Cleanup()
    {
        GameInput.ReleaseCoreMoveCallback(TryMoveTo);
    }

    protected override void OnUpdate(float deltaTime)
    {
        if (!IsOnValidGround) { world.OnCoreLostGrounding(this); return; }
    }

    public void TryMoveTo(Vector3 position)
    {
        MoveTo(position);
    }

    void OnDrawGizmos()
    {
        Gizmos.color = new Color(0, 0, 1f, 0.33f);
        Gizmos.DrawSphere(transform.position, 0.15f);
    }

    public bool IsOnValidGround
    {
        get
        {
            return GameWorld.Instance.GetPositionIsSlime(transform.position, 0.2f);
        }
    }

    public override void OnDamage(Entity attacker)
    {
        if (IsDead) { world.OnCoreWasKilled(this); }
    }

}
