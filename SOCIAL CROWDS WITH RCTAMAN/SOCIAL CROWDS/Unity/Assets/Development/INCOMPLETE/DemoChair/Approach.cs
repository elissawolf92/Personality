//using UnityEngine;
//using System;
//using System.Collections;

//public class Approach : MonoBehaviour 
//{
//    public Transform marker = null;
//    public MeshRenderer chairRenderer1 = null;
//    public MeshRenderer chairRenderer2 = null;
//    public Material redMaterial = null;
//    public Material blueMaterial = null;

//    private bool wantChair = false;

//    private UnitySteeringController steering;
//    protected UnitySteeringController Steering
//    {
//        get
//        {
//            if (this.steering == null)
//                this.steering = GetComponent<UnitySteeringController>();
//            return this.steering;
//        }
//    }

//    private BodyController body;
//    protected BodyController Body
//    {
//        get
//        {
//            if (this.body == null)
//                this.body = GetComponent<BodyController>();
//            return this.body;
//        }
//    }

//    private HeadLookController headLook;
//    protected HeadLookController HeadLook
//    {
//        get
//        {
//            if (this.headLook == null)
//                this.headLook = GetComponent<HeadLookController>();
//            return this.headLook;
//        }
//    }

//    // Use this for initialization
//    void Start () 
//    {
//    }
	
//    // Update is called once per frame
//    void Update () 
//    {
//        if (Input.GetKeyDown(KeyCode.Space))
//            this.HeadLook.enabled = !this.HeadLook.enabled;

//        if (Input.GetMouseButtonDown(1))
//        {
//            Ray cursorRay = Camera.main.ScreenPointToRay(Input.mousePosition);
//            RaycastHit hit;
//            if (Physics.Raycast(cursorRay, out hit))
//            {
//                if (hit.transform.parent != null
//                    && hit.transform.parent.name == "Bench")
//                {
//                    this.Steering.Target = Vector3.zero;
//                    this.wantChair = true;
//                    this.chairRenderer1.material = this.blueMaterial;
//                    this.chairRenderer2.material = this.blueMaterial;
//                    this.marker.renderer.enabled = false;
//                }
//                else
//                {
//                    this.marker.position = hit.point;
//                    this.wantChair = false;
//                    this.chairRenderer1.material = this.redMaterial;
//                    this.chairRenderer2.material = this.redMaterial;
//                    this.marker.renderer.enabled = true;
//                }
//            }
//        }

//        if (this.wantChair == true)
//        {
//            if (this.Steering.IsAtTarget() == true &&
//                Math.Abs(Quaternion.Dot(
//                    transform.rotation,
//                    Quaternion.Euler(0.0f, 0.0f, 0.0f))) > 0.98f)
//            {
//                this.Body.SittingController.SendMessage("Sit");
//            }
//            else if (this.Steering.IsAtTarget() == true)
//            {
//                this.Steering.desiredOrientation = Quaternion.Euler(0.0f, 0.0f, 0.0f);
//                this.Steering.orientationBehavior = OrientationBehavior.None;
//            }
//        }
//        else
//        {
//            if (this.Body.SittingController.IsActive == true)
//            {
//                this.Body.SittingController.SendMessage("Stand");
//            }
//            else
//            {
//                this.Steering.Target = marker.transform.position;
//                this.Steering.orientationBehavior = OrientationBehavior.LookForward;
//            }
//        }
//    }
//}
