using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/*
public enum BodyPart {
    Root,
    Neck,
    Head,
    Spine,
    Spine1,
    ClavicleL,
    ClavicleR,
    ShoulderL, 
    ShoulderR,
    PelvisL,
    PelvisR,
    ElbowL,
    ElbowR,
    FootL,
    FootR,
    KneeL,
    KneeR,
    ToeL,
    ToeR,
    Hips      
}

public enum BPart { //body parameter indices
    HeadX,
    NeckX,
    SpineY,
    Spine1X,
    ShouldersX,
    ShouldersY,
    ShouldersZ,
    ClaviclesX,
    ClaviclesY,
    ClaviclesZ,
    PelvisLX,
    PelvisRX,
    PelvisY,
    PelvisZ,
    KneesX,
    HipsX,
    SpineLength,
    ToesX
}
*/


[System.Serializable]
/*
public class TorsoInfo
{
    public List<Transform> BodyChain;

    public Transform Root;
    public Transform Neck;
    public Transform Head;
    public Transform Spine;
    public Transform Spine1;
    public Transform ClavicleL;
    public Transform ClavicleR;
    public Transform ShoulderL;
    public Transform ShoulderR;
    public Transform PelvisL;
    public Transform PelvisR;

    public Transform ElbowL;
    public Transform ElbowR;


    public Transform KneeL;
    public Transform KneeR;


    public Transform FootL;
    public Transform FootR;

    public Transform ToeL;
    public Transform ToeR;


    public Transform Hips;
//	[HideInInspector] 

	
	public Quaternion[] InitRot;
	public Vector3[] InitPos;

    public Vector3 InitRootPos; //global root position
    public Vector3 InitFootPos; //global foot position
    public Vector3 InitToePos; //global foot position

    public List<Quaternion>BodyRot;
    public List<Vector3>BodyPos;

}
*/

public class TorsoControllerMecanim : MonoBehaviour {

    
	public TorsoInfo Torso;

	private string _animName;
	
    
	// Use this for initialization

    //Assign initial positions and rotations  -- standing still positions as seen in scene view
	void Awake () {

        
		Torso.InitRot = new Quaternion[20];
		
		Torso.InitPos = new Vector3[20];

        AssignInitRootandFootPos();
    
        Torso.InitRot[(int)BodyPart.Root] = Torso.Root.localRotation;
        Torso.InitPos[(int)BodyPart.Root] = Torso.Root.localPosition;

        Torso.InitRot[(int)BodyPart.Head] = Torso.Head.localRotation;
        Torso.InitPos[(int)BodyPart.Head] = Torso.Head.localPosition;

        Torso.InitRot[(int)BodyPart.Neck] = Torso.Neck.localRotation;
        Torso.InitPos[(int)BodyPart.Neck] = Torso.Neck.localPosition;
        
        Torso.InitRot[(int)BodyPart.Spine] = Torso.Spine.localRotation;
        Torso.InitPos[(int)BodyPart.Spine] = Torso.Spine.localPosition;

        Torso.InitRot[(int)BodyPart.Spine1] = Torso.Spine1.localRotation;
        Torso.InitPos[(int)BodyPart.Spine1] = Torso.Spine1.localPosition;

        Torso.InitRot[(int)BodyPart.ShoulderL] = Torso.ShoulderL.localRotation;
        Torso.InitPos[(int)BodyPart.ShoulderL] = Torso.ShoulderL.localPosition;

        Torso.InitRot[(int)BodyPart.ShoulderR] = Torso.ShoulderR.localRotation;
        Torso.InitPos[(int)BodyPart.ShoulderR] = Torso.ShoulderR.localPosition;


        Torso.InitRot[(int)BodyPart.ClavicleL] = Torso.ClavicleL.localRotation;
        Torso.InitPos[(int)BodyPart.ClavicleL] = Torso.ClavicleL.localPosition;

        Torso.InitRot[(int)BodyPart.ClavicleR] = Torso.ClavicleR.localRotation;
        Torso.InitPos[(int)BodyPart.ClavicleR] = Torso.ClavicleR.localPosition;

        Torso.InitRot[(int)BodyPart.PelvisL] = Torso.PelvisL.localRotation;
        Torso.InitPos[(int)BodyPart.PelvisL] = Torso.PelvisL.localPosition;

        Torso.InitRot[(int)BodyPart.PelvisR] = Torso.PelvisR.localRotation;
        Torso.InitPos[(int)BodyPart.PelvisR] = Torso.PelvisR.localPosition;


        Torso.InitRot[(int)BodyPart.FootL] = Torso.FootL.localRotation;
        Torso.InitPos[(int)BodyPart.FootL] = Torso.FootL.localPosition;
        
        Torso.InitRot[(int)BodyPart.FootR] = Torso.FootR.localRotation;
        Torso.InitPos[(int)BodyPart.FootR] = Torso.FootR.localPosition;



        Torso.InitRot[(int)BodyPart.KneeL] = Torso.KneeL.localRotation;
        Torso.InitPos[(int)BodyPart.KneeL] = Torso.KneeL.localPosition;


        Torso.InitRot[(int)BodyPart.KneeR] = Torso.KneeR.localRotation;
        Torso.InitPos[(int)BodyPart.KneeR] = Torso.KneeR.localPosition;


        Torso.InitRot[(int)BodyPart.Hips] = Torso.Hips.localRotation;
        Torso.InitPos[(int)BodyPart.Hips] = Torso.Hips.localPosition;

        Torso.InitRot[(int)BodyPart.ToeL] = Torso.ToeL.localRotation;
        Torso.InitPos[(int)BodyPart.ToeL] = Torso.ToeL.localPosition;

        Torso.InitRot[(int)BodyPart.ToeR] = Torso.ToeR.localRotation;
        Torso.InitPos[(int)BodyPart.ToeR] = Torso.ToeR.localPosition;


        Torso.BodyChain = BodyChainToArray(Torso.Root);
        
        Torso.BodyPos = BodyPosArr(Torso.BodyChain);
        Torso.BodyRot = BodyRotArr(Torso.BodyChain);
        
        
	}
   

    //Assign the initial root position for this animation
    //Call when animation is changed
    //y values usually don't change as model in on the ground but x and z values change
    public void AssignInitRootandFootPos() {
        Torso.InitRootPos = Torso.Root.position;
        if (Torso.FootL.position.y < Torso.FootR.position.y)
            Torso.InitFootPos = Torso.FootL.position;
        else
            Torso.InitFootPos = Torso.FootR.position;

        if (Torso.ToeL.position.y < Torso.ToeR.position.y)
            Torso.InitToePos = Torso.ToeL.position;
        else
            Torso.InitToePos = Torso.ToeR.position;

    }


    //Assign the initial root position for this animation
    public void ResetToInitRootPos() {
        Torso.Root.position = Torso.InitRootPos;

    }

    public void Reset(){
        for (int i = 0; i < Torso.BodyChain.Count; i++) {
            Torso.BodyChain[i].transform.localPosition = Torso.BodyPos[i];
            Torso.BodyChain[i].transform.localRotation = Torso.BodyRot[i];
        }

        Torso.Root.position = Torso.InitRootPos;
       
    }

	public void Reset(string name){
		for (int i = 0; i < Torso.BodyChain.Count; i++) {
			Torso.BodyChain[i].transform.localPosition = Torso.BodyPos[i];
			Torso.BodyChain[i].transform.localRotation = Torso.BodyRot[i];
		}
		
		Torso.Root.position = Torso.InitRootPos;

		_animName = name;
	}

	//Reset to initial transforms
	public void ResetTransforms() {
        //if (animation.isPlaying) {
		Animator anim = GetComponent<Animator> ();
		if (anim.GetCurrentAnimatorStateInfo(0).IsName(_animName)) {
            Reset();
        }            
	}

    public List<Transform> BodyChainToArray(Transform root) {

        List<Transform> chain = new List<Transform>();

        chain.Add(root);

        for (int i = 0; i < root.childCount; i++)
            chain.AddRange(BodyChainToArray(root.GetChild(i)));



        return chain;
    }

    public List<Vector3> BodyPosArr(List<Transform> bodyChain) {
        List<Vector3> bodyPos = new List<Vector3>();
        for (int i = 0; i < bodyChain.Count; i++)
            bodyPos.Add(bodyChain[i].localPosition);

        return bodyPos;
    }

    public List<Quaternion> BodyRotArr(List<Transform> bodyChain) {
        List<Quaternion> bodyRot = new List<Quaternion>();
        for (int i = 0; i < bodyChain.Count; i++)
            bodyRot.Add(bodyChain[i].localRotation);

        return bodyRot;
    }
	
}

