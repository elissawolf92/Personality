using UnityEngine;
using System.Collections;

public class CameraScript : MonoBehaviour {

	// Use this for initialization

    float y = 180;
    float x = 10;
    float dist = 5;

    Vector2 lastMousePos;

	void Start () 
    {
     //   transform.position = Vector3.zero;
        transform.rotation = Quaternion.Euler(x, y, 0);
        lastMousePos = Input.mousePosition;
        transform.Translate(0, 0, -dist);
	}
	
	// Update is called once per frame
	void Update () 
    {
    
        Vector2 pos = Input.mousePosition;
        Vector2 delta = pos - lastMousePos;

        lastMousePos = pos;

        if (Input.GetMouseButton(0))
        {
            y += delta.x;
            x -= delta.y;
            x = Mathf.Clamp(x, 1, 89);
        }
		
        if (Input.GetMouseButton(2))
        {
            dist -= delta.y;
            dist = Mathf.Clamp(dist, 1, 100);
        }
		 
	
        transform.position = Vector3.zero;
        transform.rotation = Quaternion.Euler(x, y, 0);
        transform.Translate(0, 0, -dist);

	
		
		
/*		
	if (Input.GetAxis("Mouse ScrollWheel") < 0) // back
        {
            Camera.main.orthographicSize = Mathf.Max(Camera.main.orthographicSize-1, 1);

        }
        if (Input.GetAxis("Mouse ScrollWheel") > 0) // forward
        {
            Camera.main.orthographicSize = Mathf.Min(Camera.main.orthographicSize-1, 6);
        }
        
        */
		
	}		 
}
