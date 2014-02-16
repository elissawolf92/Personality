using UnityEngine;
using System.Collections;

public class TutorialWaypointCompleted : MonoBehaviour 
{
    public KeyCode code;
    public UnitySteeringController controller;

    void Update () 
    {
        if (Input.GetKeyDown(this.code) == true)
            this.controller.Target = transform.position;
    }
}
