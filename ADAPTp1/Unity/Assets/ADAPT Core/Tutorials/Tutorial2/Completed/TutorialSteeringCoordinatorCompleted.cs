using UnityEngine;
using System.Collections;

public class TutorialSteeringCoordinatorCompleted : ShadowCoordinator
{
    protected ShadowTransform[] buffer1 = null;
    protected ShadowLocomotionController loco = null;

    void Start()
    {
        // Allocate space for a buffer for storing and passing shadow poses
        this.buffer1 = this.NewTransformArray();

        // Get a reference to our lean ShadowController
        this.loco = this.GetComponent<ShadowLocomotionController>();

        // Call each ShadowController's ControlledStart() function
        this.ControlledStartAll();
    }

    void Update()
    {
        // Move the root position of each shadow to match the display model
        this.UpdateCoordinates();

        // Update the lean controller and write its shadow into the buffer
        this.loco.ControlledUpdate();
        this.loco.Encode(this.buffer1);

        // Write the shadow buffer to the display model, starting at the hips
        Shadow.ReadShadowData(
            this.buffer1,
            this.transform.GetChild(0),
            this);
    }
}
