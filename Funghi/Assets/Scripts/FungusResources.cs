using UnityEngine;
using System.Collections;
using UnityEngine.UI;

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

	public float CurrentResources { get; set; }

	public float MaxResources { get; set; }

	public float startResources;
	public Text resourceDisplay;


	// Use this for initialization
	void Start ()
	{
		CurrentResources = startResources;
		MaxResources = CurrentResources;
		resourceDisplay.text = CurrentResources.ToString () + "/" + MaxResources.ToString ();
	}
	
	// Update is called once per frame
	void Update ()
	{
	
	}

	public void AddResources (float _toAdd)
	{
		CurrentResources += _toAdd;

		if (CurrentResources > MaxResources) {
			MaxResources = CurrentResources;
		}

		resourceDisplay.text = CurrentResources.ToString () + "/" + MaxResources.ToString ();
	}

	public void SubResources (float _toSub)
	{
		CurrentResources -= _toSub;

		resourceDisplay.text = CurrentResources.ToString () + "/" + MaxResources.ToString ();
	}


}
