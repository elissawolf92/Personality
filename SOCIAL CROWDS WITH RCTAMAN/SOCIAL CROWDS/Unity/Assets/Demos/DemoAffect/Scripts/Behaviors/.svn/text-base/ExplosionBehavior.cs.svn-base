using UnityEngine;
using System.Collections;

public class ExplosionBehavior : MonoBehaviour {
    public GameObject explosion;
    Appraisal appraisal;
    AgentComponent agentComponent;
    const float escapeDist = 10f;
    public float dist;
    private bool isOver = false; //if explosion is over
    Vector3 escapePos; 
    
	// Use this for initialization
	void Start () {        
        appraisal = GetComponent<Appraisal>();
        agentComponent = GetComponent<AgentComponent>();
        InitAppraisalStatus();
        if (explosion) {
            escapePos = transform.position * 2f - explosion.transform.position;
            escapePos.y = transform.position.y;
        }
        else
            Debug.Log("No Explosion gameobject");
	}
		
	void Update () {
        const float fearThreshold = 0.1f;
        if(agentComponent.IsDead())
            Destroy(GetComponent<ExplosionBehavior>());

        if(explosion) {
            dist = (transform.position - explosion.transform.position).magnitude;
            if (dist < 5f) //add even more damage
                agentComponent.AddDamage(0.15f);
            else if (dist < 10f) //add more damage
                agentComponent.AddDamage(0.1f);            
            else if (dist < 15f)
                agentComponent.AddDamage(0.05f);
            
            //Wait until fear is above some threshold before reacting
            if(GetComponent<AffectComponent>().emotion[(int)EType.Fear] > fearThreshold) {
                agentComponent.DeactivateOtherBehaviors();                
                agentComponent.CurrAction = "";
                if (GetComponent<AffectComponent>().IsMood((int)MoodType.Anxious) || GetComponent<AffectComponent>().personality[(int)OCEAN.N] > 0)                
                    agentComponent.IncreasePanic();                
                agentComponent.SteerFrom(explosion.transform.position);
              //  agentComponent.SteerTo(escapePos);
             }
        }
        //Explosion is over
        else {
            if (isOver == false) {
                isOver = true;                
                if (agentComponent.IsWounded())  {
                    //change fear to fearsconfirmed   
                    appraisal.RemoveGoal("explosion", AppDef.Displeased, AppDef.ConsequenceForSelf, AppDef.ProspectRelevant);
                    appraisal.AddGoal("explosion", 1.0f, AppDef.Displeased, AppDef.ConsequenceForSelf, AppDef.ProspectRelevant, AppDef.Confirmed);
                }
                else {
                    //change fear to relief
                    appraisal.RemoveGoal("explosion", AppDef.Displeased, AppDef.ConsequenceForSelf, AppDef.ProspectRelevant);
                    appraisal.AddGoal("explosion", 0.7f, AppDef.Displeased, AppDef.ConsequenceForSelf, AppDef.ProspectRelevant, AppDef.Disconfirmed);
                }                
             }
            agentComponent.DecreasePanic();   

            //wait until fear is below some threshold
            if(GetComponent<AffectComponent>().emotion[(int)EType.Fear] <  fearThreshold ) {
                agentComponent.ReactivateOtherBehaviors();
                agentComponent.CalmDown();
                Destroy(this);
            }            
        }       	
	}

    void InitAppraisalStatus() {
        if (explosion == null) {
            Debug.Log("No explosion gameobject");
            return;
        }
        //Remove other goals
        appraisal.RemoveGoal("");
        //Fear proportional to distance
        dist = (transform.position - explosion.transform.position).magnitude;
        if( dist < 10f)
            appraisal.AddGoal("explosion", 1.0f, AppDef.Displeased, AppDef.ConsequenceForSelf, AppDef.ProspectRelevant, AppDef.Unconfirmed);        
        else if( dist < 15f)
            appraisal.AddGoal("explosion", 0.8f, AppDef.Displeased, AppDef.ConsequenceForSelf, AppDef.ProspectRelevant, AppDef.Unconfirmed);
        else if (dist < 20f)
            appraisal.AddGoal("explosion", 0.5f, AppDef.Displeased, AppDef.ConsequenceForSelf, AppDef.ProspectRelevant, AppDef.Unconfirmed);        
    }

    void OnDrawGizmosSelected(){
  //      Gizmos.color = Color.blue;
    //    Gizmos.DrawLine(transform.position, escapePos);
    }
}
