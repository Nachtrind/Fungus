using UnityEngine;
using System.Collections;

namespace NodeAbilities
{
	[AbilityIdentifier("SpeedUp")]
	[CreateAssetMenuAttribute()]
	public class SpeedUp: NodeAbility
	{		

		public float newSpeed;
		public float speedUpTime;

		public override void Execute (FungusNode node)
		{
			FungusResources.Instance.SubResources (cost);

			FungusCore core = GameWorld.Instance.CoreInRange (node.transform.position, radius);

			if (core != null) {
				core.ChangeSpeed (newSpeed, node, speedUpTime);
			}
		}

		public override void StopExecution (FungusNode node)
		{
			
		}
	}
}