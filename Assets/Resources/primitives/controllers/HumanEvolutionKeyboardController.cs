using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.UI;

public class HumanEvolutionKeyboardController : MonoBehaviour 
{
	private ILocomotionScript locomotionScript;
	private Castdar castdar;
	
	private float originalTurnSpeed;
	private float originalSpeed;
	
	private CapsuleLocomotion capsuleScript;
	private GameObject myFlag;
	
	private Text guiToChange;
	private GameObject guiPanel;
	
	public void Start()
	{
		locomotionScript = GetComponent<ILocomotionScript>();	
		castdar = GetComponentInChildren<Castdar>();
				
		capsuleScript = locomotionScript as CapsuleLocomotion;
		
		originalSpeed = capsuleScript.speed;
		originalTurnSpeed = capsuleScript.turnSpeed;
	
		var myTeam = gameObject.getTeam ();
		
		var objs = GameObject.FindGameObjectsWithTag("Prop");
		var flag = objs.Where (x => x.getData ().Equals ("Flag") && x.getTeam ().Equals (myTeam)).First ();
		
		myFlag = flag;
		
		var gui = GameObject.Find ("evolutionGUI(Clone)");
		
		guiPanel = gui.transform.FindChild ("notifyPanel").gameObject;
		guiPanel.SetActive (false);
		
		guiToChange = gui.transform.FindChild ("healthText").GetComponent<Text>();
	}
	
	public void onSelfHit(GameObject source)
	{
		locomotionScript.takeHealth (0.5f);
	}
	
	public void Update()
	{
		var allObjs = GameObject.FindGameObjectsWithTag("Player");
		
		var blueObjCount = allObjs.Where (x => x.getTeam ().Equals ("Blue")).Count ();
		var redObjCount  = allObjs.Where (x => x.getTeam ().Equals ("Red")).Count ();
		
		guiToChange.text = "Health: " + locomotionScript.health + " | Blue: " + blueObjCount + " | Red: " + redObjCount;
		
		
		
		if(redObjCount == 0 || blueObjCount == 0)
		{
			var textObj = guiPanel.GetComponentInChildren<Text>();
			
			var myTeam = gameObject.getTeam();
			
			if((redObjCount == 0 && myTeam.Equals("Red")) || (blueObjCount == 0 && myTeam.Equals ("Blue")))
			{
				textObj.text = "You lost the round!";
				guiPanel.SetActive(true);
			}	
			else
			{
				textObj.text = "You won the round!";
				guiPanel.SetActive (true);
			}
		}
							
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
		
		if(Vector3.Distance (transform.position, myFlag.transform.position) <= 5f)
		{
			if(locomotionScript.getHealth() < 100.0f)
			{
				if(GetComponentInChildren<ParticleSystem>().isStopped)
					GetComponentInChildren<ParticleSystem>().Play ();
					
				locomotionScript.giveHealth(2f);		
			}
			else
			{
				//Health is full, stop if playing
				if(!GetComponentInChildren<ParticleSystem>().isStopped)
					GetComponentInChildren<ParticleSystem>().Stop ();
			}
		}
		else
		{
			//Not in radius, stop playing if not stopped
			if(!GetComponentInChildren<ParticleSystem>().isStopped)
				GetComponentInChildren<ParticleSystem>().Stop ();
		}
	}
}
