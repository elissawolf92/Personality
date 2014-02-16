using UnityEngine;
using System.Collections;

/// <summary>
/// Class that draws the skeleton using Unity's Debug.DrawLine function.
/// Used for drawing skeletons of shadows.
/// </summary>
public class ShadowGizmo : MonoBehaviour 
{
    public ShadowController parentController = null;

    private static int _curColor = 0;
    private static readonly Color[] _colors =
    {
        Color.red,
        Color.green,
        Color.blue,
        Color.black,
        Color.yellow,
        Color.cyan,
        Color.magenta,
        Color.gray,
        Color.red
    };

    /// <summary>
    /// The color of the skeleton.
    /// </summary>
	public Color lineColor;

    void Awake()
    {
        this.lineColor = _colors[_curColor];
        _curColor = (_curColor + 1) % _colors.Length;
    }

	/// <summary>
	/// Redraws the skeleton at every frame
	/// </summary>
	void OnDrawGizmos() 
    {
        if (this.parentController == null ||
            this.parentController.showGizmo == true)
		    GizmoDraw.DrawHierarchy(
                this.transform.root.GetChild(0),
                this.lineColor);
	}
	

}
