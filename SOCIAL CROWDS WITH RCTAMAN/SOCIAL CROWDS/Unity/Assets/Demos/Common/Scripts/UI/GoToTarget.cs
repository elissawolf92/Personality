using UnityEngine;
using System.Collections;

public class GoToTarget : MonoBehaviour
{
    private GameObject target = null;
    private Vector3 targetPos = Vector3.zero;

    // Use this for initialization
    void Start()
    {
        this.target = GameObject.Find("Target");
    }

    // Update is called once per frame
    void Update()
    {
        if (this.targetPos != this.target.transform.position)
        {
            this.targetPos = this.target.transform.position;
            SteeringController steering =
                GetComponent<SteeringController>();
            if (steering != null)
                steering.Target = this.targetPos;
        }
    }
}
