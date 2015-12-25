namespace ModularBehaviour
{
    [ActionUsage(UsageType.AsContinuous, UsageType.AsCondition)]
    public class RunSpecialPath : AIAction
    {
        public override ActionResult Run(IntelligenceController controller, float deltaTime)
        {
            PatrolPath path = null;
            if (controller.GetMemoryValue(Intelligence.SpecialPathIdentifier, out path))
            {
                int currentIndex = 0;
                if (!controller.GetMemoryValue(Intelligence.SpecialPathIndexIdentifier, out currentIndex))
                {
                    controller.SetMemoryValue(Intelligence.SpecialPathIndexIdentifier, 0);
                }
                EntityMover.MoveResult result = controller.Owner.MoveTo(path.points[currentIndex].position);
                if (result == EntityMover.MoveResult.ReachedTarget)
                {
                    currentIndex++;
                    if (currentIndex >= path.points.Count)
                    {
                        currentIndex = 0;
                        controller.SetMemoryValue(Intelligence.SpecialPathIndexIdentifier, 0);
                        return ActionResult.Success;
                    }
                    controller.SetMemoryValue(Intelligence.SpecialPathIndexIdentifier, currentIndex);
                }
                return ActionResult.Running;
            }
            return ActionResult.Failed;
        }
    }
}