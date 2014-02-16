using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;
using System.Reflection;
using System.Reflection.Emit;

namespace TreeSharpPlus
{
    public class LeafInvoke : Node
    {
        // A clunky way of allowing the user to specify whether we want
        // to use a function that returns a RunStatus or not. This is
        // ugly in code, but more efficient than, say, nesting lambdas
        protected Action func_noReturn = null;
        protected Func<RunStatus> func_return = null;
        protected Func<bool> func_assert = null;

        protected Action term_noReturn = null;
        protected Func<RunStatus> term_return = null;

        protected LeafInvoke()
        {
            this.func_noReturn = null;
            this.func_return = null;
            this.func_assert = null;

            this.term_noReturn = null;
            this.term_return = null;
        }

        public LeafInvoke(
            Func<RunStatus> function)
            : this()
        {
            this.func_return = function;
            this.term_return = null;
        }

        public LeafInvoke(
            Func<bool> assertion)
            : this()
        {
            this.func_assert = assertion;
            this.term_return = null;
        }

        public LeafInvoke(
            Action function)
            : this()
        {
            this.func_noReturn = function;
            this.term_return = null;
        }

        public LeafInvoke(
            Func<RunStatus> function,
            Action terminate)
            : this()
        {
            this.func_return = function;
            this.term_noReturn = terminate;
        }

        public LeafInvoke(
            Func<bool> assertion,
            Action terminate)
            : this()
        {
            this.func_assert = assertion;
            this.term_noReturn = terminate;
        }

        public LeafInvoke(
            Action function,
            Action terminate)
            : this()
        {
            this.func_noReturn = function;
            this.term_noReturn = terminate;
        }

        public LeafInvoke(
            Func<RunStatus> function,
            Func<RunStatus> terminate)
            : this()
        {
            this.func_return = function;
            this.term_return = terminate;
        }

        public LeafInvoke(
            Func<bool> assertion,
            Func<RunStatus> terminate)
            : this()
        {
            this.func_assert = assertion;
            this.term_return = terminate;
        }

        public LeafInvoke(
            Action function,
            Func<RunStatus> terminate)
            : this()
        {
            this.func_noReturn = function;
            this.term_return = terminate;
        }

        public override RunStatus Terminate()
        {
            RunStatus curStatus = this.StartTermination();
            if (curStatus != RunStatus.Running)
                return curStatus;

            // Do we have a termination function that returns a RunStatus?
            if (this.term_return != null)
                return this.ReturnTermination(this.term_return.Invoke());
            // If not, do we have a termination function that doesn't?
            else if (this.term_noReturn != null)
                this.term_noReturn.Invoke();

            return this.ReturnTermination(RunStatus.Success);
        }

        public override IEnumerable<RunStatus> Execute()
        {
            if (this.func_return != null)
            {
                RunStatus status = RunStatus.Running;
                while (status == RunStatus.Running)
                {
                    status = this.func_return.Invoke();
                    if (status != RunStatus.Running)
                        break;
                    yield return status;
                }
                yield return status;
                yield break;
            }
            else if (this.func_assert != null)
            {
                bool result = this.func_assert.Invoke();
                if (result == true)
                    yield return RunStatus.Success;
                else
                    yield return RunStatus.Failure;
                yield break;
            }
            else if (this.func_noReturn != null)
            {
                this.func_noReturn.Invoke();
                yield return RunStatus.Success;
                yield break;
            }
            else
            {
                throw new ApplicationException(this + ": No method given");
            }
        }
    }
}