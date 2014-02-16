using UnityEngine;
using System.Collections;
using System.Collections.Generic;

using System;

public enum OCEAN
{
	O, //Openness,
	C, //Conscientiousness,
	E, //Extroversion,
	A, //Agreeableness,
	N  //Neuroticism
	
}

public enum PAD
{
	P, //Pleasure
	A, //Arousal
	D  //Dominance
}

public enum EType
{
	Admiration, //0
	Anger, //1
	Disappointment, //2
	Distress, //3
	Fear, //4
	FearsConfirmed, //5
	Gloating, //6
	Gratification, //7
	Gratitude, //8
	HappyFor, //9
	Hate, //10
	Hope, //11
	Joy, //12
	Love, //13
	Pity, //14
	Pride, //15
	Relief, //16
	Remorse, //17
	Reproach, //18
	Resentment, //19
	Satisfaction, //20
	Shame //21
}
public enum MoodType:int
{
	Exuberant,
	Bored,
	Dependent,
	Disdainful,
	Relaxed,
	Anxious,
	Docile,
	Hostile
}



public class AffectComponent : MonoBehaviour {
	
	public float [] personality = new []{0.0f,0.0f,0.0f,0.0f,0.0f};
    public Vector3 mood = Vector3.zero;
	public float [] emotion = new[] {0.0f,0.0f,0.0f,0.0f,0.0f,0.0f,0.0f,0.0f,0.0f,0.0f,0.0f,0.0f,0.0f,0.0f,0.0f,0.0f,0.0f,0.0f,0.0f,0.0f,0.0f,0.0f};
	private Vector3[] pad = new Vector3[22];		
	public Vector3 defaultMood = Vector3.zero;
	[SerializeField]
	public Contagion[] lambdaE = new Contagion[22];

	
	public void Awake() {
        //Alloc memory for contagion
        lambdaE = new Contagion[22];
        for (int i = 0; i < lambdaE.Length; i++)
            lambdaE[i] = new Contagion();
        
        InitPad();
        InitEmotionContagion();
        InitDefaultMood();
        
	}
		
	public void Restart() {
        //Update emotion and mood (personality updated by user)
        for (int i = 0; i < emotion.Length; i++)
            emotion[i] = 0f;
        mood = new Vector3(0, 0, 0);

        for (int i = 0; i < lambdaE.Length; i++)
            lambdaE[i].Dose = 0f;
    }

	public void UpdatePersonality(float[] persMean, float[] persStd) {
	
		for(int i=0; i< personality.Length; i++) {		
			ComputePersonality(i, persMean[i], persStd[i]);
		}
			
		UpdatePersonalityDependents();
	}
	
	public void UpdatePersonalityDependents()
	{
		InitEmotionContagion (); //Depends on personality
		InitDefaultMood (); //Depends on personality
	
	}
	
	
	public void InitPad() {
	
		pad[(int)EType.Admiration].x= 0.5f; 		pad[(int)EType.Admiration].y = 0.3f;  		pad[(int)EType.Admiration].z =-0.2f;
		pad[(int)EType.Anger].x = -0.51f; 			pad[(int)EType.Anger].y = 0.59f;  			pad[(int)EType.Anger].z = 0.25f;
		pad[(int)EType.Disappointment].x = -0.3f;	pad[(int)EType.Disappointment].y = 0.1f; 	pad[(int)EType.Disappointment].z = -0.4f;
		pad[(int)EType.Distress].x = -0.4f;			pad[(int)EType.Distress].y = -0.2f;  		pad[(int)EType.Distress].z = -0.5f;
		pad[(int)EType.Fear].x = -0.64f;			pad[(int)EType.Fear].y = 0.6f;  			pad[(int)EType.Fear].z = -0.43f;
		pad[(int)EType.FearsConfirmed].x = -0.5f;	pad[(int)EType.FearsConfirmed].y = -0.3f;	pad[(int)EType.FearsConfirmed].z = -0.7f;
		pad[(int)EType.Gloating].x = 0.3f;			pad[(int)EType.Gloating].y = -0.3f;  		pad[(int)EType.Gloating].z = -0.1f;	
		pad[(int)EType.Gratification].x = 0.6f;		pad[(int)EType.Gratification].y = 0.5f;  	pad[(int)EType.Gratification].z = 0.4f;	
		pad[(int)EType.Gratitude].x = 0.4f; 		pad[(int)EType.Gratitude].y = 0.2f; 		pad[(int)EType.Gratitude].z = -0.3f;	
		pad[(int)EType.HappyFor].x = 0.4f;			pad[(int)EType.HappyFor].y = 0.2f; 			pad[(int)EType.HappyFor].z = 0.2f;	
		pad[(int)EType.Hate].x = -0.6f; 			pad[(int)EType.Hate].y = 0.6f;  			pad[(int)EType.Hate].z = 0.3f;		
		pad[(int)EType.Hope].x = 0.2f; 				pad[(int)EType.Hope].y = 0.2f; 				pad[(int)EType.Hope].z = -0.1f;		
		pad[(int)EType.Joy].x = 0.4f; 				pad[(int)EType.Joy].y = 0.2f;  				pad[(int)EType.Joy].z = 0.1f;	
		pad[(int)EType.Love].x = 0.3f;		 		pad[(int)EType.Love].y = 0.1f;  			pad[(int)EType.Love].z = 0.2f;	
		pad[(int)EType.Pity].x = -0.4f; 			pad[(int)EType.Pity].y = -0.2f;  			pad[(int)EType.Pity].z = -0.5f;	
		pad[(int)EType.Pride].x = 0.4f; 			pad[(int)EType.Pride].y = 0.3f;  			pad[(int)EType.Pride].z = 0.3f;		
		pad[(int)EType.Relief].x = 0.2f; 			pad[(int)EType.Relief].y = -0.3f;  			pad[(int)EType.Relief].z = 0.4f;
		pad[(int)EType.Remorse].x = -0.3f; 			pad[(int)EType.Remorse].y = 0.1f;  			pad[(int)EType.Remorse].z = -0.6f;	
		pad[(int)EType.Reproach].x = -0.3f; 		pad[(int)EType.Reproach].y = -0.1f;  		pad[(int)EType.Reproach].z = 0.4f;	
		pad[(int)EType.Resentment].x = -0.2f; 		pad[(int)EType.Resentment].y = -0.3f;  		pad[(int)EType.Resentment].z = -0.2f;	
		pad[(int)EType.Satisfaction].x = 0.3f;		pad[(int)EType.Satisfaction].y = -0.2f;  	pad[(int)EType.Satisfaction].z = 0.4f;	
		pad[(int)EType.Shame].x = -0.3f; 			pad[(int)EType.Shame].y = 0.1f;  			pad[(int)EType.Shame].z = -0.6f;
		
	}
	
	private void InitDefaultMood() {
		
		defaultMood.x	= 0.21f * personality[(int)OCEAN.E] + 0.59f * personality[(int)OCEAN.A] + 0.19f * personality[(int)OCEAN.N];
		defaultMood.y = 0.15f * personality[(int)OCEAN.O] + 0.3f * personality[(int)OCEAN.A] + 0.57f * personality[(int)OCEAN.N];
		defaultMood.z = 0.25f * personality[(int)OCEAN.O] + 0.17f * personality[(int)OCEAN.C] +  0.6f * personality[(int)OCEAN.E] - 0.32f * personality[(int)OCEAN.A];
	}
	
	private void InitEmotionContagion() {

		//empathy  in the range [0 1]
		float empathy = ( 1f + personality[(int)OCEAN.O]*0.34f +personality[(int)OCEAN.C] *0.17f + personality[(int)OCEAN.E]*0.13f + personality[(int)OCEAN.A]*0.33f + personality[(int)OCEAN.N]*0.03f) / 2f;
						
		if(lambdaE == null) {
			Debug.Log ("Emotion contagion not initialized");
			return;
		}
		
		//dose threshold in the range [0 1], inversely correlated with empathy
		for(int i=0; i< lambdaE.Length; i++) {
				lambdaE[i].DoseThreshold = 1.0f - empathy;		
			
		}
	
	}
	
	/// <summary>
	///Emotions are contracted depending on other agents' distance and orientation 
	/// </summary>
	/// <param name="inds">
	/// Gives the indices of the affected emotions
	/// </param>
	private void ComputeEmotionContagion(List<int> inds) {
	
		//Add new doses of emotion
		for(int i = 0 ; i < inds.Count; i++) {
			lambdaE[inds[i]].AddDose();		
			
		}
			
		//Decay all contracted emotions
		for(int i = 0 ; i < lambdaE.Length; i++) {
			lambdaE[i].DecayDose();				
			
		}						
	}
	
	public void ComputePersonality (int ind, float mean, float std)  {
		
		personality[ind] = MathDefs.GaussianDist(mean,std);
		if(personality[ind] < -1f)
			personality[ind] = -1f;
		else if(personality[ind] > 1f)
			personality[ind] = 1f;
	
	}
	private void ComputeMood() {
	
		int i;
		int activeEmotionCnt;
		Vector3 emotionCenter = Vector3.zero;		
		const float pullPushSpeed = 0.2f;	
		const float decaySpeed = 0.01f;	
		Vector3 m2ec, ec2m, m2dm, ec2dm;
	
		activeEmotionCnt = 0;
		for(i=0; i< pad.Length; i++) {
		
			if(emotion[i] > 0) {
		
				emotionCenter +=  pad[i] * emotion[i];				
				activeEmotionCnt++;
				
			}			
		}
	
		m2dm = defaultMood - mood;
		
		if(activeEmotionCnt > 0) {
		
			//emotionCenter = emotionCenter / activeEmotionCnt;
			emotionCenter = emotionCenter / MathDefs.Length(emotion);
			
			m2ec = emotionCenter - mood;
			ec2m = mood - emotionCenter;
			ec2dm = defaultMood - emotionCenter;
			
			m2ec.Normalize();
			
			if(Vector3.Dot(m2ec, m2dm) > 0.0f && Vector3.Dot(ec2m, ec2dm) < 0.0f)
				mood = -pullPushSpeed * m2ec;
			else
				mood = pullPushSpeed * m2ec;			
			
			m2dm.Normalize();
	
		
		}
	
		//now decay mood to the default mood		
		mood = mood - decaySpeed * m2dm  * Time.deltaTime;
		
	
		//constrain to region -1 to 1
		
		if(mood.x > 1) mood.x = 1;
		else if(mood.x < -1) mood.x = -1;
		
		if(mood.y > 1) mood.y = 1;
		else if(mood.y < -1) mood.y = -1;
		
		if(mood.z > 1) mood.z = 1;
		else if(mood.z < -1) mood.z = -1;
		
		
		
	}

	private void ComputeEmotion(float[] eventFactor, List<int> inds) { 	
		int i;		
		float beta = (personality[(int)OCEAN.N] + 2) * 0.1f; 
		
		ComputeEmotionContagion(inds); //Updates lambda		
		for(i=0; i<emotion.Length; i++) {		
			emotion[i] += eventFactor[i] * 0.1f * Time.deltaTime; //eventfactor between 0 1
		
			if(lambdaE[i].Status == StatusType.Infected)
				emotion[i] += 0.1f * Time.deltaTime;			
		}
		
		
		//Clamp emotions to [0 1] interval
		for(i=0; i<emotion.Length; i++)	 {
			if(emotion[i] > 1f)
				emotion[i] = 1f;		
		}
		//Decay emotion
		for(i=0; i<emotion.Length; i++) {
			emotion[i] = emotion[i] - emotion[i] * beta * Time.deltaTime ;
            if(emotion[i] < 0.00001f)
				emotion[i] = 0f;			
		}
        
      
	}


    public void UpdateAffectiveState(float[] eventFactor, List<int> lambdaList)
    {		
		ComputeEmotion(eventFactor, lambdaList);		
		ComputeMood();
				
	}
	public int FindDominantEmotion() {
	
		float maxEm ;
		int	maxInd = -1;
		int i;
		
		//Find dominant emotion
		maxEm = 0;
		for(i = 0; i<emotion.Length; i++) {		
			if(emotion[i] > maxEm) {			
				maxInd = i;
				maxEm = emotion[i];				
			}
		}
		
		return maxInd;
	}
	
	public string GetEkmanEmotion() {
	
		int ind = FindDominantEmotion();
		
		if(ind == (int)EType.HappyFor || ind == (int)EType.Gloating || ind == (int)EType.Joy || ind == (int)EType.Pride || ind == (int)EType.Admiration ||
			ind == (int)EType.Love  || ind == (int)EType.Satisfaction || ind == (int)EType.Relief || ind ==  (int)EType.Gratification || 
			ind == (int)EType.Gratitude  )
			return "happy";
		else if(ind == (int)EType.Resentment || ind == (int)EType.Pity  || ind == (int)EType.Shame || ind == (int)EType.Remorse || ind == (int)EType.Disappointment 
			|| ind == (int)EType.Distress)
			return "sad";
		else if(ind == (int)EType.Anger || ind == (int)EType.Hate || ind == (int)EType.Reproach)
			return "angry";
		else if(ind == (int)EType.Fear || ind == (int)EType.FearsConfirmed)
			return "afraid";
		else  //hope or -1
			return "neutral";
	}
	
	
	
	public int GetMoodOctant(Vector3 md) {
		if (md[(int)PAD.P] >= 0 && md[(int)PAD.A] <= 0 && md[(int)PAD.D] >= 0)
			return (int)MoodType.Relaxed;
		else if (md[(int)PAD.P] < 0 && md[(int)PAD.A] > 0 && md[(int)PAD.D] < 0)
			return (int)MoodType.Anxious;
		else if(md[(int)PAD.P] > 0 && md[(int)PAD.A] > 0 && md[(int)PAD.D] > 0)
			return (int)MoodType.Exuberant;
		else if (md[(int)PAD.P] <= 0 && md[(int)PAD.A] <= 0 && md[(int)PAD.D] <= 0)
			return (int)MoodType.Bored;
		else if (md[(int)PAD.P] > 0 && md[(int)PAD.A] > 0 && md[(int)PAD.D] <= 0)
			return (int)MoodType.Dependent;
		else if (md[(int)PAD.P] <= 0 && md[(int)PAD.A] <= 0 && md[(int)PAD.D] > 0)
			return (int)MoodType.Disdainful;
		else if (md[(int)PAD.P] > 0 && md[(int)PAD.A] <= 0 && md[(int)PAD.D] <= 0)
			return (int)MoodType.Docile;
		else if (md[(int)PAD.P] <= 0 && md[(int)PAD.A] > 0 && md[(int)PAD.D] > 0)
			return (int)MoodType.Hostile;
		else
			return -1;
	}
	
	public bool IsMood(int moodType) {
		if(GetCurMoodOctant() == moodType) 
            return true;
        else 
            return false;			
	}
	public int GetCurMoodOctant() {		
		return GetMoodOctant(mood);
	}
	
	public int GetDefaultMoodOctant() {		
		return GetMoodOctant(defaultMood);
	}
		 
	public string GetMoodName(int type) {		
		switch (type) {		
			case (int)MoodType.Relaxed:
				return "Relaxed";
			case (int)MoodType.Anxious:
				return "Anxious";
			case (int)MoodType.Exuberant:
				return "Exuberant";
			case (int)MoodType.Bored:
				return "Bored";
			case (int)MoodType.Dependent:
				return "Dependent";
			case (int)MoodType.Disdainful:
				return "Disdainful";
			case (int)MoodType.Docile:
				return "Docile";
			case (int)MoodType.Hostile:
				return "Hostile";
			default:
				return "Neutral";
		}					
	}
	
}
