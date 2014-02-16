using UnityEngine;
using TreeSharpPlus;
using System.Collections;

public class WanderBehavior : MonoBehaviour
{
    // The root of the behavior tree
    public Node root = null;

    // The locations we want to randomly patrol to
    public Transform wander1 = null;
    public Transform wander2 = null;
    public Transform wander3 = null;

    // An example of a reusable parameterized subtree. Bakes a navigation
    // command and a one-second wait into a sequence. Just returns a node
    // that can be attached to any other part of a tree. It's useful to
    // prefix these functions with "ST_".
    public Node ST_WanderAndWait(Character character, Vector3 target)
    {
        return new Sequence(
            new LeafInvoke(
                () => character.NavGoTo(target), //< What to do when ticked
                () => character.NavStop()),      //< What to do if terminated
            new LeafWait(1000));
    }

    void Awake()
    {
        Character character = this.gameObject.GetComponent<Character>();
        // We can just assemble trees this way by nesting node constructors
        this.root =
            new DecoratorLoop(
                // SequenceStochastic takes three weighted nodes and shuffles
                // them each time it starts (or restarts) at the beginning
                new SequenceShuffle(
                    ST_WanderAndWait(character, this.wander1.position),
                    ST_WanderAndWait(character, this.wander2.position),
                    ST_WanderAndWait(character, this.wander3.position)));
    }

	void Start()
    {
        this.root.Start();
	}

    // This is a very simple way of doing things, but we don't need to tick
    // tree manually like in this example. See the demos/tutorials for using
    // the BehaviorManager scheduler to see how behaviors are run properly.
	void FixedUpdate() 
    {
        this.root.Tick();
	}
}
