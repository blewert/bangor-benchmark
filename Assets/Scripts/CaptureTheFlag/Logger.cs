using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Logger : MonoBehaviour 
{
	private List<string> messages;
	private int MESSAGE_COUNT = 5;
	
	public int destX = 0;
	public int destY = 0;
	public int height = 20;
	public int width = 400;

	// Use this for initialization
	void Start () 
	{
		messages = new List<string>();
		
		addMessage ("hello");
		addMessage ("my");
		addMessage ("name");
		addMessage ("is");
		addMessage ("ben");
		addMessage ("and i like");
		addMessage ("programming");	
	}
	
	public void addMessage(string message)
	{
		messages.Add (message);
	}
	
	void OnGUI()
	{
		/*for(var i = messages.Count; i > Mathf.Clamp(messages.Count - MESSAGE_COUNT, 0, MESSAGE_COUNT); i--)
		{
			GUI.Label (new Rect(destX, destY + height * i, width, height), messages[i - 1]); 
		}*/
	}
}
