using UnityEngine;
using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

public class EnemyAI : MonoBehaviour
{
	private enum Direction
	{
		LEFT,
		RIGHT,
		NONE
	};
	
	private bool firing = false;
	private IMortar mortar;
	private Castdar castdar;
	private INPC npc;
	private Direction direction = Direction.NONE;
	
	// Use this for initialization
	void Start () 
	{
		mortar  = GetComponentInChildren<IMortar>();
		npc     = GetComponent<INPC>();
		castdar = GetComponentInChildren<Castdar>();	
		
		mortar.OnProjectileHit += new OnProjectileHitHandler(projectileHit);
	}
		
	public void projectileHit(GameObject projectile, Collision col)
	{
		Debug.Log ("projectile hit " + col.contacts[0].otherCollider.gameObject.tag);
	}
	
	private void wanderBehaviour(float[] walls)
	{
		stopFiring();
		
		if(walls.Min () >= castdar.radarRangeWalls)
		{
			//Haven't hit any walls
			npc.moveForward();
			direction = Direction.NONE;
			return;
		}
		else
		{
			//Have hit a wall
			//..
			
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
	}
	
	private void aggressiveBehaviour(List<Castdar.HitObject> enemies)
	{
		Castdar.HitObject closestEnemy = enemies[0];
		float closestDistance = Mathf.Infinity;
		
		foreach(var enemy in enemies)
		{
			float distance = Vector3.Distance(enemy.seenOBJ.transform.position, transform.position);
			
			if(distance < closestDistance)
			{
				closestDistance = distance;
				closestEnemy = enemy;				
			}
		}
		
		var newRotation = Quaternion.LookRotation(closestEnemy.seenOBJ.transform.position - transform.position);
		transform.rotation = Quaternion.Slerp (transform.rotation, newRotation, Time.deltaTime * 0.8f);
		//transform.LookAt (closestEnemy.seenOBJ.transform.position);
		
		startFiring();
	}
	
	private void stopFiring()
	{
		if(!firing)
			return;
		
		firing = false;
		CancelInvoke("fireMortar");
	}
	
	private void startFiring()
	{
		if(firing)
			return;
		
		firing = true;
		InvokeRepeating ("fireMortar", 0f, 0.5f);
	}
	
	private void fireMortar()
	{
		mortar.Fire ();
	}
	
	// Update is called once per frame
	void Update () 
	{
		float[] walls = castdar.GetWalls();
		var enemies = castdar.GetSeenObject();
		
		try
		{
			enemies = enemies.FindAll (x => (x.seenOBJ.tag.StartsWith ("Team") && x.seenOBJ.tag != gameObject.tag));
			
			if(enemies.Count > 0)
				aggressiveBehaviour(enemies);
			else
				wanderBehaviour(walls);	
		}
		catch(Exception e)
		{
		}
		
	}
}
