using UnityEngine;
using System.Collections;

public class FungusResources : MonoBehaviour
{

	static FungusResources instance;

	public static FungusResources Instance {
		get {
			if (instance == null) {
				instance = FindObjectOfType<FungusResources> ();
			}
			return instance;
		}
	}

	public int CurrentResources { get; set; }

	public int MaxResources { get; set; }

	public int startResources;

	// Use this for initialization
	void Start ()
	{
		CurrentResources = startResources;
	}
	
	// Update is called once per frame
	void Update ()
	{
	
	}

	public void AddResources (int _toAdd)
	{
		CurrentResources += _toAdd;

		if (CurrentResources > MaxResources) {
			MaxResources = CurrentResources;
		}
	}

	public void SubResources (int _toSub)
	{
		CurrentResources -= _toSub;
	}


}
