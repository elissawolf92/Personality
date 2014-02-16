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
    public abstract class Parallel : CompositeGroup
    {
        public Parallel(params Composite[] children)
            : base(children)
        {
        }

        public abstract override IEnumerable<RunStatus> Execute();
    }
}