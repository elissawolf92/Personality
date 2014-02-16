using UnityEngine;
using System.Collections;

public class PersonalityMapper: MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
	
	public void PersonalityToSteering()
	{
		RecastSteeringController steering = this.GetComponent<RecastSteeringController>();	
		
		float[] personality = this.GetComponent<AffectComponent>().personality;
		
		
		//Max speed = [1 3], E
		//steering.maxSpeed = personality[(int)OCEAN.E] + 2f;
		
		
		//Radius = [0.4 0.8] , 1/E
		//steering.radius = -0.2f* personality[(int)OCEAN.E] + 0.6f;
		
		//Max speed = [1.2 2.2], E
		steering.maxSpeed = 0.5f * personality[(int)OCEAN.E] + 1.7f;
		
		//Radius = [0.5 0.9] , 1/E
		steering.radius = -0.2f* personality[(int)OCEAN.E] + 0.8f;
		
		
		//Pushiness = [low, med, high], E, 1/A
		float pushVal = personality[(int)OCEAN.E] + 1f - personality[(int)OCEAN.A];
		
		if(pushVal < 0.3f)
			steering.pushiness = RecastSteeringManager.Pushiness.PUSHINESS_LOW;
		else if(pushVal < 1.9f)
			steering.pushiness = RecastSteeringManager.Pushiness.PUSHINESS_MEDIUM;
		else
			steering.pushiness = RecastSteeringManager.Pushiness.PUSHINESS_HIGH;	
		
	
		
	
		//Change mesh color according to personality
		Color persColor = Color.white;
		persColor.r = persColor.g = persColor.b = (personality[(int)OCEAN.E]+1f)/2.0f;
		
		
		//transform.Find("Mesh").renderer.sharedMaterial.color =  persColor;
		
		//transform.GetComponent<MeshRenderer>().material.color = persColor;

	}

    
}