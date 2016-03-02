using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class HumanEvolutionKeyboardController : MonoBehaviour 
{
	private ILocomotionScript locomotionScript;
	private Castdar castdar;
	
	private float originalTurnSpeed;
	private float originalSpeed;
	
	private CapsuleLocomotion capsuleScript;
	
	public void Start()
	{
		locomotionScript = GetComponent<ILocomotionScript>();	
		castdar = GetComponentInChildren<Castdar>();
		
		capsuleScript = locomotionScript as CapsuleLocomotion;
		
		originalSpeed = capsuleScript.speed;
		originalTurnSpeed = capsuleScript.turnSpeed;
	}
	
	public void onSelfHit(GameObject source)
	{
		Debug.Log ("bing, bong " + source.getID ());
	}
	
	public void Update()
	{
		if(Input.GetKey (KeyCode.LeftShift))
		{
			capsuleScript.speed = originalSpeed * 1.5f;
			capsuleScript.turnSpeed = originalTurnSpeed * 2f; 
		}
		
		if(Input.GetKeyUp (KeyCode.LeftShift))
		{
			capsuleScript.speed = originalSpeed;
			capsuleScript.turnSpeed = originalTurnSpeed;
		}
		
		if(Input.GetKey(KeyCode.W))
			locomotionScript.moveForward();
		
		else if(Input.GetKey (KeyCode.S))
			locomotionScript.moveBackward();
		
		if(Input.GetKey (KeyCode.A))
			locomotionScript.turnLeft();
		
		else if(Input.GetKey (KeyCode.D))
			locomotionScript.turnRight();
			
		if(Input.GetKey (KeyCode.Space))
		{
			//Fire, somehow?
			//..
			
			if(castdar.GetSeen() > 0)
			{
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
		}		
	}
}
