/*
Copyright (c) 2008, Rune Skovbo Johansen & Unity Technologies ApS

See the document "TERMS OF USE" included in the project folder for licencing details.
*/
using UnityEngine;
using System.Collections;


public class IKArm : IKSolver {
	
	
	public override void Solve( IKJoint[] transforms, Transform endEffector, Vector3 target) {
    }

   public override void Solve(IKJoint[] joints, Vector3 target) {
     }
    
    
    //Default: Swivel angle = 0    
	public  override void Solve(IKJoint[] bones, float swivelAngle, Vector3 target) {
		
		Transform shoulder = bones[0].transform;
		Transform elbow = bones[1].transform;
		Transform wrist = bones[2].transform;
                                 
		// Calculate the direction in which the elbow should be pointing
		Vector3 vElbowDir = Vector3.Cross(
			wrist.position - shoulder.position,
			Vector3.Cross(
				wrist.position-shoulder.position,
				wrist.position-elbow.position      
			)
		);

		// Get lengths of arm bones
		float fUpperArmLength = (elbow.position-shoulder.position).magnitude;
		float fLowerArmLength = (wrist.position-elbow.position).magnitude;
		
		// Calculate the desired new joint positions
		Vector3 pShoulder = shoulder.position;
		Vector3 pWrist = target;
	//	Vector3 pElbow = FindElbow(pShoulder,pWrist,fUpperArmLength,fLowerArmLength,vElbowDir); //without swivel angle --> FUNDA: Doesn't bend elbow correctly
                       
        Vector3 pElbow = FindElbow(pShoulder,pWrist, elbow.position,swivelAngle, vElbowDir); //with swivel angle
      	
		// Rotate the bone transformations to align correctly
		Quaternion shoulderRot = Quaternion.FromToRotation(elbow.position-shoulder.position, pElbow-pShoulder) * shoulder.rotation;
		if (System.Single.IsNaN(shoulderRot.x)) {
			Debug.LogWarning("shoulderRot="+shoulderRot+" pShoulder="+pShoulder+" pWrist="+pWrist+" fUpperArmLength="+fUpperArmLength+" fLowerArmLength="+fLowerArmLength+" vElbowDir="+vElbowDir);
		}
		else {
            shoulder.rotation = shoulderRot;        
			elbow.rotation = Quaternion.FromToRotation(wrist.position-elbow.position, pWrist-pElbow) * elbow.rotation;
            
		}

        //TODO:  i = 0 --> check constraint angles 
    /*   
        for(int i = 0; i< 3; i++){ 
            bones[i].Constrain();
    //        bones[i].Relax(Time.deltaTime);
        }
      */  
		
		
	}

    //
    //From Tolani, Goswani and Badler: Real-Time Inverse Kinematics Techniques for Anthropomorphic Limbs    
    //
    public Vector3 FindElbow(Vector3 pShoulder, Vector3 pWrist,  Vector3 elbow, float swivelAngle, Vector3 elbowDir) {


        float l1 = (elbow - pShoulder).magnitude; //upper arm
        float l2 = (elbow - pWrist).magnitude; //lower arm
                     
        Vector3 tg = pWrist - pShoulder;
        float l3 = tg.magnitude;

        
        /*
        float maxDist = (l1 + l2 )*0.999f;
		if (l3>maxDist) {
			// wrist is too far away from shoulder - adjust wrist position
			pWrist = pShoulder+(tg.normalized*maxDist);
			tg = pWrist-pShoulder;
			l3 = maxDist;
		}
		
		float minDist = Mathf.Abs((pShoulder -elbow).magnitude - (elbow - pWrist).magnitude)*1.001f;
		if (l3<minDist) {
			// wrist is too close to shoulder - adjust wrist position
			pWrist = pShoulder+(tg.normalized*minDist);
			tg = pWrist-pShoulder;
			l3 = minDist;
		}
        */
        

        Vector3 n = tg / tg.magnitude ;        
        //Vector3 a = new Vector3(0,-1,1f);         
        Vector3 a = elbowDir;
        //Projection of a axis onto circle		
        Vector3 u = a - Vector3.Dot (a, n) * n;
        u = u / u.magnitude;
        Vector3 v = Vector3.Cross(n, u);
        float cosA = (l3 * l3 + l1 * l1 - l2 * l2) / (2 * l3 * l1);          
        Vector3 c = cosA * l1 * n;
        //   Vector3 c = -n* cosA * l1  + pShoulder; //why not??? 
        float sinA = Mathf.Sqrt(1 - cosA * cosA );
        float R =  sinA * l1;
        Vector3 desElbow = c + R * (Mathf.Cos(swivelAngle) * u + Mathf.Sin(swivelAngle) * v);

        return desElbow + pShoulder;
    }


    //FUNDA: Doesn't bend elbow correctly
	public Vector3 FindElbow(Vector3 pShoulder, Vector3 pWrist, float fUpperArm, float fLowerArm, Vector3 vElbowDir) {
		Vector3 vB = pWrist-pShoulder;
		float LB = vB.magnitude;
		
		float maxDist = (fUpperArm+fLowerArm)*0.999f;
		if (LB>maxDist) {
			// wrist is too far away from shoulder - adjust wrist position
			pWrist = pShoulder+(vB.normalized*maxDist);
			vB = pWrist-pShoulder;
			LB = maxDist;
		}
		
		float minDist = Mathf.Abs(fUpperArm-fLowerArm)*1.001f;
		if (LB<minDist) {
			// wrist is too close to shoulder - adjust wrist position
			pWrist = pShoulder+(vB.normalized*minDist);
			vB = pWrist-pShoulder;
			LB = minDist;
		}
		
        //Computes the angle between upper arm and shoulder-target vector
		float aa = (LB*LB+fUpperArm*fUpperArm-fLowerArm*fLowerArm)/2/LB;  //aa = fU * cos(theta)      	
		float bb = Mathf.Sqrt(fUpperArm*fUpperArm-aa*aa); // bb = fU * sin(theta)
		Vector3 vF = Vector3.Cross(vB,Vector3.Cross(vElbowDir,vB));
		return pShoulder+(aa*vB.normalized)+(bb*vF.normalized);
	}
     


    
}
