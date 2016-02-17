using System.Collections.Generic;
using UnityEngine;

namespace NodeAbilities
{

	[AbilityIdentifier ("Zombie")]
	[CreateAssetMenu (menuName = "Abilities/Zombie")]
	public class Zombie: NodeAbility
	{
	
		public GameObject zombieSpores;
		GameObject spores;
		public GameObject sporeAnimObjPrefab;
		GameObject sporeAnimObj;
		Animator sporeAnim;
		public float influenceRadius;
		public ModularBehaviour.Intelligence zombieIntelligence;
		public LayerMask citizenLayer;

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

			sporeAnim.SetTrigger ("Attack");


			FungusResources.Instance.SubResources (cost);

			if (spores == null) {
				spores = Instantiate (zombieSpores, node.transform.position, Quaternion.Euler (Vector3.zero)) as GameObject;
				spores.transform.parent = node.transform;
				spores.GetComponent<ParticleSystem> ().Play ();
			}
			
			if (!spores.GetComponent<ParticleSystem> ().isPlaying) {
				spores.GetComponent<ParticleSystem> ().Play ();
			}
			
			Quaternion sporeRotation = Quaternion.Euler (new Vector3 (Wind.Instance.currentRotation.eulerAngles.x, -Wind.Instance.currentRotation.eulerAngles.z + 90.0f, Wind.Instance.currentRotation.eulerAngles.y));
			spores.transform.rotation = sporeRotation;

			InfluenceEnemiesInArea (node, sporeRotation);
			
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


		private void InfluenceEnemiesInArea (FungusNode node, Quaternion sporeRot)
		{
			Vector3 rotatedVector = sporeRot * Vector3.forward;
			Vector3 dir = Vector3.Normalize (rotatedVector);
			//Vector3 tempVector = new Vector3 (0, 0, 0); 
			Collider[] peopleCollider = Physics.OverlapBox (node.transform.position + dir * radius / 2, new Vector3 (influenceRadius / 2, 1, radius / 2), sporeRot, citizenLayer);
			//Debug.Log ("Caught Citizens with lure: " + peopleCollider.Length);
			Debug.DrawLine (node.transform.position, node.transform.position + dir * radius, Color.cyan, 4.0f);

			foreach (Collider c in peopleCollider) {
				Human h = c.GetComponent<Human> ();
				h.SetBehaviour (zombieIntelligence);
				h.TriggerBehaviour ("AttachToNode", node);
				h.gameObject.tag = "Zombie";
			}
		}

	}
}
