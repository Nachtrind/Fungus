using System.Collections.Generic;
using UnityEngine;

namespace NodeAbilities
{
	[AbilityIdentifier ("KillAndConsume")]
	[CreateAssetMenu (menuName = "Abilities/KillAndConsume")]
	public class KillAndConsume: NodeAbility
	{
		public int damage;
		public GameObject attackAnimObjPrefab;
		GameObject attackAnimObj;
		Animator attackAnim;

		public override void Execute (FungusNode node)
		{

			//Create Animation Sprite Object
			if (attackAnimObj == null) {
				attackAnimObj = Instantiate (attackAnimObjPrefab, node.transform.position, node.transform.rotation) as GameObject;
				attackAnimObj.transform.SetParent (node.transform);
				attackAnimObj.transform.localRotation = Quaternion.Euler (90.0f, 0, 0);
			}

			if (attackAnim == null) {
				attackAnim = attackAnimObj.GetComponent<Animator> ();
			}

			List<Human> enemiesInRadius = GameWorld.Instance.GetEnemies (node.transform.position, radius);
			if (enemiesInRadius.Count > 0) {
				attackAnim.SetTrigger ("Attack");
			}
			for (int i = 0; i < enemiesInRadius.Count; i++) {
				enemiesInRadius [i].Damage (node, damage);
				FungusResources.Instance.AddResources (5.0f);
			}
		}

		public override void StopExecution (FungusNode node)
		{
			GameObject.Destroy (attackAnimObj);
		}
	}
}
