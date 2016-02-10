using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class Wind : MonoBehaviour
{

	public Anemometer meter;

	static Wind instance;

	public static Wind Instance {
		get {
			if (instance == null) {
				instance = FindObjectOfType<Wind> ();
			}
			return instance;
		}
	}

	float directionTimer;
	float directionChangeTick;
	float minTime = 10.0f;
	float maxTime = 15.0f;
	public Quaternion currentRotation;
	float eulerZRot;
	Quaternion formerRotation;
	Quaternion targetRotation;
	float rotationSpeed = 10.0f;
	bool changing;
	float startTime;
	float journeyLength;

	// Use this for initialization
	void Start ()
	{

		directionChangeTick = Random.Range (minTime, maxTime);


	}
	
	// Update is called once per frame
	void Update ()
	{
		if (directionTimer >= directionChangeTick) {
			ChangeWindDirection ();
		}

		if (changing) {

			float distCovered = (Time.time - startTime) * rotationSpeed;
			float fracJourney = distCovered / journeyLength;

			//transform.rotation = Quaternion.Lerp (formerRotation, targetRotation, fracJourney);
			meter.SetOrientation (Quaternion.Lerp (formerRotation, targetRotation, fracJourney).eulerAngles.z);

			if (fracJourney >= 1.0f) {
				changing = false;
			}

		} else {
			directionTimer += Time.deltaTime;
		}

	}

	private void ChangeWindDirection ()
	{
		Vector3 targetVektor = currentRotation.eulerAngles + Vector3.forward * Random.Range (-90.0f, 90.0f);
		formerRotation = currentRotation;
		targetRotation = Quaternion.Euler (targetVektor - Vector3.forward * 90.0f);
		changing = true;
		directionChangeTick = Random.Range (minTime, maxTime);
		directionTimer = 0.0f;
		startTime = Time.time;
		journeyLength = Mathf.Abs (formerRotation.eulerAngles.z - targetRotation.eulerAngles.z);

	}
}
