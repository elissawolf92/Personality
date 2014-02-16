using UnityEngine;
using System.Collections;

public class ShadowRender : MonoBehaviour 
{
    public string shadowName;
    public GameObject rootObject;

    private ShadowCoordinator coordinator;
    private ShadowController controller;

	// Use this for initialization
	void Start () 
    {
        this.coordinator = rootObject.GetComponent<ShadowCoordinator>();
        this.controller = coordinator.GetController(shadowName);
	}
	
	// Update is called once per frame
	void Update () 
    {
        transform.position = rootObject.transform.position;
        transform.rotation = rootObject.transform.rotation;

        ShadowTransform[] encoded = this.coordinator.NewTransformArray();
        this.controller.Encode(encoded);
        Shadow.ReadShadowData(
            encoded, 
            transform.GetChild(0), 
            this.coordinator);
	}
}
