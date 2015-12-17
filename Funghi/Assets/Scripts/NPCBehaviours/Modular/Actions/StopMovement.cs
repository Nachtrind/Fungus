namespace ModularBehaviour
{
    public class StopMovement : OneShotAction
    {
        public override ActionResult Fire(IntelligenceController controller)
        {
            controller.Owner.StopMovement();
            return ActionResult.Finished;
        }
    }

}