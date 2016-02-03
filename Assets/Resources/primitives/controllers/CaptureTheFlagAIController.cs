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
	private float closeRadius = 5f;
	
	private Vector3 helpPosition;
	private bool underFire = false;
	
	private float targetRotation = default(float);
	
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
		
		targetRotation = transform.rotation.eulerAngles.y;
		StartCoroutine("updateHeading");
	}
	
	IEnumerator updateHeading()
	{
		while(true)
		{	
			float oldHeading = transform.rotation.eulerAngles.y;
			float newHeading = oldHeading + UnityEngine.Random.Range (-80f, 80f); 
			
			targetRotation = newHeading;
			
			if(gameObject.getID () == 2)
				DebugLogger.Log (oldHeading + " ...  " + newHeading + " : " + gameObject.getID());
			
			yield return new WaitForSeconds(3f);
		}
	}
	
	// Update is called once per frame
	void Update () 
	{
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
		
		//If they're outside of the terrain boundaries, move them back to the base
		if(!inTerrainBounds(transform.position))
			fleeToBase();
		
		
		//Check for obstacles:
		if(castdar.GetSeen() > 0)
		{
			var objs = castdar.GetSeenObject().Where (x => x.seenOBJ.tag.Equals ("Prop"));
			
			if(objs.Count() > 0)
				avoidObstacles();
		}
		
		if(!opponentSeen())
		{
			//An opponent is not seen, and their health is low. So go back to base:
			if(healthIsLow ())
				fleeToBase();
			
			//Otherwise just wander.				
			else
				wander();
				
			underFire = false;
		}
		else
		{
			if(teammatesNearby())
			{
				//They have seen an opponent and there are team-mates nearby. If they have low hp, signal and go to base.
				if(healthIsLow())
					signalAndFlee();
				
				//If they have high hp, signal and attack.
				else
					signalAndAttack();
			}
			else
			{
				//Otherwise, there are no team-mates nearby. If they have low hp, go back to base.
				if(healthIsLow())
					fleeToBase();
					
				else
				{
					//However if they do have health, and there are enemies nearby, flee to base.
					if(enemyTeammatesNearby())
						fleeToBase();
						
					//Otherwise, attack the enemy (because they're on their own).
					else
						attackEnemy();
				}					
			}
		}
		//if(npc.getHealth() <= 50f)
		//	fleeToBase();
	}
	
	private void avoidObstacles()
	{
		npc.turnRight ();
	}
	
	private void wander()
	{
		if(!inTerrainBounds(transform.position))
		{
			fleeToBase();
			return;
		}
		
		if(transform.rotation.eulerAngles.y > targetRotation)
			npc.turnLeft();
		else
			npc.turnRight ();
		
		npc.moveForward();
	}
	
	private bool enemyTeammatesNearby()
	{
		var npcs = GameObject.FindObjectsOfType<GameObject>().Where (x => x.getTeam ().Equals (gameObject.getTeam ()) && x.getData().Equals ("NPC"));
		
		return npcs.Any (x => Vector3.Distance (transform.position, x.transform.position) <= closeRadius);
	}
	
	private bool teammatesNearby()
	{
		var npcs = GameObject.FindObjectsOfType<GameObject>().Where (x => x.getTeam().Equals(gameObject.getTeam ()) && x.getData ().Equals("NPC"));
		
		return npcs.Any(x => Vector3.Distance (transform.position, x.transform.position) <= closeRadius);
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
	
	private void signalAndAttack()
	{
		signal ();
		attackEnemy();
	}
	
	private void signalAndFlee()
	{
		signal ();
		fleeToBase();
	}
	
	public void signal()
	{
		var allObjects = GameObject.FindObjectsOfType<GameObject>();
		var otherNPCs = allObjects.Where (x => x.getTeam ().Equals (gameObject.getTeam ()) && x.getData().Equals ("NPC") && Vector3.Distance (transform.position, x.transform.position) <= closeRadius);
		
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
	
	public bool inTerrainBounds(Vector3 position)
	{
		return SettingParser.getTerrainBoundaries(Terrain.activeTerrain).Contains(position);
	}
}