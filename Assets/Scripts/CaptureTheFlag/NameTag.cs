using UnityEngine;
using System.Collections;

public class NameTag : MonoBehaviour 
{
	public string text;
	public GUIStyle style;
	public GameObject obj;
	public bool bold = false;
	public int fontSize = 12;
	private Color tempColor;
	  
	void Start()
	{
		style = new GUIStyle();
		
		if(bold)
			style.fontStyle = FontStyle.Bold;
		
		style.fontSize = fontSize;
		style.border = new RectOffset(5, 5, 5, 5);
		
		if(tempColor != null)
			setFontColor(tempColor);

	}
	
	public void setFontColor(Color color)
	{
		if(style != null)
			style.normal.textColor = color;
		else
			tempColor = color;
	}
	
	public void setFontSize(int size)
	{
		style.fontSize = size;
	}
	
	void OnGUI()
	{
		// Start with world coordinates. 
		Vector3 pos = obj.transform.position; 
		
		var heading = pos - Camera.main.transform.position;
		
		if (Vector3.Dot(Camera.main.transform.forward, heading) <= 0)
			return;
		
		pos.y += 2; // The height of the agent plus a little bit.
		
		// Convert to screen coordinates.
		pos = Camera.main.WorldToScreenPoint(pos);

		// check if point is seen
		
		// The GUI uses a different y-axis. So another conversion // is needed for the y-axis. (Flip it.) // Also, add some offsets to center the text. 
		Rect rect = new Rect(pos.x - 10 , Screen.height - pos.y - 15 , 100 , 22);
		GUI.Label(rect, text, style);

	}
}
