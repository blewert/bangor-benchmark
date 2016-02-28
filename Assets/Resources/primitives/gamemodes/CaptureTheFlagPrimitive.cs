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
	
	private GameObject flagPrefab;
	
	void Start () 
	{
		findNetworkServer();
		
		var spawnHuman  = (bool)SettingParser.getSetting(instance, "spawnHuman");
		var playerCount = (int)SettingParser.getSetting (instance, "playerCount");
		var flagSpawnRadius = (float)SettingParser.getSetting (instance, "flagSpawnRadius");
		
		flagPrefab = (GameObject)Resources.Load ((string)SettingParser.getSetting (instance, "flagPrefab"));
		
		//Actually could we just use:
		var originPoint = getOriginPoint();
		var randomOffset = randomXZAroundPoint(originPoint, 3f);
		
		//Spawn red and blue team	
		blueTeam = spawnTeam (() => randomXZAroundPoint(originPoint, 30f), playerCount / 2, characterInstance.controllerScript, "Blue");
		redTeam  = spawnTeam (() => randomXZAroundPoint(originPoint, 30f), playerCount / 2, characterInstance.controllerScript, "Red");
		
		var script = gameObject.AddComponent<CameraFollowCharacter>();
		
		if(spawnHuman)
		{
			//A human needs to be spawned. First pick a team to pull a single agent from.
			var pickedTeam = (UnityEngine.Random.value > 0.5f) ? (blueTeam) : (redTeam);
			
			//Then pick a random agent within this team.
			var pickedElem = pickedTeam.randomElement();
			
			//Remove controller
			Destroy(pickedElem.GetComponent(Type.GetType (characterInstance.controllerScript)));

			//Add human controller
			pickedElem.AddComponent (Type.GetType ("HumanKeyboardController"));
			
			human = pickedElem;
			
			script.target = human;
		}
		else
		{
			script.targets = redTeam.Concat (blueTeam).ToList();
		}
		
		spawnFlags(flagSpawnRadius);
	}
	
	private void spawnFlags(float radius)
	{
		var originPoint = getOriginPoint();
		
		var firstRandomAngle = UnityEngine.Random.Range (0f, 360f);
		var secondRandomAngle = (firstRandomAngle + 180f) % 360f;
		
		var firstPosition = originPoint;
		firstPosition.x += Mathf.Cos(firstRandomAngle * Mathf.Deg2Rad) * radius;
		firstPosition.z += Mathf.Sin(firstRandomAngle * Mathf.Deg2Rad) * radius;
		firstPosition.y  = Terrain.activeTerrain.SampleHeight(firstPosition);
		
		var secondPosition = originPoint;
		secondPosition.x += Mathf.Cos(secondRandomAngle * Mathf.Deg2Rad) * radius;
		secondPosition.z += Mathf.Sin(secondRandomAngle * Mathf.Deg2Rad) * radius;
		secondPosition.y  = Terrain.activeTerrain.SampleHeight(secondPosition);
		
		var obj1 = network.createObject(flagPrefab, firstPosition, randomYRotation());
		var obj2 = network.createObject(flagPrefab, secondPosition, randomYRotation());
		
		obj1.setTeam ("Red");
		obj2.setTeam ("Blue");
		
		obj1.setData ("Flag");
		obj2.setData ("Flag");
		
		obj1.tag = "Prop";
		obj2.tag = "Prop";
	}
}
