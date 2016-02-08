using UnityEngine;

public class ParticleEffect : MonoBehaviour
{

    ParticleSystem psys;

	// Use this for initialization
	void Start ()
	{
	    psys = GetComponent<ParticleSystem>();
	    psys.Play();
	    if (!psys) Destroy(gameObject);
	    Destroy(gameObject, psys.duration + psys.startLifetime);
	}

    public void Fire(Vector3 position)
    {
        Instantiate(this, position, Quaternion.identity);
    }
}
