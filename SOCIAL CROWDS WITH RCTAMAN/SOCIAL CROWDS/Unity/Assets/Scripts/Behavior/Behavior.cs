using UnityEngine;
using TreeSharpPlus;
using System;
using System.Reflection;
using System.Collections;
using System.Collections.Generic;

[RequireComponent(typeof(BehaviorComponent))]
public abstract class Behavior : MonoBehaviour
{
    public abstract ITreeNode BakeTree();

    void Start()
    {
        BehaviorComponent behavior = GetComponent<BehaviorComponent>();
        behavior.DefaultTree = this.BakeTree();
    }
}
