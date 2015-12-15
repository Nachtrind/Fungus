using UnityEngine;
using NPCBehaviours;

[RequireComponent(typeof(SphereCollider))]
public class HumanExitWorldTrigger: MonoBehaviour
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
        Human e = GetComponent<Human>();
        if (e != null)
        {
            if (linkedPath == null) { Destroy(e.gameObject); return; }
            e.SetBehaviour(ScriptableObject.CreateInstance<LeaveWorldBehaviour>()).path = linkedPath;
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
