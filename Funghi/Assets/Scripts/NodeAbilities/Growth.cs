using UnityEngine;
using System.Collections.Generic;

namespace NodeAbilities
{
	[AbilityIdentifier ("Growth")]
	[CreateAssetMenu (menuName = "Abilities/Growth")]
	public class Growth: NodeAbility
	{

		public GameObject growthSpores;
		GameObject spores;
		public GameObject sporeAnimObjPrefab;
		GameObject sporeAnimObj;
		Animator sporeAnim;
		public float influenceRadius;
		public ModularBehaviour.Intelligence growthIntelligence;
		public LayerMask citizenLayer;

		private GameObject infected;

		public override void Execute (FungusNode node)
		{
			//Create Animation Sprite Object
			if (sporeAnimObj == null) {
				sporeAnimObj = Instantiate (sporeAnimObjPrefab, node.transform.position, node.transform.rotation) as GameObject;
				sporeAnimObj.transform.SetParent (node.transform);
				sporeAnimObj.transform.localPosition = new Vector3 (0, 0.57f, 0);
				sporeAnimObj.transform.localRotation = Quaternion.Euler (90.0f, 0, 0);
			}

			if (sporeAnim == null) {
				sporeAnim = sporeAnimObj.GetComponent<Animator> ();
			}


			//Particles
			if (spores == null) {
				spores = Instantiate (growthSpores, new Vector3 (node.transform.position.x, node.transform.position.y + 0.5f, node.transform.position.z), Quaternion.Euler (Vector3.zero)) as GameObject;
				spores.transform.parent = node.transform;
				ParticleSystem.EmissionModule em = spores.GetComponent<ParticleSystem> ().emission;
				em.enabled = true;
				spores.GetComponent<ParticleSystem> ().Play ();
			}

			if (!spores.GetComponent<ParticleSystem> ().isPlaying) {
				spores.GetComponent<ParticleSystem> ().Play ();
				ParticleSystem.EmissionModule em = spores.GetComponent<ParticleSystem> ().emission;
				em.enabled = true;
			}

			Quaternion sporeRotation = Quaternion.Euler (new Vector3 (Wind.Instance.currentRotation.eulerAngles.x, -Wind.Instance.currentRotation.eulerAngles.z + 90.0f, Wind.Instance.currentRotation.eulerAngles.y));
			spores.transform.rotation = sporeRotation;

			InfluenceEnemyInArea (node, sporeRotation);


			if (infected == null) {
				sporeAnim.SetTrigger ("Attack");
				InfluenceEnemyInArea (node, sporeRotation);
			}
		}


		private void InfluenceEnemyInArea (FungusNode node, Quaternion sporeRot)
		{
			Vector3 rotatedVector = sporeRot * Vector3.forward;
			Vector3 dir = Vector3.Normalize (rotatedVector);
			Vector3 tempVector = new Vector3 (0, 0, 0); 
			Collider[] peopleCollider = Physics.OverlapBox (node.transform.position + dir * radius / 2, new Vector3 (influenceRadius / 2, 1, radius / 2), sporeRot, citizenLayer);

			if (peopleCollider.Length > 0) {
				Human h = peopleCollider [0].GetComponent<Human> ();
				h.SetBehaviour (growthIntelligence);
				h.TriggerBehaviour ("Infect", node);
				h.gameObject.tag = "Infected";
				infected = h.gameObject;
			}
		}

		public override void StopExecution (FungusNode node)
		{
			if (spores != null) {
				if (spores.GetComponent<ParticleSystem> ().isPlaying) {
					spores.GetComponent<ParticleSystem> ().Stop ();
					ParticleSystem.EmissionModule em = spores.GetComponent<ParticleSystem> ().emission;
					em.enabled = false;
				}
				GameObject.Destroy (sporeAnimObj);
			}
		}


	}
}