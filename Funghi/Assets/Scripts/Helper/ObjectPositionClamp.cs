using UnityEngine;
using System.Collections.Generic;

public class ObjectPositionClamp : MonoBehaviour
{

    public float left;
    public float top;
    public float right;
    public float bottom;

    public bool testClampCam = false;

    void OnDrawGizmos()
    {
        float elevation = 5f;
        Vector3 topLeft = new Vector3(left, elevation, top);
        Vector3 topRight = new Vector3(right, elevation, top);
        Vector3 botRight = new Vector3(right, elevation, bottom);
        Vector3 botLeft = new Vector3(left, elevation, bottom);

        Gizmos.DrawLine(topLeft, topRight);
        Gizmos.DrawLine(topRight, botRight);
        Gizmos.DrawLine(botRight, botLeft);
        Gizmos.DrawLine(botLeft, topLeft);
    }

    public Vector3 GetClampedPosition(Vector3 inPos)
    {
        return new Vector3(Mathf.Clamp(inPos.x, left, right), inPos.y, Mathf.Clamp(inPos.z, bottom, top));
    }


    Plane groundPlane = new Plane(Vector3.up, Vector3.zero);
    public Vector3 GetClampedCamViewPos(Camera cam)
    {
        Vector3 point = cam.transform.position;
        Ray ray = new Ray(cam.transform.position, cam.transform.forward);
        float enterDistance = 0;
        if (groundPlane.Raycast(ray, out enterDistance))
        {
            point = cam.transform.position + cam.transform.forward * enterDistance;
            point.y = cam.transform.position.y;
        }
        Vector3 delta = point- cam.transform.position;
        return GetClampedPosition(point)-delta;
    }

    void Update()
    {
        if (testClampCam)
        {
            Camera.main.transform.position = GetClampedCamViewPos(Camera.main);
        }
    }
}
