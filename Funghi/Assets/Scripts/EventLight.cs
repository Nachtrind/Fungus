using UnityEngine;
using System.Collections;

public class EventLight : MonoBehaviour
{
    public float lightDuration;

    Light l;

    void Awake()
    {
        l = GetComponent<Light>();
    }

	public void Trigger()
    {
        if (!l) { Debug.LogWarning("EventLight no light attached"); return; }
        StartCoroutine(Light(lightDuration));
    }

    IEnumerator Light(float time)
    {
        l.enabled = true;
        yield return new WaitForSeconds(time);
        l.enabled = false;
    }
}
