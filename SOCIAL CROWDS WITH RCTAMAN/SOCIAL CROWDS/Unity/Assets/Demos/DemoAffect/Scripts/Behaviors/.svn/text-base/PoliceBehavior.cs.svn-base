using UnityEngine;
using System.Collections;


public class PoliceBehavior : MonoBehaviour {
    
    public GameObject shield;
    public GameObject nightStick;    
    public Vector3 post; 
    public GameObject intruder = null;
    public Bounds zone;   //protection zone assigned in GroupBuilder
    AgentComponent agentComponent;
    int frameCnt;
    public GameObject protestCenter;
    bool isBeating = false;

	void Start () {	
		shield = (GameObject)Instantiate(UnityEngine.Resources.Load("shield"));
		nightStick = (GameObject)Instantiate(UnityEngine.Resources.Load("nightStick"));
		
		agentComponent = GetComponent<AgentComponent>();      
        post = transform.position;
        protestCenter = GameObject.Find("ProtestCenter");
        
	}
    public void Enable() {
        Transform sT = transform.Find("Hips/Spine/Spine1/Spine2/Spine3/Spine4/Neck/LeftShoulder/LeftArm/LeftForeArm/LeftHand");
        shield.transform.position = sT.position;
        shield.transform.rotation = sT.transform.rotation;
        shield.transform.Rotate(-90, 0, 0, Space.Self);
        shield.transform.Rotate(0, 180, 0, Space.Self);
        shield.transform.Translate(0.06f, 0, 0.07f);
        shield.SetActiveRecursively(true);

        Transform nT = transform.Find("Hips/Spine/Spine1/Spine2/Spine3/Spine4/Neck/RightShoulder/RightArm/RightForeArm/RightHand/RightHandRing1");
        nightStick.transform.position = nT.position;
        nightStick.transform.rotation = nT.transform.rotation;
        nightStick.SetActiveRecursively(true);
    }
	public void Restart() {
        intruder = null;
    }
	// Update is called once per frame
	void Update () {
        if (agentComponent.IsDead()) {
            shield.SetActiveRecursively(false);          
            return;
        }

        
        UpdateAction();

        if (agentComponent.IsFighting() == false) {
           // if (intruder == null && (post - transform.position).magnitude > 1f) //if no one to watch && stop jittering
            if ((post - transform.position).magnitude > 2f) //if no one to watch && stop jittering
                agentComponent.SteerTo(post);
           // else                
             //   agentComponent.GetComponent<NavMeshAgent>().Stop();            
			Vigil();            
         }
	}
	void LateUpdate() {
        Transform sT = transform.Find("Hips/Spine/Spine1/Spine2/Spine3/Spine4/Neck/LeftShoulder/LeftArm/LeftForeArm/LeftHand");
        shield.transform.position = sT.position;
        shield.transform.rotation = sT.transform.rotation;
        shield.transform.Rotate(-90, 0, 0, Space.Self);
        shield.transform.Rotate(0, 180, 0, Space.Self);
        shield.transform.Translate(0.06f, 0, 0.08f);

        Transform nT = transform.Find("Hips/Spine/Spine1/Spine2/Spine3/Spine4/Neck/RightShoulder/RightArm/RightForeArm/RightHand/RightHandRing1");
        nightStick.transform.position = nT.position;
        nightStick.transform.rotation = nT.transform.rotation;        
 
    }
	void UpdateAction() {
        if (agentComponent.IsFighting()) {
			agentComponent.CurrAction = "fight";
            shield.SetActiveRecursively(false);            
           
		}
		else if(isBeating) {
            shield.SetActiveRecursively(true);
            agentComponent.CurrAction = "nightStick";
        }
        else {
            shield.SetActiveRecursively(true);    
			agentComponent.CurrAction = "guard";	
		}			
	}
	
  
    /// <summary>
    ///Watch the closest protester in the collider zone 
    ///Prevent him from moving beyond your post
    /// </summary>
	void Vigil() {		
		float minDist = 100000f;
		Vector3 dist = Vector3.zero;
        
       GetComponent<SteeringController>().orientationBehavior = OrientationBehavior.LookAtTarget;
        agentComponent.ResetSpeed();                    

        if(agentComponent.collidingAgents.Count != 0 && isBeating == false) {			
			foreach(GameObject c in agentComponent.collidingAgents) {
                if(c.GetComponent<AgentComponent>().role == (int)RoleName.Protester && c.GetComponent<AgentComponent>().IsFighting() == false) {
				    dist = c.transform.position - transform.position;
				    if(dist.magnitude < minDist) {
					    minDist = dist.magnitude;
                        intruder = c;                            
				    }
			    }
			}
			if(intruder != null && minDist< 4f) {
                agentComponent.Watch(intruder);               
                if (minDist < 2f) {                    
                    isBeating = true;                   
                    intruder.GetComponent<ProtesterBehavior>().GetBeaten(this.gameObject);
                 }
                else
                    isBeating = false;

            }    
            else {
                agentComponent.Watch(protestCenter);                
            }
		}
        GetComponent<NavMeshAgent>().updateRotation = true;
	}


    void OnDrawGizmosSelected() {
        Gizmos.color = Color.yellow;
        if(intruder != null){
            Gizmos.DrawLine(transform.position, intruder.transform.position);
            Debug.DrawRay(intruder.transform.position, intruder.GetComponent<SteeringController>().velocity *3f, Color.white);
        }
    }
}
