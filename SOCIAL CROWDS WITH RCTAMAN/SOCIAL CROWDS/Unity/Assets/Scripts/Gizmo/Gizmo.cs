using UnityEngine;


public class Gizmo : MonoBehaviour {

    public GizmoHandle axisX;
    public GizmoHandle axisY;
    public GizmoHandle axisZ;


    public GizmoType gizmoType;

    public bool needUpdate = false;

    public void Awake() {
        axisX.axis = GizmoAxis.X;
        axisY.axis = GizmoAxis.Y;
        axisZ.axis = GizmoAxis.Z;
    
        setType(gizmoType);
    }


    public void Update() {    
        needUpdate = (axisX.needUpdate || axisY.needUpdate || axisZ.needUpdate);
    }

    public void setType(GizmoType type) {
        axisX.setType(type);
        axisY.setType(type);
        axisZ.setType(type);
    }

    public void setParent(Transform other) {
        transform.parent = other;
        axisX.setParent(other);
        axisY.setParent(other);
        axisZ.setParent(other);
    }
}