using UnityEngine;
using System.Collections;

public class ProtesterBehavior : MonoBehaviour {	
	Appraisal appraisal;
	AgentComponent agentComponent;
    AffectComponent affectComponent;
	
	public bool bannerCarrier;
	public GameObject banner;
	
	private int frameCnt = 0;  //for animation update         
    public bool isWatchingFight = false; //is the agent watching a fight. updated in fightbehavior
	
	GameObject leader;
	public GameObject Leader{
		get{ return leader; }
		set{ leader = value; }
	}
	
	// Use this for initialization
	void Start () {
		
		appraisal = GetComponent<Appraisal>();
		agentComponent = GetComponent<AgentComponent>();
        affectComponent = GetComponent<AffectComponent>();
		InitAppraisalStatus();
		
		Leader = this.transform.parent.GetComponent<LeaderComponent>().leader;

        UpdateAttractor();
        	
		if(agentComponent.id % 4 == 0) {
			bannerCarrier = true;			
			banner = (GameObject)Instantiate(UnityEngine.Resources.Load("banner"));
		}         
	}

    public void Enable() {
        if(bannerCarrier){
            Transform bT = transform.Find("Hips/Spine/Spine1/Spine2/Spine3/Spine4/Neck/RightShoulder/RightArm/RightForeArm/RightHand/RightHandRing1");
            banner.transform.position = bT.position;
            banner.transform.rotation = bT.rotation;
            banner.transform.Rotate(90, 0, 0);
        }
    }
    public void Restart() {
        InitAppraisalStatus();
        UpdateAttractor();
        frameCnt = 0;
        isWatchingFight = false; 
    }


    void Update () {        
        if(agentComponent.IsDead()){
            if(bannerCarrier) 
                 banner.SetActiveRecursively(false);
            return;
        }
        
        UpdateAction();        	
		
        if (agentComponent.IsFighting() == false) {
            frameCnt++;
            UpdateAttractor();

           // if (affectComponent.emotion[(int)EType.Anger] > 0.6f)
             //   Instantiate(UnityEngine.Resources.Load("Explosion"), transform.position, new Quaternion());

            foreach (GameObject c in agentComponent.collidingAgents) {
                if(agentComponent.IsGoodToFight(c)){
                    agentComponent.StartFight(c);
                    c.GetComponent<AgentComponent>().StartFight(this.gameObject);            
                    frameCnt = 0;
                    break;
                }
            }
            
            
        }        
	}
    void LateUpdate(){
        if (bannerCarrier) {
            Transform bT = transform.Find("Hips/Spine/Spine1/Spine2/Spine3/Spine4/Neck/RightShoulder/RightArm/RightForeArm/RightHand/RightHandRing1");
            banner.transform.position = bT.position;
            banner.transform.rotation = bT.rotation;
            banner.transform.Rotate(90, 0, 0);
            banner.transform.Rotate(0, 90, 0);
        }
    }
		 
	void UpdateAttractor() {
        if (Leader == null || Leader == this.gameObject) //if this agent is the leader himself
            agentComponent.SteerTo(agentComponent.attractor.transform.position);

        else if (Leader != null)
            agentComponent.SteerTo(Leader.transform.position);
        
		
     }
	void UpdateAction() {        
        if (agentComponent.IsFighting()) {
            agentComponent.CurrAction = "fight";
			
			if(bannerCarrier) 
				banner.SetActiveRecursively(false);				
			
		}        
		else if(bannerCarrier) 	{
			banner.SetActiveRecursively(true);
			agentComponent.CurrAction = "protest";			
		}
		else {	
		    if (frameCnt % 300 == 0) { //protesting -- update every 300 frames
                int animId;
                string exp = affectComponent.GetEkmanEmotion();
                if (exp.Equals("angry") || exp.Equals("sad"))
                    animId = MathDefs.GetRandomNumber(3);
                else if (exp.Equals("happy"))
                    animId = 0;
                else
                    animId = -1; //no action
                                  
			    if(animId == 0)
				    agentComponent.CurrAction = "clap";
			    else if(animId == 1)
				    agentComponent.CurrAction = "cheer";
			    else if(animId == 2)
                    agentComponent.CurrAction = "throw";
                else
                    agentComponent.CurrAction = "";
				    
            }
		}					
	}

    
	void InitAppraisalStatus() {	
		//Distress
		appraisal.AddGoal("protest", 0.2f, AppDef.Displeased, AppDef.ConsequenceForSelf,AppDef.ProspectIrrelevant);				
		//Standard about police
		if(affectComponent.personality[(int)OCEAN.C] < 0f) {
            PoliceBehavior[] policeComponents = FindObjectsOfType(typeof(PoliceBehavior)) as PoliceBehavior[];
            if (policeComponents.Length > 0)                
			    appraisal.AddStandard(0.5f, AppDef.Disapproving, AppDef.FocusingOnOther, policeComponents[0].transform.parent.gameObject);	//police in general --no specific police	            
		}

        //About other protesters in the same group as me
        appraisal.AddStandard(0.35f, AppDef.Approving, AppDef.FocusingOnOther, transform.parent.gameObject);		

        //About myself
        appraisal.AddStandard(0.35f, AppDef.Approving, AppDef.FocusingOnSelf);		

	}

    public void GetBeaten(GameObject other) {
        if(!appraisal.DoesStandardExist(other,AppDef.Disapproving)) {
            appraisal.AddStandard(0.9f, AppDef.Disapproving, AppDef.FocusingOnOther, other);
            appraisal.AddGoal("protest", 0.5f, AppDef.Displeased, AppDef.ConsequenceForSelf, AppDef.ProspectIrrelevant);			            
        }
        agentComponent.AddDamage(0.001f);
            
    }
	
	
}
