using UnityEngine;
using System.Collections;


public class ZoneComponent : MonoBehaviour {
    
    public void ComputeProtectionZone() {
        if (transform.GetChildCount() > 0) {
            Vector3 min = transform.GetChild(0).position;
            Vector3 max = transform.GetChild(0).position;            
            foreach(Transform a in transform) {           
                if (a.position.x < min.x)
                    min.x = a.position.x;
                if (a.position.z < min.z)
                    min.z = a.position.z;
                if (a.position.x > max.x)
                    max.x = a.position.x;
                if (a.position.z > max.z)
                    max.z = a.position.z;

                //For bounding box intersections
                min.y = a.position.y - 2f;
                max.y = a.position.y + 2f;   

            }
            foreach(Transform a in transform)            
                a.GetComponent<PoliceBehavior>().zone = new Bounds((min + max) / 2f, max - min);
        }
    }
     void OnDrawGizmosSelected() {                  
         Gizmos.color = Color.yellow;
         
         if (transform.GetChildCount() > 0) {
            Vector3 min = transform.GetChild(0).GetComponent<PoliceBehavior>().zone.min;
            Vector3 max = transform.GetChild(0).GetComponent<PoliceBehavior>().zone.max;       
            Gizmos.DrawWireCube((min + max) / 2f,max - min);          
         }
         
    }

}
