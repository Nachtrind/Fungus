
namespace ModularBehaviour
{
    public class RevertGlowColor : TriggerAction
    {
        public override ActionResult Fire(IntelligenceController controller, object value = null)
        {
            var h = controller.Owner as Human;
            if (!h) return ActionResult.Failed;
            h.ResetGlowColor();
            return ActionResult.Success;
        }
    }
}