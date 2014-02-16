using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ShadowRagdollController : ShadowController 
{
    public Transform ragdollRoot;
    public Transform ragdollHips;
    public bool isValid = true;
	
	public Transform[] notAffected;
    public Transform[] notAffectedBelow;

    public LayerMask layers;
    public float fallDuration = 0.3f;

    private float fallEndTime = 0.0f;
    private HashSet<string> _notAffected;
    private HashSet<string> _notAffectedBelow;

    private bool _isFalling = false;
    public bool IsFalling
    {
        get
        {
            return this._isFalling;
        }
        set
        {
            if (this._isFalling != value)
            {
                this._isFalling = value;
                this.ToggleKinematicLocal(this.ragdollHips, !value);
            }
        }
    }

    public override void ControlledStart()
    {
        if (this.ragdollRoot == null || this.ragdollHips == null)
        {
            //Debug.LogWarning(this.gameObject.name + ": No Ragdoll found!");
            this.isValid = false;
            return;
        }

        // Set up the notifier on the ragdoll
        CollisionNotifier notifier = 
            this.ragdollRoot.gameObject.AddComponent<CollisionNotifier>();
        notifier.target = this.gameObject;
        notifier.PropagateDetectors();

        // Copy over the names
        this._notAffected = new HashSet<string>();
        this._notAffectedBelow = new HashSet<string>();
        for (int i = 0; i < this.notAffected.Length; i++)
            this._notAffected.Add(this.notAffected[i].name);
        for (int i = 0; i < this.notAffectedBelow.Length; i++)
            this._notAffectedBelow.Add(this.notAffectedBelow[i].name);

        this.ToggleKinematic(this.ragdollHips, true);
    }

    public override void ControlledUpdate()
    {
        if (this.isValid == false)
            return;

        this.ragdollRoot.transform.position = this.transform.position;
        this.ragdollRoot.transform.rotation = this.transform.rotation;

        this.ragdollHips.transform.position = this.shadow.GetBone("Hips").position;
        this.ragdollHips.transform.rotation = this.shadow.GetBone("Hips").rotation;

        if (this.IsFalling == true)
            this.CopyRagdollToShadow();
        else
            this.CopyShadowToRagdoll();

        // TODO: This sends a lot of message spam to the coordinator. - AS
        if (this.IsFalling == true && Time.time >= this.fallEndTime)
            this.Coordinator.RelayMessage("EvtDoneFalling");
    }

    private void CopyRagdollToShadow()
    {
        ShadowTransform[] buffer = this.Coordinator.NewTransformArray();
        Shadow.WriteShadowData(
            buffer, 
            this.ragdollHips, 
            this.Coordinator, 
            new Whitelist<string>("Spine1"));
        this.Decode(buffer);
    }

    private void CopyShadowToRagdoll()
    {
        ShadowTransform[] buffer = this.Coordinator.NewTransformArray();
        this.Encode(buffer);
        Shadow.ReadShadowData(buffer, this.ragdollHips, this.Coordinator);
    }
	
	private void ToggleKinematic(Transform root, bool value)
    {
        Rigidbody rigid = root.GetComponent<Rigidbody>();

        if (rigid != null)
            rigid.isKinematic = value;
		
		foreach(Transform child in root)
            ToggleKinematic(child, value);
	}

    private void ToggleKinematicLocal(Transform root, bool value)
    {
		if (this._notAffectedBelow.Contains(root.name) == true)
			return;
		
		Rigidbody rigid = root.GetComponent<Rigidbody>();
        if (rigid != null && this._notAffected.Contains(root.name) == false)
        {
            rigid.isKinematic = false;
            if (value == true)
                rigid.Sleep();
		}
		
		foreach(Transform child in root)
            ToggleKinematicLocal(child, value);
	}

    public void OnCollisionNotify(GameObject other)
    {
        if (((1 << other.layer) & this.layers.value) > 0)
            this.EnableRagdoll();
    }

    public void EnableRagdoll()
    {
        this.IsFalling = true;
        this.fallEndTime = Time.time + this.fallDuration;
        this.Coordinator.RelayMessage("EvtBeginFalling");
    }
}
