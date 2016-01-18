using ModularBehaviour;
using UnityEngine;

[RequireComponent(typeof(CapsuleCollider))]
[RequireComponent(typeof(EntityMover))]
public class Human : Entity
{

    public string damageTriggerIdentifier = "OnDamage";

    public int resourceValue;

    //dummy feedback TODO: delete later on
    ParticleSystem particleDamage;

    protected override void OnAwake()
    {
        GetComponent<CapsuleCollider>().isTrigger = true;
        GetComponent<Rigidbody>().isKinematic = true;
		particleDamage = GetComponentInChildren<ParticleSystem> ();
    }

    public override void OnDamage(Entity attacker)
    {
        TriggerBehaviour(damageTriggerIdentifier, attacker);
        particleDamage.Play();
        if (IsDead)
        {
            world.OnHumanWasKilled(this);
        }
    }

    void OnDrawGizmos()
    {
        Gizmos.color = new Color(1, 0, 0f, 0.5f);
        Gizmos.DrawSphere(transform.position, 0.15f);
        if (behaviour)
        {
            behaviour.DrawGizmos(this);
        }
    }

#if DEBUG
    void OnGUI()
    {
        if (showDebug && behaviour)
        {
            behaviour.DrawDebugInfos(this);
        }
    }
#endif
}
