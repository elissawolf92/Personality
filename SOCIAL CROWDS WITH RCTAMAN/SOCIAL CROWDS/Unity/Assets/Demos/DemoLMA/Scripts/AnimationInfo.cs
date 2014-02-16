//#define EDITORMODE

using UnityEngine;
using System.Collections.Generic;
using System.IO;




#if EDITORMODE
using UnityEditor;
#endif

[System.Serializable]
public class KeyInfo  {
	public int FrameNo;
    public float Time;
    public bool IsGoal; //either via or goal    
	public Vector3[] EePos = new Vector3[2]; 	  			//end effector key position	
    public Quaternion[] ElbowRot = new Quaternion[2]; 	  			//elbow rotation at keypoint	
    public Vector3[] ElbowPos = new Vector3[2]; 	  			//elbow pos at keypoint	
	public Vector3[] EePosOrig  = new Vector3[2]; 	 //original end effector key position before modified by armshape    
    public Quaternion[] ElbowRotOrig = new Quaternion[2]; 	 //original elbow key rotation
    public Vector3[] ElbowPosOrig = new Vector3[2]; 	 //original elbow key position
    public Vector3[] ShoulderPos = new Vector3[2]; 	  			//shoulder key position	
    public Vector3[] ShoulderPosOrig = new Vector3[2]; 	  			//shoulder key position	
    
    
    public Vector3[] EeVel = new Vector3[2]; //velocity of the end effector	
    public Vector3[] EeAcc = new Vector3[2]; //acceleration of the end effector	
    public Quaternion ClavicleLRot, ClavicleRRot, NeckRot, SpineRot, Spine1Rot, ElbowLRot, ElbowRRot; //TO BE DELETED
    public Vector3 ClavicleLPos, ClavicleRPos, NeckPos, SpinePos, Spine1Pos, ElbowLPos, ElbowRPos;  //TO BE DELETED
    

    public List<Vector3> BodyPos;
    public List<Quaternion> BodyRot;

    public Vector3[] EeVelUpdated = new Vector3[2]; //velocity of the end effector	
    public Vector3[] EePosUpdated = new Vector3[2]; 	  			//end effector key position	
    public float[] TimeUpdated = new float[2];
}
[System.Serializable]
public class MyKeyframe {
   public float Time { get; set; }
}

public class AnimationInfo : MonoBehaviour {
	
	public float Fps;
	public float AnimLength;
	public KeyInfo[] Keys;


 //   public int PrevGoalKeyInd = 0;
//	public int NextGoalKeyInd = 1;

    public float AnimSpeed;

    public float DefaultAnimSpeed = 2f;

    public TCBSpline SIKeyTime; //Keyframe to time interpolator
    public TCBSpline[] SIee = new TCBSpline[2]; //End effector position interpolator
    public TCBSpline[] SIElbowAngle  = new TCBSpline[3]; //Elbow angle rotation interpolator
    //public TCBSpline[] SIElbowPos = new TCBSpline[2]; //Elbow position interpolator
	
    
	
	public string AnimName ="";
	public int FrameCnt;

    public bool IsContinuous; // trigger-based vs continuous running of animations

	[SerializeField]
    
    public List<Transform> BodyChain;

    //because editor does not work here we cannot get keyframe information
	public int[] GoalKeys ;  //keeps the keyframe number (not actual index)  we need to include the start and end keyframes {0, 3, 5, 8};
    public float[] MyKeyTimes; 

    
    //current animation time
    public float Curr {
        get {
            if (IsContinuous && animation[AnimName].time > animation[AnimName].length)
                animation[AnimName].time = 0f;

            return animation[AnimName].time;            
        }
    }

    public int PrevGoalKeyInd {
        get {
            if (Curr >= Keys[GoalKeys[GoalKeys.Length - 1]].Time) {
                if (IsContinuous)
                    return 0;
                else
                    return GoalKeys.Length - 1;
                    //return GoalKeys[GoalKeys.Length - 1];

            }
            for (int i = 0; i < GoalKeys.Length - 1; i++) {
                if (Curr >= Keys[GoalKeys[i]].Time && Curr < Keys[GoalKeys[i + 1]].Time)
                    return i;
                    //return GoalKeys[i];
            }


            throw new System.Exception("Unable to compute previous goal index " + Curr);
        }

    }
     
    //Previous goal keyframe's time
    public float PrevGoal {
        get {
            //return Keys[PrevGoalKeyInd].Time;
            return Keys[GoalKeys[PrevGoalKeyInd]].Time;
        }
    }


    //Next keyframe's frame number
    public float NextGoal {
        get {
            int nextGoalKeyInd = PrevGoalKeyInd + 1;
            if (nextGoalKeyInd > GoalKeys.Length - 1)
                nextGoalKeyInd = GoalKeys.Length - 1;

            return Keys[GoalKeys[nextGoalKeyInd]].Time;

        }
    }
    
    
    public void Reset(string aName) {
        AnimName = aName;
        animation.clip = animation[AnimName].clip;
		Fps = animation[AnimName].clip.frameRate;        
        AnimLength = animation[AnimName].clip.length;		
		FrameCnt = Mathf.CeilToInt(Fps * AnimLength);

        //PrevGoalKeyInd = 0;
        //NextGoalKeyInd = 1;

        AnimSpeed = DefaultAnimSpeed;


        //animation[animName].speed = 4;
        
    }
    void AssignGoalKeys() {


         if(AnimName.Contains("knock") || AnimName.Contains("Knock")) {
             int i = 0;
             MyKeyTimes = new float[6];
             MyKeyTimes[i++] = 0f; //first frame time is always 0
             MyKeyTimes[i++] = 0.916667f;
             //MyKeyTimes[i++] = 1.291667f;
             //MyKeyTimes[i++] = 1.375f;
             MyKeyTimes[i++] = 1.5f;
             //MyKeyTimes[i++] = 2.5f;
             MyKeyTimes[i++] = 2.916667f;
            // MyKeyTimes[i++] = 3.125f;
             MyKeyTimes[i++] = 4.166667f;
             //MyKeyTimes[i++] = 4.5f;
             MyKeyTimes[i++] = 4.666667f;


             GoalKeys = new int[2];
             GoalKeys[0] = 0;
             GoalKeys[1] = MyKeyTimes.Length - 1;  
            

           /* 
            // Forearm keys
             
            MyKeyTimes = new float[11];
            MyKeyTimes[i++] = 4.768372E-07f;
            MyKeyTimes[i++] = 0.8333335f;
            MyKeyTimes[i++] = 1.208333f;
            MyKeyTimes[i++] = 1.416667f;
            MyKeyTimes[i++] = 2.125f;
            MyKeyTimes[i++] = 2.75f;
            MyKeyTimes[i++] = 2.958333f;
            MyKeyTimes[i++] = 3.833333f;
            MyKeyTimes[i++] = 4.333333f;
            MyKeyTimes[i++] = 4.625f;
            MyKeyTimes[i++] = 4.666667f; 


            GoalKeys = new int[2];
            GoalKeys[0] = 0;
            
            GoalKeys[1] = 10;
            */
             /*
            GoalKeys = new int[6];
            GoalKeys[0] = 0;
            GoalKeys[1] = 2;
            GoalKeys[2] = 4;
            GoalKeys[3] = 6;
            GoalKeys[4] = 8;
            GoalKeys[5] = 10;
             */

            /*
            GoalKeys = new int[4];
            GoalKeys[0] = 0;
            GoalKeys[1] = 3;
            GoalKeys[2] = 6;
            GoalKeys[3] = 10;
             * /
 /*
             int i = 0;
             MyKeyTimes = new float[11];
             MyKeyTimes[i++] = 4.768372E-07f;
             MyKeyTimes[i++] = 0.8333335f;
             MyKeyTimes[i++] = 1.208333f;
             MyKeyTimes[i++] = 1.416667f;
             MyKeyTimes[i++] = 2.125f;
             MyKeyTimes[i++] = 2.75f;
             MyKeyTimes[i++] = 2.958333f;
             MyKeyTimes[i++] = 3.833333f;
             MyKeyTimes[i++] = 4.333333f;
             MyKeyTimes[i++] = 4.625f;
             MyKeyTimes[i++] = 4.666667f;


             GoalKeys = new int[2];
             GoalKeys[0] = 0;
             GoalKeys[1] = 10;
           */
        }
        else if(AnimName.Contains("point") || AnimName.Contains("Point")) {
            //goal keys must include start and end keys


             /*
                int  i = 0;
                    MyKeyTimes = new float[22];
                 MyKeyTimes[i++] = 0f; //first frame time is always 0
                 MyKeyTimes[i++] = 0.5f;
              MyKeyTimes[i++] = 1.0f;
              MyKeyTimes[i++] = 1.5f;
             MyKeyTimes[i++] = 2f;
                 MyKeyTimes[i++] = 2.5f;         
              MyKeyTimes[i++] = 3f;         
                 MyKeyTimes[i++] = 3.5f;
             MyKeyTimes[i++] = 4f;
             MyKeyTimes[i++] = 4.5f;
             MyKeyTimes[i++] = 5f;
             MyKeyTimes[i++] = 5.5f;
             MyKeyTimes[i++] = 6f;
             MyKeyTimes[i++] = 6.5f;
             MyKeyTimes[i++] = 7f;
             MyKeyTimes[i++] = 7.5f;        
             MyKeyTimes[i++] = 8f;
                MyKeyTimes[i++] = 8.5f;
                MyKeyTimes[i++] = 9f;
            MyKeyTimes[i++] = 9.5f;
            MyKeyTimes[i++] = 10f;
                MyKeyTimes[i++] = 10.83334f; 
                  */
             int i = 0;
            MyKeyTimes = new float[8];
            MyKeyTimes[i++] = 0f; //first frame time is always 0
            MyKeyTimes[i++] = 0.9583337f;
            MyKeyTimes[i++] = 1.791667f;
            // MyKeyTimes[i++] = 1.875f;
            //MyKeyTimes[i++] = 1.916667f;
            //MyKeyTimes[i++] = 2.041667f;         
            MyKeyTimes[i++] = 3.041667f;         
            //       MyKeyTimes[i++] = 4.125f;
            MyKeyTimes[i++] = 6.166667f;
            // MyKeyTimes[i++] = 7.916667f;
            // MyKeyTimes[i++] = 8.625001f;
            MyKeyTimes[i++] = 9.333334f;
            MyKeyTimes[i++] = 10.05f;
            MyKeyTimes[i++] = 10.83334f; 
             


            GoalKeys = new int[2];
            GoalKeys[0] = 0;
            GoalKeys[1] = MyKeyTimes.Length - 1;  
            
        }
        else if(AnimName.Contains("lift") || AnimName.Contains("Lift")) {
            GoalKeys = new int[3];
            GoalKeys[0] = 0;
            GoalKeys[1] = 7;
            GoalKeys[2] = 14;

            int i = 0;
            MyKeyTimes = new float[15];
            MyKeyTimes[i++] = 0f;  //first frame time is always 0
            MyKeyTimes[i++]  = 0.5f; 
            MyKeyTimes[i++]  = 1.458333f; 
            MyKeyTimes[i++]  = 2.416667f; 
            MyKeyTimes[i++]  = 2.625f; 
            MyKeyTimes[i++]  = 3.708334f; 
            MyKeyTimes[i++]  = 4.333334f; 
            MyKeyTimes[i++]  = 5.041667f; 
            MyKeyTimes[i++]  = 6.458334f; 
            MyKeyTimes[i++]  = 6.958334f; 
            MyKeyTimes[i++]  = 7.708334f; 
            MyKeyTimes[i++]  = 8.958334f; 
            MyKeyTimes[i++]  = 10.125f; 
            MyKeyTimes[i++]  = 11.04167f; 
            MyKeyTimes[i++]  = 11.66667f;

         

        }

        else if(AnimName.Contains("pick") || AnimName.Contains("Pick")) {
            
          
            int i = 0;

            //Equal key times
            MyKeyTimes = new float[12];
            MyKeyTimes[i++] = 0f;  //first frame time is always 0
            MyKeyTimes[i++] = 0.95f;
            MyKeyTimes[i++] = 1.89f;
            MyKeyTimes[i++] = 2.84f;
            MyKeyTimes[i++] = 3.79f;
            MyKeyTimes[i++] = 4.73f;
            MyKeyTimes[i++] = 5.68f;
            MyKeyTimes[i++] = 6.63f;
            MyKeyTimes[i++] = 7.57f;
            MyKeyTimes[i++] = 8.52f;
            MyKeyTimes[i++] = 9.47f;
            MyKeyTimes[i++] = 10.41667f;

            /*
            MyKeyTimes = new float[12];
            MyKeyTimes[i++] = 0f;  //first frame time is always 0
            MyKeyTimes[i++]  = 0.75f; 
            MyKeyTimes[i++]  = 1.625f; 
            MyKeyTimes[i++]  = 2.583333f; 
            MyKeyTimes[i++]  = 3.208333f; 
            MyKeyTimes[i++]  = 4.208333f; 
            MyKeyTimes[i++]  = 5.5f; 
            MyKeyTimes[i++]  = 6.125f; 
            MyKeyTimes[i++]  = 6.5f; 
            MyKeyTimes[i++]  = 7.125f; 
            MyKeyTimes[i++]  = 7.958334f; 
            //MyKeyTimes[i++]  = 9.541667f; 
            MyKeyTimes[i++]  = 10.41667f;
            */
            GoalKeys = new int[2];
            GoalKeys[0] = 0;
            GoalKeys[1] = MyKeyTimes.Length - 1;   
         

        }
        else if(AnimName.Contains("punch") || AnimName.Contains("Punch")) {
    
         
             int i = 0;             
            MyKeyTimes = new float[19];
            MyKeyTimes[i++] = 0f;  //first frame time is always 0
            MyKeyTimes[i++]  = 0.4583335f; 
            //MyKeyTimes[i++]  = 0.5000005f; 
            //MyKeyTimes[i++]  = 0.5833335f; 
            MyKeyTimes[i++]  = 0.8333335f; 
            //MyKeyTimes[i++]  = 0.8750005f; 
            //MyKeyTimes[i++]  = 0.916667f; 
            //MyKeyTimes[i++]  = 1f; 
            MyKeyTimes[i++]  = 1.541667f; 
            //MyKeyTimes[i++]  = 1.875f; 
            //MyKeyTimes[i++]  = 1.916667f; 
            MyKeyTimes[i++]  = 2f;
            MyKeyTimes[i++] = 2.5f; 
            MyKeyTimes[i++]  = 3f; 
            MyKeyTimes[i++]  = 3.666667f;
            MyKeyTimes[i++] = 4.1f; 
            //MyKeyTimes[i++]  = 4.416667f; 
            MyKeyTimes[i++]  = 4.791667f; 
            MyKeyTimes[i++]  = 5.125f; 
            MyKeyTimes[i++]  = 5.333333f; 
            //MyKeyTimes[i++]  = 5.583333f; 
            MyKeyTimes[i++]  = 6.083333f; 
            //MyKeyTimes[i++]  = 6.541667f; 
            //MyKeyTimes[i++]  = 6.583333f; 
            MyKeyTimes[i++]  = 6.625f; 
            //MyKeyTimes[i++]  = 6.708333f; 
            //MyKeyTimes[i++]  = 7.083333f; 
           // MyKeyTimes[i++]  = 7.333333f; 
            MyKeyTimes[i++]  = 7.458333f; 
            //MyKeyTimes[i++]  = 7.625f; 
            MyKeyTimes[i++]  = 8.041668f; 
          //  MyKeyTimes[i++]  = 8.583334f; 
            MyKeyTimes[i++]  = 9.041668f; 
            //MyKeyTimes[i++]  = 9.666668f; 
            MyKeyTimes[i++]  = 10.95833f; 
            //MyKeyTimes[i++]  = 12.16667f; 
            //MyKeyTimes[i++]  = 12.625f; 
            //MyKeyTimes[i++]  = 13f; 
            MyKeyTimes[i++]  = 13.33333f;


            GoalKeys = new int[2];
            GoalKeys[0] = 0;
            GoalKeys[1] = MyKeyTimes.Length - 1;   

         
        } 
        else if(AnimName.Contains("push") || AnimName.Contains("Push")) {
          
            int i = 0;
            MyKeyTimes = new float[25];
            MyKeyTimes[i++] = 0f;  //first frame time is always 0
            MyKeyTimes[i++]  = 0.541667f; 
            MyKeyTimes[i++]  = 1f; 
            //MyKeyTimes[i++]  = 1.25f; 
            MyKeyTimes[i++]  = 1.625f; 
            MyKeyTimes[i++]  = 2.333333f; 
            MyKeyTimes[i++]  = 2.833333f; 
            MyKeyTimes[i++]  = 3.333334f; 
            MyKeyTimes[i++]  = 4.25f; 
            //MyKeyTimes[i++]  = 4.833334f; 
            MyKeyTimes[i++]  = 5.25f; 
            MyKeyTimes[i++]  = 5.958334f; 
            MyKeyTimes[i++]  = 6.708334f; 
            MyKeyTimes[i++]  = 7.541667f; 
            MyKeyTimes[i++]  = 9f; 
            MyKeyTimes[i++]  = 9.625f; 
            MyKeyTimes[i++]  = 10.125f; 
            MyKeyTimes[i++]  = 11.625f; 
            MyKeyTimes[i++]  = 12.625f; 
            MyKeyTimes[i++]  = 13.375f; 
            MyKeyTimes[i++]  = 13.79167f; 
            MyKeyTimes[i++]  = 14.83333f; 
            MyKeyTimes[i++]  = 16f; 
            MyKeyTimes[i++]  = 16.58333f; 
            MyKeyTimes[i++]  = 17.75f; 
            MyKeyTimes[i++]  = 19.54167f; 
            MyKeyTimes[i++]  = 20.79167f;


            GoalKeys = new int[2];
            GoalKeys[0] = 0;
            GoalKeys[1] = MyKeyTimes.Length - 1;   

          
        }
                   
        else if(AnimName.Contains("throw") || AnimName.Contains("Throw")) {

            
             MyKeyTimes = new float[26];
             int i = 0;
             MyKeyTimes = new float[26];
             MyKeyTimes[i++] = 0f;  //first frame time is always 0
            MyKeyTimes[i++]  = 0.5416667f; 
            MyKeyTimes[i++]  = 1.25f; 
            MyKeyTimes[i++]  = 1.916667f; 
            MyKeyTimes[i++]  = 2.208334f; 
            MyKeyTimes[i++]  = 2.375f; 
            MyKeyTimes[i++]  = 3.25f; 
            MyKeyTimes[i++]  = 3.583334f; 
            MyKeyTimes[i++]  = 3.791667f; 
            MyKeyTimes[i++]  = 3.916667f; 
            MyKeyTimes[i++]  = 4.125f; 
            MyKeyTimes[i++]  = 4.375f; 
            MyKeyTimes[i++]  = 4.833334f; 
            MyKeyTimes[i++]  = 5.333334f; 
            MyKeyTimes[i++]  = 5.750001f; 
            MyKeyTimes[i++]  = 6.250001f; 
            MyKeyTimes[i++]  = 6.458334f; 
            MyKeyTimes[i++]  = 6.875001f; 
            MyKeyTimes[i++]  = 7.458334f; 
            MyKeyTimes[i++]  = 7.708334f; 
            MyKeyTimes[i++]  = 8.458334f; 
            MyKeyTimes[i++]  = 9.125001f; 
            MyKeyTimes[i++]  = 9.958334f; 
            MyKeyTimes[i++]  = 10.79167f; 
            MyKeyTimes[i++]  = 11.875f; 
            MyKeyTimes[i++]  = 12.5f;




            GoalKeys = new int[2];
            GoalKeys[0] = 0;
            GoalKeys[1] = MyKeyTimes.Length - 1;               
            
        }
        else if(AnimName.Contains("walk") || AnimName.Contains("Walk")) {
            
            int i = 0;
            MyKeyTimes = new float[17];
            MyKeyTimes[i++] = 0f; //first frame time is always 0
            MyKeyTimes[i++] = 1.208334f;
            MyKeyTimes[i++] = 2.000001f;
            //MyKeyTimes[i++] = 2.041667f;
            //MyKeyTimes[i++] = 2.083334f;
            //MyKeyTimes[i++] = 2.208334f;
            MyKeyTimes[i++] = 2.508334f;
            MyKeyTimes[i++] = 3.041667f;
            MyKeyTimes[i++] = 3.791667f;
            MyKeyTimes[i++] = 4.958334f;
            MyKeyTimes[i++] = 5.833334f;
            MyKeyTimes[i++] = 6.750001f;
            MyKeyTimes[i++] = 7.375001f;
            MyKeyTimes[i++] = 8.000001f;
            MyKeyTimes[i++] = 8.833335f;
            MyKeyTimes[i++] = 9.666667f;
            MyKeyTimes[i++] = 10.375f;
            MyKeyTimes[i++] = 11.375f;
            MyKeyTimes[i++] = 12f;
            //MyKeyTimes[i++] = 13.20833f;
            MyKeyTimes[i++] = 13.33333f;


            GoalKeys = new int[2];
            GoalKeys[0] = 0;
            GoalKeys[1] = MyKeyTimes.Length - 1;   


            
        }
        else if(AnimName.Contains("waving") || AnimName.Contains("Waving")) {
            int i = 0;
            MyKeyTimes = new float[24];
            MyKeyTimes[i++] = 0f;  //first frame time is always 0
            MyKeyTimes[i++]  = 0.8750002f; 
            MyKeyTimes[i++]  = 1.458334f; 
            MyKeyTimes[i++]  = 2.208334f; 
            MyKeyTimes[i++]  = 2.791667f; 
            MyKeyTimes[i++]  = 3.291667f; 
            MyKeyTimes[i++]  = 3.708334f; 
            MyKeyTimes[i++]  = 4.25f; 
            MyKeyTimes[i++]  = 4.583334f; 
            MyKeyTimes[i++]  = 4.875001f; 
            MyKeyTimes[i++]  = 5.750001f; 
            MyKeyTimes[i++]  = 6.375001f; 
            MyKeyTimes[i++]  = 7.041667f; 
            MyKeyTimes[i++]  = 7.541667f; 
            MyKeyTimes[i++]  = 8.000001f; 
            MyKeyTimes[i++]  = 8.541667f; 
            MyKeyTimes[i++]  = 9.125001f; 
            MyKeyTimes[i++]  = 9.875001f; 
            MyKeyTimes[i++]  = 10.58333f; 
            MyKeyTimes[i++]  = 11.08333f; 
            MyKeyTimes[i++]  = 11.58333f; 
            MyKeyTimes[i++]  = 12.125f; 
            MyKeyTimes[i++]  = 12.66667f; 
            MyKeyTimes[i++]  = 13.33333f;


            GoalKeys = new int[2];
            GoalKeys[0] = 0;
            GoalKeys[1] = MyKeyTimes.Length - 1;   
        }

    }
    //Vector3 ComputeInitialSwivelAngle(ArmInfo armInfo) {
    //    float theta;		
    //    Vector3 u = new Vector3(1,0,0); //Local x axis		
    //    Vector3 n = armInfo.wrist.position - armInfo.shoulder.position;		
    //    Vector3 c =  HandleUtility.ProjectPointLine (armInfo.elbow.position, armInfo.shoulder.position, armInfo.wrist.position);		
    //    Vector3 pcap = armInfo.elbow.position - c;		
    //    Vector3 ptilda = pcap - Vector3.Dot (pcap, n) * n; 		
    //    theta = Mathf.Atan2(Vector3.Cross(pcap, u).magnitude , Vector3.Dot (ptilda,u));		
    //    return new Vector3(theta,0,0);
		
    //}
	
	//ARminfo is passed as a transform because animation is sampled
	//We want the end effector position at the sampled time
    
	public void InitKeyPoints() {

        TorsoController torso = GetComponent<TorsoController>();
        ArmInfo[] arms = GetComponent<ArmController>().Arms;

        AssignGoalKeys();

#if EDITORMODE
	  
      // AnimationClipCurveData[] curveDatas = AnimationUtility.GetAllCurves(animation[animName].clip, true);
      // Keyframe[] frames = curveDatas[0].curve.keys;

        //AnimationCurve xCurve = AnimationUtility.GetEditorCurve(animation[animName].clip,"Hips/Spine/Spine1/RightShoulder/RightArm/RightForeArm/RightHand",typeof(Transform),"m_LocalRotation.x");
        //forearm keys are fewer in number, looks better with  EMOTE
        //AnimationCurve xCurve = AnimationUtility.GetEditorCurve(animation[AnimName].clip, "Hips/Spine/Spine1/RightShoulder/RightArm/RightForeArm", typeof(Transform), "m_LocalRotation.x");
        AnimationCurve xCurve = AnimationUtility.GetEditorCurve(animation[AnimName].clip, "Hips/Spine/Spine1/RightShoulder/RightArm", typeof(Transform), "m_LocalRotation.x");

      Keyframe[] frames = xCurve.keys;

      using (StreamWriter sw = new StreamWriter("keyframes_" + AnimName + ".txt")) {
          sw.WriteLine("MyKeyTimes = new float" + "[" + frames.Length + "];");
            foreach (Keyframe kf in frames) {
                sw.WriteLine("MyKeyTimes[i++]  = " + kf.time + "f; ");                
            }
        }
      Debug.Log(AnimName);
#elif !EDITORMODE
      
  		//file io does not work in web player
  /*      string[] content = File.ReadAllLines("keyframes.txt");        
        MyKeyframe[] frames = new MyKeyframe[content.Length];
        
         
        for (int i = 0; i < content.Length; i++) {
            frames[i] = new MyKeyframe();           
            frames[i].time = float.Parse(content[i]);        
            
        }*/
        //Will write IO operations  later
      MyKeyframe[] frames = new MyKeyframe[MyKeyTimes.Length];
        for (int i = 0; i < MyKeyTimes.Length; i++) {
            frames[i] = new MyKeyframe();           
            frames[i].Time = MyKeyTimes[i];           
        }
      
#endif
        animation.Play(AnimName);
        int goalKeyInd = 0;
	
		Keys = new KeyInfo[frames.Length ];
            
		for(int i = 0; i < frames.Length; i++) {
            animation[AnimName].enabled = true;   

        animation[AnimName].time = frames[i].Time;


        animation.Sample ();	
		Keys[i] = new KeyInfo();			
        Keys[i].Time = animation[AnimName].time;

        //Keys[i].Time = animation[AnimName].length * i / (frames.Length - 1); //FUNDA : If we are using equally-spaced keys. We still sample from original frames, but we change key times.
        if(i == GoalKeys[goalKeyInd]) {            
            Keys[i].IsGoal = true;
            goalKeyInd++;
        }
        else if(i < GoalKeys[goalKeyInd]) 
            Keys[i].IsGoal = false;
                   
        if (Keys[i].FrameNo >= FrameCnt) {                    
            Keys[i].FrameNo = FrameCnt - 1;
        }

           
            
        //body chain and transformation arrays for the specific animation
        BodyChain = torso.BodyChainToArray(torso.Torso.Root);
        Keys[i].BodyPos = torso.BodyPosArr(BodyChain);
        Keys[i].BodyRot = torso.BodyRotArr(BodyChain);


       
           /* 
        BodyChain = torso.BodyChain;
        Keys[i].BodyPos = torso.BodyPos;
        Keys[i].BodyRot = torso.BodyRot;
        */
        for(int arm = 0; arm < 2; arm++) {
            Keys[i].ShoulderPos[arm] = Keys[i].ShoulderPosOrig[arm] = arms[arm].Shoulder.position;            
	        Keys[i].EePos[arm] = Keys[i].EePosOrig[arm] = arms[arm].Wrist.position;
            Keys[i].ElbowRot[arm] = Keys[i].ElbowRotOrig[arm] = arms[arm].Elbow.localRotation;
            Keys[i].ElbowPos[arm] = Keys[i].ElbowPosOrig[arm] = arms[arm].Elbow.position;
        }	
        }
			
	
        animation.Stop(AnimName);
        animation.enabled = false;


			
	}


    public void ComputeEeVelUpdated(int arm) {
      
        //Compute end effector velocity
        for (int i = 0; i < Keys.Length; i++) {            
            if (i == 0)
                Keys[0].EeVelUpdated[arm] = (Keys[1].EePosUpdated[arm] - Keys[0].EePosUpdated[arm]) / (Keys[1].Time - Keys[0].Time);
            else if (i == Keys.Length - 1)
                Keys[Keys.Length - 1].EeVelUpdated[arm] = (Keys[Keys.Length - 1].EePosUpdated[arm] - Keys[Keys.Length - 2].EePosUpdated[arm]) / (Keys[Keys.Length - 1].Time - Keys[Keys.Length - 2].Time);
            else
                Keys[i].EeVelUpdated[arm] = (Keys[i + 1].EePosUpdated[arm] - Keys[i - 1].EePosUpdated[arm]) / (Keys[i + 1].Time - Keys[i - 1].Time);

            
        }
    }


    List<Transform> BodyChainToArray(Transform root) {
                
        List<Transform> chain = new List<Transform>();

        chain.Add(root);                    
                    
        for(int i = 0; i < root.childCount; i++) 
            chain.AddRange(BodyChainToArray(root.GetChild(i)));            
        

        
        return chain;
    }

    List<Vector3> BodyPosArr(List<Transform> bodyChain) {
        List<Vector3> bodyPos= new List<Vector3>();
        for (int i = 0; i < bodyChain.Count; i++)
            bodyPos.Add(bodyChain[i].localPosition);

        return bodyPos;
    }
    
    List<Quaternion> BodyRotArr(List<Transform> bodyChain) {
        List<Quaternion> bodyRot= new List<Quaternion>();
        for (int i = 0; i < bodyChain.Count; i++)
            bodyRot.Add(bodyChain[i].localRotation);

        return bodyRot;
    }


    public void InterpolateWholeBody(int keyInd, float lt) {
        
        if(keyInd + 1 > Keys.Length - 1) {
            for (int i = 0; i < BodyChain.Count; i++) {
                BodyChain[i].transform.localPosition = Keys[keyInd].BodyPos[i];
                BodyChain[i].transform.localRotation = Keys[keyInd].BodyRot[i];
            }
     
        }
        else {
           // float deltaT = 1f / (float)(keys.Length - 1);
          //  lt = (t -  deltaT*(float)keyInd) / deltaT;
            
            for (int i = 0; i < BodyChain.Count; i++) {           
                BodyChain[i].transform.localPosition = Vector3.Lerp(Keys[keyInd].BodyPos[i], Keys[keyInd+1].BodyPos[i], lt);
                BodyChain[i].transform.localRotation = Quaternion.Slerp(Keys[keyInd].BodyRot[i], Keys[keyInd+1].BodyRot[i], lt);
            }          
          
        }
      
    }

    public void ProjectWholeBodyBeyondKeyInd(int keyInd, float lt, float t){
        Vector3 pivot = Vector3.zero;

        if (t >= 0 && t <= 1)
            return; //no need for position update

        if(keyInd + 1 <= Keys.Length - 1) {
            
            for (int i = 0; i < BodyChain.Count; i++) {

                if (t < 0) 
                    pivot = Keys[0].BodyPos[i];
                else if (t > 1) 
                    pivot = Keys[Keys.Length - 1].BodyPos[i];

                BodyChain[i].transform.localPosition = 2 * pivot - Vector3.Lerp(Keys[keyInd].BodyPos[i], Keys[keyInd+1].BodyPos[i], lt);
            }          
          
        }
    }

    public void InitInterpolators(int arm, List<ControlPoint> controlPoints, float tension, float continuity, float bias) {

        ControlPoint[] controlPointsEE = new ControlPoint[controlPoints.Count];
        for (int i = 0; i < controlPoints.Count; i++)
            controlPointsEE[i] = controlPoints[i];


        SIee[arm] = new TCBSpline(controlPointsEE, tension, continuity, bias);
    }
	
	
	public void InitInterpolators (float tension, float continuity, float bias)  {


        //Keyframe to time
        ControlPoint[] controlPointsKF = new ControlPoint[Keys.Length];
        for (int i = 0; i < Keys.Length; i++) {
            controlPointsKF[i] = new ControlPoint();
            controlPointsKF[i].Point = new Vector3(Keys[i].FrameNo, 0, 0);
            controlPointsKF[i].TangentI = Vector3.zero;
            controlPointsKF[i].TangentO = Vector3.zero;
            controlPointsKF[i].Time = Keys[i].Time;
        }

        SIKeyTime = new TCBSpline(controlPointsKF, 0, 0, 0);
            


        
		for(int arm = 0; arm < 2; arm++) {
        
            //End effector
            ControlPoint[] controlPointsEE = new ControlPoint[Keys.Length];
            for(int i = 0; i< Keys.Length; i++) {
                controlPointsEE[i] = new ControlPoint();
                controlPointsEE[i].Point = Keys[i].EePos[arm];                
                controlPointsEE[i].TangentI = Vector3.zero;
                controlPointsEE[i].TangentO = Vector3.zero;                
                controlPointsEE[i].Time = Keys[i].Time;
            }

            SIee[arm] = new TCBSpline(controlPointsEE, tension, continuity, bias);
            
            //Elbow Rotation interpolator            
            ControlPoint[] controlPointsEA = new ControlPoint[Keys.Length];
            for (int i = 0; i < Keys.Length; i++){
                controlPointsEA[i] = new ControlPoint();
                controlPointsEA[i].Rotation = Keys[i].ElbowRot[arm];
                controlPointsEA[i].TangentI = Vector3.zero;
                controlPointsEA[i].TangentO = Vector3.zero;
            }


            SIElbowAngle[arm] = new TCBSpline(controlPointsEA, tension, continuity, bias);

            /*
            //Elbow position
            ControlPoint[] controlPointsEP = new ControlPoint[Keys.Length];
            for (int i = 0; i < Keys.Length; i++) {
                controlPointsEP[i] = new ControlPoint();
                controlPointsEP[i].Point = Keys[i].ElbowPos[arm];
                controlPointsEP[i].TangentI = Vector3.zero;
                controlPointsEP[i].TangentO = Vector3.zero;
                controlPointsEP[i].FrameNo = Keys[i].FrameNo;
            }

            //Rotation interpolator

            SIElbowPos[arm] = new TCBSpline(controlPointsEP, tension, continuity, bias);
            */
		}				
	}

/*
    public float ComputeInterpolatedTime(Vector3 point, int p) {
        return SIKeyTime.FindDistanceOnSegment(point, p);
    }
    */
	public Vector3 ComputeInterpolatedTarget(float lt, int p,  int arm) {
        return SIee[arm].GetInterpolatedSplinePoint(lt, p);			
	}
    
    public Quaternion ComputeInterpolatedElbowAngle(float lt, int p, int arm) {

        if (p + 1 < SIElbowAngle[arm]._controlPoints.Length - 1) {
            return Quaternion.Slerp(SIElbowAngle[arm]._controlPoints[p].Rotation, SIElbowAngle[arm]._controlPoints[p + 1].Rotation, lt);
        }
        else
            return SIElbowAngle[arm]._controlPoints[p].Rotation;
       // return SIElbowAngle[arm].GetInterpolatedSplinePoint(lt, p);
    }

    /*
    public Vector3 ComputeInterpolatedElbowPos(float lt, int p,   int arm) {
        return SIElbowPos[arm].GetInterpolatedSplinePoint(lt, p);
    }
	*/
    //t is between 0 and 1
    //Find the corresponding previous keyframe number at t
    //TODO: binary search
    public int FindKeyNumberAtNormalizedTime(float t) {
        if (t < 0 || t > 1) {
            Debug.Log("Incorrect time coefficient");
            return -1;
        }
      
        
        

        float appTime = t * AnimLength;
        for(int i = 0; i < Keys.Length-1; i++) {
            if (Keys[i].Time <= appTime && Keys[i + 1].Time > appTime)
                return i;
        }
        return Keys.Length - 1;
      
    /*    if (t == 1)
            return Keys.Length - 1;

        return Mathf.FloorToInt(t * Keys.Length);
    */
    }

    public int FindKeyNumberAtTime(float t) {

        
        for (int i = 0; i < Keys.Length - 1; i++) {
            if (Keys[i].Time <= t && Keys[i + 1].Time > t)
                return i;
        }
        return Keys.Length - 1;

    }


      public int FindFrameNumberAtTime(float t) {
        if (t < 0 || t > 1) {
            Debug.Log("Incorrect time coefficient");
            return -1;
        }

        //Compute it as in spline interpolation
        return (int) (t * FrameCnt);        
    }
      
    //key index of the previous goal
    public int FindPrevGoalAtTime(float t) {
        for (int i = 0; i < GoalKeys.Length - 1; i++) {
            if (Keys[GoalKeys[i]].Time >= Keys[GoalKeys[GoalKeys.Length - 1]].Time)
                //return GoalKeys[GoalKeys.Length - 1];
                return GoalKeys.Length - 1;
            else if (Keys[GoalKeys[i]].Time <= t && Keys[GoalKeys[i + 1]].Time > t)
                //return GoalKeys[i];
                return i;
        }

        //return Keys.Length - 1;
        return GoalKeys.Length - 1;
     
    }

    void Update() {
        
        if (!animation.isPlaying)
            return;

        
        animation[AnimName].speed = AnimSpeed;

	}


	
    void OnDrawGizmos(){

        /*
         //Draw velocity and position
        if (this.gameObject == GameObject.Find("AgentPrefab")) {
            Gizmos.color = Color.white;
            Gizmos.DrawLine(Vector3.zero, new Vector3(0, Keys[0].EeVelUpdated[1].magnitude * 40, 0));
            // for(int j = 1; j <= prevKeyInd; j++) {                    
            for (int j = 1; j < Keys.Length - 2; j++) {
                Gizmos.DrawLine(new Vector3(j - 1, Keys[j - 1].EeVelUpdated[1].magnitude * 40, 0), new Vector3(j, Keys[j].EeVelUpdated[1].magnitude * 40, 0));
            }
            Gizmos.color = Color.red;
            Gizmos.DrawLine(Vector3.zero, new Vector3(0, Keys[0].EePosUpdated[1].magnitude * 0.1f, 0));
            // for(int j = 1; j <= prevKeyInd; j++) {                    
            for (int j = 1; j < Keys.Length - 2; j++) {

                Gizmos.DrawLine(new Vector3(j - 1, Keys[j - 1].EePosUpdated[1].magnitude*0.1f , 0), new Vector3(j, Keys[j].EePosUpdated[1].magnitude*0.1f , 0));
            }
        }
        */
        for(int j = 0; j < Keys.Length; j++) {            
            //Gizmos.color = keyColor;
        



            if (Keys[j].IsGoal)
                Gizmos.color = Color.red;



            /*//Tangents -- SCALED DOWN!
            Gizmos.color = Color.magenta;
            Gizmos.DrawLine(SIee[1]._controlPoints[j].Point, SIee[1]._controlPoints[j].TangentI* 0.5f  + SIee[1]._controlPoints[j].Point);
            Gizmos.color = Color.red;
            Gizmos.DrawLine(SIee[1]._controlPoints[j].Point, SIee[1]._controlPoints[j].TangentO * 0.5f + SIee[1]._controlPoints[j].Point);
            */

            Gizmos.color = new Color(0, j / 10f, 1);//bluish
            int stepCnt;
            if (j < Keys.Length - 1)
                stepCnt = (int)((Keys[j + 1].Time - Keys[j].Time) / 0.05f);
            else
                stepCnt = 1000;

            for (int arm = 0; arm < 2; arm++) {

                for (int i = 0; i < stepCnt; i++)
                    Gizmos.DrawSphere(SIee[arm].GetInterpolatedSplinePoint((float)i / stepCnt, j), 0.0008f);

                Gizmos.DrawSphere(SIee[arm]._controlPoints[j].Point, 0.002f);
            }

            //Gizmos.DrawSphere(Keys[j].EePos[1], 0.002f);
            Gizmos.color = Color.yellow;
            if(j + 1 < Keys.Length) 
                Gizmos.DrawLine(Keys[j].EePos[1], Keys[j+1].EePos[1]);            
            
        }

         


    }
	
}

