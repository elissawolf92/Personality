using UnityEngine;
using System.Collections;

public class NewBehaviourScript : MonoBehaviour 
{
    public Transform cursor;
    public SmartChair chair;

	// Use this for initialization
	void Start () 
    {
	
	}
	
	// Update is called once per frame
	void Update () 
    {
        //if (Input.GetKeyDown(KeyCode.V) == true)
        //    gameObject.GetComponent<UnitySteeringController>().Move(new Vector3(1.0f, 0.0f, 0.0f));
        if (Input.GetKeyDown(KeyCode.Space) == true)
        {
            gameObject.GetComponent<UnitySteeringController>().orientationBehavior = OrientationBehavior.None;
            gameObject.GetComponent<UnitySteeringController>().desiredOrientation = 
                Quaternion.LookRotation(
                    new Vector3(cursor.position.x, 0.0f, cursor.position.z)
                    - new Vector3(transform.position.x, 0.0f, transform.position.z));
        }
        if (Input.GetKeyDown(KeyCode.M) == true)
            chair.Use(gameObject.GetComponent<BodyInterface>());
	}
}
