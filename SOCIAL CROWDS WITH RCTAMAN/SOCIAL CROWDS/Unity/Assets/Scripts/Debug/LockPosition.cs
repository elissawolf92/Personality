using UnityEngine;
using System.Collections;

public class LockPosition : MonoBehaviour 
{
    private Vector3 position;

	// Use this for initialization
	void Start () 
    {
        this.position = transform.position;
	}
	
	// Update is called once per frame
	void FixedUpdate () 
    {
        transform.position = this.position;
	}
}
