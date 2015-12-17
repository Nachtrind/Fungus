namespace ModularBehaviour
{
    public class RunPatrolPath : AIAction
    {
        public override ActionResult Run(IntelligenceController controller, float deltaTime)
        {
            PatrolPath path = null;
            if (controller.Storage.TryGetParameter(Intelligence.PathIdentifier, out path))
            {
                int currentIndex = 0;
                if (!controller.Storage.TryGetParameter(Intelligence.PathIndexIdentifier, out currentIndex))
                {
                    controller.Storage.SetParameter(Intelligence.PathIndexIdentifier, 0);
                }
                Human.MoveResult result = controller.Owner.MoveTo(path.points[currentIndex].position);
                if (result == Human.MoveResult.ReachedTarget)
                {
                    currentIndex++;
                    if (currentIndex >= path.points.Count)
                    {
                        currentIndex = 0;
                    }
                    controller.Storage.SetParameter(Intelligence.PathIndexIdentifier, currentIndex);
                }
                return ActionResult.Running;
            }
            return ActionResult.Failed;
        }
    }
}