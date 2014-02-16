#define WEBMODE


using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;

public class LMAComparisonGUI : MonoBehaviour {

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
    public Texture TexBorder = new Texture();
    public GUISkin ButtonSkin;

    
   // private string[] _animNameStr = { "Knocking", "Pointing", "Lifting", "Picking up pillow", "Punching", "Pushing", "Throwing", "Walking", "Waving" };
    private string[] _animNameStr = { "Pointing", "Picking up a pillow"};
    private static int _animInd = 0;

    int _driveIndLeft = 0, _driveIndRight = 0;
    static int _qInd = 0; //question index
    private int[,] _effortList =  {{-1, -1, -1, 0}, {-1, -1, 1, 0}, {-1, 1, -1, 0}, {-1, 1, 1, 0}, {1, -1, -1, 0}, {1, -1, 1, 0}, {1, 1, -1, 0}, {1, 1, 1, 0}, 
                                       {-1, -1, 0, -1}, {-1, -1, 0, 1},{-1, 1,0,  -1}, {-1, 1, 0, 1}, {1, -1, 0, -1},{1, -1, 0,  1}, {1, 1, 0, -1}, {1, 1, 0, 1},
                                        {-1,  0, -1, -1}, {-1, 0, -1,  1}, {-1, 0, 1,  -1}, {-1, 0, 1, 1}, {1, 0, -1, -1}, {1, 0, -1,  1}, {1, 0, 1, -1}, {1,  0, 1, 1},
                                       { 0, -1, -1, -1}, {0, -1, -1,  1}, {0, -1, 1,  -1}, {0, -1, 1, 1}, {0, 1, -1, -1}, {0, 1, -1,  1}, {0, 1, 1, -1}, {0, 1, 1, 1}};


    Dictionary<int, int> _effortCombination = new Dictionary<int , int>();

    private string[] _effortNames = { "Space", "Weight", "Time", "Flow" };

    static int _selectOe = -1, _selectNca = -1; //to ensure that none is selected by default
    static int _answerOe = -1, _answerNca = -1; //to ensure that none is selected by default
    bool [] _arePositionsSwapped = new bool[48 * 2];
    //Record the positions of the left and right agents
    Vector3 _leftPos, _rightPos;
    private float[] _oe = new float[48 * 2]; //effort combination cnt * animCnt    
    private float[] _nca = new float[48 * 2];

    public string ShapeInfo = "";
    public string Info = "waiting...";
    private static Vector2 _scrollPosition;

    private static string _questionNo = "";
    
    private static string[] _isSubmittedStr = new string[48 * 2];
    private static bool[] _isSubmitted= new bool[48 * 2];
    private int _submittedCnt = 0;

    private float _scrollWidth = 0;

    //read shape values once in the beginning. no need to reread everytime
    float[][] _shapeParams = new float[18][];

    GameObject _agentLeft, _agentRight;
    void Start(){

        for (int i = 0; i < 32; i++) {
            _arm[i] = new Vector3[2];
        }
            
        _agentLeft = GameObject.Find("AgentPrefabLeft");        
        _agentRight = GameObject.Find("AgentPrefabRight");
        
        
        if (!_agentLeft) {
            
            Debug.Log("AgentLeft prefab not found");
            return;
        }
        if (!_agentRight) {
            Debug.Log("AgentRight prefab not found");
            return;
        }

       
        
        _qInd = 0;
        //compute effortCombination hashes
        for (int i = 0; i < 32; i++) {
            int val = _effortList[i, 3] + _effortList[i, 2] * 3 + _effortList[i, 1] * 9 + _effortList[i, 0] * 27;
            _effortCombination.Add(val, i);
        }

  
      
 
        for (int i = 0; i < _isSubmittedStr.Length; i++)
            _isSubmittedStr[i] = "Answer NOT submitted";

        UpdateCameraBoundaries();

        for (int i = 0; i < 18; i++)
            _shapeParams[i] = new float[6];

        //Read all drive and shape parameters

#if !WEBMODE
        for (int i = 0; i < 32; i++)
            ReadValuesDrives(i);

        for (int i = 5; i >= 0; i--) {            
            ReadValuesShapes( i);
        }

#elif WEBMODE

        for (int i = 0; i < 32; i++)
            this.StartCoroutine(GetValuesDrives(i));

        for (int i = 5; i >= 0; i--)
            this.StartCoroutine(GetValuesShapes(i));

        for(int i = 0; i < 2; i++)
            for(int j = 0; j < 48; j++)
                this.StartCoroutine(GetValuesSubmitted(j, i));

#endif
        //Read all shape parameters


        //Select if positions are swapped in the beginning
        for (int i = 0; i < _arePositionsSwapped.Length; i++) {
            if (MathDefs.GetRandomNumber(2) == 1) //50% chance
                _arePositionsSwapped[i] = true;
            else
                _arePositionsSwapped[i] = false;
        }


        Reset();

        _leftPos = _agentLeft.transform.position;
        _rightPos = _agentRight.transform.position;

    }


    public void Reset() {
        for (int i = 0; i < _oe.Length; i++) {
            _oe[i] = _nca[i] = 0;
        }

        InitAgent(_agentRight, "Pointing_to_Spot_Netural_02_Updated");
        InitAgent(_agentLeft, "Pointing_to_Spot_Netural_02_Updated");


        
        UpdateParameters();

        
        

        PlayAnim(_agentRight, _animInd);
        StopAnim(_agentRight);

        PlayAnim(_agentLeft, _animInd);
        StopAnim(_agentLeft);
       
        
    }

    void UpdateCameraBoundaries() {
        GameObject cam1 = GameObject.Find("Camera1");
        GameObject cam2 = GameObject.Find("Camera2");
        GameObject cam3 = GameObject.Find("Camera3");

        cam1.camera.rect = new Rect(0, 0, _scrollWidth / Screen.width, 1); //320 is the width of the parameters

        if (_arePositionsSwapped[_animInd * 48 + _qInd]) {
            cam3.camera.rect = new Rect(_scrollWidth / Screen.width, 0, ((Screen.width - _scrollWidth) / 2f) / Screen.width, 1);
            cam2.camera.rect = new Rect((Screen.width - (Screen.width - _scrollWidth) / 2f) / Screen.width, 0, ((Screen.width - _scrollWidth) / 2f) / Screen.width, 1);
        }
        else {
            cam2.camera.rect = new Rect(_scrollWidth / Screen.width, 0, ((Screen.width - _scrollWidth) / 2f) / Screen.width, 1);
            cam3.camera.rect = new Rect((Screen.width - (Screen.width - _scrollWidth) / 2f) / Screen.width, 0, ((Screen.width - _scrollWidth) / 2f) / Screen.width, 1);
        }


       
    }

   

    void Update(){
        UpdateCameraBoundaries();

        if (Input.GetKeyDown("left")) {
            GetPrevQuestion();
            StopAnimations();
        }
        else if (Input.GetKeyDown("right")) {

            GetNextQuestion();
            StopAnimations();
        }
        else if (Input.GetKeyDown("up"))  {
            _animInd++;
            if (_animInd >= _animNameStr.Length)
                    _animInd = _animNameStr.Length - 1;
            StopAnimations();

            //Animation changed -- reassign foot and root positions
            _agentRight.GetComponent<TorsoController>().AssignInitRootandFootPos();
            _agentLeft.GetComponent<TorsoController>().AssignInitRootandFootPos();


        }
        else if(Input.GetKeyDown("down")) {
            _animInd--;            
            if (_animInd < 0)
                _animInd = 0;
            StopAnimations();

            //Animation changed -- reassign foot and root positions
            _agentRight.GetComponent<TorsoController>().AssignInitRootandFootPos();
            _agentLeft.GetComponent<TorsoController>().AssignInitRootandFootPos();

        }

    }
    
    void GetNextQuestion() {
        _qInd++;
        if (_qInd > 47 && _animInd < _animNameStr.Length) {
            _qInd = 0;
            _animInd++;
            if (_animInd >= _animNameStr.Length) {
                _animInd = _animNameStr.Length - 1;
                _qInd = 47;

            }

        }
#if WEBMODE
        this.StartCoroutine(GetValuesSubmitted(_qInd, _animInd));
#endif
        
    }
    void GetPrevQuestion() {
        _qInd--;
        if (_qInd < 0) {
            if (_animInd > 0) {
                _qInd = 47;
                _animInd--;
            }
            else
                _qInd = 0;
        }
      
#if WEBMODE
        this.StartCoroutine(GetValuesSubmitted(_qInd, _animInd));
#endif
    }

    void StopAnimations() {

       // _agentLeft.transform.position = _leftPos;
      //  _agentRight.transform.position = _rightPos;

         _agentLeft.GetComponent<TorsoController>().Reset();
         _agentRight.GetComponent<TorsoController>().Reset();
    
        StopAnim(_agentLeft);
        PlayAnim(_agentLeft, _animInd); //start the next animation
        StopAnim(_agentLeft);

        StopAnim(_agentRight);
        PlayAnim(_agentRight, _animInd); //start the next animation
        StopAnim(_agentRight);


        //changes the names of the drives
       UpdateParameters();

    }

    
   
    
    //Left and right drive indices
    //map ind of 48 to left and right drive indices
    void ComputeBothDriveInds(int qInd) {
        int space, weight, time, flow;
        int key = -1; //key for effort combination dictionary
        //Find which effort is compared
        int cVal = qInd % 4;//qInd / 12;
        int qVal = qInd / 4; //qInd % 12; // between 0 and 11 
        int[,] othersList = {{-1, -1, 0},  {-1, 1, 0},  {1, -1, 0},  {1, 1, 0},  {-1, 0, -1}, {-1, 0, 1}, {1, 0, -1}, {1, 0, 1}, {0, -1, -1}, {0, -1, 1}, {0, 1, -1}, {0, 1, 1}};


        if (cVal == 0) { //space

            weight = othersList[qVal,0];
            time = othersList[qVal,1];
            flow = othersList[qVal,2];

            //Left
            space = -1;
            key = space * 27 + weight * 9 + time * 3 + flow;            
            _driveIndLeft = _effortCombination[key];
            //right
            space = 1;
            key = space * 27 + weight * 9 + time * 3 + flow;
            _driveIndRight = _effortCombination[key];
        }

        else if (cVal == 1) { //weight

            space = othersList[qVal,0];
            time = othersList[qVal,1];
            flow = othersList[qVal,2];

            //Left
            weight = -1;
            key = space * 27 + weight * 9 + time * 3 + flow;
            _driveIndLeft = _effortCombination[key];


            //right
            weight = 1;
            key = space * 27 + weight * 9 + time * 3 + flow;
            _driveIndRight = _effortCombination[key];
        }

        else if (cVal == 2) { //time

            space = othersList[qVal,0];
            weight = othersList[qVal,1];
            flow = othersList[qVal,2];

            //Left
            time = -1;
            key = space * 27 + weight * 9 + time * 3 + flow;
            _driveIndLeft = _effortCombination[key];
     
            //right
            time = 1;
            key = space * 27 + weight * 9 + time * 3 + flow;
            _driveIndRight = _effortCombination[key];
        }

        else if (cVal == 3) { //flow

            space = othersList[qVal,0];
            weight = othersList[qVal,1];
            time = othersList[qVal,2];

            //Left
            flow = -1;
            key = space * 27 + weight * 9 + time * 3 + flow;
            _driveIndLeft = _effortCombination[key];


            //right
            flow = 1;
            key = space * 27 + weight * 9 + time * 3 + flow;
            _driveIndRight = _effortCombination[key];
        }

        
    }
    

    void UpdateParameters() {

        
        //Find driveIndLeft and driveIndRight
        ComputeBothDriveInds(_qInd);
        //Update right
        UpdateEmoteParams(_agentRight, _driveIndRight);

        //Update left
        UpdateEmoteParams(_agentLeft, _driveIndLeft);

    }
	

	void OnGUI () {
        
        GUIStyle style = new GUIStyle();

        GUI.DrawTexture(new Rect(Screen.width / 2f - 1f, 0, 2, Screen.height), TexBorder, ScaleMode.ScaleToFit, true, 2f/Screen.height);
        GUI.skin = ButtonSkin;
        GUILayout.BeginArea (new Rect (220,10,300,250));
        GUILayout.BeginHorizontal ();	
        //GUI.color = Color.black;
        style.fontSize = 22;
      
        GUILayout.EndHorizontal ();	
        
        GUILayout.EndArea();

        string[] selectStr = { "Left", "Equal", "Right"};
        string[] scaleStr = { "Slightly more", "Moderately more", "Much more" };


        
           
        GUILayout.BeginArea (new Rect (20,10,250,250));
        style.fontSize = 18;
        style.normal.textColor = new Color(0.2f, 0.2f, 0.2f);

        GUILayout.Label ("Animation: " + GetAnimName(_animInd), style);
        style.fontSize = 14;
     /*   if (_arePositionsSwapped[_animInd * 48 + _qInd]) {
            GUILayout.Label("Right: " + ComputeEffortCombinationStr(_driveIndLeft), style);
            GUILayout.Label("Left: " + ComputeEffortCombinationStr(_driveIndRight), style);
        }
        else {
            GUILayout.Label("Left: " + ComputeEffortCombinationStr(_driveIndLeft), style);
            GUILayout.Label("Right: " + ComputeEffortCombinationStr(_driveIndRight), style);
        }
       */ 
        GUILayout.EndArea();

        //GUI.color = Color.white;


        _scrollPosition = GUI.BeginScrollView(new Rect(Screen.width / 2f - 480, Screen.height - 270f, 1000, 290), _scrollPosition, new Rect(0, 0, 220, 230));

        


        style.fontSize = 20;
        
        

        GUILayout.BeginHorizontal();
        GUILayout.Label(_questionNo, style);
        GUILayout.Label(_isSubmittedStr[_animInd * 48 + _qInd], style);
        GUILayout.EndHorizontal();

        //GUI.contentColor = new Color(0, 0, 0, 1);
        //GUI.color = Color.white;

      
        GUI.color = Color.white;
        if(GUILayout.Button ( "Play")) {
            _agentLeft.GetComponent<TorsoController>().Reset();
            _agentRight.GetComponent<TorsoController>().Reset();

            PlayAnim(_agentRight, _animInd);
            PlayAnim(_agentLeft, _animInd);
            UpdateParameters(); //we need to update after play because playanim resets torso parameters for speed etc. when animinfo is reset
        }


        

        style.fontSize = 18;
        GUILayout.Label("");
        GUILayout.BeginHorizontal();
        GUILayout.Label("a. Which one looks more \"extraverted & enthusiastic \", and less \"reserved & quiet\"?", style);
        GUI.color = Color.white;    
        _selectOe = GUILayout.SelectionGrid(_selectOe, selectStr, 3);
        GUILayout.EndHorizontal();

        //GUILayout.Label("");
        //GUILayout.BeginHorizontal();
        //GUILayout.Label("b. Which one looks more reserved and quiet?", style);
        //GUI.color = Color.white;
        //_selectOe = GUILayout.SelectionGrid(_selectOe, selectStr, 3);
        //GUILayout.EndHorizontal();

        
     /*   GUILayout.Label("", style);
        GUILayout.BeginHorizontal();
        GUILayout.Label("1.b. How much?", style);
        _answerOe = GUILayout.SelectionGrid(_answerOe, scaleStr, 3);
        GUILayout.EndHorizontal();
        */

        GUILayout.Label("");
        GUILayout.BeginHorizontal();
        GUILayout.Label("b. Which one looks more \"calm & emotionally stable\", and less \"anxious & easily upset\"?", style);
        GUI.color = Color.white;
        _selectNca = GUILayout.SelectionGrid(_selectNca, selectStr, 3);
        GUILayout.EndHorizontal();


        //GUILayout.Label("");
        //GUILayout.BeginHorizontal();
        //GUILayout.Label("d. Which one looks more calm and emotionally stable?", style);
        //GUI.color = Color.white;
        //_selectNca = GUILayout.SelectionGrid(_selectNca, selectStr, 3);
        //GUILayout.EndHorizontal();


      /*      GUILayout.Label("", style);
          GUILayout.BeginHorizontal();
          GUILayout.Label("2.b. How much?", style);
          _answerNca = GUILayout.SelectionGrid(_answerNca, scaleStr, 3);
          GUILayout.EndHorizontal();

          */
        
        if (_submittedCnt >= 96) {
            style.normal.textColor = Color.red;
            _isSubmittedStr[_animInd * 48 + _qInd] = "Study is complete. Thank you!";
            GUILayout.Label("");
        }
        else
            GUILayout.Label("");

       
       

        GUI.color = Color.white;
        
        if (GUILayout.Button("Submit")) {
            //   if (_answerOe == -1 || _answerNca == -1 || _selectOe == -1 || _selectNca == -1) {
            if (_selectOe == -1 || _selectNca == -1) {
                _isSubmittedStr[_animInd * 48 + _qInd] = "Please answer all questions";
            }

            else {
                if (_isSubmitted[_animInd * 48 + _qInd] == false) {
                    _isSubmitted[_animInd * 48 + _qInd] = true;
                    _submittedCnt++;
                }


                //        _isSubmittedStr[_animInd * 48 + _qInd] = "Answer submitted";


                //_answerOe++; //map to [1 2 3]
                //_answerNca++; //map to [1 2 3]

                //change signs
                if (_arePositionsSwapped[_animInd * 48 + _qInd]) { //Agent positions are swapped, left is on the right, and right is on the left
                    if (_selectOe == 0) //left 
                        _answerOe = 1; //right
                    else if (_selectOe == 2) //right
                        _answerOe = -1;
                    else
                        _answerOe = 0; //equal

                    if (_selectNca == 0) //left 
                        _answerNca = 1; //right
                    else if (_selectNca == 2) //right
                        _answerNca = -1;
                    else
                        _answerNca = 0; //equal

                }
                else {//positions not swapped
                    if (_selectOe == 0) //left 
                        _answerOe = -1; //left
                    else if (_selectOe == 2) //right
                        _answerOe = 1; //right
                    else
                        _answerOe = 0; //equal

                    if (_selectNca == 0) //left 
                        _answerNca = -1; //left
                    else if (_selectNca == 2) //right
                        _answerNca = 1; //right
                    else
                        _answerNca = 0; //equal
                }

#if !WEBMODE
                RecordValues(_qInd, _answerOe, _answerNca);
#elif WEBMODE
                this.StartCoroutine(PostValues(_qInd, _answerOe, _answerNca, _arePositionsSwapped[_animInd * 48 + _qInd]));
#endif

                //Move on to the next question
                GetNextQuestion();
                StopAnimations();

                _answerOe = _answerNca = -1;
                _selectOe = _selectNca = -1;
            }

        }
         
        GUI.EndScrollView();

        _questionNo = "Question: " + (_animInd * 48 + _qInd + 1) + " of 96";

        
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
        //agent.GetComponent<TorsoAnimator>().Reset();
        //Read values for the shape parameters 
        //Need to call this only once

//#if !WEBMODE
//        for (int i = 5; i >= 0; i--) {            
//            ReadValuesShapes(agent, i);
//        }
//#elif WEBMODE
//        for(int i = 5; i >= 0; i--)                
//            this.StartCoroutine(GetValuesShapes(agent, i));
//#endif
        
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
       // agent.animation.Stop(); //in order to restart animation
        StopAnim(agent); //stop first
        
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


        //Update shape parameters
        for (int j = 0; j < _shapeParams.Length; j++)
            for (int i = 0; i < _shapeParams[j].Length; i++)
                agent.GetComponent<TorsoAnimator>().ShapeParams[j][i] = _shapeParams[j][i];
            
        agent.GetComponent<TorsoAnimator>().UpdateAnglesLinearComb();
        

        
    }

   
        // remember to use StartCoroutine when calling this function!
    //AnswerOE and AnswerNCA are always 0 for the negative values of Effort and always 1 for positive values.
    //I.e. ind, lgt, sus, fre
    IEnumerator PostValues(int effortInd, int answerOE, int answerNCA, bool arePositionsSwapped) {
        string resultURL = "https://fling.seas.upenn.edu/~fundad/cgi-bin/RCTAMAN/putComparisonData.php";
               
     // Create a form object for sending high score data to the server
        var form = new WWWForm();        
        form.AddField( "userId", UserInfo.userId);        
        form.AddField( "animName", GetAnimName(_animInd));        
        form.AddField( "effortInd", effortInd );
        form.AddField( "answerOE", answerOE.ToString());
        form.AddField( "answerNCA", answerNCA.ToString());
        form.AddField("areSwapped", arePositionsSwapped.ToString());
        
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
    IEnumerator GetValuesShapes( int shapeInd) {
        string resultURL = "https://fling.seas.upenn.edu/~fundad/cgi-bin/RCTAMAN/getShapeData.php";

        // Create a form object for sending high score data to the server
        var form = new WWWForm();
        form.AddField("userId", "susand948");
        form.AddField("shapeInd", shapeInd.ToString());
        // Create a download object


        var download = new WWW(resultURL, form);

        // Wait until the download is done
        yield return download;

       
        if (download.error != null) {
            ShapeInfo = download.error;
            print("Error: " + download.error);
        }
        else {
            ShapeInfo = download.text;
            String[] vals = ShapeInfo.Split('\t');

            int i = 0;
            _shapeParams[(int)BPart.HeadX][shapeInd] = float.Parse(vals[i++]);
            _shapeParams[(int)BPart.NeckX][shapeInd] = float.Parse(vals[i++]);
            _shapeParams[(int)BPart.SpineY][shapeInd] = float.Parse(vals[i++]);
            _shapeParams[(int)BPart.Spine1X][shapeInd] = float.Parse(vals[i++]);
            _shapeParams[(int)BPart.ShouldersX][shapeInd] = float.Parse(vals[i++]);
            _shapeParams[(int)BPart.ShouldersY][shapeInd] = float.Parse(vals[i++]);
            _shapeParams[(int)BPart.ShouldersZ][shapeInd] = float.Parse(vals[i++]);
            _shapeParams[(int)BPart.ClaviclesX][shapeInd] = float.Parse(vals[i++]);
            _shapeParams[(int)BPart.ClaviclesY][shapeInd] = float.Parse(vals[i++]);
            _shapeParams[(int)BPart.ClaviclesZ][shapeInd] = float.Parse(vals[i++]);
            _shapeParams[(int)BPart.PelvisLX][shapeInd] = float.Parse(vals[i++]);
            _shapeParams[(int)BPart.PelvisRX][shapeInd] = float.Parse(vals[i++]);
            _shapeParams[(int)BPart.PelvisY][shapeInd] = float.Parse(vals[i++]);
            _shapeParams[(int)BPart.PelvisZ][shapeInd] = float.Parse(vals[i++]);
            _shapeParams[(int)BPart.KneesX][shapeInd] = float.Parse(vals[i++]);
            _shapeParams[(int)BPart.HipsX][shapeInd] = float.Parse(vals[i++]);
            _shapeParams[(int)BPart.ToesX][shapeInd] = float.Parse(vals[i++]);
            _shapeParams[(int)BPart.SpineLength][shapeInd] = float.Parse(vals[i++]);


        }
    }

    // remember to use StartCoroutine when calling this function!
    IEnumerator GetValuesSubmitted(int qInd, int animInd) {
        string resultURL = "https://fling.seas.upenn.edu/~fundad/cgi-bin/RCTAMAN/checkSubmitted.php";

        // Create a form object for sending high score data to the server
        var form = new WWWForm();
        form.AddField("userId", UserInfo.userId);
        form.AddField("qInd", qInd.ToString());
        form.AddField("animName", GetAnimName(_animInd));
        // Create a download object


        var download = new WWW(resultURL, form);

        // Wait until the download is done
        yield return download;

        if (download.error != null) {            
            print("Error: " + download.error);
        }
        else {
            _isSubmittedStr[_animInd * 48 + _qInd] = download.text;
            

        }
    }

    void ReadValuesShapes( int shapeInd) {
        string fileName = "shapesSusan.txt";
        StreamReader sr = new StreamReader(fileName);

        string[] content = File.ReadAllLines(fileName);

        String[] tokens = content[shapeInd + 1].Split('\t');
        

        int i = 2;
        _shapeParams[(int)BPart.HeadX][shapeInd] = float.Parse(tokens[i++]);
        _shapeParams[(int)BPart.NeckX][shapeInd] = float.Parse(tokens[i++]);
        _shapeParams[(int)BPart.SpineY][shapeInd] = float.Parse(tokens[i++]);
        _shapeParams[(int)BPart.Spine1X][shapeInd] = float.Parse(tokens[i++]);
        _shapeParams[(int)BPart.ShouldersX][shapeInd] = float.Parse(tokens[i++]);
        _shapeParams[(int)BPart.ShouldersY][shapeInd] = float.Parse(tokens[i++]);
        _shapeParams[(int)BPart.ShouldersZ][shapeInd] = float.Parse(tokens[i++]);
        _shapeParams[(int)BPart.ClaviclesX][shapeInd] = float.Parse(tokens[i++]);
        _shapeParams[(int)BPart.ClaviclesY][shapeInd] = float.Parse(tokens[i++]);
        _shapeParams[(int)BPart.ClaviclesZ][shapeInd] = float.Parse(tokens[i++]);
        _shapeParams[(int)BPart.PelvisLX][shapeInd] = float.Parse(tokens[i++]);
        _shapeParams[(int)BPart.PelvisRX][shapeInd] = float.Parse(tokens[i++]);
        _shapeParams[(int)BPart.PelvisY][shapeInd] = float.Parse(tokens[i++]);
        _shapeParams[(int)BPart.PelvisZ][shapeInd] = float.Parse(tokens[i++]);
        _shapeParams[(int)BPart.KneesX][shapeInd] = float.Parse(tokens[i++]);
        _shapeParams[(int)BPart.HipsX][shapeInd] = float.Parse(tokens[i++]);
        _shapeParams[(int)BPart.ToesX][shapeInd] = float.Parse(tokens[i++]);
        _shapeParams[(int)BPart.SpineLength][shapeInd] = float.Parse(tokens[i++]);


        
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
                for (int j = 0; j < 48; j++) // 32 combinations of effort elements
                    sw.WriteLine(j + "\t0\t0");
            }
            sw.Close();
        }

        string[] content = File.ReadAllLines(fileName);


        //Update the content 
        content[(48 + 2) * _animInd + effortInd + 2] = string.Format("{0:0}\t{1:0}\t{2:0}",
                                   effortInd, answerOE, answerNCA);


        using (StreamWriter sw = new StreamWriter(fileName)) {
            for (int i = 0; i < content.Length; i++)
                sw.WriteLine(content[i]);
        }
    }

   
    }		

