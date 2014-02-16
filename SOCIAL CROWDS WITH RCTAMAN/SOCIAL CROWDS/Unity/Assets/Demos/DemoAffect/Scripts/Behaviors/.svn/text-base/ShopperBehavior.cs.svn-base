using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ShopperBehavior : MonoBehaviour {
    Appraisal appraisal;
    AgentComponent agentComponent;
    AffectComponent affectComponent;
    SteeringController steering;
    Vector3 counterPos; //check out counter position
    public GameObject cashier;
    public GameObject storeEntrance;
    public Transform desiredObj;
    public Transform currentObj;
    public Vector3 destination = Vector3.zero; //desired object's closest point
    
    public int desiredObjCnt; //[1 11]
    public int acquiredObjCnt; 
    
    public bool isPicking = false;
    public bool hasPaid = false;
    public bool isPaying = false;
    public bool isInside = false;
    
    
    public int totalObjCnt = 0;
    public int payDuration = 0;
    public int pickDuration = 0;
    Transform hand;
    Vector3 objMeshPos;
    public float dist2dest{
        get{
            return (destination - transform.position).magnitude;
        }
    }

    // Use this for initialization
	void Start () {
        appraisal = GetComponent<Appraisal>();
        agentComponent = GetComponent<AgentComponent>();
        affectComponent = GetComponent<AffectComponent>();
        steering = GetComponent<SteeringController>();

     //   navmeshAgent.radius -= 0.1f; //smaller than regular size
        //navmeshAgent.speed += 0.6f; //faster than regular speed
        agentComponent.walkingSpeed = steering.maxSpeed;

        counterPos = GameObject.Find("counter").transform.position;
        cashier = GameObject.Find("cashier");
        storeEntrance = GameObject.Find("storeEntrance");

        acquiredObjCnt = 0;
        desiredObjCnt = (int)(5f * affectComponent.personality[(int)OCEAN.E] + 6f); //correlated to extroversion [1 11]
        
        
        InitAppraisalStatus();	
       
        
	}

    public void Restart() {
        InitAppraisalStatus();
        UpdateAttractor();
        isPicking = false;
        acquiredObjCnt = 0;
        hasPaid = false;
        isPaying = false;
        payDuration = 0;
        pickDuration = 0;
        isInside = false;

        GameObject objs = GameObject.Find("Objects");
          for(int i = 0; i < objs.transform.GetChildCount(); i++) {
            objs.transform.GetChild(i).gameObject.SetActiveRecursively(true);
            objs.transform.GetChild(i).gameObject.GetComponent<ObjComponent>().achieved = false;
          }
    }

	void LateUpdate () {
       
        if(agentComponent.IsFighting() == false)
            UpdateAttractor();
        else{
            if(currentObj != null)
                currentObj.gameObject.SetActiveRecursively(false);
        }
        
        //Fight with disapproved agents
        //Disapproved agents are the ones who achieve my desired object before me
        foreach(GameObject c in agentComponent.collidingAgents) {
            if(c.GetComponent<AgentComponent>().role == (int)RoleName.Shopper && agentComponent.IsGoodToFight(c)){
                agentComponent.StartFight(c);
                c.GetComponent<AgentComponent>().StartFight(this.gameObject);               
            }
        }
            
        UpdateAction();                 
	}

    void CheckInside(){
        if ((transform.position - storeEntrance.transform.position).magnitude < 3f)
            isInside = true;
        else
            isInside = false;

    }
    
    void UpdateAttractor() {
        //Find closest object with the least number of people headed towards
        GameObject objs = GameObject.Find("Objects");
        int objCnt = objs.transform.GetChildCount();
        Transform closestObj = null;
        float minDist = 100000f;
        totalObjCnt = 0;
        Vector3 tmpMeshPos = objMeshPos;

        if(isPicking) {
             hand = transform.Find("Hips/Spine/Spine1/Spine2/Spine3/Spine4/Neck/RightShoulder/RightArm/RightForeArm/RightHand/RightHandRing1");             
             currentObj.position = hand.position - objMeshPos ;
        
        }


    

        for(int i = 0; i < objCnt; i++){
            Transform obj = objs.transform.GetChild(i);
            if (obj.gameObject.active == false || obj.GetComponent<ObjComponent>().achieved == true)
                continue;
            totalObjCnt++;
            MeshFilter[] mfs = obj.GetComponents<MeshFilter>();            
            foreach(MeshFilter mf in mfs){
                Mesh m = mf.mesh;
                float dist = (m.bounds.center + obj.transform.position - transform.position).magnitude;
                if( dist < minDist){
                    destination = m.bounds.center + obj.transform.position;
                    tmpMeshPos = m.bounds.center;
                    minDist = dist;
                    closestObj = obj;
                }
            }
        }

        if (totalObjCnt == 0 || acquiredObjCnt >= desiredObjCnt){ //Nothing left to buy
            if (hasPaid || affectComponent.personality[(int)OCEAN.C] < 0) {// go to exit without paying                
                destination = agentComponent.attractor.transform.position;
                UpdateAppraisalStatus(); 
            }
            else if(acquiredObjCnt > 0){
                Pay();
                UpdateAppraisalStatus(); 
            }
        }
        else {
            desiredObj = closestObj;
            if(closestObj != null) {                
                if (minDist < 3f) {//Pick up object
                    isPicking = true;                      
                    closestObj.GetComponent<ObjComponent>().achieved = true;
                    if(currentObj != null)
                        currentObj.gameObject.SetActiveRecursively(false);
                    //a new object is obtained
                    currentObj = closestObj;
                    objMeshPos = tmpMeshPos;
                        // closestObj.position =  hand.position;
                    //closestObj.gameObject.SetActiveRecursively(false);
                    acquiredObjCnt++;
                    UpdateAppraisalStatus(); 
                    UpdateAppraisalStatusOfOthers();
                    
                }            
            }    
        }

        agentComponent.SteerTo(destination);
   
    }

    void Pay() {
        destination = counterPos;          
        if ((transform.position - counterPos).magnitude < 2.5f) {
            payDuration++;
             GetComponent<SteeringController>().orientationBehavior = OrientationBehavior.LookAtTarget;
            transform.rotation = Quaternion.LookRotation(cashier.transform.position - transform.position, Vector3.up);
            isPaying = true;

        }
        if(payDuration >= 100f){
            hasPaid = true;
            isPaying = false;
             GetComponent<SteeringController>().orientationBehavior = OrientationBehavior.LookForward;
        }         

    }
    void UpdateAction() {

        if(agentComponent.IsFighting())                    
            agentComponent.CurrAction = "fight";
        else if (isPicking ) {            
                agentComponent.CurrAction = "pick";
               // isPicking = false;
        }
        else
            agentComponent.CurrAction = "";

    }
    void InitAppraisalStatus() {
        //Hope to get into store
        appraisal.AddGoal("sales", 0.3f, AppDef.Pleased, AppDef.ConsequenceForSelf, AppDef.ProspectRelevant, AppDef.Unconfirmed);

        //Standard about other shoppers
   //     if (affectComponent.personality[(int)OCEAN.N] > 0f)  //if neurotic feel jealousy
     //       appraisal.AddStandard(0.3f, AppDef.Disapproving, AppDef.FocusingOnOther, transform.parent.gameObject);	//shoppers in general --no specific shopper	            
        //Attitude towards objects in the store
        appraisal.AddAttitude(null, 0.5f, AppDef.Liking); //General liking
        
    }

    void UpdateAppraisalStatus() {
        if(acquiredObjCnt >= desiredObjCnt){
            //Change hope to satisfaction                  
            bool exists = appraisal.RemoveGoal("sales", AppDef.Pleased, AppDef.ConsequenceForSelf, AppDef.ProspectRelevant, AppDef.Unconfirmed);
            if(exists) {
                appraisal.AddGoal("sales", 0.6f, AppDef.Pleased, AppDef.ConsequenceForSelf, AppDef.ProspectRelevant, AppDef.Confirmed);		
                //If neurotic, gloating towards other shoppers
                if (affectComponent.personality[(int)OCEAN.N] > 0f)
                    appraisal.AddGoal("sales", 0.3f, AppDef.Pleased, AppDef.ConsequenceForOther, transform.parent.gameObject, AppDef.UndesirableForOther);
            }
        }
        else {
            if(totalObjCnt == 0) {
                //Change hope to disappointment
                bool exists = appraisal.RemoveGoal("sales", AppDef.Pleased, AppDef.ConsequenceForSelf, AppDef.ProspectRelevant, AppDef.Unconfirmed);                
                if(exists) {
                    appraisal.AddGoal("sales", 0.6f, AppDef.Pleased, AppDef.ConsequenceForSelf, AppDef.ProspectRelevant, AppDef.Disconfirmed);		
                    //Resentment towards other shoppers
                    appraisal.AddGoal("sales", 0.3f, AppDef.Displeased, AppDef.ConsequenceForOther, transform.parent.gameObject, AppDef.DesirableForOther);
                }
            }

        }
    }

    //When I achieve an object others with the same object as their goal disapprove of me
    void UpdateAppraisalStatusOfOthers() {
        foreach (GameObject other in agentComponent.collidingAgents)
        {
            if (other.GetComponent<ShopperBehavior>().desiredObj == desiredObj)
                other.GetComponent<Appraisal>().AddStandard(0.2f, AppDef.Disapproving, AppDef.FocusingOnOther, this.gameObject);
        }
    }

    void OnDrawGizmosSelected(){
        Gizmos.color = Color.blue;
        Gizmos.DrawLine(transform.position, destination);
    }
}
