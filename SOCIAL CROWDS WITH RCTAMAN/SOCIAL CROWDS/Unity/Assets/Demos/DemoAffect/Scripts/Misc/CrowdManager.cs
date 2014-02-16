using UnityEngine;
using System.Collections;

public class CrowdManager : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    public void Restart() {
      
        AgentComponent[] agentComponents = FindObjectsOfType(typeof(AgentComponent)) as AgentComponent[];
        foreach(AgentComponent a in agentComponents)
            a.Restart();
    
        AffectComponent[] affectComponents = FindObjectsOfType(typeof(AffectComponent)) as AffectComponent[];
        foreach (AffectComponent a in affectComponents)
            a.Restart();


        Appraisal[] appraisalComponents = FindObjectsOfType(typeof(Appraisal)) as Appraisal[];
        foreach (Appraisal a in appraisalComponents)
            a.Restart();
				
        ProtesterBehavior[] protesterComponents = FindObjectsOfType(typeof(ProtesterBehavior)) as ProtesterBehavior[];
        foreach (ProtesterBehavior p in protesterComponents)
            p.Restart();

        PoliceBehavior[] policeComponents = FindObjectsOfType(typeof(PoliceBehavior)) as PoliceBehavior[];
        foreach (PoliceBehavior p in policeComponents)
            p.Restart();

        ShopperBehavior[] shopperComponents = FindObjectsOfType(typeof(ShopperBehavior)) as ShopperBehavior[];
        foreach (ShopperBehavior s in shopperComponents)
            s.Restart();
        UpdateAnimation[] animationComponents = FindObjectsOfType(typeof(UpdateAnimation)) as UpdateAnimation[];
        foreach (UpdateAnimation a in animationComponents)
            a.Restart();

    }
}
