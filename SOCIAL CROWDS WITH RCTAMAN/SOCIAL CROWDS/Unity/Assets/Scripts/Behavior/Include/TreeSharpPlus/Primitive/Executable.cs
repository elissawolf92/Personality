using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using System.Reflection.Emit;

namespace TreeSharpPlus
{
    public abstract class Executable : Composite
    {
        public MethodInfo Method { get; protected set; }
        public object Target { get; protected set; }

        protected ParameterInfo[] expectedParams = null;
        protected object[] paramFields = null;

        protected void Initialize(MethodInfo method, params object[] parameters)
        {
            this.Target = null;
            this.Method = method;
            this.paramFields = parameters;
            this.expectedParams = this.Method.GetParameters();
            ValidateParams();
        }

        protected Executable()
        {
            this.Method = null;
            this.Target = null;
            this.expectedParams = null;
            this.paramFields = null;
        }

        protected Executable(MethodInfo method, params object[] parameters)
        {
            this.Initialize(method, parameters);
        }

        /// <summary>
        /// Gets the parameter at a given index
        /// </summary>
        public object GetParam(int index)
        {
            return this.paramFields[index];
        }

        /// <summary>
        /// Sets a parameter at a given index and validates the
        /// new value
        /// </summary>
        /// <param name="index"></param>
        /// <param name="value"></param>
        public void SetParam(int index, object value)
        {
            this.paramFields[index] = value;
            
        }

        /// <summary>
        /// Evaluates a list of given parameters against the expected parameters for
        /// a method, throwing errors if anything is invalid
        /// </summary>
        /// <param name="parameters">The parameters given</param>
        /// <param name="paramInfo">The parameters expected</param>
        protected void ValidateParams()
        {
            if (paramFields.Count() > expectedParams.Count())
                throw new ApplicationException("TreeSharpPlus: More parameters than parameter fields");

            for (int i = 0; i < expectedParams.Count(); i++)
            {
                if ((expectedParams[i].IsOptional == false) && (paramFields.Count() <= i))
                    throw new ApplicationException("TreeSharpPlus: Not enough parameters given");

                Type expectedType = expectedParams[i].ParameterType;
                Type givenType = paramFields[i].GetType();
                if (expectedType.IsAssignableFrom(givenType) == false)
                {
                    throw new ApplicationException("TreeSharpPlus: Parameter type mismatch");
                }
            }
        }

        /// <summary>
        /// Returns an array of the types of the given parameters
        /// </summary>
        /// <param name="parameters"></param>
        /// <returns></returns>
        protected Type[] GetParameterTypes(object[] param)
        {
            Type[] types = new Type[param.Length];
            for (int i = 0; i < param.Length; i++)
                types[i] = param[i].GetType();
            return types;
        }
    }
}
