using UnityEngine;


public class Gizmonizer: MonoBehaviour{

    public GameObject gizmoAxis;
    public float gizmoSize  = 1.0f;

    private GameObject gizmoObj ;
    private Gizmo gizmo ;
    private GizmoType gizmoType = GizmoType.Rotation;

    public void Update() {
        if (Input.GetKeyDown(KeyCode.Escape)) {
            removeGizmo();
        }
    
        if (gizmo) {
            if (Input.GetKeyDown(KeyCode.Alpha1)) {
                gizmoType = GizmoType.Position;
                gizmo.setType(gizmoType);
            }
            if (Input.GetKeyDown(KeyCode.Alpha2)) {
                gizmoType = GizmoType.Rotation;
                gizmo.setType(gizmoType);
            }
            if (Input.GetKeyDown(KeyCode.Alpha3)) {
                gizmoType = GizmoType.Scale;
                gizmo.setType(gizmoType);
            }        
            if (gizmo.needUpdate) {
                resetGizmo();
            }
        }
    }


    public void OnMouseDown() {
        if (!gizmoObj) {
            resetGizmo();
        }
    }

    public void removeGizmo() {
        if (gizmoObj) {
            gameObject.layer = 0;
            foreach (Transform child  in transform) {
                child.gameObject.layer = 0;
            }        
            Destroy(gizmoObj);    
            Destroy(gizmo);    
        }
    }

    public void resetGizmo() {
        removeGizmo();
        gameObject.layer = 2;
        foreach (Transform child in transform) {
            child.gameObject.layer = 2;
        }        
        gizmoObj = (GameObject) Instantiate(gizmoAxis, transform.position, transform.rotation);
        gizmoObj.transform.localScale *= gizmoSize;
        gizmo = (Gizmo) gizmoObj.GetComponent("Gizmo");
        gizmo.setParent(transform);
        gizmo.setType(gizmoType);
    }

}