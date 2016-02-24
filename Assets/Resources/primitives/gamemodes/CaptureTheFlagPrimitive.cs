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

		var humanVsHuman = (bool)SettingParser.getSetting (instance, "humanVsHuman");
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
		
		if (spawnHuman && !humanVsHuman) {
			//If we're playing with JUST a human (so human and observer)
			//NOT human vs human with no observer.
			//..

			//A human needs to be spawned. First pick a team to pull a single agent from.
			var pickedTeam = (UnityEngine.Random.value > 0.5f) ? (blueTeam) : (redTeam);
			
			//Then pick a random agent within this team.
			var pickedElem = pickedTeam.randomElement ();
			
			//Remove controller
			Destroy (pickedElem.GetComponent (Type.GetType (characterInstance.controllerScript)));
						
			//Add human controller
			pickedElem.AddComponent (Type.GetType ("HumanKeyboardController"));

			//Human target gets set
			human = pickedElem;
			script.target = human;
		}
		else if (humanVsHuman) 
		{
			//How many players?
			var networkPlayerCount = network.players.Count();

			//Shuffle all npcs (locally, before picking)
			var shuffleCharacters = network.characters.OrderBy(x => Guid.NewGuid()).ToList ();

			//attach first one for this player.
			//..
			var thePlayer = shuffleCharacters.ElementAt(0).Value;

			// apply playerContoller script to that character.
			thePlayer.AddComponent<PlayerController>();
			
			// uncheck or remove humanAIcontroller
			thePlayer.GetComponent<HumanEnemyAI> ().enabled = false;

			//Get rid of the main camera for now
			Camera.main.enabled = false;
			thePlayer.transform.Find("Main Camera").gameObject.SetActive(true);

			Debug.Log ("the server player is npc " + shuffleCharacters.ElementAt(0).Key + " ...");

			//Remove that element so it doesnt get repicked!
			shuffleCharacters = shuffleCharacters.Where (x => shuffleCharacters.IndexOf(x) != 0).ToList ();

			//Run for the given amount of players.
			for(int i = 0; i < networkPlayerCount; i++)
			{
				//Pick a random NPC
				Debug.Log ("player " + i + " needs to attach a human controller to npc " + shuffleCharacters.ElementAt(i).Key);
			

				//Call to this player to attach the controller thingymabob
				network.networkView.RPC("setHumanControlledCharacter", network.players[i], shuffleCharacters.ElementAt(i).Key);

				// unchecking AI controls so it cannot be controlled by AI script
				shuffleCharacters.ElementAt(i).Value.GetComponent<HumanEnemyAI>().enabled = false;

				//Remove the character so we dont pick it again and therefore we dont attach two controllers
				//to the same npc!
				shuffleCharacters = shuffleCharacters.Where (x => x.Key != shuffleCharacters.ElementAt(i).Key).ToList ();
			}

			//Go through the rest of these characters
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
