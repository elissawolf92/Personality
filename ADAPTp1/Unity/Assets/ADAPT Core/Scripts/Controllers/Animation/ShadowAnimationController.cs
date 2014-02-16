using UnityEngine;
using System.Collections;

public class ShadowAnimationController : ShadowController
{
    private bool started = false;

    public bool IsPlaying()
    {
        return transform.animation.isPlaying;
    }

    public bool IsPlaying(string name)
    {
        return transform.animation.IsPlaying(name);
    }

    public override void ControlledStart()
    {
        this.started = false;
    }

    public override void ControlledUpdate()
    {
        if (this.started == true && this.IsPlaying() == false)
        {
            this.started = false;
            this.Coordinator.SendMessage(
                "EvtDoneAnimation",
                SendMessageOptions.DontRequireReceiver);
        }
    }

    public void AnimPlay(string name)
    {
        this.started = true;
        transform.animation.CrossFade(name);
    }

    public void AnimStop()
    {
        this.started = false;
        transform.animation.Stop();
    }

    #region Messages
    void CmdStartAnimation(string name)
    {
        this.AnimPlay(name);
    }

    void CmdStopAnimation()
    {
        this.AnimStop();
    }
    #endregion
}
