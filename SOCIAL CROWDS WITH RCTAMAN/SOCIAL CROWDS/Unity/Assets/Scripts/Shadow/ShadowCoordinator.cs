using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public abstract class ShadowCoordinator : MonoBehaviour
{
    protected Dictionary<string, ShadowController> shadowControllers = null;
    protected Dictionary<string, int> boneKeys = null;

    #region BoneKey Functions
    protected void ReadBone(Transform t)
    {
        int curBone = this.boneKeys.Count;
        this.boneKeys[t.name] = curBone;
        foreach (Transform child in t)
            this.ReadBone(child);
    }

    protected void InitBoneKeys()
    {
        this.boneKeys = new Dictionary<string, int>();
        this.ReadBone(this.transform);
    }

    public void ClearTransformArray(ShadowTransform[] array)
    {
        for (int i = 0; i < array.Length; i++)
            array[i].valid = false;
    }

    public ShadowTransform[] NewTransformArray()
    {
        ShadowTransform[] buffer = new ShadowTransform[this.boneKeys.Count];
        for (int i = 0; i < buffer.Length; i++)
            buffer[i] = new ShadowTransform();
        return buffer;
    }

    public int GetBoneKey(string name)
    {
        return this.boneKeys[name];
    }
    #endregion

    #region Controller Functions
    public ShadowController GetController(string name)
    {
        return this.shadowControllers[name];
    }

    public void RegisterController(ShadowController controller)
    {
        if (this.shadowControllers == null)
            this.shadowControllers = new Dictionary<string, ShadowController>();

        if (this.boneKeys == null)
            this.InitBoneKeys();

        string name = controller.GetType().ToString();
        this.shadowControllers[name] = controller;
        controller.shadow = new Shadow(this.transform, controller);

        // The controller is now ready to wake up
        controller.ControlledAwake();
    }

    protected void UpdateCoordinates()
    {
        foreach (ShadowController controller in this.shadowControllers.Values)
        {
            controller.shadow.transform.position = transform.position;
            controller.shadow.transform.rotation = transform.rotation;
        }
    }
    #endregion

    #region MonoBehavior Functions
    /// <summary>
    /// Propagates the Start function to all registerred children in order
    /// </summary>
    void Start()
    {
        if (this.shadowControllers != null)
            foreach (ShadowController sc in this.shadowControllers.Values)
                sc.ControlledStart();
    }

    /// <summary>
    /// Override this function to execute your controller update order pipeline
    /// </summary>
    void Update()
    {
        throw new NotImplementedException("No Update() function!");
    }

    /// <summary>
    /// Propagates the LateUpdate function to all registerred children in order
    /// </summary>
    void LateUpdate()
    {
        if (this.shadowControllers != null)
            foreach (ShadowController sc in this.shadowControllers.Values)
                sc.ControlledLateUpdate();
    }

    /// <summary>
    /// Propagates the FixedUpdate function to all registerred children in order
    /// </summary>
    void FixedUpdate()
    {
        if (this.shadowControllers != null)
            foreach (ShadowController sc in this.shadowControllers.Values)
                sc.ControlledFixedUpdate();
    }
    #endregion

    #region Messages
    public void RelayMessage(string methodName)
    {
        this.SendMessage(methodName, SendMessageOptions.DontRequireReceiver);
        foreach (ShadowController controller in this.shadowControllers.Values)
            controller.SendMessage(methodName, SendMessageOptions.DontRequireReceiver);
    }

    public void RelayMessage(string methodName, object value)
    {
        this.SendMessage(methodName, value, SendMessageOptions.DontRequireReceiver);
        foreach (ShadowController controller in this.shadowControllers.Values)
            controller.SendMessage(methodName, value, SendMessageOptions.DontRequireReceiver);
    }
    #endregion
}
