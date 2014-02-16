using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TreeSharpPlus
{
    public class SelectorStochastic : CompositeGroupWeighted
    {
        public SelectorStochastic(params CompositeWeight[] weightedchildren)
            : base(weightedchildren)
        {
        }

        public override IEnumerable<RunStatus> Execute()
        {
            // Shuffle the children using their weights
            List<Composite> shuffledChildren = this.ShuffleChildren();

            // Proceed as we do with the original selector
            foreach (Composite node in shuffledChildren)
            {
                node.Start();
                // If the current node is still running, report that. Don't 'break' the enumerator
                while (node.Tick() == RunStatus.Running)
                {
                    this.Selection = node;
                    yield return RunStatus.Running;
                }

                // Clear the selection
                this.Selection = null;

                // Call Stop to allow the node to clean anything up.
                node.Stop();

                // If it succeeded, we return success without trying any subsequent nodes
                if (node.LastStatus == RunStatus.Success)
                {
                    yield return RunStatus.Success;
                    yield break;
                }

                // Otherwise, we're still running
                yield return RunStatus.Running;
            }
            // We ran out of children, and none succeeded. Return failed.
            yield return RunStatus.Failure;
            // Make sure we tell our parent composite, that we're finished.
            yield break;
        }
    }
}
