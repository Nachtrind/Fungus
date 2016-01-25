using System.Collections.Generic;
using UnityEngine;
using ModularBehaviour;

namespace NodeAbilities
{
	[AbilityIdentifier ("Attract")]
	[CreateAssetMenu (menuName = "Abilities/Attract")]
	public class Attract: NodeAbility
	{
		public GameObject attractSpores;
		GameObject spores;
		public GameObject sporeAnimObjPrefab;
		GameObject sporeAnimObj;
		Animator sporeAnim;
		public float influenceRadius;

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

			//Particles
			if (spores == null) {
				spores = Instantiate (attractSpores, new Vector3 (node.transform.position.x, node.transform.position.y + 0.5f, node.transform.position.z), Quaternion.Euler (Vector3.zero)) as GameObject;
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

			Quaternion rotation = Quaternion.Euler (Wind.Instance.arrowTrans.transform.rotation.eulerAngles - Vector3.up * 90);
			spores.transform.rotation = Quaternion.Euler (spores.transform.rotation.eulerAngles.x, 
				Wind.Instance.arrowTrans.transform.rotation.eulerAngles.z * -1.0f - 90.0f,  
				spores.transform.rotation.eulerAngles.z);
			Vector3 rotatedVector = (rotation * Vector3.up * radius);

			InfluenceEnemiesInArea (node, rotatedVector);

		}

		public override void StopExecution (FungusNode node)
		{
			if (spores.GetComponent<ParticleSystem> ().isPlaying) {
				spores.GetComponent<ParticleSystem> ().Stop ();
				ParticleSystem.EmissionModule em = spores.GetComponent<ParticleSystem> ().emission;
				em.enabled = false;
			}
			GameObject.Destroy (sporeAnimObj);

		}

		private void InfluenceEnemiesInArea (FungusNode node, Vector3 rotatedVector)
		{

			Vector3 dir = Vector3.Normalize (rotatedVector);
			Vector3 tempVector = new Vector3 (0, 0, 0); 
			int i = 0;
			while (Vector3.Magnitude (tempVector) < Vector3.Magnitude (rotatedVector)) {
				List<Human> enemiesInRadius = GameWorld.Instance.GetEnemies (node.transform.position + tempVector, influenceRadius);
				foreach (Human h in enemiesInRadius) {
					h.TriggerBehaviour ("Lure", node);
				}
				i++;
				tempVector = dir * (i * influenceRadius);
			}
		}
	}
}