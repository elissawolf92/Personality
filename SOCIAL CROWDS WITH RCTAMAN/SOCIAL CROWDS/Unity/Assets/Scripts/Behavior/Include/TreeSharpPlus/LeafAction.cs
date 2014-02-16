using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using System.Reflection.Emit;

namespace TreeSharpPlus
{
    public class LeafAction : Executable
    {
        /// <summary>
        /// Initializes an action from a reflected method
        /// </summary>
        /// <param name="method">The MethodInfo of the action to invoke</param>
        /// <param name="parameters">The parameters (or references) to pass to
        /// the invoked method</param>
        protected LeafAction(MethodInfo method, params object[] parameters)
            : base(method, parameters)
        {
        }

        /// <summary>
        /// Initializes this action node on a delegate
        /// </summary>
        /// <param name="func">The function delegate to invoke</param>
        /// <param name="parameters">The parameters (or references) to pass
        /// to the invoked method</param>
        public LeafAction(Func<object[], RunStatus> func, params object[] parameters)
            : this(func.Method, parameters)
        {
        }

        /// <summary>
        /// Initializes an action from a reflected method
        /// </summary>
        /// <param name="method">The MethodInfo of the action to invoke</param>
        /// <param name="target">The target of the action (usually an Agent)</param>
        /// <param name="parameters">The parameters (or references) to pass to 
        /// the invoked method</param>
        public LeafAction(
            string methodName, 
            object target, 
            params object[] parameters)
        {
            Type[] types = GetParameterTypes(parameters);
            this.Method = target.GetType().GetMethod(methodName, types);

            if (this.Method == null)
                throw new ApplicationException(
                    "TreeSharpPlus: No matching method " 
                    + methodName 
                    + " in target");

            if ((typeof(RunStatus).IsAssignableFrom(Method.ReturnType)) == false)
                throw new ApplicationException("TreeSharpPlus: Method " 
                    + methodName 
                    + " does not return RunStatus");

            this.Initialize(this.Method, parameters);
            this.Target = target;
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
                RunStatus status = RunStatus.Running;
                while (status == RunStatus.Running)
                {
                    status =
                        (RunStatus)this.Method.Invoke(
                            this.Target,
                            this.paramFields);
                    yield return status;
                } 
            }
        }
    }

    public class LeafAction<T1> : LeafAction
    {
        public LeafAction(Func<T1, RunStatus> func, params object[] parameters)
            : base(func.Method, parameters)
        {
        }
    }

    public class LeafAction<T1, T2> : LeafAction
    {
        public LeafAction(Func<T1, T2, RunStatus> func, params object[] parameters)
            : base(func.Method, parameters)
        {
        }
    }

    public class LeafAction<T1, T2, T3> : LeafAction
    {
        public LeafAction(Func<T1, T2, T3, RunStatus> func, params object[] parameters)
            : base(func.Method, parameters)
        {
        }
    }

    public class LeafAction<T1, T2, T3, T4> : LeafAction
    {
        public LeafAction(Func<T1, T2, T3, T4, RunStatus> func, params object[] parameters)
            : base(func.Method, parameters)
        {
        }
    }
}
