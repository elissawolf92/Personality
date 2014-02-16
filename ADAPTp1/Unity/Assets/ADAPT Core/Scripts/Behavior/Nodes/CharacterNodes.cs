using UnityEngine;
using TreeSharpPlus;
using System.Collections;

public static class CharacterNodes
{
    public static Node Node_Gesture(this Character c, Val<string> name)
    {
        return new LeafInvoke(() => c.Gesture(name), () => c.GestureStop());
    }

    public static Node Node_GoTo(this Character c, Val<Vector3> targ)
    {
        return new LeafInvoke(() => c.NavGoTo(targ), () => c.NavStop());
    }

    public static Node Node_Reach(this Character c, Val<Vector3> targ)
    {
        return new LeafInvoke(() => c.ReachFor(targ), () => c.ReachStop());
    }

    public static Node Node_HeadLook(this Character c, Val<Vector3> targ)
    {
        return new LeafInvoke(() => c.HeadLook(targ), () => c.HeadLookStop());
    }
}
