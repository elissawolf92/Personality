using UnityEngine;
using TreeSharpPlus;
using System;
using System.Reflection;
using System.Collections;
using System.Collections.Generic;
using System.Linq.Expressions;

[AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
public class AffordanceAttribute : Attribute {}

public abstract class SmartObject : MonoBehaviour 
{
    protected Dictionary<string, Func<Character, RunStatus>> registry;

    public RunStatus Affordance(Character c, string name) 
    {
        return this.registry[name].Invoke(c);
	}

    protected void RegisterAffordances()
    {
        // Find all of the functions with the affordance attribute and 
        // bake them into the registry so they can be invoked
        this.registry = new Dictionary<string, Func<Character, RunStatus>>();
        MethodInfo[] methods = 
            this.GetType().GetMethods(
                BindingFlags.Public
                | BindingFlags.NonPublic
                | BindingFlags.Instance);
        foreach (MethodInfo method in methods)
            foreach (object attr in method.GetCustomAttributes(false))
                if (attr is AffordanceAttribute)
                    this.AddAffordance(method, (AffordanceAttribute)attr);
    }

    protected void AddAffordance(MethodInfo method, AffordanceAttribute attr)
    {
        if (method == null 
            || method.ReturnType != typeof(RunStatus)
            || method.GetParameters().Length != 1
            || method.GetParameters()[0].ParameterType != typeof(Character))
            throw new ApplicationException(
                this.gameObject.name 
                + ": Wrong function signature for affordance");

        // Reads the methodinfo and converts it into a pre-compiled Func<>
        // expression. This should make it cheaper to invoke at runtime.
        ParameterExpression param = ParameterExpression.Parameter(typeof(Character), "c");
        Expression invoke = Expression.Call(Expression.Constant(this), method, param);
        Func<Character, RunStatus> result =
            Expression.Lambda<Func<Character, RunStatus>>(invoke, param).Compile();

        this.registry.Add(method.Name, result);
    }
}
