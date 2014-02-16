#define WEBMODE


using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;

public class LMATuningGUI : MonoBehaviour {

    
    //High level
    private static float _space = 0f, _weight = 0f, _time = 0f, _flow = 0f;  //[-1 1]
    private static float _verArm = 0f, _horArm = 0f, _sagArm = 0f; //[-1 1] 
    private static float _verTorso = 0f, _horTorso = 0f, _sagTorso = 0f;
    private float _effortMin = -1.0f;
    private float _effortMax = 1.0f;
    private float _shapeMin = -1.0f;
    private float _shapeMax = 1.0f;
    //Low level
    
    static bool _toggleContinuous = false;

    
    private bool _shapeChanged = false;


    private string[] _actionStr = { "Custom", "Punch", "Dab", "Slash", "Flick", "Press", "Glide", "Wring", "Float" };
    private string[] _effortStr = { "Custom", "Ind", "Dir", "Lgt", "Str", "Sus", "Sud", "Fre", "Bnd" };
    private static int _action;

    private string[] _animNameStr = { "Knocking", "Pointing", "Lifting", "Picking up pillow", "Punching", "Pushing", "Throwing", "Walking", "Waving" };
//    static string[] persStr = { "Open-", "Open+", "Conscientious-", "Conscientious+", "Extrovert-", "Extrovert+", "Agreeable-", "Agreeable+", "Neurotic-", "Neurotic+" };
    private static string[] _persStr = { "OE-", "OE+", "NCA-", "NCA+" };
    private int _persInd = 0;
    private static int _animInd = 0;

    public string ShapeInfo = "";
    public string Info = "waiting...";
    private static Vector2 _scrollPosition;

    private static string _questionNo = "";
    
    private static string[] _isSubmittedStr = new string[36];

    private float _scrollWidth = 220;

    void Start(){
        //ResetTransforms();

        Reset();

        for (int i = 0; i < _isSubmittedStr.Length; i++)
            _isSubmittedStr[i] = "Answer NOT submitted";


        UpdateCameraBoundaries();
    }

    public void Reset() {
        _space = _weight = _time = _flow = 0f;
        _horArm = _verArm = _sagArm = 0f;
        _horTorso = _verTorso = _sagTorso = 0f;
        _persInd = 0;

        _action = 0;

     
        InitAgent(GameObject.Find("AgentPrefab"), "Knocking_Neutral_1");
        InitAgent(GameObject.Find("AgentControlPrefab"), "Knocking_Neutral_1");

        GameObject.Find("AgentPrefab").GetComponent<ArmAnimator>().ResetParameters();
        GameObject.Find("AgentControlPrefab").GetComponent<ArmAnimator>().ResetParameters();


//        ResetTransforms();

        StopAnim();


    }

    void UpdateCameraBoundaries() {
        GameObject cam1 = GameObject.Find("Camera1");
        GameObject cam2 = GameObject.Find("Camera2");
        GameObject cam3 = GameObject.Find("Camera3");

        cam1.camera.rect = new Rect(0, 0, _scrollWidth / Screen.width, 1); //320 is the width of the parameters
        cam2.camera.rect = new Rect(_scrollWidth / Screen.width, 0, ((Screen.width - _scrollWidth) / 2f) / Screen.width, 1);
        cam3.camera.rect = new Rect((Screen.width - (Screen.width - _scrollWidth) / 2f) / Screen.width, 0, ((Screen.width - _scrollWidth) / 2f) / Screen.width, 1);

    }

    void Update(){

        UpdateCameraBoundaries();
 
        if (Input.GetKeyDown("left")) {
            _persInd--;               
            if (_persInd < 0)
                _persInd = 0;        
            ResetEffort();
            ResetArmShape();
            ResetTorsoShape();
            UpdateEmoteParams();
        }
        else if (Input.GetKeyDown("right")) {
            _persInd++;               
            if (_persInd > 3)
                _persInd = 3;
            
            ResetEffort();
            ResetArmShape();
            ResetTorsoShape();
            UpdateEmoteParams();
        }
        else if (Input.GetKeyDown("up"))  {
            _animInd++;
            if (_animInd > 8)
                    _animInd = 8;
            _persInd = 0;             
            StopAnim(); 
            PlayAnim(_animInd); //start the next animation
            StopAnim(); //rewind to the start
            ResetEffort();
            ResetArmShape();
            ResetTorsoShape();
            UpdateEmoteParams();

        }
        else if(Input.GetKeyDown("down")) {
            _animInd--;            
            if (_animInd < 0)
            _animInd = 0;       
            _persInd = 0;
            StopAnim();                        
            PlayAnim(_animInd); //start the next animation
            StopAnim(); //rewint to the start
            ResetEffort();
            ResetArmShape();
            ResetTorsoShape();
            UpdateEmoteParams();
        }


        
    }
	

	void OnGUI () {

        GameObject agent = GameObject.Find("AgentPrefab");                
        GUIStyle style = new GUIStyle();


             
        GUILayout.BeginArea (new Rect (220,40,300,250));
        style.fontSize = 18;
        style.normal.textColor = Color.black;              
        GUILayout.Label(_questionNo, style);
        GUILayout.Label ("Animation: " + GetAnimName(_animInd), style);
        GUILayout.Label ("Personality type: " + _persStr[_persInd], style);
        GUILayout.Label(_isSubmittedStr[_animInd * 4 + _persInd], style);
    //     GUILayout.Label(""+GameObject.Find("AgentPrefab").GetComponent<AnimationInfo>().Curr, style);
        GUILayout.EndArea();


        GUILayout.BeginArea (new Rect (left: Screen.width/2f, top: Screen.height-150, width: 300, height: 200));
        GUILayout.Space (10);
        GUI.color = Color.black;
        _toggleContinuous =  GUILayout.Toggle(_toggleContinuous, "Animation looping");

        GUI.color = Color.white;

        if(GUILayout.Button ( "Play")) {
            PlayAnim(_animInd);
            //assign initial positions for torso and feet for the current animation
            agent.GetComponent<TorsoController>().AssignInitRootandFootPos();          
        }
            
        GUILayout.Label("");
        GUILayout.BeginHorizontal ();	
        GUI.color = Color.white;        

        if(GUILayout.Button ( "Previous question")) {    
            _persInd--;               
            if (_persInd < 0) {
                _persInd = 3;   
                    _animInd--;
                if(_animInd < 0) {
                    _animInd = 0;
                    _persInd = 0;
                }
            }
               
            ResetEffort();
            ResetArmShape();
            ResetTorsoShape();
            UpdateEmoteParams();
                
        }        
        GUI.color = Color.white;    
        if(GUILayout.Button ( "Next question")) {                  
            _persInd++;               
            if (_persInd > 3) {
                _persInd = 0;
                _animInd++;
                if (_animInd > 8){
                    _animInd = 8;
                    _persInd = 3;                        
                }
            }
            ResetEffort();
            ResetArmShape();
            ResetTorsoShape();
            UpdateEmoteParams();
        }
        GUILayout.EndHorizontal ();


        GUI.color = Color.white;  

        if(GUILayout.Button("Submit")){
            _isSubmittedStr[_animInd * 4 + _persInd] = "Answer submitted";
               
            #if !WEBMODE
                RecordValues();  
            #elif WEBMODE
                this.StartCoroutine(PostValues());
            #endif

        }
         
        GUILayout.EndArea();
     
        _scrollPosition = GUILayout.BeginScrollView(_scrollPosition,  GUILayout.Width(220f), GUILayout.Height(Screen.height*0.98f));
            
            
        //style.fontSize = 22;
        //style.normal.textColor = Color.black;      
        //GUILayout.Label ("User ID:  " + UserInfo.userId, style);
        GUI.color = Color.white;
        
           
        GUI.color = Color.black;
         //EMOTE PARAMETERS: Effort, arm shape, torso shape
            GUILayout.Space (10);
            style.fontSize = 15;
            style.normal.textColor = Color.black;
            GUILayout.Label ("Effort", style);
            GUI.color = Color.grey;
            //Space        
            GUILayout.BeginHorizontal ();	    
	        GUILayout.Label("");
	        GUILayout.Label("Indirect");
	        GUILayout.Label("Direct");
	        GUILayout.EndHorizontal ();            	    
            _space = GUILayout.HorizontalSlider(_space,_effortMin, _effortMax);
        //      space = Mathf.Round(space);
            

            //Weight
            GUILayout.BeginHorizontal ();	    
	        GUILayout.Label("");
	        GUILayout.Label("Light");
	        GUILayout.Label("Strong");
	        GUILayout.EndHorizontal ();            	    
            _weight = GUILayout.HorizontalSlider(_weight,_effortMin, _effortMax);
        //    weight = Mathf.Round(weight);
            
            //Time
            GUILayout.BeginHorizontal ();	    
	        GUILayout.Label("");
	        GUILayout.Label("Sustained");
	        GUILayout.Label("Sudden");
	        GUILayout.EndHorizontal ();            	    
            _time = GUILayout.HorizontalSlider(_time,_effortMin, _effortMax);
            //   time = Mathf.Round(time);

            //Flow
            GUILayout.BeginHorizontal ();	    
	        GUILayout.Label("");
	        GUILayout.Label("Free");
	        GUILayout.Label("Bound");
	        GUILayout.EndHorizontal ();            	    
            _flow = GUILayout.HorizontalSlider(_flow,_effortMin, _effortMax);
            // flow = Mathf.Round(flow);

            GUI.color = Color.black;
            GUILayout.Label("Presets", style);
            GUI.color = Color.white;
            _action = GUILayout.SelectionGrid(_action, _actionStr,2);

            switch(_action) {
                case 0:
                    break;
                case 1:
                    _weight = 1f;
                    _space = 1f;
                    _time = 1f;
                    break;
                case 2:
                    _weight = -1f;
                    _space = 1f;
                    _time = 1f;
                    break;
                case 3:
                    _weight = 1f;
                    _space = -1f;
                    _time = 1f;
                    break;
                case 4:
                    _weight = -1f;
                    _space = -1f;
                    _time = 1f;
                    break;
                case 5:
                    _weight = 1f;
                    _space = 1f;
                    _time = -1f;
                    break;
                case 6:
                    _weight = -1f;
                    _space = 1f;
                    _time = -1f;
                    break;
                case 7:
                    _weight = 1f;
                    _space = -1f;
                    _time = -1f;
                    break;
                case 8:
                    _weight = -1f;
                    _space = -1f;
                    _time = -1f;
                    break;                
            }
        
            GUILayout.Space (10);
                GUI.color = Color.white;
            if(GUILayout.Button ( "Reset Effort")) {
                ResetEffort();
               
		            //ResetTransforms();			
            }

         
            GUILayout.Space (10);

            style.fontSize = 15;
            style.normal.textColor = Color.black;
            GUILayout.Label ("Arm Shape", style);
            GUI.color = Color.grey;

            bool changed1 = GUI.changed;
            GUILayout.Label("Horizontal");
            _horArm = GUILayout.HorizontalSlider (_horArm, _shapeMin, _shapeMax);        
            GUILayout.Label("Vertical");
		    _verArm = GUILayout.HorizontalSlider ( _verArm, _shapeMin, _shapeMax);
            GUILayout.Label("Sagittal");
		    _sagArm = GUILayout.HorizontalSlider (_sagArm, _shapeMin, _shapeMax);

            _shapeChanged = false;
            GUILayout.Space (10);
            GUI.color = Color.white; 
            if(GUILayout.Button ( "Reset Arm Shape")) {
                ResetArmShape();
                	
		        _shapeChanged = true;    		
            }

            if(GUI.changed && changed1 == false) {
                _shapeChanged = true;
            }

         

                GUILayout.Space (10);
            style.fontSize = 15;
            style.normal.textColor = Color.black;
            GUILayout.Label ("Torso Shape", style);
		    GUI.color = Color.grey;        
		    GUILayout.BeginHorizontal ();
		    GUILayout.Label("");        
		    GUILayout.Label("Enclosing");
		    GUILayout.Label("Spreading");		
		    GUILayout.EndHorizontal ();
		    _horTorso = GUILayout.HorizontalSlider (_horTorso, _shapeMin, _shapeMax);
		        
		    GUILayout.BeginHorizontal ();
		    GUILayout.Label("");
		    GUILayout.Label("Sinking");
		    GUILayout.Label("Rising");		
		    GUILayout.EndHorizontal ();
		    _verTorso = GUILayout.HorizontalSlider (_verTorso, _shapeMin, _shapeMax);
		        
		    GUILayout.BeginHorizontal ();
		    GUILayout.Label("");
		    GUILayout.Label("Retreating");
		    GUILayout.Label("Advancing");		
		    GUILayout.EndHorizontal ();
		    _sagTorso = GUILayout.HorizontalSlider (_sagTorso, _shapeMin, _shapeMax);
		       

            GUILayout.Space (10);
            GUI.color = Color.white;
		    if(GUILayout.Button("Reset Torso Shape")){
                ResetTorsoShape();
			    
		    }

         
            UpdateEmoteParams();

            _questionNo = "Question: " + (_animInd * 4 + _persInd + 1) + " of 36";
       

        
            GUILayout.EndScrollView();                  
        
         
        
	}


    void StopAnim(){
        GameObject agent = GameObject.Find("AgentPrefab");	
        GameObject agentControl = GameObject.Find("AgentControlPrefab");	        
	    if(agent.GetComponent<ArmAnimator>().ArmC == null  ||agent.GetComponent<TorsoAnimator>().TorsoC == null) {
			Debug.Log ("Controller not assigned");
			return;
		}

        if(agent.animation.isPlaying) {
           agent.SampleAnimation(agent.animation.clip, 0); //instead of rewind
           agent.animation.Stop();           
        }
        if(agentControl && agentControl.animation.isPlaying) {
            agentControl.SampleAnimation(agentControl.animation.clip, 0);//instead of rewind            
            agentControl.animation.Stop();
        }        
     
    }

    void InitAgent(GameObject agent, string animName) {
        if (!agent) 
            return;
        agent.GetComponent<AnimationInfo>().Reset(animName);                            
        agent.GetComponent<ArmAnimator>().Reset();                
        agent.GetComponent<TorsoAnimator>().Reset();

        //Read values for the shape parameters 
        //Need to call this only once
#if !WEBMODE
        for (int i = 0; i < 6; i++)
            ReadValuesShapes(i);
#elif WEBMODE
        for (int i = 0; i < 6; i++)
            this.StartCoroutine(GetValuesShapes(i));
#endif
        
        
        agent.animation.enabled = true;                        
        agent.animation.Play(animName);      

    }

    void PlayAnim(int ind) {
        GameObject agent = GameObject.Find("AgentPrefab");	
        GameObject agentControl = GameObject.Find("AgentControlPrefab");	
        
	    if(agent.GetComponent<ArmAnimator>().ArmC == null  ||agent.GetComponent<TorsoAnimator>().TorsoC == null) {
			Debug.Log ("Controller not assigned");
			return;
		}
     
        AnimationInfo animInfo = agent.GetComponent<AnimationInfo>();
        animInfo.IsContinuous = _toggleContinuous;
        if(agentControl)
            agentControl.GetComponent<AnimationInfo>().IsContinuous = _toggleContinuous;

        agent.animation.Stop(); //in order to restart animation
        if(agentControl)
            agentControl.animation.Stop(); //in order to restart animation
        
        
        switch(ind) {
            case 0:
                InitAgent(agent, "Knocking_Neutral_1");
                if(agentControl) {
                    InitAgent(agentControl, "Knocking_Neutral_1");                 
                    agentControl.animation[animInfo.AnimName].speed = animInfo.DefaultAnimSpeed;
                }
                     
                break;
            case 1:
                InitAgent(agent, "Pointing_to_Spot_Netural_02");
                if(agentControl) {                
                    InitAgent(agentControl, "Pointing_to_Spot_Netural_02");                                        
                    agentControl.animation[animInfo.AnimName].speed = animInfo.DefaultAnimSpeed;
                }
                break;
            case 2:
                InitAgent(agent, "Lifting_Netural_01");
                if(agentControl) {                
                    InitAgent(agentControl, "Lifting_Netural_01");                                        
                    agentControl.animation[animInfo.AnimName].speed = animInfo.DefaultAnimSpeed;
                }
              
                break;
            case 3:
                InitAgent(agent, "Picking_Up_Pillow_Netural_01");
                if(agentControl) {                
                    InitAgent(agentControl, "Picking_Up_Pillow_Netural_01");                                        
                    agentControl.animation[animInfo.AnimName].speed = animInfo.DefaultAnimSpeed;
                }
              
                break;
            case 4:
                 InitAgent(agent, "Punching_Netural_02");
                if(agentControl) {                
                    InitAgent(agentControl, "Punching_Netural_02");                                        
                    agentControl.animation[animInfo.AnimName].speed = animInfo.DefaultAnimSpeed;
                }
                
                break;
            case 5:
                InitAgent(agent, "Pushing_Netural_02");
                if(agentControl) {                
                    InitAgent(agentControl, "Pushing_Netural_02");                                        
                    agentControl.animation[animInfo.AnimName].speed = animInfo.DefaultAnimSpeed;          
                }
                
                break;
            case 6:
                 InitAgent(agent, "Throwing_Netural_02");
                if(agentControl) {                
                    InitAgent(agentControl, "Throwing_Netural_02");                                        
                    agentControl.animation[animInfo.AnimName].speed = animInfo.DefaultAnimSpeed;             
                }
                
                break;
            case 7:
                 InitAgent(agent, "Walking_Netural_02");
                if(agentControl) {                
                    InitAgent(agentControl, "Walking_Netural_02");                                        
                    agentControl.animation[animInfo.AnimName].speed = animInfo.DefaultAnimSpeed;
                }
                
                break;
            case 8:
                 InitAgent(agent, "Waving_Netural_02");
                 if(agentControl) {              
                    InitAgent(agentControl, "Waving_Netural_02");                                        
                    agentControl.animation[animInfo.AnimName].speed = animInfo.DefaultAnimSpeed;
                 }       
               
                break;                    
        }

        if(animInfo.IsContinuous){
            agent.animation[animInfo.AnimName].wrapMode = WrapMode.Loop;
            if(agentControl)                
                agentControl.animation[animInfo.AnimName].wrapMode = WrapMode.Loop;           
           
        }
        else{
            agent.animation[animInfo.AnimName].wrapMode = WrapMode.Once;
            if(agentControl)
                agentControl.animation[animInfo.AnimName].wrapMode = WrapMode.Once;
           
        }        
        

    }
    string GetAnimName(int ind) {
       GameObject agent = GameObject.Find("AgentPrefab");	
	    if(agent.GetComponent<ArmAnimator>().ArmC == null  ||agent.GetComponent<TorsoAnimator>().TorsoC == null) {
			Debug.Log ("Controller not assigned");
			return "";
		}
        return _animNameStr[ind];        
    }
    public void ResetTransforms() {
		GameObject agent = GameObject.Find("AgentPrefab");	
		
		if(agent.GetComponent<ArmAnimator>().ArmC == null  ||agent.GetComponent<TorsoAnimator>().TorsoC == null) {
			Debug.Log ("Controller not assigned");
			return;
		}
			
			
		agent.GetComponent<ArmAnimator>().ArmC.ResetTransforms();
		agent.GetComponent<TorsoAnimator>().TorsoC.ResetTransforms();
	}

    
    void ResetEffort(){            
        _space = _weight = _time = _flow = 0f;        
        _action = 0;
    }
    void ResetArmShape(){
            _horArm = _verArm = _sagArm = 0f;			
        
    }
    void ResetTorsoShape(){
        _horTorso = _verTorso = _sagTorso = 0f;	
    }

  
	void UpdateEmoteParams() {
		GameObject agent = GameObject.Find("AgentPrefab");
		if(agent == null){		
			Debug.Log("AgentPrefab not found");
			return;
		}

        if(_shapeChanged) {
	        //Update arm Shape params
            //Left arm
	        agent.GetComponent<ArmAnimator>().Hor = _horArm;
	        agent.GetComponent<ArmAnimator>().Ver = _verArm;
	        agent.GetComponent<ArmAnimator>().Sag = _sagArm;		
	        agent.GetComponent<ArmAnimator>().UpdateKeypointsByShape(0); //Update keypoints
	        //RightArm 
            //Only horizontal motion is the opposite for each arm
	        agent.GetComponent<ArmAnimator>().Hor = -_horArm;
	        agent.GetComponent<ArmAnimator>().UpdateKeypointsByShape(1); //Update keypoints
        }
        	 	 
		
	    //Update arm Effort params
	    //Space
	    if(_space > 0){
		    agent.GetComponent<ArmAnimator>().Dir = _space;
		    agent.GetComponent<ArmAnimator>().Ind = 0f;
	    }
	    else{
		    agent.GetComponent<ArmAnimator>().Ind = -_space;
		    agent.GetComponent<ArmAnimator>().Dir = 0f;
	    }
	    //Weight
	    if(_weight > 0){
		    agent.GetComponent<ArmAnimator>().Str = _weight;
		    agent.GetComponent<ArmAnimator>().Lgt = 0f;
	    }
	    else{
		    agent.GetComponent<ArmAnimator>().Lgt = -_weight;
		    agent.GetComponent<ArmAnimator>().Str = 0f;
	    }
	    //Time				
	    if(_time > 0){
		    agent.GetComponent<ArmAnimator>().Sud = _time;
		    agent.GetComponent<ArmAnimator>().Sus = 0f;
	    }
	    else{
		    agent.GetComponent<ArmAnimator>().Sus = -_time;
		    agent.GetComponent<ArmAnimator>().Sud = 0f;
	    }
		
	    //Flow
	    if(_flow > 0){
		    agent.GetComponent<ArmAnimator>().Bnd = _flow;
		    agent.GetComponent<ArmAnimator>().Fre = 0f;
	    }
	    else{
		    agent.GetComponent<ArmAnimator>().Fre = -_flow	;
		    agent.GetComponent<ArmAnimator>().Bnd = 0;
	    }
		
		
	    //Update effort parameters
	    agent.GetComponent<ArmAnimator>().Effort2LowLevel();
        


	    //Update torso shape parameters
        //TODO : UPDATE
        agent.GetComponent<TorsoAnimator>().EncSpr[0] = agent.GetComponent<TorsoAnimator>().EncSpr[1] = _horTorso;
        agent.GetComponent<TorsoAnimator>().SinRis[0] = agent.GetComponent<TorsoAnimator>().SinRis[1] = _verTorso;
        agent.GetComponent<TorsoAnimator>().RetAdv[0] = agent.GetComponent<TorsoAnimator>().RetAdv[1] = _sagTorso;

        agent.GetComponent<TorsoAnimator>().UpdateAnglesLinearComb();

    }

    // remember to use StartCoroutine when calling this function!
    IEnumerator GetValuesShapes(int shapeInd) {
        string resultURL = "https://fling.seas.upenn.edu/~fundad/cgi-bin/RCTAMAN/getShapeData.php";

        // Create a form object for sending high score data to the server
        var form = new WWWForm();
        form.AddField("userId", "susand948");
        form.AddField("shapeInd", shapeInd.ToString());
        // Create a download object


        var download = new WWW(resultURL, form);

        // Wait until the download is done
        yield return download;


        //reset anyway
        TorsoController torso = GameObject.Find("AgentPrefab").GetComponent<TorsoController>();
        torso.Reset();

        if (download.error != null) {
            ShapeInfo = download.error;
            print("Error: " + download.error);
        }
        else {
            ShapeInfo = download.text;
            String[] vals = ShapeInfo.Split('\t');
            
            //Assign shape values 
            TorsoAnimator torsoAnim = GameObject.Find("AgentPrefab").GetComponent<TorsoAnimator>();

            int i = 0;
            torsoAnim.ShapeParams[(int)BPart.HeadX][shapeInd] = float.Parse(vals[i++]);
            torsoAnim.ShapeParams[(int)BPart.NeckX][shapeInd] = float.Parse(vals[i++]);
            torsoAnim.ShapeParams[(int)BPart.SpineY][shapeInd] = float.Parse(vals[i++]);
            torsoAnim.ShapeParams[(int)BPart.Spine1X][shapeInd] = float.Parse(vals[i++]);
            torsoAnim.ShapeParams[(int)BPart.ShouldersX][shapeInd] = float.Parse(vals[i++]);
            torsoAnim.ShapeParams[(int)BPart.ShouldersY][shapeInd] = float.Parse(vals[i++]);
            torsoAnim.ShapeParams[(int)BPart.ShouldersZ][shapeInd] = float.Parse(vals[i++]);
            torsoAnim.ShapeParams[(int)BPart.ClaviclesX][shapeInd] = float.Parse(vals[i++]);
            torsoAnim.ShapeParams[(int)BPart.ClaviclesY][shapeInd] = float.Parse(vals[i++]);
            torsoAnim.ShapeParams[(int)BPart.ClaviclesZ][shapeInd] = float.Parse(vals[i++]);
            torsoAnim.ShapeParams[(int)BPart.PelvisLX][shapeInd] = float.Parse(vals[i++]);
            torsoAnim.ShapeParams[(int)BPart.PelvisRX][shapeInd] = float.Parse(vals[i++]);
            torsoAnim.ShapeParams[(int)BPart.PelvisY][shapeInd] = float.Parse(vals[i++]);
            torsoAnim.ShapeParams[(int)BPart.PelvisZ][shapeInd] = float.Parse(vals[i++]);
            torsoAnim.ShapeParams[(int)BPart.KneesX][shapeInd] = float.Parse(vals[i++]);
            torsoAnim.ShapeParams[(int)BPart.HipsX][shapeInd] = float.Parse(vals[i++]);
            torsoAnim.ShapeParams[(int)BPart.ToesX][shapeInd] = float.Parse(vals[i++]);
            torsoAnim.ShapeParams[(int)BPart.SpineLength][shapeInd] = float.Parse(vals[i++]);

        }
    }

        
    void ReadValuesShapes(int shapeInd) {        
        
        string fileName = "shapesSusan.txt";
        StreamReader sr = new StreamReader(fileName);

        
        string[] content = File.ReadAllLines(fileName);

        String[] tokens = content[shapeInd + 1].Split('\t');

        TorsoAnimator torsoAnim = GameObject.Find("AgentPrefab").GetComponent<TorsoAnimator>();
        int i = 2;

        torsoAnim.ShapeParams[(int)BPart.HeadX][shapeInd] = float.Parse(tokens[i++]);
        torsoAnim.ShapeParams[(int)BPart.NeckX][shapeInd] = float.Parse(tokens[i++]);
        torsoAnim.ShapeParams[(int)BPart.SpineY][shapeInd] = float.Parse(tokens[i++]);
        torsoAnim.ShapeParams[(int)BPart.Spine1X][shapeInd] = float.Parse(tokens[i++]);
        torsoAnim.ShapeParams[(int)BPart.ShouldersX][shapeInd] = float.Parse(tokens[i++]);
        torsoAnim.ShapeParams[(int)BPart.ShouldersY][shapeInd] = float.Parse(tokens[i++]);
        torsoAnim.ShapeParams[(int)BPart.ShouldersZ][shapeInd] = float.Parse(tokens[i++]);
        torsoAnim.ShapeParams[(int)BPart.ClaviclesX][shapeInd] = float.Parse(tokens[i++]);
        torsoAnim.ShapeParams[(int)BPart.ClaviclesY][shapeInd] = float.Parse(tokens[i++]);
        torsoAnim.ShapeParams[(int)BPart.ClaviclesZ][shapeInd] = float.Parse(tokens[i++]);
        torsoAnim.ShapeParams[(int)BPart.PelvisLX][shapeInd] = float.Parse(tokens[i++]);
        torsoAnim.ShapeParams[(int)BPart.PelvisRX][shapeInd] = float.Parse(tokens[i++]);
        torsoAnim.ShapeParams[(int)BPart.PelvisY][shapeInd] = float.Parse(tokens[i++]);
        torsoAnim.ShapeParams[(int)BPart.PelvisZ][shapeInd] = float.Parse(tokens[i++]);
        torsoAnim.ShapeParams[(int)BPart.KneesX][shapeInd] = float.Parse(tokens[i++]);
        torsoAnim.ShapeParams[(int)BPart.HipsX][shapeInd] = float.Parse(tokens[i++]);
        torsoAnim.ShapeParams[(int)BPart.ToesX][shapeInd] = float.Parse(tokens[i++]);
        torsoAnim.ShapeParams[(int)BPart.SpineLength][shapeInd] = float.Parse(tokens[i++]);
        sr.Close();
    }

    // remember to use StartCoroutine when calling this function!
    IEnumerator PostValues() {
        string resultURL = "https://fling.seas.upenn.edu/~fundad/cgi-bin/RCTAMAN/putData.php";
               
     // Create a form object for sending high score data to the server
        var form = new WWWForm();        
        form.AddField( "userId", UserInfo.userId);        
        form.AddField( "animName", GetAnimName(_animInd));        
        form.AddField( "persName", _persStr[_persInd] );
        form.AddField( "space", _space.ToString());
        form.AddField( "weight", _weight.ToString());
        form.AddField( "time", _time.ToString());
        form.AddField( "flow", _flow.ToString());
        form.AddField( "horArm", _horArm.ToString());
        form.AddField( "verArm", _verArm.ToString());
        form.AddField( "sagArm", _sagArm.ToString());
        form.AddField( "horTorso", _horTorso.ToString());
        form.AddField( "verTorso", _verTorso.ToString());
        form.AddField( "sagTorso", _sagTorso.ToString());

        // Create a download object
        var download = new WWW( resultURL, form );

        // Wait until the download is done
        yield return download;

        if(download.error!= null) {
            Info = download.error;
            print( "Error: " + download.error );                         
        } else {
            Info = "success " + download.text;                        
        }
    }

    void RecordValues(){
  
        string fileName = "results.txt";     
        if(!File.Exists(fileName)){
            StreamWriter sw = new StreamWriter(fileName);
            
            //Blank lines
            for(int i = 0; i< 9; i++) {
                
                sw.WriteLine(_animNameStr[i]);                        
                sw.WriteLine("Personality\tSpace\tWeight\tTime\tFlow\tHorArm\tVerArm\tSagArm\tHorTorso\tVerTorso\tSagTorso");
                for (int j = 0; j < _persStr.Length; j++)
                    sw.WriteLine(_persStr[j] + "\t0.0000\t0.0000\t0.0000\t0.0000\t0.0000\t0.0000\t0.0000\t0.0000\t0.0000\t0.0000");
            }

                        
            sw.Close();
        }
         

        
        string[] content = File.ReadAllLines(fileName);        
        //Debug.Log(persInd);
        content[(_persStr.Length + 2) *_animInd + _persInd + 2] = string.Format(_persStr[_persInd] + "\t{0:0.0000}\t{1:0.0000}\t{2:0.0000}\t{3:0.0000}\t{4:0.0000}\t{5:0.0000}\t{6:0.0000}\t{7:0.0000}\t{8:0.0000}\t{9:0.0000}",
                                    _space, _weight, _time, _flow, _horArm, _verArm, _sagArm, _horTorso, _verTorso, _sagTorso);
        
        
        using(StreamWriter sw = new StreamWriter(fileName)) {
            for (int i = 0; i < content.Length; i++)
                sw.WriteLine(content[i]);
        }
      
    }
    }		

