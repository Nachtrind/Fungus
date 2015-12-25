namespace ModularBehaviour
{
    [ActionUsage(UsageType.AsCondition, UsageType.AsOneShot)]
    public class StopMovement : AIAction
    {
        public override ActionResult Fire(IntelligenceController controller)
        {
            controller.Owner.StopMovement();
            return ActionResult.Success;
        }

        public override ActionResult Run(IntelligenceController controller, float deltaTime)
        {
            return Fire(controller);
        }
    }

}