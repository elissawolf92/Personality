using UnityEngine;
using System.Collections;

public class FlyingCamera : MonoBehaviour
{
    private float lookSpeed = 90.0f;
    private float moveSpeed = 15.0f;
    private float updownSpeed = 10.0f;

    private float rotationX = 180.0f;
    private float rotationY = -35.0f;

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButton(1))
        {
            rotationX += Input.GetAxis("Mouse X") * lookSpeed * Time.deltaTime;
            rotationY += Input.GetAxis("Mouse Y") * lookSpeed * Time.deltaTime;
            rotationY = Mathf.Clamp(rotationY, -90, 90);
        }

        transform.localRotation = Quaternion.AngleAxis(rotationX, Vector3.up);
        transform.localRotation *= Quaternion.AngleAxis(rotationY, Vector3.left);

        float deltaMove = Time.deltaTime * this.moveSpeed;
        transform.position += transform.forward * Input.GetAxis("Vertical") * deltaMove;
        transform.position += transform.right * Input.GetAxis("Horizontal") * deltaMove;

        float deltaUpdown = Time.deltaTime * this.updownSpeed;
        if (Input.GetKey(KeyCode.Space))
            transform.position += Vector3.up * deltaUpdown;
        if (Input.GetKey(KeyCode.LeftControl))
            transform.position += Vector3.up * -deltaUpdown;
    }
}
