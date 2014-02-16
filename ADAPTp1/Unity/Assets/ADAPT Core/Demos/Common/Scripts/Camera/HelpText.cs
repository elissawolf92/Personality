using UnityEngine;
using System.Collections;

public class HelpText : MonoBehaviour
{
    public string[] helptext;
    void OnGUI()
    {
        GUIStyle style = new GUIStyle();
        style.normal.textColor = Color.black;
        for (int i = 0; i < helptext.Length; i++)
            GUILayout.Label(helptext[i], style);
    }
}
