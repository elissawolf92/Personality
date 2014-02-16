using UnityEngine;
using System.Collections;

public class BlendText : MonoBehaviour
{
    public string[] helptext;
    public BodyCoordinator coordinator;

    void OnGUI()
    {
        GUIStyle style = new GUIStyle();
        style.normal.textColor = Color.black;
        for (int i = 0; i < helptext.Length; i++)
            GUILayout.Label(helptext[i], style);

        GUILayout.Label("Sitting: " + coordinator.sWeight.Value, style);
        GUILayout.Label("Gesture: " + coordinator.aWeight.Value, style);
        GUILayout.Label("HeadLook: " + coordinator.hWeight.Value, style);
        GUILayout.Label("Reaching: " + coordinator.rWeight.Value, style);
        GUILayout.Label("Ragdoll: " + coordinator.dWeight.Value, style);
    }
}
