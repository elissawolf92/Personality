#define WEBMODE


using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;

public class LMAScalingGUI : MonoBehaviour {

    private float[] _speed = new float[32];
    private float[] _v0 = new float[32];
    private float[] _v1 = new float[32];
    private float[] _ti = new float[32];
    private float[] _texp = new float[32];
    private float[] _tval = new float[32];
    private float[] _continuity = new float[32];
    private float[] _bias = new float[32];
    private float[] _t0 = new float[32];
    private float[] _t1 = new float[32];

    //Flourishes
    private float[] _trMag = new float[32]; //torso rotation
    private float[] _tfMag = new float[32];

    private float[] _hrMag = new float[32]; //head rotation
    private float[] _hfMag = new float[32];
    private float[] _squashMag = new float[32];
    private float[] _wbMag = new float[32];
    private float[] _wxMag = new float[32];
    private float[] _wtMag = new float[32];
    private float[] _wfMag = new float[32];
    private float[] _etMag = new float[32];
    private float[] _dMag = new float[32];
    private float[] _efMag = new float[32];

    //Shape for drives
    private float[] _encSpr0 = new float[32];
    private float[] _sinRis0 = new float[32];
    private float[] _retAdv0 = new float[32];

    private float[] _encSpr1 = new float[32];
    private float[] _sinRis1 = new float[32];
    private float[] _retAdv1 = new float[32];

    //Arm shape for drives
    private static Vector3[][] _arm = new Vector3[32][];

    static bool _toggleContinuous = false;


    
    private string[] _animNameStr = { "Pointing", "Picking up a pillow"};
    private static int _animInd = 0;

    int _driveInd = 0;
    static int _qInd = 0; //question index
    private int[,] _effortList =  {{-1, -1, -1, 0}, {-1, -1, 1, 0}, {-1, 1, -1, 0}, {-1, 1, 1, 0}, {1, -1, -1, 0}, {1, -1, 1, 0}, {1, 1, -1, 0}, {1, 1, 1, 0}, 
                                       {-1, -1, 0, -1}, {-1, -1, 0, 1},{-1, 1,0,  -1}, {-1, 1, 0, 1}, {1, -1, 0, -1},{1, -1, 0,  1}, {1, 1, 0, -1}, {1, 1, 0, 1},
                                        {-1,  0, -1, -1}, {-1, 0, -1,  1}, {-1, 0, 1,  -1}, {-1, 0, 1, 1}, {1, 0, -1, -1}, {1, 0, -1,  1}, {1, 0, 1, -1}, {1,  0, 1, 1},
                                       { 0, -1, -1, -1}, {0, -1, -1,  1}, {0, -1, 1,  -1}, {0, -1, 1, 1}, {0, 1, -1, -1}, {0, 1, -1,  1}, {0, 1, 1, -1}, {0, 1, 1, 1}};


    Dictionary<int, int> _effortCombination = new Dictionary<int , int>();

    private string[] _effortNames = { "Space", "Weight", "Time", "Flow" };

    static int _answerOe = 2, _answerNca = 2;
    private float[] _oe = new float[32 * 2]; //effort combination cnt * animCnt    
    private float[] _nca = new float[32 * 2];

    public string ShapeInfo = "";
    public string Info = "waiting...";
    private static Vector2 _scrollPosition;

    private static string _questionNo = "";
    
    private static string[] _isSubmittedStr = new string[32 * 2];
    private static bool[] _isSubmitted= new bool[32 * 2];
    private int _submittedCnt = 0;

    private float _scrollWidth = 0;

    GameObject _agent;
    void Start(){

        for (int i = 0; i < 32; i++) {
            _arm[i] = new Vector3[2];
        }
            
        _agent = GameObject.Find("AgentPrefabLeft");



        if (!_agent) {
            
            Debug.Log("AgentLeft prefab not found");
            return;
        }
        
        _qInd = 0;
       

        Reset();

       
        for (int i = 0; i < _isSubmittedStr.Length; i++)
            _isSubmittedStr[i] = "Answer NOT submitted";

        UpdateCameraBoundaries();


        //Read all drive parameters
#if !WEBMODE
        for (int i = 0; i < 32; i++)
            ReadValuesDrives(i);
#elif WEBMODE

        for (int i = 0; i < 32; i++)
            this.StartCoroutine(GetValuesDrives(i));
#endif
       

    }


    public void Reset() {
        for (int i = 0; i < _oe.Length; i++) {
            _oe[i] = _nca[i] = 0;
        }

        InitAgent(_agent, "Pointing_to_Spot_Netural_02_Updated");
        

        UpdateParameters();

        StopAnim(_agent);
        
    }

    void UpdateCameraBoundaries() {
        GameObject cam1 = GameObject.Find("Camera1");
        GameObject cam2 = GameObject.Find("Camera2");
        GameObject cam3 = GameObject.Find("Camera3");

        cam1.camera.rect = new Rect(0, 0, _scrollWidth / Screen.width, 1); //320 is the width of the parameters
        cam2.camera.rect = new Rect(_scrollWidth / Screen.width, 0, Screen.width, Screen.height);
        cam3.camera.rect = new Rect(0,0,0,0);

    }

   

    void Update(){

        UpdateCameraBoundaries();

        

        if (Input.GetKeyDown("left")) {
            _qInd--;
            if (_qInd < 0 ) {
                if (_animInd > 0) {
                    _qInd = 31;
                    _animInd--;
                }
                else
                    _qInd = 0;
            }


            UpdateParameters();
        }
        else if (Input.GetKeyDown("right")) {

            _qInd++;

            if (_qInd > 31 && _animInd < _animNameStr.Length) {
                _qInd = 0;
                _animInd++;
                if (_animInd >= _animNameStr.Length) {
                    _animInd = _animNameStr.Length - 1;
                    _qInd = 31;

                }

            }
          
            UpdateParameters();
        }
        else if (Input.GetKeyDown("up"))  {
            _animInd++;
            if (_animInd >= _animNameStr.Length)
                    _animInd = _animNameStr.Length - 1;

            _qInd = 0;

            UpdateParameters();
        }
        else if(Input.GetKeyDown("down")) {
            _animInd--;            
            if (_animInd < 0)
                _animInd = 0;

            _qInd = 0;
            UpdateParameters();

        }


        
    }
    
  

    void UpdateParameters() {



        UpdateEmoteParams(_agent, _qInd);

       

    }
	

	void OnGUI () {
        
        GUIStyle style = new GUIStyle();

   
        
        GUILayout.BeginArea (new Rect (220,10,300,250));
        GUILayout.BeginHorizontal ();	
        GUI.color = Color.black;
        style.fontSize = 22;
      
        GUILayout.EndHorizontal ();	
        
        GUILayout.EndArea();


        string[] scalePlasticityStr = { "Highly rigid", "Somewhat rigid", "Neutral", "Somewhat plastic", "Highly plastic" };
        string[] scaleStabilityStr = { "Highly unstable", "Somewhat unstable", "Neutral", "Somewhat stable", "Highly stable" };

        //this.StartCoroutine(GetValuesAnswers(_qInd));
           
        GUILayout.BeginArea (new Rect (20,40,250,250));
        style.fontSize = 18;
        style.normal.textColor = Color.black;              
        
        GUILayout.Label ("Animation: " + GetAnimName(_animInd), style);
        GUILayout.Label(ComputeEffortCombinationStr(_qInd));
        //GUILayout.Label("Effort combination: " + (_driveInd + 1) + " of 32", style);
        
        GUILayout.EndArea();

        GUI.color = Color.gray;
        _scrollPosition = GUI.BeginScrollView(new Rect(Screen.width/2f-250, Screen.height-260f, 1000, 250), _scrollPosition, new Rect(0, 0, 220, 230));

       
        GUI.color = Color.white;

        if(GUILayout.Button ( "Play")) {

            
            _agent.GetComponent<TorsoController>().AssignInitRootandFootPos();


            PlayAnim(_agent, _animInd);

            UpdateParameters();

        }


        

        GUILayout.Label("");
        GUI.color = Color.black;

        GUILayout.Label("How plastic (flexible and social) does the agent look?");
        GUI.color = Color.white;

        _answerOe = GUILayout.SelectionGrid(_answerOe, scalePlasticityStr,5);
            
        GUI.color = Color.black;
        GUILayout.Label("How stable does the agent look? ");
        GUI.color = Color.white;
        _answerNca = GUILayout.SelectionGrid(_answerNca, scaleStabilityStr, 5);
            
        

        
        if (_submittedCnt ==  64) {
            style.normal.textColor = Color.red;
            GUILayout.Label("Study is completed. Thank you!", style);
        }
        else
            GUILayout.Label("");

        style.normal.textColor = Color.black;
        GUILayout.Label(_questionNo, style);
        style.fontSize = 14;
        GUILayout.Label(_isSubmittedStr[_animInd * 32 + _qInd], style);

        GUI.color = Color.white;
        
        if (GUILayout.Button("Submit")) {
            if (_isSubmitted[_animInd * 32 + _qInd] == false) {
                _isSubmitted[_animInd * 32 + _qInd] = true;
                _submittedCnt++;
            }

            
            _isSubmittedStr[_animInd * 32 + _qInd] = "Answer submitted";
#if !WEBMODE
            RecordValues(_qInd, _answerOe, _answerNca);
#elif WEBMODE
            this.StartCoroutine(PostValues(_qInd, _answerOe, _answerNca));
#endif

            
            _qInd++;
            if (_qInd > 31  && _animInd < _animNameStr.Length) {
                   _qInd = 0;
                _animInd++;
                if (_animInd >= _animNameStr.Length) {                    
                    _animInd = _animNameStr.Length - 1;
                    _qInd = 31;
                    
                }

            }

            StopAnim(_agent);
            PlayAnim(_agent, _animInd); //start the next animation
            StopAnim(_agent);
            
            UpdateParameters();



            _answerOe = 2;
            _answerNca = 2;


        }
         
        GUI.EndScrollView();

        _questionNo = "Question: " + (_animInd * 32 + _qInd + 1) + " of 64";

               
	}


    string ComputeEffortCombinationStr(int driveInd) {
        string str = "";
        if (_effortList[driveInd,0] == -1)
            str += "Indirect";
        else if (_effortList[driveInd,0] == 1)
            str += "Direct";

        str += " ";
        if (_effortList[driveInd,1] == -1)
            str += "Light";
        else if (_effortList[driveInd,1] == 1)
            str += "Strong";

        str += " ";
        if (_effortList[driveInd,2] == -1)
            str += "Sustained";
        else if (_effortList[driveInd,2] == 1)
            str += "Sudden";


        str += " ";
        if (_effortList[driveInd,3] == -1)
            str += "Free";
        else if (_effortList[driveInd,3] == 1)
            str += "Bound";

        return str;

    }


    void StopAnim(GameObject agent){
        if (agent.GetComponent<ArmAnimator>().ArmC == null || agent.GetComponent<TorsoAnimator>().TorsoC == null) {
			Debug.Log ("Controller not assigned");
			return;
		}

        if (agent.animation.isPlaying) {
            agent.SampleAnimation(agent.animation.clip, 0); //instead of rewind
            agent.animation.Stop();           
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
        for (int i = 5; i >= 0; i--) {            
            ReadValuesShapes(agent, i);
        }
#elif WEBMODE
        for(int i = 5; i >= 0; i--)                
            this.StartCoroutine(GetValuesShapes(agent, i));
#endif
        
        agent.animation.enabled = true;                        
        agent.animation.Play(animName);

    }

    void PlayAnim(GameObject agent, int ind) {

        if (agent.GetComponent<ArmAnimator>().ArmC == null || agent.GetComponent<TorsoAnimator>().TorsoC == null) {
			Debug.Log ("Controller not assigned");
			return;
		}

 

        AnimationInfo animInfo = agent.GetComponent<AnimationInfo>();
        animInfo.IsContinuous = _toggleContinuous;
        agent.animation.Stop(); //in order to restart animation
        
        switch(ind) {
            case 0:
                InitAgent(agent, "Pointing_to_Spot_Netural_02_Updated");                                                
                break;
            case 1:
                InitAgent(agent, "Picking_Up_Pillow_Netural_01");
                      
                break;
            case 2:
                InitAgent(agent, "Lifting_Netural_01");                
                break;
            case 3:
                InitAgent(agent, "Knocking_Neutral_1");          
                break;
            case 4:
                 InitAgent(agent, "Punching_Netural_02");               
                break;
            case 5:
                InitAgent(agent, "Pushing_Netural_02");                
                break;
            case 6:
                 InitAgent(agent, "Throwing_Netural_02");               
                break;
            case 7:
                 InitAgent(agent, "Walking_Netural_02");               
                break;
            case 8:
                 InitAgent(agent, "Waving_Netural_02");                 
                break;                    
        }

        if(animInfo.IsContinuous)
            agent.animation[animInfo.AnimName].wrapMode = WrapMode.Loop;            
        
        else
            agent.animation[animInfo.AnimName].wrapMode = WrapMode.ClampForever;                            

    }
    string GetAnimName(int ind) {
        
        return _animNameStr[ind];        
    }


    
	void UpdateEmoteParams(GameObject agent, int driveInd) {
		if(agent == null){		
			Debug.Log("AgentPrefab not found");
			return;
		}
        agent.GetComponent<ArmAnimator>().SetSpeed(_speed[driveInd]);
            
        agent.GetComponent<ArmAnimator>().V0 = _v0[driveInd];
        agent.GetComponent<ArmAnimator>().V1 = _v1[driveInd];
        agent.GetComponent<ArmAnimator>().Ti = _ti[driveInd];
        agent.GetComponent<ArmAnimator>().Texp = _texp[driveInd];
        agent.GetComponent<ArmAnimator>().Tval = _tval[driveInd];
        agent.GetComponent<ArmAnimator>().Continuity = _continuity[driveInd];
        agent.GetComponent<ArmAnimator>().Bias = _bias[driveInd];
        agent.GetComponent<AnimationInfo>().InitInterpolators(_tval[driveInd], _continuity[driveInd], _bias[driveInd]);

        agent.GetComponent<ArmAnimator>().T0 = _t0[driveInd];
        agent.GetComponent<ArmAnimator>().T1 = _t1[driveInd];
        agent.GetComponent<ArmAnimator>().TrMag = _trMag[driveInd];
        agent.GetComponent<ArmAnimator>().TfMag = _tfMag[driveInd];
        agent.GetComponent<ArmAnimator>().HrMag = _hrMag[driveInd];
        agent.GetComponent<ArmAnimator>().HfMag = _hfMag[driveInd];
        agent.GetComponent<ArmAnimator>().SquashMag = _squashMag[driveInd];
        agent.GetComponent<ArmAnimator>().WbMag = _wbMag[driveInd];
        agent.GetComponent<ArmAnimator>().WxMag = _wxMag[driveInd];
        agent.GetComponent<ArmAnimator>().WtMag = _wtMag[driveInd];
        agent.GetComponent<ArmAnimator>().WfMag = _wfMag[driveInd];
        agent.GetComponent<ArmAnimator>().EtMag = _etMag[driveInd];
        agent.GetComponent<ArmAnimator>().DMag = _dMag[driveInd];
        agent.GetComponent<ArmAnimator>().EfMag = _efMag[driveInd];

        agent.GetComponent<TorsoAnimator>().EncSpr[0] = _encSpr0[driveInd];
        agent.GetComponent<TorsoAnimator>().SinRis[0] = _sinRis0[driveInd];
        agent.GetComponent<TorsoAnimator>().RetAdv[0] = _retAdv0[driveInd];

        agent.GetComponent<TorsoAnimator>().EncSpr[1] = _encSpr1[driveInd];
        agent.GetComponent<TorsoAnimator>().SinRis[1] = _sinRis1[driveInd];
        agent.GetComponent<TorsoAnimator>().RetAdv[1] = _retAdv1[driveInd];

        agent.GetComponent<ArmAnimator>().Hor = _arm[driveInd][0].x;
        agent.GetComponent<ArmAnimator>().Ver = _arm[driveInd][0].y;
        agent.GetComponent<ArmAnimator>().Sag = _arm[driveInd][0].z;
        agent.GetComponent<ArmAnimator>().UpdateKeypointsByShape(0); //Update keypoints
      
   
        //RightArm 
        //Only horizontal motion is the opposite for each arm
        agent.GetComponent<ArmAnimator>().Hor = -_arm[driveInd][1].x;
        agent.GetComponent<ArmAnimator>().Ver = _arm[driveInd][1].y;
        agent.GetComponent<ArmAnimator>().Sag = _arm[driveInd][1].z;
        agent.GetComponent<ArmAnimator>().UpdateKeypointsByShape(1); //Update keypoints

            
        agent.GetComponent<TorsoAnimator>().UpdateAnglesLinearComb();
        

        
    }

   
        // remember to use StartCoroutine when calling this function!
    IEnumerator PostValues(int effortInd, int answerOE, int answerNCA) {
        string resultURL = "https://fling.seas.upenn.edu/~fundad/cgi-bin/RCTAMAN/putScalingData.php";
               
     // Create a form object for sending high score data to the server
        var form = new WWWForm();        
        form.AddField( "userId", UserInfo.userId);        
        form.AddField( "animName", GetAnimName(_animInd));        
        form.AddField( "effortInd", effortInd );
        form.AddField( "answerOE", answerOE.ToString());
        form.AddField( "answerNCA", answerNCA.ToString());
        
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

    
    //Read everything into the memory
    // remember to use StartCoroutine when calling this function!
    IEnumerator GetValuesDrives(int driveInd) {
        string resultURL = "https://fling.seas.upenn.edu/~fundad/cgi-bin/RCTAMAN/getDriveData.php";

        // Create a form object for sending high score data to the server
        var form = new WWWForm();
        form.AddField("userId", "susand948");
        form.AddField("driveInd", driveInd.ToString());

        // Create a download object
        var download = new WWW(resultURL, form);

        // Wait until the download is done
        yield return download;

        if (download.error != null) {
            Info = download.error;
            print("Error: " + download.error);
        }
        else {
            Info = download.text;
            String[] vals = Info.Split('\t');
            //Assign drive values 
            //should be exactly in this order
            int i = 0;
            _speed[driveInd] = float.Parse(vals[i++]);
            _v0[driveInd] = float.Parse(vals[i++]);
            _v1[driveInd] = float.Parse(vals[i++]);
            _ti[driveInd] = float.Parse(vals[i++]);
            _texp[driveInd] = float.Parse(vals[i++]);
            _tval[driveInd] = float.Parse(vals[i++]);
            _continuity[driveInd] = float.Parse(vals[i++]);
            _bias[driveInd] = float.Parse(vals[i++]);
            _t0[driveInd] = float.Parse(vals[i++]);
            _t1[driveInd] = float.Parse(vals[i++]);
            _trMag[driveInd] = float.Parse(vals[i++]);
            _tfMag[driveInd] = float.Parse(vals[i++]);
            _hrMag[driveInd] = float.Parse(vals[i++]);
            _hfMag[driveInd] = float.Parse(vals[i++]);
            _squashMag[driveInd] = float.Parse(vals[i++]);
            _wbMag[driveInd] = float.Parse(vals[i++]);
            _wxMag[driveInd] = float.Parse(vals[i++]);
            _wtMag[driveInd] = float.Parse(vals[i++]);
            _wfMag[driveInd] = float.Parse(vals[i++]);
            _etMag[driveInd] = float.Parse(vals[i++]);
            _efMag[driveInd] = float.Parse(vals[i++]);
            _dMag[driveInd] = float.Parse(vals[i++]);
            _encSpr0[driveInd] = float.Parse(vals[i++]);
            _sinRis0[driveInd] = float.Parse(vals[i++]);
            _retAdv0[driveInd] = float.Parse(vals[i++]);
            _encSpr1[driveInd] = float.Parse(vals[i++]);
            _sinRis1[driveInd] = float.Parse(vals[i++]);
            _retAdv1[driveInd] = float.Parse(vals[i++]);
            _arm[driveInd][0].x = float.Parse(vals[i++]);
            _arm[driveInd][0].y = float.Parse(vals[i++]);
            _arm[driveInd][0].z = float.Parse(vals[i++]);
            _arm[driveInd][1].x = float.Parse(vals[i++]);
            _arm[driveInd][1].y = float.Parse(vals[i++]);
            _arm[driveInd][1].z = float.Parse(vals[i++]);

        }
    }

    // remember to use StartCoroutine when calling this function!
    IEnumerator GetValuesShapes(GameObject agent, int shapeInd) {
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
        TorsoController torso = agent.GetComponent<TorsoController>();
        torso.Reset();


        if (download.error != null) {
            ShapeInfo = download.error;
            print("Error: " + download.error);
        }
        else {
            ShapeInfo = download.text;
            String[] vals = ShapeInfo.Split('\t');
          
            //Assign shape values 
            TorsoAnimator torsoAnim = agent.GetComponent<TorsoAnimator>();

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

    // remember to use StartCoroutine when calling this function!
    IEnumerator GetValuesAnswers(int driveInd) {
        string resultURL = "https://fling.seas.upenn.edu/~fundad/cgi-bin/RCTAMAN/getScalingData.php";

        // Create a form object for sending high score data to the server
        var form = new WWWForm();
        form.AddField("userId", UserInfo.userId);
        form.AddField("animName", GetAnimName(_animInd));
        form.AddField("driveInd", driveInd.ToString());
        

        // Create a download object
        var download = new WWW(resultURL, form);

        // Wait until the download is done
        yield return download;

        if (download.error != null) {
            Info = download.error;
            print("Error: " + download.error);
        }
        else {
            Info = download.text;
            String[] vals = Info.Split('\t');

            //Assign answer values 
            //should be exactly in this order
            int i = 0;
            _answerOe = int.Parse(vals[i++]);
            _answerNca = int.Parse(vals[i++]);

        }
    }
    

    void ReadValuesShapes(GameObject agent, int shapeInd) {
        string fileName = "shapesSusan.txt";
        StreamReader sr = new StreamReader(fileName);

        TorsoAnimator torsoAnim = agent.GetComponent<TorsoAnimator>();
        string[] content = File.ReadAllLines(fileName);

        String[] tokens = content[shapeInd + 1].Split('\t');
        

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

    void ReadValuesDrives(int driveInd) {
        string fileName = "drivesSusan.txt";
        StreamReader sr = new StreamReader(fileName);

        
        string[] content = File.ReadAllLines(fileName);

        String[] tokens = content[driveInd + 1].Split('\t');

        int i = 2;
        _speed[driveInd] = float.Parse(tokens[i++]);
        _v0[driveInd] = float.Parse(tokens[i++]);
        _v1[driveInd] = float.Parse(tokens[i++]);
        _ti[driveInd] = float.Parse(tokens[i++]);
        _texp[driveInd] = float.Parse(tokens[i++]);
        _tval[driveInd] = float.Parse(tokens[i++]);
        _t0[driveInd] = float.Parse(tokens[i++]);
        _t1[driveInd] = float.Parse(tokens[i++]);
        _hrMag[driveInd] = float.Parse(tokens[i++]);
        _hfMag[driveInd] = float.Parse(tokens[i++]);
        _squashMag[driveInd] = float.Parse(tokens[i++]);
        _wbMag[driveInd] = float.Parse(tokens[i++]);
        _wxMag[driveInd] = float.Parse(tokens[i++]);
        _wtMag[driveInd] = float.Parse(tokens[i++]);
        _wfMag[driveInd] = float.Parse(tokens[i++]);
        _etMag[driveInd] = float.Parse(tokens[i++]);
        _efMag[driveInd] = float.Parse(tokens[i++]);
        _dMag[driveInd] = float.Parse(tokens[i++]);
        _trMag[driveInd] = float.Parse(tokens[i++]);
        _tfMag[driveInd] = float.Parse(tokens[i++]);
        _encSpr0[driveInd] = float.Parse(tokens[i++]);
        _sinRis0[driveInd] = float.Parse(tokens[i++]);
        _retAdv0[driveInd] = float.Parse(tokens[i++]);
        _encSpr1[driveInd] = float.Parse(tokens[i++]);
        _sinRis1[driveInd] = float.Parse(tokens[i++]);
        _retAdv1[driveInd] = float.Parse(tokens[i++]);
        _continuity[driveInd] = float.Parse(tokens[i++]);
        _bias[driveInd] = float.Parse(tokens[i++]);
        _arm[driveInd][0].x = float.Parse(tokens[i++]);
        _arm[driveInd][0].y = float.Parse(tokens[i++]);
        _arm[driveInd][0].z = float.Parse(tokens[i++]);
        _arm[driveInd][1].x = float.Parse(tokens[i++]);
        _arm[driveInd][1].y = float.Parse(tokens[i++]);
        _arm[driveInd][1].z = float.Parse(tokens[i++]);
       
        sr.Close();
        
    }
    //If answerOE = 0, agent is OE+ else OE-
    //qInd = question index
    void RecordValues(int effortInd, int answerOE, int answerNCA) {

        string fileName = "resultsPersonality.txt";

        if (!File.Exists(fileName)) {
            StreamWriter sw = new StreamWriter(fileName);
            //Blank lines
            for (int i = 0; i < _animNameStr.Length; i++) {
                sw.WriteLine(_animNameStr[i]);
                sw.WriteLine("QInd\tOE\tNCA");
                for (int j = 0; j < 32; j++) // 32 combinations of effort elements
                    sw.WriteLine(j + "\t0\t0");
            }
            sw.Close();
        }

        string[] content = File.ReadAllLines(fileName);


        //Update the content 
        content[(32 + 2) * _animInd + effortInd + 2] = string.Format("{0:0}\t{1:0}\t{2:0}",
                                   effortInd, answerOE, answerNCA);


        using (StreamWriter sw = new StreamWriter(fileName)) {
            for (int i = 0; i < content.Length; i++)
                sw.WriteLine(content[i]);
        }
    }

   
    }		

