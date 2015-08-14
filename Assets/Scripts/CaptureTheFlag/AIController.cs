using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System;

public class AIController : MonoBehaviour 
{
	private enum Team
	{
		RED, BLUE 
	}
	
	private enum Direction
	{
		NONE, LEFT, RIGHT
	}
	
	private float FLAG_PICKUP_RADIUS = 5.0f;
	
	private Direction direction = Direction.NONE;
	private Direction steerDirection;
	
	private Castdar castdar;
	private Team myTeam;
	
	public INPC npc;
	public Gun gun;
	
	private bool hasFlag = false;

	public GameObject myBaseRadius;
	public GameObject enemyBaseRadius;
	
	public GameObject enemyFlag;
	public GameObject myFlag;
	
	public Vector3 enemyBasePos;
	public Vector3 myBasePos;
	
	private Logger logger;
	private float yCull = 12f;
	
	// Use this for initialization
	void Start () 
	{
		InvokeRepeating ("updateSteerDirection", 0, 0.2f);
		npc = GetComponent<INPC>();
		gun = GetComponentInChildren<Gun>();
		castdar = GetComponentInChildren<Castdar>();
		
		if(npc.gameObject.tag == "Team1")
			myTeam = Team.RED;
		else
			myTeam = Team.BLUE;
			
		logger = GameObject.Find ("Observer").GetComponent<Logger>();
			
		GameObject redFlag = GameObject.FindGameObjectWithTag("Red Flag");
		GameObject blueFlag = GameObject.FindGameObjectWithTag("Blue Flag");
		
		GameObject redBase = GameObject.Find("Red Base");
		GameObject blueBase = GameObject.Find("Blue Base");
		
		if(myTeam == Team.RED)
		{
			enemyFlag = blueFlag;
			enemyBasePos = blueFlag.transform.position;
			enemyBaseRadius = blueBase;
			
			myFlag = redFlag;
			myBasePos = redFlag.transform.position;
			myBaseRadius = redBase;
		}
		else
		{
			enemyFlag = redFlag;
			enemyBasePos = redFlag.transform.position;
			enemyBaseRadius = redBase;
			
			myFlag = blueFlag;
			myBasePos = blueFlag.transform.position;
			myBaseRadius = blueBase;
		}
	}
	
	void updateSteerDirection()
	{
		int randomValue = UnityEngine.Random.Range (0, 3);
		
		if(randomValue == 0)
			steerDirection = Direction.NONE;
			
		else if(randomValue == 1)
			steerDirection = Direction.LEFT;
		
		else
			steerDirection = Direction.RIGHT;
	}
	
	private string getNPCName()
	{
		return npc.GetComponent<NameTag>().text;
	}
	
	// Update is called once per frame
	void Update () 
	{		
		//gun.Fire ();
		var walls = castdar.GetWalls();
		var objects = castdar.GetSeenObject(); 
		
		//Debug.Log (objects.Count);
		//Walls are ignored with GetSeenObject()
		
		if(npc.health <= 0f)
		{
			if(hasFlag)
			{
				enemyFlag.transform.SetParent(null);
				enemyFlag.transform.position = enemyBasePos;
			}
			
			logger.addMessage (getNPCName() + " died!");
						
			hasFlag = false;
			
			npc.health = INPC.MAX_HEALTH;
			
			Vector3 position = myBasePos;
			
			position.x += UnityEngine.Random.Range (-10f, 10f);
			position.z += UnityEngine.Random.Range (-10f, 10f);
			
			Vector3 euler = new Vector3(0, UnityEngine.Random.Range (0, 360), 0); 
			
			npc.transform.rotation = Quaternion.Euler(euler);
			npc.transform.position = position;
		}
		
		if(walls.Min () >= castdar.radarRangeWalls)
		{
			//Debug.Log("no walls?");
			wander ();
			
			if(Vector3.Distance (npc.transform.position, enemyFlag.transform.position) < FLAG_PICKUP_RADIUS)
			{
				logger.addMessage (getNPCName() + " picked up the flag!");
				hasFlag = true;
				enemyFlag.transform.SetParent (npc.transform);
			}
			
			if(hasFlag)
			{
				Direction steerDirection = getSteerDirection(myBasePos);
				
				if(steerDirection == Direction.LEFT)
					npc.turnLeft ();
				else
					npc.turnRight ();
					
				npc.moveForward();
				
				if(Vector3.Distance (npc.transform.position, myBasePos) < FLAG_PICKUP_RADIUS)
				{
					logger.addMessage (getNPCName() + " dropped the flag!");
					enemyFlag.transform.SetParent(null);
					enemyFlag.transform.position = myBasePos;
					hasFlag = false;
				}
			}
		}
		else
		{
			var wallObjects = castdar.GetSeenWalls();
			
			if(npc.transform.position.y <= yCull)
			{
				var direction = getSteerDirection(myBasePos);
				
				if(direction == Direction.LEFT)
					npc.turnLeft ();
					
				else
					npc.turnRight ();
			
				npc.moveForward();
			}
			else if(wallObjects.Find(x => x.seenOBJ.tag == "Obstacle" || x.seenOBJ.tag == "Prop") != null)
			{
				//Walls found
				avoid (walls);
			}
			else
			{
				//Walls not found
				
				//checkety check
				if(objects.Find (x => (x.seenOBJ.tag.StartsWith("Team") && x.seenOBJ.tag != this.npc.tag)) != null)
				{
					//enemy detected
					var enemy = objects.Find (x => (x.seenOBJ.tag.StartsWith("Team") && x.seenOBJ.tag != this.npc.tag));
					
					attack(enemy);
				}
				else
				{
					if(objects.Find (x => x.seenOBJ == enemyBaseRadius) != null)
					{
						//Found enemy base
						Direction steerDirection = getSteerDirection(enemyBasePos);
						
						if(steerDirection == Direction.LEFT)
							npc.turnLeft ();
						else
							npc.turnRight ();
							
						npc.moveForward();
					}
					else
					{
						wander ();
					}
				}
			}
			
		}
	}
	
	private void attack(Castdar.HitObject enemy)
	{
		var enemyNPC = enemy.seenOBJ.GetComponent<INPC>();
		
		enemyNPC.takeHealth(1f/30f);
		
		Direction steerDirection = getSteerDirection(enemy.seenOBJ.transform.position);
		
		if(steerDirection == Direction.LEFT)
			npc.turnLeft ();
		else
			npc.turnRight ();
	}
	
	private void avoid(float[] walls)
	{
		int index = Array.IndexOf (walls, walls.Min());
		
		if(direction == Direction.NONE)
		{
			if(index < walls.Length / 2)
				direction = Direction.RIGHT;
			else
				direction = Direction.LEFT;
		}
		
		if(direction == Direction.RIGHT)
			npc.turnRight();
		
		else if(direction == Direction.LEFT)
			npc.turnLeft ();
	}
	
	private void wander()
	{
		npc.moveForward ();
	}
	
	private Direction getSteerDirection(Vector3 position) 
	{
		Vector3 localPoint = npc.transform.InverseTransformPoint(position);
		
		if(Mathf.Sign (localPoint.x) > 0)
			return Direction.RIGHT;
		
		else
			return Direction.LEFT;
	}
	
}
