using UnityEngine;

namespace NodeAbilities
{
	public abstract class NodeAbility: ScriptableObject
	{
        #region Presentation
		[Header("AudioVisual")]
		public Color color;
		public AudioClip executionSound;
		public AudioClip deathSound;
        #endregion
		[Header("Skill")]
		public new string name;
		public float radius;
		public float cost;
		public float tickRate;

		public abstract void Execute (FungusNode node);

		public abstract void StopExecution (FungusNode node);

	}
}
