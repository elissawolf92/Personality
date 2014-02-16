using UnityEngine;
using System.Collections;

public class TutorialWaypoint2Completed : MonoBehaviour
{
    public KeyCode code;
    public Body body;

    void Update()
    {
        if (Input.GetKeyDown(this.code) == true)
            this.body.NavGoTo(transform.position);
    }
}
