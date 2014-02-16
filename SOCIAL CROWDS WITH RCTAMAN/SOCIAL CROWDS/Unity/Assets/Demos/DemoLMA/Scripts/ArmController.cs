using UnityEngine;
using System.Collections;


[System.Serializable]
public class ArmInfo {
	public IKJoint Shoulder; //arm
	public IKJoint Elbow;    //forearm
	public IKJoint Wrist;	//hand
    /*public IKJoint hand; //wrist_end
    public IKJoint thumb; //thumb
    public IKJoint site; //site
	*/
	
//	[HideInInspector] public Transform[] armChain;
//	[HideInInspector] 
	 public IKJoint[] Bones;
	
	public Quaternion[] InitRot;
	public Vector3[] InitPos;
	
}
[System.Serializable]
public class ArmController : MonoBehaviour {
	
	public ArmInfo[] Arms;
	
	// Use this for initialization
	void Awake () {

        
		// Calculate data for ArmInfo objects
		for (int arm=0; arm<Arms.Length; arm++) {
			 //arms[arm].armChain = GetTransformChain(arms[arm].shoulder, arms[arm].wrist);
			
			//For restoring initial rotations and positions
			Arms[arm].InitRot = new Quaternion[Arms[arm].Bones.Length];
			Arms[arm].InitPos = new Vector3[Arms[arm].Bones.Length];
			
			for(int i=0; i< Arms[arm].Bones.Length;i++){
				Arms[arm].InitRot[i] = Arms[arm].Bones[i].transform.rotation;
				Arms[arm].InitPos[i] = Arms[arm].Bones[i].transform.position;
			}

            Arms[arm].Bones = new IKJoint[3];
            Arms[arm].Bones[0] = Arms[arm].Shoulder;
            Arms[arm].Bones[1] = Arms[arm].Elbow;
            Arms[arm].Bones[2] = Arms[arm].Wrist;
		}
	
		
	}
	
	//Reset to initial transforms
	public void ResetTransforms()
	{		
        if(animation.isPlaying) {
		    for(int arm = 0; arm < Arms.Length; arm++){
			    for(int i= 0; i< Arms[arm].Bones.Length;i++){
				    Arms[arm].Bones[i].transform.rotation = Arms[arm].InitRot[i];			
				    Arms[arm].Bones[i].transform.position = Arms[arm].InitPos[i];
			    }
		    }
        }
		
	}
	
	public Transform[] GetTransformChain(Transform upper, Transform lower) {
		Transform t = lower;
		int chainLength = 1;
		while (t != upper) {
			t = t.parent;
			chainLength++;
		}
		Transform[] chain = new Transform[chainLength];
		t = lower;
		for (int j=0; j<chainLength; j++) {
			chain[chainLength-1-j] = t;
			t = t.parent;
		}
		return chain;
	}
}
