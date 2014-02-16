using UnityEditor;
using System.Collections;
using UnityEngine;
using System.Collections.Generic;

[CustomEditor(typeof(Appraisal))]

public class AppraisalEditor : Editor {
	
	Appraisal appraisal;
	//Goals
	bool pleased = false;
	bool prospectRel = false;
	bool conseqForSelf = false;
	bool desirable = false;	
	bool checkAllGoals = false;
	//Standards
	bool approving = false;
	bool focOnSelf = false;	
	bool checkAllStandards = false;
	//Attitudes
	bool liking = false;
	bool checkAllAttitudes = false;

    float weight = 0.5f; //default

	void OnEnable()
    {	
		appraisal = target as Appraisal;
				
		
	}
	 
	public override void OnInspectorGUI () {
		
		
		//Add new goals
		Goal gl = new Goal();
		
		GUILayout.Label("Goals", EditorStyles.boldLabel);
		
		
		EditorGUILayout.BeginHorizontal();	
		pleased = EditorGUILayout.Toggle("Pleased", pleased);
		gl.pleased = pleased;
		conseqForSelf = EditorGUILayout.Toggle("ConsequenceForSelf", conseqForSelf,  GUILayout.ExpandWidth(true));
		gl.consequenceForSelf = conseqForSelf;
		if(conseqForSelf == false) {			
			desirable = EditorGUILayout.Toggle("DesirableForOther", desirable,  GUILayout.ExpandWidth(true));
			gl.desirableForOther = desirable;
			
		}
		else {
			prospectRel = EditorGUILayout.Toggle("ProspectRelevant", prospectRel,  GUILayout.ExpandWidth(true));
			gl.prospectRelevant = prospectRel;			
			gl.confirmed = AppDef.Unconfirmed;
			
		}
				
		EditorGUILayout.EndHorizontal();	
		
		weight = EditorGUILayout.Slider ("weight", weight, 0f, 1f,  GUILayout.ExpandWidth(true));
		gl.weight = weight;
		
		
		if(GUILayout.Button("Add Goal", GUILayout.ExpandWidth(false)))
			appraisal.goals.Add(gl);
		
				
		if(appraisal.goals.Count  > 0) {
            EditorGUILayout.Separator();
            GUI.contentColor = Color.yellow;                        
            GUILayout.Label("Current goals", EditorStyles.boldLabel);
            checkAllGoals = EditorGUILayout.Toggle("Check all", checkAllGoals);            
        }
			
		foreach(Goal g in appraisal.goals) {
			EditorGUILayout.BeginHorizontal();			
		
			g.selected= EditorGUILayout.Toggle(g.selected, GUILayout.ExpandWidth(false));
			
			
			if(g.pleased)
				GUILayout.Label  ("Pleased");
			else
				GUILayout.Label  ("Displeased");
						
			if(g.consequenceForSelf) {
			
				GUILayout.Label ("ConsequenceForSelf");
				if(g.prospectRelevant) {
				
					GUILayout.Label ("ProspectRelevant");
					if(g.confirmed == AppDef.Confirmed)
						GUILayout.Label ("Confirmed");
					else if(g.confirmed == AppDef.Disconfirmed)
						GUILayout.Label ("Disconfirmed");
					else
						GUILayout.Label ("Unconfirmed");
			
				}
				else
					GUILayout.Label ("ProspectIrrelevant");
			}
			else {
			
				GUILayout.Label ("ConsequenceForOther");
				if(g.desirableForOther)
					GUILayout.Label ("Desirable");
				else
					GUILayout.Label ("Unesirable");
			}
			
			
			GUILayout.Label ("weight: " + g.weight);
			
			EditorGUILayout.EndHorizontal ();
			
			
			
			
		}
		
		if(checkAllGoals)
			foreach(Goal g in appraisal.goals)
				g.selected = true;
			
		
		
		if(appraisal.goals.Count > 0) {
		
			EditorGUILayout.Separator();
			
			if(GUILayout.Button("RemoveChecked", GUILayout.ExpandWidth(false))) {		
				int i = 0;		
				while(i < appraisal.goals.Count) {
					if(appraisal.goals[i].selected){
						appraisal.goals.Remove(appraisal.goals[i]);					
					}
					else
						i++;
						
				}
			
			}
		}
       
        
		EditorGUILayout.Separator();
        EditorGUILayout.Separator();
        GUI.contentColor = Color.white;

		GUILayout.Label("Standards", EditorStyles.boldLabel);
		
		Standard st = new Standard();
		
		EditorGUILayout.BeginHorizontal();	
		approving = EditorGUILayout.Toggle("Approving", approving);
		st.approving = approving;
		focOnSelf = EditorGUILayout.Toggle("Focusing on Self", focOnSelf,  GUILayout.ExpandWidth(true));
		st.focusingOnSelf = focOnSelf;
				
		EditorGUILayout.EndHorizontal();	
		
		weight = EditorGUILayout.Slider ("weight", weight, 0f, 1f,  GUILayout.ExpandWidth(true));
		st.weight = weight;
		
		
		if(GUILayout.Button("Add Standard", GUILayout.ExpandWidth(false)))
			appraisal.standards.Add(st);
		
				
		if(appraisal.standards.Count  > 0) {
			EditorGUILayout.Separator();
            GUI.contentColor = Color.yellow;
            GUILayout.Label("Current standards", EditorStyles.boldLabel);
            checkAllStandards = EditorGUILayout.Toggle("Check all", checkAllStandards);            
         }

		foreach(Standard s in appraisal.standards) {
			
			EditorGUILayout.BeginHorizontal();
			
			s.selected= EditorGUILayout.Toggle(s.selected, GUILayout.ExpandWidth(false));
			
						
			if(s.approving)
				GUILayout.Label ("Approving");
			else
				GUILayout.Label ("Disapproving");
			
			if(s.focusingOnSelf)
				GUILayout.Label ("FocusingOnSelf");
			else
				GUILayout.Label ("FocusingOnOther");
						
			
			GUILayout.Label ("weight: " + s.weight);
			EditorGUILayout.EndHorizontal ();
		}
		
		if(checkAllStandards)
			foreach(Standard s in appraisal.standards)
				s.selected = true;
			
		if(appraisal.standards.Count > 0) {
		
			EditorGUILayout.Separator();
			
			if(GUILayout.Button("RemoveChecked", GUILayout.ExpandWidth(false))) {		
				int i = 0;		
				while(i < appraisal.standards.Count) {
					if(appraisal.standards[i].selected){
						appraisal.standards.Remove(appraisal.standards[i]);					
					}
					else
						i++;
						
				}
			
			}
		}

        EditorGUILayout.Separator();
		EditorGUILayout.Separator();
        GUI.contentColor = Color.white;
		
        GUILayout.Label("Attitudes", EditorStyles.boldLabel);
		
		Attitude at = new Attitude();
		
		EditorGUILayout.BeginHorizontal();	
		liking = EditorGUILayout.Toggle("Liking", liking);
		at.liking = liking;
				
		EditorGUILayout.EndHorizontal();	
		
		weight = EditorGUILayout.Slider ("weight", weight, 0f, 1f,  GUILayout.ExpandWidth(true));
		at.weight = weight;
		
		
		if(GUILayout.Button("Add Attitude", GUILayout.ExpandWidth(false)))
			appraisal.attitudes.Add(at);


        EditorGUILayout.Separator();
        if(appraisal.attitudes.Count  > 0) {
			EditorGUILayout.Separator();
            GUI.contentColor = Color.yellow;
            GUILayout.Label("Current attitudes", EditorStyles.boldLabel);
            checkAllAttitudes = EditorGUILayout.Toggle("Check all", checkAllAttitudes);            
        }
       
		foreach(Attitude a in appraisal.attitudes) {
			EditorGUILayout.BeginHorizontal();			
			a.selected= EditorGUILayout.Toggle(a.selected, GUILayout.ExpandWidth(false));
			
			
			if(a.liking)
				GUILayout.Label ("Liking");
			else
				GUILayout.Label ("Disliking");
						
			GUILayout.Label ("weight: " + a.weight);
			
			EditorGUILayout.EndHorizontal ();
		}
	
		if(checkAllAttitudes)
			foreach(Attitude a in appraisal.attitudes)
				a.selected = true;

        GUI.contentColor = Color.white;
		
		if(appraisal.attitudes.Count > 0) {
		
			EditorGUILayout.Separator();
			
			if(GUILayout.Button("RemoveChecked", GUILayout.ExpandWidth(false))) {		
				int i = 0;		
				while(i < appraisal.attitudes.Count) {
					if(appraisal.attitudes[i].selected){
						appraisal.attitudes.Remove(appraisal.attitudes[i]);					
					}
					else
						i++;
						
				}
			
			}
		}
		
		 EditorUtility.SetDirty (target);
	}
}
