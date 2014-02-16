using UnityEngine;
using System.Collections;

public class TriggerExplosionBehavior : MonoBehaviour {
   
	void Start () {
        GameObject[] agents = GameObject.FindGameObjectsWithTag("Player");
        foreach(GameObject a in agents) {
            a.AddComponent("ExplosionBehavior");
            ExplosionBehavior[] exps = a.GetComponents<ExplosionBehavior>();
            exps[exps.Length - 1].explosion = this.gameObject;                   
        }            
	  }	
}
