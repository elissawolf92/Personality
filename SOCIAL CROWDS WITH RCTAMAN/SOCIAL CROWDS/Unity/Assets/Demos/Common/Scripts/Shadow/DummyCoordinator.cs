using UnityEngine;
using System.Collections;


using System.Collections.Generic;
using System.Linq;

/// <summary>
/// A very simple shadow coordinator that expects only one
/// ShadowController, and gives it full control of the body
/// </summary>
public class DummyCoordinator : ShadowCoordinator 
{
    private ShadowTransform[] buffer = null;
    private ShadowController controller = null;

	void Update() 
    {
        if (this.buffer == null)
            this.buffer = this.NewTransformArray();

        if (this.controller == null)
            foreach (ShadowController sc in this.shadowControllers.Values)
                this.controller = sc;

        UpdateCoordinates();
        this.controller.ControlledUpdate();
        this.controller.Encode(this.buffer);

        Shadow.ReadShadowData(
            this.buffer, 
            transform.GetChild(0), 
            this);
	}
}
