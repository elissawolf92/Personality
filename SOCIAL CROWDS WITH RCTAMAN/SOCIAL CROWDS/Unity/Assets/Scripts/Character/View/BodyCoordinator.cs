using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class BodyCoordinator : ShadowCoordinator
{
    // Interpolation parameters
    public Blender sWeight = null;
    public Blender aWeight = null;
    public Blender hWeight = null;
    public Blender rWeight = null;
    public Blender dWeight = null;

    private Vector3 oldPosition = Vector3.zero;

    [HideInInspector]
    public ShadowSittingController sitting = null;
    [HideInInspector]
    public ShadowLocomotionController locomotion = null;
    [HideInInspector]
    public ShadowAnimationController animation = null;
    [HideInInspector]
    public ShadowHeadLookController headLook = null;
    [HideInInspector]
    public ShadowReachController reach = null;
    [HideInInspector]
    public ShadowRagdollController ragdoll = null;  

    void Awake()
    {
        this.sWeight = new Blender(2.0f);
        this.aWeight = new Blender(2.0f);
        this.hWeight = new Blender(2.0f);
        this.rWeight = new Blender(2.0f);
        this.dWeight = new Blender(2.0f);

        this.sitting = this.GetComponent<ShadowSittingController>();
        this.locomotion = this.GetComponent<ShadowLocomotionController>();
        this.animation = this.GetComponent<ShadowAnimationController>();
        this.headLook = this.GetComponent<ShadowHeadLookController>();
        this.reach = this.GetComponent<ShadowReachController>();
        this.ragdoll = this.GetComponent<ShadowRagdollController>();
    }

    /// <summary>
    /// A rather ugly, complicated update and blend pipeline for four controllers
    /// </summary>
    void Update()
    {
        this.sWeight.Tick(Time.deltaTime);
        this.aWeight.Tick(Time.deltaTime);
        this.hWeight.Tick(Time.deltaTime);
        this.rWeight.Tick(Time.deltaTime);
        this.dWeight.Tick(Time.deltaTime);

        // Set all of the shadows' root transform positions and orientations
        // to the real root position and orientation
        this.UpdateCoordinates();

        // This tells the headlook controller to go into "restricted" mode
        float speed = (this.oldPosition - transform.position).sqrMagnitude;
        if (speed > 0.0001f)
            this.RelayMessage("CmdSetRestricted", true);
        else
            this.RelayMessage("CmdSetRestricted", false);
        this.oldPosition = transform.position;

        ShadowTransform[] lg = this.BlendLegsAndSitting();
        ShadowTransform[] anim = this.BlendAnimations(lg);
        ShadowTransform[] head = this.BlendHeadLook(anim);
        ShadowTransform[] reach = this.BlendReach(head);
        ShadowTransform[] rag = this.BlendRagdoll(reach);

        // Special management of the ragdoll controller for telling it
        // that it's fully faded out and done falling
        if (this.dWeight.IsMin == true)
            this.ragdoll.IsFalling = false;

        Shadow.ReadShadowData(rag, transform.GetChild(0), this);
    }

    private ShadowTransform[] BlendLegsAndSitting()
    {
        // Update the leg controller
        this.locomotion.ControlledUpdate();
        ShadowTransform[] legs =
            this.locomotion.Encode(this.NewTransformArray());

        // If we don't need to blend the gesture controller, don't bother
        if (sWeight.IsMin == true)
            return legs;

        this.sitting.ControlledUpdate();
        ShadowTransform[] sitBody = 
            this.sitting.Encode(this.NewTransformArray());

        return BlendSystem.Blend(
            this.NewTransformArray(),
            new BlendPair(legs, sWeight.Inverse),
            new BlendPair(sitBody, sWeight.Value));
    }

    private ShadowTransform[] BlendController(
        ShadowController controller,
        ShadowTransform[] input,
        Blender weight,
        FilterList<string> filter = null)
    {
        if (weight.IsMin == true)
            return input;

        // Update the target controller from that blend
        if (filter == null)
            controller.Decode(input);
        else
            controller.Decode(input, filter);
        controller.ControlledUpdate();
        ShadowTransform[] result 
            = controller.Encode(this.NewTransformArray());

        return BlendSystem.Blend(
            this.NewTransformArray(),
            new BlendPair(input, weight.Inverse),
            new BlendPair(result, weight.Value));
    }

    private ShadowTransform[] BlendAnimations(ShadowTransform[] input)
    {
        return BlendController(
            this.animation,
            input,
            this.aWeight,
            // We want to filter out the upper body from the sitting
            // and locomotion blend when we're doing the animation on top
            new Blacklist<string>("Spine1"));
    }

    private ShadowTransform[] BlendHeadLook(ShadowTransform[] input)
    {
        return BlendController(
            this.headLook,
            input,
            this.hWeight);
    }

    private ShadowTransform[] BlendReach(ShadowTransform[] input)
    {
        return BlendController(
            this.reach,
            input,
            this.rWeight,
            new Blacklist<string>("LeftArm"));
    }

    private ShadowTransform[] BlendRagdoll(ShadowTransform[] input)
    {
        if (this.dWeight.IsMin == true)
            this.ragdoll.Decode(
                input, 
                new Blacklist<string>("LeftUpLeg", "RightUpLeg"));
        this.ragdoll.ControlledUpdate();
        ShadowTransform[] result
            = this.ragdoll.Encode(this.NewTransformArray());

        return BlendSystem.Blend(
            this.NewTransformArray(),
            new BlendPair(input, this.dWeight.Inverse),
            new BlendPair(result, this.dWeight.Value));
    }

    #region Controller Events
    void EvtDoneStanding() { sWeight.ToMin(); }
    void EvtDoneAnimation() { aWeight.ToMin(); }
    void EvtBeginFalling() { this.dWeight.ForceMax(); }
    void EvtDoneFalling() { this.dWeight.ToMin(); }
    #endregion
}
