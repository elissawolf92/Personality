using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

[RequireComponent(typeof(ShadowCoordinator))]
public abstract class ShadowController : MonoBehaviour 
{
    public virtual void ControlledAwake() { }
    public virtual void ControlledStart() { }
    public virtual void ControlledUpdate() { }
    public virtual void ControlledFixedUpdate() { }
    public virtual void ControlledLateUpdate() { }

    public Shadow shadow = null;
    public bool showGizmo = true;

    private ShadowCoordinator _coordinator = null;
    public ShadowCoordinator Coordinator { get { return this._coordinator; } }

    // Ignore these objects in the base character model when cloning a shadow
    // for this controller
    public string[] ignoreTransforms = { "MixamoMesh" };

    new public Transform transform { get { return this.shadow.transform; } }
    new public Animation animation { get { return this.shadow.animation; } }

    void Awake()
    {
        if (this.enabled == true)
        {
            this._coordinator =
                this.gameObject.GetComponent<ShadowCoordinator>();
            this._coordinator.RegisterController(this);
        }
    }

    public ShadowTransform[] Encode(ShadowTransform[] buffer)
    {
        return this.shadow.Encode(buffer);
    }

    public ShadowTransform[] Encode(
        ShadowTransform[] buffer, 
        FilterList<string> nameFilter)
    {
        return this.shadow.Encode(buffer, nameFilter);
    }

    public void Decode(ShadowTransform[] data)
    {
        this.shadow.Decode(data);
    }

    public void Decode(
        ShadowTransform[] data,
        FilterList<string> nameFilter)
    {
        this.shadow.Decode(data, nameFilter);
    }

    // This is just here so we can enable or 
    // disable the script from the inspector
    void Update() { }
}
