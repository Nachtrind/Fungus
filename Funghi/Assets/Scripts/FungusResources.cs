using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using NodeAbilities;

public class FungusResources : MonoBehaviour
{
	public NodeAbility beatneat;
	public NodeAbility attract;
	public NodeAbility slowdown;
	public NodeAbility speedup;
	public NodeAbility zombies;
	public NodeAbility growth;

	public AbilityButton bBeatneat;
	public AbilityButton bAttract;
	public AbilityButton bSlowdown;
	public AbilityButton bSpeedup;
	public AbilityButton bZombies;
	public AbilityButton bGrowth;

	static FungusResources instance;

	public List<NodeAbility> unlockedAbilities;

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
	    Debug.Log("Die folgende Zeile im Code wirft eine nullref");
        /*
		resourceDisplay.text = CurrentResources.ToString () + "/" + MaxResources.ToString ();
        */
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

//		resourceDisplay.text = CurrentResources.ToString () + "/" + MaxResources.ToString ();
	}

	public void SubResources (float _toSub)
	{
		CurrentResources -= _toSub;

		//resourceDisplay.text = CurrentResources.ToString () + "/" + MaxResources.ToString ();
	}


	public void UnlockAbility (NodeAbility _toUnlock)
	{

		switch (_toUnlock.name) {
		case "growth":
			{
				bGrowth.Unlock ();
				break;
			}
		case "slowdown":
			{
				bSlowdown.Unlock ();
				break;
			}
		case "speedup":
			{
				bSpeedup.Unlock ();
				break;
			}
		case "zombie":
			{
				bZombies.Unlock ();
				break;
			}

		default:
			{
				Debug.Log ("Something went wrong");
				break;
			}

		}

	}


}
