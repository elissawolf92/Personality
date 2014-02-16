using UnityEngine;
using System.Collections;


//[RequireComponent (typeof (AgentComponent))]
public class UpdateAnimation : MonoBehaviour {
		
	public string currentAnim;
    public string expression; //for debugging
    public bool isLocomotion;
    public string locomAnim;
    public string allAnims;
    public float scale;
    
	void Start() {

        currentAnim = "idle_1";
        scale = 0.05f;

		animation["clap"].layer = 1;
       	animation["clap"].AddMixingTransform(transform.Find("Hips/Spine/Spine1/Spine2"));
		
		animation["throw"].layer = 1;
       	animation["throw"].AddMixingTransform(transform.Find("Hips/Spine/Spine1/Spine2"));
		
		animation["cheer"].layer = 1;
       	animation["cheer"].AddMixingTransform(transform.Find("Hips/Spine/Spine1/Spine2"));
				
		animation["protest"].layer = 1;
       	animation["protest"].AddMixingTransform(transform.Find("Hips/Spine/Spine1/Spine2"));

        animation["pick"].layer = 1;        
        animation["pick"].AddMixingTransform(transform.Find("Hips/Spine/Spine1/Spine2"));		
		

		animation["fight"].layer = 1;

        if(GetComponent<AgentComponent>().role == (int)RoleName.Shopper)
       	    animation["fight"].AddMixingTransform(transform.Find("Hips/Spine/Spine1/Spine2"));		
        else
            animation["fight"].AddMixingTransform(transform.Find("Hips"));		
		
		if(GetComponent<AgentComponent>().role == (int)RoleName.Police) {			
			animation["guard"].layer = 1;
            animation["guard"].AddMixingTransform(transform.Find("Hips/Spine/Spine1/Spine2")); // /Spine/Spine1/Spine2/Spine3/Spine4/Neck"));
			
			animation["nightStick"].layer = 1;
	       	animation["nightStick"].AddMixingTransform(transform.Find("Hips/Spine/Spine1/Spine2"));
		}
        
        //Posture related
        animation["afraidPosture"].layer = 0;
        animation["afraidPosture"].AddMixingTransform(transform.Find("Hips/Spine/Spine1/Spine2"));

        animation["angryPosture"].layer = 0;
        animation["angryPosture"].AddMixingTransform(transform.Find("Hips/Spine/Spine1/Spine2"));

        animation["happyPosture"].layer = 0;
        animation["happyPosture"].AddMixingTransform(transform.Find("Hips/Spine/Spine1/Spine2"));

        animation["sadPosture"].layer = 0;
        animation["sadPosture"].AddMixingTransform(transform.Find("Hips/Spine/Spine1/Spine2"));
        
    //    animation.SyncLayer(1);
      //  animation.SyncLayer(0);

        AdjustScale(false);
        

	}

	public void Restart() {
        currentAnim = "idle_1";        
        animation.Stop();        
    }
    public void AdjustScale(bool screenShotMode) {
        if (screenShotMode) //good in the protest scene
            scale = 0.1f;
        else
            scale = 0.01f;
      //  scale = 0.01f; //good in the sales scene
    }

	void SetAnimationSpeed()  {        
		foreach (AnimationState state in animation) {                        
			state.speed = (1 / Time.deltaTime) * scale;  
           //  state.time += 0.01f;

        }        
	}
	
	void SwitchAnimation(string nextAnim) {

        float cw = 0.5f; //curr fade out weight
        float nw = 0.5f; //next fade in weight

        if (nextAnim.Equals("pick")) {
            currentAnim = "pick";         
         //   animation.Blend("pick");
               animation.Play("pick");
            animation["pick"].time = 1.5f; // because pick starts a bit late
        }
        else {
            if(currentAnim.Equals("") == false)
                animation.Blend(currentAnim, 0, cw);
            if(nextAnim.Equals("") == false)
                animation.Blend(nextAnim, 1, nw);	    
        }
                
        currentAnim = nextAnim;
    
	}
	
	public void AnimatePosture() {		
		expression = GetComponent<AgentComponent>().GetExpression();        
			
		if(!expression.Equals("neutral")) {
			animation.Blend (expression + "Posture", 0.5f);                                
        }
        
	}
	
	//Select animation according to velocity
	//Run, walk or stop
	public void Update() {

        
		SetAnimationSpeed ();        
		
        if (GetComponent<AgentComponent>().IsDead()) {
            animation.Stop();            
            gameObject.transform.Rotate(90, 0, 0);
        }
        else{
            /*if(!isLocomotion) {
                 
                if (GetComponent<SteeringController>().velocity.magnitude < 0.1f){
                    animation.CrossFade("idle_1", 0.3f); //vs play
                    locomAnim = "idle_1";
               
                }
                else if (GetComponent<SteeringController>().velocity.magnitude > 1.5f) {
                    //animation["run"].speed = (2f * scale / Time.deltaTime) * GetComponent<SteeringController>().velocity.magnitude / 2.0f;
                    animation["run"].speed = (2f * scale ) * GetComponent<SteeringController>().velocity.magnitude / 2.0f;
                    animation.CrossFade("run", 0.3f);
                    locomAnim = "run";
            
                 
                }
                else {
                    //animation["walk"].speed = (2f * scale / Time.deltaTime) * GetComponent<NavMeshAgent>().velocity.magnitude;
                    animation["walk"].speed = 0.5f * GetComponent<SteeringController>().velocity.magnitude;
                    animation.CrossFade("walk", 0.3f);
                    locomAnim = "walk";
                 
              
                }            
            }
            */
           
           //  AnimatePosture();  //disabled for sales
		
            string act = GetComponent<AgentComponent>().CurrAction;         
            SwitchAnimation(act); //animations except walk, run or idle            		                    
         
        }

			
		
		
		
	}
	
}

