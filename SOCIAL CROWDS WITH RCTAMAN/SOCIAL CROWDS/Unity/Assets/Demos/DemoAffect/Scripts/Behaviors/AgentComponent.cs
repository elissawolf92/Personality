using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public struct tTask
{
	public Vector3 location;		// position where the agent needs to be located before starting the action.
	public float 	proximity; 		// how close I need to be to that location to be able to perform the action
	public float	duration;		// How long the action lasts
	public float	timer;			// When the action starts, timer is Set to duration, and then every simulation step it de
	
	public void Reset(Vector3 loc)
	{
		location = loc;
		proximity = 0.5f;
		duration = 10000.0f; //10 seconds
		timer = duration;
	}			
}

public enum RoleName{
	Audience,
	Provocateur, 
	Singer,
	Protester,
	Police,
    Shopper,
	Attacker,
	Victim
}


//[RequireComponent (typeof (AffectComponent))]
public class AgentComponent: MonoBehaviour {
   
	public GameObject attractor;	
	public int id;
	public int role;
	public float walkingSpeed = 1f; //Computed according to personality
    public Vector3 initialPos;
    Contagion damage;
    public int groupId;
	public float panicThreshold = 0.5f; //between 0 and 1 computed according to personality ( Initial personality 0)
	public float panicLevel;
	public ArrayList collidingAgents;
    public float timeLastFight = 0f; //end time of the last fight
    public float velocityMagnitude;
    public int damageStatus;    
    string currAction = "idle_1";
    public float waitDuration; //Duration to wait until a new path is set. Proportional to patience
    public List<float> avgVelList = new List<float>(); //average velocity over the last 10 frames
	public string CurrAction {
        get { return currAction; }            
		set { currAction = value; }
	}
    public int collidingCnt;
    
	// Use this for initialization
	void Awake () {
        collidingAgents = new ArrayList(); //agents that are colliding with me
        if(!GetComponent<SteeringController>())
            gameObject.AddComponent("SteeringController");
        GetComponent<SteeringController>().maxSpeed = walkingSpeed; //maximum speed
        damage = new Contagion();
        damage.WoundThreshold = 2f;
        damage.DoseThreshold = 8f; //agent dies
        GetComponent<PersonalityMapper>().PersonalityToSteering(); //initialize steering parameters according to personality
        initialPos = transform.position;
       
	}

	public void Restart() {
        //DestroyImmediate(GetComponent<NavMeshAgent>());
        //gameObject.AddComponent("NavMeshAgent");
         GetComponent<SteeringController>().maxSpeed = walkingSpeed; //maximum speed
        GetComponent<PersonalityMapper>().PersonalityToSteering(); //update steering parameters in navmesh according to personality
        transform.position = initialPos;
        avgVelList.Clear();        
        damage.Dose = 0f;      		
        CurrAction = "idle_1";
        if(IsFighting()) {
            DestroyImmediate(GetComponent<FightBehavior>());
        }
        
    }

	void Update () {        
		List<int> lambdaList = new List<int>(); //indices of dominant emotions around us
		float[] eventFactor = new float[22];        

        if (IsDead()) {            
            GetComponent<SteeringController>().Stop(true);            
            return;
        }

//        SteerTo(attractor.transform.position);
        
        damageStatus = (int) damage.Status;
        velocityMagnitude = GetComponent<SteeringController>().velocity.magnitude;        
     
        GetComponent<SteeringController>().orientationBehavior = OrientationBehavior.LookForward; 
        
        eventFactor = GetComponent<Appraisal>().ComputeEventFactor();                
        //Emotion contagion
		foreach(GameObject c in collidingAgents) { //Agents within a radius
            if (IsVisible(c, Mathf.PI / 2f)) { //if c is in my visual cone
				int em = c.GetComponent<AffectComponent>().FindDominantEmotion();
				if(em > -1){
					lambdaList.Add (em);	
				}								
			}
		}
        collidingCnt = collidingAgents.Count;
		
		GetComponent<AffectComponent>().UpdateAffectiveState(eventFactor, lambdaList);		
       
        if(IsFighting() == false) {
            if (TimeSinceLastFight() > 10) //start healing after 10 seconds
                Heal();		
        }			
	}

	public void SteerTo(Vector3 pos) {
        GetComponent<SteeringController>().Target = pos;        
	}
	
	///Steer away from the location pos
	public void SteerFrom(Vector3 pos) {        
        Vector3 dir = transform.position - pos;
        dir.y = 0f;
        dir.Normalize();
        GetComponent<SteeringController>().Target =  transform.position + dir ;			
	}
	
	/// Finds the agents around me
	void OnTriggerEnter(Collider collider) {		
		if(collider.gameObject.name.Contains("Male") || collider.gameObject.name.Contains("Female") || collider.gameObject.name.Contains("Police"))	{			
			if(!collidingAgents.Contains(collider))
				collidingAgents.Add(collider.gameObject);          
		}		
	}
	
	void OnTriggerExit(Collider collider) {	
		if(collider.gameObject.name.Contains("Male") || collider.gameObject.name.Contains("Female") || collider.gameObject.name.Contains("Police"))	 {						
			collidingAgents.Remove(collider.gameObject);            
		}		
	}
	
	public void AddDamage(float amount) {				
		damage.AddDose(amount);				
	}

	public void Heal() {			
		damage.DecayDose();
        if (!IsWounded())
            ResetSpeed();
            //CalmDown();
         
	}
	public bool IsDead() {			        
		return damage.Status == StatusType.Infected; 
	}
	
	public bool IsWounded() {				        
		return (damage.Status == StatusType.Wounded  ||  damage.Status == StatusType.Infected);
	}
		
	public void Follow(GameObject agent) {
		this.attractor = agent;
	}
		
	public string GetExpression() {
		return GetComponent<AffectComponent>().GetEkmanEmotion();
	}
	
	public void IncreaseSpeed(int c) {
		float maxSpeed = 4.5f;
		GetComponent<SteeringController>().maxSpeed += c * 0.2f*Time.deltaTime;
		if(GetComponent<SteeringController>().maxSpeed > maxSpeed)
			GetComponent<SteeringController>().maxSpeed = maxSpeed; 		
	}

	public void DecreaseSpeed(int c) {
		GetComponent<SteeringController>().maxSpeed -= c * 0.1f*Time.deltaTime;
		if(GetComponent<SteeringController>().maxSpeed < 0f)
			GetComponent<SteeringController>().maxSpeed = 0f; 
	}
    
    public void ResetSpeed() {
        GetComponent<SteeringController>().maxSpeed = walkingSpeed; 		
    }
	
	public void IncreasePanic() {
		panicLevel += 0.1f*Time.deltaTime;
		if(panicLevel > 1f)
			panicLevel = 1f;		
		
		if(IsPanicking())
			IncreaseSpeed(2);
		
	}
	public void DecreasePanic() {	
		panicLevel -=0.1f*Time.deltaTime;					
		if(panicLevel < 0f)
			panicLevel = 0f;
		
		if(IsPanicking() && GetComponent<SteeringController>().maxSpeed > walkingSpeed ) 
			DecreaseSpeed(1);
		else
			GetComponent<SteeringController>().maxSpeed = walkingSpeed;			
	}

    public void CalmDown() {
        panicLevel = 0f;
        GetComponent<SteeringController>().maxSpeed = walkingSpeed;			
    }
	
	public bool IsPanicking() {
		return panicLevel > panicThreshold;			
	}
	

    //Change average velocity over the last waitDuration number of frames
    public float ComputeAvgVel(){        
        avgVelList.Add(GetComponent<SteeringController>().velocity.magnitude);
        if(avgVelList.Count > waitDuration)
            avgVelList.RemoveAt(0);
        float avgVel = 0f;
        foreach(float v in avgVelList)
            avgVel += v;

        return avgVel;
   }

	/// If other is in my visual cone 
	public bool IsVisible(GameObject other, float viewAngle) {
			
		Vector3 orientation = GetComponent<SteeringController>().velocity;
		Vector3 distVec = other.transform.position - transform.position ;
        orientation.y = distVec. y  = 0f;        
		orientation.Normalize();
		distVec.Normalize ();
		float angle = Mathf.Acos(Vector3.Dot (distVec, orientation));        
        if(angle < viewAngle)
			return true;
		return false;
			
	}
		
	

    public void Watch(GameObject other) {
        Quaternion wantedRotation = Quaternion.LookRotation(other.transform.position - transform.position, Vector3.up);
        transform.rotation = Quaternion.Lerp(transform.rotation, wantedRotation, Time.time * 0.01f);
    }
    public void Watch(Vector3 pos) {
        Quaternion wantedRotation = Quaternion.LookRotation(pos - transform.position, Vector3.up);
        transform.rotation = Quaternion.Lerp(transform.rotation, wantedRotation, Time.time * 0.01f);
    }
    public void Face(GameObject other) {
        SteerTo(other.transform.position + other.GetComponent<SteeringController>().velocity);
    }

    public bool IsFacing(GameObject other){
        //(other.transform.position - transform.position).magnitude < 2f &&
        return (IsVisible(other, Mathf.PI / 4f) && other.GetComponent<AgentComponent>().IsVisible(this.gameObject, Mathf.PI / 4f));
    }

    

    public bool IsFighting() {
        if (GetComponent<FightBehavior>() == null)
            return false;
        else
            return true;
    }
    public bool CanFight() {
        if (IsFighting()) //already fighting
            return false;
        if (IsWounded() == true)
            return false;
        return true;

    }
    
    ///Returns true if a fight conditions are met
    public bool IsGoodToFight(GameObject other) {
        if (GetComponent<AffectComponent>().IsMood((int)MoodType.Hostile) && (other.transform.position - transform.position).magnitude < 3f)
            if (CanFight() && other.GetComponent<AgentComponent>().CanFight())
                if (GetComponent<Appraisal>().DoesStandardExist(other, AppDef.Disapproving) || GetComponent<Appraisal>().DoesStandardExist(other.transform.parent.gameObject, AppDef.Disapproving))
                    return true;
        return false;
    }
     

	/// Start a fight with the other agent	
	public void StartFight(GameObject other) {
		
		if(GetComponent("FightBehavior") == null) {
			this.gameObject.AddComponent("FightBehavior");
			GetComponent<FightBehavior>().Init(other);           
		}				
	}

    
    public float TimeSinceLastFight() {
        return Time.time - timeLastFight;
    }
	
	public void DeactivateOtherBehaviors(){
        if (GetComponent<ProtesterBehavior>()) {
            GetComponent<ProtesterBehavior>().enabled = false;
            if (GetComponent<ProtesterBehavior>().bannerCarrier)
                GetComponent<ProtesterBehavior>().banner.SetActiveRecursively(false);
        }
        if (GetComponent<PoliceBehavior>()) {
            GetComponent<PoliceBehavior>().enabled = false;
            GetComponent<PoliceBehavior>().shield.SetActiveRecursively(false);
            GetComponent<PoliceBehavior>().nightStick.SetActiveRecursively(false);
        }

        //Stop fighting    
        if (GetComponent<FightBehavior>()) {
            GetComponent<FightBehavior>().FinishFight();           
        }

        GetComponent<SteeringController>().orientationBehavior = OrientationBehavior.LookForward; //in case police vigiling is stopped in the middle
        
    }
    public void ReactivateOtherBehaviors()
    {
        if (GetComponent<ProtesterBehavior>()) {
            GetComponent<ProtesterBehavior>().enabled = true;
            if(GetComponent<ProtesterBehavior>().bannerCarrier){
                GetComponent<ProtesterBehavior>().banner.SetActiveRecursively(true);
                GetComponent<ProtesterBehavior>().Enable();
            }
        }
        if (GetComponent<PoliceBehavior>()) {
            GetComponent<PoliceBehavior>().enabled = true;
            GetComponent<PoliceBehavior>().shield.SetActiveRecursively(true);
            GetComponent<PoliceBehavior>().Enable();
        }
        
    }
	
	
}

	

