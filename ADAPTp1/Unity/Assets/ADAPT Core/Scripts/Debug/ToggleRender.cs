using UnityEngine;
using System.Collections;

/// <summary>
/// Class that toggles a renderer on and off.
/// </summary>
public class ToggleRender : MonoBehaviour 
{
    public string toggleKeyName;
    public bool defaultState = true;

	void Update () 
    {
        renderer.enabled = this.defaultState;
        if (Input.GetKeyDown(toggleKeyName))
            this.defaultState = !this.defaultState;
	}
}
