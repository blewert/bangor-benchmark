using UnityEngine;
using System.Collections;
using System.Linq;
using System;

public class ForestPrimitive : MonoBehaviour 
{
	private enum Density
	{
		LOW, MEDIUM, HIGH, INVALID
	}
	
	public EnvironmentInstance instance;
	private GameObject terrain;
	private Density density;
	
	void Start () 
	{
		//Two settings:
		// - Terrain to instantiate at Vector3.zero
		// - forestDensity: how much trees are we to spawn?
		// (later on, the algorithm type - naive, spawnpoints, real)
		//
		// Find a way to tweak density/algorithm - new Instance(density, algorithm)(?)
		
		//Show debug if needed
		foreach(var setting in instance.settings)	
			DebugLogger.Log("Setting <b>" + setting.Key + "</b>" + " = " + setting.Value);
	
		//Instantiate terrain at (0, 0, 0)
		terrain = (GameObject)Instantiate(Resources.Load (instance.settings["terrainToLoad"]));
		
		//Set up density
		density = densityStringToEnum(instance.settings["density"]);
		
		if(density == Density.INVALID)
		{
			//Something has gone terribly wrong.
			Debug.LogError("Couldn't parse density setting. Expected values are {\"sparse\", \"medium\", \"dense\"}, and are case-insensitive!");
			return;
		}
	}
	
	private Density densityStringToEnum(string value) 
	{
		//Possible values & mapped densities
		string[] possibleValues	= { "sparse", "medium", "dense" };
		Density[] densities = { Density.LOW, Density.MEDIUM, Density.HIGH };
		
		//Does it exist? Can we map?
		if(!possibleValues.Contains(value.ToLower()))
			return Density.INVALID;
			
		//Return the mapped value
		return densities[Array.IndexOf(possibleValues, value.ToLower())];			
		
	}
}
