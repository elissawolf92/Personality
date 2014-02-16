using UnityEngine;
using System.Collections;

///
///  Original source code : http://www.darwin3d.com/gamedev/CCD3D.cpp
///  This class is ported on the original source code for Unity
/// 

public class IKCCD3D : IKSolver
{
    public bool damping = true;
    public float dampingMax = 0.001f;
    

    public override void Solve( IKJoint[] joints, Transform endEffector,  Vector3 tarPos)
    {
        float damp = this.dampingMax * Time.deltaTime;
        int link = joints.Length - 1;
        Vector3 endPos = endEffector.position;

        // Cap out the number of iterations
        for (int tries = 0; tries < this.maxIterations; tries++)
        {
            
            // Are we there yet?
            if ((endPos - tarPos).sqrMagnitude <= epsilon)
                break;
            if (link < 0)
                link = joints.Length - 1;

            endPos = endEffector.position;

            Vector3 rootPos = joints[link].position;
            Vector3 currentDirection = (endPos - rootPos).normalized;
            Vector3 targetDirection = (tarPos - rootPos).normalized;
            float dot = Vector3.Dot(currentDirection, targetDirection);

            if (dot < (1.0f - epsilon))
            {
                float turnRad = Mathf.Acos(dot);
                if (damping == true && turnRad > damp)
                    turnRad = damp;
                float turnDeg = turnRad * Mathf.Rad2Deg;

                // Use the cross product to determine which way to rotate
                Vector3 cross = Vector3.Cross(currentDirection, targetDirection);
                joints[link].rotation = Quaternion.AngleAxis(turnDeg, cross) * joints[link].rotation;
                joints[link].Constrain();
                joints[link].Relax(Time.deltaTime);
            }

            //// Move back in the array
            link--;
        }
    }

    public override void Solve(
        IKJoint[] joints,
        Vector3 tarPos)
    {
        this.Solve(joints, joints[joints.Length - 1].transform, tarPos);
    }

      public override void Solve(IKJoint[] joints, float angle, Vector3 target) {
          throw new System.NotImplementedException();
      }
}