﻿namespace NPCBehaviours
{
    public class LeaveWorldBehaviour : NPCBehaviour
    {
        int pathIndex = 0;

        public override void Evaluate(Human owner, float deltaTime)
        {
            if (pathIndex < path.points.Count)
            {
                if (owner.MoveToDirect(path.points[pathIndex].position) == Human.MoveResult.ReachedTarget)
                {
                    pathIndex++;
                }
            }
            else
            {
                Destroy(owner.gameObject);
            }
        }
    }
}