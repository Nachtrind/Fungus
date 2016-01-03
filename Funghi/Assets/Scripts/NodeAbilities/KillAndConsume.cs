using System.Collections.Generic;
using UnityEngine;
using System.IO;

namespace NodeAbilities
{
	[AbilityIdentifier("KillAndConsume")]
	[CreateAssetMenuAttribute()]
	public class KillAndConsume: NodeAbility
	{
		public int damage;
		 
		public override void Execute (FungusNode node)
		{
			List<Human> enemiesInRadius = GameWorld.Instance.GetEnemies (node.transform.position, radius);
			for (int i = 0; i < enemiesInRadius.Count; i++) {
				enemiesInRadius [i].Damage (node, damage);
				FungusResources.Instance.AddResources (5.0f);
			}
		}

		public override void StopExecution (FungusNode node)
		{
			
		}
	}
}
