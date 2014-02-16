using UnityEngine;
using System.Collections;

public class GUIHandler : MonoBehaviour {
	
	static public bool runSim = false;
	 static public int pickInd = -1;
	static public Vector3 pickedPoint;
    RaycastHit hit;
    public bool getScreenShot = false;

	void OnGUI () {		
	// Make a background box
	    //GUI.Box (new Rect (10,10,120,50), "Main Menu");
	// Make the first button. If it is pressed, running Simulation Toggle will be activated
		
		if(GUI.Button(new Rect(10,10,90,20),"Reset")) 			
			GameObject.Find("Crowd").GetComponent<CrowdManager>().Restart();

        //if(getScreenShot)
          //  GUI.Label(new Rect(150, 10, 90, 20), "screenshot");
        
	}

	void Update() {        
        if (Input.GetMouseButtonDown(0))  {
            
            Ray ray = Camera.mainCamera.ScreenPointToRay(Input.mousePosition);
    
            Physics.Raycast(ray, out hit, Mathf.Infinity);
            
            pickedPoint = hit.point; //gives the gameobject's position            
            
            if(Input.GetKey("i")){
                if(hit.collider.tag.Equals("Player")) 
                    hit.transform.gameObject.GetComponentInChildren<SkinnedMeshRenderer>().material.color = Color.red;
                //Handles.DrawWireDisc(hit.transform.gameObject.transform.position, this.transform.forward, hit.transform.gameObject.collider.radius);
               // Debug.DrawLine(hit.transform.gameObject.transform.position, hit.transform.gameObject.transform.position + new Vector3(0,10,0), Color.red);
                //GameObject.Find("AgentInfoText").guiText.text =hit.transform.GetComponent<Appraisal>().ToString();                    
            }
            if(Input.GetKey("e"))
                Instantiate(UnityEngine.Resources.Load("Explosion"), pickedPoint, new Quaternion());
            
        }

        if (Input.GetKey("s")) {
            getScreenShot = !getScreenShot;

            GetComponent<Screenshot>().enabled = getScreenShot;
            UpdateAnimation[] uas = GameObject.Find("Crowd").GetComponentsInChildren<UpdateAnimation>();
            foreach (UpdateAnimation ua in uas)
                ua.AdjustScale(getScreenShot);
       }
        		
	}
    void OnDrawGizmos(){
        Gizmos.color = Color.blue;
        Gizmos.DrawLine(pickedPoint, pickedPoint+new Vector3(0,5,0));
    }
  
    
}

