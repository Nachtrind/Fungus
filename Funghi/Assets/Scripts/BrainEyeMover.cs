using UnityEngine;
using System.Collections;

public class BrainEyeMover : MonoBehaviour
{

    public Sprite[] eyeSprites;
    SpriteRenderer sr;

    public float RandomTimebetweenMin = 0.2f;
    public float RandomTimebetweenMax = 2f;
	// Use this for initialization
	void Start ()
	{
	    sr = GetComponent<SpriteRenderer>();
        if (!sr) return;
	    Invoke("ChangeEye", 1f);
	}

    void ChangeEye()
    {
        sr.sprite = eyeSprites[Random.Range(0, eyeSprites.Length)];
        Invoke("ChangeEye", Random.Range(RandomTimebetweenMin, RandomTimebetweenMax));
    }
}
