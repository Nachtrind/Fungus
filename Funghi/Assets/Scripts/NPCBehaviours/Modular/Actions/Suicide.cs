namespace ModularBehaviour
{
    public class Suicide : AIAction
    {
        public override ActionResult Run(IntelligenceController controller, float deltaTime)
        {
            controller.Owner.Kill(controller.Owner);
            return ActionResult.Finished;
        }
    }
}