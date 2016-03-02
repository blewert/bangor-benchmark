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
	
	private int lowerLimit;
	private int playerCount;
	private int higherLimit;
	private int availableGenerations;

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
		availableGenerations = EvolutionResultsParser.getAvailableGenerations(runNumber, "generations.txt").Count;
		
		Debug.Log ("There are " + availableGenerations + " generations available.");
		
		lowerLimit  = (int)(availableGenerations * lowPercentile);
		higherLimit = (int)(availableGenerations - (availableGenerations * highPercentile));
		
		Debug.Log ("low 0 to " + lowerLimit + ", high " + higherLimit + " to " + availableGenerations);
		
		//Call capture the flag stuff
		captureTheFlagInit();	
		
		//Attach particles
		attachParticles();		
		
		//Add gui
		addGUI();	
	}
	
	private void addGUI()
	{
		var prefab = Resources.Load ("prefabs/evolutionGUI");
		
		Instantiate (prefab, Vector3.zero, Quaternion.identity);
	}
	
	private void attachParticles()
	{
		//Get all npcs
		var npcs = blueTeam.Concat (redTeam);
		
		//Get the prefab from resources
		var prefab = Resources.Load ("prefabs/healthParticleSystem");
		
		
		//Instantiate the object
		foreach(var npc in npcs)
		{
			var obj = Instantiate (prefab, npc.transform.position, npc.transform.rotation) as GameObject;
			
			obj.transform.parent = npc.transform;
			obj.transform.localPosition = Vector3.zero;
			obj.transform.rotation = Quaternion.Euler(new Vector3(-90, 0, 0));
			
			obj.GetComponentInChildren<ParticleSystem>().Stop();
		}
		
	}
	
	private void captureTheFlagInit()
	{
		//Get player count
		playerCount = (int)SettingParser.getSetting(instance, "playerCount");
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
		
		//Apply evolution stuff to each agent
		changeGenes();	
		
		//Add in the human
		addHuman();		
		
		//And finally spawn the flags	
		spawnFlags(flagSpawnRadius);
	}
	
	private void changeGenes()
	{
		//Get all agents
		var agents = blueTeam.Concat(redTeam);
		
		//Get all agents which aren't a human character
		agents = agents.Where (x => !x.getData ().Equals("Human"));

		//Generations
		var lowGenerations  = new List<EvolutionAgent>();
		var highGenerations = new List<EvolutionAgent>();
		
		for(int i = 0; i <= lowerLimit; i++)
		{
			//Run through every generation. Get all agents for this generation.
			var generationData = EvolutionResultsParser.getEvolutionData(runNumber, i);
			
			//Get the highest fitness
			var highestFitness = generationData.OrderByDescending(x => x.Value.fitness).ToList ();
			
			//Add this agent with the highest fitness to the list of agents
			lowGenerations.Add (highestFitness[0].Value);
		}
		
		for(int i = higherLimit; i < availableGenerations - 1; i++)
		{
			//Run through every generation. Get all agents for this generation.
			var generationData = EvolutionResultsParser.getEvolutionData(runNumber, i);
			
			//Get the highest fitness
			var highestFitness = generationData.OrderByDescending(x => x.Value.fitness).ToList ();
			
			//Add this agent with the highest fitness to the list of agents
			highGenerations.Add (highestFitness[0].Value);
		}
		
		//Sort the lists of highest fitnesses by highest fitness
		lowGenerations = lowGenerations.OrderByDescending(x => x.fitness).ToList ();
		highGenerations = highGenerations.OrderByDescending(x => x.fitness).ToList ();
		
		var team = blueTeam.Concat (redTeam).ToList ();
		
		for(int i = 0; i < team.Count(); i++)
		{
			team[i].GetComponent<CaptureTheFlagEvolutionController>().setGenes(highGenerations[i].genes);
		}

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
		
		foreach(var agent in blueTeam)
			agent.GetComponentInChildren<MeshRenderer>().material.color = new Color(52/255f, 152/255f, 219/255f);
		
		foreach(var agent in redTeam)
			agent.GetComponentInChildren<MeshRenderer>().material.color = new Color(192/255f, 57/255f, 43/255f);
		
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
		pickedElem.AddComponent (Type.GetType ("HumanEvolutionKeyboardController"));
		
		//Make this person a human
		pickedElem.setData ("NPC");
		
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
		
		var objs = obj1.GetComponentsInChildren<MeshRenderer>();
		
		foreach(var obj in objs)
			obj.material.color = new Color(192/255f, 57/255f, 43/255f);
			
		objs = obj2.GetComponentsInChildren<MeshRenderer>();
		
		foreach(var obj in objs)
			obj.material.color = new Color(52/255f, 152/255f, 219/255f);
		
		obj1.setData ("Flag");
		obj2.setData ("Flag");
		
		obj1.tag = "Prop";
		obj2.tag = "Prop";
	}
}
