using UnityEngine;
using System.Collections;

[RequireComponent(typeof(AudioSource))]
public class EventSound : MonoBehaviour
{
    AudioSource a;

    void Awake()
    {
        a = GetComponent<AudioSource>();
    }

	public void Trigger()
    {
        if (!a) { Debug.LogWarning("EventAudio no audiosource attached"); return; }
        a.PlayOneShot(a.clip);
    }
}
