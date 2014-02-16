using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using System.Reflection.Emit;

namespace TreeSharpPlus
{
    public abstract class LeafAssert : Executable
    {
        /// <summary>
        /// Initializes an action from a reflected method
        /// </summary>
        /// <param name="method">The MethodInfo of the action to invoke</param>
        /// <param name="parameters">The parameters (or references) to pass to the invoked method</param>
        protected LeafAssert(MethodInfo method, params object[] parameters)
            : base(method, parameters)
        {
        }

        public LeafAssert(Func<object[], bool> func, params object[] parameters)
            : this(func.Method, parameters)
        {
        }

        /// <summary>
        /// Execute the wrapped method
        /// </summary>
        /// <param name="context">Passed to the delegate or the Run() function</param>
        /// <returns>The status of the action.</returns>
        public override IEnumerable<RunStatus> Execute()
        {
            if (this.Method != null)
            {
                bool result = (bool)this.Method.Invoke(null, this.paramFields);
                yield return result ? RunStatus.Success : RunStatus.Failure;
            }
        }
    }

    public class LeafAssert<T1> : LeafAssert
    {
        public LeafAssert(Func<T1, bool> func, params object[] parameters)
            : base(func.Method, parameters)
        {
        }
    }

    public class LeafAssert<T1, T2> : LeafAssert
    {
        public LeafAssert(Func<T1, T2, bool> func, params object[] parameters)
            : base(func.Method, parameters)
        {
        }
    }

    public class LeafAssert<T1, T2, T3> : LeafAssert
    {
        public LeafAssert(Func<T1, T2, T3, bool> func, params object[] parameters)
            : base(func.Method, parameters)
        {
        }
    }

    public class LeafAssert<T1, T2, T3, T4> : LeafAssert
    {
        public LeafAssert(Func<T1, T2, T3, T4, bool> func, params object[] parameters)
            : base(func.Method, parameters)
        {
        }
    }
}
