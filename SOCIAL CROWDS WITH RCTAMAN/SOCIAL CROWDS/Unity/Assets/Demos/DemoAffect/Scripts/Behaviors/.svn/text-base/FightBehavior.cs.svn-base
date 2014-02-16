using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class FightBehavior : MonoBehaviour {
	public GameObject opponent;
	bool isOver = false;
	
	Appraisal appraisal;
	AgentComponent agentComponent;
	AgentComponent oAgentComponent;
	public float beginTime;
	public float endTime;
    public List<GameObject> watchers = new List<GameObject>();
	
	public bool IsOver {
		get {
			return isOver;
		}
		set {
			isOver = value;
		}
	}	

	public void Init(GameObject o) {
        appraisal = GetComponent<Appraisal>();
        agentComponent = GetComponent<AgentComponent>();
        beginTime = Time.time;
        InitAppraisalStatus();
	
		opponent = o;
		oAgentComponent = opponent.GetComponent<AgentComponent>();
        
        GetComponent<SteeringController>().orientationBehavior = OrientationBehavior.LookAtTarget; //rotation is updated according to opponent's direction
	}
	
	void Update () {			
		
		if(agentComponent.IsWounded () || oAgentComponent.IsWounded()) {            
			UpdateAppraisalStatus();
            endTime = Time.time;
            FinishFight();            
		}
		else {			
            if(oAgentComponent.role == (int)RoleName.Police)
			    agentComponent.AddDamage(0.1f); //extra damage if opponent is a police
            else
                agentComponent.AddDamage(0.05f);

            agentComponent.SteerTo(opponent.transform.position);
			transform.rotation = Quaternion.LookRotation(opponent.transform.position - transform.position, Vector3.up);							

             //Send message to others to watch this fight        
            foreach(GameObject c in agentComponent.collidingAgents) {
                if (c.GetComponent<AgentComponent>().role == (int)RoleName.Protester && c.GetComponent<AgentComponent>().IsVisible(this.gameObject, Mathf.PI / 2f) 
                    && c.GetComponent<AgentComponent>().IsFighting() == false &&  c.GetComponent<ProtesterBehavior>().isWatchingFight == false) {            
                    if(c.GetComponent<AffectComponent>().personality[(int)OCEAN.E] > 0){  //intervene if assertive
                        c.GetComponent<AgentComponent>().SteerTo((transform.position + opponent.transform.position) / 2f);                                    
                        watchers.Add(c);
                        c.GetComponent<ProtesterBehavior>().isWatchingFight = true;
                    }
                    else if(c.GetComponent<AffectComponent>().personality[(int)OCEAN.O] > 0) { //watch if curious
                        c.GetComponent<SteeringController>().Stop(true);
                        c.GetComponent<AgentComponent>().Watch((transform.position + opponent.transform.position) / 2f);                     
                        watchers.Add(c);
                        c.GetComponent<ProtesterBehavior>().isWatchingFight = true;
                    }
                }
            }
		}
		
       
	}	
	
	void InitAppraisalStatus() {
		
		//General distress
		appraisal.AddGoal("fight", 0.2f, AppDef.Displeased, AppDef.ConsequenceForSelf, AppDef.ProspectIrrelevant);
				
		//Fear of losing
		appraisal.AddGoal("fight",  0.1f, AppDef.Displeased, AppDef.ConsequenceForSelf, AppDef.ProspectRelevant, AppDef.Unconfirmed);		
		
		//Hope to win
		appraisal.AddGoal("fight",  0.1f, AppDef.Pleased, AppDef.ConsequenceForSelf, AppDef.ProspectRelevant, AppDef.Unconfirmed);
		
		//about opponent
		appraisal.AddStandard(0.8f, AppDef.Disapproving, AppDef.FocusingOnOther, opponent);		
        
	}
	
	
	void UpdateAppraisalStatus() {
		if(agentComponent.IsWounded()) {
			//Resentment to opponent
			appraisal.AddGoal("fight", 0.5f, AppDef.Displeased, AppDef.ConsequenceForOther, opponent, AppDef.DesirableForOther);
					
			//Change hope to disappointment			
            appraisal.RemoveGoal("fight", AppDef.Pleased, AppDef.ConsequenceForSelf, AppDef.ProspectRelevant, AppDef.Unconfirmed);            
			appraisal.AddGoal("fight", 0.5f, AppDef.Pleased, AppDef.ConsequenceForSelf, AppDef.ProspectRelevant, AppDef.Disconfirmed);
					
			//Change fear to fearsConfirmed
			appraisal.RemoveGoal("fight", AppDef.Displeased, AppDef.ConsequenceForSelf, AppDef.ProspectRelevant, AppDef.Unconfirmed);
			appraisal.AddGoal("fight",0.5f, AppDef.Displeased, AppDef.ConsequenceForSelf, AppDef.ProspectRelevant, AppDef.Confirmed);
			
					
		}
		if(oAgentComponent.IsWounded ()) { //If opponent is also wounded
			//Gloating to opponent
			appraisal.AddGoal("fight", 0.2f,  AppDef.Pleased, AppDef.ConsequenceForOther,opponent, AppDef.UndesirableForOther);
						
			//Change hope to satisfaction
			appraisal.RemoveGoal("fight", AppDef.Pleased, AppDef.ConsequenceForSelf, AppDef.ProspectRelevant, AppDef.Unconfirmed);
			appraisal.AddGoal("fight", 0.5f, AppDef.Pleased, AppDef.ConsequenceForSelf, AppDef.ProspectRelevant, AppDef.Confirmed);
		
            //Change fear to relief
			appraisal.RemoveGoal("fight", AppDef.Displeased, AppDef.ConsequenceForSelf, AppDef.ProspectRelevant, AppDef.Unconfirmed);
			appraisal.AddGoal("fight",0.2f, AppDef.Displeased, AppDef.ConsequenceForSelf, AppDef.ProspectRelevant, AppDef.Disconfirmed);
			
		}			
	}

    //Update other person's goals about me if they have standards about me
    public void UpdateAppraisalStatusOfOther(GameObject other) {        
        foreach(Standard s in other.GetComponent<Appraisal>().standards) {
            if(s.subject!= null && (s.subject.Equals(this.gameObject) || s.subject.Equals(transform.parent.gameObject))) {            
                if (s.approving)  {                    
                    if (agentComponent.IsWounded()) {
                        //Pity for this agent
                        other.GetComponent<Appraisal>().AddGoal("fight", 0.2f, AppDef.Displeased, AppDef.ConsequenceForOther, this.gameObject, AppDef.UndesirableForOther);
                        //Resentment to the opponent
                        other.GetComponent<Appraisal>().AddGoal("fight", 0.2f, AppDef.Displeased, AppDef.ConsequenceForOther, opponent, AppDef.DesirableForOther);
                    }
                    if (oAgentComponent.IsWounded()) {
                        //Happy-for  this agent
                        other.GetComponent<Appraisal>().AddGoal("fight", 0.2f, AppDef.Pleased, AppDef.ConsequenceForOther, this.gameObject, AppDef.DesirableForOther);
                        //Gloating to the opponent
                        other.GetComponent<Appraisal>().AddGoal("fight", 0.2f, AppDef.Pleased, AppDef.ConsequenceForOther, opponent, AppDef.UndesirableForOther);
                    }
                }
                else {//disapproving
                    if (agentComponent.IsWounded()) {
                        //Gloating for this agent
                        other.GetComponent<Appraisal>().AddGoal("fight", 0.2f, AppDef.Pleased, AppDef.ConsequenceForOther, this.gameObject, AppDef.UndesirableForOther);
                        //Happy-for  the opponent
                        other.GetComponent<Appraisal>().AddGoal("fight", 0.2f, AppDef.Pleased, AppDef.ConsequenceForOther, opponent, AppDef.DesirableForOther);
                    }
                    if (oAgentComponent.IsWounded()) {
                        //Resentment for this agent
                        other.GetComponent<Appraisal>().AddGoal("fight", 0.2f, AppDef.Displeased, AppDef.ConsequenceForOther, this.gameObject, AppDef.DesirableForOther);
                        //Pity for the opponent
                        other.GetComponent<Appraisal>().AddGoal("fight", 0.2f, AppDef.Displeased, AppDef.ConsequenceForOther, opponent, AppDef.UndesirableForOther);
                    }
                }
            }
        }
            
   }
   	
	public void FinishFight() {

        if (agentComponent.IsWounded())
            GetComponent<AgentComponent>().IncreaseSpeed(10);
            //GetComponent<AgentComponent>().IncreasePanic();

		//Remove related goals and standards
		//appraisal.RemoveGoal ("fight");
		//appraisal.RemoveStandard (opponent);

         GetComponent<SteeringController>().orientationBehavior = OrientationBehavior.LookForward;//rotation is updated by steeringcontroller




        foreach (GameObject c in watchers) {
            UpdateAppraisalStatusOfOther(c);
            c.GetComponent<ProtesterBehavior>().isWatchingFight = false;
        }

        agentComponent.timeLastFight = Time.time;
        DestroyImmediate(this);
        //IsOver = true;
        
	}
	

		
}
