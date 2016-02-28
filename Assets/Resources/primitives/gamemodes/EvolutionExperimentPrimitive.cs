using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using System.Linq;

public class EvolutionExperimentPrimitive : GamemodeScript
{
	private List<GameObject> blueTeam;
	private List<GameObject> redTeam;
	
	private GameObject human;
	
	private string resultsPath;
	private int runNumber;
	
	private float highPercentile = 0.25f;
	private float lowPercentile  = 0.25f;

	private GameObject flagPrefab;
		
	void Start () 
	{
		//Set up multiplayer stuff
		findNetworkServer();
		
		//Find base path for results directory so we can pull out agent genes. Also pull out the run number (part of the path too)
		resultsPath = (string)SettingParser.getSetting(instance, "resultsDirectory");
		runNumber   = (int)SettingParser.getSetting (instance, "runNumber");
		
		//Tell the parser to pull results from this directory
		EvolutionResultsParser.setResultsDirectory(resultsPath);		
		
		//Get the amount of available generations.
		var availableGenerations = EvolutionResultsParser.getAvailableGenerations(runNumber, "generations.txt").Count;
		
		Debug.Log ("There are " + availableGenerations + " generations available.");
		
		var lowerLimit  = (int)(availableGenerations * lowPercentile);
		var higherLimit = (int)(availableGenerations - (availableGenerations * highPercentile));
		
		Debug.Log ("low 0 to " + lowerLimit + ", high " + higherLimit + " to " + availableGenerations);
		
		//Call capture the flag stuff
		captureTheFlagInit();				
	}
	
	private void captureTheFlagInit()
	{
		//Get player count
		var playerCount = (int)SettingParser.getSetting(instance, "playerCount");
		var flagSpawnRadius = (float)SettingParser.getSetting (instance, "flagSpawnRadius");
		
		//Get flag prefab
		flagPrefab = (GameObject)Resources.Load ((string)SettingParser.getSetting (instance, "flagPrefab"));
		
		//Actually could we just use:
		var originPoint = getOriginPoint();
		var randomOffset = randomXZAroundPoint(originPoint, 3f);
		
		//Spawn red and blue team	
		blueTeam = spawnTeam (() => randomXZAroundPoint(originPoint, 30f), playerCount / 2, characterInstance.controllerScript, "Blue");
		redTeam  = spawnTeam (() => randomXZAroundPoint(originPoint, 30f), playerCount / 2, characterInstance.controllerScript, "Red");
		
		//Swap out controllers to evolution controllers
		changeControllers();
		
		//Add in the human
		addHuman();		
		
		//And finally spawn the flags	
		spawnFlags(flagSpawnRadius);
		
		//Apply evolution stuff to each agent
		changeGenes();		
	}
	
	private void changeGenes()
	{
		//Get all agents
		var agents = blueTeam.Concat(redTeam);
		
		//Get all agents which aren't a human character
		agents = agents.Where (x => !x.getData ().Equals("Human"));
		
		//At this point, pick random generation range(?) to pull fitnesses from
		//Debug.Log ("... " + agents.Count());
	}
	
	private void changeControllers()
	{
		foreach(var agent in blueTeam.Concat(redTeam))
		{
			//Remove existing 
			Destroy (agent.GetComponent<CaptureTheFlagAIController>());
			
			//Add evolution controller
			var controller = agent.AddComponent<CaptureTheFlagEvolutionController>();
		}
	}
	
	private void addHuman()
	{
		//Attach a camera following script so the player can see where they're going
		var script = gameObject.AddComponent<CameraFollowCharacter>();
		
		//A human needs to be spawned. First pick a team to pull a single agent from.
		var pickedTeam = (UnityEngine.Random.value > 0.5f) ? (blueTeam) : (redTeam);
		
		//Then pick a random agent within this team.
		var pickedElem = pickedTeam.randomElement();
		
		//Remove controller
		Destroy(pickedElem.GetComponent(Type.GetType (characterInstance.controllerScript)));
		Destroy (pickedElem.GetComponent<CaptureTheFlagEvolutionController>());
		
		//Add human controller
		pickedElem.AddComponent (Type.GetType ("HumanKeyboardController"));
		
		//Make this person a human
		pickedElem.setData ("Human");
		
		//Replace the human
		human = pickedElem;
		script.target = human;
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
