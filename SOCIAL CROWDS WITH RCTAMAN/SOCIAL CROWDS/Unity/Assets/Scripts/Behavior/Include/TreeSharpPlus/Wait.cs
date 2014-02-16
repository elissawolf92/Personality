using System.Collections.Generic;
using System.Diagnostics;
using System;

namespace TreeSharpPlus
{
    /// <summary>
    ///    Waits for a given period of time, set by the wait parameter
    /// </summary>
    public class Wait : Composite
    {
        protected Stopwatch stopwatch;
        protected long waitMax;

        /// <summary>
        ///    Initializes with the wait period
        /// </summary>
        /// <param name="waitMax">The time (in seconds) for which to wait</param>
        public Wait(long waitMax)
        {
            this.waitMax = waitMax;
            this.stopwatch = new Stopwatch();
        }

        /// <summary>
        ///    Resets the wait timer
        /// </summary>
        /// <param name="context"></param>
        public override void Start()
        {
            base.Start();
            this.stopwatch.Start();
        }

        public override void Stop()
        {
            base.Stop();
            this.stopwatch.Reset();
        }

        public override sealed IEnumerable<RunStatus> Execute()
        {
            while (true)
            {
                // Count down the wait timer
                // If we've waited long enough, succeed
                if (this.stopwatch.ElapsedMilliseconds >= this.waitMax)
                {
                    yield return RunStatus.Success;
                    yield break;
                }
                // Otherwise, we're still waiting
                yield return RunStatus.Running;
            }
        }
    }
}