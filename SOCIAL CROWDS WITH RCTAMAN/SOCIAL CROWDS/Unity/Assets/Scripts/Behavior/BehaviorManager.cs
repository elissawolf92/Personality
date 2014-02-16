using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TreeSharpPlus;
using UnityEngine;

public abstract class BehaviorManager : MonoBehaviour
{
	protected List<BehaviorEvent> activeEvents =
		new List<BehaviorEvent>();
	protected List<BehaviorEvent> finishedEvents =
		new List<BehaviorEvent>();
	
	protected List<BehaviorComponent> idleAgents =
		new List<BehaviorComponent>();

	public void RegisterAgent(BehaviorComponent agent)
	{
		this.idleAgents.Add(agent);
	}

	protected void TickActiveTrees()
	{
		foreach (BehaviorEvent evt in this.activeEvents)
		{
			if (evt.Tick() != RunStatus.Running)
			{
				this.finishedEvents.Add(evt);
			}
		}
	}

	protected void TickIdleAgents()
	{
		foreach (BehaviorComponent agent in this.idleAgents)
		{
			agent.TickTree();
		}
	}

	protected void CleanupFinishedTrees()
	{
		foreach (BehaviorEvent evt in this.finishedEvents)
		{
			this.OnEventCompleted(evt);
			BehaviorComponent[] agents = evt.Agents;
			for (int i = 0; i < agents.Count(); i++)
			{
				agents[i].Suspended = false;
				this.idleAgents.Add(agents[i]);
			}
			this.activeEvents.Remove(evt);
			evt.Stop();
		}
	}

	protected void InvokeEvent(BehaviorEvent evt)
	{
		BehaviorComponent[] agents = evt.Agents;
		for (int i = 0; i < agents.Count(); i++)
		{
			agents[i].Suspended = true;
			this.idleAgents.Remove(agents[i]);
		}
		this.activeEvents.Add(evt);
		evt.Start();
	}

	protected void Tick()
	{
		this.TickIdleAgents();
		this.TickActiveTrees();
		this.CleanupFinishedTrees();
	}

	protected abstract void OnEventCompleted(BehaviorEvent evt);
}
