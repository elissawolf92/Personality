using UnityEngine;
using System.Collections;

public class CursorParent : MonoBehaviour
{
    public new Camera camera;
    private float offset = 1.5f;

    void LateUpdate()
    {
        if (Input.GetKey(KeyCode.T))
            offset += Time.deltaTime;
        if (Input.GetKey(KeyCode.G))
            offset -= Time.deltaTime;

        Ray cursorRay = camera.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        if (Physics.Raycast(cursorRay, out hit))
            transform.position = hit.point + offset * Vector3.up;
    }
}

