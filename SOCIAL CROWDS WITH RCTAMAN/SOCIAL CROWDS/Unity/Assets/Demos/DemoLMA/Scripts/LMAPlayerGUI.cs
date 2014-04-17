#define WEBMODE


using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;

public class LMAPlayerGUI : MonoBehaviour {

	private static Vector2 _scrollPosition;

	// Animation selector
	GUIContent[] animComboBoxList;
	private ComboBox animComboBoxControl; // = new ComboBox();
	private GUIStyle listStyle = new GUIStyle();

	// Culture selector
	GUIContent[] cultureComboBoxList;
	private ComboBox cultureComboBoxControl;

	// Ocean personality parameters, range from -50 to 50
	private static float minVal = -50.0f;
	private static float maxVal = 50.0f;
	// Relative
	private float[] _oceanRel = new float[5] {0,0,0,0,0};
	// Need a temp one to detect changes to the sliders
	private float[] _oceanRelTemp = new float[5] {0,0,0,0,0};

	// Absolute (combines relative with cultural baseline)
	private float[] _oceanAbs = new float[5] {0,0,0,0,0};
	private float _absRange = 50;


	// Cultures
	private static string[] _cultureNames = {"American", "Arab"};
	private int _cultureInd = 0;
	// Mean OCEAN values for different cultures          
	// Row is culture, col is personality factor            O    C    E    A   N
	private static float[,] _cultureMeans= new float[2, 5] {  { 30, -10,  10, -20, 0}, 
		          									      {-30,  20, -10,  30, 0}};
	private static float[,] _cultureMeansTemp = new float[2, 5] {  { 30, -10,  10, -20, 0}, 
		{-30,  20, -10,  30, 0}};


	private static float[,] _cultureRanges = new float[2, 5] {{ 40,  50,  50,  20, 40},
														  { 20,  50,  30,  40, 50}};
	private static float[,] _cultureRangesTemp = new float[2, 5] {{ 40,  50,  50,  20, 40},
		{ 20,  50,  30,  40, 50}};

	// LMA Parameters
	private static float _space = 0f, _weight = 0f, _time = 0f, _flow = 0f;  //[-1 1]
	private static float _verArm = 0f, _horArm = 0f, _sagArm = 0f; //[-1 1] 
	private static float _verTorso = 0f, _horTorso = 0f, _sagTorso = 0f;
	private float _effortMin = -1.0f;
	private float _effortMax = 1.0f;
	private float _shapeMin = -1.0f;
	private float _shapeMax = 1.0f;

	// Personality -> Effort coefficients
	// TODO: Read these in from a file
	private float[,] _pe_coeffs = new float[5, 4]; 
	/*
		{{ .230284f, .086698f, -.01184f, .254413f }, // O
		{ -.29321f, .382264f, .005813f, -.14883f }, // C
		{ .03929f, .45567f, -.48084f, .230689f }, // E
		{ -.18096f, -.01417f, .194684f, -.17622f }, // A
		{ -.25626f, .061196f, .306826f, -.18985f }}; // N */
	// Row = OCEAN factor (row 0 = O, row 1 = C, etc)
	// Col = Effort factor (col 0 = space, 1 = weight, 2 = time, 3 = flow) 


	// Low level motion parameters
	private float _speed = 0f;
	private float _v0 = 0f;
	private float _v1 = 0f;
	private float _ti = 0f;
	private float _texp = 0f;
	private float _tval = 0f;
	private float _continuity = 0f;
	private float _bias = 0f;
	private float _t0 = 0f;
	private float _t1 = 0f;

    //Flourishes
	private float _trMag = 0f;
	private float _tfMag = 0f;

	private float _hrMag = 0f;
	private float _hfMag = 0f;
	private float _squashMag = 0f;
	private float _wbMag = 0f;
	private float _wxMag = 0f;
	private float _wtMag = 0f;
	private float _wfMag = 0f;
	private float _etMag = 0f;
	private float _dMag = 0f;
	private float _efMag = 0f;

    //Shape for drives
	private float _encSpr0 = 0f;
	private float _sinRis0 = 0f;
	private float _retAdv0 = 0f;

	private float _encSpr1 = 0f;
	private float _sinRis1 = 0f;
	private float _retAdv1 = 0f;

    //Arm shape for drives
    private static Vector3[] _arm = new Vector3[2];

    static bool _toggleContinuous = false;
    public Texture TexBorder = new Texture();
    public GUISkin ButtonSkin;

    
   // private string[] _animNameStr = { "Knocking", "Pointing", "Lifting", "Picking up pillow", "Punching", "Pushing", "Throwing", "Walking", "Waving" };
	private static string[] _animDisplayNameStr = { "Pointing", "Picking up a pillow", "Lifting", "Knocking"};
	//, "Punching", "Pushing", "Throwing", "Walking", "Waving"};
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
	

    public string ShapeInfo = "";
    public string Info = "waiting...";
	
    private float _scrollWidth = 0;

    //read shape values once in the beginning. no need to reread everytime
    float[][] _shapeParams = new float[18][];
	
    GameObject _agent;



    void Start(){

		// Setting up the combo boxes

		// Styling for both combo boxes
		listStyle.normal.textColor = Color.white; 
		listStyle.onHover.background =
			listStyle.hover.background = new Texture2D(2, 2);
		listStyle.padding.left =
			listStyle.padding.right =
				listStyle.padding.top =
					listStyle.padding.bottom = 4;

		// Combo box for selecting animation
		animComboBoxList = new GUIContent[_animDisplayNameStr.Length];
		for (int i= 0; i < _animDisplayNameStr.Length; i++) {
			animComboBoxList[i] = new GUIContent(_animDisplayNameStr[i]);
		}

		
		animComboBoxControl = new ComboBox(new Rect(0, 30, 200, 20), animComboBoxList[0], animComboBoxList, "button", "box", listStyle);

		// Combo box for selecting culture
		cultureComboBoxList = new GUIContent[_cultureNames.Length];
		for (int i= 0; i < _cultureNames.Length; i++) {
			cultureComboBoxList[i] = new GUIContent(_cultureNames[i]);
		}
		cultureComboBoxControl = new ComboBox(new Rect(0, 30, 200, 20), cultureComboBoxList[0], 
		                                      cultureComboBoxList, "button", "box", listStyle);
        
		// Load the agent
		_agent = GameObject.Find ("AgentPrefab");

		if (!_agent) {

			Debug.Log("Agent prefab not found");
			return;
		}


        UpdateCameraBoundaries();

        for (int i = 0; i < 18; i++)
            _shapeParams[i] = new float[6];

        //Read all shape parameters
        for (int i = 5; i >= 0; i--) { 
            ReadValuesShapes( i);
        }

		ReadPersonalityEffortCoeffs ();
        Reset();
    }


    public void Reset() {
		InitAgent (_agent, "Pointing_to_Spot_Netural_02_Updated");
		CalculateAbsolutePersonality ();
		UpdateLaban ();
		UpdateEmoteParams (_agent);
        PlayAnim(_agent, _animInd);
        StopAnim(_agent);  
    }

	void ResetPersonality() {
		for (int i = 0; i < 5; i++){
			_oceanRel[i] = 0.0f;
			_oceanRelTemp[i] = 0.0f;
		}
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
			UpdateEmoteParams(_agent);
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
			UpdateLaban();
		}

		// Check if any of the relative personality sliders have changed
		for (int i = 0; i < 5; i++) {
			if (_oceanRelTemp[i] != _oceanRel[i]) {
				// Update to the new value
				_oceanRel[i] = _oceanRelTemp[i];
				// Recalculate absolute personality
				CalculateAbsolutePersonality();
				UpdateLaban();
			}
		}

		// Check if any of the culture mean sliders have changed
		for (int i = 0; i < _cultureNames.Length; i++) {
			for (int j = 0; j < 5; j++){
				if (_cultureMeans[i,j] != _cultureMeansTemp[i,j]) {
					// Update to the new value
					_cultureMeans[i,j] = _cultureMeansTemp[i,j];
					// Recalculate absolute personality
					CalculateAbsolutePersonality();
					UpdateLaban();
				}
				if (_cultureRanges[i,j] != _cultureRangesTemp[i,j]){
					// Update to the new value
					_cultureRanges[i,j] = _cultureRangesTemp[i,j];
					// Recalculate absolute personality
					CalculateAbsolutePersonality();
					UpdateLaban();
				}
			}
		}

    }
    

    void StopAnimations() {

         _agent.GetComponent<TorsoController>().Reset();
    
        StopAnim(_agent);
        PlayAnim(_agent, _animInd); //start the next animation
        StopAnim(_agent);
		UpdateEmoteParams (_agent);

    }


	void UpdateLaban() {
		// Calculate effort from absolute personality
		_space = _weight = _time = _flow = 0f;
		for (int i = 0; i < 5; i++) {
			// For each of the 5 personality factors
			// (_pe_coeffs) Row = OCEAN factor (row 0 = O, row 1 = C, etc)
			//              Col = Effort factor (col 0 = space, 1 = weight, 2 = time, 3 = flow)
			_space += ( _oceanAbs[i]/50.0f * _pe_coeffs[i, 0]);
			_weight += (_oceanAbs[i]/50.0f * _pe_coeffs[i, 1]);
			_time += (_oceanAbs[i]/50.0f * _pe_coeffs[i, 2]);
			_flow += (_oceanAbs[i]/50.0f * _pe_coeffs[i, 3]);

		}
	}


	/*
    void UpdateLaban() {
		// Calculate Laban parameters from the absolute OCEAN values
		// Weight each equally - each will be the average of 5 values
		float[] spaceVals = new float[5];   // Indirect <-> Direct [-1 1]
		float[] weightVals = new float[5];  // Light <-> Strong
		float[] timeVals = new float[5];    // Sustained <-> Sudden
		float[] flowVals = new float[5];    // Free <-> Bound

		// Openness
		// High O -> Indirect, Light, Sustained, Free
		spaceVals [0] = _oceanAbs [0] / 50 * -1;
		weightVals [0] = _oceanAbs [0] / 50 * -1;
		timeVals[0] = _oceanAbs [0] / 50 * -1;
		flowVals[0] = _oceanAbs [0] / 50 * -1;

		// Conscientiousness
		// High C -> Direct, Strong, Sudden, Bound
		spaceVals [1] = _oceanAbs [1] / 50 * 1;
		weightVals [1] = _oceanAbs [1] / 50 * 1;
		timeVals[1] = _oceanAbs [1] / 50 * 1;
		flowVals[1] = _oceanAbs [1] / 50 * 1;

		// Extroversion
		// High E -> Indirect, Light, Sustained, Free
		spaceVals [2] = _oceanAbs [2] / 50 * -1;
		weightVals [2] = _oceanAbs [2] / 50 * -1;
		timeVals[2] = _oceanAbs [2] / 50 * -1;
		flowVals[2] = _oceanAbs [2] / 50 * -1;

		// Agreeableness
		// High A -> Indirect, Light, Sustained, Free
		spaceVals [3] = _oceanAbs [3] / 50 * -1;
		weightVals [3] = _oceanAbs [3] / 50 * -1;
		timeVals[3] = _oceanAbs [3] / 50 * -1;
		flowVals[3] = _oceanAbs [3] / 50 * -1;

		// Neuroticism
		// High N -> Direct, Strong, Sudden, Free
		spaceVals [4] = _oceanAbs [4] / 50 * 1;
		weightVals [4] = _oceanAbs [4] / 50 * 1;
		timeVals[4] = _oceanAbs [4] / 50 * 1;
		flowVals[4] = _oceanAbs [4] / 50 * -1;

		// Calculate averages
		_space = 0.0f;  _weight = 0.0f;  _time = 0.0f; _flow = 0.0f;
		for (int i = 0; i < 5; i++) {
			_space += spaceVals[i];
			_weight += weightVals[i];
			_time += timeVals[i];
			_flow += flowVals[i];
				}
		_space = _space / 5.0f;
		_weight = _weight / 5.0f;
		_time = _time / 5.0f;
		_flow = _flow / 5.0f;

		// Calculate horizontal shape
		// Associated negatively (?) with space
		_horArm = -_space;
		_horTorso = -_space;

		// Calculate vertical shape
		// Associated negatively (?) with weight
		_verArm = -_weight;
		_verTorso = -_weight;

		// Calculate sagittal shape
		// Associated negatively (?) with time
		_sagArm = -_time;
		_sagTorso = -_time;


	}
*/
	void OnGUI () {
        
        GUIStyle style = new GUIStyle();
        GUI.skin = ButtonSkin;
		style.normal.textColor = new Color(0.2f, 0.2f, 0.2f);

		// Play button
		GUILayout.BeginArea (new Rect(Screen.width/2 - 50,500,100,100));
		GUI.color = Color.white;
		//GUILayout.Space (50);
		
		if(GUILayout.Button ( "Play")) {
			_agent.GetComponent<TorsoController>().Reset();
			
			PlayAnim(_agent, _animInd);
			UpdateEmoteParams(_agent);
		}

		GUILayout.EndArea();

		// Left side
		// Includes dropdowns for animation and culture, play button

		// Culture selector
		style.fontSize = 18;
		GUILayout.BeginArea (new Rect (30, 25, 250, 150));
			GUILayout.Label ("Culture: ", style);
			cultureComboBoxControl.Show ();
		GUILayout.EndArea ();

		// Dividing line
		GUI.DrawTexture(new Rect(300 , 0, 4, Screen.height), TexBorder, ScaleMode.ScaleToFit, true, 2f/Screen.height);


		// Adjust the means and ranges for the selected culture
		GUILayout.BeginArea (new Rect (30, 150, 250, 500));
			style.fontSize = 18;
			//GUILayout.Label("Means for culture: " + _cultureNames[_cultureInd],style);
			GUILayout.Label("Cultural Means",style);
			style.fontSize = 14;
			GUILayout.Label ("Openness: " + _cultureMeans [_cultureInd, 0].ToString (), style);
			_cultureMeansTemp [_cultureInd, 0] = GUILayout.HorizontalSlider (_cultureMeans [_cultureInd, 0], minVal, maxVal);
			GUILayout.Label ("Conscientiousness: " + _cultureMeans [_cultureInd, 1].ToString (), style);
			_cultureMeansTemp [_cultureInd, 1] = GUILayout.HorizontalSlider (_cultureMeans [_cultureInd, 1], minVal, maxVal);
			GUILayout.Label ("Extroversion: " + _cultureMeans [_cultureInd, 2].ToString (), style);
			_cultureMeansTemp [_cultureInd, 2] = GUILayout.HorizontalSlider (_cultureMeans [_cultureInd, 2], minVal, maxVal);
			GUILayout.Label ("Agreeableness: " + _cultureMeans [_cultureInd, 3].ToString (), style);
			_cultureMeansTemp [_cultureInd, 3] = GUILayout.HorizontalSlider (_cultureMeans [_cultureInd, 3], minVal, maxVal);
			GUILayout.Label ("Neuroticism: " + _cultureMeans [_cultureInd, 4].ToString (), style);
			_cultureMeansTemp [_cultureInd, 4] = GUILayout.HorizontalSlider (_cultureMeans [_cultureInd, 4], minVal, maxVal);

			GUILayout.Space (50);

			style.fontSize = 18;
			GUILayout.Label("Cultural Ranges",style);
			style.fontSize = 14;
			GUILayout.Label ("Openness: " + _cultureRanges [_cultureInd, 0].ToString (), style);
			_cultureRangesTemp [_cultureInd, 0] = GUILayout.HorizontalSlider (_cultureRanges [_cultureInd, 0], 0f, maxVal);
			GUILayout.Label ("Conscientiousness: " + _cultureRanges [_cultureInd, 1].ToString (), style);
			_cultureRangesTemp [_cultureInd, 1] = GUILayout.HorizontalSlider (_cultureRanges [_cultureInd, 1], 0f, maxVal);
			GUILayout.Label ("Extroversion: " + _cultureRanges [_cultureInd, 2].ToString (), style);
			_cultureRangesTemp [_cultureInd, 2] = GUILayout.HorizontalSlider (_cultureRanges [_cultureInd, 2], 0f, maxVal);
			GUILayout.Label ("Agreeableness: " + _cultureRanges [_cultureInd, 3].ToString (), style);
			_cultureRangesTemp [_cultureInd, 3] = GUILayout.HorizontalSlider (_cultureRanges [_cultureInd, 3], 0f, maxVal);
			GUILayout.Label ("Neuroticism: " + _cultureRanges [_cultureInd, 4].ToString (), style);
			_cultureRangesTemp [_cultureInd, 4] = GUILayout.HorizontalSlider (_cultureRanges [_cultureInd, 4], 0f, maxVal);

		GUILayout.EndArea ();


		// Displaying emote parameters
		GUILayout.BeginArea (new Rect (320, 30, 200, 500));
			style.fontSize = 18;
			GUILayout.Label ("Laban Motion", style);
			style.fontSize = 16;
			GUILayout.Space (10);
			GUILayout.Label ("Effort", style);
			style.fontSize = 14;
			//GUILayout.Label ("Space: " + (Mathf.Round(_space * 1000)/1000).ToString (), style);
			GUILayout.Label ("Space: " + _space.ToString (), style);
			GUILayout.Label ("Weight: " + _weight.ToString (), style);
			GUILayout.Label ("Time: " + _time.ToString (), style);
			GUILayout.Label ("Flow: " + _flow.ToString (), style);

			style.fontSize = 16;
			GUILayout.Space (10);
			GUILayout.Label ("Arm shape",style);
			style.fontSize = 14;
			GUILayout.Label ("Vertical: " + _verArm.ToString (), style);
			GUILayout.Label ("Horizontal: " + _horArm.ToString (), style);
			GUILayout.Label ("Sagittal: " + _sagArm.ToString (), style);

			style.fontSize = 16;
			GUILayout.Space (10);
			GUILayout.Label ("Torso shape",style);
			style.fontSize = 14;
			GUILayout.Label ("Vertical: " + _verTorso.ToString (), style);
			GUILayout.Label ("Horizontal: " + _horTorso.ToString (), style);
			GUILayout.Label ("Sagittal: " + _sagTorso.ToString (), style);
		GUILayout.EndArea ();



		// Far Right side
		// Includes relative and absolute personality

		//GUILayout.BeginArea (new Rect (450,30,250,250));

		GUI.DrawTexture(new Rect(Screen.width -310 , 0, 4, Screen.height), TexBorder, ScaleMode.ScaleToFit, true, 2f/Screen.height);

		GUILayout.BeginArea (new Rect (Screen.width - 290,30,290,800));

			//_scrollPosition = GUILayout.BeginScrollView(_scrollPosition,  GUILayout.Width(220f), GUILayout.Height(Screen.height*0.98f));
			_scrollPosition = GUILayout.BeginScrollView (_scrollPosition, GUILayout.Width(285f), GUILayout.Height(Screen.height*0.98f-30));

				// Animation selector
				//GUILayout.BeginArea (new Rect (Screen.width - 300 ,25,250,300));
				style.fontSize = 18;
				style.normal.textColor = new Color(0.2f, 0.2f, 0.2f);
				
				GUILayout.Label ("Animation: ", style);
				animComboBoxControl.Show ();
				//GUILayout.EndArea ();

				GUILayout.Space (150);

				//GUILayout.BeginArea (new Rect (Screen.width - 300, 200, 250, 600));
				style.fontSize = 18;
				GUILayout.Label ("Relative Personality", style);		

				style.fontSize = 14;
				GUILayout.Label ("Openness: " + _oceanRel[0],style);
				_oceanRelTemp[0] = GUILayout.HorizontalSlider (_oceanRel[0], minVal, maxVal);
				GUILayout.Label("Conscientiousness: " + _oceanRel[1], style);
				_oceanRelTemp[1] = GUILayout.HorizontalSlider (_oceanRel[1], minVal, maxVal);
				GUILayout.Label ("Extroversion: " + _oceanRel[2], style);
				_oceanRelTemp[2] = GUILayout.HorizontalSlider (_oceanRel[2], minVal, maxVal);
				GUILayout.Label ("Agreeableness: " + _oceanRel[3], style);
				_oceanRelTemp[3] = GUILayout.HorizontalSlider (_oceanRel[3], minVal, maxVal);
				GUILayout.Label ("Neuroticism: " + _oceanRel[4], style);
				_oceanRelTemp[4] = GUILayout.HorizontalSlider (_oceanRel[4], minVal, maxVal);

				// Reset button
				GUILayout.Space (10);
				if(GUILayout.Button ( "Reset")) {
					StopAnim(_agent);
					ResetPersonality();
					CalculateAbsolutePersonality();
					UpdateLaban();
					UpdateEmoteParams(_agent);
				}
		      
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

				GUILayout.Space (50);

				//GUILayout.EndArea();

			GUILayout.EndScrollView ();

		GUILayout.EndArea ();

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

		//GameObject agentControl = GameObject.Find("AgentControlPrefab");

        if (agent.GetComponent<ArmAnimator>().ArmC == null || agent.GetComponent<TorsoAnimator>().TorsoC == null) {
			Debug.Log ("Controller not assigned");
			return;
		}


        AnimationInfo animInfo = agent.GetComponent<AnimationInfo>();
        animInfo.IsContinuous = _toggleContinuous;
        StopAnim(agent); //stop first

		InitAgent (agent, _animNames [ind]);
		agent.animation[animInfo.AnimName].speed = animInfo.DefaultAnimSpeed;
	
        if(animInfo.IsContinuous)
            agent.animation[animInfo.AnimName].wrapMode = WrapMode.Loop;            
        
        else
            agent.animation[animInfo.AnimName].wrapMode = WrapMode.ClampForever;                            

    }


	void UpdateEmoteParams(GameObject agent) {

		if(agent == null){		
			Debug.Log("AgentPrefab not found");
			return;
		}

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

		//Update shape parameters
		for (int j = 0; j < _shapeParams.Length; j++)
			for (int i = 0; i < _shapeParams[j].Length; i++)
				agent.GetComponent<TorsoAnimator>().ShapeParams[j][i] = _shapeParams[j][i];

		
		agent.GetComponent<TorsoAnimator>().UpdateAnglesLinearComb();
		
	}

	
	
	/*
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
	*/

	void ReadPersonalityEffortCoeffs() {
		string fileName = "personalityToEffort.txt";
		StreamReader sr = new StreamReader (fileName);
		string[] content = File.ReadAllLines (fileName);

		for (int oceanInd = 1; oceanInd<content.Length; oceanInd++) {
			// Start at 1 because the first line of the file is text labels
			string line = content[oceanInd];
			// Each line contains parameters for an OCEAN factor
			String[] tokens = line.Split('\t');
			for (int effortInd = 1; effortInd < tokens.Length; effortInd++){
				// Again, start at 1 because first token is a string label
				_pe_coeffs[oceanInd-1, effortInd-1] = float.Parse(tokens[effortInd]);
				Debug.Log(tokens[effortInd]);
			}
		}
	}


    void ReadValuesShapes( int shapeInd) {
        string fileName = "shapesSusan.txt";
        StreamReader sr = new StreamReader(fileName);

        string[] content = File.ReadAllLines(fileName);
		//Debug.Log (content [0]);

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


 }		

