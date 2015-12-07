using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System;

public class CaptureTheFlagAIController : MonoBehaviour 
{
	private enum Team
	{
		RED, BLUE 
	}

	private ILocomotionScript npc;

	private Castdar castdar;
	private Team myTeam;
	
	private int steerDirection = 1;
	
	private GameObject myFlag;
	private Vector3 basePos;
	private float safeRadius = 5f;
	
	private Vector3 helpPosition;
	private bool underFire = false;
	
	// Use this for initialization
	void Start () 
	{
		npc = GetComponent<ILocomotionScript>();
		castdar = GetComponentInChildren<Castdar>();
		
		if(npc.gameObject.getTeam() == "Red")
		{
			myTeam = Team.RED;
			myFlag = GameObject.FindGameObjectsWithTag("Prop").Where (x => x.getTeam ().Equals ("Red") && x.getData ().Equals ("Flag")).FirstOrDefault();
		}
		else
		{
			myTeam = Team.BLUE;
			myFlag = GameObject.FindGameObjectsWithTag("Prop").Where (x => x.getTeam ().Equals ("Blue") && x.getData ().Equals ("Flag")).FirstOrDefault();
		}
		
		basePos = myFlag.transform.position;
	}

	// Update is called once per frame
	void Update () 
	{
		Debug.Log (gameObject.getID() + " .. " + npc.getHealth());
		
		//Is the npc dead?
		if(npc.dead)
		{
			//Call up to gamemode instance to decrement team count. Or just monitor globally?
			Destroy (gameObject);
			Destroy(this);
		}
		
		//Are they inside a base? If so, give health
		if(isInsideBase())
			npc.giveHealth(2f);
		
		//Check for obstacles:
		if(castdar.GetSeen() > 0)
		{
			var objs = castdar.GetSeenObject().Where (x => x.seenOBJ.tag.Equals ("Prop"));
			
			//if(objs.Count > 0)
			//	avoidObstacles();
		}
		
		if(!opponentSeen())
		{
			if(healthIsLow ())
				fleeToBase();
				
			//else
				//wander
				
			underFire = false;
		}
		else
		{
			attackEnemy ();
		}
		//if(npc.getHealth() <= 50f)
		//	fleeToBase();
	}
	
	private bool healthIsLow()
	{
		return npc.getHealth() <= 50f;
	}
	
	private void attackEnemy()
	{
		var objs = castdar.GetSeenObject().Where (x => x.seenOBJ.getData ().Equals ("NPC") && x.seenOBJ.getTeam () != gameObject.getTeam());
		
		var closestObj = objs.OrderBy(x => Vector3.Distance (x.seenOBJ.transform.position, transform.position)).FirstOrDefault();
		
		if(getSteerDirection(closestObj.seenOBJ.transform.position) > 0)
			npc.turnRight ();
		else
			npc.turnLeft ();
			
		if(Vector3.Distance (closestObj.seenOBJ.transform.position, transform.position) <= 8f)
			closestObj.seenOBJ.GetComponent<ILocomotionScript>().takeHealth(0.5f);
		else
			npc.moveForward();
	}
	
	private bool opponentSeen()
	{
		var objs = castdar.GetSeenObject().Where (x => x.seenOBJ.getData ().Equals ("NPC") && x.seenOBJ.getTeam () != gameObject.getTeam());
		
		return objs.Count() > 0;
	}
	
	private bool isInsideBase()
	{
		return Vector3.Distance (transform.position, basePos) <= safeRadius;
	}
	
	private void fleeToBase()
	{
		if(!isInsideBase())
		{
			if(getSteerDirection(basePos) < 0)
				npc.turnLeft ();
			else
				npc.turnRight();
			
			npc.moveForward();
		}
	}
	
	public void onReceiveHelpRequest(Vector3 position)
	{
		helpPosition = position;
	}
	
	public void signal()
	{
		var allObjects = GameObject.FindObjectsOfType<GameObject>();
		var otherNPCs = allObjects.Where (x => x.getTeam ().Equals (gameObject.getTeam ()) && x.getData().Equals ("NPC"));
		
		foreach(var npc in otherNPCs)
			npc.SendMessage ("onReceiveHelpRequest", transform.position); 
	}
	
	private bool hasBeenSignalled()
	{
		return helpPosition != default(Vector3);
	}
	
	public int getSteerDirection(Vector3 position)
	{
		var localPosition = transform.InverseTransformPoint(position);
		
		return Math.Sign (localPosition.x);
	}
}