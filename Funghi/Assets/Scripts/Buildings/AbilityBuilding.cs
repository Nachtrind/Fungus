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
		if (other.gameObject.CompareTag ("FungusCore")) {
			FungusResources.Instance.UnlockAbility (ability);
			Destroy (Instantiate (Resources.Load<GameObject> ("AbilityGainParticles"), other.transform.position, Quaternion.identity), 1.5f);
			UserMenu.current.PingSkillButton ();
			if (OnAbilityGained != null) {
				OnAbilityGained (ability);
			}
		}
	}
}
