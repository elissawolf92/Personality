using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TreeSharpPlus;

public class BehaviorDomain : Dictionary<string, object> { }

public class BehaviorEvent
{
	public ITreeNode Tree { get; protected set; }
    public BehaviorComponent[] Agents { get; protected set; }
	public BehaviorDomain Domain { get; protected set; }

    public BehaviorEvent(ITreeNode tree, params BehaviorComponent[] agents)
	{
		this.Tree = tree;
		this.Agents = agents;
		this.Domain = new BehaviorDomain();
	}

    public BehaviorEvent(ITreeNode tree, BehaviorDomain Domain, params BehaviorComponent[] agents)
	{
		this.Tree = tree;
		this.Agents = agents;
		this.Domain = Domain;
	}

	public void Start() { this.Tree.Start(); }
	public void Stop() { this.Tree.Stop(); }

	public RunStatus Tick() { return this.Tree.Tick(); }
}