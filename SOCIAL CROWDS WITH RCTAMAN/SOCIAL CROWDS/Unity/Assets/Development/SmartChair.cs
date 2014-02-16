using UnityEngine;
using System.Collections;

public class SmartChair : MonoBehaviour 
{
    public enum UseState
    {
        Approaching,
        Arrived,
        Using,
        Unused
    }

    public UseState state = UseState.Unused;
    public Transform navTarget = null;
    public Transform useTarget = null;

    private Vector3 correction;

    [HideInInspector]
    public BodyInterface user = null;

	// Use this for initialization
	void Start () 
    {
	
	}

    public void Use(BodyInterface user)
    {
        if (this.state == UseState.Unused)
        {
            Debug.Log("Using");
            this.user = user;
            user.NavSetTarget(this.navTarget.position);
            this.state = UseState.Approaching;
        }
    }

	// Update is called once per frame
	void Update () 
    {
        switch (this.state)
        {
            case UseState.Approaching:
                if (this.user.NavHasArrived() == true)
                {
                    Debug.Log("Arrived");
                    this.user.NavSetOrientationBehavior(OrientationBehavior.None);
                    this.user.NavSetDesiredOrientation(this.useTarget.rotation);
                    this.user.NavSetAttached(false);
                    this.correction = this.useTarget.position - this.user.transform.position;
                    this.state = UseState.Arrived;
                }
                break;
            case UseState.Arrived:
                // TODO: Make an interface call for this - AS
                this.user.transform.Translate(this.correction * Time.deltaTime);
                this.correction = this.useTarget.position - this.user.transform.position;
                Debug.Log(Quaternion.Dot(this.user.transform.rotation, this.navTarget.rotation) + " " + correction.sqrMagnitude);
                if ((Quaternion.Dot(this.user.transform.rotation, this.useTarget.rotation) > 0.997f
                    || Quaternion.Dot(this.user.transform.rotation, this.useTarget.rotation) < -0.997f)
                    && this.correction.sqrMagnitude < 0.003f)
                {
                    Debug.Log("Using");
                    this.user.SitDown();
                    this.state = UseState.Using;
                }
                break;
        }
	}
}
