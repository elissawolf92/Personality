using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ShadowTransform
{
    public static bool IsValid(ShadowTransform t)
    {
        return (t != null && t.valid == true);
    }

    public bool valid = false;
    public Vector3 Position;
    public Quaternion Rotation;

    /// <summary>
    /// Shallow-clones a transform
    /// </summary>
    /// <param name="t">The transform to shallow copy</param>
    public ShadowTransform()
    {
        this.valid = false;
        this.Position = default(Vector3);
        this.Rotation = default(Quaternion);
    }

    public void ReadFrom(Transform t, bool valid = true)
    {
        this.Position = t.localPosition;
        this.Rotation = t.localRotation;
        this.valid = valid;
    }

    public void ReadFrom(Vector3 pos, Quaternion rot, bool valid = true)
    {
        this.Position = pos;
        this.Rotation = rot;
        this.valid = valid;
    }

    public void WriteTo(Transform t)
    {
        t.localPosition = this.Position;
        t.localRotation = this.Rotation;
    }
}
