using UnityEngine;
using System.Collections;
using UnityEditor;

[CustomEditor(typeof(AffectComponent))]
public class AffectEditor : Editor{
	
	AffectComponent affect;
	
	void OnEnable(){
		affect = target as AffectComponent;
	
	}
    
	
	public override void OnInspectorGUI () {
		
		
		//Personality is editable
		GUILayout.Label("Personality", EditorStyles.boldLabel);
		affect.personality[(int)OCEAN.O] = EditorGUILayout.Slider ("Openness", affect.personality[(int)OCEAN.O], -1f, 1f);
		affect.personality[(int)OCEAN.C] = EditorGUILayout.Slider ("Conscientiousness", affect.personality[(int)OCEAN.C], -1f, 1f);
		affect.personality[(int)OCEAN.E] = EditorGUILayout.Slider ("Extroversion", affect.personality[(int)OCEAN.E], -1f, 1f);
		affect.personality[(int)OCEAN.A] = EditorGUILayout.Slider ("Agreeableness", affect.personality[(int)OCEAN.A], -1f, 1f);
		affect.personality[(int)OCEAN.N] = EditorGUILayout.Slider ("Neuroticism", affect.personality[(int)OCEAN.N], -1f, 1f);
		affect.UpdatePersonalityDependents();
		EditorGUILayout.Space ();
		
		
		//Mood is not editable
		GUILayout.Label("Mood", EditorStyles.boldLabel);
		
		EditorGUILayout.FloatField("Pleasure", affect.mood[(int)PAD.P],GUILayout.ExpandWidth(true));
		EditorGUILayout.FloatField("Arousal", affect.mood[(int)PAD.A],GUILayout.ExpandWidth(true));
		EditorGUILayout.FloatField("Dominance", affect.mood[(int)PAD.D],GUILayout.ExpandWidth(true));
		
		EditorGUILayout.Space ();		
		EditorGUILayout.BeginHorizontal();
		GUILayout.Label("Current mood: "+ affect.GetMoodName(affect.GetCurMoodOctant()),EditorStyles.largeLabel);
		GUILayout.Label("Default mood: " + affect.GetMoodName(affect.GetDefaultMoodOctant()),EditorStyles.largeLabel);
		EditorGUILayout.EndHorizontal();
		EditorGUILayout.Space ();
		
		
		//Emotion is not editable
		GUILayout.Label("Emotion", EditorStyles.boldLabel);
		
		EditorGUILayout.BeginHorizontal();
		EditorGUILayout.FloatField("Admiration", affect.emotion[(int)EType.Admiration],GUILayout.ExpandWidth(true));
		GUILayout.Label(" " + affect.lambdaE[(int)EType.Admiration].Dose);
			
		EditorGUILayout.EndHorizontal();
		
		EditorGUILayout.BeginHorizontal();
		EditorGUILayout.FloatField("Anger", affect.emotion[(int)EType.Anger],GUILayout.ExpandWidth(true));
		GUILayout.Label(" " + affect.lambdaE[(int)EType.Anger].Dose);
		EditorGUILayout.EndHorizontal();
		
		EditorGUILayout.BeginHorizontal();
		EditorGUILayout.FloatField("Disappointment", affect.emotion[(int)EType.Disappointment],GUILayout.ExpandWidth(true));
		GUILayout.Label(" " + affect.lambdaE[(int)EType.Disappointment].Dose);
		EditorGUILayout.EndHorizontal();
		
		EditorGUILayout.BeginHorizontal();
		EditorGUILayout.FloatField("Distress", affect.emotion[(int)EType.Distress],GUILayout.ExpandWidth(true));
		GUILayout.Label(" " + affect.lambdaE[(int)EType.Distress].Dose);
		EditorGUILayout.EndHorizontal();
		
		EditorGUILayout.BeginHorizontal();
		EditorGUILayout.FloatField("Fear", affect.emotion[(int)EType.Fear],GUILayout.ExpandWidth(true));
		GUILayout.Label(" " + affect.lambdaE[(int)EType.Fear].Dose);
		EditorGUILayout.EndHorizontal();
		
		EditorGUILayout.BeginHorizontal();
		EditorGUILayout.FloatField("FearsConfirmed", affect.emotion[(int)EType.FearsConfirmed],GUILayout.ExpandWidth(true));
		GUILayout.Label(" " + affect.lambdaE[(int)EType.FearsConfirmed].Dose);
		EditorGUILayout.EndHorizontal();
		
		EditorGUILayout.BeginHorizontal();
		EditorGUILayout.FloatField("Gloating", affect.emotion[(int)EType.Gloating],GUILayout.ExpandWidth(true));
		GUILayout.Label(" " + affect.lambdaE[(int)EType.Gloating].Dose);
		EditorGUILayout.EndHorizontal();
		
		EditorGUILayout.BeginHorizontal();
		EditorGUILayout.FloatField("Gratification", affect.emotion[(int)EType.Gratification],GUILayout.ExpandWidth(true));
		GUILayout.Label(" " + affect.lambdaE[(int)EType.Gratification].Dose);
		EditorGUILayout.EndHorizontal();
		
		EditorGUILayout.BeginHorizontal();
		EditorGUILayout.FloatField("Gratitude", affect.emotion[(int)EType.Gratitude],GUILayout.ExpandWidth(true));
		GUILayout.Label(" " + affect.lambdaE[(int)EType.Gratitude].Dose);
		EditorGUILayout.EndHorizontal();
		
		EditorGUILayout.BeginHorizontal();
		EditorGUILayout.FloatField("HappyFor", affect.emotion[(int)EType.HappyFor],GUILayout.ExpandWidth(true));
		GUILayout.Label(" " + affect.lambdaE[(int)EType.HappyFor].Dose);
		EditorGUILayout.EndHorizontal();
		
		EditorGUILayout.BeginHorizontal();
		EditorGUILayout.FloatField("Hate", affect.emotion[(int)EType.Hate],GUILayout.ExpandWidth(true));
		GUILayout.Label(" " + affect.lambdaE[(int)EType.Hate].Dose);
		EditorGUILayout.EndHorizontal();
		
		EditorGUILayout.BeginHorizontal();
		EditorGUILayout.FloatField("Hope", affect.emotion[(int)EType.Hope],GUILayout.ExpandWidth(true));
		GUILayout.Label(" " + affect.lambdaE[(int)EType.Hope].Dose);
		EditorGUILayout.EndHorizontal();
		
		EditorGUILayout.BeginHorizontal();
		EditorGUILayout.FloatField("Joy", affect.emotion[(int)EType.Joy],GUILayout.ExpandWidth(true));
		GUILayout.Label(" " + affect.lambdaE[(int)EType.Joy].Dose);
		EditorGUILayout.EndHorizontal();
		
		EditorGUILayout.BeginHorizontal();
		EditorGUILayout.FloatField("Love", affect.emotion[(int)EType.Love],GUILayout.ExpandWidth(true));
		GUILayout.Label(" " + affect.lambdaE[(int)EType.Love].Dose);
		EditorGUILayout.EndHorizontal();
		
		EditorGUILayout.BeginHorizontal();
		EditorGUILayout.FloatField("Pity", affect.emotion[(int)EType.Pity],GUILayout.ExpandWidth(true));
		GUILayout.Label(" " + affect.lambdaE[(int)EType.Pity].Dose);
		EditorGUILayout.EndHorizontal();
		
		EditorGUILayout.BeginHorizontal();
		EditorGUILayout.FloatField("Pride", affect.emotion[(int)EType.Pride],GUILayout.ExpandWidth(true));
		GUILayout.Label(" " + affect.lambdaE[(int)EType.Pride].Dose);
		EditorGUILayout.EndHorizontal();
		
		EditorGUILayout.BeginHorizontal();
		EditorGUILayout.FloatField("Relief", affect.emotion[(int)EType.Relief],GUILayout.ExpandWidth(true));
		GUILayout.Label(" " + affect.lambdaE[(int)EType.Relief].Dose);
		EditorGUILayout.EndHorizontal();
		
		EditorGUILayout.BeginHorizontal();
		EditorGUILayout.FloatField("Remorse", affect.emotion[(int)EType.Remorse],GUILayout.ExpandWidth(true));
		GUILayout.Label(" " + affect.lambdaE[(int)EType.Remorse].Dose);
		EditorGUILayout.EndHorizontal();
		
		EditorGUILayout.BeginHorizontal();
		EditorGUILayout.FloatField("Reproach", affect.emotion[(int)EType.Reproach],GUILayout.ExpandWidth(true));
		GUILayout.Label(" " + affect.lambdaE[(int)EType.Reproach].Dose);
		EditorGUILayout.EndHorizontal();
		
		EditorGUILayout.BeginHorizontal();
		EditorGUILayout.FloatField("Resentment", affect.emotion[(int)EType.Resentment],GUILayout.ExpandWidth(true));
		GUILayout.Label(" " + affect.lambdaE[(int)EType.Resentment].Dose);
		EditorGUILayout.EndHorizontal();
		
		EditorGUILayout.BeginHorizontal();
		EditorGUILayout.FloatField("Satisfaction", affect.emotion[(int)EType.Satisfaction],GUILayout.ExpandWidth(true));		
		GUILayout.Label(" " + affect.lambdaE[(int)EType.Satisfaction].Dose);
		EditorGUILayout.EndHorizontal();
		
		EditorGUILayout.BeginHorizontal();
		EditorGUILayout.FloatField("Shame", affect.emotion[(int)EType.Shame],GUILayout.ExpandWidth(true));
		GUILayout.Label(" " + affect.lambdaE[(int)EType.Shame].Dose);
		EditorGUILayout.EndHorizontal();
		
		
		
	
		GUILayout.Label("Contagion", EditorStyles.boldLabel);
		
		//EditorGUILayout.BeginHorizontal();
		//EditorGUILayout.EndHorizontal();
	}
}
