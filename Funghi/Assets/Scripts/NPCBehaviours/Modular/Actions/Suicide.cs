namespace ModularBehaviour
{
    [ActionUsage(UsageType.AsContinuous, UsageType.AsCondition, UsageType.AsOneShot)]
    public class Suicide : AIAction
    {
        public override ActionResult Run(IntelligenceController controller, float deltaTime)
        {
            return Fire(controller);
        }

        public override ActionResult Fire(IntelligenceController controller)
        {
            controller.Owner.Kill(controller.Owner);
            return ActionResult.Success;
        }
    }
}