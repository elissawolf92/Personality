/*
Copyright (c) 2008, Rune Skovbo Johansen & Unity Technologies ApS

See the document "TERMS OF USE" included in the project folder for licencing details.
*/
using UnityEngine;
using System.Collections;

public abstract class IKSolver 
{
	public float epsilon = 0.001f;
    public int maxIterations = 100; // (If applicable)

    public abstract void Solve(IKJoint[] bones, float swivelAngle, Vector3 target);
    public abstract void Solve(IKJoint[] joints, Vector3 target);
    public abstract void Solve(IKJoint[] transforms, Transform endEffector, Vector3 target);

    public void Solve(Transform[] transforms,Transform endEffector, Vector3 target)
    {
        IKJoint[] joints = new IKJoint[transforms.Length];
        for (int i = 0; i < transforms.Length; i++)
            joints[i] = new IKJoint(transforms[i]);
        this.Solve(joints, endEffector, target);
    }

    public void Solve(Transform[] transforms, Vector3 target)
    {
        IKJoint[] joints = new IKJoint[transforms.Length];
        for (int i = 0; i < transforms.Length; i++)
            joints[i] = new IKJoint(transforms[i]);
        this.Solve(joints, target);
    }
    public void Solve(Transform[] transforms, float angle, Vector3 target)
    {
        IKJoint[] joints = new IKJoint[transforms.Length];
        for (int i = 0; i < transforms.Length; i++)
            joints[i] = new IKJoint(transforms[i]);
        this.Solve(joints, angle, target);
    }

}
