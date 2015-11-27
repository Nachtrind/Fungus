using UnityEngine;

namespace NodeAbilities
{
    public abstract class NodeAbility: ScriptableObject
    {
        #region Presentation
        public Texture2D icon;
        public AudioClip executionSound;
        public AudioClip deathSound;
        #endregion

        public float radius;

        public abstract void Execute(FungusNode node);
    }
}
