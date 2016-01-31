using System.Collections.Generic;
using UnityEngine;

namespace NodeAbilities
{
	[AbilityIdentifier ("SlowDown")]
	[CreateAssetMenu (menuName = "Abilities/SlowDown")]
	public class SlowDown: NodeAbility
	{
		public EntityMover.SpeedModType slowDownSpeed;
		public float slowDownTime;

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

			attackAnim.SetTrigger ("Attack");

			List<Human> enemiesInRadius = GameWorld.Instance.GetHumans (node.transform.position, radius);
			for (int i = 0; i < enemiesInRadius.Count; i++)
			{
			    enemiesInRadius[i].ApplySpeedMod(slowDownSpeed, node, slowDownTime);
			}
		}

		public override void StopExecution (FungusNode node)
		{
			Destroy (attackAnimObj);
		}
	}
}

