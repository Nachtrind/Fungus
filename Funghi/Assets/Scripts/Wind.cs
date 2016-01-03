using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class Wind : MonoBehaviour
{


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
	float minTime = 15.0f;
	float maxTime = 30.0f;
	Quaternion currentRotation;
	float eulerZRot;
	public RectTransform arrowTrans;
	Quaternion formerRotation;
	Quaternion targetRotation;
	float rotationSpeed = 10.0f;
	bool changing;
	float startTime;
	float journeyLength;

	// Use this for initialization
	void Start ()
	{

		arrowTrans = GetComponent<RectTransform> ();
		directionChangeTick = Random.Range (minTime, maxTime);

		formerRotation = arrowTrans.rotation;


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

			transform.rotation = Quaternion.Lerp (formerRotation, targetRotation, fracJourney);

			if (fracJourney >= 1.0f) {
				changing = false;
			}

		} else {
			directionTimer += Time.deltaTime;
		}

	}

	private void ChangeWindDirection ()
	{
		Vector3 targetVektor = arrowTrans.eulerAngles + Vector3.forward * Random.Range (-90.0f, 90.0f);
		formerRotation = arrowTrans.rotation;
		targetRotation = Quaternion.Euler (targetVektor);
		changing = true;
		directionChangeTick = Random.Range (minTime, maxTime);
		directionTimer = 0.0f;
		startTime = Time.time;

		journeyLength = Mathf.Abs (formerRotation.eulerAngles.y - targetRotation.eulerAngles.y);

	}
}
