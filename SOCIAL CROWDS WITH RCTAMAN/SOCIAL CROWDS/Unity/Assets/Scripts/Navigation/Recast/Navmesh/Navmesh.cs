using UnityEngine;
using System.Collections;

public class Navmesh : MonoBehaviour 
{
	[SerializeField]
	[HideInInspector]
	private byte[] data;
	public byte[] Data 
	{ 
		get
		{
			return this.data;
		}
		private set
		{
			this.data = value;
		}
	}
	
	void OnDrawGizmosSelected()
    {
		if(this.Data != null)
			NavmeshDebugRenderer.RenderNavmesh(this.Data);
    }
	
	public void SetData(byte[] data)
	{
		if (this.Data == null)
			this.Data = data;
		else
			Debug.LogError("Cannot overwrite Navmesh");
	}
}
