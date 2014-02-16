using UnityEngine;
using System.Collections;

public class ObjScript : MonoBehaviour {

    

     void Start () {  
     GameObject objs = GameObject.Find("Objects");
          for(int i = 0; i < objs.transform.GetChildCount(); i++) {
              objs.transform.GetChild(i).gameObject.AddComponent<ObjComponent>();
              objs.transform.GetChild(i).gameObject.GetComponent<ObjComponent>().achieved = false;

          }
     }
}
