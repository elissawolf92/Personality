using UnityEngine;
using System.Collections;

public class ShadowLeanControllerCompleted : ShadowController
{
    public Transform spine;

    public override void ControlledStart()
    {
        // Find the cloned version of the bone we were given in the inspector
        // so that we're editing our own shadow, not the display model
        this.spine = this.shadow.GetBone(this.spine);
    }

    public override void ControlledUpdate()
    {
        // Get the current euler angle rotation
        Vector3 rot = spine.rotation.eulerAngles;

        // Detect key input and add or subtract from the x rotation (scaling
        // by deltaTime to make this speed independent from the frame rate)
        if (Input.GetKey(KeyCode.R))
            rot.x -= Time.deltaTime * 50.0f;
        if (Input.GetKey(KeyCode.F))
            rot.x += Time.deltaTime * 50.0f;

        // Apply the new rotation
        spine.rotation = Quaternion.Euler(rot);
    }
}
