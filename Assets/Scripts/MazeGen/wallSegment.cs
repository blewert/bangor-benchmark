using UnityEngine;
using System.Collections;

public class wallSegment : MonoBehaviour {

	public int xpos;
	public int ypos;
	public bool remove = false;
	public bool corridor = false;
	public int corridorID = 0;
	public bool border = false;
	public bool accessable = false;
	public int roomID = -1;

	// Use this for initialization
	void Start () {
		//accessable = false;
		//remove = false;
		//corridor = false;
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public void setID (int y, int x) {

		xpos = x;
		ypos = y;
	
	}
}
