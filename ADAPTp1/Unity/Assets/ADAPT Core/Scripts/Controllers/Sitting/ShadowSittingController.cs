using UnityEngine;
using System.Collections;

public class ShadowSittingController : ShadowController 
{

    public enum SitState
    {
        SITTING,
        SIT_DOWN,
        STAND_UP,
        STANDING
    }

    public AnimationClip standing;
    public AnimationClip sitDown;
    public AnimationClip standUp;
    public AnimationClip sitting;

    public float blendTime = 2.0f;
    protected float transitionEnd;

    public SitState CurrentState { get; protected set; }

    public bool IsSitting
    { 
        get { return this.CurrentState == SitState.SITTING; } 
    }

    public bool IsStanding
    { 
        get { return this.CurrentState == SitState.STANDING; } 
    }

    public void Reset()
    {
        this.CurrentState = SitState.STANDING;
        this.transitionEnd = 0.0f;
    }

	public override void ControlledStart() 
    {
        CurrentState = SitState.STANDING;
	}

    public override void ControlledUpdate() 
    {
        switch (this.CurrentState)
        {
            case SitState.SIT_DOWN:
                if ((this.transitionEnd - Time.time) <= blendTime)
                {
                    animation.CrossFade(this.sitting.name, blendTime);
                    this.CurrentState = SitState.SITTING;
                }
                break;
            case SitState.STAND_UP:
                if ((this.transitionEnd - Time.time) <= blendTime)
                {
                    animation.CrossFade(this.standing.name, blendTime);
                    this.CurrentState = SitState.STANDING;
                    // Tell the coordinator we're done
                    this.Coordinator.SendMessage(
                        "EvtDoneStanding",
                        SendMessageOptions.DontRequireReceiver);
                }
                break;
        }
	}

    void SitDown()
    {
        if (animation.IsPlaying(this.sitDown.name) == false
            && (   this.CurrentState == SitState.STAND_UP 
                || this.CurrentState == SitState.STANDING))
        {
            animation.CrossFade(this.sitDown.name, blendTime);
            this.transitionEnd = Time.time + this.sitDown.length;
            this.CurrentState = SitState.SIT_DOWN;
        }
    }

    void StandUp()
    {
        if (animation.IsPlaying(this.standUp.name) == false
            && (   this.CurrentState == SitState.SIT_DOWN 
                || this.CurrentState == SitState.SITTING))
        {
            animation.CrossFade(this.standUp.name, blendTime);
            this.transitionEnd = Time.time + this.standUp.length;
            this.CurrentState = SitState.STAND_UP;
        }
    }

    #region Messages
    void CmdSitDown() { this.SitDown(); }
    void CmdStandUp() { this.StandUp(); }
    #endregion
}
