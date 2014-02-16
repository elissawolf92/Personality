using UnityEngine;
using System.Collections;

[RequireComponent(typeof(RecastSteeringController))]
public class RecastNavmeshPlatformController : MonoBehaviour
{
    private RecastSteeringController steering;

    public float maxForwardSpeed = 1.5f;
    public float maxBackwardsSpeed = 1.5f;
    public float maxSidewaysSpeed = 1.5f;

    new public Camera camera = null;

    public float walkMultiplier = 0.5f;
    public bool defaultIsWalk = false;

    void Start()
    {
        this.steering = GetComponent<RecastSteeringController>();
    }

    // Update is called once per frame
    void Update()
    {
        // Get input vector from kayboard or analog stick and make it length 1 at most
        Vector3 directionVector = new Vector3(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"), 0);
        if (directionVector.magnitude > 1) directionVector = directionVector.normalized;
        directionVector = directionVector.normalized * Mathf.Pow(directionVector.magnitude, 2);

        // Rotate input vector into camera space so is up camera's up and right is camera's right
        directionVector = this.camera.transform.rotation * directionVector;

        // Rotate input vector to be perpendicular to character's up vector
        Quaternion camToCharacterSpace = Quaternion.FromToRotation(this.camera.transform.forward * -1, transform.up);
        directionVector = (camToCharacterSpace * directionVector);

        // Make input vector relative to Character's own orientation
        directionVector = Quaternion.Inverse(transform.rotation) * directionVector;

        if (walkMultiplier != 1)
            if ((Input.GetKey("left shift") || Input.GetKey("right shift")) != defaultIsWalk)
                directionVector *= walkMultiplier;

        float difference = 0.0f;
        if (Input.GetKey(KeyCode.Q) == true)
            difference -= 1.0f;
        if (Input.GetKey(KeyCode.E) == true)
            difference += 1.0f;

        this.UpdateOrientation(difference, 200.0f);
        this.steering.SetVelocity(this.DesiredVelocity(directionVector));
    }

    private void UpdateOrientation(float difference, float speed)
    {
        if (difference != 0.0f)
        {
            Vector3 euler = this.transform.rotation.eulerAngles;
            Vector3 eulerPlus = 
                new Vector3(
                    euler.x,
                    euler.y + (difference * speed) * Time.deltaTime, 
                    euler.z);
            this.steering.orientationBehavior = OrientationBehavior.None;
            this.steering.desiredOrientation = Quaternion.Euler(eulerPlus);
        }
        else
        {
            this.steering.orientationBehavior = OrientationBehavior.LookForward;
        }
    }

    public Vector3 DesiredVelocity(Vector3 directionVector)
    {
        if (directionVector == Vector3.zero)
        {
            return Vector3.zero;
        }
        else
        {
            float zAxisEllipseMultiplier = (directionVector.z > 0 ? maxForwardSpeed : maxBackwardsSpeed) / maxSidewaysSpeed;
            Vector3 temp = new Vector3(directionVector.x, 0, directionVector.z / zAxisEllipseMultiplier).normalized;
            float length = new Vector3(temp.x, 0, temp.z * zAxisEllipseMultiplier).magnitude * maxSidewaysSpeed;
            Vector3 velocity = directionVector * length;
            return transform.rotation * velocity;
        }
    }

}
