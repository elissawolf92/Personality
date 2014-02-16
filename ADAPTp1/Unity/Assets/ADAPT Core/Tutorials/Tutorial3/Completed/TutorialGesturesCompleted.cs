using UnityEngine;
using System.Collections;

public class TutorialGesturesCompleted : MonoBehaviour 
{
    protected Body body;

	void Start () 
    {
        // Get a reference to the body component
        this.body = this.GetComponent<Body>();	
	}
	
	void Update () 
    {
        if (Input.GetKeyDown(KeyCode.Alpha1) == true)
            this.body.AnimPlay("dismissing_gesture");
        if (Input.GetKeyDown(KeyCode.Alpha2) == true)
            this.body.AnimPlay("being_cocky");
	}
}
