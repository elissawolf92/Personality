using UnityEngine;
using System;
using System.Reflection;
using System.Collections;

public class CollisionNotifier : MonoBehaviour 
{
    public GameObject target = null;

    public void PropagateDetectors()
    {
        foreach (Transform child in this.transform)
            this.AddDetector(child);
    }

    protected void AddDetector(Transform t)
    {
        t.gameObject.AddComponent<CollisionDetector>();
        foreach (Transform child in t)
            AddDetector(child);
    }

    void BroadcastHit(GameObject other)
    {
        this.target.SendMessage("OnCollisionNotify", other);
    }
}
