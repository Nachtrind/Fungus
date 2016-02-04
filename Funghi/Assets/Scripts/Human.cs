using ModularBehaviour;
using UnityEngine;

[RequireComponent(typeof(CapsuleCollider))]
[RequireComponent(typeof(EntityMover))]
public class Human : Entity
{

    public string damageTriggerIdentifier = "OnDamage";

    public int resourceValue;

    [SerializeField] SpriteRenderer glowRenderer;

    [SerializeField] SpriteRenderer[] slimeComponents;

    Color initialGlowColor;

    protected override void OnAwake()
    {
        GetComponent<CapsuleCollider>().isTrigger = true;
        GetComponent<Rigidbody>().isKinematic = true;
        initialGlowColor = glowRenderer.color;
        DisableSlimeComponents();
    }

    public override void OnDamage(Entity attacker)
    {
        TriggerBehaviour(damageTriggerIdentifier, attacker);
        if (IsDead)
        {
            world.OnHumanWasKilled(this);
        }
    }

    public void SetGlowColor(Color col)
    {
        if (!glowRenderer) return;
        glowRenderer.color = col;
    }

    public void ResetGlowColor()
    {
        if (!glowRenderer) return;
        glowRenderer.color = initialGlowColor;
    }

    public void EnableSlimeComponents()
    {
        for (var i = 0; i < slimeComponents.Length; i++)
        {
            if (!slimeComponents[i]) continue;
            slimeComponents[i].gameObject.SetActive(true);
        }
    }

    public void DisableSlimeComponents()
    {
        for (var i = 0; i < slimeComponents.Length; i++)
        {
            if (!slimeComponents[i]) continue;
            slimeComponents[i].gameObject.SetActive(false);
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
