using UnityEngine;
using System.Collections;

public class GroupBuilder : MonoBehaviour {

	public int agentCnt = 0; //number of agents in the group
	public GameObject[] agents;
	
	/// <param>
	/// <param name="totalAgentCnt"> Kept for assigning agent ids
	/// </param>
	public void Init(int role, int cnt,int groupId, int totalAgentCnt)
	{
		int i;		
		this.agentCnt = cnt;
		agents = new GameObject[agentCnt];
		
		
		for(i=0; i< agentCnt; i++) {
            Debug.Log(i);
            agents[i] = (GameObject)Instantiate(UnityEngine.Resources.Load("AgentPrefab"), transform.position, transform.rotation);
			/*
			if(role == (int)RoleName.Police)
				agents[i] = (GameObject)Instantiate(UnityEngine.Resources.Load("MalePolice01"), transform.position, transform.rotation);
			
			else{
                if(i % 8 == 0)
				    agents[i] = (GameObject)Instantiate(UnityEngine.Resources.Load("Male01"), transform.position, transform.rotation);
                else if (i % 8 == 1)
                    agents[i] = (GameObject)Instantiate(UnityEngine.Resources.Load("Female01"), transform.position, transform.rotation);
                else if (i % 8 == 2)
                    agents[i] = (GameObject)Instantiate(UnityEngine.Resources.Load("Male02"), transform.position, transform.rotation);
                else if (i % 8 == 3)
                    agents[i] = (GameObject)Instantiate(UnityEngine.Resources.Load("Female02"), transform.position, transform.rotation);
                else if (i % 8 == 4)
                    agents[i] = (GameObject)Instantiate(UnityEngine.Resources.Load("Male03"), transform.position, transform.rotation);
                else if (i % 8 == 5)
                    agents[i] = (GameObject)Instantiate(UnityEngine.Resources.Load("Female03"), transform.position, transform.rotation);
                else if (i % 8 == 6)
                    agents[i] = (GameObject)Instantiate(UnityEngine.Resources.Load("Male04"), transform.position, transform.rotation);
                else if (i % 8 == 7)
                    agents[i] = (GameObject)Instantiate(UnityEngine.Resources.Load("Female04"), transform.position, transform.rotation);

            }
            */
            agents[i].AddComponent("AgentComponent");
			agents[i].AddComponent("UpdateAnimation");
			agents[i].AddComponent("Appraisal");
            agents[i].AddComponent("AffectComponent");	
			agents[i].AddComponent("PersonalityMapper");
		
			
			agents[i].GetComponent<AgentComponent>().groupId = groupId;
			agents[i].GetComponent<AgentComponent>().id = totalAgentCnt + i;
			
			
			agents[i].GetComponent<AgentComponent>().attractor = GameObject.Find ("target");
            agents[i].tag = "Player";
						
			
	//		task.Reset(agents[i].GetComponent<AgentComponent>().attractor.position); 
	//		agents[i].GetComponent<AgentComponent>().tasks.Add(task);	

            agents[i].transform.parent = GameObject.Find("CrowdGroup" + groupId).transform;						
			agents[i].GetComponent<AgentComponent>().role = role;
			
		
			switch(role){				
				case (int)RoleName.Protester:					
					agents[i].AddComponent("ProtesterBehavior");
                    GameObject protestCenter = GameObject.Find("ProtestCenter");
                    if(protestCenter == null){
                        protestCenter = new GameObject("ProtestCenter");
                        protestCenter.AddComponent("UpdateProtestCenter");
                    }
					break;              
				case (int)RoleName.Police:
					agents[i].AddComponent("PoliceBehavior");
                    break;
                case (int)RoleName.Provocateur:
                    agents[i].AddComponent("ProvocateurBehavior");
                    break;				
                case (int)RoleName.Shopper:
                agents[i].AddComponent("ShopperBehavior");
                    break;
				
			}
			
		}

        //Group components
        switch(role){				
				case (int)RoleName.Protester:
                    GameObject grp; 
                    grp = GameObject.Find("CrowdGroup" + groupId);
					if(grp.GetComponent<LeaderComponent>() == null) //Leader should be added manually
						grp.AddComponent("LeaderComponent");									
                    break;
                case (int)RoleName.Police:
                    grp = GameObject.Find("CrowdGroup" + groupId);
					if(grp.GetComponent<ZoneComponent>() == null) //Protection zone
				    grp.AddComponent("ZoneComponent");
                    grp.GetComponent<ZoneComponent>().ComputeProtectionZone();
                    break;
         }
				   
	}
	
	public void UpdatePersonalityAndBehavior(float[] persMean, float[] persStd) {
		for( int i=0; i< agentCnt; i++)	 {						
			agents[i].GetComponent<AffectComponent>().UpdatePersonality(persMean, persStd);			
			//Update steering behaviors according to personality parameters
			agents[i].GetComponent<PersonalityMapper>().PersonalityToSteering();
		}		
	}
	public void ToggleLocomotionController(bool check) {
        foreach(Transform a in transform)	 {
            a.GetComponent<LegController>().enabled = check;
            a.GetComponent<LegAnimator>().enabled = check;
            a.GetComponent<AlignmentTracker>().enabled = check;
            a.GetComponent<UpdateAnimation>().isLocomotion = check;
		}
 
    }
    public void UpdateRegion(float rectX, float rectZ) {
        for (int i = 0; i < agentCnt; i++){
            agents[i].transform.Translate(transform.position - agents[i].transform.position);
            agents[i].transform.Translate(MathDefs.GetRandomNumber(-rectX, rectX), 0, MathDefs.GetRandomNumber(-rectZ, rectZ));               
        }
    }
	
	
}
