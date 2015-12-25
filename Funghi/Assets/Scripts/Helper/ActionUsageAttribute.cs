using System;
using ModularBehaviour;
public class ActionUsageAttribute: Attribute
{
    public AIAction.UsageType type = AIAction.UsageType.AsContinuous | AIAction.UsageType.AsOneShot;

    public ActionUsageAttribute(params AIAction.UsageType[] types)
    {
        if (types.Length == 0) { throw new ArgumentException("no action type specified"); }
        type = types[0];
        for (int i = 1; i < types.Length; i++)
        {
            type |= types[i];
        }
    }

    public bool IsType(AIAction.UsageType type)
    {
        return (this.type & type) == type;
    }
}
