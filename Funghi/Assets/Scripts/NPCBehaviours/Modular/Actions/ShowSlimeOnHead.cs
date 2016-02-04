
namespace ModularBehaviour
{
    public class ShowSlimeOnHead : TriggerAction
    {
        public override ActionResult Fire(IntelligenceController controller, object value = null)
        {
            var h = controller.Owner as Human;
            if (!h) return ActionResult.Failed;
            h.EnableSlimeComponents();
            return ActionResult.Success;
        }
    }
}