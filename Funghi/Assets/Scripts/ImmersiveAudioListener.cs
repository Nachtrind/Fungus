using UnityEngine;

public class ImmersiveAudioListener : MonoBehaviour
{

    GameObject _listener;
    Plane _groundPlane = new Plane(Vector3.up, Vector3.zero);
    [Range(0, 5f)]
    public float HeightAboveGround = 0.5f;

    [Range(0, 1f)] public float PointToCam = 0.5f;

    void Start()
    {
        var existing = FindObjectOfType<AudioListener>();
        if (existing) { Destroy(existing); }
        _listener = new GameObject("Listener");
        _listener.AddComponent<AudioListener>();
        _listener.transform.parent = transform;
    }

    void Update()
    {
        var groundPoint = transform.position;
        var ray = new Ray(Camera.main.transform.position, Camera.main.transform.forward);
        float enterDistance = 0;
        if (_groundPlane.Raycast(ray, out enterDistance))
        {
            groundPoint = Camera.main.transform.position + Camera.main.transform.forward * enterDistance;
            groundPoint.y += HeightAboveGround;
        }
        _listener.transform.position = Vector3.Lerp(groundPoint, Camera.main.transform.position, PointToCam);
    }

    void OnDrawGizmos()
    {
        if (!_listener) return;
        Gizmos.color = new Color(0, 0, 1, 0.1f);
        Gizmos.DrawLine(_listener.transform.position, new Vector3(_listener.transform.position.x, 0, _listener.transform.position.z));
        Gizmos.DrawWireSphere(_listener.transform.position, 0.05f);
    }

}
