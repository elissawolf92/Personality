using System;
using System.Reflection;
using System.Collections.Generic;
using TreeSharpPlus;
using UnityEngine;

/// <summary>
/// The ADAPTAgent class is the base class for all agents that use
/// ADAPT behavior trees. If an agent has autonomy, it should tick
/// its active behavior tree. If that autonomy is suspended, that
/// tree is stored as a dormant tree and is not ticked. 
/// </summary>
public class BehaviorComponent : MonoBehaviour
{
	private bool suspended = false;
	public bool Suspended
	{
		get
		{
			return this.suspended;
		}
		set
		{
			if (this.suspended != value)
			{
				this.suspended = value;
				if (value == true)
					this.StopTree();
				else
					this.StartTree();
			}
		}
	}

	private ITreeNode defaultTree = null;
	public ITreeNode DefaultTree
	{
		get
		{
			return this.defaultTree;
		}
		set
		{
			this.StopTree();
			this.defaultTree = value;
			if (this.Suspended == false)
				this.StartTree();
		}
	}

	/// <summary>
	/// Stops the agent's active tree.
	/// </summary>
	private void StopTree()
	{
		if (this.DefaultTree != null)
			this.DefaultTree.Stop();
	}

	/// <summary>
	/// Starts the agent's active tree.
	/// </summary>
	private void StartTree()
	{
		if (this.DefaultTree != null)
			this.DefaultTree.Start();
	}

	/// <summary>
	/// Ticks the agent's active tree.
	/// </summary>
	public void TickTree()
	{
		if (suspended == false && this.DefaultTree != null)
			this.DefaultTree.Tick();
	}
	
	void FixedUpdate()
	{
		this.TickTree();
	}
}

