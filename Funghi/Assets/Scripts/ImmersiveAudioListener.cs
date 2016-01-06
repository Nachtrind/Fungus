using UnityEngine;

public class ImmersiveAudioListener : MonoBehaviour
{

    GameObject listener;
    Plane groundPlane = new Plane(Vector3.up, Vector3.zero);
    [Range(0, 5f)]
    public float heightAboveGround = 0.5f;

    void Start()
    {
        AudioListener existing = FindObjectOfType<AudioListener>();
        if (existing) { Destroy(existing); }
        listener = new GameObject("Listener");
        listener.AddComponent<AudioListener>();
        listener.transform.parent = transform;
    }

    void Update()
    {
        Vector3 groundPoint = transform.position;
        Ray ray = new Ray(Camera.main.transform.position, Camera.main.transform.forward);
        float enterDistance = 0;
        if (groundPlane.Raycast(ray, out enterDistance))
        {
            groundPoint = Camera.main.transform.position + Camera.main.transform.forward * enterDistance;
            groundPoint.y += heightAboveGround;
        }
        listener.transform.position = groundPoint;
    }

    void OnDrawGizmos()
    {
        if (listener)
        {
            Gizmos.color = new Color(0, 0, 1, 0.1f);
            Gizmos.DrawLine(listener.transform.position, new Vector3(listener.transform.position.x, 0, listener.transform.position.z));
            Gizmos.DrawWireSphere(listener.transform.position, 0.05f);
        }
    }

}
