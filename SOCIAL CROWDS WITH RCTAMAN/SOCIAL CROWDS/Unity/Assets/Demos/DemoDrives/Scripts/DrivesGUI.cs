#define WEBMODE


using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;

public enum DType { //drive parameter indices
    Speed,
    V0,
    V1,
    Ti,
    TExp,
    TVal,
    Continuity,
    Bias,
    T0,
    T1,
    Tr,
    Tf,
    Hr,
    Hf,
    Squash,
    Wb,
    Wx,
    Wt,
    Wf,
    Et,
    D,
    Ef,
    EncSpr0,
    EncSpr1,
    SinRis0,
    SinRis1,
    RetAdv0,
    RetAdv1,
    ArmLX,
    ArmLY,
    ArmLZ,
    ArmRX,
    ArmRY,
    ArmRZ
}



public class DrivesGUI : MonoBehaviour {

    private static bool _driveMode = true;
    private static bool _toggleContinuous = false;
    private static bool _toggleDrawCurves = false;


    private static bool[] _checkDrives = new  bool[34];
    private static bool[] _checkShape = new bool[18];

    //Timing
    private static float _speed = 2f;    
    private static float _v0 = 0f;
    private static float _v1 = 0f;
    private static float _ti = 0.5f;
    private static float _texp = 1.0f;
    private static float _tval = 0f;
    private static float _continuity = 0f;
    private static float _bias = 0f;
    private static float _t0 = 0.01f;
    private static float _t1 = 0.99f;
    
    //Flourishes
    private static float _trMag = 0f; //torso rotation
    private static float _tfMag = 0f;
    
    private static float _hrMag = 0f; //head rotation
    private static float _hfMag = 0f;
    private static float _squashMag = 0f;
    private static float _wbMag = 0f;
    private static float _wxMag = 0f;
    private static float _wtMag = 0f;
    private static float _wfMag = 0f;
    private static float _etMag = 0f;
    private static float _dMag = 0f;
    private static float _efMag = 0f;

    //Shape for drives
    private static float _encSpr0 = 0f;
    private static float _sinRis0 = 0f;
    private static float _retAdv0 = 0f;

    private static float _encSpr1 = 0f;
    private static float _sinRis1 = 0f;
    private static float _retAdv1 = 0f;

    //Arm shape for drives
    private static Vector3[] _arm = new Vector3[2];
    

    //Timing range
    private float _speedMin = 0.2f;
    private float _speedMax = 7.0f;
    private float _vMin = 0f;
    private float _vMax = 1f;
    private float _tMin = 0f;
    private float _tMax = 1f;
    private float _texpMin = 0.0f;
    private float _texpMax = 5.0f;
    private float _tvalMin = -1f;
    private float _tvalMax = 1f;
    private float _continuityMin = -1f;
    private float _continuityMax = 1f;
    private float _biasMin = -1f;
    private float _biasMax = 1f;
    
    //Flourishes range
    private static float _trMin = 0f;
    private static float _trMax = 1f;
    private static float _tfMin = 0f;
    private static float _tfMax = 4f;

    private static float _hrMin = 0f;
    private static float _hrMax = 1f; //rotates max 57 degrees
    private static float _hfMin = 0f;
    private static float _hfMax = 4f;
   private float _squashMin = 0f;
    private float _squashMax = 1f;
    private float _wbMin = -0.5f;
    private float _wbMax = 1f;
    private float _wxMin = -1.2f;
    private float _wxMax = 1.3f;
    private float _wtMin = 0f;
    private float _wtMax = 1.4f;
    private float _wfMin = 0f;
    private float _wfMax = 2f;
    private float _etMin = 0f;
    private float _etMax = 1.4f;
    private float _efMin = 0f;
    private float _efMax = 4f;
    private float _dMin = 0f;
    private float _dMax = 1.4f;

    //Shape for drives range
    private float _shapeMin = -1f;
    private float _shapeMax = 1f;

    
    //Shape
    private static Vector3 _pelvisL = Vector3.zero;
    private static Vector3 _pelvisR = Vector3.zero;
    private static Vector3 _neck = Vector3.zero;
    private static Vector3 _head = Vector3.zero;
    private static Vector3 _shoulders = Vector3.zero;
    private static Vector3 _clavicles = Vector3.zero;
    private static Vector3 _spine = Vector3.zero;
    private static Vector3 _spine1 = Vector3.zero;
    private static Vector3 _knees = Vector3.zero;
    private static Vector3 _hips = Vector3.zero;
    private static Vector3 _toes = Vector3.zero;
    private float _spineLength = 0;

    //Shape rotation range
    private static float _rotMin = -100f;
    private static float _rotMax = 100f;

    //Spine length range
    private float _spineLengthMin = -0.05f;
    private float _spineLengthMax = 0.05f;
    
    
    

    private bool[] _armShapeChanged = new bool [2];
    private bool _isPlaying;

    private static int _driveInd = 0 ;
    private static int _shapeInd = 0; 


    private int[,] _effortList = {{-1, -1, -1, 0}, {-1, -1, 1, 0}, {-1, 1, -1, 0}, {-1, 1, 1, 0}, {1, -1, -1, 0}, {1, -1, 1, 0}, {1, 1, -1, 0}, {1, 1, 1, 0}, 
                                        {-1, -1, 0, -1}, {-1, -1, 0, 1}, {-1, 1,0,  -1}, {-1, 1, 0, 1}, {1, -1, 0, -1}, {1, -1, 0,  1}, {1, 1, 0, -1}, {1, 1, 0,  1},
                                        {-1,  0, -1, -1}, {-1, 0, -1,  1}, {-1, 0, 1,  -1}, {-1, 0, 1, 1}, {1, 0, -1, -1}, {1, 0, -1,  1}, {1, 0, 1, -1}, {1,  0, 1, 1},
                                        { 0, -1, -1, -1}, {0, -1, -1,  1}, {0, -1, 1,  -1}, {0, -1, 1, 1}, {0, 1, -1, -1}, {0, 1, -1,  1}, {0, 1, 1, -1}, {0, 1, 1, 1}};

    private string[] _shapeStr = { "Enclosing", "Spreading", "Sinking", "Rising", "Retreating", "Advancing" };
    

    public string Info = "waiting.. waiting";
    public string ShapeInfo = "";
    private static Vector2 _scrollPosition;

    private static string _questionNo = "";
    private static string _questionStr = "";
    
    private static string[] _isSubmittedStrDrive = new string[32];
    private static string[] _isSubmittedStrShape = new string[6];

    //UI related
    private float _scrollWidth = 320f;

    private bool[] _animType;
    private string[] _animName = {"Pointing_to_Spot_Netural_02_Updated", 
                                     "Picking_Up_Pillow_Netural_01", 
                                     "Knocking_Neutral_1", 
                                     "Lifting_Netural_01",                                      
                                     "Punching_Netural_02",                                     
                                     "Pushing_Netural_02",
                                     "Walking_Netural_02", 
                                     "Waving_Netural_02", 
                                     "Throwing_Netural_02"};
    private int _animIndex = 0;

  
    void Start(){


        for (int i = 0; i < _isSubmittedStrDrive.Length; i++)
            _isSubmittedStrDrive[i] = "Answer NOT submitted";

        for (int i = 0; i < _isSubmittedStrShape.Length; i++)
            _isSubmittedStrShape[i] = "Answer NOT submitted";

        for (int i = 0; i < _checkDrives.Length; i++)
            _checkDrives[i] = false;

        for (int i = 0; i < _checkShape.Length; i++)
            _checkShape[i] = false;

        for (int i = 5; i >= 0; i--)
            UpdateShapeParameters(i); //to set enclosing as the first loaded item

        UpdateDriveParameters();
        _arm[0] = _arm[1] = Vector3.zero;
        //Assign cameras

        UpdateCameraBoundaries(_driveMode && _toggleDrawCurves);


        _animType = new bool[9];

    }


    void UpdateShapeParameters(int i) {
#if !WEBMODE

        ReadValuesShapes(i);
#elif WEBMODE
                    this.StartCoroutine(GetValuesShapes(i));
#endif
    }


    void UpdateDriveParameters() {


#if !WEBMODE

        ReadValuesDrives(_driveInd);
#elif WEBMODE
                    this.StartCoroutine(GetValuesDrives());
#endif

    }

    void UpdateCameraBoundaries(bool mode) {
        
        
        GameObject cam1 = GameObject.Find("Camera1");
        GameObject cam2 = GameObject.Find("Camera2");
        GameObject cam3 = GameObject.Find("Camera3");
        GameObject cam4 = GameObject.Find("Camera4");

        if (mode == true ) {
            
            cam1.camera.rect = new Rect(0, 0, _scrollWidth / Screen.width, 1); //320 is the width of the parameters
            cam2.camera.rect = new Rect(_scrollWidth / Screen.width, 0, ((Screen.width - _scrollWidth) / 2f) / Screen.width, 0.6f);
            cam3.camera.rect = new Rect((Screen.width - (Screen.width - _scrollWidth) / 2f) / Screen.width, 0, ((Screen.width - _scrollWidth) / 2f) / Screen.width, 0.6f);
            cam4.camera.rect = new Rect(_scrollWidth / Screen.width, 0.6f, (Screen.width - _scrollWidth) / Screen.width, 0.4f);
        }
        else {
            
            cam1.camera.rect = new Rect(0, 0, _scrollWidth / Screen.width, 1); //320 is the width of the parameters
            cam2.camera.rect = new Rect(_scrollWidth / Screen.width, 0, ((Screen.width - _scrollWidth) / 2f) / Screen.width, 1);
            cam3.camera.rect = new Rect((Screen.width - (Screen.width - _scrollWidth) / 2f) / Screen.width, 0, ((Screen.width - _scrollWidth) / 2f) / Screen.width, 1);
            cam4.camera.rect = new Rect(0, 0, 0, 0);
        }
         
    }

    void Update() {


        GameObject agent = GameObject.Find("AgentPrefab");
        GameObject agentControl = GameObject.Find("AgentControlPrefab");
        UpdateCameraBoundaries(_driveMode && _toggleDrawCurves);

        if (_driveMode) {
            if (Input.GetKeyDown("up")) {
                _animIndex++;
                if (_animIndex > _animName.Length - 1)
                    _animIndex = _animName.Length - 1;

                agent.GetComponent<TorsoController>().AssignInitRootandFootPos();
                PlayAnim(_animName[_animIndex]);
                agent.animation.Stop(); //in order to restart animation
                agentControl.animation.Stop(); //in order to restart animation
                UpdateEmoteParams();
            }
            else if (Input.GetKeyDown("down")) {
                _animIndex--;
                if (_animIndex < 0)
                    _animIndex = 0;

                agent.GetComponent<TorsoController>().AssignInitRootandFootPos();
                PlayAnim(_animName[_animIndex]);
                agent.animation.Stop(); //in order to restart animation
                agentControl.animation.Stop(); //in order to restart animation
                UpdateEmoteParams();
            }
        }

        if (Input.GetKeyDown("left")) {
            if (_driveMode) {
                _driveInd--;
                if (_driveInd < 0)
                    _driveInd = 0;
                //ResetDriveParameters();                
                UpdateDriveParameters();
            }
            else {
                _shapeInd--;
                if (_shapeInd < 0)
                    _shapeInd = 0;                
                UpdateShapeParameters(_shapeInd);
                //ResetShapeParameters();
               // this.StartCoroutine(GetValuesShapes(_shapeInd));
            }
        }
        
        else if (Input.GetKeyDown("right")) {
            if (_driveMode) {
                _driveInd++;
                if (_driveInd >= 31)
                    _driveInd = 31;
                //ResetDriveParameters();                
                UpdateDriveParameters();
            }
            else {
                _shapeInd++;
                if (_shapeInd >= 5)
                    _shapeInd = 5;                
                UpdateShapeParameters(_shapeInd);
              //  this.StartCoroutine(GetValuesShapes(_shapeInd));
            //    ResetShapeParameters();
            }
        }


        if (!_driveMode) {//shapemode
            //headx
            if (_checkShape[(int)BPart.HeadX]) {
                if (Input.GetKey("a")) {
                    _head.x -= 1f;
                    if (_head.x < _rotMin)
                        _head.x = _rotMin;
                }
                else if (Input.GetKey("s")) {
                    _head.x += 1f;
                    if (_head.x > _rotMax)
                        _head.x = _rotMax;
                }
            }

            //neckx
            if (_checkShape[(int)BPart.NeckX]) {
                if (Input.GetKey("a")) {
                    _neck.x -= 1f;
                    if (_neck.x < _rotMin)
                        _neck.x = _rotMin;
                }
                else if (Input.GetKey("s")) {
                    _neck.x += 1f;
                    if (_neck.x > _rotMax)
                        _neck.x = _rotMax;
                }
            }

            //spiney
            if (_checkShape[(int)BPart.SpineY]) {
                if (Input.GetKey("a")) {
                    _spine.y -= 1f;
                    if (_spine.y < _rotMin)
                        _spine.y = _rotMin;
                }
                else if (Input.GetKey("s")) {
                    _spine.y += 1f;
                    if (_spine.y > _rotMax)
                        _spine.y = _rotMax;
                }
            }
            //spine1x
            if (_checkShape[(int)BPart.Spine1X]) {
                if (Input.GetKey("a")) {
                    _spine1.x -= 1f;
                    if (_spine1.x < _rotMin)
                        _spine1.x = _rotMin;
                }
                else if (Input.GetKey("s")) {
                    _spine1.x += 1f;
                    if (_spine1.x > _rotMax)
                        _spine1.x = _rotMax;
                }
            }
            //shouldersx
            if (_checkShape[(int)BPart.ShouldersX]) {
                if (Input.GetKey("a")) {
                    _shoulders.x -= 1f;
                    if (_shoulders.x < _rotMin)
                        _shoulders.x = _rotMin;
                }
                else if (Input.GetKey("s")) {
                    _shoulders.x += 1f;
                    if (_shoulders.x > _rotMax)
                        _shoulders.x = _rotMax;
                }
            }
            //shouldersy
            if (_checkShape[(int)BPart.ShouldersY]) {
                if (Input.GetKey("a")) {
                    _shoulders.y -= 1f;
                    if (_shoulders.y < _rotMin)
                        _shoulders.y = _rotMin;
                }
                else if (Input.GetKey("s")) {
                    _shoulders.y += 1f;
                    if (_shoulders.y > _rotMax)
                        _shoulders.y = _rotMax;
                }
            }
            //shouldersz
            if (_checkShape[(int)BPart.ShouldersZ]) {
                if (Input.GetKey("a")) {
                    _shoulders.z -= 1f;
                    if (_shoulders.z < _rotMin)
                        _shoulders.z = _rotMin;
                }
                else if (Input.GetKey("s")) {
                    _shoulders.z += 1f;
                    if (_shoulders.z > _rotMax)
                        _shoulders.z = _rotMax;
                }
            }
            //claviclesx
            if (_checkShape[(int)BPart.ClaviclesX]) {
                if (Input.GetKey("a")) {
                    _clavicles.x -= 1f;
                    if (_clavicles.x  < _rotMin)
                        _clavicles.x = _rotMin;
                }
                else if (Input.GetKey("s")) {
                    _clavicles.x += 1f;
                    if (_clavicles.x > _rotMax)
                        _clavicles.x = _rotMax;
                }
            }
            //claviclesy
            if (_checkShape[(int)BPart.ClaviclesY]) {
                if (Input.GetKey("a")) {
                    _clavicles.y -= 1f;
                    if (_clavicles.y < _rotMin)
                        _clavicles.y = _rotMin;
                }
                else if (Input.GetKey("s")) {
                    _clavicles.y += 1f;
                    if (_clavicles.y > _rotMax)
                        _clavicles.y = _rotMax;
                }
            }
            //claviclesz
            if (_checkShape[(int)BPart.ClaviclesZ]) {
                if (Input.GetKey("a")) {
                    _clavicles.z -= 1f;
                    if (_clavicles.z < _rotMin)
                        _clavicles.z = _rotMin;
                }
                else if (Input.GetKey("s")) {
                    _clavicles.z += 1f;
                    if (_clavicles.z > _rotMax)
                        _clavicles.z = _rotMax;
                }
            }
            //pelvis L x
            if (_checkShape[(int)BPart.PelvisLX]) {
                if (Input.GetKey("a")) {
                    _pelvisL.x -= 1f;
                    if (_pelvisL.x < _rotMin)
                        _pelvisL.x = _rotMin;
                }
                else if (Input.GetKey("s")) {
                    _pelvisL.x += 1f;
                    if (_pelvisL.x > _rotMax)
                        _pelvisL.x = _rotMax;
                }
            }
            //pelvis Rx
            if (_checkShape[(int)BPart.PelvisRX]) {
                if (Input.GetKey("a")) {
                    _pelvisR.x -= 1f;
                    if (_pelvisR.x < _rotMin)
                        _pelvisR.x = _rotMin;
                }
                else if (Input.GetKey("s")) {
                    _pelvisR.x += 1f;
                    if (_pelvisR.x > _rotMax)
                        _pelvisR.x = _rotMax;
                }
            }
            //pelvisy
            if (_checkShape[(int)BPart.PelvisY]) {
                if (Input.GetKey("a")) {
                    _pelvisL.y -= 1f;
                    if (_pelvisL.y < _rotMin)
                        _pelvisL.y = _pelvisR.y = _rotMin;
                }
                else if (Input.GetKey("s")) {
                    _pelvisL.y += 1f;
                    if (_pelvisL.y > _rotMax)
                        _pelvisL.y = _pelvisR.y = _rotMax;
                }
            }
            //pelvisz
            if (_checkShape[(int)BPart.PelvisZ]) {
                if (Input.GetKey("a")) {
                    _pelvisL.z -= 1f;
                    if (_pelvisL.z < _rotMin)
                        _pelvisL.z = _pelvisR.z = _rotMin;
                }
                else if (Input.GetKey("s")) {
                    _pelvisL.z += 1f;
                    if (_pelvisL.z > _rotMax)
                        _pelvisL.z = _pelvisR.z = _rotMax;
                }
            }

            //KneesX
            if (_checkShape[(int)BPart.KneesX]) {
                if (Input.GetKey("a")) {
                    _knees.x -= 1f;
                    if (_knees.x < _rotMin)
                        _knees.x = _rotMin;
                }
                else if (Input.GetKey("s")) {
                    _knees.x += 1f;
                    if (_knees.x > _rotMax)
                        _knees.x = _rotMax;
                }
            }

            //hipsX
            if (_checkShape[(int)BPart.HipsX]) {
                if (Input.GetKey("a")) {
                    _hips.x -= 1f;
                    if (_hips.x < _rotMin)
                        _hips.x = _rotMin;
                }
                else if (Input.GetKey("s")) {
                    _hips.x += 1f;
                    if (_hips.x > _rotMax)
                        _hips.x = _rotMax;
                }
            }

            //toesZ
            if (_checkShape[(int)BPart.ToesX]) {
                if (Input.GetKey("a")) {
                    _toes.x -= 1f;
                    if (_toes.x < _rotMin)
                        _toes.x = _rotMin;
                }
                else if (Input.GetKey("s")) {
                    _toes.x += 1f;
                    if (_toes.x > _rotMax)
                        _toes.x = _rotMax;
                }
            }

            //spineLength
            if (_checkShape[(int)BPart.SpineLength]) {
                if (Input.GetKey("a")) {
                    _spineLength -= 0.01f;
                    if (_spineLength < _spineLengthMin)
                        _spineLength = _spineLengthMin;
                }
                else if (Input.GetKey("s")) {
                    _spineLength += 0.01f;
                    if (_spineLength > _spineLengthMax)
                        _spineLength = _spineLengthMax;
                }
            }
        }
        
        if (_driveMode) {

            //speed
            if(_checkDrives[(int)DType.Speed]) {
                if (Input.GetKey("a")) {                 
                    _speed -= 0.01f;
                    if (_speed < _speedMin)
                        _speed = _speedMin;
                }
                else if (Input.GetKey("s")) {                 
                    _speed += 0.01f;
                    if (_speed > _speedMax)
                        _speed = _speedMax;
                }
            }
                
            //v0
            else if (_checkDrives[(int)DType.V0]) {
                if (Input.GetKey("a")) {                 
                    _v0 -= 0.01f;
                    if (_v0 < _vMin)
                        _v0 = _vMin;
                }
                else if (Input.GetKey("s")) {
                    _v0 += 0.01f;
                    if (_v0 > _vMax)
                        _v0 = _vMax;
                }

            }

            //v1
            else if (_checkDrives[(int)DType.V1]) {
                if (Input.GetKey("a")) {
                    _v1 -= 0.01f;
                    if (_v1 < _vMin)
                        _v1 = _vMin;
                }
                else if (Input.GetKey("s")) {
                    _v1 += 0.01f;
                    if (_v1 > _vMax)
                        _v1 = _vMax;
                }
            }
            
            //ti
            else if (_checkDrives[(int)DType.Ti]) {
                if (Input.GetKey("a")) {
                    _ti -= 0.01f;
                    if (_ti < _tMin)
                        _ti = _tMin;
                }
                else if (Input.GetKey("s")) {
                    _ti += 0.01f;
                    if (_ti > _tMax)
                        _ti = _tMax;
                }
            }

            //texp
            else if (_checkDrives[(int)DType.TExp]) {
                if (Input.GetKey("a")) {
                    _texp -= 0.01f;
                    if (_texp < _texpMin)
                        _texp = _texpMin;
                }
                else if (Input.GetKey("s")) {
                    _texp += 0.01f;
                    if (_texp > _texpMax)
                        _texp = _texpMax;
                }
            }

            //tval
            else if (_checkDrives[(int)DType.TVal]) {
                if (Input.GetKey("a")) {
                    _tval -= 0.01f;
                    if (_tval < _tvalMin)
                        _tval = _tvalMin;

                    
                }
                else if (Input.GetKey("s")) {
                    _tval += 0.01f;
                    if (_tval > _tvalMax)
                        _tval = _tvalMax;
                }
            }

            //t0
            else if (_checkDrives[(int)DType.T0]) {
                if (Input.GetKey("a")) {
                    _t0 -= 0.01f;
                    if (_t0 < _tMin)
                        _t0 = _tMin;
                }
                else if (Input.GetKey("s")) {
                    _t0 += 0.01f;
                    if (_t0 > _tMax)
                        _t0 = _tMax;
                }
            }
                            
            //t1
            else if (_checkDrives[(int)DType.T1]) {
                if (Input.GetKey("a")) {
                    _t1 -= 0.01f;
                    if (_t1 < _tMin)
                        _t1 = _tMin;
                }
                else if (Input.GetKey("s")) {
                    _t1 += 0.01f;
                    if (_t1 > _tMax)
                        _t1 = _tMax;
                }
            }


            //tr
            else if (_checkDrives[(int)DType.Tr]) {
                if (Input.GetKey("a")) {
                    _trMag -= 0.01f;
                    if (_trMag < _trMin)
                        _trMag = _trMin;
                }
                else if (Input.GetKey("s")) {
                    _trMag += 0.01f;
                    if (_trMag > _trMax)
                        _trMag = _trMax;
                }
            }
            //tf
            else if (_checkDrives[(int)DType.Tf]) {
                if (Input.GetKey("a")) {
                    _tfMag -= 0.01f;
                    if (_tfMag < _tfMin)
                        _tfMag = _tfMin;
                }
                else if (Input.GetKey("s")) {
                    _tfMag += 0.01f;
                    if (_tfMag > _tfMax)
                        _tfMag = _tfMax;
                }
            }

            //hr
            else if (_checkDrives[(int)DType.Hr]) {
                if (Input.GetKey("a")) {
                    _hrMag -= 0.01f;
                    if (_hrMag < _hrMin)
                        _hrMag = _hrMin;
                }
                else if (Input.GetKey("s")) {
                    _hrMag += 0.01f;
                    if (_hrMag > _hrMax)
                        _hrMag = _hrMax;
                }
            }
            //hf
            else if (_checkDrives[(int)DType.Hf]) {
                if (Input.GetKey("a")) {
                    _hfMag -= 0.01f;
                    if (_hfMag < _hfMin)
                        _hfMag = _hfMin;
                }
                else if (Input.GetKey("s")) {
                    _hfMag += 0.01f;
                    if (_hfMag > _hfMax)
                        _hfMag = _hfMax;
                }
            }

            //squash
            else if (_checkDrives[(int)DType.Squash]) {
                if (Input.GetKey("a")) {
                    _squashMag -= 0.01f;
                    if (_squashMag < _squashMin)
                        _squashMag = _squashMin;
                }
                else if (Input.GetKey("s")) {
                    _squashMag += 0.01f;
                    if (_squashMag > _squashMax)
                        _squashMag = _squashMax;
                }
            }
            
            //wb
            else if (_checkDrives[(int)DType.Wb]) {
                if (Input.GetKey("a")) {
                    _wbMag -= 0.01f;
                    if (_wbMag < _wbMin)
                        _wbMag = _wbMin;
                }
                else if (Input.GetKey("s")) {
                    _wbMag += 0.01f;
                    if (_wbMag > _wbMax)
                        _wbMag = _wbMax;
                }
            }

            //wx
            else if (_checkDrives[(int)DType.Wx]) {
                if (Input.GetKey("a")) {
                    _wxMag -= 0.01f;
                    if (_wxMag < _wxMin)
                        _wxMag = _wxMin;
                }
                else if (Input.GetKey("s")) {
                    _wxMag += 0.01f;
                    if (_wxMag > _wxMax)
                        _wxMag = _wxMax;
                }
            }

            //wt
            else if (_checkDrives[(int)DType.Wt]) {
                if (Input.GetKey("a")) {
                    _wtMag -= 0.01f;
                    if (_wtMag < _wtMin)
                        _wtMag = _wtMin;
                }
                else if (Input.GetKey("s")) {
                    _wtMag += 0.01f;
                    if (_wtMag > _wtMax)
                        _wtMag = _wtMax;
                }
            }

            //wf
            else if (_checkDrives[(int)DType.Wf]) {
                if (Input.GetKey("a")) {
                    _wfMag -= 0.01f;
                    if (_wfMag < _wfMin)
                        _wfMag = _wfMin;
                }
                else if (Input.GetKey("s")) {
                    _wfMag += 0.01f;
                    if (_wfMag > _wfMax)
                        _wfMag = _wfMax;
                }
            }

            //et
            else if (_checkDrives[(int)DType.Et]) {
                if (Input.GetKey("a")) {
                    _etMag -= 0.01f;
                    if (_etMag < _etMin)
                        _etMag = _etMin;
                }
                else if (Input.GetKey("s")) {
                    _etMag += 0.01f;
                    if (_etMag > _etMax)
                        _etMag = _etMax;
                }
            }
            //d
            else if (_checkDrives[(int)DType.D]) {
                if (Input.GetKey("a")) {
                    _dMag -= 0.01f;
                    if (_dMag < _dMin)
                        _dMag = _dMin;
                }
                else if (Input.GetKey("s")) {
                    _dMag += 0.01f;
                    if (_dMag > _dMax)
                        _dMag = _dMax;
                }
            }
            //ef
            else if (_checkDrives[(int)DType.Ef]) {
                if (Input.GetKey("a")) {
                    _efMag -= 0.01f;
                    if (_efMag < _efMin)
                        _efMag = _efMin;
                }
                else if (Input.GetKey("s")) {
                    _efMag += 0.01f;
                    if (_efMag > _efMax)
                        _efMag = _efMax;
                }
            }

            //encSpr0
            else if (_checkDrives[(int)DType.EncSpr0]) {
                if (Input.GetKey("a")) {
                    _encSpr0 -= 0.01f;
                    if (_encSpr0 < _shapeMin)
                        _encSpr0 = _shapeMin;
                }
                else if (Input.GetKey("s")) {
                    _encSpr0 += 0.01f;
                    if (_encSpr0 > _shapeMax)
                        _encSpr0 = _shapeMax;
                }            
            }
            //encSpr1
            else if (_checkDrives[(int)DType.EncSpr1]) {
                if (Input.GetKey("a")) {
                    _encSpr1 -= 0.01f;
                    if (_encSpr1 < _shapeMin)
                        _encSpr1 = _shapeMin;
                }
                else if (Input.GetKey("s")) {
                    _encSpr1 += 0.01f;
                    if (_encSpr1 > _shapeMax)
                        _encSpr1 = _shapeMax;
                }
            }

            //sinRis0
            else if (_checkDrives[(int)DType.SinRis0]) {
                if (Input.GetKey("a")) {
                    _sinRis0 -= 0.01f;
                    if (_sinRis0 < _shapeMin)
                        _sinRis0 = _shapeMin;
                }
                else if (Input.GetKey("s")) {
                    _sinRis0 += 0.01f;
                    if (_sinRis0 > _shapeMax)
                        _sinRis0 = _shapeMax;
                }
            }
            //sinRis1
            else if (_checkDrives[(int)DType.SinRis1]) {
                if (Input.GetKey("a")) {
                    _sinRis1 -= 0.01f;
                    if (_sinRis1 < _shapeMin)
                        _sinRis1 = _shapeMin;
                }
                else if (Input.GetKey("s")) {
                    _sinRis1 += 0.01f;
                    if (_sinRis1 > _shapeMax)
                        _sinRis1 = _shapeMax;
                }
            }
            //retAdv0
            else if (_checkDrives[(int)DType.RetAdv0]) {
                if (Input.GetKey("a")) {
                    _retAdv0 -= 0.01f;
                    if (_retAdv0 < _shapeMin)
                        _retAdv0 = _shapeMin;
                }
                else if (Input.GetKey("s")) {
                    _retAdv0 += 0.01f;
                    if (_retAdv0 > _shapeMax)
                        _retAdv0 = _shapeMax;
                }
            }
            //retAdv1
            else if (_checkDrives[(int)DType.RetAdv1]) {
                if (Input.GetKey("a")) {
                    _retAdv1 -= 0.01f;
                    if (_retAdv1 < _shapeMin)
                        _retAdv1 = _shapeMin;
                }
                else if (Input.GetKey("s")) {
                    _retAdv1 += 0.01f;
                    if (_retAdv1 > _shapeMax)
                        _retAdv1 = _shapeMax;
                }
            }

            //ArmLX                  
            else if (_checkDrives[(int)DType.ArmLX]) {
                if (Input.GetKey("a")) {
                    _arm[0].x -= 0.01f;
                    if (_arm[0].x < _shapeMin)
                        _arm[0].x = _shapeMin;
                }
                else if (Input.GetKey("s")) {
                    _arm[0].x += 0.01f;
                    if (_arm[0].x > _shapeMax)
                        _arm[0].x = _shapeMax;
                }
            }
            //ArmLY                  
            else if (_checkDrives[(int)DType.ArmLY]) {
                if (Input.GetKey("a")) {
                    _arm[0].y -= 0.01f;
                    if (_arm[0].y < _shapeMin)
                        _arm[0].y = _shapeMin;
                }
                else if (Input.GetKey("s")) {
                    _arm[0].y += 0.01f;
                    if (_arm[0].y > _shapeMax)
                        _arm[0].y = _shapeMax;
                }
            }
            //ArmLZ                  
            else if (_checkDrives[(int)DType.ArmLZ]) {
                if (Input.GetKey("a")) {
                    _arm[0].z -= 0.01f;
                    if (_arm[0].z < _shapeMin)
                        _arm[0].z = _shapeMin;
                }
                else if (Input.GetKey("s")) {
                    _arm[0].z += 0.01f;
                    if (_arm[0].z > _shapeMax)
                        _arm[0].z = _shapeMax;
                }
            }

            //ArmRX                  
            else if (_checkDrives[(int)DType.ArmRX]) {
                if (Input.GetKey("a")) {
                    _arm[1].x -= 0.01f;
                    if (_arm[1].x < _shapeMin)
                        _arm[1].x = _shapeMin;
                }
                else if (Input.GetKey("s")) {
                    _arm[1].x += 0.01f;
                    if (_arm[1].x > _shapeMax)
                        _arm[1].x = _shapeMax;
                }
            }
            //ArmRY                  
            else if (_checkDrives[(int)DType.ArmRY]) {
                if (Input.GetKey("a")) {
                    _arm[1].y -= 0.01f;
                    if (_arm[1].y < _shapeMin)
                        _arm[1].y = _shapeMin;
                }
                else if (Input.GetKey("s")) {
                    _arm[1].y += 0.01f;
                    if (_arm[1].y > _shapeMax)
                        _arm[1].y = _shapeMax;
                }
            }
            //ArmRZ                  
            else if (_checkDrives[(int)DType.ArmRZ]) {
                if (Input.GetKey("a")) {
                    _arm[1].z -= 0.01f;
                    if (_arm[1].z < _shapeMin)
                        _arm[1].z = _shapeMin;
                }
                else if (Input.GetKey("s")) {
                    _arm[1].z += 0.01f;
                    if (_arm[1].z > _shapeMax)
                        _arm[1].z = _shapeMax;
                }
            }
        }

        
    }

    void UncheckOthers(int ind) {
        if (_driveMode) {
            for (int i = 0; i < _checkDrives.Length; i++) {
                if (i != ind)
                    _checkDrives[i] = false;
            }
        }
        else {
            for (int i = 0; i < _checkShape.Length; i++) {
                if (i != ind)
                    _checkShape[i] = false;
            }
        }
    }

	void OnGUI () {


        GameObject agent = GameObject.Find("AgentPrefab");
        GameObject agentControl = GameObject.Find("AgentControlPrefab");
        TorsoController torso = agent.GetComponent<TorsoController>();
        TorsoController torsoControl = agentControl.GetComponent<TorsoController>();
        
        GUIStyle style = new GUIStyle();

        GUI.color = Color.black;

        
        GUILayout.BeginArea (new Rect (320,10,300,250));
        
        style.fontSize = 18;
        style.normal.textColor = Color.black;
        GUILayout.Label(_animName[_animIndex]);
        
        bool newDriveMode = GUILayout.Toggle(_driveMode, "Switch mode");
        if (newDriveMode == true && _driveMode == false) { //switch to drive mode
            //ResetDriveParameters();
            this.StartCoroutine(GetValuesDrives());
            //InitAgent(agent, "Pointing_to_Spot_Netural_02");
            InitAgent(agent, _animName[_animIndex]);
            InitAgent(agentControl, _animName[_animIndex]);
            agentControl.GetComponent<ArmAnimator>().ResetParameters();

            //Assign torsoAnimator shapeParams


            for (int i = 5; i >= 0; i--) 
                UpdateShapeParameters(i);
            
            StopAnim();            
        }
        else if (newDriveMode == false && _driveMode == true) {
            ResetShapeParameters();
            StopAnim();
            torso.Reset();
            torsoControl.Reset();
            this.StartCoroutine(GetValuesShapes(_shapeInd));
        }
        
        _driveMode = newDriveMode;

        if (_driveMode) {
            GUILayout.Label("Mode: Drives", style);
            _toggleDrawCurves = GUILayout.Toggle(_toggleDrawCurves, "Draw velocity curves");
        }
        else {
            GUILayout.Label("Mode: Shapes", style);
        }

        //GUILayout.Label(ShapeInfo);
        //GUILayout.Label(Info);
        
        GUILayout.Label(_questionNo, style);
    //    GUILayout.Label(_questionStr, style); //Effort combination
        /*
        for (int i = 0; i < 6; i++) {
            GUILayout.BeginHorizontal();
            for (int j = 0; j < 13; j++)
                GUILayout.Label(torsoAnim.ShapeParams[j][i] + "");
            GUILayout.EndHorizontal();
        }
        */

        GUILayout.EndArea();


        GUILayout.BeginArea (new Rect (left: Screen.width/2f, top: Screen.height-150, width: 300, height: 200));
        GUILayout.Space (10);
        GUI.color = Color.black;

        if (_driveMode) {
            _toggleContinuous = GUILayout.Toggle(_toggleContinuous, "Animation looping");

            GUI.color = Color.white;
            _armShapeChanged[0] = _armShapeChanged[1] = false;
            if (GUILayout.Button("Play")) {
                agent.GetComponent<TorsoController>().AssignInitRootandFootPos();
                PlayAnim(_animName[_animIndex]);
                _armShapeChanged[0]  = _armShapeChanged[1]  = true;
                //reset to initial positions for torso and feet                
             //   agent.GetComponent<TorsoController>().ResetToInitRootPos();
            }
        }            
        GUILayout.Label("");
        GUILayout.BeginHorizontal ();	
        GUI.color = Color.white;        

        if(GUILayout.Button ( "Previous question")) {
            if (_driveMode) {
                _driveInd--;
                if (_driveInd < 0)
                    _driveInd = 0;
                //ResetDriveParameters();
                this.StartCoroutine(GetValuesDrives());
            }
            else {
                _shapeInd--;
                if (_shapeInd < 0)
                    _shapeInd = 0;
                //ResetShapeParameters();
                this.StartCoroutine(GetValuesShapes(_shapeInd));
            }

                
        }        
        GUI.color = Color.white;    
        if(GUILayout.Button ( "Next question")) {
            if (_driveMode) {
                _driveInd++;
                if (_driveInd >= 31)
                    _driveInd = 31;
                //ResetDriveParameters();
                UpdateDriveParameters();
            }
            else {
                _shapeInd++;
                if (_shapeInd >= 5)
                    _shapeInd = 5;
                //ResetShapeParameters();
                //this.StartCoroutine(GetValuesShapes(_shapeInd));
                UpdateShapeParameters(_shapeInd);
            }

        }
        GUILayout.EndHorizontal ();


        GUI.color = Color.white;  

        if(GUILayout.Button("Submit")){            

            if (_driveMode) {
                _isSubmittedStrDrive[_driveInd] = "Answer submitted";
                
                #if !WEBMODE
                       RecordValuesDrives();
                #elif WEBMODE
                    this.StartCoroutine(PostValuesDrives());
                #endif
            }
            else {
                _isSubmittedStrShape[_shapeInd] = "Answer submitted";
                
                #if !WEBMODE
                    RecordValuesShapes();
                #elif WEBMODE
                    this.StartCoroutine(PostValuesShapes());
                #endif
            }

        }


        if (_driveMode) 
            GUILayout.Label(_isSubmittedStrDrive[_driveInd], style);
        else
            GUILayout.Label(_isSubmittedStrShape[_shapeInd], style);
        GUILayout.EndArea();

        _scrollPosition = GUILayout.BeginScrollView(_scrollPosition, GUILayout.Width(_scrollWidth), GUILayout.Height(Screen.height * 0.98f));
            
        GUILayout.Space (10);
        style.fontSize = 11;
        style.normal.textColor = Color.black;            
        GUI.color = Color.grey;

        if (_driveMode) {
            GUI.color = Color.Lerp(Color.black, Color.grey, 0.5f);
            
            
            
            //speed
            _checkDrives[(int)DType.Speed] = GUILayout.Toggle(_checkDrives[(int)DType.Speed], "Animation speed");
            if (_checkDrives[(int)DType.Speed]) //if speed is checked
                UncheckOthers((int)DType.Speed);
            GUILayout.BeginHorizontal();
            
            GUILayout.Label("" + _speedMin);
            GUILayout.Label("" + _speed);
            GUILayout.Label("" + _speedMax);
            if (GUILayout.Button("Reset"))
                _speed = 2f;
            GUILayout.EndHorizontal();
            GUI.SetNextControlName("speed");
            _speed = GUILayout.HorizontalSlider(_speed, _speedMin, _speedMax);
            GUILayout.Label("");

            bool animChanged = GUI.changed;
           
            //V0            
            _checkDrives[(int)DType.V0] = GUILayout.Toggle(_checkDrives[(int)DType.V0], "Max. velocity for anticipation");
            if (_checkDrives[(int)DType.V0])
                UncheckOthers((int)DType.V0);     
            GUILayout.BeginHorizontal();            
            GUILayout.Label("" + _vMin);
            GUILayout.Label("" + _v0);
            GUILayout.Label("" + _vMax);
            if (GUILayout.Button("Reset"))
                _v0 = 0;
            GUILayout.EndHorizontal();
            _v0 = GUILayout.HorizontalSlider(_v0, _vMin, _vMax);
            GUILayout.Label("");


            //V1                                  
            _checkDrives[(int)DType.V1] = GUILayout.Toggle(_checkDrives[(int)DType.V1], "Max. velocity for overshoot");
            if (_checkDrives[(int)DType.V1])
                UncheckOthers((int)DType.V1);                       
            GUILayout.BeginHorizontal();
            GUILayout.Label("" + _vMin);
            GUILayout.Label("" + _v1);
            GUILayout.Label("" + _vMax);
            if (GUILayout.Button("Reset"))
                _v1 = 0;
            GUILayout.EndHorizontal();
            _v1 = GUILayout.HorizontalSlider(_v1, _vMin, _vMax);
            GUILayout.Label("");


            //ti
            _checkDrives[(int)DType.Ti] = GUILayout.Toggle(_checkDrives[(int)DType.Ti], "Inflection point\n where movement changes from accelerating to decelerating");
            if (_checkDrives[(int)DType.Ti])
                UncheckOthers((int)DType.Ti);        
            GUILayout.BeginHorizontal();
            GUILayout.Label("" + _tMin);
            GUILayout.Label("" + _ti);
            GUILayout.Label("" + _tMax);
            if (GUILayout.Button("Reset"))
                _ti = 0.5f;
            GUILayout.EndHorizontal();
            _ti = GUILayout.HorizontalSlider(_ti, _tMin, _tMax);
            GUILayout.Label("");


            

            //t0
            _checkDrives[(int)DType.T0] = GUILayout.Toggle(_checkDrives[(int)DType.T0], "t0 (Anticipation time)");
            if (_checkDrives[(int)DType.T0])
                UncheckOthers((int)DType.T0);                    
            GUILayout.BeginHorizontal();
            GUILayout.Label("" + _tMin);
            GUILayout.Label("" + _t0);
            GUILayout.Label("" + _ti); //t0max = ti
            if (GUILayout.Button("Reset"))
                _t0 = 0.01f;
            GUILayout.EndHorizontal();
            _t0 = GUILayout.HorizontalSlider(_t0, _tMin, _ti);
            GUILayout.Label("");

            //t1
            _checkDrives[(int)DType.T1] = GUILayout.Toggle(_checkDrives[(int)DType.T1], "t1 (Overshoot time)");
            if (_checkDrives[(int)DType.T1])
                UncheckOthers((int)DType.T1);   
            GUILayout.BeginHorizontal();
            GUILayout.Label("" + _ti); //t1min  = ti
            GUILayout.Label("" + _t1);
            GUILayout.Label("" + _tMax);
            if (GUILayout.Button("Reset"))
                _t1 = 0.99f;
            GUILayout.EndHorizontal();
            _t1 = GUILayout.HorizontalSlider(_t1, _ti, _tMax);
            GUILayout.Label("");

            //texp
            _checkDrives[(int)DType.TExp] = GUILayout.Toggle(_checkDrives[(int)DType.TExp], "Time exponent to magnify acceleration or deceleration");
            if (_checkDrives[(int)DType.TExp])
                UncheckOthers((int)DType.TExp);
            GUILayout.BeginHorizontal();
            GUILayout.Label("" + _texpMin);
            GUILayout.Label("" + _texp);
            GUILayout.Label("" + _texpMax);
            if (GUILayout.Button("Reset"))
                _texp = 1f;
            GUILayout.EndHorizontal();
            _texp = GUILayout.HorizontalSlider(_texp, _texpMin, _texpMax);
            GUILayout.Label("");


            //Tval                       
            _checkDrives[(int)DType.TVal] = GUILayout.Toggle(_checkDrives[(int)DType.TVal], "Tension");
            if (_checkDrives[(int)DType.TVal])
                UncheckOthers((int)DType.TVal);
            GUILayout.BeginHorizontal();
            GUILayout.Label("" + _tvalMin);
            GUILayout.Label("" + _tval);
            GUILayout.Label("" + _tvalMax);
            if (GUILayout.Button("Reset"))
                _tval = 0;
            GUILayout.EndHorizontal();
            _tval = GUILayout.HorizontalSlider(_tval, _tvalMin, _tvalMax);
            GUILayout.Label("");

            //Continuity                     
            _checkDrives[(int)DType.Continuity] = GUILayout.Toggle(_checkDrives[(int)DType.Continuity], "Continuity");
            if (_checkDrives[(int)DType.Continuity])
                UncheckOthers((int)DType.Continuity);
            GUILayout.BeginHorizontal();
            GUILayout.Label("" + _continuityMin);
            GUILayout.Label("" + _continuity);
            GUILayout.Label("" + _continuityMax);
            if (GUILayout.Button("Reset"))
                _continuity = 0;
            GUILayout.EndHorizontal();
            _continuity = GUILayout.HorizontalSlider(_continuity, _continuityMin, _continuityMax);
            GUILayout.Label("");

            //Bias                     
            _checkDrives[(int)DType.Bias] = GUILayout.Toggle(_checkDrives[(int)DType.TVal], "Bias");
            if (_checkDrives[(int)DType.Bias])
                UncheckOthers((int)DType.Bias);
            GUILayout.BeginHorizontal();
            GUILayout.Label("" + _biasMin);
            GUILayout.Label("" + _bias);
            GUILayout.Label("" + _biasMax);
            if (GUILayout.Button("Reset"))
                _bias = 0;
            GUILayout.EndHorizontal();
            _bias = GUILayout.HorizontalSlider(_bias, _biasMin, _biasMax);
            GUILayout.Label("");
            
         
          
            GUI.color = Color.grey;
            GUILayout.Label("Flourishes");

            GUI.color = Color.Lerp(Color.black, Color.grey, 0.5f);


            //TorsoRotMag
            _checkDrives[(int)DType.Tr] = GUILayout.Toggle(_checkDrives[(int)DType.Tr], "Torso rotation magnitude");
            if (_checkDrives[(int)DType.Tr])
                UncheckOthers((int)DType.Tr);
            GUILayout.BeginHorizontal();
            GUILayout.Label("" + _trMin);
            GUILayout.Label("" + _trMag);
            GUILayout.Label("" + _trMax);
            if (GUILayout.Button("Reset"))
                _trMag = 0f;
            GUILayout.EndHorizontal();
            _trMag = GUILayout.HorizontalSlider(_trMag, _trMin, _trMax);
            GUILayout.Label("");

            //tfMag

            _checkDrives[(int)DType.Tf] = GUILayout.Toggle(_checkDrives[(int)DType.Tf], "Torso rotation frequency");
            if (_checkDrives[(int)DType.Tf])
                UncheckOthers((int)DType.Tf);
            GUILayout.BeginHorizontal();
            GUILayout.Label("" + _tfMin);
            GUILayout.Label("" + _tfMag);
            GUILayout.Label("" + _tfMax);
            if (GUILayout.Button("Reset"))
                _tfMag = 0f;
            GUILayout.EndHorizontal();
            _tfMag = GUILayout.HorizontalSlider(_tfMag, _tfMin, _tfMax);
            GUILayout.Label("");


            
            //HeadRotMag
            _checkDrives[(int)DType.Hr] = GUILayout.Toggle(_checkDrives[(int)DType.Hr], "Head rotation magnitude");
            if (_checkDrives[(int)DType.Hr])
                UncheckOthers((int)DType.Hr);  
            GUILayout.BeginHorizontal();
            GUILayout.Label("" + _hrMin);
            GUILayout.Label("" + _hrMag);
            GUILayout.Label("" + _hrMax);
            if (GUILayout.Button("Reset"))
                _hrMag = 0f;
            GUILayout.EndHorizontal();
            _hrMag = GUILayout.HorizontalSlider(_hrMag, _hrMin, _hrMax);
            GUILayout.Label("");

            //hfMag

            _checkDrives[(int)DType.Hf] = GUILayout.Toggle(_checkDrives[(int)DType.Hf], "Head rotation frequency");
            if (_checkDrives[(int)DType.Hf])
                UncheckOthers((int)DType.Hf);  
            GUILayout.BeginHorizontal();
            GUILayout.Label("" + _hfMin);
            GUILayout.Label("" + _hfMag);
            GUILayout.Label("" + _hfMax);
            if (GUILayout.Button("Reset"))
                _hfMag = 0f;
            GUILayout.EndHorizontal();
            _hfMag = GUILayout.HorizontalSlider(_hfMag, _hfMin, _hfMax);
            GUILayout.Label("");


            //SquashMag
            _checkDrives[(int)DType.Squash] = GUILayout.Toggle(_checkDrives[(int)DType.Squash], "Torso squash magnitude (Breathing)");
            if (_checkDrives[(int)DType.Squash])
                UncheckOthers((int)DType.Squash);  
            GUILayout.BeginHorizontal();
            GUILayout.Label("" + _squashMin);
            GUILayout.Label("" + _squashMag);
            GUILayout.Label("" + _squashMax);
            if (GUILayout.Button("Reset"))
                _squashMag = 0f;
            GUILayout.EndHorizontal();
            _squashMag = GUILayout.HorizontalSlider(_squashMag, _squashMin, _squashMax);
            GUILayout.Label("");

            //WbMag
            _checkDrives[(int)DType.Wb] = GUILayout.Toggle(_checkDrives[(int)DType.Wb], "Wrist bend");
            if (_checkDrives[(int)DType.Wb])
                UncheckOthers((int)DType.Wb);  
            GUILayout.BeginHorizontal();
            GUILayout.Label("" + _wbMin);
            GUILayout.Label("" + _wbMag);
            GUILayout.Label("" + _wbMax);
            if (GUILayout.Button("Reset"))
                _wbMag = 0;
            GUILayout.EndHorizontal();
            _wbMag = GUILayout.HorizontalSlider(_wbMag, _wbMin, _wbMax);
            GUILayout.Label("");

            //WxMag
            _checkDrives[(int)DType.Wx] = GUILayout.Toggle(_checkDrives[(int)DType.Wx], "Wrist extension\n (Sets beginning wrist extension inward or outward)");
            if (_checkDrives[(int)DType.Wx])
                UncheckOthers((int)DType.Wx);  
            GUILayout.BeginHorizontal();
            GUILayout.Label("" + _wxMin);
            GUILayout.Label("" + _wxMag);
            GUILayout.Label("" + _wxMax);
            if (GUILayout.Button("Reset"))
                _wxMag = 0f;
            GUILayout.EndHorizontal();
            _wxMag = GUILayout.HorizontalSlider(_wxMag, _wxMin, _wxMax);
            GUILayout.Label("");

            //Wtmag
            _checkDrives[(int)DType.Wt] = GUILayout.Toggle(_checkDrives[(int)DType.Wt], "Wrist twist");
            if (_checkDrives[(int)DType.Wt])
                UncheckOthers((int)DType.Wt);  
            GUILayout.BeginHorizontal();
            GUILayout.Label("" + _wtMin);
            GUILayout.Label("" + _wtMag);
            GUILayout.Label("" + _wtMax);
            if (GUILayout.Button("Reset"))
                _wtMag = 0f;
            GUILayout.EndHorizontal();
            _wtMag = GUILayout.HorizontalSlider(_wtMag, _wtMin, _wtMax);
            GUILayout.Label("");


            //WfMag
            _checkDrives[(int)DType.Wf] = GUILayout.Toggle(_checkDrives[(int)DType.Wf], "Wrist frequency");
            if (_checkDrives[(int)DType.Wf])
                UncheckOthers((int)DType.Wf);  
            GUILayout.BeginHorizontal();
            GUILayout.Label("" + _wfMin);
            GUILayout.Label("" + _wfMag);
            GUILayout.Label("" + _wfMax);
            if (GUILayout.Button("Reset"))
                _wfMag = 0f;
            GUILayout.EndHorizontal();            
            _wfMag = GUILayout.HorizontalSlider(_wfMag, _wfMin, _wfMax);
            GUILayout.Label("");



            //EtMag
            _checkDrives[(int)DType.Et] = GUILayout.Toggle(_checkDrives[(int)DType.Et], "Elbow twist");
            if (_checkDrives[(int)DType.Et])
                UncheckOthers((int)DType.Et);  
            GUILayout.BeginHorizontal();
            GUILayout.Label("" + _etMin);
            GUILayout.Label("" + _etMag);
            GUILayout.Label("" + _etMax);
            if (GUILayout.Button("Reset"))
                _etMag = 0f;
            GUILayout.EndHorizontal();
            _etMag = GUILayout.HorizontalSlider(_etMag, _etMin, _etMax);
            GUILayout.Label("");

            //EfMag
            _checkDrives[(int)DType.Ef] = GUILayout.Toggle(_checkDrives[(int)DType.Ef], "Elbow frequency");
            if (_checkDrives[(int)DType.Ef])
                UncheckOthers((int)DType.Ef);
            GUILayout.BeginHorizontal();
            GUILayout.Label("" + _efMin);
            GUILayout.Label("" + _efMag);
            GUILayout.Label("" + _efMax);
            if (GUILayout.Button("Reset"))
                _efMag = 0f;
            GUILayout.EndHorizontal();
            _efMag = GUILayout.HorizontalSlider(_efMag, _efMin, _efMax);
            GUILayout.Label("");

            //DMag
            _checkDrives[(int)DType.D] = GUILayout.Toggle(_checkDrives[(int)DType.D], "Elbow displacement magnitude");
            if (_checkDrives[(int)DType.D])
                UncheckOthers((int)DType.D);
            GUILayout.BeginHorizontal();
            GUILayout.Label("" + _dMin);
            GUILayout.Label("" + _dMag);
            GUILayout.Label("" + _dMax);
            if (GUILayout.Button("Reset"))
                _dMag = 0f;
            GUILayout.EndHorizontal();
            _dMag = GUILayout.HorizontalSlider(_dMag, _dMin, _dMax);
            GUILayout.Label("");

            GUI.color = Color.grey;
            GUILayout.Label("Shape");
            GUI.color = Color.Lerp(Color.black, Color.grey, 0.5f);

            //encSpr0
            _checkDrives[(int)DType.EncSpr0] = GUILayout.Toggle(_checkDrives[(int)DType.EncSpr0], "Enclosing Spreading Start");
            if (_checkDrives[(int)DType.EncSpr0])
                UncheckOthers((int)DType.EncSpr0);
            GUILayout.BeginHorizontal();
            GUILayout.Label("" + _shapeMin);
            GUILayout.Label("" + _encSpr0);
            GUILayout.Label("" + _shapeMax);
            if (GUILayout.Button("Reset"))
                _encSpr0 = 0f;
            GUILayout.EndHorizontal();
            _encSpr0 = GUILayout.HorizontalSlider(_encSpr0, _shapeMin, _shapeMax);
            GUILayout.Label("");

            //encSpr1
            _checkDrives[(int)DType.EncSpr1] = GUILayout.Toggle(_checkDrives[(int)DType.EncSpr1], "Enclosing Spreading End");
            if (_checkDrives[(int)DType.EncSpr1])
                UncheckOthers((int)DType.EncSpr1);
            GUILayout.BeginHorizontal();
            GUILayout.Label("" + _shapeMin);
            GUILayout.Label("" + _encSpr1);
            GUILayout.Label("" + _shapeMax);
            if (GUILayout.Button("Reset"))
                _encSpr1 = 0f;
            GUILayout.EndHorizontal();
            _encSpr1 = GUILayout.HorizontalSlider(_encSpr1, _shapeMin, _shapeMax);
            GUILayout.Label("");


            //sinRis0
            _checkDrives[(int)DType.SinRis0] = GUILayout.Toggle(_checkDrives[(int)DType.SinRis0], "Sinking Rising Start");
            if (_checkDrives[(int)DType.SinRis0])
                UncheckOthers((int)DType.SinRis0);
            GUILayout.BeginHorizontal();
            GUILayout.Label("" + _shapeMin);
            GUILayout.Label("" + _sinRis0);
            GUILayout.Label("" + _shapeMax);
            if (GUILayout.Button("Reset"))
                _sinRis0 = 0f;
            GUILayout.EndHorizontal();
            _sinRis0 = GUILayout.HorizontalSlider(_sinRis0, _shapeMin, _shapeMax);
            GUILayout.Label("");


            //sinRis1
            _checkDrives[(int)DType.SinRis1] = GUILayout.Toggle(_checkDrives[(int)DType.SinRis1], "Sinking Rising End");
            if (_checkDrives[(int)DType.SinRis1])
                UncheckOthers((int)DType.SinRis1);
            GUILayout.BeginHorizontal();
            GUILayout.Label("" + _shapeMin);
            GUILayout.Label("" + _sinRis1);
            GUILayout.Label("" + _shapeMax);
            if (GUILayout.Button("Reset"))
                _sinRis1 = 0f;
            GUILayout.EndHorizontal();
            _sinRis1 = GUILayout.HorizontalSlider(_sinRis1, _shapeMin, _shapeMax);
            GUILayout.Label("");


            //retAdv0
            _checkDrives[(int)DType.RetAdv0] = GUILayout.Toggle(_checkDrives[(int)DType.RetAdv0], "Retreating Advancing Start");
            if (_checkDrives[(int)DType.RetAdv0])
                UncheckOthers((int)DType.RetAdv0);
            GUILayout.BeginHorizontal();
            GUILayout.Label("" + _shapeMin);
            GUILayout.Label("" + _retAdv0);
            GUILayout.Label("" + _shapeMax);
            if (GUILayout.Button("Reset"))
                _retAdv0 = 0f;
            GUILayout.EndHorizontal();
            _retAdv0 = GUILayout.HorizontalSlider(_retAdv0, _shapeMin, _shapeMax);
            GUILayout.Label("");


            //retAdv1
            _checkDrives[(int)DType.RetAdv1] = GUILayout.Toggle(_checkDrives[(int)DType.RetAdv1], "Retreating Advancing End");
            if (_checkDrives[(int)DType.RetAdv1])
                UncheckOthers((int)DType.RetAdv1);
            GUILayout.BeginHorizontal();
            GUILayout.Label("" + _shapeMin);
            GUILayout.Label("" + _retAdv1);
            GUILayout.Label("" + _shapeMax);
            if (GUILayout.Button("Reset"))
                _retAdv1 = 0f;
            GUILayout.EndHorizontal();
            _retAdv1 = GUILayout.HorizontalSlider(_retAdv1, _shapeMin, _shapeMax);
            GUILayout.Label("");

            bool changed;
            
            changed = GUI.changed;

            //armRX
            _checkDrives[(int)DType.ArmRX] = GUILayout.Toggle(_checkDrives[(int)DType.ArmRX], "Right Arm X");
            if (_checkDrives[(int)DType.ArmRX])
                UncheckOthers((int)DType.ArmRX);
            GUILayout.BeginHorizontal();
            GUILayout.Label("" + _shapeMin);
            GUILayout.Label("" + _arm[1].x);
            GUILayout.Label("" + _shapeMax);
            if (GUILayout.Button("Reset"))
                _arm[1].x = 0f;
            GUILayout.EndHorizontal();
            _arm[1].x = GUILayout.HorizontalSlider(_arm[1].x, _shapeMin, _shapeMax);
            GUILayout.Label("");

            //armRX
            _checkDrives[(int)DType.ArmRY] = GUILayout.Toggle(_checkDrives[(int)DType.ArmRY], "Right Arm Y");
            if (_checkDrives[(int)DType.ArmRY])
                UncheckOthers((int)DType.ArmRY);
            GUILayout.BeginHorizontal();
            GUILayout.Label("" + _shapeMin);
            GUILayout.Label("" + _arm[1].y);
            GUILayout.Label("" + _shapeMax);
            if (GUILayout.Button("Reset"))
                _arm[1].y = 0f;
            GUILayout.EndHorizontal();
            _arm[1].y = GUILayout.HorizontalSlider(_arm[1].y, _shapeMin, _shapeMax);
            GUILayout.Label("");

            //armRX
            _checkDrives[(int)DType.ArmRZ] = GUILayout.Toggle(_checkDrives[(int)DType.ArmRZ], "Right Arm Z");
            if (_checkDrives[(int)DType.ArmRZ])
                UncheckOthers((int)DType.ArmRZ);
            GUILayout.BeginHorizontal();
            GUILayout.Label("" + _shapeMin);
            GUILayout.Label("" + _arm[1].z);
            GUILayout.Label("" + _shapeMax);
            if (GUILayout.Button("Reset"))
                _arm[1].z = 0f;
            GUILayout.EndHorizontal();
            _arm[1].z = GUILayout.HorizontalSlider(_arm[1].z, _shapeMin, _shapeMax);
            GUILayout.Label("");


            if (GUI.changed && !changed) {
                _armShapeChanged[1] = true; //right arm shape is changed, update in armanimator
            }

            changed = GUI.changed;

            //armLX
            _checkDrives[(int)DType.ArmLX] = GUILayout.Toggle(_checkDrives[(int)DType.ArmLX], "Left Arm X");
            if (_checkDrives[(int)DType.ArmLX])
                UncheckOthers((int)DType.ArmLX);
            GUILayout.BeginHorizontal();
            GUILayout.Label("" + _shapeMin);
            GUILayout.Label("" + _arm[0].x);
            GUILayout.Label("" + _shapeMax);
            if (GUILayout.Button("Reset"))
                _arm[0].x = 0f;
            GUILayout.EndHorizontal();
            _arm[0].x = GUILayout.HorizontalSlider(_arm[0].x, _shapeMin, _shapeMax);
            GUILayout.Label("");

            //armLY
            _checkDrives[(int)DType.ArmLY] = GUILayout.Toggle(_checkDrives[(int)DType.ArmLY], "Left Arm Y");
            if (_checkDrives[(int)DType.ArmLY])
                UncheckOthers((int)DType.ArmLY);
            GUILayout.BeginHorizontal();
            GUILayout.Label("" + _shapeMin);
            GUILayout.Label("" + _arm[0].y);
            GUILayout.Label("" + _shapeMax);
            if (GUILayout.Button("Reset"))
                _arm[0].y = 0f;
            GUILayout.EndHorizontal();
            _arm[0].y = GUILayout.HorizontalSlider(_arm[0].y, _shapeMin, _shapeMax);
            GUILayout.Label("");

            //armLZ
            _checkDrives[(int)DType.ArmLZ] = GUILayout.Toggle(_checkDrives[(int)DType.ArmLZ], "Left Arm Z");
            if (_checkDrives[(int)DType.ArmLZ])
                UncheckOthers((int)DType.ArmLZ);
            GUILayout.BeginHorizontal();
            GUILayout.Label("" + _shapeMin);
            GUILayout.Label("" + _arm[0].z);
            GUILayout.Label("" + _shapeMax);
            if (GUILayout.Button("Reset"))
                _arm[0].z = 0f;
            GUILayout.EndHorizontal();
            _arm[0].z = GUILayout.HorizontalSlider(_arm[0].z, _shapeMin, _shapeMax);
            GUILayout.Label("");

            if (GUI.changed && !changed) {
                _armShapeChanged[0] = true; //left arm shape is changed, update in armanimator
            }
            
            if (GUILayout.Button("Reset All"))
                ResetDriveParameters();

            UpdateEmoteParams();


            _questionNo = "Question: " + (_driveInd + 1) + " of 32";
            _questionStr = ComputeEffortCombinationStr(_driveInd);
        }
        else {

            GUI.color = Color.Lerp(Color.black,Color.grey, 0.5f);
            
            //Head
            _checkShape[(int)BPart.HeadX] = GUILayout.Toggle(_checkShape[(int)BPart.HeadX], "Head");
            if (_checkShape[(int)BPart.HeadX])
                UncheckOthers((int)BPart.HeadX);    
            GUILayout.BeginHorizontal();
            GUILayout.Label("" + _rotMin);
            GUILayout.Label("" + _head.x);
            GUILayout.Label("" + _rotMax);
            if (GUILayout.Button("Reset"))
                _head.x = 0;
            GUILayout.EndHorizontal();
            _head.x = GUILayout.HorizontalSlider(_head.x, _rotMin, _rotMax);
            GUILayout.Label("");
            
            torso.Torso.Head.localRotation = torso.Torso.InitRot[(int)BodyPart.Head];
            torso.Torso.Head.Rotate(_head);



            //Neck
            _checkShape[(int)BPart.NeckX] = GUILayout.Toggle(_checkShape[(int)BPart.NeckX], "Neck");
            if (_checkShape[(int)BPart.NeckX])
                UncheckOthers((int)BPart.NeckX);    
            GUILayout.BeginHorizontal();
            GUILayout.Label("" + _rotMin);
            GUILayout.Label("" + _neck.x);
            GUILayout.Label("" + _rotMax  );
            if (GUILayout.Button("Reset"))
                _neck.x = 0;
            GUILayout.EndHorizontal();
            _neck.x = GUILayout.HorizontalSlider(_neck.x, _rotMin,_rotMax);
            GUILayout.Label("");
      

            torso.Torso.Neck.localRotation = torso.Torso.InitRot[(int)BodyPart.Neck];
            torso.Torso.Neck.Rotate(_neck);

            
            //Spine
            _checkShape[(int)BPart.SpineY] = GUILayout.Toggle(_checkShape[(int)BPart.SpineY], "Spine");
            if (_checkShape[(int)BPart.SpineY])
                UncheckOthers((int)BPart.SpineY);  
            GUILayout.BeginHorizontal();
            GUILayout.Label("" + _rotMin);
            GUILayout.Label("" + _spine.y);
            GUILayout.Label("" + _rotMax);
            if (GUILayout.Button("Reset"))
                _spine.y = 0;
            GUILayout.EndHorizontal();
            _spine.y = GUILayout.HorizontalSlider(_spine.y, _rotMin, _rotMax);
            GUILayout.Label("");

            torso.Torso.Spine.localRotation = torso.Torso.InitRot[(int)BodyPart.Spine];
            torso.Torso.Spine.Rotate(_spine);
            
            //Spine1
            _checkShape[(int)BPart.Spine1X] = GUILayout.Toggle(_checkShape[(int)BPart.Spine1X], "Spine1");
            if (_checkShape[(int)BPart.Spine1X])
                UncheckOthers((int)BPart.Spine1X);  
            GUILayout.BeginHorizontal();
            GUILayout.Label("" + _rotMin);
            GUILayout.Label("" + _spine1.x);
            GUILayout.Label("" + _rotMax);
            if (GUILayout.Button("Reset"))
                _spine1.x = 0;
            GUILayout.EndHorizontal();
            _spine1.x = GUILayout.HorizontalSlider(_spine1.x, _rotMin, _rotMax);
            GUILayout.Label("");

            torso.Torso.Spine1.localRotation = torso.Torso.InitRot[(int)BodyPart.Spine1];
            torso.Torso.Spine1.Rotate(_spine1);


            //SpineLength
            _checkShape[(int)BPart.SpineLength] = GUILayout.Toggle(_checkShape[(int)BPart.SpineLength], "Spine Length");
            if (_checkShape[(int)BPart.SpineLength])
                UncheckOthers((int)BPart.SpineLength);
            GUILayout.BeginHorizontal();
            GUILayout.Label("" + _spineLengthMin);
            GUILayout.Label("" + _spineLength);
            GUILayout.Label("" + _spineLengthMax);
            if (GUILayout.Button("Reset"))
                _spineLength = 0;
            GUILayout.EndHorizontal();
            _spineLength = GUILayout.HorizontalSlider(_spineLength, _spineLengthMin, _spineLengthMax);
            GUILayout.Label("");


            torso.Torso.Spine1.localPosition = torso.Torso.InitPos[(int)BodyPart.Spine1];
            torso.Torso.Spine1.Translate(0, 0,  _spineLength );
           

            
            //Shoulders x
            _checkShape[(int)BPart.ShouldersX] = GUILayout.Toggle(_checkShape[(int)BPart.ShouldersX], "Shoulders axis 1");
            if (_checkShape[(int)BPart.ShouldersX])
                UncheckOthers((int)BPart.ShouldersX);  
            GUILayout.BeginHorizontal();
            GUILayout.Label("" + _rotMin);
            GUILayout.Label("" + _shoulders.x);
            GUILayout.Label("" + _rotMax);
            if (GUILayout.Button("Reset"))
                _shoulders.x = 0;
            GUILayout.EndHorizontal();
            _shoulders.x = GUILayout.HorizontalSlider(_shoulders.x, _rotMin, _rotMax);
            
            GUILayout.Label("");

            //Shoulders y
            _checkShape[(int)BPart.ShouldersY] = GUILayout.Toggle(_checkShape[(int)BPart.ShouldersY], "Shoulders axis 2");
            if (_checkShape[(int)BPart.ShouldersY])
                UncheckOthers((int)BPart.ShouldersY);  
            GUILayout.BeginHorizontal();
            GUILayout.Label("" + _rotMin);
            GUILayout.Label("" + _shoulders.y);
            GUILayout.Label("" + _rotMax);
            if (GUILayout.Button("Reset"))
                _shoulders.y = 0;
            GUILayout.EndHorizontal();
            _shoulders.y = GUILayout.HorizontalSlider(_shoulders.y, _rotMin, _rotMax);
            GUILayout.Label("");

            //Shoulders z
            _checkShape[(int)BPart.ShouldersZ] = GUILayout.Toggle(_checkShape[(int)BPart.ShouldersZ], "Shoulders axis 3");
            if (_checkShape[(int)BPart.ShouldersZ])
                UncheckOthers((int)BPart.ShouldersZ);  
            GUILayout.BeginHorizontal();
            GUILayout.Label("" + _rotMin);
            GUILayout.Label("" + _shoulders.z);
            GUILayout.Label("" + _rotMax);
            if (GUILayout.Button("Reset"))
                _shoulders.z = 0;
            GUILayout.EndHorizontal();
            _shoulders.z = GUILayout.HorizontalSlider(_shoulders.z, _rotMin, _rotMax);
            GUILayout.Label("");


            torso.Torso.ShoulderL.localRotation = torso.Torso.InitRot[(int)BodyPart.ShoulderL];
            torso.Torso.ShoulderL.Rotate(_shoulders.x, _shoulders.y, _shoulders.z);

            torso.Torso.ShoulderR.localRotation = torso.Torso.InitRot[(int)BodyPart.ShoulderR];
            torso.Torso.ShoulderR.Rotate(_shoulders.x, -_shoulders.y,-_shoulders.z);


            //Clavicles z
            _checkShape[(int)BPart.ClaviclesZ] = GUILayout.Toggle(_checkShape[(int)BPart.ClaviclesZ], "Clavicles axis 1");
            if (_checkShape[(int)BPart.ClaviclesZ])
                UncheckOthers((int)BPart.ClaviclesZ); 
            GUILayout.BeginHorizontal();
            GUILayout.Label("" + _rotMin);
            GUILayout.Label("" + _clavicles.z);
            GUILayout.Label("" + _rotMax);
            if (GUILayout.Button("Reset"))
                _clavicles.z = 0;
            GUILayout.EndHorizontal();
            _clavicles.z = GUILayout.HorizontalSlider(_clavicles.z, _rotMin, _rotMax);
            GUILayout.Label("");

            //Clavicles x
            _checkShape[(int)BPart.ClaviclesX] = GUILayout.Toggle(_checkShape[(int)BPart.ClaviclesX], "Clavicles axis 2");
            if (_checkShape[(int)BPart.ClaviclesX])
                UncheckOthers((int)BPart.ClaviclesX); 
            GUILayout.BeginHorizontal();
            GUILayout.Label("" + _rotMin);
            GUILayout.Label("" + _clavicles.x);
            GUILayout.Label("" + _rotMax);
            if (GUILayout.Button("Reset"))
                _clavicles.x = 0;
            GUILayout.EndHorizontal();
            _clavicles.x = GUILayout.HorizontalSlider(_clavicles.x, _rotMin, _rotMax);
            GUILayout.Label("");

            //Clavicles y
            _checkShape[(int)BPart.ClaviclesY] = GUILayout.Toggle(_checkShape[(int)BPart.ClaviclesY], "Clavicles axis 3");
            if (_checkShape[(int)BPart.ClaviclesY])
                UncheckOthers((int)BPart.ClaviclesY); 
            GUILayout.BeginHorizontal();
            GUILayout.Label("" + _rotMin);
            GUILayout.Label("" + _clavicles.y);
            GUILayout.Label("" + _rotMax);
            if (GUILayout.Button("Reset"))
                _clavicles.y = 0;
            GUILayout.EndHorizontal();
            _clavicles.y = GUILayout.HorizontalSlider(_clavicles.y, _rotMin, _rotMax);
            GUILayout.Label("");


            torso.Torso.ClavicleL.localRotation = torso.Torso.InitRot[(int)BodyPart.ClavicleL];
            torso.Torso.ClavicleL.Rotate(_clavicles.x, _clavicles.y, _clavicles.z);

            torso.Torso.ClavicleR.localRotation = torso.Torso.InitRot[(int)BodyPart.ClavicleR];
            torso.Torso.ClavicleR.Rotate(_clavicles.x, -_clavicles.y, -_clavicles.z);



            //Pelvis L x
            _checkShape[(int)BPart.PelvisLX] = GUILayout.Toggle(_checkShape[(int)BPart.PelvisLX], "Left Pelvis axis 1");
            if (_checkShape[(int)BPart.PelvisLX])
                UncheckOthers((int)BPart.PelvisLX); 
            GUILayout.BeginHorizontal();
            GUILayout.Label("" + _rotMin);
            GUILayout.Label("" + _pelvisL.x);
            GUILayout.Label("" + _rotMax);
            if (GUILayout.Button("Reset"))
                _pelvisL.x = 0;
            GUILayout.EndHorizontal();
            _pelvisL.x = GUILayout.HorizontalSlider(_pelvisL.x, _rotMin, _rotMax);
            GUILayout.Label("");

            //Pelvis R x
            _checkShape[(int)BPart.PelvisRX] = GUILayout.Toggle(_checkShape[(int)BPart.PelvisRX], "Right Pelvis axis 1");
            if (_checkShape[(int)BPart.PelvisRX])
                UncheckOthers((int)BPart.PelvisRX);
            GUILayout.BeginHorizontal();
            GUILayout.Label("" + _rotMin);
            GUILayout.Label("" + _pelvisR.x);
            GUILayout.Label("" + _rotMax);
            if (GUILayout.Button("Reset"))
                _pelvisR.x = 0;
            GUILayout.EndHorizontal();
            _pelvisR.x = GUILayout.HorizontalSlider(_pelvisR.x, _rotMin, _rotMax);
            GUILayout.Label("");


            //Pelvis y
            _checkShape[(int)BPart.PelvisY] = GUILayout.Toggle(_checkShape[(int)BPart.PelvisY], "Pelvis axis 2");
            if (_checkShape[(int)BPart.PelvisY])
                UncheckOthers((int)BPart.PelvisY); 
            GUILayout.BeginHorizontal();
            GUILayout.Label("" + _rotMin);
            GUILayout.Label("" + _pelvisL.y);
            GUILayout.Label("" + _rotMax);
            if (GUILayout.Button("Reset"))
                _pelvisL.y = _pelvisR.y = 0;
            GUILayout.EndHorizontal();
            _pelvisL.y = _pelvisR.y = GUILayout.HorizontalSlider(_pelvisL.y, _rotMin, _rotMax);
            GUILayout.Label("");

            //Pelvis z
            _checkShape[(int)BPart.PelvisZ] = GUILayout.Toggle(_checkShape[(int)BPart.PelvisZ], "Pelvis axis 3");
            if (_checkShape[(int)BPart.PelvisZ])
                UncheckOthers((int)BPart.PelvisZ); 
            GUILayout.BeginHorizontal();
            GUILayout.Label("" + _rotMin);
            GUILayout.Label("" + _pelvisL.z);
            GUILayout.Label("" + _rotMax);
            if (GUILayout.Button("Reset"))
                _pelvisL.z = _pelvisR.z = 0;
            GUILayout.EndHorizontal();
            _pelvisL.z = _pelvisR.z = GUILayout.HorizontalSlider(_pelvisL.z, _rotMin, _rotMax);
            GUILayout.Label("");


            //Hips
            _checkShape[(int)BPart.HipsX] = GUILayout.Toggle(_checkShape[(int)BPart.HipsX], "Hips");
            if (_checkShape[(int)BPart.HipsX])
                UncheckOthers((int)BPart.HipsX);
            GUILayout.BeginHorizontal();
            GUILayout.Label("" + _rotMin);
            GUILayout.Label("" + _hips.x);
            GUILayout.Label("" + _rotMax);
            if (GUILayout.Button("Reset"))
                _hips.x = 0;
            GUILayout.EndHorizontal();
            _hips.x = GUILayout.HorizontalSlider(_hips.x, _rotMin, _rotMax);
            GUILayout.Label("");


       //     GUILayout.Label("1: " + torso.Torso.Root.position + " " + torso.Torso.InitRootPos + " " + torso.Torso.FootL.position + " " + torso.Torso.InitFootPos);

          //  torso.Torso.Root.position = torso.Torso.InitRootPos;
            
            //Debug.Log(torso.Torso.InitPos[(int)BodyPart.Root] + " " + torso.Torso.InitRootPos);
          
            
        //    torso.Torso.Root.localPosition = torso.Torso.InitPos[(int)BodyPart.Root] - ((torso.Torso.FootL.localPosition + torso.Torso.FootR.localPosition) / 2f - (torso.Torso.InitPos[(int)BodyPart.FootL] + torso.Torso.InitPos[(int)BodyPart.FootR]) / 2);
           

         //  GUILayout.Label("2: " + torso.Torso.Root.position + " " + torso.Torso.InitRootPos + " " + torso.Torso.FootL.position + " " + torso.Torso.InitFootPos);

       //    GUILayout.Label("3: " + _pelvis.x + " " + _pelvis.y + " " + _pelvis.z);


            //Knees x
            _checkShape[(int)BPart.KneesX] = GUILayout.Toggle(_checkShape[(int)BPart.KneesX], "Knees");
            if (_checkShape[(int)BPart.KneesX])
                UncheckOthers((int)BPart.KneesX);
            GUILayout.BeginHorizontal();
            GUILayout.Label("" + _rotMin);
            GUILayout.Label("" + _knees.x);
            GUILayout.Label("" + 0);
            if (GUILayout.Button("Reset"))
                _knees.x = 0;
            GUILayout.EndHorizontal();
            _knees.x = GUILayout.HorizontalSlider(_knees.x, _rotMin, 0);
            GUILayout.Label("");


            //Toes x
            _checkShape[(int)BPart.ToesX] = GUILayout.Toggle(_checkShape[(int)BPart.ToesX], "Toes");
            if (_checkShape[(int)BPart.ToesX])
                UncheckOthers((int)BPart.ToesX);
            GUILayout.BeginHorizontal();
            GUILayout.Label("" + 0);
            GUILayout.Label("" + _toes.x);
            GUILayout.Label("" + _rotMax);
            if (GUILayout.Button("Reset")) {                
                _toes = Vector3.zero;
            }
            GUILayout.EndHorizontal();
            _toes.x = GUILayout.HorizontalSlider(_toes.x, 0, _rotMax);
            GUILayout.Label("");



            if (GUILayout.Button("Reset All")) {
                ResetShapeParameters();
            }

            torso.Torso.PelvisL.localRotation = torso.Torso.InitRot[(int)BodyPart.PelvisL];
            torso.Torso.PelvisL.Rotate(_pelvisL.x, _pelvisL.y, _pelvisL.z);

            torso.Torso.PelvisR.localRotation = torso.Torso.InitRot[(int)BodyPart.PelvisR];
            torso.Torso.PelvisR.Rotate(_pelvisR.x, -_pelvisR.y, -_pelvisR.z);


            torso.Torso.KneeL.localRotation = torso.Torso.InitRot[(int)BodyPart.KneeL];
            torso.Torso.KneeL.Rotate(_knees.x, _knees.y, _knees.z);

            torso.Torso.KneeR.localRotation = torso.Torso.InitRot[(int)BodyPart.KneeR];
            torso.Torso.KneeR.Rotate(_knees.x, _knees.y, _knees.z);


            //Toes are rotated in world space because their local axes are skewed
            torso.Torso.ToeL.localRotation = torso.Torso.InitRot[(int)BodyPart.ToeL];
            torso.Torso.ToeL.Rotate(_toes.x, 0, 0, Space.World);

            //Toes are rotated in world space because their local axes are skewed
            torso.Torso.ToeR.localRotation = torso.Torso.InitRot[(int)BodyPart.ToeR];
            torso.Torso.ToeR.Rotate(_toes.x, 0, 0, Space.World);

            //Hips are rotated in world space because their local axes are skewed
            torso.Torso.Hips.localRotation = torso.Torso.InitRot[(int)BodyPart.Hips];
            torso.Torso.Hips.Rotate(_hips.x, 0, 0, Space.World);

            //Feet are rotated in world space because their local axes are skewed
            torso.Torso.FootL.localRotation = torso.Torso.InitRot[(int)BodyPart.FootL];
            //torso.Torso.FootL.Rotate(-_pelvisL.x - _knees.x + _hips.x - _toes.x, 0, 0, Space.Self);
            torso.Torso.FootL.Rotate(-_pelvisL.x - _knees.x - _hips.x - _toes.x, 0, 0, Space.World);

            //Feet are rotated in world space because their local axes are skewed
            torso.Torso.FootR.localRotation = torso.Torso.InitRot[(int)BodyPart.FootR];
            torso.Torso.FootR.Rotate(-_pelvisR.x - _knees.x - _hips.x - _toes.x, 0, 0, Space.World);
            //torso.Torso.FootR.Rotate(-_pelvisR.x - _knees.x + _hips.x - _toes.x, 0, 0, Space.Self);

            

            Vector3 groundToe;
            if (torso.Torso.ToeL.position.y < torso.Torso.ToeR.position.y)
                groundToe = torso.Torso.ToeL.position;
            else
                groundToe = torso.Torso.ToeR.position;

            //torso.Torso.Root.position = torso.Torso.InitRootPos - ((torso.Torso.FootL.position + torso.Torso.FootR.position) / 2f - torso.Torso.InitFootPos);

            float groundY = torso.Torso.InitRootPos.y - (groundToe.y - torso.Torso.InitToePos.y) ;



            torso.Torso.Root.position = new Vector3(torso.Torso.Root.position.x, groundY, torso.Torso.Root.position.z);

            
           // Debug.Log(_pelvisR.x + " " + _pelvisR.y + " " + _pelvisR.z + " " + _hips.x  + " " + _hips.y  +   " " +  _hips.z +  " "  +_toes.x  + " " +_toes.y + " "  +_toes.z +  " " + groundY + " " + torso.Torso.Root.position);




            GUI.color = Color.white;


            _questionNo = "Question: " + (_shapeInd + 1) + " of 6";
            _questionStr = _shapeStr[_shapeInd];
        }
     
        GUILayout.EndScrollView();                  
         
        
	}

    string ComputeEffortCombinationStr(int driveInd) {
        string str = "";
        if (_effortList[driveInd, 0] == -1)
            str += "Indirect";
        else if (_effortList[driveInd, 0] == 1)
            str += "Direct";

        str += " ";
        if (_effortList[driveInd, 1] == -1)
            str += "Light";
        else if (_effortList[driveInd, 1] == 1)
            str += "Strong";

        str += " ";
        if (_effortList[driveInd, 2] == -1)
            str += "Sustained";
        else if (_effortList[driveInd, 2] == 1)
            str += "Sudden";


        str += " ";
        if (_effortList[driveInd, 3] == -1)
            str += "Free";
        else if (_effortList[driveInd, 3] == 1)
            str += "Bound";

        return str;

    }

    void SetSubmittedDriveParameters() {
    }

    void ResetDriveParameters() {
        _speed = 2f;
        _tval = 0;
        _v0 = 0;
        _v1 = 0;
        _ti = 0.5f;
        _texp = 1f;
        _tval = 0f;
        _t0 = 0.01f;
        _t1 = 0.99f;
        _trMag = 0f;
        _tfMag = 0f;
        _hrMag = 0f;
        _hfMag = 0f;
        _squashMag = 0f;
        _wbMag = 0f;
        _wxMag = 0f;
        _wtMag = 0f;
        _wfMag = 0f;
        _etMag = 0f;
        _efMag = 0f;
        _dMag = 0f;
        _encSpr0 = 0f;
        _sinRis0 = 0f;
        _retAdv0 = 0f;
        _encSpr1 = 0f;
        _sinRis1 = 0f;
        _retAdv1 = 0f;
        _arm[0] = Vector3.zero;
        _arm[1] = Vector3.zero;
        
    }
    void ResetShapeParameters() {
        _head = Vector3.zero;
        _neck = Vector3.zero;
        _spine = Vector3.zero;
        _spine1 = Vector3.zero;
        _shoulders = Vector3.zero;
        _clavicles = Vector3.zero;
        _pelvisL = Vector3.zero;
        _pelvisR = Vector3.zero;
        _knees = Vector3.zero;
        _hips = Vector3.zero;
        _toes = Vector3.zero;
        _spineLength = 0;

    }
    void StopAnim(){
        GameObject agent = GameObject.Find("AgentPrefab");
        GameObject agentControl = GameObject.Find("AgentControlPrefab");	
	    
        if(agent.animation.isPlaying) {
           agent.SampleAnimation(agent.animation.clip, 0); //instead of rewind
           agent.animation.Stop();           
        }
        if (agentControl.animation.isPlaying) {
            agentControl.SampleAnimation(agentControl.animation.clip, 0); //instead of rewind
            agentControl.animation.Stop();
        }
        
    }

    void InitAgent(GameObject agent, string animName) {
        if (!agent) 
            return;

        
        agent.GetComponent<AnimationInfo>().Reset(animName);                            
        agent.GetComponent<ArmAnimator>().Reset();

        agent.animation.enabled = true;                        
        agent.animation.Play(animName);

        if(agent == GameObject.Find("AgentPrefab"))
            agent.GetComponent<ArmAnimator>().UpdateInterpolators();
    }

    void PlayAnim(string animName) {
        GameObject agent = GameObject.Find("AgentPrefab");
        GameObject agentControl = GameObject.Find("AgentControlPrefab");	
        if(agent.GetComponent<ArmAnimator>().ArmC == null ) {
			Debug.Log ("Controller not assigned");
			return;
		}
     
        AnimationInfo animInfo = agent.GetComponent<AnimationInfo>();
        AnimationInfo animInfoControl = agentControl.GetComponent<AnimationInfo>();
        animInfo.IsContinuous = _toggleContinuous;
        animInfoControl.IsContinuous = _toggleContinuous;
        agent.animation.Stop(); //in order to restart animation
        agentControl.animation.Stop(); //in order to restart animation
        InitAgent(agent, animName);
        InitAgent(agentControl, animName);
       
        agentControl.GetComponent<ArmAnimator>().ResetParameters();
                           

        if(animInfo.IsContinuous){
            agent.animation[animInfo.AnimName].wrapMode = WrapMode.Loop;
            agentControl.animation[animInfo.AnimName].wrapMode = WrapMode.Loop;
            
        }
        else{
            agent.animation[animInfo.AnimName].wrapMode = WrapMode.ClampForever;
            agentControl.animation[animInfo.AnimName].wrapMode = WrapMode.ClampForever;
        }        
        

    }
    
    void ResetTransforms() {
		GameObject agent = GameObject.Find("AgentPrefab");
        GameObject agentControl = GameObject.Find("AgentControlPrefab");	
		
		if(agent.GetComponent<ArmAnimator>().ArmC == null ) {
			Debug.Log ("Controller not assigned");
			return;
		}
			
			
		agent.GetComponent<ArmAnimator>().ArmC.ResetTransforms();
        agentControl.GetComponent<ArmAnimator>().ArmC.ResetTransforms();
		
	}


    void UpdateEmoteParams() {
        GameObject agent = GameObject.Find("AgentPrefab");
		if(agent == null){		
			Debug.Log("AgentPrefab not found");
			return;
		}
        
        if (_armShapeChanged[0]) {
            agent.GetComponent<ArmAnimator>().Hor = _arm[0].x;
            agent.GetComponent<ArmAnimator>().Ver = _arm[0].y;
            agent.GetComponent<ArmAnimator>().Sag = _arm[0].z;
            agent.GetComponent<ArmAnimator>().UpdateKeypointsByShape(0); //Update keypoints
        }
        if(_armShapeChanged[1]){
            //RightArm 
            //Only horizontal motion is the opposite for each arm
            agent.GetComponent<ArmAnimator>().Hor = -_arm[1].x;
            agent.GetComponent<ArmAnimator>().Ver = _arm[1].y;
            agent.GetComponent<ArmAnimator>().Sag = _arm[1].z;
            agent.GetComponent<ArmAnimator>().UpdateKeypointsByShape(1); //Update keypoints

        }

        agent.GetComponent<ArmAnimator>().SetSpeed(_speed);        
        agent.GetComponent<ArmAnimator>().V0 = _v0;
        agent.GetComponent<ArmAnimator>().V1 = _v1;
        /*agent.GetComponent<ArmAnimator>().T0 = _t0 /_speed;
        agent.GetComponent<ArmAnimator>().T1 = _t1 /_speed;
        agent.GetComponent<ArmAnimator>().Ti = _ti /_speed;
       */
       
        agent.GetComponent<ArmAnimator>().T0 = _t0 ;
        agent.GetComponent<ArmAnimator>().T1 = _t1 ;
        agent.GetComponent<ArmAnimator>().Ti = _ti ;
        

        agent.GetComponent<ArmAnimator>().Texp = _texp;

        float prevTVal = agent.GetComponent<ArmAnimator>().Tval;
        float prevContinuity = agent.GetComponent<ArmAnimator>().Continuity;
        float prevBias = agent.GetComponent<ArmAnimator>().Bias;
        agent.GetComponent<ArmAnimator>().Tval = _tval;
        agent.GetComponent<ArmAnimator>().Continuity = _continuity;
        agent.GetComponent<ArmAnimator>().Bias = _bias;

        if (_tval != prevTVal || _continuity != prevContinuity || _bias != prevBias) 
            agent.GetComponent<AnimationInfo>().InitInterpolators(_tval, _continuity, _bias);

       

        agent.GetComponent<ArmAnimator>().TrMag = _trMag;
        agent.GetComponent<ArmAnimator>().TfMag = _tfMag;
        
        agent.GetComponent<ArmAnimator>().HrMag = _hrMag;
        agent.GetComponent<ArmAnimator>().HfMag = _hfMag;
        agent.GetComponent<ArmAnimator>().SquashMag = _squashMag;

        agent.GetComponent<ArmAnimator>().WbMag = _wbMag;
        agent.GetComponent<ArmAnimator>().WxMag = _wxMag;
        agent.GetComponent<ArmAnimator>().WfMag = _wfMag;
        agent.GetComponent<ArmAnimator>().WtMag = _wtMag;
        agent.GetComponent<ArmAnimator>().EfMag = _efMag;
        agent.GetComponent<ArmAnimator>().EtMag = _etMag;
        agent.GetComponent<ArmAnimator>().DMag = _dMag;


        agent.GetComponent<TorsoAnimator>().EncSpr[0] = _encSpr0;
        agent.GetComponent<TorsoAnimator>().SinRis[0] = _sinRis0;
        agent.GetComponent<TorsoAnimator>().RetAdv[0] = _retAdv0;

        agent.GetComponent<TorsoAnimator>().EncSpr[1] = _encSpr1;
        agent.GetComponent<TorsoAnimator>().SinRis[1] = _sinRis1;
        agent.GetComponent<TorsoAnimator>().RetAdv[1] = _retAdv1;

        agent.GetComponent<TorsoAnimator>().UpdateAnglesLinearComb();

        
    }

   

    // remember to use StartCoroutine when calling this function!
    IEnumerator PostValuesDrives() {
        string resultURL = "https://fling.seas.upenn.edu/~fundad/cgi-bin/RCTAMAN/putDriveData.php";
               
     // Create a form object for sending high score data to the server
        var form = new WWWForm();        
        form.AddField( "userId", UserInfo.userId);
        form.AddField("driveInd", _driveInd.ToString());
                
        form.AddField("speed", _speed.ToString());
        form.AddField("v0", _v0.ToString());
        form.AddField("v1", _v1.ToString());
        form.AddField("ti", _ti.ToString());
        form.AddField("texp", _texp.ToString());
        form.AddField("tval", _tval.ToString());
        form.AddField("continuity", _continuity.ToString());
        form.AddField("bias", _bias.ToString());
        form.AddField("t0", _t0.ToString());
        form.AddField("t1", _t1.ToString());

        form.AddField("tr", _trMag.ToString());
        form.AddField("tf", _tfMag.ToString());

        form.AddField("hr", _hrMag.ToString());
        form.AddField("hf", _hfMag.ToString());
        form.AddField("squash", _squashMag.ToString());
        form.AddField("wb", _wbMag.ToString());
        form.AddField("wx", _wxMag.ToString());
        form.AddField("wt", _wtMag.ToString());
        form.AddField("wf", _wfMag.ToString());
        form.AddField("et", _etMag.ToString());
        form.AddField("d", _dMag.ToString());
        form.AddField("ef", _efMag.ToString());

        form.AddField("encSpr0", _encSpr0.ToString());
        form.AddField("sinRis0", _sinRis0.ToString());
        form.AddField("retAdv0", _retAdv0.ToString());
        form.AddField("encSpr1", _encSpr1.ToString());
        form.AddField("sinRis1", _sinRis1.ToString());
        form.AddField("retAdv1", _retAdv1.ToString());

        form.AddField("armLX", _arm[0].x.ToString());
        form.AddField("armLY", _arm[0].y.ToString());
        form.AddField("armLZ", _arm[0].z.ToString());
        form.AddField("armRX", _arm[1].x.ToString());
        form.AddField("armRY", _arm[1].y.ToString());
        form.AddField("armRZ", _arm[1].z.ToString());
        

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

    // remember to use StartCoroutine when calling this function!
    IEnumerator GetValuesDrives() {
        string resultURL = "https://fling.seas.upenn.edu/~fundad/cgi-bin/RCTAMAN/getDriveData.php";

        // Create a form object for sending high score data to the server
        var form = new WWWForm();
        form.AddField("userId", UserInfo.userId);
        form.AddField("driveInd", _driveInd.ToString());

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
            if (Info.Length == 0)
                ResetDriveParameters();
            //Assign drive values 
            //should be exactly in this order
            int i = 0;
            _speed = float.Parse(vals[i++]);
            _v0 = float.Parse(vals[i++]);
            _v1 = float.Parse(vals[i++]);
            _ti = float.Parse(vals[i++]);
            _texp = float.Parse(vals[i++]);
            _tval = float.Parse(vals[i++]);
            _continuity = float.Parse(vals[i++]);
            _bias = float.Parse(vals[i++]);
            _t0 = float.Parse(vals[i++]);
            _t1 = float.Parse(vals[i++]);
            _trMag = float.Parse(vals[i++]);
            _tfMag = float.Parse(vals[i++]);
            _hrMag = float.Parse(vals[i++]);
            _hfMag = float.Parse(vals[i++]);
            _squashMag = float.Parse(vals[i++]);
            _wbMag = float.Parse(vals[i++]);
            _wxMag = float.Parse(vals[i++]);
            _wtMag = float.Parse(vals[i++]);
            _wfMag = float.Parse(vals[i++]);
            _etMag = float.Parse(vals[i++]);
            _efMag = float.Parse(vals[i++]);
            _dMag = float.Parse(vals[i++]);
            _encSpr0 = float.Parse(vals[i++]);
            _sinRis0 = float.Parse(vals[i++]);
            _retAdv0 = float.Parse(vals[i++]);
            _encSpr1 = float.Parse(vals[i++]);
            _sinRis1 = float.Parse(vals[i++]);
            _retAdv1 = float.Parse(vals[i++]);
            _arm[0].x = float.Parse(vals[i++]);
            _arm[0].y = float.Parse(vals[i++]);
            _arm[0].z = float.Parse(vals[i++]);
            _arm[1].x = float.Parse(vals[i++]);
            _arm[1].y = float.Parse(vals[i++]);
            _arm[1].z = float.Parse(vals[i++]);

        }
    }

    // remember to use StartCoroutine when calling this function!
    IEnumerator PostValuesShapes() {
        string resultURL = "https://fling.seas.upenn.edu/~fundad/cgi-bin/RCTAMAN/putShapeData.php";

        // Create a form object for sending high score data to the server
        var form = new WWWForm();
        form.AddField("userId", UserInfo.userId);
        form.AddField("shapeInd", _shapeInd.ToString());

        form.AddField("head", _head.x.ToString());
        form.AddField("neck", _neck.x.ToString());
        form.AddField("spine", _spine.y.ToString());
        form.AddField("spine1", _spine1.x.ToString());
        form.AddField("shouldersX", _shoulders.x.ToString());
        form.AddField("shouldersY", _shoulders.y.ToString());
        form.AddField("shouldersZ", _shoulders.z.ToString());
        form.AddField("claviclesX", _clavicles.x.ToString());
        form.AddField("claviclesY", _clavicles.y.ToString());
        form.AddField("claviclesZ", _clavicles.z.ToString());
        form.AddField("pelvisLX", _pelvisL.x.ToString());
        form.AddField("pelvisRX", _pelvisR.x.ToString());
        form.AddField("pelvisY", _pelvisL.y.ToString());
        form.AddField("pelvisZ", _pelvisL.z.ToString());
        form.AddField("kneesX", _knees.x.ToString());
        form.AddField("hipsX", _hips.x.ToString());
        form.AddField("toesX", _toes.x.ToString());
        form.AddField("spineLength", _spineLength.ToString());

        // Create a download object
        var download = new WWW(resultURL, form);

        // Wait until the download is done
        yield return download;

        if (download.error != null) {
            Info = download.error;
            print("Error: " + download.error);
        }
        else {
            Info = "success " + download.text;
        }
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
            if (ShapeInfo.Length == 0)
                ResetShapeParameters();
            //Assign shape values 
            TorsoAnimator torsoAnim = GameObject.Find("AgentPrefab").GetComponent<TorsoAnimator>();

            int i = 0;
            _head.x = torsoAnim.ShapeParams[(int)BPart.HeadX][shapeInd] = float.Parse(vals[i++]);
            _neck.x = torsoAnim.ShapeParams[(int)BPart.NeckX][shapeInd] = float.Parse(vals[i++]);
            _spine.y = torsoAnim.ShapeParams[(int)BPart.SpineY][shapeInd] = float.Parse(vals[i++]);
            _spine1.x = torsoAnim.ShapeParams[(int)BPart.Spine1X][shapeInd] = float.Parse(vals[i++]);
            _shoulders.x = torsoAnim.ShapeParams[(int)BPart.ShouldersX][shapeInd] = float.Parse(vals[i++]);
            _shoulders.y = torsoAnim.ShapeParams[(int)BPart.ShouldersY][shapeInd] = float.Parse(vals[i++]);
            _shoulders.z = torsoAnim.ShapeParams[(int)BPart.ShouldersZ][shapeInd] = float.Parse(vals[i++]);
            _clavicles.x = torsoAnim.ShapeParams[(int)BPart.ClaviclesX][shapeInd] = float.Parse(vals[i++]);
            _clavicles.y = torsoAnim.ShapeParams[(int)BPart.ClaviclesY][shapeInd] = float.Parse(vals[i++]);
            _clavicles.z = torsoAnim.ShapeParams[(int)BPart.ClaviclesZ][shapeInd] = float.Parse(vals[i++]);
            _pelvisL.x = torsoAnim.ShapeParams[(int)BPart.PelvisLX][shapeInd] = float.Parse(vals[i++]);
            _pelvisR.x = torsoAnim.ShapeParams[(int)BPart.PelvisRX][shapeInd] = float.Parse(vals[i++]);
            _pelvisL.y = _pelvisR.y = torsoAnim.ShapeParams[(int)BPart.PelvisY][shapeInd] = float.Parse(vals[i++]);
            _pelvisL.z = _pelvisR.z =  torsoAnim.ShapeParams[(int)BPart.PelvisZ][shapeInd] = float.Parse(vals[i++]);
            _knees.x = torsoAnim.ShapeParams[(int)BPart.KneesX][shapeInd] = float.Parse(vals[i++]);
            _hips.x = torsoAnim.ShapeParams[(int)BPart.HipsX][shapeInd] = float.Parse(vals[i++]);
            _toes.x = torsoAnim.ShapeParams[(int)BPart.ToesX][shapeInd] = float.Parse(vals[i++]);
            _spineLength = torsoAnim.ShapeParams[(int)BPart.SpineLength][shapeInd] = float.Parse(vals[i++]);

        }
    }

    void ReadValuesShapes(int shapeInd) {
        string fileName = "shapesSusan.txt";
        StreamReader sr = new StreamReader(fileName);

        TorsoAnimator torsoAnim = GameObject.Find("AgentPrefab").GetComponent<TorsoAnimator>();
        string[] content = File.ReadAllLines(fileName);

        String[] tokens = content[shapeInd + 1].Split('\t');

        int i = 2;
        _head.x = torsoAnim.ShapeParams[(int)BPart.HeadX][shapeInd] = float.Parse(tokens[i++]);
        _neck.x = torsoAnim.ShapeParams[(int)BPart.NeckX][shapeInd] = float.Parse(tokens[i++]);
        _spine.y = torsoAnim.ShapeParams[(int)BPart.SpineY][shapeInd] = float.Parse(tokens[i++]);
        _spine1.x = torsoAnim.ShapeParams[(int)BPart.Spine1X][shapeInd] = float.Parse(tokens[i++]);
        _shoulders.x = torsoAnim.ShapeParams[(int)BPart.ShouldersX][shapeInd] = float.Parse(tokens[i++]);
        _shoulders.y = torsoAnim.ShapeParams[(int)BPart.ShouldersY][shapeInd] = float.Parse(tokens[i++]);
        _shoulders.z = torsoAnim.ShapeParams[(int)BPart.ShouldersZ][shapeInd] = float.Parse(tokens[i++]);
        _clavicles.x = torsoAnim.ShapeParams[(int)BPart.ClaviclesX][shapeInd] = float.Parse(tokens[i++]);
        _clavicles.y = torsoAnim.ShapeParams[(int)BPart.ClaviclesY][shapeInd] = float.Parse(tokens[i++]);
        _clavicles.z = torsoAnim.ShapeParams[(int)BPart.ClaviclesZ][shapeInd] = float.Parse(tokens[i++]);
        _pelvisL.x = torsoAnim.ShapeParams[(int)BPart.PelvisLX][shapeInd] = float.Parse(tokens[i++]);
        _pelvisR.x = torsoAnim.ShapeParams[(int)BPart.PelvisRX][shapeInd] = float.Parse(tokens[i++]);
        _pelvisL.y = _pelvisR.y = torsoAnim.ShapeParams[(int)BPart.PelvisY][shapeInd] = float.Parse(tokens[i++]);
        _pelvisL.z = _pelvisR.z = torsoAnim.ShapeParams[(int)BPart.PelvisZ][shapeInd] = float.Parse(tokens[i++]);
        _knees.x = torsoAnim.ShapeParams[(int)BPart.KneesX][shapeInd] = float.Parse(tokens[i++]);
        _hips.x = torsoAnim.ShapeParams[(int)BPart.HipsX][shapeInd] = float.Parse(tokens[i++]);
        _toes.x = torsoAnim.ShapeParams[(int)BPart.ToesX][shapeInd] = float.Parse(tokens[i++]);
        _spineLength = torsoAnim.ShapeParams[(int)BPart.SpineLength][shapeInd] = float.Parse(tokens[i++]);

      
        sr.Close();
    }


    void ReadValuesDrives(int effortInd) {
        string fileName = "drivesSusan.txt";
        StreamReader sr = new StreamReader(fileName);


        string[] content = File.ReadAllLines(fileName);

        String[] tokens = content[_driveInd + 1].Split('\t');

        int i = 2;
        _speed = float.Parse(tokens[i++]);
        _v0 = float.Parse(tokens[i++]);
        _v1 = float.Parse(tokens[i++]);
        _ti = float.Parse(tokens[i++]);
        _texp = float.Parse(tokens[i++]);
        _tval = float.Parse(tokens[i++]);
        _t0 = float.Parse(tokens[i++]);
        _t1 = float.Parse(tokens[i++]);                        
        _hrMag = float.Parse(tokens[i++]);
        _hfMag = float.Parse(tokens[i++]);        
        _squashMag = float.Parse(tokens[i++]);
        _wbMag = float.Parse(tokens[i++]);
        _wxMag = float.Parse(tokens[i++]);
        _wtMag = float.Parse(tokens[i++]);
        _wfMag = float.Parse(tokens[i++]);
        _etMag = float.Parse(tokens[i++]);        
        _efMag = float.Parse(tokens[i++]);
        _dMag = float.Parse(tokens[i++]);
        _trMag = float.Parse(tokens[i++]);
        _tfMag = float.Parse(tokens[i++]);
        _encSpr0 = float.Parse(tokens[i++]);
        _sinRis0 = float.Parse(tokens[i++]);
        _retAdv0 = float.Parse(tokens[i++]);
        _encSpr1 = float.Parse(tokens[i++]);
        _sinRis1 = float.Parse(tokens[i++]);
        _retAdv1 = float.Parse(tokens[i++]);
        _continuity = float.Parse(tokens[i++]);
        _bias = float.Parse(tokens[i++]);       
        _arm[0].x = float.Parse(tokens[i++]);
        _arm[0].y = float.Parse(tokens[i++]);
        _arm[0].z = float.Parse(tokens[i++]);
        _arm[1].x = float.Parse(tokens[i++]);
        _arm[1].y = float.Parse(tokens[i++]);
        _arm[1].z = float.Parse(tokens[i++]);

        sr.Close();

    }

    void RecordValuesDrives() {
   
        string fileName = "drivesSusan.txt";     
        if(!File.Exists(fileName)){
            StreamWriter sw = new StreamWriter(fileName);

            sw.WriteLine("DriveInd\tSpeed\tv0\tv1\tti\ttexp\ttval\tcontinuity\tbias\tt0\tt1\ttr\ttf\thr\thf\tsquash\twb\twx\twt\twf\tet\td\tef\tencSpr0\tsinRis0\tretAdv0tencSpr1\tsinRis1\tretAdv1\tarmLX\tarmLY\tarmLZ\tarmRX\tarmRY\tarmRZ");
                
            for (int j = 0; j < 32; j++) { // 32 drives
                sw.WriteLine(j + "\t0.0000\t0.0000\t0.0000\t0.0000\t0.0000\t0.0000\t0.0000\t0.0000\t0.0000\t0.0000\t0.0000\t0.0000\t0.0000\t0.0000\t0.0000\t0.0000\t0.0000\t0.0000\t0.0000\t0.0000\t0.0000\t0.0000\t0.0000\t0.0000\t0.0000\t0.0000\t0.0000\t0.0000\t0.0000\t0.0000\t0.0000\t0.0000\t0.0000\t0.0000");
            }

            sw.Close();
        }

        string[] content = File.ReadAllLines(fileName);


        content[_driveInd + 1] = string.Format(_driveInd + "\t{0:0.0000}\t{1:0.0000}\t{2:0.0000}\t{3:0.0000}\t{4:0.0000}\t{5:0.0000}\t{6:0.0000}\t{7:0.0000}\t{8:0.0000}\t{9:0.0000}\t{10:0.0000}\t{11:0.0000}\t{12:0.0000}\t{13:0.0000}\t{14:0.0000}\t{15:0.0000}\t{16:0.0000}\t{17:0.0000}\t{18:0.0000}\t{19:0.0000}\t{20:0.0000}\t{21:0.0000}\t{22:0.0000}\t{23:0.0000}\t{24:0.0000}\t{25:0.0000}\t{26:0.0000}\t{27:0.0000}\t{28:0.0000}\t{29:0.0000}\t{30:0.0000}\t{31:0.0000}\t{32:0.0000}\t{33:0.0000}",
                                _speed, _v0, _v1, _ti, _texp, _tval, _continuity, _bias, _t0, _t1, _trMag, _tfMag, _hrMag, _hfMag, _squashMag, _wbMag, _wxMag, _wtMag, _wfMag, _etMag, _dMag, _efMag, _encSpr0, _sinRis0, _retAdv0, _encSpr1, _sinRis1, _retAdv1);
        
        
        using(StreamWriter sw = new StreamWriter(fileName)) {
            for (int i = 0; i < content.Length; i++)
                sw.WriteLine(content[i]);
        }
                        
      
    }

    void RecordValuesShapes() {

        string fileName = "shapesSusan.txt";
        
        if (!File.Exists(fileName)) {
            StreamWriter sw = new StreamWriter(fileName);

            sw.WriteLine("userId\tshapeInd\tHead\tNeck\tSpine\tSpine1\tShouldersX\tShouldersY\tShouldersZ\tClaviclesX\tClaviclesY\tClaviclesZ\tPelvisLX\tPelvisRX\tPelvisY\tPelvisZ\tKneesX\tHipsX\tToesX\tSpineLength");

            for (int j = 0; j < 6; j++) { // 6 shapes
                sw.WriteLine(j + "\t0.0000\t0.0000\t0.0000\t0.0000\t0.0000\t0.0000\t0.0000\t0.0000\t0.0000\t0.0000\t0.0000\t0.0000\t0.0000\t0.0000\t0.0000\t0.0000\t0.0000\t0.0000\t0.0000");
            }

            sw.Close();
        }

        string[] content = File.ReadAllLines(fileName);

        
        content[_shapeInd + 1] = string.Format("susand948\t" + _shapeInd + "\t{0:0.0000}\t{1:0.0000}\t{2:0.0000}\t{3:0.0000}\t{4:0.0000}\t{5:0.0000}\t{6:0.0000}\t{7:0.0000}\t{8:0.0000}\t{9:0.0000}\t{10:0.0000}\t{11:0.0000}\t{12:0.0000}\t{13:0.0000}\t{14:0.0000}\t{15:0.0000}\t{16:0.0000}\t{17:0.0000}",
                                _head.x, _neck.x, _spine.x, _spine1.x, _shoulders.x, _shoulders.y, _shoulders.z, _clavicles.x, _clavicles.y, _clavicles.z, _pelvisL.x, _pelvisR.x, _pelvisL.y, _pelvisL.z, _knees.x, _hips.x, _toes.x, _spineLength);


        using (StreamWriter sw = new StreamWriter(fileName)) {
            for (int i = 0; i < content.Length; i++)
                sw.WriteLine(content[i]);
        }


    }
    
}		

