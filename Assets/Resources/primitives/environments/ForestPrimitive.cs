using UnityEngine;
using System.Collections;
using System.Linq;
using System;

public class ForestPrimitive : PrimitiveScript
{
	private enum Density
	{
		LOW, MEDIUM, HIGH, INVALID
	}
	
	private GameObject terrain;
	private Density density;
	
	public void Start () 
	{
		//Two settings:
		// - Terrain to instantiate at Vector3.zero
		// - forestDensity: how much trees are we to spawn?
		// (later on, the algorithm type - naive, spawnpoints, real)
		//
		// Find a way to tweak density/algorithm - new Instance(density, algorithm)(?)

		//Get density setting, algorithm and terrain to load
		var densitySetting   = (string)SettingParser.getSetting(instance, "density");
		var algorithmSetting = (string)SettingParser.getSetting(instance, "algorithm");
		var terrainToLoad    = (string)SettingParser.getSetting(instance, "terrainToLoad");
		
		DebugLogger.Log (instance.name + ": Algorithm is " +  algorithmSetting + ", density is " + densitySetting + ". The terrain that needs to be loaded is " + terrainToLoad);
		
		//Instantiate terrain at (0, 0, 0)
		terrain = (GameObject)Instantiate(Resources.Load (terrainToLoad));
		
		//Set up density
		density = densityStringToEnum(densitySetting);
		
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
