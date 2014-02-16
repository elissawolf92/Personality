using UnityEngine;
using System.Collections;

public class DemoKeyCommands : MonoBehaviour 
{
    public BodyInterface bodyInterface;
    private GameObject bodyObject = null;

    void Awake()
    {
        this.bodyObject = bodyInterface.gameObject;
    }

	void Update () 
    {
        if (Input.GetKeyDown(KeyCode.Alpha1) == true)
            bodyInterface.StartAnimation("dismissing_gesture");
        if (Input.GetKeyDown(KeyCode.Alpha2) == true)
            bodyInterface.StartAnimation("being_cocky");

        if (Input.GetKeyDown(KeyCode.Y) == true)
            bodyInterface.SitDown();
        if (Input.GetKeyDown(KeyCode.H) == true)
            bodyInterface.StandUp();

        if (Input.GetKeyDown(KeyCode.U) == true)
            bodyInterface.HeadLookSetActive(true);
        if (Input.GetKeyDown(KeyCode.J) == true)
            bodyInterface.HeadLookSetActive(false);

        if (Input.GetKeyDown(KeyCode.I) == true)
            bodyInterface.ReachSetActive(true);
        if (Input.GetKeyDown(KeyCode.K) == true)
            bodyInterface.ReachSetActive(false);
	}
}
