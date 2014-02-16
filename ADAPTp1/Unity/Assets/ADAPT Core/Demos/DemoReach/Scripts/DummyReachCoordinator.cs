using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// A very simple shadow coordinator that expects only one
/// ShadowController, and gives it full control of the body
/// </summary>
public class DummyReachCoordinator : ShadowCoordinator
{
    public float interp = 0.999f;

    [HideInInspector]
    public ShadowController dummyController;

    [HideInInspector]
    public ShadowController reachController;

    void Start()
    {
        this.dummyController = this.shadowControllers["ShadowDummyController"];
        this.reachController = this.shadowControllers["ShadowReachController"];
    }

    // Update is usually where we do the blending and fading
    void Update()
    {
        this.UpdateCoordinates();
        this.UpdateInterpolation();
        
        // Project the idle controller onto the reach controller
        // each frame, except for the hierarchy beginning with the
        // LeftArm joint, otherwise the reach control would be
        // overwritten each frame
        this.reachController.Decode(
            this.dummyController.Encode(
                this.NewTransformArray()),
                new Blacklist<string>("LeftArm"));
        this.reachController.ControlledUpdate();

        ShadowTransform[] dummy =
            this.dummyController.Encode(this.NewTransformArray());
        ShadowTransform[] reach =
            this.reachController.Encode(this.NewTransformArray());

        ShadowTransform[] blend = 
            BlendSystem.Blend(
                this.NewTransformArray(),
                new BlendPair(reach, interp),
                new BlendPair(dummy, 1.0f - interp));

        Shadow.ReadShadowData(blend, transform.GetChild(0), this);
    }

    private void UpdateInterpolation()
    {
        if (Input.GetKey(KeyCode.Y))
            interp += Time.deltaTime * 2.0f;
        if (Input.GetKey(KeyCode.H))
            interp -= Time.deltaTime * 2.0f;
        interp = Mathf.Clamp(interp, 0.001f, 0.999f);
    }
}
