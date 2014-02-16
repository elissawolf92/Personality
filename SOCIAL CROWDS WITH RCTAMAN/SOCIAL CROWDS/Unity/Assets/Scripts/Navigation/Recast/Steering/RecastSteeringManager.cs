using UnityEngine;

using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;

public class RecastSteeringManager : MonoBehaviour
{
    private static RecastSteeringManager instance = null;
    public static RecastSteeringManager Instance
    {
        get
        {
            if (instance == null)
            {
                UnityEngine.Object[] objs =
                    GameObject.FindObjectsOfType(typeof(RecastSteeringManager));
                if (objs.Length == 1)
                    instance = (RecastSteeringManager)objs[0];
                else if (objs.Length == 0)
                    Debug.LogError("No SteeringManager found");
                else
                    Debug.LogError("Multiple SteeringManagers found");
            }
            return instance;
        }
    }

	public Navmesh navmesh = null;
	public int maxAgents = 10000;
	public float maxAgentRadius = 0.5f;

    bool initialized = false;
    private int lastUpdateFrame = -1;
    protected IntPtr steeringManager;

    public List<RecastSteeringController> agents;
	
    public enum Pushiness 
    { 
        PUSHINESS_LOW, 
        PUSHINESS_MEDIUM, 
        PUSHINESS_HIGH 
    };

    public enum NavigationQuality 
    { 
        NAVIGATIONQUALITY_LOW, 
        NAVIGATIONQUALITY_MED, 
        NAVIGATIONQUALITY_HIGH 
    };
	
	void Update()
    {
        this.EnsureUpdated();
    }

    public void EnsureUpdated()
    {
        if (this.lastUpdateFrame != Time.frameCount)
        {
            this.DoUpdate();
            this.lastUpdateFrame = Time.frameCount;
        }
    }

    private void DoUpdate()
    {
        if (initialized)
            NativeUpdate(this.steeringManager, Time.deltaTime);
    }

    void OnEnable()
    {
        if (this.navmesh != null)
        {
            if (this.navmesh.Data != null)
            {
                this.steeringManager = NativeCreateSteeringManager();
                NativeInit(
                    this.steeringManager,
                    this.navmesh.Data,
                    this.navmesh.Data.Length,
                    this.maxAgents,
                    this.maxAgentRadius);
                this.initialized = true;
            }
            else
            {
                Debug.LogError("Null Navmesh");
            }
        }
        else
        {
            Debug.LogError("No Navmesh");
        }
    }
	
	void OnDisable()
	{
		if (initialized)
		{
            NativeDestroySteeringManager(this.steeringManager);
		}
	}
	
	public int AddAgent(Vector3 vPos, float radius, float height, float accel, float maxSpeed)
	{
		if (initialized)
            return NativeAddAgent(this.steeringManager, vPos, radius, height, accel, maxSpeed);
		return -1;
	}

    public void RemoveAgent(int agent)
    {
        if (initialized)
            NativeRemoveAgent(this.steeringManager, agent);
    }

    public Vector3 GetClosestWalkablePosition(Vector3 pos)
    {
        if (initialized)
            return NativeGetClosestWalkablePosition(this.steeringManager, pos);
        return Vector3.zero;
    }

    public Vector3 GetAgentPosition(int agent)
	{
		if (initialized)
            return NativeGetAgentPosition(this.steeringManager, agent);
		return Vector3.zero;
	}

    public Vector3 GetAgentCurrentVelocity(int agent)
	{
		if (initialized)
            return NativeGetAgentCurrentVelocity(this.steeringManager, agent);
		return Vector3.zero;
	}

    public Vector3 GetAgentDesiredVelocity(int agent)
	{
		if (initialized)
            return NativeGetAgentDesiredVelocity(this.steeringManager, agent);
		return Vector3.zero;
	}

    public void UpdateAgentNavigationQuality(int agent, NavigationQuality nq)
    {
        if (initialized)
            NativeUpdateAgentNavigationQuality(this.steeringManager, agent, nq);
    }

    public void UpdateAgentPushiness(int agent, Pushiness p)
    {
        if (initialized)
            NativeUpdateAgentPushiness(this.steeringManager, agent, p);
    }

    public void UpdateAgentHoldingRadius(int agent, float radius)
    {
        if (initialized)
            NativeUpdateAgentHoldingRadius(this.steeringManager, agent, radius);
    }

    public void UpdateAgentMaxSpeed(int agent, float speed)
    {
        if (initialized)
            NativeUpdateAgentMaxSpeed(this.steeringManager, agent, speed);
    }

    public void UpdateAgentMaxAcceleration(int agent, float accel)
    {
        if (initialized)
            NativeUpdateAgentMaxAcceleration(this.steeringManager, agent, accel);
    }

    // TODO: Investigate the usefulness of this function
    public void SetAgentMobile(int agent, bool mobile)
    {
        if (initialized)
            NativeSetAgentMobile(this.steeringManager, agent, mobile);
    }

    public void SetAgentTarget(int agent, Vector3 vGoal)
    {
        if (initialized)
            NativeSetAgentTarget(this.steeringManager, agent, vGoal);
    }

    [DllImport("Steering_RecastDetour", EntryPoint = "createSteeringManager")]
    public static extern IntPtr NativeCreateSteeringManager();
    [DllImport("Steering_RecastDetour", EntryPoint = "destroySteeringManager")]
    public static extern void NativeDestroySteeringManager(IntPtr steeringManager);

    [DllImport("Steering_RecastDetour", EntryPoint = "init")]
    public static extern bool NativeInit(
        IntPtr steeringManager,
        [MarshalAs(UnmanagedType.LPArray)] byte[] data,
        int dataSize,
        int maxAgents,
        float maxAgentRadius);

	[DllImport("Steering_RecastDetour", EntryPoint="update")]
    public static extern IntPtr NativeUpdate(IntPtr steeringManager, float dT);

	[DllImport("Steering_RecastDetour", EntryPoint="addAgent")]
    public static extern int NativeAddAgent(
        IntPtr steeringManager, 
        [MarshalAs(UnmanagedType.LPArray)] Vector3 vPos,
		float radius, 
		float height, 
		float accel, 
		float maxSpeed);
    [DllImport("Steering_RecastDetour", EntryPoint = "removeAgent")]
    public static extern void NativeRemoveAgent(
        IntPtr steeringManager,
        int agent);

    [DllImport("Steering_RecastDetour", EntryPoint = "updateAgentNavigationQuality")]
    public static extern void NativeUpdateAgentNavigationQuality(IntPtr steeringManager, int agent, NavigationQuality nq);
    [DllImport("Steering_RecastDetour", EntryPoint = "updateAgentPushiness")]
    public static extern void NativeUpdateAgentPushiness(IntPtr steeringManager, int agent, Pushiness p);
    [DllImport("Steering_RecastDetour", EntryPoint = "updateAgentHoldingRadius")]
    public static extern void NativeUpdateAgentHoldingRadius(IntPtr steeringManager, int agent, float radius);
    [DllImport("Steering_RecastDetour", EntryPoint = "updateAgentMaxSpeed")]
    public static extern void NativeUpdateAgentMaxSpeed(IntPtr steeringManager, int agent, float speed);
    [DllImport("Steering_RecastDetour", EntryPoint = "updateAgentMaxAcceleration")]
    public static extern void NativeUpdateAgentMaxAcceleration(IntPtr steeringManager, int agent, float accel);

    [DllImport("Steering_RecastDetour", EntryPoint = "setAgentTarget")]
    public static extern int NativeSetAgentTarget(
        IntPtr steeringManager,
        int agent,
        [MarshalAs(UnmanagedType.LPArray)] Vector3 vPos);
    [DllImport("Steering_RecastDetour", EntryPoint = "setAgentMobile")]
    public static extern void NativeSetAgentMobile(IntPtr steeringManager, int agent, bool mobile);

    [DllImport("Steering_RecastDetour", EntryPoint = "getAgentPosition")]
    public static extern Vector3 NativeGetAgentPosition(IntPtr steeringManager, int agent);
    [DllImport("Steering_RecastDetour", EntryPoint = "getAgentCurrentVelocity")]
    public static extern Vector3 NativeGetAgentCurrentVelocity(IntPtr steeringManager, int agent);
    [DllImport("Steering_RecastDetour", EntryPoint = "getAgentDesiredVelocity")]
    public static extern Vector3 NativeGetAgentDesiredVelocity(IntPtr steeringManager, int agent);

    [DllImport("Steering_RecastDetour", EntryPoint = "getClosestWalkablePosition")]
    public static extern Vector3 NativeGetClosestWalkablePosition(
        IntPtr steeringManager,
        [MarshalAs(UnmanagedType.LPArray)] Vector3 pos);
}

/*
        if (orientate && status==MoveStatus.EnRoute) {
			// If we're close enough to the end-point, turn to face it
            if (targetName != null && (targetPos - transform.position).magnitude < turnAtDistance + holdingRadius)
            {
                if(holdingRadius > 0)
                    dho.desiredOrientation = Quaternion.LookRotation(targetPos - transform.position);
                else
                    dho.desiredOrientation = endRotation;
			// Drivespeed is how fast you turn? (Less for walking, more for ending)
                dho.driveSpeed = arrivingOrientSpeed;
            }
            else
            {
			// If we're moving, significantly
                if(vel.sqrMagnitude > 0.01f)
                {
                    dho.desiredOrientation = Quaternion.LookRotation(vel);
                    dho.driveSpeed = walkingOrientSpeed;
                }
            }
        }
*/
