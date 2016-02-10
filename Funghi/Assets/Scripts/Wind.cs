using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class Wind : MonoBehaviour
{

	public static event System.Action<float> OnWind;

	static Wind instance;

	public static Wind Instance {
		get {
			if (instance == null) {
				instance = FindObjectOfType<Wind> ();
			}
			return instance;
		}
	}

	public Quaternion currentRotation;

	float _currentDirection;
	float _nextDirection;
	public float DirectionChangeIntervalMin = 1f;
	public float DirectionChangeIntervalMax = 5f;
	public float DirectionChangeSpeed = 1f;

	public float AfterUserChangeTimeout = 5f;

	float _userRequestTimeout;

	// Use this for initialization
	void Start ()
	{
		UserMenu.OnRequestWindDirection += OnRequestedWindDirection;
		ChangeDirection ();
	}

	void ChangeDirection ()
	{
		if (Mathf.Approximately (_userRequestTimeout, 0)) {
			_nextDirection = Random.Range (0, 360f);
		}
		Invoke ("ChangeDirection", Random.Range (DirectionChangeIntervalMin, DirectionChangeIntervalMax) + _userRequestTimeout);
		_userRequestTimeout = 0;
	}

	void OnDestroy ()
	{
		UserMenu.OnRequestWindDirection -= OnRequestedWindDirection;
	}
	
	// Update is called once per frame
	void Update ()
	{
		currentRotation = Quaternion.AngleAxis (_currentDirection, Vector3.forward);
		_currentDirection = Mathf.MoveTowards (_currentDirection, _nextDirection, Time.deltaTime * DirectionChangeSpeed);
		if (OnWind != null) {
			OnWind (_currentDirection);
		}
	}

	void OnRequestedWindDirection (float degree)
	{
		_nextDirection = degree;
		_userRequestTimeout = AfterUserChangeTimeout;
	}

}
