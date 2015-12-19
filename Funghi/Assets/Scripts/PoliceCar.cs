using UnityEngine;
using ModularBehaviour;

public class PoliceCar : Entity
{
    public Human policePrefab;
    public Intelligence policeBehaviour;
    public int numPoliceInside = 3;
    public Vector3 doorOffsetLeft;
    public Vector3 doorOffsetRight;

    public bool debug = false;

    public void SpawnPolice()
    {
        if (numPoliceInside <= 0) { return; }
        Human h = null;
        if (Random.Range(0, 100) >= 50)
        {
            h = Instantiate(policePrefab, transform.position + doorOffsetLeft, transform.rotation) as Human;
        }
        else
        {
            h = Instantiate(policePrefab, transform.position + doorOffsetRight, transform.rotation) as Human;
        }
        if (h)
        {
            h.SetBehaviour(policeBehaviour);
        }
        numPoliceInside--;
    }

    public override void OnDamage(Entity attacker)
    {
        //boom?
    }

    Color gizmoColor = new Color(1, 1, 1, 0.33f);
    void OnDrawGizmos()
    {
        Gizmos.color = gizmoColor;
        Gizmos.DrawLine(transform.position, transform.position + doorOffsetLeft);
        Gizmos.DrawWireSphere(transform.position + doorOffsetLeft, 0.05f);
        Gizmos.DrawLine(transform.position, transform.position + doorOffsetRight);
        Gizmos.DrawWireSphere(transform.position + doorOffsetRight, 0.05f);
    }

    void OnGUI()
    {
        if (debug && behaviour)
        {
            behaviour.DrawDebugInfos(this);
        }
    }
}
