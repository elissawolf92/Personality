using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TreeSharpPlus
{
    public class Param<T>
    {
        T _value = default(T);
        public T Value
        {
            get
            {
                return this._value;
            }
            set
            {
                this._value = value;
            }
        }

        public Param()
        {
            this._value = default(T);
        }

        public Param(T initialValue)
        {
            this._value = initialValue;
        }

        public override string ToString()
        {
            return this.Value.ToString();
        }
    }
}
