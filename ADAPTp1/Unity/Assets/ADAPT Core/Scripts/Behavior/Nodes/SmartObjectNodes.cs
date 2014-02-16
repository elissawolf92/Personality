using UnityEngine;
using TreeSharpPlus;
using System.Collections;

public static class SmartObjectNodes
{
    public static Node Node_Affordance(
        this SmartObject o,
        Behavior b,
        Val<string> affordance)
    {
        return new LeafInvoke(
            () => o.Affordance(b.Character, affordance.Value));
    }

    public static Node Node_Affordance(
        this SmartObject o,
        Behavior b,
        Val<string> affordance,
        Val<string> affordance_terminate)
    {
        return new LeafInvoke(
            () => o.Affordance(b.Character, affordance.Value),
            () => o.Affordance(b.Character, affordance_terminate.Value));
    }

}
