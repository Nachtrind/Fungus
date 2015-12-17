namespace ModularBehaviour
{
    public class RunSpecialPath : AIAction
    {
        public override ActionResult Run(IntelligenceController controller, float deltaTime)
        {
            PatrolPath path = null;
            if (controller.Storage.TryGetParameter(Intelligence.SpecialPathIdentifier, out path))
            {
                int currentIndex = 0;
                if (!controller.Storage.TryGetParameter(Intelligence.SpecialPathIndexIdentifier, out currentIndex))
                {
                    controller.Storage.SetParameter(Intelligence.SpecialPathIndexIdentifier, 0);
                }
                Human.MoveResult result = controller.Owner.MoveTo(path.points[currentIndex].position);
                if (result == Human.MoveResult.ReachedTarget)
                {
                    currentIndex++;
                    if (currentIndex >= path.points.Count)
                    {
                        currentIndex = 0;
                        controller.Storage.SetParameter(Intelligence.SpecialPathIndexIdentifier, 0);
                        return ActionResult.Finished;
                    }
                    controller.Storage.SetParameter(Intelligence.SpecialPathIndexIdentifier, currentIndex);
                }
                return ActionResult.Running;
            }
            return ActionResult.Failed;
        }
    }
}