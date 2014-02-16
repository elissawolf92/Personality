//#define EDITORMODE
using UnityEngine;
using System;
#if EDITORMODE
using UnityEditor;
#endif

///Same as AnimationInfo but used in MOCAP data

public class AnimationManager : MonoBehaviour {
	
	public float Fps;
	public float AnimLength;
	public KeyInfo[] Keys;
	public int PrevKeyInd = 0;
	public int NextKeyInd = 1;
	
	public string AnimName;
	public int FrameCnt;
    static int _animIndex = 0;
    public string[] AnimNameArr;
	private int _weightStartFrame, _weightEndFrame;

    public bool MetricsComputed = false;
	public Metrics Metrics;
    
    public float[] MinSpaceAll = new float[2];
    public float[] MaxSpaceAll = new float[2];

    public float[] MinWeightAll = new float[2];
    public float[] MaxWeightAll = new float[2];

    public float[] MinTimeAll = new float[2];
    public float[] MaxTimeAll = new float[2];

    public float[] MinFlowAll = new float[2];
    public float[] MaxFlowAll = new float[2];

    public float[] MinPostureAll = new float[10];
    public float[] MaxPostureAll = new float[10];

	//Current frame number
	public int Curr {
		get {
			int val = Mathf.CeilToInt(animation[AnimName].time * animation[AnimName].clip.frameRate)%FrameCnt;
			if(val == 0 && Next == FrameCnt)
				val = FrameCnt;
			//return keyInfo.frames.CurrentFrame();
			return val ;						
		}
	}
	

    //Previous keyframe's frame number
	public int Prev {		
		get {			 			
			return Keys[PrevKeyInd].FrameNo;				
		}
	}
	//Next keyframe's frame number
	public int Next{		
		get {		
			return Keys[NextKeyInd].FrameNo;				
		}
	}
	
    public int GetKeyNumber(int frameNo) {
        for (int i = 0; i < Keys.Length; i++) {
            if (Keys[i].FrameNo == frameNo)
                return i;
        }
        return -1;


    }
	void Awake() {
        MetricsComputed = false;
        GetAnimNames();
        ChangeAnim(AnimNameArr[0]);

        for(int i = 0; i < 2; i++) {
            MinSpaceAll[i] = 100000f;
            MaxSpaceAll[i] = -100000f;
            MinWeightAll[i] = 100000f;
            MaxWeightAll[i] = -100000f;
            MinTimeAll[i] = 100000f;
            MaxTimeAll[i] = -100000f;
            MinFlowAll[i] = 100000f;
            MaxFlowAll[i] = -100000f;
        }
        for(int i = 0; i <10; i++) {
            MinPostureAll[i] = 100000f;
            MaxPostureAll[i] = -100000f;
        }
	}

    void GetAnimNames() {
        AnimNameArr = new string[animation.GetClipCount()];
        int i = 0;
        foreach(AnimationState s in animation) {
            AnimNameArr[i++] = s.name;            
        }
    }

    public void ChangeAnim(string aName){
        AnimName = aName;        
        animation.clip = animation[aName].clip;
        Fps = animation[AnimName].clip.frameRate;        
        AnimLength = animation[AnimName].clip.length;		
		FrameCnt = (int)(Fps * AnimLength);		
        //change playback speed
        animation[AnimName].speed = 4;
		animation.Play();
        InitKeyPoints(); //samples animation and initializes keypoints
        
    }

    public void ComputeMetrics(){

        Metrics = new Metrics();
		Metrics.Initialize (Keys);

        Metrics.SegmentMotion(1);
        
        
        for (int arm = 0; arm < 2; arm++) {
            Metrics.Weight(arm);
            Metrics.Space(arm);
            Metrics.Time(arm);
            Metrics.Flow(arm);
            Metrics.Emote(arm);            
        }
        
        Metrics.Posture();
        
        
	}
    public void ComputeHistograms(){
        Metrics.ComputeHistograms(MinSpaceAll,MaxSpaceAll,MinWeightAll,MaxWeightAll, MinTimeAll,MaxTimeAll,MinFlowAll,MaxFlowAll, MinPostureAll, MaxPostureAll);        
    }

	//Gets only transform Keys
	public void InitKeyPoints() {

        ArmInfo[] arms = GetComponent<ArmController>().Arms;
        TorsoInfo torso = GetComponent<TorsoController>().Torso;
#if EDITORMODE
		
        //Rotation x, y and z curves are the same, so one is enough
        //No keypoints defined for localTransform
        AnimationCurve xCurve = AnimationUtility.GetEditorCurve(animation[AnimName].clip,"Hips/Spine/Spine1/RightShoulder/RightArm/RightForeArm/RightHand",typeof(Transform),"m_LocalRotation.x");

        Keyframe[] frames = xCurve.keys;

         //Keyframe[] frames =animation[AnimName].;
        
        Keys = new KeyInfo[frames.Length];			
         //all frames as Keys   Keys = new KeyInfo[frameCnt ];			
				
		for(int i = 0; i < Keys.Length; i++) {
        
			animation[AnimName].time = frames[i].time;//(float) Keys[i].frameNo / animation[AnimName].clip.frameRate;			
            //animation[AnimName].time = i/animation[AnimName].clip.frameRate;//(float) Keys[i].frameNo / animation[AnimName].clip.frameRate;			
			animation.Sample ();	
			Keys[i] = new KeyInfo();		
			Keys[i].FrameNo  = Mathf.FloorToInt(animation[AnimName].time * animation[AnimName].clip.frameRate);
            Keys[i].Time = animation[AnimName].time;		
            for(int arm = 0; arm < 2; arm++) { 				
				Keys[i].EePos[arm] = Keys[i].EePosOrig[arm] = arms[arm].Wrist.position;
            }
            
            Keys[i].ClavicleLRot = torso.ClavicleL.rotation;
            Keys[i].ClavicleRRot = torso.ClavicleR.rotation;
            Keys[i].NeckRot = torso.Neck.rotation;
            Keys[i].SpineRot = torso.Spine.rotation;
            Keys[i].Spine1Rot = torso.Spine1.rotation;
            Keys[i].ElbowLRot = torso.ElbowL.rotation;
            Keys[i].ElbowRRot = torso.ElbowR.rotation;


            Keys[i].ClavicleLPos = torso.ClavicleL.position;
            Keys[i].ClavicleRPos = torso.ClavicleR.position;
            Keys[i].NeckPos = torso.Neck.position;
            Keys[i].SpinePos = torso.Spine.position;
            Keys[i].Spine1Pos = torso.Spine1.position;
            Keys[i].ElbowLPos = torso.ElbowL.position;
            Keys[i].ElbowRPos = torso.ElbowR.position;

		
        }
        //Compute end effector velocity
        for(int i = 0; i < Keys.Length; i++) {
            for(int arm = 0; arm < 2; arm++) { 				
                if(i == 0)
                    Keys[0].EeVel[arm] = (Keys[1].EePos[arm] - Keys[0].EePos[arm]) / (Keys[1].Time - Keys[0].Time);
                else if(i == Keys.Length - 1)
                    Keys[Keys.Length - 1].EeVel[arm] = (Keys[Keys.Length - 1].EePos[arm] - Keys[Keys.Length - 2].EePos[arm]) / (Keys[Keys.Length - 1].Time - Keys[Keys.Length - 2].Time);
				else
                    Keys[i].EeVel[arm] = (Keys[i + 1].EePos[arm] - Keys[i - 1].EePos[arm]) / (Keys[i + 1].Time - Keys[i - 1].Time);					
                                                            
            }                                             			
        }

        //Compute end effector acceleration
         for(int i = 0; i < Keys.Length; i++) {
            for(int arm = 0; arm < 2; arm++) { 				
                if(i == 0)
                    Keys[0].EeAcc[arm] = (Keys[1].EeVel[arm] - Keys[0].EeVel[arm]) / (Keys[1].Time - Keys[0].Time);
                else if(i == Keys.Length - 1)
                    Keys[Keys.Length - 1].EeAcc[arm] = (Keys[Keys.Length - 1].EeVel[arm] - Keys[Keys.Length - 2].EeVel[arm]) / (Keys[Keys.Length - 1].Time - Keys[Keys.Length - 2].Time);
				else
                    Keys[i].EeAcc[arm] = (Keys[i + 1].EeVel[arm] - Keys[i - 1].EeVel[arm]) / (Keys[i + 1].Time - Keys[i - 1].Time);					
                                                            
            }                                             			
        }       
 		
        #endif

	}
			
	void Update() {
        if (Input.GetKeyDown("right")) {
            _animIndex++;
            if (_animIndex > animation.GetClipCount() - 1) 
                _animIndex = 0;
            ChangeAnim(AnimNameArr[_animIndex]);						
            ComputeMetrics();
            ComputeHistograms();
            //Reset key indices
            PrevKeyInd = 0;
            NextKeyInd = 1;
        }
        else if (Input.GetKeyDown("left")) {
            _animIndex--;
            if (_animIndex < 0) 
                _animIndex = animation.GetClipCount() - 1;
            ChangeAnim(AnimNameArr[_animIndex]);
			InitKeyPoints();
			
            ComputeMetrics();
            ComputeHistograms();
            //Reset key indices
            PrevKeyInd = 0;
            NextKeyInd = 1;
            
        }
		
        
		if(Curr == 0) {
			PrevKeyInd = 0;
			NextKeyInd = 1;
			
		}

        //print(Keys[metrics.strongestKey[1]].frameNo); 
        //if(prevKeyInd == metrics.strongestKey[1])
		  //  Application.CaptureScreenshot("METRICS\\" + animNameArr[animIndex] + ".png");

        if(NextKeyInd > 0 && NextKeyInd < Keys.Length && Curr >= Keys[NextKeyInd].FrameNo) {
			PrevKeyInd = NextKeyInd;
			NextKeyInd++;
			if( NextKeyInd >= Keys.Length) {
				PrevKeyInd = 0;
				NextKeyInd = 1;

			}
           
		}
		
       // animation.Stop();
      
        //animation.Play(animNameArr[animIndex]);
        //animation.clip = animation[(animNameArr[animIndex])].clip;
                
    }

    void OnGUI(){
        GUILayout.Label ("Animation  " + AnimNameArr[_animIndex]);
        if (!MetricsComputed)
            return;
        
        if(PrevKeyInd < Keys.Length - 2) {
            GUILayout.Label("waist" +Metrics.PostureMetric[0][PrevKeyInd] + "chest " + Metrics.PostureMetric[1][PrevKeyInd] + "neck" +  Metrics.PostureMetric[2][PrevKeyInd] ) ;
        /*    GUILayout.Label("Segment #" + metrics.GetSegmentNo(curr));
            for (int i = 0; i < metrics.segments.Count; i++) {
                //print(metrics.segments[i] + " " + curr);
                GUILayout.Label("Segment " + metrics.segments[i]);
            }
           */
        }
        
        
    }
    //Visualize metrics
    void OnDrawGizmos(){

        if (!MetricsComputed)
            return;
        int k = Keys.Length-1;
      
        Gizmos.color = Color.yellow;
        for(int j = 0; j < k; j++) {            
            Gizmos.DrawLine(Keys[j].EePos[1], Keys[j+1].EePos[1]);            
            Gizmos.DrawSphere(Keys[j].EePos[1], 0.005f);
        }
         
        
        Gizmos.color = Color.green;        
        for(int j = 0; j < Metrics.NBins; j++)  {
        //    Debug.Log(metrics.nBins + " " + metrics.spaceHist[1][j]);
            Gizmos.DrawLine(new Vector3(j, 0,0), new Vector3(j, Metrics.SpaceHist[1][j],0));
        }

        Gizmos.color = Color.red;
        for(int j = 0; j < Metrics.NBins; j++) 
            Gizmos.DrawLine(new Vector3(j+Metrics.NBins, 0,0), new Vector3(j+Metrics.NBins, Metrics.WeightHist[1][j],0));
        
        Gizmos.color = Color.blue;
        for(int j = 0; j < Metrics.NBins; j++) 
            Gizmos.DrawLine(new Vector3(j+ 2* Metrics.NBins, 0,0), new Vector3(j+ 2* Metrics.NBins, Metrics.TimeHist[1][j],0));
                
        Gizmos.color = Color.yellow;
        for(int j = 0; j < Metrics.NBins; j++) 
            Gizmos.DrawLine(new Vector3(j+ 3* Metrics.NBins, 0,0), new Vector3(j+ 3* Metrics.NBins, Metrics.FlowHist[1][j],0));         
   

        //posture histogram 
        

        //for(int i = 0; i < 10; i++) {
        int i = 0;
            for (int j = 0; j < Metrics.NBins; j++)
            {
                Gizmos.color = Metrics.MaxPostureBin[i] == j ? Color.cyan : Color.magenta;
                if (Math.Abs(Metrics.PostureHist[i][j] - 0) < 0.00000001)
                    Gizmos.DrawSphere(new Vector3(j+ (i+4)* Metrics.NBins, 0,0),0.1f);
                else
                Gizmos.DrawLine(new Vector3(j+ (i+4)* Metrics.NBins, 0,0), new Vector3(j+ (i+4)* Metrics.NBins, Metrics.PostureHist[i][j],0));
            }
        // }






            //Visualize velocity of right arm
            
            //if(prevKeyInd < Keys.Length - 2)  {
            Gizmos.DrawLine(Vector3.zero, new Vector3(0, Keys[0].EeVel[1].magnitude * 40, 0));
            // for(int j = 1; j <= prevKeyInd; j++) {                    
            for (int j = 1; j < Keys.Length - 2; j++) {
                Gizmos.color = Color.white;
                Gizmos.DrawLine(new Vector3(j - 1, Keys[j - 1].EeVel[1].magnitude * 40, 0), new Vector3(j, Keys[j].EeVel[1].magnitude * 40, 0));
            //    Gizmos.DrawLine(Keys[j - 1].EeVel[1], Keys[j].EeVel[1]);
                //Acceleration
                Gizmos.color = Color.yellow;
                Gizmos.DrawLine(new Vector3(j - 1, Keys[j - 1].EeAcc[1].magnitude * 40, 0), new Vector3(j, Keys[j].EeAcc[1].magnitude * 40, 0));

                if (Math.Abs(Keys[j].EeVel[1].magnitude - 0) < 0.001)
                    Gizmos.color = Color.blue;
                if(PrevKeyInd == j)
                    Gizmos.color = Color.white;
                else
                    Gizmos.color = Color.red;
                Gizmos.DrawSphere(new Vector3(j , 0 ,0), 0.2f);

                Gizmos.color = Color.blue;
                Gizmos.DrawLine(new Vector3(j - 1, Metrics.Curvature[1][j - 1], 0), new Vector3(j, Metrics.Curvature[1][j ], 0));

            }

        //Draw curvature
           

            //Draw segments
            Gizmos.color = Color.black;
            for (int j = 1; j < Metrics.KeySegments.Count; j++)
                Gizmos.DrawSphere(new Vector3(Metrics.KeySegments[j], 0, 0), 0.2f); 
        
         /*   

        //Draw segments
        Gizmos.color = Color.black;
        for(int j = 1; j < metrics.keySegments.Count; j++)                  
            Gizmos.DrawSphere(new Vector3(metrics.keySegments[j],0,0), 0.5f);
        
        
        //Draw current key
        Gizmos.DrawLine(new Vector3(prevKeyInd, 0, 0), new Vector3(prevKeyInd,  5, 0));

        //Visualize acceleration of right arm
        Gizmos.color = Color.green;
        //if(prevKeyInd < Keys.Length - 2)  {
            Gizmos.DrawLine(Vector3.zero, new Vector3(0,Keys[0].EeAcc[1].magnitude*5,0));        
        //    for(int j = 1; j <= prevKeyInd; j++) {                    
                for(int j = 1; j < Keys.Length-2; j++) {                    
                Gizmos.DrawLine(new Vector3(j-1,Keys[j-1].EeAcc[1].magnitude*5,0), new Vector3(j,Keys[j].EeAcc[1].magnitude*5,0));        
            }
        //}
		
        //Visualize velocity of right arm
        Gizmos.color = Color.white;
        //if(prevKeyInd < Keys.Length - 2)  {
            Gizmos.DrawLine(Vector3.zero, new Vector3(0,Keys[0].EeVel[1].magnitude*5,0));        
           // for(int j = 1; j <= prevKeyInd; j++) {                    
            for(int j = 1; j < Keys.Length-2; j++) {  
                Gizmos.DrawLine(new Vector3(j-1,Keys[j-1].EeVel[1].magnitude*5,0), new Vector3(j,Keys[j].EeVel[1].magnitude*5,0));        
            }
//        }

        //Visualize space metric for right arm on the xy plane
        
        Gizmos.color = Color.yellow;
        cGraph = 10f; 
  //      if(prevKeyInd < Keys.Length - 2)  {
            Gizmos.DrawLine(Vector3.zero, new Vector3(0,metrics.spaceMetric[1,0]*cGraph,0));        
           // for(int j = 1; j <= prevKeyInd; j++) {                    
            for(int j = 1; j < Keys.Length-2; j++) {  
                Gizmos.DrawLine(new Vector3(j-1,metrics.spaceMetric[1,j-1]*cGraph,0), new Vector3(j,metrics.spaceMetric[1,j]*cGraph,0));        
            }
    //    }

        //Visualize weight metric for right arm on the xy plane 
	    Gizmos.color = Color.red;      
        cGraph = 10f;
      //  if(prevKeyInd < Keys.Length - 2)  {
            Gizmos.DrawLine(Vector3.zero, new Vector3(0,metrics.weightMetric[1,0]*cGraph,0));        
           // for(int j = 1; j <= prevKeyInd; j++) {                    
            for(int j = 1; j < Keys.Length-2; j++) {  
                Gizmos.DrawLine(new Vector3(j-1,metrics.weightMetric[1,j-1]*cGraph,0), new Vector3(j,metrics.weightMetric[1,j]*cGraph,0));        
            }
        //}
		


        //Visualize time metric for right arm on the xy plane
        cGraph = 10f;
        Gizmos.color = Color.blue;       
        //if(prevKeyInd < Keys.Length - 2)  {
            Gizmos.DrawLine(Vector3.zero, new Vector3(0,metrics.timeMetric[1,0]*cGraph,0));        
          //  for(int j = 1; j <= prevKeyInd; j++) {                    
            for(int j = 1; j < Keys.Length-2; j++) {  
                Gizmos.DrawLine(new Vector3(j-1,metrics.timeMetric[1,j-1]*cGraph,0), new Vector3(j,metrics.timeMetric[1,j]*cGraph,0));        
            }
        //}

        //Visualize flow metric for right arm on the xy plane
        cGraph = 1f;
        Gizmos.color = Color.magenta;       
        //if(prevKeyInd < Keys.Length - 2)  {
            Gizmos.DrawLine(Vector3.zero, new Vector3(0,metrics.flowMetric[1,0]*cGraph,0));        
          //  for(int j = 1; j <= prevKeyInd; j++) {                    
            for(int j = 1; j < Keys.Length-2; j++) {  
                Gizmos.DrawLine(new Vector3(j-1,metrics.flowMetric[1,j-1]*cGraph,0), new Vector3(j,metrics.flowMetric[1,j]*cGraph,0));        
            }
        //}
    */
             
        }
	
	
    
	
}
