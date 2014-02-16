using UnityEditor;
using System.Collections;
using UnityEngine;

[CustomEditor(typeof(GroupBuilder))]
public class GroupBuilderEditor:Editor{
	
	GroupBuilder groupBuilder;
	
	static float[] sliderMean = {0f, 0f, 0f, 0f, 0f};
	static float[] sliderStd = {0f, 0f, 0f, 0f, 0f};

    static float sliderRectX;
    static float sliderRectZ;
    
	float minMean = -1.0f;
	float maxMean = 1.0f;
	float minStd = 0.0f;
	float maxStd = 1.0f;

    static bool isLocomotion = false;
	
	void OnEnable() {
		 groupBuilder = target as GroupBuilder;                  
	}
	 
	public override void OnInspectorGUI () {
		
		GUILayout.Label ("Personality Settings", EditorStyles.largeLabel);		
		
		EditorGUILayout.BeginHorizontal ();
		GUILayout.Label("Mean", EditorStyles.boldLabel);
		GUILayout.Label("StdDev", EditorStyles.boldLabel);
		EditorGUILayout.EndHorizontal ();		
	
		EditorGUILayout.BeginHorizontal();
		sliderMean[0] = EditorGUILayout.Slider ("Openness", sliderMean[0], minMean, maxMean);
		sliderStd[0] = EditorGUILayout.Slider ("Openness", sliderStd[0], minStd,maxStd);
		EditorGUILayout.EndHorizontal ();
		
		EditorGUILayout.BeginHorizontal();
		sliderMean[1] = EditorGUILayout.Slider("Concscientiousness", sliderMean[1], minMean, maxMean);
		sliderStd[1] = EditorGUILayout.Slider ("Concscientiousness", sliderStd[1], minStd,maxStd);		
		EditorGUILayout.EndHorizontal ();
		
		EditorGUILayout.BeginHorizontal();
		sliderMean[2] = EditorGUILayout.Slider("Extroversion", sliderMean[2], minMean, maxMean);
		sliderStd[2] = EditorGUILayout.Slider("Extroversion", sliderStd[2], minStd,maxStd);
		EditorGUILayout.EndHorizontal ();
		
		EditorGUILayout.BeginHorizontal();
		sliderMean[3] = EditorGUILayout.Slider ("Agreeableness", sliderMean[3], minMean, maxMean);
		sliderStd[3] = EditorGUILayout.Slider ("Agreeableness", sliderStd[3], minStd,maxStd);
		EditorGUILayout.EndHorizontal ();
		
		EditorGUILayout.BeginHorizontal();
		sliderMean[4] = EditorGUILayout.Slider ("Neuroticism", sliderMean[4], minMean, maxMean);
		sliderStd[4] = EditorGUILayout.Slider ("Neuroticism", sliderStd[4], minStd,maxStd);
		EditorGUILayout.EndHorizontal ();
		
		if(GUILayout.Button("Update Personality", GUILayout.ExpandWidth(false))) 	
			groupBuilder.UpdatePersonalityAndBehavior(sliderMean, sliderStd);
        
        EditorGUILayout.Separator();
		
        if (GUILayout.Button("Assign Random Personality", GUILayout.ExpandWidth(false)))  {
            float[] persMean = { 0f, 0f, 0f, 0f, 0f };
            float[] persStd = { 1f, 1f, 1f, 1f, 1f };
            groupBuilder.UpdatePersonalityAndBehavior(persMean, persStd);
        }

        EditorGUILayout.Separator();
        EditorGUILayout.Separator();
        

        EditorGUILayout.BeginHorizontal();
        sliderRectX = EditorGUILayout.Slider(" X", sliderRectX, 0f, 80f);
        sliderRectZ = EditorGUILayout.Slider("Z", sliderRectZ, 0f, 80f);        
        EditorGUILayout.EndHorizontal();
        if(GUILayout.Button("Update region", GUILayout.ExpandWidth(false)))
            groupBuilder.UpdateRegion(sliderRectX, sliderRectZ);

  
        EditorUtility.SetDirty(target);
	}			 
	
	
}
	