using UnityEngine;
using System.Collections;

public class ShadowAnimationController : ShadowController
{
    public bool IsPlaying { get { return transform.animation.isPlaying; } }

    private bool started = false;

    public override void ControlledStart()
    {
        this.started = false;
    }

    public override void ControlledUpdate()
    {
        if (this.started == true && this.IsPlaying == false)
        {
            this.started = false;
            this.Coordinator.SendMessage(
                "EvtDoneAnimation",
                SendMessageOptions.DontRequireReceiver);
        }
    }

    #region Messages
    void CmdStartAnimation(string name)
    {
        this.started = true;
        transform.animation.CrossFade(name);
    }
    #endregion
}
