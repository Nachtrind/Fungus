using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace NodeAbilities
{
	[AbilityIdentifier("SlowDown")]
	[CreateAssetMenuAttribute()]
	public class SlowDown: NodeAbility
	{		
		public float slowDownSpeed;
		public float slowDownTime;

		public override void Execute (FungusNode node)
		{
			FungusResources.Instance.SubResources (cost);

			List<Human> enemiesInRadius = GameWorld.Instance.GetEnemies (node.transform.position, radius);
			for (int i = 0; i < enemiesInRadius.Count; i++) {
				enemiesInRadius [i].ChangeSpeed (slowDownSpeed, node, slowDownTime);
			}
		}

		public override void StopExecution (FungusNode node)
		{
			
		}
	}
}

