using UnityEngine;
using System.Collections;

public class RecastSteeringController : SteeringController 
{
	private RecastSteeringManager manager = null;
    private int id = -1;

    // Whether we should do a sticky stop when arriving
    public bool StickyStopping = true;

    public RecastSteeringManager.Pushiness pushiness = RecastSteeringManager.Pushiness.PUSHINESS_MEDIUM; //Funda

    public override Vector3 Target 
    { 
        get
        {
            return this.target;
        }
        set
        {
            if (this.manager != null)
            {
                this.manager.SetAgentTarget(this.id, value);
                this.target = value;
                this.State = SteeringState.Navigating;
            }
        }
    }

    public override bool Attached
    {
        get { throw new System.NotImplementedException(); }
        set { throw new System.NotImplementedException(); }
    }


    private SteeringState state = SteeringState.Stopped;
    public SteeringState State
    {
        get
        {
            return this.state;
        }
        private set
        {
            SteeringState oldState = this.state;
            this.state = value;
            if (oldState != value)
                this.SendMessage("SteeringNewState", value, 
                    SendMessageOptions.DontRequireReceiver);
        }
    }

	void Awake()
	{
        this.State = SteeringState.Stopped;
        this.Target = transform.position;
		this.manager = RecastSteeringManager.Instance;
		if (this.manager == null)
			Debug.LogError("Null SteeringManager");
	}
	
    void Start() 
	{
		if (this.manager != null)
		{
			this.id = manager.AddAgent(
				transform.position, 
				this.radius, 
				this.height, 
				this.acceleration, 
				this.maxSpeed);
			// Make sure we get assigned a point on the NavMesh
            if (this.id >= 0)
            {
                this.manager.SetAgentMobile(this.id, true);
                transform.position = this.manager.GetAgentPosition(this.id);
                this.Target = transform.position;
            }
		}

        UpdatePushiness(this.pushiness); //Funda
	}

    void Update()
    {
        if (this.manager != null)
            this.HandleMovement();
        this.HandleOrientation();
        this.lastPosition = transform.position;
    }

    public override bool IsAtTarget()
    {
        return (transform.position - this.target).magnitude
            <= this.stoppingRadius;
    }

    public override bool IsStopped()
    {
        Vector3 vel = manager.GetAgentCurrentVelocity(this.id);
        return (vel.sqrMagnitude < STOP_EPSILON);
    }

    public override bool HasArrived()
    {
        return this.IsAtTarget() && this.IsStopped();
    }

    public override void Move(Vector3 offset)
    {
        // TODO: This is a pretty bad way to do it. - AS
        this.Target = transform.position + offset;
    }

    private void HandleMovement()
    {
        this.manager.EnsureUpdated();
        Vector3 steerPos = manager.GetAgentPosition(this.id);
        steerPos.y -= this.YOffset;
        transform.position = steerPos;

        float dist = (steerPos - this.Target).magnitude;
        if ((this.State != SteeringState.Stopped)
            && (dist < this.stoppingRadius))
            this.Stop(this.StickyStopping);
        else if ((this.State == SteeringState.Navigating)
            && (dist < this.arrivingRadius))
            this.State = SteeringState.Arriving;

        if ((this.StickyStopping == true)
            && ((transform.position - this.lastPosition).sqrMagnitude > 0.0001f)
            && (this.State == SteeringState.Stopped))
            this.Stop(true);
    }

    private void HandleOrientation()
    {
        switch (this.orientationBehavior)
        {
            case OrientationBehavior.LookForward:
                this.desiredOrientation = this.CalcHeadingOrientation();
                break;
            case OrientationBehavior.LookAtTarget:
                this.desiredOrientation = this.CalcTargetOrientation();
                break;
        }

        switch (this.orientationQuality)
        {
            case OrientationQuality.Low:
                transform.rotation = desiredOrientation;
                break;
            case OrientationQuality.High:
                transform.rotation = Quaternion.RotateTowards(
                    transform.rotation,
                    desiredOrientation,
                    driveSpeed * Time.deltaTime);
                break;
        }
    }

    public override void Stop(bool sticky = true)
    {
        if (this.manager != null)
        {
            // "Sticky" means that we move the target to the agent's current
            // position, so that they don't backtrack and try to find the
            // exact target
            if (sticky == true)
            {
                this.manager.SetAgentTarget(this.id, transform.position);
                this.Target = transform.position;
            }
            this.State = SteeringState.Stopped;
        }
    }

    internal void OnDrawGizmos()
    {
        if (this.ShowAgentRadiusGizmo == true)
        {
            Vector3 top = this.transform.position + (Vector3.up * this.height);
            Vector3 bottom = this.transform.position;
            if (Application.isPlaying == false)
            {
                top.y += this.YOffset;
                bottom.y += this.YOffset;
            }

            float diameter = this.radius * 2.0f;
            Matrix4x4 trs = Matrix4x4.TRS(
                Vector3.Lerp(bottom, top, 0.5f),
                Quaternion.LookRotation(Vector3.forward),
                new Vector3(
                    diameter, this.height / 2.0f, diameter));

            GizmoDraw.DrawCylinder(trs, (Color.green + Color.white) / 2);
        }

        if (this.ShowTargetRadiusGizmo == true)
        {
            Vector3 target = 
                (this.Target == Vector3.zero)
                    ? transform.position
                    : this.Target;

            float stoppingDiameter = this.stoppingRadius * 2.0f;
            float arrivingDiameter = this.arrivingRadius * 2.0f;

            Matrix4x4 holdingTrs = Matrix4x4.TRS(
                target,
                Quaternion.LookRotation(Vector3.forward),
                new Vector3(
                    stoppingDiameter, 0.0f, stoppingDiameter));

            Matrix4x4 totalTrs = Matrix4x4.TRS(
                target,
                Quaternion.LookRotation(Vector3.forward),
                new Vector3(
                    arrivingDiameter, 0.0f, arrivingDiameter));

            GizmoDraw.DrawCylinder(holdingTrs, Color.blue);
            GizmoDraw.DrawCylinder(totalTrs, Color.red);
        }

        if (this.ShowDragGizmo == true)
        {
            Vector3 pointArm = this.CalcDragArm();
            Gizmos.matrix = Matrix4x4.identity;

            Gizmos.color = Color.cyan;
            Gizmos.DrawSphere(transform.position, 0.05f);
            Gizmos.DrawLine(transform.position, transform.position + pointArm);
            Gizmos.DrawSphere(transform.position + pointArm, 0.07f);

            if (driveOrientation)
            {
                Vector3 desiredPointArm =
                    desiredOrientation * new Vector3(0, 0, -1);
                Gizmos.color = Color.green;
                Gizmos.DrawLine(transform.position,
                    transform.position + desiredPointArm);
                Gizmos.DrawSphere(transform.position + desiredPointArm, 0.06f);
            }
        }
    }

    private Vector3 CalcDragArm()
    {
        Vector3 dragPoint = lastPosition - transform.forward * dragRadius;
        Vector3 pointArm = dragPoint - transform.position;
        pointArm = pointArm.normalized * dragRadius;
        if (this.planar == true)
            pointArm.y = 0;
        return pointArm;
    }

    private Quaternion CalcHeadingOrientation()
    {
        Vector3 heading;
        switch (this.orientationQuality)
        {
            case OrientationQuality.High:
                heading = -CalcDragArm();
                break;
            default:
                heading = transform.position - lastPosition;
                if (this.planar == true)
                    heading.y = 0;
                break;
        }
        return Quaternion.LookRotation(heading);
    }

    private Quaternion CalcTargetOrientation()
    {
        Vector3 toTarget = this.Target - transform.position;
        if (this.planar == true)
            toTarget.y = 0;
        return Quaternion.LookRotation(toTarget);
    }

    	//Funda
	public void UpdatePushiness(RecastSteeringManager.Pushiness pushiness) {			
		
		this.manager.UpdateAgentPushiness(this.id, pushiness);		
	}

    	//Funda
	public void NewTarget(Vector3 pos, Quaternion orientation)
    {
				
		
        if (this.manager != null)
        {
            this.manager.SetAgentTarget(this.id, pos);
           // this.endOrientation = orientation;
			  
            this.Target = pos;
        }
       
    }
	//Funda
	  public void NewTarget(Vector3 pos)
    {
        this.NewTarget(pos, Quaternion.identity);
    }
}