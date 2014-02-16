using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TreeSharpPlus;

public interface IBehaviorUpdate
{
    /// <summary>
    /// A regular pulse to update anything requiring being ticked.
    /// </summary>
    /// <param name="deltaTime">The deltaTime for this update pulse</param>
    /// <returns>true if the manager should continue updating this object,
    /// false if the manager should forget about this object and never
    /// update it again</returns>
    bool BehaviorUpdate(float deltaTime);
}

public class BehaviorManager : MonoBehaviour
{
    public bool Active = true;

    private static BehaviorManager instance = null;
    public static BehaviorManager Instance
    {
        get
        {
            if (instance == null)
                throw new ApplicationException("No BehaviorManager found");
            return instance;
        }
    }

    private List<IBehaviorUpdate> receivers = null;

    public float updateTime = 0.05f;
    private float nextUpdate = 0.0f;

    void OnEnable()
    {
        if (instance != null)
            throw new ApplicationException("Multiple BehaviorManagers found");
        instance = this;
        this.receivers = new List<IBehaviorUpdate>();
    }

    void Start()
    {
        this.nextUpdate = Time.time + this.updateTime;
    }

    void FixedUpdate()
    {
        if (Time.time > this.nextUpdate)
        {
            this.BehaviorUpdate(this.updateTime);
            this.nextUpdate += this.updateTime;
        }
    }

    private void UpdateReceivers(float updateTime)
    {
        for (int i = this.receivers.Count - 1; i >= 0; i--)
            if (this.receivers[i].BehaviorUpdate(updateTime) == false)
                this.receivers.RemoveAt(i);
    }

    /// <summary>
    /// Updates all events and agents for a behavior tick
    /// </summary>
    // TODO: Spread this out across frames do we don't get a chug
    // every time we do a behavior update
    private void BehaviorUpdate(float updateTime)
    {
        if (this.Active == true)
            this.UpdateReceivers(updateTime);
    }

    public static void RegisterReceiver(IBehaviorUpdate receiver)
    {
        Instance.receivers.Add(receiver);
    }
}
