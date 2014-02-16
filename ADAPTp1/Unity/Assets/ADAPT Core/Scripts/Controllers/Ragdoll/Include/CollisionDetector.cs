using UnityEngine;
using System.Collections;

public class CollisionDetector : MonoBehaviour 
{
    void BroadcastHit(GameObject other)
    {
        this.transform.parent.SendMessage("BroadcastHit", other);
    }

    void OnCollisionEnter(Collision other)
    {
        this.BroadcastHit(other.collider.gameObject);
    }
}
