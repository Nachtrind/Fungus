using System.Collections.Generic;
using UnityEngine;

namespace NodeAbilities
{
    [AbilityIdentifier("KillAndConsume")]
    [CreateAssetMenu(menuName = "Abilities/KillAndConsume")]
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
