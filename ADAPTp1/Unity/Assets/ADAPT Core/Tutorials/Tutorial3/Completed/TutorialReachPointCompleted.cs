using UnityEngine;
using System.Collections;

public class TutorialReachPointCompleted : MonoBehaviour
{
    public KeyCode on;
    public KeyCode off;
    public Body body;

    void Update()
    {
        if (Input.GetKeyDown(this.on) == true)
            this.body.ReachFor(transform.position);
        if (Input.GetKeyDown(this.off) == true)
            this.body.ReachStop();
    }
}
