using UnityEngine;
using System.Collections;

public class AudioFadeIn : MonoBehaviour
{

    float _maxVolume;
    AudioSource aus;
    public float FadeInSpeed = 0.5f;
	// Use this for initialization
	void Start ()
	{
	    aus = GetComponent<AudioSource>();
        if (!aus) return;
	    _maxVolume = aus.volume;
	    aus.volume = 0;
	}
	
	// Update is called once per frame
	void Update ()
	{
	    if (aus.volume >= _maxVolume) Destroy(this);
	    aus.volume += Time.deltaTime*FadeInSpeed;
	}
}
