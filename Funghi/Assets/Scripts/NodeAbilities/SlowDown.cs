using System.Collections.Generic;
using UnityEngine;

namespace NodeAbilities
{
    [AbilityIdentifier("SlowDown")]
    [CreateAssetMenu(menuName = "Abilities/SlowDown")]
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

