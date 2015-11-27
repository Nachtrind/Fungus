using System.Collections.Generic;
using UnityEngine;

namespace NodeAbilities
{
    [CreateAssetMenu]
    public class KillAndConsume: NodeAbility
    {

        public override void Execute(FungusNode node)
        {
            List<Enemy> enemiesInRadius = GameWorld.Instance.GetEnemies(node.transform.position, radius);
            for (int i = 0; i < enemiesInRadius.Count; i++)
            {
                enemiesInRadius[i].Kill(node);
            }
        }
    }
}
