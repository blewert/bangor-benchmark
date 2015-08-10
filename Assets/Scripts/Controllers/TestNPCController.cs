using UnityEngine;
using System.Collections;

public class TestNPCController : MonoBehaviour 
{
	public INPC npc;
	
	// Use this for initialization
	void Start () 
	{
		npc = GetComponent<INPC>();
	}
	
	// Update is called once per frame
	void Update () 
	{
		float accelValue = Mathf.PingPong (Time.time, 10);
		float turnValue = Mathf.PingPong (Time.time, 1);
		
		if(accelValue > 5)
			npc.moveForward();
		else
			npc.moveBackward();
			
		if(turnValue > 0.5)
			npc.turnRight ();
		else
			npc.turnLeft();
	}
}
