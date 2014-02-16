using UnityEngine;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

public class ShootBall : MonoBehaviour 
{	
	public Camera targetCamera;
	public GameObject ballPrefab;
	public float magnitude;
    public float lifetime = 1.0f;

    private Vector3 lastPosition = Vector3.zero;
    private List<KeyValuePair<GameObject, float>> cleanup;

	void Start () 
    {
        this.cleanup = new List<KeyValuePair<GameObject, float>>();
	}
	
	void Update()
    {
		if (Input.GetKeyDown(KeyCode.Space) == true)
        {
            Ray cursorRay = targetCamera.ScreenPointToRay(Input.mousePosition);
            GameObject newBall = 
                Instantiate(
                    ballPrefab, 
                    targetCamera.transform.position, 
                    targetCamera.transform.rotation) 
                as GameObject;
            Vector3 currentPosition = this.targetCamera.transform.position;
            Vector3 velocity = 
                (currentPosition - this.lastPosition) / Time.deltaTime;
			newBall.rigidbody.velocity = 
                velocity + (cursorRay.direction * magnitude);
            this.cleanup.Add(
                new KeyValuePair<GameObject, float>(
                    newBall, 
                    Time.time + this.lifetime));
		}

        foreach (KeyValuePair<GameObject, float> ball in this.cleanup)
            if (ball.Value < Time.time)
                GameObject.Destroy(ball.Key);
        this.cleanup.RemoveAll(n => n.Value < Time.time);
        this.lastPosition = this.targetCamera.transform.position;
	}
}
