using UnityEngine;
using NPCBehaviours;

[RequireComponent(typeof(SphereCollider))]
public class EnemyExitWorldTrigger: MonoBehaviour
{
    public PatrolPath linkedPath;

    SphereCollider sc;
    void Start()
    {
        SphereCollider sc = GetComponent<SphereCollider>();
        if (sc != null)
        {
            sc.isTrigger = true;
        }
        else
        {
            Debug.Log("No collider assigned");
        }
    }

	void OnTriggerEnter(Collider other)
    {
        IBehaviourControllable ibc = other.GetComponent(typeof(IBehaviourControllable)) as IBehaviourControllable;
        if (ibc != null)
        {
            if (linkedPath == null) { Destroy(ibc.entity.gameObject); return; }
            ibc.SetBehaviour(ScriptableObject.CreateInstance<LeaveWorldBehaviour>()).path = linkedPath;
        }
    }

    Color gizmoSColor = new Color(1, 1, 0, 0.25f);
    void OnDrawGizmos()
    {
        Gizmos.color = gizmoSColor;
        if (sc == null)
        {
            sc = GetComponent<SphereCollider>();
        }
        if (sc)
        {
            Gizmos.DrawSphere(transform.position, sc.radius);
        }
    }
}
