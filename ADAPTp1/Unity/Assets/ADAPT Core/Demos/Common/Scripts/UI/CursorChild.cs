using UnityEngine;
using System.Collections;

public class CursorChild : MonoBehaviour
{
    public enum Type
    {
        Reach,
        HeadLook
    }

    public Type type;
    public CursorParent parent;
    public string activateKey = "5";
    public bool locked = false;
    public Vector3 offset = Vector3.zero;
    public GameObject character;

    private Body bodyInterface = null;
    private ShadowReachController reach = null;
    private ShadowHeadLookController headLook = null;

    void Start()
    {
        this.bodyInterface = character.GetComponent<Body>();
        // If we're working with a reduced character, like in the HeadLook and
        // Reach demo, we might not have a BodyInterface, so we'll need to manually
        // fetch the right shadow controllers. In practice, you don't want to do this.
        if (this.bodyInterface == null)
        {
            this.reach = character.GetComponent<ShadowReachController>();
            this.headLook = character.GetComponent<ShadowHeadLookController>();
        }
    }

    // Update is called once per frame
    void LateUpdate()
    {
        if (this.activateKey.Length > 0 && Input.GetKeyDown(this.activateKey))
            this.locked = !this.locked;

        if (this.locked == false)
            this.transform.position = 
                this.parent.transform.position + this.offset;

        // If we have a full BodyInterface character, set the HeadLook
        // the "proper" way
        if (bodyInterface != null)
        {
            if (type == Type.HeadLook)
                bodyInterface.HeadLookSetTarget(transform.position);
            else
                bodyInterface.ReachSetTarget(transform.position);
        }
        // Otherwise, we have to do it the ugly, manual way
        else
        {
            if (type == Type.HeadLook)
                this.headLook.target = transform.position;
            else
                this.reach.target = transform.position;
        }
    }
}
