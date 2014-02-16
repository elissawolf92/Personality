//using UnityEngine;
//using System.Collections;

//public class BodyController : MonoBehaviour 
//{
//    private LegAnimator legAnimator = null;
//    public LegAnimator LegAnimator
//    {
//        get
//        {
//            if (this.legAnimator == null)
//                this.legAnimator = GetComponent<LegAnimator>();
//            return this.legAnimator;
//        }
//    }

//    private SittingController sittingController;
//    public SittingController SittingController
//    {
//        get
//        {
//            if (this.sittingController == null)
//                this.sittingController = GetComponent<SittingController>();
//            return this.sittingController;
//        }
//    }

//    private UnitySteeringController steeringController;
//    public UnitySteeringController SteeringController
//    {
//        get
//        {
//            if (this.steeringController == null)
//                this.steeringController = GetComponent<UnitySteeringController>();
//            return this.steeringController;
//        }
//    }
        
//    void Start () 
//    {
	
//    }
	
//    // Update is called once per frame
//    void Update () 
//    {
//        if (this.SittingController != null && this.LegAnimator != null)
//        {
//            this.LegAnimator.enabled = !this.SittingController.IsActive;
//        }
//    }
//}
