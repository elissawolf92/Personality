using UnityEngine;
using System.Collections;
using Meta.Numerics.Statistics;

/*
public enum SType { //shape  indices
    Enc,
    Spr,
    Sin,
    Ris,
    Ret,
    Adv
}
public enum AxisType { //axes
    X,
    Y,
    Z
}
*/

[RequireComponent(typeof(TorsoControllerMecanim))]
public class TorsoAnimatorMecanim : MonoBehaviour {
	
	public TorsoControllerMecanim TorsoC;
    public float[] EncSpr = new float[2];
    public float[] SinRis = new float[2];
    public float[] RetAdv = new float[2];
    

    Vector3[] _headAngle = new Vector3[2];
    Vector3[] _neckAngle = new Vector3[2];
    Vector3[] _spineAngle = new Vector3[2];
    Vector3[] _spine1Angle = new Vector3[2];
    Vector3[] _shoulderAngle = new Vector3[2];
    Vector3[] _clavicleAngle = new Vector3[2];
    Vector3[] _pelvisLAngle = new Vector3[2];
    Vector3[] _pelvisRAngle = new Vector3[2];
    Vector3[] _kneeAngle = new Vector3[2];
    Vector3[] _hipAngle = new Vector3[2];
    Vector3[] _toeAngle = new Vector3[2];
    float[] _spineLength = new float[2];

	private string _animName;
    
    
    
    public float[][] ShapeParams = new float[18][] { //Angles for enclosing, spreading, sinking, rising, retreating and advancing 
        new float[] { -2.6667f,	7f,	-2f,	18f,	-6f,	15f}, //head
        new float[] {3f,	-12f,	-10f,	-10f,	9f,	-12f}, //neck
        new float[] { -3.33333f,	-2f,	-2f,	-4f,	15f,	-9f}, //spine
        new float[] { -4f,	10f,	6f,	5.33333f,	-15f,	-5f}, //spine1
        new float[] {-44.6667f,	34f,	-12f,	23.3333f,	-47f,	-24f}, //shoulderX
        new float[] { 24f,	-11f,	-22f,	-15.3333f,	8f,	20f},//shoulderY
        new float[] { -18f,	12f,	11.6667f,	-8.33333f,	2f,	3f}, //shoulderZ
        new float[] {7f,	4f,	8f,	11f,	3f,	-5f}, //claviclesX
        new float[] {-3f,	9f,	20f,	-2f,	6f,	6f}, //claviclesY
        new float[] {6f,	13f,	3f,	7f,	-10f,	3f}, //claviclesZ
        new float[] {-1f,	-3f,	0f,	-2f,	4f,	-14f}, //pelvisLX
        new float[] {-1f,	-3f,	0f,	-2f,	4f,	-14f}, //pelvisRX
        new float[] {5f,	-5f,	0f,	-0.66667f,	2f,	2f}, //pelvisY
        new float[] {0f,	-2f,	6f,	1f,	0f,	-1f}, //pelvisZ
        new float[] {0f,	-2f,	6f,	1f,	0f,	-1f}, //knees
        new float[] {0f,	-2f,	6f,	1f,	0f,	-1f},//hips
        new float[] {0f,	-2f,	6f,	1f,	0f,	-1f}, //toes
        new float[] {0f,	-2f,	6f,	1f,	0f,	-1f}}; //spineLength


	void Start() {
        Reset();
        
	}

    public void Reset(){
		TorsoC = GetComponent(typeof(TorsoControllerMecanim)) as TorsoControllerMecanim;
    
    }

	public void Reset(string animName){
		TorsoC = GetComponent(typeof(TorsoControllerMecanim)) as TorsoControllerMecanim;
		_animName = animName;
	}


    public void UpdateAnglesLinearComb() {

        for(int timeInd = 0; timeInd < 2; timeInd++){ //time = begin or time = end
            _headAngle[timeInd] = Vector3.zero;
            _neckAngle[timeInd] = Vector3.zero;
            _spineAngle[timeInd] = Vector3.zero;
            _spine1Angle[timeInd] = Vector3.zero;
            _shoulderAngle[timeInd] = Vector3.zero;
            _clavicleAngle[timeInd] = Vector3.zero;
            _pelvisLAngle[timeInd] = Vector3.zero;
            _pelvisRAngle[timeInd] = Vector3.zero;
            _kneeAngle[timeInd] = Vector3.zero;
            _hipAngle[timeInd] = Vector3.zero;
            _toeAngle[timeInd] = Vector3.zero;
            _spineLength[timeInd] = 0;

            
            for (int i = 0; i < ShapeParams.Length; i++) {
                float encSprAngle;
                if (EncSpr[timeInd] < 0) //enclosing
                    encSprAngle = -EncSpr[timeInd] * ShapeParams[i][(int)SType.Enc];
                else //spreading
                    encSprAngle = EncSpr[timeInd] * ShapeParams[i][(int)SType.Spr];

                
                float sinRisAngle;
                if (SinRis[timeInd] < 0) //sinking
                    sinRisAngle = -SinRis[timeInd] * ShapeParams[i][(int)SType.Sin];
                else            //rising
                    sinRisAngle = SinRis[timeInd] * ShapeParams[i][(int)SType.Ris];

                float retAdvAngle;
                if (RetAdv[timeInd] < 0) //retreating
                    retAdvAngle = -RetAdv[timeInd] * ShapeParams[i][(int)SType.Ret];
                else            //advancing
                    retAdvAngle = RetAdv[timeInd] * ShapeParams[i][(int)SType.Adv];

                float val = encSprAngle + sinRisAngle + retAdvAngle;



                if (i == (int)BPart.HeadX) 
                    _headAngle[timeInd].x = val;                    
                else if (i == (int)BPart.NeckX)
                    _neckAngle[timeInd].x = val;
                else if (i == (int)BPart.SpineY)
                    _spineAngle[timeInd].y = val;
                else if (i == (int)BPart.Spine1X)
                    _spine1Angle[timeInd].x = val;
                else if (i == (int)BPart.ShouldersX)
                    _shoulderAngle[timeInd].x = val;
                else if (i == (int)BPart.ShouldersY)
                    _shoulderAngle[timeInd].y = val;
                else if (i == (int)BPart.ShouldersZ)
                    _shoulderAngle[timeInd].z = val;
                else if (i == (int)BPart.ClaviclesX)
                    _clavicleAngle[timeInd].x = val;
                else if (i == (int)BPart.ClaviclesY)
                    _clavicleAngle[timeInd].y = val;
                else if (i == (int)BPart.ClaviclesZ)
                    _clavicleAngle[timeInd].z = val;
                else if (i == (int)BPart.PelvisLX)
                    _pelvisLAngle[timeInd].x = val;
                else if (i == (int)BPart.PelvisRX)
                    _pelvisRAngle[timeInd].x = val;
                else if (i == (int)BPart.PelvisY)
                    _pelvisLAngle[timeInd].y = _pelvisRAngle[timeInd].y = val;
                else if (i == (int)BPart.PelvisZ)
                    _pelvisLAngle[timeInd].z = _pelvisRAngle[timeInd].z =  val;
                else if (i == (int)BPart.KneesX) {
                    _kneeAngle[timeInd].x = val;                    
                }
                else if (i == (int)BPart.HipsX)
                    _hipAngle[timeInd].x = val;
                else if (i == (int)BPart.ToesX)
                    _toeAngle[timeInd].x = val;
                else if (i == (int)BPart.SpineLength)
                    _spineLength[timeInd] = val;

                
            }
        }        
    }

    
    void UpdateTransforms() {
        
        //Interpolate angles
		//float t = animation[GetComponent<AnimationInfoMecanim>().AnimName].normalizedTime;
		float t = GetComponent<Animator>().animation[GetComponent<AnimationInfoMecanim>().AnimName].normalizedTime;
		//Animator anim = GetComponent<Animator>();
		//float t = anim.GetCurrentAnimatorStateInfo(0).normalizedTime;
        Vector3 headAngle, neckAngle, spineAngle, spine1Angle, shoulderAngle, clavicleAngle, pelvisLAngle, pelvisRAngle, kneeAngle, hipAngle, toeAngle;
        float spineLength;


        if (t > 1f)
            t = 1f;

        headAngle = t * _headAngle[1] + (1 - t) * _headAngle[0];
        neckAngle = t * _neckAngle[1] + (1 - t) * _neckAngle[0];
        spineAngle = t * _spineAngle[1] + (1 - t) * _spineAngle[0];
        spine1Angle = t * _spine1Angle[1] + (1 - t) * _spine1Angle[0];
        shoulderAngle = t * _shoulderAngle[1] + (1 - t) * _shoulderAngle[0];
        clavicleAngle = t * _clavicleAngle[1] + (1 - t) * _clavicleAngle[0];
        pelvisLAngle = t * _pelvisLAngle[1] + (1 - t) * _pelvisLAngle[0];
        pelvisRAngle = t * _pelvisRAngle[1] + (1 - t) * _pelvisRAngle[0];
        kneeAngle = t * _kneeAngle[1] + (1 - t) * _kneeAngle[0];
        hipAngle = t * _hipAngle[1] + (1 - t) * _hipAngle[0];
        toeAngle = t * _toeAngle[1] + (1 - t) * _toeAngle[0];
        spineLength = t * _spineLength[1] + (1 - t) * _spineLength[0];


        //pelvis should not move -- assign it to the final angle
        pelvisLAngle = _pelvisLAngle[1];
        pelvisRAngle = _pelvisRAngle[1];
        hipAngle = _hipAngle[1];


        //head
        
        TorsoC.Torso.Head.Rotate(headAngle);
        //ConstrainAngles(TorsoC.Torso.Head, (int)AxisType.X, -60f, 60f);
        //ConstrainAngles(TorsoC.Torso.Head, (int)AxisType.Y, -50f, 50f);
        //ConstrainAngles(TorsoC.Torso.Head, (int)AxisType.Z, -60, 60f);


        //neck         
        TorsoC.Torso.Neck.Rotate(neckAngle);
        //ConstrainAngles(TorsoC.Torso.Neck, (int)AxisType.X, -70f, 30f);
        //ConstrainAngles(TorsoC.Torso.Neck, (int)AxisType.Y, -20f, 20f);
        //ConstrainAngles(TorsoC.Torso.Neck, (int)AxisType.Z, -110f, 110f);

        //spine
        
        TorsoC.Torso.Spine.Rotate(spineAngle);
        //ConstrainAngles(TorsoC.Torso.Spine, (int)AxisType.X, -70f, 70f);
        //ConstrainAngles(TorsoC.Torso.Spine, (int)AxisType.Y, -40f, 40f);
        //ConstrainAngles(TorsoC.Torso.Spine, (int)AxisType.Z, -10f, 150f);


        //spine1
        TorsoC.Torso.Spine1.Rotate(spine1Angle);
        //ConstrainAngles(TorsoC.Torso.Spine1, (int)AxisType.X, -40f, 40f);
        //ConstrainAngles(TorsoC.Torso.Spine1, (int)AxisType.Y, -70f, 70f);
        //ConstrainAngles(TorsoC.Torso.Spine1, (int)AxisType.Z, -30f, 150f);


        //spineLength
        TorsoC.Torso.Spine1.Translate(0, 0, spineLength);

        //shoulders
        TorsoC.Torso.ShoulderL.Rotate(shoulderAngle.x, shoulderAngle.y, shoulderAngle.z);
        //ConstrainAngles(TorsoC.Torso.ShoulderL, (int)AxisType.X, -60f, 180f);
        //ConstrainAngles(TorsoC.Torso.ShoulderL, (int)AxisType.Y, -50f, 10f);
        //ConstrainAngles(TorsoC.Torso.ShoulderL, (int)AxisType.Z, -40f, 40f);

        TorsoC.Torso.ShoulderR.Rotate(shoulderAngle.x, -shoulderAngle.y, -shoulderAngle.z);
        //ConstrainAngles(TorsoC.Torso.ShoulderR, (int)AxisType.X, -60f, 180f);
        //ConstrainAngles(TorsoC.Torso.ShoulderR, (int)AxisType.Y, -10f, 50f);
        //ConstrainAngles(TorsoC.Torso.ShoulderR, (int)AxisType.Z, -40f, 30f);
       
        //claviclesX
        TorsoC.Torso.ClavicleL.Rotate(clavicleAngle.x, clavicleAngle.y, clavicleAngle.z);
        ////ConstrainAngles(TorsoC.Torso.ClavicleL, (int)AxisType.X, 20f, 180f);
        //ConstrainAngles(TorsoC.Torso.ClavicleL, (int)AxisType.Y, -90f, 100f);
        //ConstrainAngles(TorsoC.Torso.ClavicleL, (int)AxisType.Z, -10f, 180f);
      
        TorsoC.Torso.ClavicleR.Rotate(clavicleAngle.x, -clavicleAngle.y, -clavicleAngle.z);
        //ConstrainAngles(TorsoC.Torso.ClavicleR, (int)AxisType.X, 0f, 270f);
        //ConstrainAngles(TorsoC.Torso.ClavicleR, (int)AxisType.Y, -20f, 120f);
        //ConstrainAngles(TorsoC.Torso.ClavicleR, (int)AxisType.Z, 90f, -90f);
        
   
        //pelvis

      
        TorsoC.Torso.PelvisL.Rotate(pelvisLAngle.x, pelvisLAngle.y, pelvisLAngle.z);

        TorsoC.Torso.PelvisR.Rotate(pelvisRAngle.x, -pelvisRAngle.y, -pelvisRAngle.z);


        TorsoC.Torso.KneeL.Rotate(kneeAngle.x, kneeAngle.y, kneeAngle.z);

        TorsoC.Torso.KneeR.Rotate(kneeAngle.x, kneeAngle.y, kneeAngle.z);


        //Warning: This can only be done if models are facing a specific direction!
        //Toes are rotated in world space because their local axes are skewed
        TorsoC.Torso.ToeL.Rotate(toeAngle.x, toeAngle.y, toeAngle.z, Space.World);

        //Toes are rotated in world space because their global axes are more correct in the model
        TorsoC.Torso.ToeR.Rotate(toeAngle.x, toeAngle.y, toeAngle.z, Space.World);

        //Hips are rotated in world space because their local axes are skewed

       // if (GetComponent<AnimationInfo>().AnimName.Contains("Knock") || GetComponent<AnimationInfo>().AnimName.Contains("knock")) {
       //     TorsoC.Torso.Hips.Rotate(hipAngle.x, -110, 0, Space.World);          
       // }
       // else

        TorsoC.Torso.Hips.Rotate(hipAngle.x, 0, 0, Space.World);
        
        //Feet are rotated in world space because their local axes are skewed
        TorsoC.Torso.FootL.Rotate(-pelvisLAngle.x - kneeAngle.x - hipAngle.x - toeAngle.x, 0, 0, Space.World);
        //TorsoC.Torso.FootL.Rotate(-pelvisLAngle.x - kneeAngle.x + hipAngle.x - toeAngle.x, 0, 0, Space.Self);
        //Feet are rotated in world space because their local axes are skewed
        TorsoC.Torso.FootR.Rotate(-pelvisRAngle.x - kneeAngle.x - hipAngle.x - toeAngle.x, 0, 0, Space.World);
        //TorsoC.Torso.FootR.Rotate(-pelvisRAngle.x - kneeAngle.x + hipAngle.x - toeAngle.x, 0, 0, Space.Self);

        Vector3 groundToe;
        if (TorsoC.Torso.ToeL.position.y < TorsoC.Torso.ToeR.position.y)
            groundToe = TorsoC.Torso.ToeL.position;
        else
            groundToe = TorsoC.Torso.ToeR.position;

        //torso.Torso.Root.position = torso.Torso.InitRootPos - ((torso.Torso.FootL.position + torso.Torso.FootR.position) / 2f - torso.Torso.InitFootPos);
        float groundY = TorsoC.Torso.InitRootPos.y - (groundToe.y - TorsoC.Torso.InitToePos.y);
        //Debug.Log(groundToe.y + " " + TorsoC.Torso.InitToePos.y);
       
        TorsoC.Torso.Root.position = new Vector3(TorsoC.Torso.Root.position.x, groundY, TorsoC.Torso.Root.position.z);

       
    
    }

    //System updates transformations after Update(), so LateUpdate() overrides them
    public void LateUpdate() {

        //if (!animation.isPlaying)
		Animator anim = GetComponent<Animator> ();
		if (!anim.GetCurrentAnimatorStateInfo(0).IsName(_animName))
            return;
        UpdateTransforms();
    }

    void ConstrainAngles(Transform transform, int axis, float minAngle, float maxAngle) {
        Vector3 currAngle = transform.localEulerAngles;
        if(axis == (int)AxisType.X) {

            if (transform.localEulerAngles.x < minAngle || (transform.localEulerAngles.x < minAngle + 360 && transform.localEulerAngles.x > maxAngle + 360))
                transform.localEulerAngles = new Vector3(minAngle, transform.localEulerAngles.y, transform.localEulerAngles.z);
            else if ((transform.localEulerAngles.x > maxAngle  && transform.localEulerAngles.x < (minAngle + 360))||  (transform.localEulerAngles.x > maxAngle + 360))
                transform.localEulerAngles = new Vector3(maxAngle, transform.localEulerAngles.y, transform.localEulerAngles.z);
            
        }
        else if (axis == (int)AxisType.Y) {

            if (transform.localEulerAngles.y < minAngle || (transform.localEulerAngles.y < minAngle + 360 && transform.localEulerAngles.y > maxAngle + 360))
                transform.localEulerAngles = new Vector3(transform.localEulerAngles.x, minAngle, transform.localEulerAngles.z);
            else if ((transform.localEulerAngles.y > maxAngle && transform.localEulerAngles.y < (minAngle + 360))  || (transform.localEulerAngles.x > maxAngle + 360))
                transform.localEulerAngles = new Vector3(transform.localEulerAngles.x, maxAngle, transform.localEulerAngles.z);

        }
        else if (axis == (int)AxisType.Z) {


            if (transform.localEulerAngles.z < minAngle || (transform.localEulerAngles.z < minAngle + 360 && transform.localEulerAngles.z > maxAngle + 360))
                transform.localEulerAngles = new Vector3(transform.localEulerAngles.x, transform.localEulerAngles.y, minAngle);
            else if ((transform.localEulerAngles.z > maxAngle && transform.localEulerAngles.z < (minAngle + 360)) || (transform.localEulerAngles.x > maxAngle + 360))
                transform.localEulerAngles = new Vector3(transform.localEulerAngles.x, transform.localEulerAngles.y, maxAngle);

        }

    }
	
}
