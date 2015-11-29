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
	
	// Use this for initialization
	void Start () 
	{
		InvokeRepeating("updateSteerDirection", 0f, 2f);
		
		npc = GetComponent<ILocomotionScript>();
		//gun = GetComponentInChildren<Gun>();
		castdar = GetComponentInChildren<Castdar>();
		
		if(npc.gameObject.getTeam() == "Red")
			myTeam = Team.RED;
		else
			myTeam = Team.BLUE;
		
	}
	
	private void updateSteerDirection()
	{
		steerDirection = UnityEngine.Random.Range (0, 2) - 1;
	}
	
	// Update is called once per frame
	void Update () 
	{
		var walls = castdar.GetWalls();
		var objects = castdar.GetSeenObject(); 
		
		if(castdar.GetSeen () > 0)
		{
			//They've hit something..
			
			var props = objects.Where(x => x.seenOBJ.tag.Equals ("Prop"));
			var enemies = objects.Where (x => x.seenOBJ.getTeam () != gameObject.getTeam () && x.seenOBJ.getTeam () != "None");
			
			if(props.Count() > 0)
				avoidObstacles(props);
				
			else if(enemies.Count() > 0)
				attackEnemies(enemies);
				
		}
		else
		{
			//Nothing hit, wander!
			wander();
		}
	}
	
	public void avoidObstacles(IEnumerable<Castdar.HitObject> obstacles)
	{
		//Find closest object
		var closest = obstacles.OrderBy(x => x.distance).FirstOrDefault();
		
		//Debug.DrawLine (transform.position, closest.seenOBJ.transform.position, Color.yellow, 3f);
		
		if(getSteerDirection(closest.seenOBJ.transform.position) > 0)
			npc.turnRight();
		
		else
			npc.turnLeft();
	}
	
	
	public void wander()
	{	
		if(steerDirection < 0)
			npc.turnLeft();
			
		else
			npc.turnRight ();
			
		npc.moveForward();
	}
	
	public void attackEnemies(IEnumerable<Castdar.HitObject> enemies)
	{
		//Find closest enemy
		var closest = enemies.OrderBy(x => x.distance).FirstOrDefault();
		
		if(getSteerDirection(closest.seenOBJ.transform.position) > 0)
			npc.turnLeft();
			
		else
			npc.turnRight();
			
		npc.moveForward();		
	}
	
	public int getSteerDirection(Vector3 position)
	{
		var localPosition = transform.InverseTransformPoint(position);
		
		return -Math.Sign (localPosition.x);
	}
}
