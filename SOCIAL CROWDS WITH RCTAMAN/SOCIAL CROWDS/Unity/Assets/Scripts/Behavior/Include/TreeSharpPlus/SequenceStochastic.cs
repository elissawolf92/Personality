using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TreeSharpPlus
{
    public class SequenceStochastic : CompositeGroupWeighted
    {
        public SequenceStochastic(params CompositeWeight[] weightedchildren)
            : base(weightedchildren)
        {
        }

        public override IEnumerable<RunStatus> Execute()
        {
            // Shuffle the children using their weights
            List<Composite> shuffledChildren = this.ShuffleChildren();

            // Proceed as we do with the original sequence
            foreach (Composite node in shuffledChildren)
            {
                node.Start();
                while (node.Tick() == RunStatus.Running)
                {
                    this.Selection = node;
                    yield return RunStatus.Running;
                }

                this.Selection = null;
                node.Stop();

                if (node.LastStatus == RunStatus.Failure)
                {
                    yield return RunStatus.Failure;
                    yield break;
                }

                yield return RunStatus.Running;
            }
            yield return RunStatus.Success;
            yield break;
        }
    }
}
