using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace NPCBehaviours
{
    public enum MoveResult { Preparing, Moving, ReachedTarget, TargetNotReachable }

    public interface IBehaviourControllable
    {
        NPCBehaviour SetBehaviour(NPCBehaviour behaviour);
        void RemoveBehaviour();
        /// <summary>
        /// Uses Pathfinding to reach the target
        /// </summary>
        /// <returns>true if target reached</returns>
        MoveResult MoveTo(Vector3 position);
        /// <summary>
        /// Directly moves to target position without pathfinding
        /// </summary>
        /// <returns>true if target reached</returns>
        MoveResult MoveToDirect(Vector3 position);
        bool Attack(Entity target, int damage);
        Enemy entity { get; }
    }
}
