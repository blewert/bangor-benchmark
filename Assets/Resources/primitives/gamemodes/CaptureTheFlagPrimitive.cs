using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;
using System.Linq;

public class CaptureTheFlagPrimitive : GamemodeScript
{
	private List<GameObject> blueTeam;
	private List<GameObject> redTeam;
	
	private GameObject human;
	
	void Start () 
	{
		var spawnHuman  = (bool)SettingParser.getSetting(instance, "spawnHuman");
		var playerCount = (int)SettingParser.getSetting (instance, "playerCount");

		//Actually could we just use:
		var originPoint = getOriginPoint();
		var randomOffset = randomXZAroundPoint(originPoint, 3f);
		
		//Spawn red and blue team	
		blueTeam = spawnTeam (() => randomXZAroundPoint(originPoint, 5f), playerCount / 2, characterInstance.controllerScript, "Blue");
		redTeam  = spawnTeam (() => randomXZAroundPoint(originPoint, 5f), playerCount / 2, characterInstance.controllerScript, "Red");
		
		if(spawnHuman)
		{
			//A human needs to be spawned. First pick a team to pull a single agent from.
			var pickedTeam = (UnityEngine.Random.value > 0.5f) ? (blueTeam) : (redTeam);
			
			//Then pick a random agent within this team.
			var pickedElem = pickedTeam.randomElement();
			
			//Remove controller
			Destroy(pickedElem.GetComponent(Type.GetType (characterInstance.controllerScript)));
			
			Debug.Log("spawn human.. " + pickedElem.getTeam ());
						
			//Add human controller
			//pickedElem.AddComponent (Type.GetType ("HumanKeyboardController"));
			
			human = pickedElem;
			
			var script = gameObject.AddComponent<CameraFollowCharacter>();
			script.target = human;
		}
	}
}
