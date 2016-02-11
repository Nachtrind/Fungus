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

	public static event System.Action OnResourceGain;

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

    public void Reset()
    {
        beatneat.isUnlocked = true;
        attract.isUnlocked = true;
        slowdown.isUnlocked = false;
        speedup.isUnlocked = false;
        zombies.isUnlocked = false;
        growth.isUnlocked = false;
        Start();
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
		resourceDisplay.text = CurrentResources.ToString ();

	}

	public void AddResources (float _toAdd)
	{
		if (OnResourceGain != null) {
			OnResourceGain ();
		}
		CurrentResources += _toAdd;

		if (CurrentResources > MaxResources) {
			MaxResources = CurrentResources;
		}

		resourceDisplay.text = CurrentResources.ToString ();
	}

	public void SubResources (float _toSub)
	{
		CurrentResources -= _toSub;

		resourceDisplay.text = CurrentResources.ToString ();
	}


	public void UnlockAbility (NodeAbility _toUnlock)
	{

		switch (_toUnlock.name) {
		case "growth":
			{
				growth.UnlockAbillity ();
				break;
			}
		case "slowdown":
			{
				slowdown.UnlockAbillity ();
				break;
			}
		case "speedup":
			{
				speedup.UnlockAbillity ();
				break;
			}
		case "zombie":
			{
				zombies.UnlockAbillity ();
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
