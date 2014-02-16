using UnityEngine;
using System.Collections;

public class TutorialLookPointCompleted : MonoBehaviour
{
    public KeyCode on;
    public KeyCode off;
    public Body body;

    void Update()
    {
        if (Input.GetKeyDown(this.on) == true)
            this.body.HeadLookAt(transform.position);
        if (Input.GetKeyDown(this.off) == true)
            this.body.HeadLookStop();
    }
}
