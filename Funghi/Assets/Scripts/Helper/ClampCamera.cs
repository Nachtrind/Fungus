using UnityEngine;
using System.Collections.Generic;

public class ClampCamera : MonoBehaviour
{

    public float left;
    public float top;
    public float right;
    public float bottom;

    void OnDrawGizmos()
    {
        float elevation = 1f;
        Vector3 topLeft = new Vector3(left, elevation, top);
        Vector3 topRight = new Vector3(right, elevation, top);
        Vector3 botRight = new Vector3(right, elevation, bottom);
        Vector3 botLeft = new Vector3(left, elevation, bottom);
        Gizmos.color = Color.red;
        Gizmos.DrawLine(topLeft, topRight);
        Gizmos.DrawLine(topRight, botRight);
        Gizmos.DrawLine(botRight, botLeft);
        Gizmos.DrawLine(botLeft, topLeft);
    }

    Vector3 GetClampedPosition(Vector3 inPos)
    {
        return new Vector3(Mathf.Clamp(inPos.x, left, right), inPos.y, Mathf.Clamp(inPos.z, bottom, top));
    }

    Vector3 GetClampedDelta(Vector3 inPos)
    {
        return GetClampedPosition(inPos) - inPos;
    }

    Plane groundPlane = new Plane(Vector3.up, Vector3.zero);
    public void ClampCamViewPos(Camera cam)
    {
        cam.transform.position += GetClampedDelta(GetPoint(cam.ViewportPointToRay(Vector3.zero)));
        cam.transform.position += GetClampedDelta(GetPoint(cam.ViewportPointToRay(Vector2.one)));
        cam.transform.position += GetClampedDelta(GetPoint(cam.ViewportPointToRay(new Vector2(1, 0))));
        cam.transform.position += GetClampedDelta(GetPoint(cam.ViewportPointToRay(new Vector2(0, 1))));
    }

    Vector3 GetPoint(Ray ray)
    {
        Vector3 point = ray.origin;
        float enterDistance = 0;
        if (groundPlane.Raycast(ray, out enterDistance))
        {
            point = ray.GetPoint(enterDistance);
        }
        return point;
    }
}
