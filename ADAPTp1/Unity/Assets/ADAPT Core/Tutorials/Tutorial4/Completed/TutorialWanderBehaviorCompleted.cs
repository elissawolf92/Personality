using System;
using UnityEngine;
using TreeSharpPlus;
using System.Collections;

public class TutorialWanderBehaviorCompleted : Behavior 
{
    public Transform wander1;
    public Transform wander2;
    public Transform wander3;

    protected Node ST_ApproachAndWait(Transform target)
    {
        Val<Vector3> position = Val.Val(() => target.position);

        return new Sequence(
            //new LeafTrace("Going to: " + position.Value),
            this.Node_GoTo(position),
            new LeafWait(1000));
    }

    protected Node BuildTreeRoot()
    {
        return
            new DecoratorLoop(
                new DecoratorForceStatus(RunStatus.Success,
                    new SequenceShuffle(
                        ST_ApproachAndWait(this.wander1),
                        ST_ApproachAndWait(this.wander2),
                        ST_ApproachAndWait(this.wander3))));
    }

	// Use this for initialization
	void Start() 
    {
        base.StartTree(this.BuildTreeRoot());
	}
}
