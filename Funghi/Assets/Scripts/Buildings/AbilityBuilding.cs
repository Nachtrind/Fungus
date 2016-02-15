using UnityEngine;
using System.Collections;
using NodeAbilities;

public class AbilityBuilding : MonoBehaviour
{

	public NodeAbility ability;
	public LayerMask centerLayer;

	public static event System.Action<NodeAbility> OnAbilityGained;

	// Use this for initialization
	void Start ()
	{
	
	}
	
	// Update is called once per frame
	void Update ()
	{
	
	}

	void OnTriggerEnter (Collider other)
	{
		if (other.tag.Equals ("FungusCore")) {
			FungusResources.Instance.UnlockAbility (ability);
			if (OnAbilityGained != null) {
				Debug.Log ("Gained!");
				OnAbilityGained (ability);
			}
		}
	}
}
