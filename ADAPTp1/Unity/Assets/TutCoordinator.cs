using UnityEngine;
using System.Collections;

public class TutCoordinator : ShadowCoordinator {

	protected ShadowTransform[] buffer1 = null;
	protected ShadowTransform[] buffer2 = null;
	protected ShadowLeanController lean = null;
	protected ShadowAnimationController anim = null;
	protected Slider weight;

	// Use this for initialization
	void Start () {
		// Allocate space for a buffer for storing and passing shadow poses
		this.buffer1 = this.NewTransformArray();
		this.buffer2 = this.NewTransformArray();

		// Get a reference to our lean ShadowController
		this.lean = this.GetComponent<ShadowLeanController>();

		// Get a reference to our animation ShadowController
		this.anim = this.GetComponent<ShadowAnimationController>();

		// Set the weight
		this.weight = new Slider(4.0f);

		// Call each ShadowController's ControlledStart() function
		this.ControlledStartAll();
	
	}
	
	// Update is called once per frame
	void Update () {
		this.weight.Tick(Time.deltaTime);

		this.UpdateCoordinates ();

		// Update the lean controller and write its shadow into the buffer
		this.lean.ControlledUpdate();
		this.lean.Encode(this.buffer1);

		// Update the anim controller and write its shadow into the buffer
		this.anim.ControlledUpdate();
		this.anim.Encode(this.buffer2,new Whitelist<string>("Spine1"));

		// Control the weight with the Y and H keys
		/*if (Input.GetKey (KeyCode.Y) == true)
						this.weight.ToMax ();
		if (Input.GetKey (KeyCode.H) == true)
						this.weight.ToMin ();*/

		if (Input.GetKeyDown (KeyCode.T) == true) 
		{
			this.anim.AnimPlay ("dismissing_gesture");
			this.weight.ToMin();
		}

		// Fade out the animation controller if we're finished
		if (anim.IsPlaying() == false)
			this.weight.ToMax();

		BlendSystem.Blend(
			this.buffer1,
			new BlendPair(this.buffer1, this.weight.Value),
			new BlendPair(this.buffer2, this.weight.Inverse));

		// Write the shadow buffer to the display model, starting at the hips
		Shadow.ReadShadowData(
			this.buffer1,
			this.transform.GetChild(0),
			this);

		//Debug.Log(weight);
	}
}
