using UnityEngine;
using System;
using System.Collections;


/// <summary>
/// A very basic graphical interface for what should be treated as basic motor
/// skills performed by the character. These actions include no preconditions
/// and can fail if executed in impossible/nonsensical situations. In this
/// case, the functions will usually try their best.
/// 
/// Used with a BodyCoordinator and/or a SteeringController. Needs at least
/// one on the same GameObject to be able to do anything.
/// </summary>
public class BodyInterface : MonoBehaviour 
{
    private BodyCoordinator _coordinator = null;
    private SteeringController _steering = null;

    public BodyCoordinator Coordinator
    {
        get
        {
            if (this._coordinator == null)
                throw new ApplicationException(
                    this.gameObject.name + ": No BodyCoordinator found!");
            return this._coordinator;
        }
    }

    public SteeringController Steering
    {
        get
        {
            if (this._steering == null)
                throw new ApplicationException(
                    this.gameObject.name + ": No SteeringController found!");
            return this._steering;
        }
    }

	// Use this for initialization
	void Awake() 
    {
        this._coordinator = this.gameObject.GetComponent<BodyCoordinator>();
        this._steering = this.gameObject.GetComponent<SteeringController>();
	}

    #region Reach Commands
    public void ReachSetActive(bool status)
    {
        if (status == true)
            this.Coordinator.rWeight.ToMax();
        else
            this.Coordinator.rWeight.ToMin();
    }

    public void ReachSetTarget(Vector3 target)
    {
        this.Coordinator.RelayMessage("CmdSetReachTarget", target);
    }

    public void ReachFor(Vector3 target)
    {
        this.ReachSetActive(true);
        this.ReachSetTarget(target);
    }

    public void ReachStop()
    {
        this.ReachSetActive(false);
    }

    public bool ReachHasReached()
    {
        // TODO: Kind of a clumsy way to do it. Maybe we should check the 
        //       hand position of the display model instead? - AS
        return this.Coordinator.reach.HasReached 
            && this.Coordinator.rWeight.IsMax;
    }
    #endregion

    #region HeadLook Commands
    public void HeadLookSetActive(bool status)
    {
        if (status == true)
            this.Coordinator.hWeight.ToMax();
        else
            this.Coordinator.hWeight.ToMin();
    }

    public void HeadLookSetTarget(Vector3 target)
    {
        this.Coordinator.RelayMessage("CmdSetHeadLookTarget", target);
    }

    public void HeadLookAt(Vector3 target)
    {
        this.HeadLookSetActive(true);
        this.HeadLookSetTarget(target);
    }

    public void HeadLookStop()
    {
        this.HeadLookSetActive(false);
    }
    #endregion

    #region Animation Commands
    public void StartAnimation(string name)
    {
        this.Coordinator.RelayMessage("CmdStartAnimation", name);
        this.Coordinator.aWeight.ToMax();
    }
    #endregion

    #region Sitting Commands
    public void SitDown()
    {
        this.Coordinator.RelayMessage("CmdSitDown");
        this.Coordinator.sWeight.ToMax();
    }

    public void StandUp()
    {
        this.Coordinator.RelayMessage("CmdStandUp");
    }
    #endregion

    #region Navigation Commands
    public void NavSetTarget(Vector3 target)
    {
        this.Steering.Target = target;
    }

    public void NavStop(bool sticky = true)
    {
        this.Steering.Stop(sticky);
    }

    public bool NavIsStopped()
    {
        return this.Steering.IsStopped();
    }

    public bool NavIsAtTarget()
    {
        return this.Steering.IsAtTarget();
    }

    public bool NavHasArrived()
    {
        return this.Steering.HasArrived();
    }

    public void NavSetDesiredOrientation(Quaternion desired)
    {
        this.Steering.desiredOrientation = desired;
    }

    public void NavSetOrientationBehavior(OrientationBehavior behavior)
    {
        this.Steering.orientationBehavior = behavior;
    }

    public void NavSetAttached(bool value)
    {
        this.Steering.Attached = value;
    }
    #endregion
}   
