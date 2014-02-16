using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// A coordinator for blending locomotion and a ragdoll
/// </summary>
public class DummyRagdollCoordinator : ShadowCoordinator
{
    public Slider dWeight = null;

    private ShadowRagdollController ragdoll = null;
    private ShadowController locomotion = null;

    private ShadowTransform[] ragdollPose = null;
    private ShadowTransform[] locomotionPose = null;
	
    void Awake()
    {
        CharacterController cc = GetComponent<CharacterController>();
        cc.collider.isTrigger = true;
        this.dWeight = new Slider(2.0f);
        this.ragdollPose = this.NewTransformArray();
        this.locomotionPose = this.NewTransformArray();
    }

	void Update()
    {
        if (this.ragdoll == null)
            this.ragdoll =
                this.shadowControllers["ShadowRagdollController"]
                as ShadowRagdollController;
        if (this.locomotion == null)
            this.locomotion = 
                this.shadowControllers["ShadowLocomotionController"];

        this.dWeight.Tick(Time.deltaTime);

        // Set all of the shadows' root transform positions and orientations
        // to the real root position and orientation
        UpdateCoordinates();

        this.locomotion.ControlledUpdate();
        this.locomotion.Encode(this.locomotionPose);

        // Special management of the ragdoll controller for telling it
        // that it's fully faded out and done falling
        if (this.dWeight.IsMin == true)
            this.ragdoll.IsFalling = false;

        // Reuse the locomotion pose buffer
        this.locomotionPose = this.BlendRagdoll(this.locomotionPose);
        Shadow.ReadShadowData(
            this.locomotionPose,
            hips, 
            this);
    }

    private ShadowTransform[] BlendRagdoll(ShadowTransform[] input)
    {
        if (this.dWeight.IsMin == true)
            this.ragdoll.Decode(
                input,
                new Blacklist<string>("LeftUpLeg", "RightUpLeg"));
        this.ragdoll.ControlledUpdate();
        ShadowTransform[] result
            = this.ragdoll.Encode(this.ragdollPose);

        return BlendSystem.Blend(
            this.NewTransformArray(),
            new BlendPair(input, this.dWeight.Inverse),
            new BlendPair(result, this.dWeight.Value));
    }

    #region Controller Events
    void EvtBeginFalling() { this.dWeight.ForceMax(); }
    void EvtDoneFalling() { this.dWeight.ToMin(); }
    #endregion
}
