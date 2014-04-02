#define DEBUGMODE
using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
//using UnityEditor;

/*
public enum InterpolatorType { 
    EndEffector,
    ElbowAngle,
    ElbowPos
}
*/

[RequireComponent(typeof(ArmControllerMecanim))]
public class ArmAnimatorMecanim : MonoBehaviour {
	

	public ArmControllerMecanim ArmC;
	//public GameObject trajectory;
	private Vector3 _targetL, _targetR; //current target

	
	private ArmInfo[] _arms;
	private AnimationInfoMecanim _animInfo;

	private string _animName;


#if DEBUGMODE
    private List<Vector3> _targetLPrev, _targetRPrev;
    List<float> _sArr = new List<float>();
   
#endif
    List<float> _velArr = new List<float>();
    List<float> _tppArr = new List<float>();

	public const float Abratio = 2.5f;//1.1f; //shoulde be >1
	public const float MaxdTheta = Mathf.PI/6.0f; //Mathf.PI/20.0f;  //values in Liwei Zhao's thesis
    
	
	//Shape parameters
	public float Hor = 0.02f; //[-1 1]
	public float Ver = 0.03f;
	public float Sag = 0.01f; 
	
	//Effort parameters
	public float Ind = 0f;	
	public float Sus = 0f;
	public float Sud = 0f;
	public float Lgt = 0f;
	public float Str = 0f;
	public float Fre = 0f;
	public float Bnd = 0f;
	public float Dir = 0f;
	
	//Low-level parameters
	public float WbMag; //Wrist bend multiplier
	public float WxMag; //Wrist extension magnitude
	public float Tval;  //Tension	    
	public float DMag;  //Displacement magnitude
	public float EtMag; //Elbow twist magnitude
	public float WtMag; //
	public float EfMag; //Elbow frequency magnitude
	public float WfMag; //
	public float Ti; 	 //Inflection point where the movement changes from accelerating to decelerating
	public float V0; 	 //Start velocity
	public float V1; 	 //End velocity

    public float HrMag; //Head rotation magnitude
    public float HfMag; //Head rotation frequency

    public float TrMag; //Torso rotation magnitude
    public float TfMag; //Torso rotation frequency

    
	public float Texp;  //Default time exponent
    public float SquashMag; //Squash magnitude
    public float SwivelAngle; //Elbow swivel angle
    
    public float T0 = 0.01f; //EMOTE values
    public float T1 = 0.99f; //EMOTE values


    public float Bias = 0f;
    public float Continuity = 0f;
        
    private float _vi; //inflection velocity --cannot be updated

	IKSolver _ikSolver;


    
    public InterpolatorType _interpolatorType;

    public bool DrawGizmos = true;
    StreamWriter _sw ;

    
   
	float Tp {//Normalized time between goal keypoints	
		get {            
            if (_animInfo.Curr <= _animInfo.PrevGoal)
                return 0;
            if (_animInfo.NextGoal == _animInfo.PrevGoal)
                return 1;
            return (float)(_animInfo.Curr - _animInfo.PrevGoal)/ (_animInfo.NextGoal - _animInfo.PrevGoal);
            

		}

	}


	void Awake () {
		ArmC = GetComponent(typeof(ArmControllerMecanim)) as ArmControllerMecanim;
	    if (ArmC != null) _arms = ArmC.Arms;

#if DEBUGMODE		
        _targetRPrev = new List<Vector3>();
        _targetLPrev = new List<Vector3>();
        #endif

        _sArr = new List<float>();
        _velArr = new List<float>();
        _tppArr = new List<float>();

        
	}

    public void Reset(){
        
		_animInfo = GetComponent<AnimationInfoMecanim>(); 
		if (_animInfo == null)
						Debug.Log ("animInfo is null after Reset() in AAM");

    	_animInfo.InitKeyPoints(); //should always update interpolators
        _animInfo.InitInterpolators(Tval, Continuity, Bias);

        _interpolatorType = InterpolatorType.EndEffector;
		// FUNDA Effort2LowLevel(); //initialize low level parameters

        #if DEBUGMODE
        _targetRPrev = new List<Vector3>();
        _targetLPrev = new List<Vector3>();
        #endif

         _velArr = new List<float>();
        _tppArr = new List<float>();


    }

    public void ResetParameters() {
        SetSpeed(2f);
        V0 = V1 = 0f;
        Ti = 0.5f;
        Texp = 1.0f;
        Tval = 0f;
        T0 = 0.01f;
        T1 = 0.99f;
        TrMag = TfMag = 0f;
        HrMag = HfMag = 0f;
        SquashMag = 0f;
        WbMag = 0f;
        WxMag = WtMag = WfMag = 0f;
        EtMag = DMag = EfMag = 0f;
        _interpolatorType = InterpolatorType.EndEffector;
    }

    public void UpdateInterpolators() {
        float tp;
        float[] newKeyTimes = new float[_animInfo.Keys.Length];
        float[] originalKeyTimes = new float[_animInfo.Keys.Length];
        for (int i = 0; i < _animInfo.Keys.Length; i++) {

            int prevGoalKeyInd = _animInfo.FindPrevGoalAtTime(_animInfo.Keys[i].Time);
            int nextGoalKeyInd = prevGoalKeyInd + 1;
            if (nextGoalKeyInd > _animInfo.GoalKeys.Length - 1)
                nextGoalKeyInd = _animInfo.GoalKeys.Length - 1;

            int prevGoal = _animInfo.GoalKeys[prevGoalKeyInd];
            int nextGoal = _animInfo.GoalKeys[nextGoalKeyInd];
            

            if (_animInfo.Keys[i].Time <= _animInfo.Keys[prevGoal].Time)
                tp =  0;
            else if (_animInfo.Keys[nextGoal].Time == _animInfo.Keys[prevGoal].Time)
                tp = 1;
            else {
                tp = (_animInfo.Keys[i].Time - _animInfo.Keys[prevGoal].Time) / (_animInfo.Keys[nextGoal].Time - _animInfo.Keys[prevGoal].Time);
            }

            
            float s = TimingControl(tp);

            //map s into the whole spline        		   
            float t = (s * (_animInfo.Keys[nextGoal].Time - _animInfo.Keys[prevGoal].Time) + _animInfo.Keys[prevGoal].Time);
            newKeyTimes[i] = t;

          
        }
        
        //Record original keytimes
        for (int i = 0; i < _animInfo.Keys.Length; i++) {
            originalKeyTimes[i] = _animInfo.Keys[i].Time;
            _animInfo.Keys[i].Time = newKeyTimes[i];
        }

        //Update interpolators
        _animInfo.InitInterpolators(Tval, Continuity, Bias);
        //Reset key times back
         for (int i = 0; i < _animInfo.Keys.Length; i++)
             _animInfo.Keys[i].Time = originalKeyTimes[i];
        

    }

	public void changeAnim(string name) {
		_animName = name;
	}
    
    //Has to be lateupdate because we overwrite the transforms
    void LateUpdate() {
		return;
       //if (!animation.isPlaying)         
		Animator anim = GetComponent<Animator> ();
		if (!anim.GetCurrentAnimatorStateInfo(0).IsName(_animName))
            return;

        _ikSolver = new IKArm();
	    int arm ; //arm index
	    int keyInd;

        

        float s = TimingControl(Tp);

       

       //map s into the whole spline        		   
        float t = (s * (_animInfo.NextGoal - _animInfo.PrevGoal) + _animInfo.PrevGoal) / _animInfo.AnimLength;

     

       
  //     Debug.Log(s + " " + Tp + " " + t +  " " + Time.deltaTime);
       if (_animInfo.NextGoal == _animInfo.PrevGoal)             
           t = 1f;

       if(t < 0) 
           keyInd = _animInfo.FindKeyNumberAtNormalizedTime(-t); //find an imaginary key before the start of keyframes         
       else if (t > 1) 
           keyInd =  _animInfo.FindKeyNumberAtNormalizedTime(2 - t); //find an imaginary key beyond the keyframes   1 - ( t - 1)       
       else 
           keyInd = _animInfo.FindKeyNumberAtNormalizedTime(t); //including via keys


      // if (this.gameObject == GameObject.Find("AgentPrefab"))
       //    Debug.Log(s + " " + t);
       //if (this.gameObject == GameObject.Find("AgentPrefab"))
        //   Debug.Log(keyInd);

       float lt; //local time between keyframes including via keys
         
       if (keyInd + 1 < _animInfo.Keys.Length) {            
           if(t < 0 )
               lt = (float)(-t *_animInfo.AnimLength - _animInfo.Keys[keyInd].Time) / (_animInfo.Keys[keyInd + 1].Time - _animInfo.Keys[keyInd].Time);            
           else if (t > 1)
               lt = (float)((2 - t) * _animInfo.AnimLength - _animInfo.Keys[keyInd].Time) / (_animInfo.Keys[keyInd + 1].Time - _animInfo.Keys[keyInd].Time);            
           else
               lt = (float)(t * _animInfo.AnimLength - _animInfo.Keys[keyInd].Time) / (_animInfo.Keys[keyInd + 1].Time - _animInfo.Keys[keyInd].Time);            
       }
       else
           lt = 0f;

       

    
        //   if (this.gameObject == GameObject.Find("AgentPrefab"))
       //       Debug.Log("t: " + t + " lt: " + lt + " keyInd: " + keyInd + " frameInd:  " + frameInd + " Curr: " + _animInfo.Curr + " PrevGoal: " + _animInfo.PrevGoal  + " NextGoal: " + _animInfo.NextGoal  + " prevGoalInd " + _animInfo.PrevGoalKeyInd    + " Tp: " + Tp);
        _animInfo.InterpolateWholeBody(keyInd, lt); //linear interpolation
        _animInfo.ProjectWholeBodyBeyondKeyInd(keyInd, lt, t); //linear interpolation
		
        //update both arms
        for (arm = 0; arm < 2; arm++) {
            if (_interpolatorType == InterpolatorType.EndEffector) {//position
                Vector3 target = _animInfo.ComputeInterpolatedTarget(lt, keyInd, arm);

                if (t < 0) { // project target to a position before position at keyInd                
                    Vector3 pivot = _animInfo.ComputeInterpolatedTarget(0f, 0,arm); //TCB interpolation for position
                    target = 2 * pivot - target;
                }
                else if (t > 1) { // project target to a position beyond keyInd
                    Vector3 pivot = _animInfo.ComputeInterpolatedTarget(0, _animInfo.Keys.Length - 1,arm); //TCB interpolation  for position
                    target = 2 * pivot - target;
                    
            //        Debug.Log(Time.time + "  "+ t);
                }


                if (arm == 0)            
                    _targetL = target;
            
                else 
                    _targetR = target;

                              
                        
        #if DEBUGMODE
                    //To see tension
                    if(arm == 0) 
                        _targetLPrev.Add(_targetL);
                    else
                        _targetRPrev.Add(_targetR);
        #endif
                    
                    if(arm == 0)               
                        ((IKArm)_ikSolver).Solve(_arms[arm].Bones, -SwivelAngle, target);         			    
                    else
                        ((IKArm)_ikSolver).Solve(_arms[arm].Bones, SwivelAngle, target);
                
                    
                            
            }

            else if (_interpolatorType == InterpolatorType.ElbowAngle)
            {

             //   _arms[arm].Elbow.transform.localRotation = Quaternion.Slerp(Keys[keyInd].BodyRot[i], Keys[keyInd + 1].BodyRot[i], lt);
                _arms[arm].Elbow.transform.localRotation = _animInfo.ComputeInterpolatedElbowAngle(lt, keyInd, arm);

                //Vector3 targetElbowAngle = _animInfo.ComputeInterpolatedElbowAngle(lt, keyInd, arm);
                //_arms[arm].Elbow.transform.localEulerAngles = targetElbowAngle;



            }
            /*
        else if (_interpolatorType == InterpolatorType.ElbowPos) {
            _arms[arm].Elbow.position = _animInfo.ComputeInterpolatedElbowPos(lt, keyInd,arm); //TCB interpolation for position   

        }
        */


            Flourish(arm, Tp, t);       
            //Flourish(arm, Mathf.Pow(tp, texp));   //Funda
            /*
            
            if (this.gameObject == GameObject.Find("AgentPrefab")) {
                
                if (Math.Abs(lt - 0) < 0.001) { //at keyframes
                    _animInfo.Keys[keyInd].EePosUpdated[arm] = target;
                    _animInfo.Keys[keyInd].TimeUpdated[arm] = Time.time;
                   
                }
                if (t > 1f) {
                    _animInfo.ComputeEeVelUpdated(arm);
                }
                
                    
                
            }
            */    
        }

        
            
        if (this.gameObject != GameObject.Find("AgentControlPrefab")) {
            
            if (Tp == 0) {
                _velArr.Clear();
                _tppArr.Clear();
            }
            //Current velocity curve
            GameObject velCurveCurr = GameObject.Find("VelCurveCurr");
            if (velCurveCurr == null) {
                return;
            }
            velCurveCurr.GetComponent<LineRenderer>().SetVertexCount(_velArr.Count);
            for(int i = 0; i < _velArr.Count; i++) {
                velCurveCurr.GetComponent<LineRenderer>().SetPosition(i, new Vector3(_tppArr[i], _velArr[i], 0));                
            }
            //General velocity curve as in EMOTE
            GameObject velCurveGen = GameObject.Find("VelCurveGeneral");
            velCurveGen.GetComponent<LineRenderer>().SetVertexCount(5);

            velCurveGen.GetComponent<LineRenderer>().SetPosition(0, new Vector3(0, 0, 0));
            velCurveGen.GetComponent<LineRenderer>().SetPosition(1, new Vector3(T0 , -V0, 0));
            velCurveGen.GetComponent<LineRenderer>().SetPosition(2, new Vector3(Ti , _vi, 0));
            velCurveGen.GetComponent<LineRenderer>().SetPosition(3, new Vector3(T1 , -V1, 0));
            velCurveGen.GetComponent<LineRenderer>().SetPosition(4, new Vector3(1 , 0, 0));
            /*velCurveGen.GetComponent<LineRenderer>().SetPosition(1, new Vector3( T0 / _animInfo.AnimSpeed, -V0, 0));
            velCurveGen.GetComponent<LineRenderer>().SetPosition(2, new Vector3( Ti / _animInfo.AnimSpeed, _vi, 0));
            velCurveGen.GetComponent<LineRenderer>().SetPosition(3, new Vector3( T1 / _animInfo.AnimSpeed, -V1, 0));
            velCurveGen.GetComponent<LineRenderer>().SetPosition(4, new Vector3( 1 / _animInfo.AnimSpeed, 0, 0));
            */
        }

	}



    public void UpdateKeypointsByShape(int arm) {
		if(_animInfo == null)
			return;        
	
		for(int i = 0; i < _animInfo.Keys.Length; i++) {

			//initialize to original positions
			_animInfo.Keys[i].EePos[arm] = _animInfo.Keys[i].EePosOrig[arm];
            _animInfo.Keys[i].ShoulderPos[arm] = _animInfo.Keys[i].ShoulderPosOrig[arm];
            //Give shoulder position as a reference

            
           // Debug.Log(shoulderPos);
			_animInfo.Keys[i].EePos[arm] = ArmShape(arm, _animInfo.Keys[i].EePos[arm], _animInfo.Keys[i].ShoulderPos[arm]);
                
		}

        _animInfo.InitInterpolators(Tval, Continuity, Bias);
		
	}

    
    
	
	/// <summary>
	/// Update Arm Shape
	/// target: Keypoint to modify
	/// Returns modified keypoint
	/// </summary>
	Vector3  ArmShape(int arm, Vector3 target, Vector3 shoulderPos) {
	    float rotTheta = 0f;
        Vector3 centerEllipse;
	    //Transform target from world space to local EMOTE coordinates
	//	targetLocal = transform.InverseTransformPoint(target);	
		//Translate to world
	
        //Vector3 targetLocal = target - _arms[arm].Shoulder.position;		

     
        
        Vector3 targetLocal = target - shoulderPos;


      //  Debug.Log(targetLocal);
		//Rotate to emote	
		targetLocal = new Vector3(targetLocal.y, -targetLocal.z, targetLocal.x);
		
		//hor				
		float theta = Mathf.Atan(Abratio* targetLocal.y / -targetLocal.z);
        
		if(-targetLocal.z < 0)
			theta += Mathf.PI;
		if(theta < 0)
			theta += 2* Mathf.PI;
		
		float a = -targetLocal.z / Mathf.Cos(theta);


        

	    if(Hor == 0){
			// WRONG! rotTheta = 0f;
			rotTheta = theta;
		}
		else if(Hor < 0f && 0 < theta && theta <= Mathf.PI){			
			rotTheta = Mathf.Min(theta - Hor * MaxdTheta, Mathf.PI);
		}
		else if(Hor < 0f &&   Mathf.PI< theta && theta <= 2* Mathf.PI){			
			rotTheta = Mathf.Max(theta + Hor * MaxdTheta, Mathf.PI);
		}
		else if(Hor > 0f && 0 < theta && theta <= Mathf.PI){		
			rotTheta = Mathf.Max(theta - Hor * MaxdTheta, 0);
		}
		else if(Hor > 0f &&   Mathf.PI< theta && theta <= 2* Mathf.PI){					
			rotTheta = Mathf.Min(theta + Hor * MaxdTheta,2*Mathf.PI );
		}


	    float hdz = -(a * Mathf.Cos(rotTheta))-  targetLocal.z;
		float hdy = (a * Mathf.Sin(rotTheta)/Abratio) -  targetLocal.y;



        

		//sag
		theta  = Mathf.Atan(Abratio* targetLocal.x / -targetLocal.y);
		
		
		if(targetLocal.y < 0)
			theta += Mathf.PI;
		if(theta < 0)
			theta += 2* Mathf.PI;

     
		
		a = targetLocal.y / Mathf.Cos(theta);
		
		if(Sag == 0){
			// WRONG! rotTheta = 0f;
			rotTheta = theta;
		}
		else if(Sag < 0f && 0 < theta && theta <= Mathf.PI){			
			rotTheta = Mathf.Min(theta - Sag * MaxdTheta, Mathf.PI);
		}
		else if(Sag < 0f &&   Mathf.PI< theta && theta <= 2* Mathf.PI){			
			rotTheta = Mathf.Max(theta + Sag * MaxdTheta, Mathf.PI);
		}
		else if(Sag > 0f && 0 < theta && theta <= Mathf.PI){		
			rotTheta = Mathf.Max(theta - Sag * MaxdTheta, 0);
		}
		else if(Sag > 0f &&   Mathf.PI< theta && theta <= 2* Mathf.PI){					
			rotTheta = Mathf.Min(theta + Sag * MaxdTheta,2*Mathf.PI );
		}


	    float sdx = -(a * Mathf.Sin(rotTheta)/Abratio)-  targetLocal.x;
		float sdy = (a * Mathf.Cos(rotTheta)) -  targetLocal.y;


		//ver
		theta  = Mathf.Atan(-Abratio* targetLocal.z/ -targetLocal.x);
		if(-targetLocal.x < 0)
			theta += Mathf.PI;
		if(theta < 0)
			theta += 2* Mathf.PI;
		
		a = -targetLocal.x / Mathf.Cos(theta);
		
		
		if(Ver == 0){
			// WRONG! rotTheta = 0f;
			rotTheta = theta;
		}
		else if(Ver < 0f && 0 < theta && theta <= Mathf.PI) {			
			rotTheta = Mathf.Min(theta - Ver * MaxdTheta, Mathf.PI);
		}
		else if(Ver < 0f &&   Mathf.PI< theta && theta <= 2* Mathf.PI) {			
			rotTheta = Mathf.Max(theta + Ver * MaxdTheta, Mathf.PI);
		}
		else if(Ver > 0f && 0 < theta && theta <= Mathf.PI) {		
			rotTheta = Mathf.Max(theta - Ver * MaxdTheta, 0);
		}
		else if(Ver > 0f &&   Mathf.PI< theta && theta <= 2* Mathf.PI) {					
			rotTheta = Mathf.Min(theta + Ver * MaxdTheta,2*Mathf.PI );
		}


	    float vdx = -(a * Mathf.Cos(rotTheta))-  targetLocal.x;
		float vdz = -(a * Mathf.Sin(rotTheta)/Abratio)-  targetLocal.z;
		
		if(Mathf.Abs(sdx) < 0.0001f) sdx = 0f;
		if(Mathf.Abs(sdy) < 0.0001f) sdy = 0f;
		if(Mathf.Abs(vdx) < 0.0001f) vdx = 0f;
		if(Mathf.Abs(vdz) < 0.0001f) vdz = 0f;
		if(Mathf.Abs(hdy) < 0.0001f) hdy = 0f;
		if(Mathf.Abs(hdz) < 0.0001f) hdz = 0f;
		
		
		//Update keypoint position

        if (arm == 1) {
            sdx = -sdx;            
        }
		
		targetLocal.x += sdx + vdx;
		targetLocal.y += sdy + hdy;
		targetLocal.z += hdz + vdz;

		
		//Transform target from local EMOTE space to world coordinates
		
	//	target = transform.TransformPoint(targetLocal);
		
		//Convert back to unity coordinate system
		targetLocal = new Vector3(targetLocal.z, targetLocal.x, -targetLocal.y);


		//Translate back to world coordinate
		//target = targetLocal + _arms[arm].Shoulder.position;					
        target = targetLocal + shoulderPos;					
        
		return target;
		
	}
	
   

    public void SetSpeed(float speed) {               
       //funda v0 = v1 = 1.0f;
        //if (animation.isPlaying) //open in demodrives close in democomparison
		if (GetComponent<Animator>().GetCurrentAnimatorStateInfo(0).IsName(_animName))
          _animInfo.AnimSpeed = speed;
        
    }
	public void Effort2LowLevel() {

   //     Tval = (-1f * Ind + Mathf.Min(Ind, Fre)) + Dir; //EMOTE and chi
        Tval = (-1f * Mathf.Max(Ind, Fre)) + Mathf.Max( Dir, Bnd); //Funda
        				
	//	WbMag = Mathf.Max(0.6f * ind, Mathf.Max(0.5f * lgt, 1.0f * fre) ); //In Chi's thesis
		WbMag = Mathf.Max(0.6f * Ind, Mathf.Max(0.5f * Lgt, 0.4f * Fre) ); //In emote paper
					
		//WxMag = -0.3f * lgt + (0.2f *  fre - 0.8f * Mathf.Min(str,  fre )); //In Chi's thesis
		WxMag = -0.3f * Lgt + (0.3f *  Fre - 0.9f * Mathf.Min(Str,  Fre )); //In emote paper
		
		DMag = EtMag = WtMag = 0.4f  *  Ind;
		
		EfMag = WfMag = 2f *Ind;

        Texp = 1f + 2f* Sud + (0.2f * Mathf.Min(Str, Sud) - 0.6f* Mathf.Min(Fre, Sud)) - 0.2f * Mathf.Max(Str, 0.5f* Mathf.Min(Dir, Sus)) - 0.4f * Fre - 0.1f * Mathf.Min(Ind, Fre); //chi
      //  Texp = 1f + 2f * Sud + (0.2f * Mathf.Min(Str, Sud) - 0.1f * Mathf.Min(Fre, Sud)) - 0.2f * Mathf.Max(Str, Mathf.Min(Dir, Sus)) - 0.4f * Fre - 0.5f * Mathf.Min(Ind, Fre); //emote ==> causes strange behaviors in ind + fre
     

		Ti  = 0.5f + 0.4f * Mathf.Max(Str, Sud) - 0.4f * Mathf.Max(Lgt, Sus) + 0.8f * Mathf.Min(Bnd, Lgt); //both


    //    V0 = 0.1f * Str - Mathf.Max(0.06f * Mathf.Min(Sus, Str), 0.1f * Mathf.Min(Fre, Str));
        V0 = 0.2f * Str - Mathf.Max(0.06f * Mathf.Min(Sus, Str), 0.1f * Mathf.Min(Fre, Str)); //funda

		V1 = Mathf.Max(0.03f * Mathf.Max( Lgt ,  Sus ), 0.2f * Fre - 0.1f * Mathf.Min( Ind  ,  Fre  ));
        

        T0 = 0.2f * Str + 0.01f;

        SquashMag = Mathf.Max(0.2f * Lgt, 0.15f * Fre);

        //speed = 2 in neutral state

        //TODO: Bound also affects speed
        SetSpeed(2+  Mathf.Max(Sud, 0.5f * Str) - Mathf.Max( Sus, 0.2f * Lgt, 0.2f * Bnd) );
        _interpolatorType = InterpolatorType.EndEffector;
        /*
        if (fre > 0)
            _interpolatorType = InterpolatorType.elbowAngle;
        else if (ind > 0 )
            _interpolatorType = InterpolatorType.elbowPos;
        else
            _interpolatorType = InterpolatorType.endEffector;
        */
    }
	
	float TimingControl(float tp) {

        float area1 = 0f, area2 = 0f, area3  = 0f;
        float vel;
        float tpp = Mathf.Pow(tp, Texp);        
        float tpp2 = tpp * tpp;        
        float t02 = T0 * T0;
        float t12 = T1 * T1;
        float ti2 = Ti * Ti;
        float s = 0f;

        
        if (T0 == T1)
            _vi = 0f;
        else
          //  _vi = (2f + 2f * V1 * T1 - V1 + V0 * Ti - V1 * Ti) / (T1 - T0);
            _vi = (2f + V1 + V0 * Ti - V1 * Ti) / (T1 - T0);



	    area1 = -0.5f * V0 * T0;
        if(T0 == Ti)
            area2 = area1;            
        else
            area2 = area1 + (-0.5f * (V0 + _vi) * ti2 + (V0 * Ti + T0 * _vi) * Ti - (-0.5f * (V0 + _vi) * t02 + (V0 * Ti + T0 * _vi) * T0)) / (T0 - Ti);
            
        if(T1 == Ti)
            area3 = area2;
        else
            area3 = area2 + (-0.5f * (V1 + _vi) * t12 + (V1 * Ti + T1 * _vi) * T1 - (-0.5f * (V1 + _vi) * ti2 + (V1 * Ti + T1 * _vi) * Ti)) / (T1 - Ti);


	    //Compute s
        if (tpp >= 0f && tpp < T0) {            
            vel = (-V0 / T0) * tpp;
            s = 0.5f * (-V0 / T0) * tpp2;            
        }
        else if (tpp >= T0 && tpp < Ti) {
            vel = (-(V0 + _vi) * tpp + V0 * Ti + T0 * _vi) / (T0 - Ti);            
            s = area1 + (-(V0 + _vi) * tpp2 * 0.5f + (V0 * Ti + T0 * _vi) * tpp - (-(V0 + _vi) * t02 * 0.5f + (V0 * Ti + T0 * _vi) * T0)) / (T0 - Ti);            
        }
        else if (tpp >= Ti && tpp < T1) {            
            vel = (-(V1 + _vi) * tpp + V1 * Ti + T1 * _vi) / (T1 - Ti);
            s = area2 + (-(V1 + _vi) * tpp2 * 0.5f + (V1 * Ti + T1 * _vi) * tpp - (-(V1 + _vi) * ti2 * 0.5f + (V1 * Ti + T1 * _vi) * Ti)) / (T1 - Ti);
        }
        else if (tpp >= T1 && tpp < 1f) {            
            vel = (-V1 * tpp + V1) / (T1 - 1f);            
            s = area3 + (-V1 * tpp2 * 0.5f + V1 * tpp - (-V1 * t12 * 0.5f + V1 * T1)) / (T1 - 1f);
            
        }
        else if (tpp == 1f) {            
            s = area3;
            vel = 0f;            
        }

        else 
            vel = s = 0f;

        
        _velArr.Add(vel);
        _tppArr.Add(tpp);

	    // If we do this, we lose anticipation and overshoot
		//if(s < 0.0001f)
		//	s = 0f;
        //if (s > 1.0f)
         //   s = 1.0f;


		return s;
     
		
				
	}
	//Tn = normalized time between keypoints
    //t = normalized time between initial and final keys
	void Flourish(int arm, float tn, float t) {		

		float wristBend;
	    float elbowAngle = 0f;
	    float squash = 1 + ( SquashMag * Mathf.Sin(Mathf.Pow(tn, 1.6f) * Mathf.PI ));


        //related to torso
         float breath = 1 - (Mathf.Pow(Mathf.Sin((Mathf.PI/2 * Mathf.PI / 2) * (tn - 0.4f)), 2) / 7);

		TorsoControllerMecanim torso = GetComponent(typeof(TorsoControllerMecanim)) as TorsoControllerMecanim;


        float headRot = HrMag * Mathf.Sin(HfMag * Mathf.PI * t);
        float torsoRot = TrMag * Mathf.Sin(TfMag * Mathf.PI * t);
  

	    if (torso != null) {
	        torso.Torso.Spine1.transform.localScale = new Vector3(1, 1, squash);
	        torso.Torso.Neck.transform.localScale = new Vector3(1, 1, 1/squash); //correct neck scale 

	        torso.Torso.Spine1.transform.localScale = new Vector3(1, breath, 1);
	        torso.Torso.Neck.transform.localScale = new Vector3(1, 1/breath, 1); //correct neck scale 

            //Head and neck rotation
            torso.Torso.Head.transform.Rotate(0, 0, Mathf.Rad2Deg * headRot);
            torso.Torso.Neck.transform.Rotate(0, 0, Mathf.Rad2Deg * headRot);
            
            //Spine rotation
            torso.Torso.Spine.transform.Rotate(0, 0, Mathf.Rad2Deg * torsoRot);

            
            
	    }
        

	    //rotate wrist around the x-axis in local coordinate system	(x-axis in EMOTE coordinate system)	
      //  if (WbMag == 0)  //FUNDA commented out
        //    wristBend = 0.6f; //In EMOTE
        //else
            wristBend = WbMag * (Mathf.Sin(2f * Mathf.PI * (tn + 0.75f)) + 1f - WxMag);

        if (arm == 1)
            wristBend *= -1;
		
		//rotation of wrist around the y-axis	(z-axis in EMOTE coordinate system)			
		float wristTwist = WtMag * Mathf.Sin( WfMag * Mathf.PI * tn);		
		
	
        if (arm == 1)
            wristTwist *= -1;
	
        _arms[arm].Wrist.transform.Rotate(0,Mathf.Rad2Deg * wristBend, 0);


		_arms[arm].Wrist.transform.Rotate(0, Mathf.Rad2Deg * wristTwist, 0);
		
		//rotate elbow  around the y-axis	(z-axis in EMOTE coordinate system)	
		
		float elbowTwist = EtMag * Mathf.Sin( EfMag * Mathf.PI * tn);		
	
         if (arm == 1)
            elbowTwist *= -1;
		
	
		// Elbow displacement angle		
        //elbowAngle *= (1f + DMag * Mathf.Sin(2 * Mathf.PI * tn));
         elbowAngle = (elbowAngle + DMag * Mathf.Sin(2 * Mathf.PI * tn));
        //Debug.Log(elbowAngle);

 
        if (arm == 0 && elbowTwist < 0 || arm == 1 && elbowTwist > 0)
            elbowTwist = 0f;
                
        _arms[arm].Elbow.transform.Rotate(0,Mathf.Rad2Deg * elbowTwist,0);

        _arms[arm].Elbow.transform.Rotate(Mathf.Rad2Deg * elbowAngle, 0, 0);
	}


	void OnDrawGizmos() {
        if(DrawGizmos) {

	        Gizmos.color = Color.yellow;
   		    Gizmos.DrawSphere (_targetL, 0.01f);
		    Gizmos.DrawSphere (_targetR, 0.01f);


            #if DEBUGMODE
            Gizmos.color = Color.magenta;
            
			if (GetComponent<Animator>().GetCurrentAnimatorStateInfo(0).IsName(_animName)) {
            	//if(animation.isPlaying && _targetRPrev.Count > 1) {
				if(_targetRPrev.Count > 1) {

                	for (int i = 0; i < _targetRPrev.Count - 1; i++) {

                	//    Gizmos.DrawSphere(_targetRPrev[i], 0.001f);
                    Gizmos.DrawLine(_targetRPrev[i], _targetRPrev[i + 1]);
                	}
            	}
			}
            #endif        
            

            //Draw the velocity curves
            if (_animInfo) {
                Gizmos.DrawLine(new Vector3(0, 0, 0), new Vector3(T0 / _animInfo.AnimSpeed, -V0, 0)); //[0 t0]
                Gizmos.DrawLine(new Vector3(T0 / _animInfo.AnimSpeed, -V0, 0), new Vector3(Ti / _animInfo.AnimSpeed, _vi, 0)); //[t0 ti]
                Gizmos.DrawLine(new Vector3(Ti / _animInfo.AnimSpeed, _vi, 0), new Vector3(T1 / _animInfo.AnimSpeed, -V1, 0)); //[ti t1]
                Gizmos.DrawLine(new Vector3(T1 / _animInfo.AnimSpeed, -V1, 0), new Vector3(1 / _animInfo.AnimSpeed, 0, 0)); //[t1 1]

                
               /* for (int i = 0; i < _animInfo.Keys.Length; i++) {
                    Gizmos.color = Color.red;
                    Gizmos.DrawSphere(_animInfo.Keys[i].EePos[1], 0.01f);
                    Gizmos.DrawSphere(_animInfo.Keys[i].ShoulderPos[1], 0.01f);
                }*/

                /*
                for (int i = 0; i < _animInfo.Keys.Length - 2; i++) {
                    float velCurr = (_animInfo.Keys[i + 1].EePos[1] - _animInfo.Keys[i].EePos[1]).magnitude / (_animInfo.Keys[i + 1].Time - _animInfo.Keys[i].Time);
                    float velNext = (_animInfo.Keys[i + 2].EePos[1] - _animInfo.Keys[i + 1].EePos[1]).magnitude / (_animInfo.Keys[i + 2].Time - _animInfo.Keys[i + 1].Time);
                    Gizmos.DrawLine(new Vector3(_animInfo.Keys[i].Time / _animInfo.AnimLength / _animInfo.AnimSpeed, velCurr, 0), new Vector3(_animInfo.Keys[i + 1].Time / _animInfo.AnimLength / _animInfo.AnimSpeed, velNext, 0));
                }

                int n = _animInfo.Keys.Length - 1;
                float velCurrN = (_animInfo.Keys[n - 1].EePos[1] - _animInfo.Keys[n - 2].EePos[1]).magnitude / (_animInfo.Keys[n - 1].Time - _animInfo.Keys[n - 2].Time);
                float velNextN = (_animInfo.Keys[n].EePos[1] - _animInfo.Keys[n - 1].EePos[1]).magnitude / (_animInfo.Keys[n].Time - _animInfo.Keys[n - 1].Time);
                Gizmos.DrawLine(new Vector3(_animInfo.Keys[n - 1].Time / _animInfo.AnimLength / _animInfo.AnimSpeed, velCurrN, 0), new Vector3(_animInfo.Keys[n].Time / _animInfo.AnimLength / _animInfo.AnimSpeed, velNextN, 0));
                */


                //Frame number to time
          //      for (int i = 0; i < _animInfo.Keys.Length - 1; i++)
            //        Gizmos.DrawLine(new Vector3(_animInfo.Keys[i].Time, i, 0), new Vector3(_animInfo.Keys[i + 1].Time, i + 1, 0));

            }

            
            //Draw the position curves
      //      for (int i = 1; i < _sArr.Count; i++)
        //        Gizmos.DrawLine(new Vector3((i - 1) / (float)_sArr.Count, _sArr[i - 1], 0), new Vector3(i / (float)_sArr.Count, _sArr[i], 0));
            
        }
	}




    
}
