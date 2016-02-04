namespace ModularBehaviour
{
    [ActionUsage(UsageType.AsCondition, UsageType.AsOneShot)]
    public class DisplaySlimeOnHead : AIAction
    {
        public override ActionResult Fire(IntelligenceController controller)
        {
            var h = controller.Owner as Human;
            if (!h) return ActionResult.Failed;
            h.EnableSlimeComponents();
            return ActionResult.Success;
        }
    }
}