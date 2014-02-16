//#define WEBMODE


using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;

public class SelectorGUI : MonoBehaviour {


    static bool _studyMode = true;




    void Start() {
        
    }


    void OnGUI() {

        GUIStyle style = new GUIStyle();

        GUILayout.BeginArea(new Rect(220, 10, 300, 250));
        GUILayout.BeginHorizontal();
        GUI.color = Color.black;
        style.fontSize = 22;
        if (_studyMode)
            GUILayout.Label("Study 1", style);
        else
            GUILayout.Label("Study 2", style);
  
        bool newStudyMode = GUILayout.Toggle(_studyMode, "Switch study mode");
        

        if (newStudyMode == true) {
            GetComponent<LMAComparisonGUI>().enabled = true;
            GetComponent<LMAScalingGUI>().enabled = false;
            if (_studyMode == false) {
                GetComponent<LMAComparisonGUI>().Reset();
                //GetComponent<LMAScalingGUI>().ResetTransforms();
            }
            

        }
        else if(newStudyMode == false ) {
            GetComponent<LMAComparisonGUI>().enabled = false;
            GetComponent<LMAScalingGUI>().enabled = true;
            if (_studyMode == true) {
                GetComponent<LMAScalingGUI>().Reset();

                
            }
        }

        _studyMode = newStudyMode;

        GUILayout.EndHorizontal();

        GUILayout.EndArea();
    }
}