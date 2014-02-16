using UnityEngine;
using System.Collections;

public class StayInPlace : MonoBehaviour 
{
    public Vector3 position;

	// Use this for initialization
	void Start () 
    {
        this.position = transform.position;
	}
	
	// Update is called once per frame
	void Update () 
    {
        Vector3 difference = this.position - transform.position;
        this.GetComponent<UnitySteeringController>().Move(difference);
	}
}
