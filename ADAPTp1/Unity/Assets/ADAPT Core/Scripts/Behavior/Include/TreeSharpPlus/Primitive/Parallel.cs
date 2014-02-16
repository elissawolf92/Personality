using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TreeSharpPlus
{
    /// <summary>
    /// The base Parallel class. Parallel nodes execute all of their children simultaneously, with
    /// varying termination conditions.
    /// </summary>
    public abstract class Parallel : NodeGroup
    {
        public Parallel(params Node[] children)
            : base(children)
        {
        }

        public abstract override IEnumerable<RunStatus> Execute();
        protected RunStatus[] childStatus = null;

        public override void Start()
        {
            if (this.childStatus == null)
                this.childStatus = new RunStatus[this.Children.Count];
            for (int i = 0; i < this.childStatus.Length; i++)
                this.childStatus[i] = RunStatus.Running;
            base.Start();
        }

        // Used for keeping track of which children have terminated
        protected RunStatus[] terminateStatus = null;
        protected void InitTerminateStatus()
        {
            if (this.terminateStatus == null)
                this.terminateStatus = new RunStatus[this.Children.Count];
            for (int i = 0; i < this.terminateStatus.Length; i++)
                this.terminateStatus[i] = RunStatus.Running;
        }

        protected RunStatus TerminateChildren()
        {
            return TreeUtils.DoUntilComplete<Node>(
                (Node n) => n.Terminate(),
                this.Children);
        }

        public override RunStatus Terminate()
        {
            RunStatus curStatus = this.StartTermination();
            if (curStatus != RunStatus.Running)
                return curStatus;
            // Just terminate each child
            return this.ReturnTermination(this.TerminateChildren());
        }
    }
}