using System.Collections.Generic;
using UnityEngine;

namespace NodeAbilities
{
	[CreateAssetMenu]
	[AbilityIdentifier("KillAndConsume")]
	public class KillAndConsume: NodeAbility
	{
		public int damage;

		public override bool Execute (FungusNode node)
		{
			List<Enemy> enemiesInRadius = GameWorld.Instance.GetEnemies (node.transform.position, radius);
			for (int i = 0; i < enemiesInRadius.Count; i++) {
				enemiesInRadius [i].Damage (node, damage);
			}

			if (enemiesInRadius.Count > 0) {
				return true;
			}

			return false;
		}
	}
}
