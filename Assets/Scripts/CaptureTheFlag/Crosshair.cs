using UnityEngine;
using System.Collections;

public class Crosshair : MonoBehaviour 
{
	private INPC npc; 
	private Gun gun;
	private Vector3 projectionPos;
	 
	// Use this for initialization
	void Start () 
	{
		npc = GetComponent<INPC>();
		gun = GetComponentInChildren<Gun>();
	}
	
	// Update is called once per frame
	void Update () 
	{
		float projectionRange = gun.shotDistance;
		
		RaycastHit[] hitInfo = Physics.RaycastAll(gun.transform.position, -transform.up, gun.shotDistance);
		
		foreach(var hit in hitInfo)
		{
			if(hit.transform.root != this.transform.root)
			{
				if(hit.transform.root.tag.StartsWith ("Team") || hit.transform.root.tag == "Wall")
				{
					projectionRange = hit.distance;
					
					break;
				}
			}
		}
		
		projectionPos = gun.transform.TransformPoint(-transform.up * projectionRange);
	}
	
	public void OnGUI()
	{
		Vector3 pos = projectionPos;
		
		pos = Camera.main.WorldToScreenPoint(pos);
		
		// The GUI uses a different y-axis. So another conversion // is needed for the y-axis. (Flip it.) // Also, add some offsets to center the text. 
		Rect rect = new Rect(pos.x - 10 , Screen.height - pos.y - 15 , 100 , 22);
		GUI.Label(rect, "x");
		
	}
}
