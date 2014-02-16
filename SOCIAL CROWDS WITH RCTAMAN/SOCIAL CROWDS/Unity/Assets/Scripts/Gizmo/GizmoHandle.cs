using UnityEngine;
using System.Collections;

public enum GizmoControl { Horizontal, Vertical, Both }
public enum GizmoType { Position, Rotation, Scale }
public enum GizmoAxis { X, Y, Z }

public class GizmoHandle: MonoBehaviour {

    public GameObject positionEnd;
    public GameObject rotationEnd;
    public GameObject scaleEnd;
    public float moveSensitivity = 2f;
    public float rotationSensitivity = 64f;
    public bool needUpdate = false;



    public GizmoType type = GizmoType.Rotation;
    public GizmoControl control = GizmoControl.Both;
    public GizmoAxis axis = GizmoAxis.X;

    private bool mouseDown = false;
    private Transform otherTrans;

    public void Awake() {
        otherTrans = transform.parent;
    }

    public void Update() {

    }

    public void setParent(Transform other) {
        otherTrans = other;
    }

    public void setType(GizmoType type) {
        this.type = type;
        positionEnd.active = type == GizmoType.Position;
        rotationEnd.active = type == GizmoType.Rotation;
        scaleEnd.active = type == GizmoType.Scale;
    }

    public void OnMouseDown() {
        mouseDown = true;
    }

    public void OnMouseUp() {
        mouseDown = false;
        needUpdate = true;
    }


    public void OnMouseDrag() {
        float delta = 0f;
        if (mouseDown) {
            switch (control) {
                case GizmoControl.Horizontal:
                    delta = Input.GetAxis("Mouse X") * Time.deltaTime;
                    break;
                case GizmoControl.Vertical:
                    delta = Input.GetAxis("Mouse Y") * Time.deltaTime;
                    break;
                case GizmoControl.Both:
                    delta = (Input.GetAxis("Mouse X") + Input.GetAxis("Mouse Y")) * Time.deltaTime;
                    break;
            }


            if(type == GizmoType.Position) {
                
                        delta *= moveSensitivity;

                        switch (axis) {
                            case GizmoAxis.X:
                                otherTrans.Translate(Vector3.right * delta);
                                break;
                            case GizmoAxis.Y:
                                otherTrans.Translate(Vector3.up * delta);
                                break;
                            case GizmoAxis.Z:
                                otherTrans.Translate(Vector3.forward * delta);
                                break;
                            default:
                                break;
                        }
                    
            }
            else if(type == GizmoType.Scale) {
                
                        delta *= moveSensitivity;
                        switch (axis) {
                            case GizmoAxis.X:

                                otherTrans.localScale = new Vector3(otherTrans.localScale.x + delta, otherTrans.localScale.y, otherTrans.localScale.z);
                                break;
                            case GizmoAxis.Y:
                                otherTrans.localScale = new Vector3(otherTrans.localScale.x, otherTrans.localScale.y + delta, otherTrans.localScale.z);
                                break;
                            case GizmoAxis.Z:
                                otherTrans.localScale = new Vector3(otherTrans.localScale.x, otherTrans.localScale.y, otherTrans.localScale.z + delta);
                                break;
                        }
                    
            }
            else if(type == GizmoType.Rotation) {

                        delta *= rotationSensitivity;
                        switch (axis) {
                            case GizmoAxis.X:
                                otherTrans.Rotate(Vector3.right * delta);
                                break;
                            case GizmoAxis.Y:
                                otherTrans.Rotate(Vector3.up * delta);
                                break;
                            case GizmoAxis.Z:
                                otherTrans.Rotate(Vector3.forward * delta);
                                break;
                        }
                    
            }
            
        }

        
    }

}
