using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// A very simple shadow coordinator that expects only one
/// ShadowController, and gives it full control of the body
/// </summary>
public class DummyHeadlookCoordinator : ShadowCoordinator
{
    public float interp = 0.999f;

    [HideInInspector]
    public ShadowController dummyController;

    [HideInInspector]
    public ShadowController headlookController;


    void Start()
    {
        this.dummyController = this.shadowControllers["ShadowDummyController"];
        this.headlookController = this.shadowControllers["ShadowHeadLookController"];
    }

    void Update()
    {
        UpdateCoordinates();
        UpdateInterpolation();

        // Project the idle controller onto the headLook controller
        // each frame, since the headLook controller calculates a full
        // offset each frame from the base pose to the gazing pose
        headlookController.Decode(
            this.dummyController.Encode(
                this.NewTransformArray()));
        this.headlookController.ControlledUpdate();

        ShadowTransform[] dummy = 
            dummyController.Encode(this.NewTransformArray());
        ShadowTransform[] headLook =
            this.headlookController.Encode(this.NewTransformArray());

        ShadowTransform[] blend = BlendSystem.Blend(
            this.NewTransformArray(),
            new BlendPair(headLook, interp),
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
