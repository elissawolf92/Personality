#define WEBMODE


using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;

public class LMAPlayerGUI : MonoBehaviour {

	private static Vector2 _scrollPositionRight;
	private static Vector2 _scrollPositionLeft;

	// Animation selector
	GUIContent[] animComboBoxList;
	private ComboBox animComboBoxControl; // = new ComboBox();
	private GUIStyle listStyle = new GUIStyle();

	// Culture selector
	GUIContent[] cultureComboBoxList;
	private ComboBox cultureComboBoxControl;

	// Gender selector
	GUIContent[] genderComboBoxList;
	private ComboBox genderComboBoxControl;

	// Mode selector
	GUIContent[] modeComboBoxList;
	private ComboBox modeComboBoxControl;

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

	// Modes
	private int _modeInd = 0;
	private static string[] _modeNames = {"Culture", "Gender", "Both"};

	// Genders
	private int _genderInd = 0;
	private static string[] _genderNames = {"Male", "Female"};
	private static float[,] _genderMeans = new float[2,5] {{22.0f, 16.4f, 17.4f, 23f, 3.6f}, // Male
		{22.2f, 17.0f, 18.4f, 27.8f, 8.8f}}; // Female

	private static float[,] _genderMeansTemp = new float[2,5] {{22.0f, 16.4f, 17.4f, 23f, 3.6f}, // Male
		{22.2f, 17.0f, 18.4f, 27.8f, 8.8f}}; // Female

	private static float[,] _genderRanges = new float[2,5] {{30.6f, 32.4f, 33.0f, 30.0f, 39.0f}, // Male
		{31.2f, 34.8f, 35.4f, 30.0f, 40.2f}}; // Female

	private static float[,] _genderRangesTemp = new float[2,5] {{30.6f, 32.4f, 33.0f, 30.0f, 39.0f}, // Male
		{31.2f, 34.8f, 35.4f, 30.0f, 40.2f}}; // Female

	// Cultures
	private static string[] _cultureNames = {"US", "Lebanon", "Japan"};
	private int _cultureInd = 0;
	// Mean OCEAN values for different cultures          
	// Row is culture, col is personality factor            O    C    E    A   N
	private static float[,] _cultureMeans= new float[3, 5] {  { 0f, 0f, 0f, 0f, 0f}, // US
		{-0.6f,  -5.44f, -1.68f,  -3.90f, 3.35f}, // Lebanon
		{-8.47f, -12.17f, -3.27f, -7.79f, 7.87f}}; // Japan

	private static float[,] _cultureMeansTemp= new float[3, 5] {  { 0f, 0f,  0f, 0f, 0f}, // US
		{-0.6f,  -5.44f, -1.68f,  -3.90f, 3.35f}, // Lebanon
		{-8.47f, -12.17f, -3.27f, -7.79f, 7.87f}}; // Japan

	private static float[,] _cultureRanges = new float[3, 5] {{ 30f,  30f,  30f,  30f, 30f}, // US
		{ 27.33f,  31.2f,  25.74f,  24.42f, 27.42f}, // Lebanon
		{ 31.38f, 27.9f, 24.18f, 26.43f, 22.14f}}; // Japan
	
	private static float[,] _cultureRangesTemp = new float[3, 5] {{ 30f,  30f,  30f,  30f, 30f}, // US
		{ 27.33f,  31.2f,  25.74f,  24.42f, 27.42f}, // Lebanon
		{ 31.38f, 27.9f, 24.18f, 26.43f, 22.14f}}; // Japan

	// LMA Parameters
	private static float _space = 0f, _weight = 0f, _time = 0f, _flow = 0f;  //[-1 1]
	private static float _verArm = 0f, _horArm = 0f, _sagArm = 0f; //[-1 1] 
	private static float _verTorso = 0f, _horTorso = 0f, _sagTorso = 0f;
	private float _effortMin = -1.0f;
	private float _effortMax = 1.0f;
	private float _shapeMin = -1.0f;
	private float _shapeMax = 1.0f;

	// Personality -> Effort coefficients
	private float[,] _pe_coeffs = new float[5, 4]; 
	// Row = OCEAN factor (row 0 = O, row 1 = C, etc)
	// Col = Effort factor (col 0 = space, 1 = weight, 2 = time, 3 = flow) 

	// Effort -> Motion coefficients
	private float[,] _em_coeffs = new float[33, 5];
	// Rows are motion parameters
	// Cols are effort components


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
	private static string[] _animDisplayNameStr = { "Pointing", "Picking up a pillow", "Lifting", "Knocking" };
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

		// Combo box for selecting gender
		genderComboBoxList = new GUIContent[_genderNames.Length];
		for (int i= 0; i < _genderNames.Length; i++) {
			genderComboBoxList[i] = new GUIContent(_genderNames[i]);
		}
		genderComboBoxControl = new ComboBox(new Rect(0, 600, 200, 20), genderComboBoxList[0], 
		                                      genderComboBoxList, "button", "box", listStyle);

		// Combo box for selecting mode
		modeComboBoxList = new GUIContent[_modeNames.Length];
		for (int i= 0; i < _modeNames.Length; i++) {
			modeComboBoxList[i] = new GUIContent(_modeNames[i]);
		}
		modeComboBoxControl = new ComboBox(new Rect(0, 30, 200, 20), modeComboBoxList[0], 
		                                     modeComboBoxList, "button", "box", listStyle);
        
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
		ReadEffortMotionCoeffs ();
        Reset();


		// Testing
		/*
		Debug.Log ("Testing motion parameter calculating");
		Debug.Log ("Space: " + _space.ToString ());
		Debug.Log ("Weight: " + _weight.ToString ());
		Debug.Log ("Time: " + _time.ToString ());
		Debug.Log ("Flow: " + _flow.ToString ());
		Debug.Log ("Calculated speed: " + CalculateMotionParameter (0).ToString ());
	*/
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
			//float mean = _cultureMeans[_cultureInd, i];
			//float range = _cultureRanges[_cultureInd, i];
			float mean = 0.0f;
			float range = 0.0f;

			if (_modeInd == 0) { // Culture
				mean = _cultureMeans[_cultureInd, i];
				range = _cultureRanges[_cultureInd, i];
			}
			else if (_modeInd == 1) { // Gender
				mean = _genderMeans[_genderInd, i];
				range = _genderRanges[_genderInd, i];
			}
			else { // Combined
				mean = (_genderMeans[_genderInd, i] + _cultureMeans[_cultureInd, i])/2.0f;
				range = (_genderRanges[_genderInd, i] + _cultureRanges[_cultureInd, i])/2.0f;
			}

			// Calculate what to add to the mean - the relative diff normalized for this range
			float diff = _oceanRel[i] * (range/_absRange);
			// Absolute value is this diff added to the cultural mean
			// Make sure it stays in absolute range
			_oceanAbs[i] = Mathf.Clamp(mean + diff, -50.0f, 50.0f);
		}
	}

    void Update(){
        UpdateCameraBoundaries();

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

		// Check if the selected gender has changed
		if (_genderInd != genderComboBoxControl.SelectedItemIndex) {
			_genderInd = genderComboBoxControl.SelectedItemIndex;
			// Recalculate absolute personality
			CalculateAbsolutePersonality();
			UpdateLaban();
		}

		// Check if selected mode has changed
		if (_modeInd != modeComboBoxControl.SelectedItemIndex) {
			_modeInd = modeComboBoxControl.SelectedItemIndex;
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

		// Check if any of the culture mean or range sliders have changed
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

		// Check if any of the gender mean or range sliders have changed
		for (int i = 0; i < _genderNames.Length; i++) {
			for (int j = 0; j < 5; j++){
				if (_genderMeans[i,j] != _genderMeansTemp[i,j]) {
					// Update to the new value
					_genderMeans[i,j] = _genderMeansTemp[i,j];
					// Recalculate absolute personality
					CalculateAbsolutePersonality();
					UpdateLaban();
				}
				if (_genderRanges[i,j] != _genderRangesTemp[i,j]){
					// Update to the new value
					_genderRanges[i,j] = _genderRangesTemp[i,j];
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

	void OnGUI () {
        
        GUIStyle style = new GUIStyle();
        GUI.skin = ButtonSkin;
		style.normal.textColor = new Color(0.2f, 0.2f, 0.2f);

		// Play button
		GUILayout.BeginArea (new Rect(600,500,100,100));
		GUI.color = Color.white;
		//GUILayout.Space (50);
		
			if(GUILayout.Button ( "Play")) {
				_agent.GetComponent<TorsoController>().Reset();
				
				PlayAnim(_agent, _animInd);
				UpdateEmoteParams(_agent);
			}

			GUILayout.Space (10);
			if(GUILayout.Button ( "Reset")) {
				StopAnim(_agent);
				ResetPersonality();
				CalculateAbsolutePersonality();
				UpdateLaban();
				UpdateEmoteParams(_agent);
			}
		GUILayout.EndArea ();

		GUILayout.BeginArea (new Rect (750, 500, 200, 300));
			style.fontSize = 18;
			GUILayout.Label ("Mode:", style);
			modeComboBoxControl.Show ();

		GUILayout.EndArea();

		GUILayout.BeginArea (new Rect (360, 500, 200, 200));
			style.fontSize = 18;
			GUILayout.Label ("Laban Motion - Effort", style);
			//style.fontSize = 16;
			GUILayout.Space (10);
			//GUILayout.Label ("Effort", style);
			style.fontSize = 14;
			//GUILayout.Label ("Space: " + (Mathf.Round(_space * 1000)/1000).ToString (), style);
			GUILayout.Label ("Space: " + _space.ToString (), style);
			GUILayout.Label ("Weight: " + _weight.ToString (), style);
			GUILayout.Label ("Time: " + _time.ToString (), style);
			GUILayout.Label ("Flow: " + _flow.ToString (), style);
		GUILayout.EndArea ();

		// Left side
		// Includes dropdowns for animation and culture, play button

		GUILayout.BeginArea (new Rect (30, 25, 310, 1000));

		_scrollPositionLeft = GUILayout.BeginScrollView (_scrollPositionLeft, GUILayout.Width(300f), GUILayout.Height(Screen.height*0.98f-30));

			// Culture selector
			style.fontSize = 18;
			//GUILayout.BeginArea (new Rect (30, 25, 250, 150));
				GUILayout.Label ("Country: ", style);
				cultureComboBoxControl.Show ();
			//GUILayout.EndArea ();

			// Dividing line
			GUI.DrawTexture(new Rect(300 , 0, 4, Screen.height), TexBorder, ScaleMode.ScaleToFit, true, 2f/Screen.height);

			GUILayout.Space (110);
			// Adjust the means and ranges for the selected culture
			//GUILayout.BeginArea (new Rect (30, 150, 250, 500));
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

			GUILayout.Space (20);

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

			//GUILayout.EndArea ();

			GUILayout.Space (20);
			// Gender stuff
			//GUILayout.BeginArea (new Rect (30, 800, 250, 500));
			style.fontSize = 18;
			GUILayout.Label ("Gender: ", style);
			genderComboBoxControl.Show ();
			
			GUILayout.Space (80);
			GUILayout.Label("Gender Means",style);
			style.fontSize = 14;
			GUILayout.Label ("Openness: " + _genderMeans [_genderInd, 0].ToString (), style);
			_genderMeansTemp [_genderInd, 0] = GUILayout.HorizontalSlider (_genderMeans [_genderInd, 0], minVal, maxVal);
			GUILayout.Label ("Conscientiousness: " + _genderMeans [_genderInd, 1].ToString (), style);
			_genderMeansTemp [_genderInd, 1] = GUILayout.HorizontalSlider (_genderMeans [_genderInd, 1], minVal, maxVal);
			GUILayout.Label ("Extroversion: " + _genderMeans [_genderInd, 2].ToString (), style);
			_genderMeansTemp [_genderInd, 2] = GUILayout.HorizontalSlider (_genderMeans [_genderInd, 2], minVal, maxVal);
			GUILayout.Label ("Agreeableness: " + _genderMeans [_genderInd, 3].ToString (), style);
			_genderMeansTemp [_genderInd, 3] = GUILayout.HorizontalSlider (_genderMeans [_genderInd, 3], minVal, maxVal);
			GUILayout.Label ("Neuroticism: " + _genderMeans [_genderInd, 4].ToString (), style);
			_genderMeansTemp [_genderInd, 4] = GUILayout.HorizontalSlider (_genderMeans [_genderInd, 4], minVal, maxVal);
			
			GUILayout.Space (20);
			
			style.fontSize = 18;
			GUILayout.Label("Gender Ranges",style);
			style.fontSize = 14;
			GUILayout.Label ("Openness: " + _genderRanges [_genderInd, 0].ToString (), style);
			_genderRangesTemp [_genderInd, 0] = GUILayout.HorizontalSlider (_genderRanges [_genderInd, 0], 0f, maxVal);
			GUILayout.Label ("Conscientiousness: " + _genderRanges [_genderInd, 1].ToString (), style);
			_genderRangesTemp [_genderInd, 1] = GUILayout.HorizontalSlider (_genderRanges [_genderInd, 1], 0f, maxVal);
			GUILayout.Label ("Extroversion: " + _genderRanges [_genderInd, 2].ToString (), style);
			_genderRangesTemp [_genderInd, 2] = GUILayout.HorizontalSlider (_genderRanges [_genderInd, 2], 0f, maxVal);
			GUILayout.Label ("Agreeableness: " + _genderRanges [_genderInd, 3].ToString (), style);
			_genderRangesTemp [_genderInd, 3] = GUILayout.HorizontalSlider (_genderRanges [_genderInd, 3], 0f, maxVal);
			GUILayout.Label ("Neuroticism: " + _genderRanges [_genderInd, 4].ToString (), style);
			_genderRangesTemp [_genderInd, 4] = GUILayout.HorizontalSlider (_genderRanges [_genderInd, 4], 0f, maxVal);

			//GUILayout.EndArea ();

		GUILayout.EndScrollView ();

		GUILayout.EndArea ();


		// Far Right side
		// Includes relative and absolute personality

		//GUILayout.BeginArea (new Rect (450,30,250,250));

		GUI.DrawTexture(new Rect(Screen.width -310 , 0, 4, Screen.height), TexBorder, ScaleMode.ScaleToFit, true, 2f/Screen.height);

		GUILayout.BeginArea (new Rect (Screen.width - 290,30,280,800));

			//_scrollPosition = GUILayout.BeginScrollView(_scrollPosition,  GUILayout.Width(220f), GUILayout.Height(Screen.height*0.98f));
			//_scrollPositionRight = GUILayout.BeginScrollView (_scrollPositionRight, GUILayout.Width(285f), GUILayout.Height(Screen.height*0.98f-30));

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
				GUILayout.Space (20);
		      
				//GUILayout.Space (30);
				style.fontSize = 18;
				GUILayout.Label ("Absolute Personality", style);
				style.fontSize = 14;
				GUILayout.Label ("Openness:",style);
				GUILayout.Label (_oceanAbs[0].ToString(),style);
				GUILayout.Label("Conscientiousness:", style);
				GUILayout.Label (_oceanAbs[1].ToString(),style);
				GUILayout.Label ("Extroversion:", style);
				GUILayout.Label (_oceanAbs[2].ToString(),style);
				GUILayout.Label ("Agreeableness:", style);
				GUILayout.Label (_oceanAbs[3].ToString(),style);
				GUILayout.Label ("Neuroticism:", style);
				GUILayout.Label (_oceanAbs[4].ToString(),style);

				GUILayout.Space (50);

				//GUILayout.EndArea();

			//GUILayout.EndScrollView ();

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

		//Debug.Log ("PlayAnim");

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
	

	float CalculateMotionParameter(int motionInd){
		float val = _em_coeffs [motionInd, 0]; // Intercept
		val += _em_coeffs [motionInd, 1] * _space;
		val += _em_coeffs [motionInd, 2] * _weight;
		val += _em_coeffs [motionInd, 3] * _time;
		val += _em_coeffs [motionInd, 4] * _flow;

		return val;
	}

	void UpdateEmoteParams(GameObject agent) {
			if (agent == null) {		
					Debug.Log ("AgentPrefab not found");
					return;
			}
			agent.GetComponent<ArmAnimator> ().SetSpeed (CalculateMotionParameter (0));
	
			agent.GetComponent<ArmAnimator> ().V0 = CalculateMotionParameter (1);
			agent.GetComponent<ArmAnimator> ().V1 = CalculateMotionParameter (2);
			agent.GetComponent<ArmAnimator> ().T0 = CalculateMotionParameter (3);
			agent.GetComponent<ArmAnimator> ().T1 = CalculateMotionParameter (4);
			agent.GetComponent<ArmAnimator> ().Ti = CalculateMotionParameter (5);
			agent.GetComponent<ArmAnimator> ().Texp = CalculateMotionParameter (6);
			agent.GetComponent<ArmAnimator> ().Tval = CalculateMotionParameter (7);
			agent.GetComponent<ArmAnimator> ().Continuity = CalculateMotionParameter (8);
			//agent.GetComponent<ArmAnimator>().Bias = _bias[driveInd];
			//agent.GetComponent<AnimationInfo>().InitInterpolators(_tval[driveInd], _continuity[driveInd], _bias[driveInd]);
			agent.GetComponent<ArmAnimator> ().TrMag = CalculateMotionParameter (9);
			agent.GetComponent<ArmAnimator> ().TfMag = CalculateMotionParameter (10);
			agent.GetComponent<ArmAnimator> ().HrMag = CalculateMotionParameter (11);
			agent.GetComponent<ArmAnimator> ().HfMag = CalculateMotionParameter (12);
			agent.GetComponent<ArmAnimator> ().SquashMag = CalculateMotionParameter (13);
			agent.GetComponent<ArmAnimator> ().WbMag = CalculateMotionParameter (14);
			agent.GetComponent<ArmAnimator> ().WxMag = CalculateMotionParameter (15);
			agent.GetComponent<ArmAnimator> ().WtMag = CalculateMotionParameter (16);
			agent.GetComponent<ArmAnimator> ().WfMag = CalculateMotionParameter (17);
			agent.GetComponent<ArmAnimator> ().EtMag = CalculateMotionParameter (18);
			agent.GetComponent<ArmAnimator> ().EfMag = CalculateMotionParameter (19);
			agent.GetComponent<ArmAnimator> ().DMag = CalculateMotionParameter (20);
	
			agent.GetComponent<TorsoAnimator> ().EncSpr [0] = CalculateMotionParameter (21);
			agent.GetComponent<TorsoAnimator> ().SinRis [0] = CalculateMotionParameter (22);
			agent.GetComponent<TorsoAnimator> ().RetAdv [0] = CalculateMotionParameter (23);
	
			agent.GetComponent<TorsoAnimator> ().EncSpr [1] = CalculateMotionParameter (24);
			agent.GetComponent<TorsoAnimator> ().SinRis [1] = CalculateMotionParameter (25);
			agent.GetComponent<TorsoAnimator> ().RetAdv [1] = CalculateMotionParameter (26);

			// Left arm
			agent.GetComponent<ArmAnimator> ().Hor = CalculateMotionParameter (27);
			agent.GetComponent<ArmAnimator> ().Ver = CalculateMotionParameter (28);
			agent.GetComponent<ArmAnimator> ().Sag = CalculateMotionParameter (29);
			agent.GetComponent<ArmAnimator> ().UpdateKeypointsByShape (0); //Update keypoints
	
	
			//RightArm 
			//Only horizontal motion is the opposite for each arm
			agent.GetComponent<ArmAnimator> ().Hor = CalculateMotionParameter (30);
			agent.GetComponent<ArmAnimator> ().Ver = CalculateMotionParameter (31);
			agent.GetComponent<ArmAnimator> ().Sag = CalculateMotionParameter (32);
			agent.GetComponent<ArmAnimator> ().UpdateKeypointsByShape (1); //Update keypoints
	
	
			//Update shape parameters
			for (int j = 0; j < _shapeParams.Length; j++)
					for (int i = 0; i < _shapeParams[j].Length; i++)
							agent.GetComponent<TorsoAnimator> ().ShapeParams [j] [i] = _shapeParams [j] [i];
	
			agent.GetComponent<TorsoAnimator> ().UpdateAnglesLinearComb ();
	}

	
	/*
	void UpdateEmoteParams(GameObject agent) {
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
				//Debug.Log(tokens[effortInd]);
			}
		}
		sr.Close ();
	}

	void ReadEffortMotionCoeffs(){
		string fileName = "effortToMotion.txt";
		StreamReader sr = new StreamReader (fileName);
		string[] content = File.ReadAllLines (fileName);

		for (int motionInd = 1; motionInd < content.Length; motionInd++) {
			// First line of the file is labels
			string line = content[motionInd];
			//Debug.Log(line);
			// Each line contains coefficients for a motion parameter
			String[] tokens = line.Split('\t');
			for (int effortInd = 1; effortInd<tokens.Length; effortInd++){
				//Debug.Log(tokens[effortInd].ToString());
				_em_coeffs[motionInd-1, effortInd-1] = float.Parse(tokens[effortInd]);
			}
		}

		sr.Close ();
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

