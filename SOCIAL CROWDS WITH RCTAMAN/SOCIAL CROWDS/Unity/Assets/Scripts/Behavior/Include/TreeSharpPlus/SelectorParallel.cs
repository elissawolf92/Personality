#region License

// A simplistic Behavior Tree implementation in C#
// Copyright (C) 2010-2011 ApocDev apocdev@gmail.com
// 
// This file is part of TreeSharp
// 
// This program is free software: you can redistribute it and/or modify
// it under the terms of the GNU Lesser General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
// 
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.
// 
// You should have received a copy of the GNU General Public License
// along with this program.  If not, see <http://www.gnu.org/licenses/>.

// TODO: THIS WAS A NEW FILE -- MODIFY THIS HEADER
#endregion

using System;
using System.Collections.Generic;

namespace TreeSharpPlus
{
    /// <summary>
    /// Parallel Selector nodes execute all of their children in parallel. If any
    /// sequence reports success, we finish all of the other ticks, but then stop
    /// all other children and report success. We report failure when all children
    /// report failure.
    /// </summary>
    public class SelectorParallel : Parallel
    {
        private int runningNodes;
        private bool doneExec;

        public SelectorParallel(params Composite[] children)
            : base(children)
        {
        }

        public override void Start()
        {
            // We start with no failed nodes
            this.doneExec = false;

            // Start all children
            this.runningNodes = this.Children.Count;
            foreach (Composite node in this.Children)
            {
                node.Start();
            }
            base.Start();
        }

        public override void Stop()
        {
            // Stop all children
            this.runningNodes = 0;
            foreach (Composite node in this.Children)
            {
                node.Stop();
            }
            base.Stop();
        }

        public override IEnumerable<RunStatus> Execute()
        {
            while (true)
            {
                foreach (Composite node in Children)
                {
                    RunStatus tickResult = node.Tick();
                    // Check to see if anything finished
                    if (tickResult != RunStatus.Running)
                    {
                        // If the node succeeded
                        if (tickResult == RunStatus.Success)
                        {
                            // We succeeded, but first we finish all of the ticks
                            this.doneExec = true;
                        }

                        // Otherwise, just stop that node
                        node.Stop();
                        this.runningNodes--;
                    }
                }
                // If we finished while ticking, we've succeeded
                if (this.doneExec == true)
                {
                    // Stop all of the children
                    foreach (Composite child in Children)
                    {
                        child.Stop();
                    }
                    // Report success
                    yield return RunStatus.Success;
                    yield break;
                }

                // If we're out of running nodes, we're done
                if (this.runningNodes == 0)
                {
                    yield return RunStatus.Failure;
                    yield break;
                }

                // For forked ticking
                yield return RunStatus.Running;
            }
        }
    }
}