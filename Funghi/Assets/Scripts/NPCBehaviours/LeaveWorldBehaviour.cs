using UnityEngine;

namespace NPCBehaviours
{
    public class LeaveWorldBehaviour : NPCBehaviour
    {
        int pathIndex = 0;

        public override void Evaluate(IBehaviourControllable owner, float deltaTime)
        {
            if (pathIndex < path.points.Count)
            {
                if (owner.MoveToDirect(path.points[pathIndex].position) == MoveResult.ReachedTarget)
                {
                    pathIndex++;
                }
            }
            else
            {
                Destroy(owner.entity.gameObject);
            }
        }
    }
}
