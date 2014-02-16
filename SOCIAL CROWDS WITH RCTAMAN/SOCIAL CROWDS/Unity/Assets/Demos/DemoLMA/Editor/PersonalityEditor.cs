using UnityEngine;
using UnityEditor;
using System.Collections;


public class PersonalityEditor:EditorWindow{
	
	
	static float o, c, e, a, n;

	
	
	float minVal = -1.0f;
	float maxVal = 1.0f;
	
	[MenuItem ("ADAPT/LMA/OCEAN")]
	 static void Init () {
		PersonalityEditor window = (PersonalityEditor)EditorWindow.GetWindow(typeof (PersonalityEditor));		
	
	}

	 
	public void OnGUI () {
		
		GUILayout.Label ("Personality Settings", EditorStyles.largeLabel);		
			
	
		o = EditorGUILayout.Slider ("Openness", o, minVal, maxVal);
		c = EditorGUILayout.Slider ("Conscientiousness", c, minVal, maxVal);
		e = EditorGUILayout.Slider ("Extroversion", e, minVal, maxVal);
		a = EditorGUILayout.Slider ("Agreeableness", a, minVal, maxVal);
		n = EditorGUILayout.Slider ("Neuroticism", n, minVal, maxVal);
		
	
		
				
		MapPersonalityToLMA();
			
		
		
		if(GUILayout.Button("Reset", GUILayout.ExpandWidth(false))){
			o = c = e = a = n = 0f;
			
		ResetTransforms();
		}
		
			
	}
		
	
	void ResetTransforms()
	{
		GameObject agent = GameObject.Find("AgentPrefab");	
		agent.GetComponent<ArmAnimator>().ArmC.ResetTransforms();
		agent.GetComponent<TorsoAnimator>().TorsoC.ResetTransforms();
		
	}
				 
	void MapPersonalityToLMA()
	{
		
		float space, weight, time, flow;
		float horTorso;
		GameObject agent = GameObject.Find("AgentPrefab");
		if(agent == null){		
			Debug.Log("AgentPrefab not found");
			return;
		}
		
		space = (e - o - n ) / 3f;
		
		weight = (e - a)  / 2f;
		
		time  = (e + n) / 2f;
		
		flow = (c + e - o) / 3f;
		
		
		if(space > 0){
			agent.GetComponent<ArmAnimator>().Dir = space;
			agent.GetComponent<ArmAnimator>().Ind = 0f;
		}
		else{
			agent.GetComponent<ArmAnimator>().Ind = -space;
			agent.GetComponent<ArmAnimator>().Dir = 0f;
		}
		//Weight
		if(weight > 0){
			agent.GetComponent<ArmAnimator>().Str = weight;
			agent.GetComponent<ArmAnimator>().Lgt = 0f;
		}
		else{
			agent.GetComponent<ArmAnimator>().Lgt = -weight;
			agent.GetComponent<ArmAnimator>().Str = 0f;
		}
		//Time				
		if(time > 0){
			agent.GetComponent<ArmAnimator>().Sud = time;
			agent.GetComponent<ArmAnimator>().Sus = 0f;
		}
		else{
			agent.GetComponent<ArmAnimator>().Sus = -time;
			agent.GetComponent<ArmAnimator>().Sud = 0f;
		}
		
		//Flow
		if(flow > 0){
			agent.GetComponent<ArmAnimator>().Bnd = flow;
			agent.GetComponent<ArmAnimator>().Fre = 0f;
		}
		else{
			agent.GetComponent<ArmAnimator>().Fre = -flow	;
			agent.GetComponent<ArmAnimator>().Bnd = 0;
		}
		
		
		
		//Update effort parameters
		agent.GetComponent<ArmAnimator>().Effort2LowLevel();
		
		//Torso
		/*
		if(n < 0)
			horTorso = e;
		else
			horTorso = (e - n) / 2f;
		
		if(horTorso > 0){
			agent.GetComponent<TorsoAnimator>().Spr = horTorso;
			agent.GetComponent<TorsoAnimator>().Enc = 0f;
		}
		else{
			agent.GetComponent<TorsoAnimator>().Enc = -horTorso;
			agent.GetComponent<TorsoAnimator>().Spr = 0f;
		}
			
			*/
			
		
		
	}
	
}
	