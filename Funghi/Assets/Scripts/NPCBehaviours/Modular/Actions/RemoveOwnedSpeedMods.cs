namespace ModularBehaviour
{
    [ActionUsage(UsageType.AsCondition, UsageType.AsOneShot)]
    public class RemoveOwnedSpeedMods : AIAction
    {
        public override ActionResult Fire(IntelligenceController controller)
        {
            controller.Owner.RevokeAllSpeedMods(controller.Owner);
            return ActionResult.Success;
        }

        public override ActionResult Run(IntelligenceController controller, float deltaTime)
        {
            return Fire(controller);
        }
    }
}