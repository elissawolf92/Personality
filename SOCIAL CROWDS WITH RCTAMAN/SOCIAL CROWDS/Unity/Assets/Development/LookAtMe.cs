using UnityEngine;
using System.Collections;

public class LookAtMe : MonoBehaviour 
{
    public BodyInterface[] characters;
    public Transform target;
	
	// Update is called once per frame
	void Update () 
    {
        
        foreach (BodyInterface character in characters)
            character.HeadLookSetTarget(target.transform.position);

        if (Input.GetKeyDown(KeyCode.O) == true)
            foreach (BodyInterface character in characters)
                if (Random.Range(0.0f, 1.0f) > 0.7f)
                    character.HeadLookSetActive(true);
        if (Input.GetKeyDown(KeyCode.L) == true)
            foreach (BodyInterface character in characters)
                if (Random.Range(0.0f, 1.0f) > 0.7f)
                    character.HeadLookSetActive(false);

        //NavMeshAgent agent = GetComponent<NavMeshAgent>();
        //UnitySteeringController steer = GetComponent<UnitySteeringController>();
        //Debug.Log(
        //    agent.velocity.sqrMagnitude 
        //    + " " 
        //    + agent.remainingDistance 
        //    + " " 
        //    + agent.stoppingDistance 
        //    + " " 
        //    + steer.HasArrived());
	}
}
