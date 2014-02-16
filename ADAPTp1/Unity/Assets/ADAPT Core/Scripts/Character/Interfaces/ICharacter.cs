using UnityEngine;
using TreeSharpPlus;
using System.Collections;

public interface ICharacter
{
    RunStatus NavGoTo(Val<Vector3> target);
    RunStatus NavStop();
    RunStatus NavTurn(Val<Vector3> target);
    RunStatus NavTurn(Val<Quaternion> target);
    RunStatus NavOrientBehavior(Val<OrientationBehavior> behavior);
    RunStatus ReachFor(Val<Vector3> target);
    RunStatus ReachStop();
    RunStatus HeadLook(Val<Vector3> target);
    RunStatus HeadLookStop();
    RunStatus Gesture(Val<string> name);
    RunStatus GestureStop();
}
