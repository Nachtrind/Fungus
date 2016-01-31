using UnityEngine;

namespace NodeAbilities
{
    [AbilityIdentifier("SpeedUp")]
    [CreateAssetMenu(menuName = "Abilities/SpeedUp")]
    public class SpeedUp: NodeAbility
	{		

		public EntityMover.SpeedModType newSpeed;
		public float speedUpTime;

		public override void Execute (FungusNode node)
		{
			FungusResources.Instance.SubResources (cost);

			FungusCore core = GameWorld.Instance.CoreInRange (node.transform.position, radius);

			if (core != null)
			{
			    core.ApplySpeedMod(newSpeed, node, speedUpTime);
			}
		}

		public override void StopExecution (FungusNode node)
		{
			
		}
	}
}