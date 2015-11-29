using UnityEngine;
using System.Collections;

public class FungusStart : MonoBehaviour
{
    Color gizmoColor = new Color(0.25f, 1, 0f, 0.5f);
    void OnDrawGizmos()
    {
        Gizmos.color = gizmoColor;
        Gizmos.DrawSphere(transform.position, 0.3f);
    }
}
