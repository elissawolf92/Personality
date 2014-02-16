using UnityEngine;
using System.Collections;

/// <summary>
/// A ShadowController that plays the idle animation.
/// </summary>
[RequireComponent(typeof(ShadowCoordinator))]
public class ShadowIdleController : ShadowController
{
    public string AnimationName = "idle_1";

    /// <summary>
    /// Plays the named animation
    /// </summary>
    public override void ControlledStart()
    {
        this.animation[this.AnimationName].wrapMode = WrapMode.Loop;
        this.animation.Play(this.AnimationName);
    }
}
