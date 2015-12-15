using UnityEngine;

namespace NPCBehaviours
{
    public class EnterWorldBehaviour : NPCBehaviour
    {
        [HideInInspector]
        public NPCBehaviour afterPathBehaviour;

        [HideInInspector]
        public PatrolPath afterSpawnPath;

        int pathIndex = 0;

        public override void Evaluate(Enemy owner, float deltaTime)
        {
            if (pathIndex < path.points.Count)
            {
                if (owner.MoveToDirect(path.points[pathIndex].position) == Enemy.MoveResult.ReachedTarget)
                {
                    pathIndex++;
                }
            }
            else
            {
                owner.isAttackable = true;
                owner.SetBehaviour(afterPathBehaviour).path = afterSpawnPath;
            }
        }
    }
}
