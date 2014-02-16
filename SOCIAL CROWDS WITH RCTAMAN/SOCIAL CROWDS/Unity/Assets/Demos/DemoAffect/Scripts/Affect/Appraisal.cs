using UnityEngine;
using System.Collections;
using System.Collections.Generic;



[System.Serializable]
public class Attitude
{
	public bool liking;
	public float weight;	 //appealingness
    public GameObject subject;
	public bool selected; //For editing purposes
}

[System.Serializable]
public class Standard
{
	public bool focusingOnSelf; //if false focusing on other is true
	public bool approving; 
	public float weight; //praiseworthiness
	public GameObject subject;
	public bool selected; //For editing purposes		
}
[System.Serializable]
public class Goal
{
	public bool consequenceForSelf; //if false consequence for other is true
	public bool desirableForOther; 
	public bool prospectRelevant;
	public int  confirmed;
	public bool pleased;
	public float weight; //desirability
	public string about; //event name
	public GameObject subject;
	public bool selected; //For editing purposes
	
}

//Appraisal definitions
public static class AppDef{
	//About goals
	public const bool Pleased = true;
	public const bool Displeased = false;
	public const bool ConsequenceForSelf = true;
	public const bool ConsequenceForOther = false;
	public const bool ProspectRelevant = true;
	public const bool ProspectIrrelevant = false;
	public const bool DesirableForOther = true;
	public const bool UndesirableForOther = false;
	public const int  Unconfirmed = 0;
	public const int  Confirmed = 1;
	public const int  Disconfirmed = 2;
	
	//About standards
	public const  bool FocusingOnSelf = true;
	public const  bool FocusingOnOther = false;
	public const bool  Approving = true;
	public const bool  Disapproving = false;
	
	//About attitudes
	public const bool  Liking = true;
	public const bool  Disliking  = false;
	
}

[System.Serializable]
public class Appraisal : MonoBehaviour {
	 [SerializeField]
	public List<Goal> goals= new List<Goal>();
	 [SerializeField]
	public List<Standard> standards = new List<Standard>();
	 [SerializeField]
	public List<Attitude> attitudes = new List<Attitude>();
	
	float[] eventFactor = new float[22];
	
    public void Restart() {
        goals.Clear();
        standards.Clear();
        attitudes.Clear();
    }
	
	public float[] ComputeEventFactor() {
		int i;

		
		for(i=0; i < eventFactor.Length; i++)
			eventFactor[i]= 0;

		foreach(Goal g in goals) {
		
			if(g.consequenceForSelf) {
			
				if(g.prospectRelevant) {
				
					if(g.confirmed == AppDef.Unconfirmed) {
					
						if(g.pleased)
							eventFactor[(int)EType.Hope] += 0.02f * g.weight ;//(1.7 * sqrt(g.expectation)) + (-0.7*g.weight);
						else
							eventFactor[(int)EType.Fear] += 0.9f * g.weight;//0.4;//2 *g.expectation * g.expectation - g.weight;
						
						
					}
					else if(g.confirmed == AppDef.Confirmed) {
				
						if(g.pleased)
							eventFactor[(int)EType.Satisfaction] += 0.7f  * g.weight ;
						else
							eventFactor[(int)EType.FearsConfirmed] += 0.8f * g.weight;
	
						//Confirmed goals are removed later
						
					
					}
					else  {//disconfirmed
					
						if(g.pleased)
							eventFactor[(int)EType.Disappointment] += 0.5f  * g.weight ;//(1.7 * sqrt(g.expectation)) + (-0.7*g.weight) * g.weight;  //hope x weight
						else
							eventFactor[(int)EType.Relief] += 0.6f  * g.weight ;//(2 * g.expectation *g.expectation -g.weight)*g.weight; //fear x weight
	
					
						//goals.Remove (g);
						//g = goals.erase(g);
	
					}
				}
				else { //prospect irrelevant
				
						if(g.pleased)
							eventFactor[(int)EType.Joy] += 0.4f * g.weight;//(1.7 * sqrt(g.expectation)) + (-0.7*g.weight);
						else
							eventFactor[(int)EType.Distress] += 0.5f * g.weight;// (2 * g.expectation * g.expectation) -g.weight;
	
						
				}
								
			}
			else { //consequences for others
			
					if(g.desirableForOther) {
					
						if(g.pleased)
							eventFactor[(int)EType.HappyFor] += 0.4f * g.weight;
						else
							eventFactor[(int)EType.Resentment] += 0.5f * g.weight;
					
					}		
					else { //undesirable for other
					
						if(g.pleased)
							eventFactor[(int)EType.Gloating] += 0.5f * g.weight;
						else
							eventFactor[(int)EType.Pity] += 0.4f * g.weight;
					}
					
			}
				
				
				
		}
		
	
		foreach(Standard s in standards) {
		
			if(s.focusingOnSelf) {
			
				if(s.approving)
					eventFactor[(int)EType.Pride] += 0.2f * s.weight;
				else
					eventFactor[(int)EType.Shame] += 0.2f *  s.weight;
			}
			else { //focusing on others		
				if(s.approving)
					eventFactor[(int)EType.Admiration] += 0.3f  * s.weight;
				else
					eventFactor[(int)EType.Reproach] += 0.5f  * s.weight;
			}
	
		}
	
		foreach (Attitude a in attitudes) {		
			if(a.liking)
				eventFactor[(int)EType.Love] += 0.02f * a.weight;
			else
				eventFactor[(int)EType.Hate] += 0.2f * a.weight;
	
		}
	
	
		
		//compound emotions
		if(eventFactor[(int)EType.Admiration] > 0 && eventFactor[(int)EType.Joy] > 0)
			eventFactor[(int)EType.Gratification] = (eventFactor[(int)EType.Admiration] + eventFactor[(int)EType.Joy])/2.0f;
	
		if(eventFactor[(int)EType.Pride] > 0 && eventFactor[(int)EType.Joy] > 0)
			eventFactor[(int)EType.Gratitude] = (eventFactor[(int)EType.Pride] + eventFactor[(int)EType.Joy])/2.0f;
	
		if(eventFactor[(int)EType.Shame] > 0 && eventFactor[(int)EType.Distress] > 0)
			eventFactor[(int)EType.Remorse] = (eventFactor[(int)EType.Shame] + eventFactor[(int)EType.Distress])/2.0f;
	
		if(eventFactor[(int)EType.Reproach] > 0 && eventFactor[(int)EType.Distress] > 0)
			eventFactor[(int)EType.Anger] = (eventFactor[(int)EType.Reproach] + eventFactor[(int)EType.Distress])/2.0f;
	
		
		//Normalize eventFactor 
		MathDefs.NormalizeElements(eventFactor, 0f, 1f);
	
		return eventFactor;
	}
	
	
	//Order: pleased, consequenceForself, prospectRelevant, {confirmed}
	//pleased, consequenceForOther, subject, desirableForOther
	public void AddGoal(string relEvent, float weight, params object[] values) {		
		
		if(values.Length < 3) {
			Debug.Log ("Missing parameter in AddGoal");
			return;
		}
				
		Goal g = new Goal();
		
		g.about = relEvent;
		g.weight = weight;
		
		g.pleased = (bool)values[0];
		
		
		g.consequenceForSelf = (bool)values[1];
		if(values[1].Equals(AppDef.ConsequenceForOther)) {
			if(values.Length < 4) {
				Debug.Log ("Missing parameter in AddGoal");
				return;
			}
			g.subject = (GameObject)values[2];
			g.desirableForOther = (bool)values[3];
			
		}
		else{ //conseqForSelf
			
			g.prospectRelevant = (bool)values[2];
			
			if(values[2].Equals(AppDef.ProspectRelevant)) {
				if(values.Length < 4) {
					Debug.Log ("Missing parameter in AddGoal");
					return;
					
				}
				else {
					g.confirmed = (int)values[3];                    
				}
			}
				
		}
        
		goals.Add(g);
			
	}
	
	//order approving, focusingonSelf, {subject}
	public void AddStandard(float weight,  bool approving, params object[] values) {		
		Standard s = new Standard();		
		s.weight = weight;
		s.approving = approving;
		
		if(values.Length < 1) {
			Debug.Log ("Missing parameter in AddStandard");
			return;
		}
			
		s.focusingOnSelf = (bool)values[0];
		
		if(values[0].Equals(AppDef.FocusingOnOther)) {
			
			if(values.Length < 1) {
				Debug.Log ("Missing parameter \"subject\" in AddStandard");
				return;
			}
			else {
				s.subject = (GameObject)values[1];	
			}
						
		}		
		standards.Add (s);				
	}
		
	
	public void AddAttitude(GameObject subject, float weight,  bool liking) {
        Attitude a = new Attitude();
        a.subject = subject;
        a.weight = weight;
        a.liking = liking;
        attitudes.Add(a);
    }
		
		
	//Remove goal about the related event with the indicated values
	//Order: pleased, consequenceForself, prospectRelevant, confirmed
    //Returns true if goal is found
	public bool RemoveGoal(string relEvent, params object[] values) {		
        bool exists = false;
		int i = 0;
		while(i < goals.Count) {
			if(relEvent.Equals("") || goals[i].about.Equals(relEvent)) {
				if(values.Length == 0){ 
					goals.Remove(goals[i]);
                    exists = true;
                }
				else if(values[0].Equals(goals[i].pleased)) {
					if(values.Length == 1) {
						goals.Remove(goals[i]);										
                        exists = true;
                    }
					else if(values[1].Equals(goals[i].consequenceForSelf)){
						if(values.Length == 2){
							goals.Remove(goals[i]);						
                            exists = true;
                        }
						else if(values[1].Equals(true) && values[2].Equals(goals[i].prospectRelevant)) { 
							if( values[2].Equals(true)) { //prospect relevant
								if(values.Length == 3) {
									goals.Remove(goals[i]);
                                    exists = true;
                                }
								else if(values[3].Equals(goals[i].confirmed)) {
										goals.Remove(goals[i]);
                                        exists = true;
                                }
								else
									i++;										
							}
							else {//prospect irrelevant
								goals.Remove(goals[i]);						
                                exists = true;
                            }
						}
						else if(values[1].Equals(false) && values[2].Equals(goals[i].desirableForOther)) {
							goals.Remove(goals[i]);
                            exists = true;
                        }
						else
							i++;
						
					}
					else
						i++;
				}
				else
					i++;
			}						
			else
				i++;
		}


        return exists;
	}
	
	//Remove standard about the related subject
	//Order: Approvingm focusingonself
    //Returns true if standard is found
	public bool  RemoveStandard(GameObject sub, params object[] values) {
        bool exists = false;
		int i = 0;
		while(i < standards.Count) {
			if(sub == null || standards[i].subject.Equals(sub) ) {
				if(values.Length == 0){					
					standards.Remove(standards[i]);
                    exists = true;
                }
				else if(values[0].Equals(standards[i].approving)) {
					if(values.Length == 1) {
						standards.Remove(standards[i]);
                        exists = true;
                    }
					else if(values[1].Equals(standards[i].focusingOnSelf)) {
						standards.Remove(standards[i]);
                        exists = true;
                    }
					else
						i++;
				}
				else
					i++;				
				
			}
								
			else
				i++;
		}
        return exists;
	}

    //Returns true if attitude is found
    public bool RemoveAttitude(GameObject sub, params object[] values) {
        bool exists = false;
		int i = 0;
		while(i < attitudes.Count) {
			if(sub == null || attitudes[i].subject.Equals(sub) ) {
                if (values.Length == 0) {
                    attitudes.Remove(attitudes[i]);
                    exists = true;
                }
                else
                    i++;
            }
            else if(values[0].Equals(attitudes[i].liking)) {
                attitudes.Remove(attitudes[i]);
                exists = true;
            }
            else
                i++;
        }

        return exists;
		        
    }

    //Return whether a standard with the approvalStatus about a subject exists
	public bool DoesStandardExist(GameObject sub, bool approvalStatus) {        
        foreach(Standard s in standards) {
            if (s.subject == sub){
                if (s.approving == approvalStatus)
                    return true;
            }                
        }
        return false; //no standard exits about the subject or the approvalStatus does not match
    }
	
	
}
