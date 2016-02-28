using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System;

public class CaptureTheFlagEvolutionController : MonoBehaviour 
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
	public bool underFire = false;
	
	private float targetRotation = default(float);
	
	public List<Action> actions = new List<Action>();
	public List<Action> genes = new List<Action>();

	// Use this for initialization
	void Start () 
	{	
		setupActions();
		
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
	
	private void setupActions()
	{
		//Set up global actions
		Action[] localActions = 
		{
			() => flee(),                 // 0
			() => signalAndFlee(),        // 1
			() => fleeToBase(),           // 2
			() => wander(),               // 3
			() => wander(),               // 4
			() => attackEnemy(),          // 5
			() => signalAndAttack(),      // 6
			() => attackSignalledAgent()  // 7
		};
		
		//Add all actions
		actions.AddRange (localActions);
			
		//Set up genes
		genes = actions;
	}
	
	private void playAction(int number)
	{
		actions[number].Invoke();
	}
	
	private void playAttack(int number)
	{
		actions[number + 5].Invoke ();
	}
	
	IEnumerator updateHeading()
	{
		while(true)
		{	
			float oldHeading = transform.rotation.eulerAngles.y;
			float newHeading = oldHeading + UnityEngine.Random.Range (-80f, 80f); 
			
			targetRotation = newHeading;
			
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
		{
			if(npc.getHealth() < 100.0f)
				npc.giveHealth(2f);
		}
		
		//If they're outside of the terrain boundaries, move them back to the base
		if(!inTerrainBounds(transform.position))
		{
			fleeToBase();
			targetRotation = transform.eulerAngles.y - 180f;
		}
		
		
		//Check for obstacles:
		if(castdar.GetSeen() > 0)
		{
			var objs = castdar.GetSeenObject().Where (x => x.seenOBJ != null && x.seenOBJ.tag.Equals ("Prop"));
			
			if(objs.Count() > 0)
				avoidObstacles();
		}
		
		
		if(underFire)
		{
			if(!opponentSeen())
			{
				//An opponent is not seen, and their health is low. So go back to base:
				if(healthIsLow ())
					//fleeToBase();
					playAction (2);
				
				//Otherwise just wander.				
				else
					//wander();
					playAction (3);
				
				underFire = false;
			}
			else
			{
				if(teammatesNearby())
				{
					//They have seen an opponent and there are team-mates nearby. If they have low hp, signal and go to base.
					if(healthIsLow())
						//signalAndFlee();
						playAction (1);
					
					//If they have high hp, signal and attack.
					else
						//signalAndAttack();
						playAttack (1);
				}
				else
				{
					//Otherwise, there are no team-mates nearby. If they have low hp, flee.
					if(healthIsLow())
						//flee ();
						playAction (0);
					
					else
					{
						//However if they do have health, and there are enemies nearby, flee.
						if(enemyTeammatesNearby())
							//flee();
							playAction (0);
						
						//Otherwise, attack the enemy (because they're on their own).
						else
							//attackEnemy();
							playAttack (0);
					}					
				}
			}
		}
		else
		{
			if(!opponentSeen())
			{
				if(hasBeenSignalled())
				{
					if(healthIsLow())
						//flee ();
						playAction (0);
						
					else
						//attackSignalledAgent();
						playAttack (2);
						
				}
				else
				{
					if(healthIsLow())
						//fleeToBase();
						playAction (2);
					
					else
						//wander ();
						playAction(4);
				}
			}
			else
			{
				if(teammatesNearby())
				{
					if(healthIsLow())
						//signalAndFlee();
						playAction (1);
						
					else
						//signalAndAttack();
						playAttack (1);
				}
				else
				{
					if(healthIsLow ())
						//flee ();
						playAction (0);
						
					else
					{
						if(enemyTeammatesNearby())
							//flee ();
							playAction (0);
							
						else
							//attackEnemy ();
							playAttack(0);
					}
						
				}
					
			}
		}
		//if(npc.getHealth() <= 50f)
		//	fleeToBase();
	}
	
	private void avoidObstacles()
	{
		helpPosition = default(Vector3);
		npc.turnRight ();
	}
	
	private void flee()
	{
		npc.modifySpeed(1.5f);
		npc.modifyTurnRate(2f);

		wander ();
		
		npc.modifySpeed(1f / 1.5f);
		npc.modifyTurnRate(1f / 2f);
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
	
	private void attackSignalledAgent()
	{
		if(getSteerDirection(helpPosition) > 0)
			npc.turnRight();
		
		else
			npc.turnLeft ();
		
		npc.moveForward();		
			
		//Get closest enemy
		var objs = castdar.GetSeenObject().Where (x => x.seenOBJ.getData ().Equals ("NPC") && x.seenOBJ.getTeam () != gameObject.getTeam());
		var closestObj = objs.OrderBy(x => Vector3.Distance (x.seenOBJ.transform.position, transform.position)).FirstOrDefault();
		
		if(closestObj == null || closestObj.seenOBJ == null)
			return;
			
		if(Vector3.Distance (closestObj.seenOBJ.transform.position, transform.position) <= 8f)
		{
			closestObj.seenOBJ.GetComponent<ILocomotionScript>().takeHealth(0.5f);
			
			if(closestObj.seenOBJ.getData ().Equals ("NPC"))
			{
				//Get the controller 
				var controller = closestObj.seenOBJ.GetComponent<CaptureTheFlagEvolutionController>();
				
				//Set them to be under fire
				controller.underFire = true;
			}
		}		
	}
	
	private void attackEnemy()
	{
		var objs = castdar.GetSeenObject().Where (x => x.seenOBJ.getData ().Equals ("NPC") && x.seenOBJ.getTeam () != gameObject.getTeam());
		
		var closestObj = objs.OrderBy(x => Vector3.Distance (x.seenOBJ.transform.position, transform.position)).FirstOrDefault();
		
		if(closestObj == null || closestObj.seenOBJ == null)
			return;
		
		if(getSteerDirection(closestObj.seenOBJ.transform.position) > 0)
			npc.turnRight ();
		else
			npc.turnLeft ();
		
		if(Vector3.Distance (closestObj.seenOBJ.transform.position, transform.position) <= 8f)
		{
			closestObj.seenOBJ.GetComponent<ILocomotionScript>().takeHealth(0.5f);
			
			if(closestObj.seenOBJ.getData ().Equals ("NPC"))
			{
				//Get the controller 
				var controller = closestObj.seenOBJ.GetComponent<CaptureTheFlagEvolutionController>();
				
				//Set them to be under fire
				controller.underFire = true;
			}
		}

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
		flee();
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