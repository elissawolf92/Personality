#define WEBMODE


using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;

public class LMAPlayerGUI : MonoBehaviour {

	// Animation selector
	GUIContent[] animComboBoxList;
	private ComboBox animComboBoxControl; // = new ComboBox();
	private GUIStyle listStyle = new GUIStyle();

	// Culture selector
	GUIContent[] cultureComboBoxList;
	private ComboBox cultureComboBoxControl;

	// Ocean personality parameters, range from -50 to 50
	// Relative
	private float[] _oceanRel = new float[5] {0,0,0,0,0};
	// Need a temp one to detect changes to the sliders
	private float[] _oceanRelTemp = new float[5] {0,0,0,0,0};
	/*
	private int oRel = 0;
	private int cRel = 0;
	private int eRel = 0;
	private int aRel = 0;
	private int nRel = 0;
	*/

	// Absolute (combines relative with cultural baseline)
	private float[] _oceanAbs = new float[5] {0,0,0,0,0};
	private float _absRange = 50;
	/*
	private int oAbs = 0;
	private int cAbs = 0;
	private int eAbs = 0;
	private int aAbs = 0;
	private int nAbs = 0;
	*/

	// Cultures
	private static string[] _cultureNames = {"American", "Arab"};
	private int _cultureInd = 0;
	// Mean OCEAN values for different cultures          
	// Row is culture, col is personality factor            O    C    E    A   N
	private static int[,] _cultureMeans= new int[2, 5] {  { 30, -10,  10, -20, 0}, 
		          									      {-30,  20, -10,  30, 0}};
	private static int[,] _cultureRanges = new int[2, 5] {{ 40,  50,  50,  20, 40},
														  { 20,  50,  30,  40, 50}};
		
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
    private static string[] _animDisplayNameStr = { "Pointing", "Picking up a pillow"};
	private int _animInd = 0;
	private static string[] _animNames = {"Pointing_to_Spot_Netural_02_Updated", 
		"Picking_Up_Pillow_Netural_01",
		"Lifting_Netural_01",
		"Knocking_Neutral_1",
		"Punching_Netural_02",
		"Pushing_Netural_02",
		"Throwing_Netural_02",
		"Walking_Netural_02", 
		"Waving_Netural_02"};
		
	int _driveInd = 0;
	static int _qInd = 0; //question index
	private int[,] _effortList =  {{-1, -1, -1, 0}, {-1, -1, 1, 0}, {-1, 1, -1, 0}, {-1, 1, 1, 0}, {1, -1, -1, 0}, {1, -1, 1, 0}, {1, 1, -1, 0}, {1, 1, 1, 0}, 
			{-1, -1, 0, -1}, {-1, -1, 0, 1},{-1, 1,0,  -1}, {-1, 1, 0, 1}, {1, -1, 0, -1},{1, -1, 0,  1}, {1, 1, 0, -1}, {1, 1, 0, 1},
            {-1,  0, -1, -1}, {-1, 0, -1,  1}, {-1, 0, 1,  -1}, {-1, 0, 1, 1}, {1, 0, -1, -1}, {1, 0, -1,  1}, {1, 0, 1, -1}, {1,  0, 1, 1},
            { 0, -1, -1, -1}, {0, -1, -1,  1}, {0, -1, 1,  -1}, {0, -1, 1, 1}, {0, 1, -1, -1}, {0, 1, -1,  1}, {0, 1, 1, -1}, {0, 1, 1, 1}};


    Dictionary<int, int> _effortCombination = new Dictionary<int , int>();

    private string[] _effortNames = { "Space", "Weight", "Time", "Flow" };

    //Record the positions of the left and right agents
    //Vector3 _leftPos, _rightPos;
	// NEW: Only one agent
	Vector3 _pos;

    public string ShapeInfo = "";
    public string Info = "waiting...";
    private static Vector2 _scrollPosition;
	
    private float _scrollWidth = 0;

    //read shape values once in the beginning. no need to reread everytime
    float[][] _shapeParams = new float[18][];

	// NEW: Only one agent
    GameObject _agent;



    void Start(){

		// Setting up the combo boxes
		animComboBoxList = new GUIContent[_animDisplayNameStr.Length];
		for (int i= 0; i < _animDisplayNameStr.Length; i++) {
			animComboBoxList[i] = new GUIContent(_animDisplayNameStr[i]);
				}

		listStyle.normal.textColor = Color.white; 
		listStyle.onHover.background =
		listStyle.hover.background = new Texture2D(2, 2);
		listStyle.padding.left =
		listStyle.padding.right =
		listStyle.padding.top =
		listStyle.padding.bottom = 4;

		
		animComboBoxControl = new ComboBox(new Rect(0, 30, 200, 20), animComboBoxList[0], animComboBoxList, "button", "box", listStyle);

		cultureComboBoxList = new GUIContent[_cultureNames.Length];
		for (int i= 0; i < _cultureNames.Length; i++) {
			cultureComboBoxList[i] = new GUIContent(_cultureNames[i]);
		}
		cultureComboBoxControl = new ComboBox(new Rect(0, 30, 200, 20), cultureComboBoxList[0], 
		                                      cultureComboBoxList, "button", "box", listStyle);
		
		for (int i = 0; i < 32; i++) {
            _arm[i] = new Vector3[2];
        }
        
		// Load the agent
		_agent = GameObject.Find ("AgentPrefab");

		if (!_agent) {

			Debug.Log("Agent prefab not found");
			return;
		}
        
        _qInd = 0;
        //compute effortCombination hashes
        for (int i = 0; i < 32; i++) {
            int val = _effortList[i, 3] + _effortList[i, 2] * 3 + _effortList[i, 1] * 9 + _effortList[i, 0] * 27;
            _effortCombination.Add(val, i);
        }


        UpdateCameraBoundaries();

        for (int i = 0; i < 18; i++)
            _shapeParams[i] = new float[6];

        //Read all drive and shape parameters

//#if !WEBMODE
        for (int i = 0; i < 32; i++)
            ReadValuesDrives(i);

        for (int i = 5; i >= 0; i--) { 
            ReadValuesShapes( i);
        }


//#elif WEBMODE
		/*
        for (int i = 0; i < 32; i++)
            this.StartCoroutine(GetValuesDrives(i));

        for (int i = 5; i >= 0; i--)
            this.StartCoroutine(GetValuesShapes(i));
            */

//#endif

        //Read all shape parameters


        Reset();

		// Get agent's position
		_pos = _agent.transform.position;


    }


    public void Reset() {
		InitAgent (_agent, "Pointing_to_Spot_Netural_02_Updated");
        UpdateParameters();
		CalculateAbsolutePersonality ();
        PlayAnim(_agent, _animInd);
        StopAnim(_agent);  
    }

    void UpdateCameraBoundaries() {
        GameObject camButton = GameObject.Find("ButtonCamera");
        GameObject camChar = GameObject.Find("CharacterCamera");

        camButton.camera.rect = new Rect(0, 0, _scrollWidth / Screen.width, 1); //320 is the width of the parameters
		camChar.camera.rect = new Rect((_scrollWidth / Screen.width) *2, 0, ((Screen.width - _scrollWidth)) / Screen.width, 1);
       
    }

    void CalculateAbsolutePersonality() {
		//  For o,c,e,a,n
		for (int i = 0; i < 5; i++) {
			// Get mean and range for current culture
			float mean = _cultureMeans[_cultureInd, i];
			float range = _cultureRanges[_cultureInd, i];
			// Calculate what to add to the mean - the relative diff normalized for this range
			float diff = _oceanRel[i] * (range/_absRange);
			// Absolute value is this diff added to the cultural mean
			// Make sure it stays in absolute range
			_oceanAbs[i] = Mathf.Clamp(mean + diff, -50.0f, 50.0f);
		}
	}

    void Update(){
        UpdateCameraBoundaries();

		if(Input.GetKeyDown("p")) {
			Debug.Log("P pressed");
			_agent.GetComponent<TorsoController>().Reset();
			
			PlayAnim(_agent, _animInd);
			UpdateParameters(); //we need to update after play because playanim resets torso parameters for speed etc. when animinfo is reset
		}

		// Check if the animation has changed
		if (_animInd != animComboBoxControl.SelectedItemIndex) {
			// User selected a new animation
			_animInd = animComboBoxControl.SelectedItemIndex;
			StopAnimations();
			_agent.GetComponent<TorsoController>().AssignInitRootandFootPos();
		} 

		// Check if the selected culture has changed
		if (_cultureInd != cultureComboBoxControl.SelectedItemIndex) {
			_cultureInd = cultureComboBoxControl.SelectedItemIndex;
			// Recalculate absolute personality
			CalculateAbsolutePersonality();
		}

		// Check if any of the sliders have changed
		for (int i = 0; i < 5; i++) {
			if (_oceanRelTemp[i] != _oceanRel[i]) {
				// Update to the new value
				_oceanRel[i] = _oceanRelTemp[i];
				// Recalculate absolute personality
				CalculateAbsolutePersonality();
			}
		}

    }
    

    void StopAnimations() {

         _agent.GetComponent<TorsoController>().Reset();
    
        StopAnim(_agent);
        PlayAnim(_agent, _animInd); //start the next animation
        StopAnim(_agent);

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
            _driveInd = _effortCombination[key];
            //right
            //space = 1;
            //key = space * 27 + weight * 9 + time * 3 + flow;
            //_driveIndRight = _effortCombination[key];
        }

        else if (cVal == 1) { //weight

            space = othersList[qVal,0];
            time = othersList[qVal,1];
            flow = othersList[qVal,2];

            //Left
            weight = -1;
            key = space * 27 + weight * 9 + time * 3 + flow;
            _driveInd = _effortCombination[key];


            //right
            //weight = 1;
            //key = space * 27 + weight * 9 + time * 3 + flow;
            //_driveIndRight = _effortCombination[key];
        }

        else if (cVal == 2) { //time

            space = othersList[qVal,0];
            weight = othersList[qVal,1];
            flow = othersList[qVal,2];

            //Left
            time = -1;
            key = space * 27 + weight * 9 + time * 3 + flow;
            _driveInd = _effortCombination[key];
     
            //right
            //time = 1;
            //key = space * 27 + weight * 9 + time * 3 + flow;
            //_driveIndRight = _effortCombination[key];
        }

        else if (cVal == 3) { //flow

            space = othersList[qVal,0];
            weight = othersList[qVal,1];
            time = othersList[qVal,2];

            //Left
            flow = -1;
            key = space * 27 + weight * 9 + time * 3 + flow;
            _driveInd = _effortCombination[key];


            //right
            //flow = 1;
            //key = space * 27 + weight * 9 + time * 3 + flow;
            //_driveIndRight = _effortCombination[key];
        }

        
    }
    

    void UpdateParameters() {

        
        //Find driveIndLeft and driveIndRight
        ComputeBothDriveInds(_qInd);
        //Update character
        UpdateEmoteParams(_agent, _driveInd);

    }



	void OnGUI () {
        
        GUIStyle style = new GUIStyle();
        GUI.skin = ButtonSkin;
    
		// Left side
		// Includes dropdowns for animation and culture, play button

		// Animation selector
        GUILayout.BeginArea (new Rect (30,25,250,300));
        style.fontSize = 18;
        style.normal.textColor = new Color(0.2f, 0.2f, 0.2f);

		GUILayout.Label ("Animation: ", style);
		animComboBoxControl.Show ();
		GUILayout.EndArea ();

		// Culture selector
		GUILayout.BeginArea (new Rect (275, 25, 250, 300));
		GUILayout.Label ("Culture: ", style);
		cultureComboBoxControl.Show ();

		GUILayout.EndArea ();

		// Play button
		GUILayout.BeginArea (new Rect(30,250,100,100));
		GUI.color = Color.white;
		//GUILayout.Space (50);

		if(GUILayout.Button ( "Play")) {
			_agent.GetComponent<TorsoController>().Reset();
			
			PlayAnim(_agent, _animInd);
			UpdateParameters(); //we need to update after play because playanim resets torso parameters for speed etc. when animinfo is reset
		}
		
		GUILayout.EndArea();


		// Right side
		// Includes relative and absolute personality

		//GUILayout.BeginArea (new Rect (450,30,250,250));
		GUILayout.BeginArea (new Rect (Screen.width - 300,30,250,600));

		GUILayout.Label ("Relative Personality", style);		

		float minVal = -50.0f;
		float maxVal = 50.0f;

		style.fontSize = 14;
		GUILayout.Label ("Openness",style);
		_oceanRelTemp[0] = GUILayout.HorizontalSlider (_oceanRel[0], minVal, maxVal);
		GUILayout.Label("Conscientiousness", style);
		_oceanRelTemp[1] = GUILayout.HorizontalSlider (_oceanRel[1], minVal, maxVal);
		GUILayout.Label ("Extroversion", style);
		_oceanRelTemp[2] = GUILayout.HorizontalSlider (_oceanRel[2], minVal, maxVal);
		GUILayout.Label ("Agreeableness", style);
		_oceanRelTemp[3] = GUILayout.HorizontalSlider (_oceanRel[3], minVal, maxVal);
		GUILayout.Label ("Neuroticism", style);
		_oceanRelTemp[4] = GUILayout.HorizontalSlider (_oceanRel[4], minVal, maxVal);
      
		//style.fontSize = 18;
		//GUILayout.Label("");
		GUILayout.Space (30);
		style.fontSize = 18;
		GUILayout.Label ("Absolute Personality", style);
		style.fontSize = 14;
		GUILayout.Label ("Openness",style);
		GUILayout.Label (_oceanAbs[0].ToString(),style);
		GUILayout.Label("Conscientiousness", style);
		GUILayout.Label (_oceanAbs[1].ToString(),style);
		GUILayout.Label ("Extroversion", style);
		GUILayout.Label (_oceanAbs[2].ToString(),style);
		GUILayout.Label ("Agreeableness", style);
		GUILayout.Label (_oceanAbs[3].ToString(),style);
		GUILayout.Label ("Neuroticism", style);
		GUILayout.Label (_oceanAbs[4].ToString(),style);

		GUILayout.EndArea();

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
        
        agent.animation.enabled = true;                        
        agent.animation.Play(animName);

    }


    void PlayAnim(GameObject agent, int ind) {

		Debug.Log ("PlayAnim");

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
        
        return _animDisplayNameStr[ind];        
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
            Debug.Log("Error: " + download.error);
        }
        else {
			Debug.Log("Getting drive values");
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
//            print("Error: " + download.error);
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

   
 }		

