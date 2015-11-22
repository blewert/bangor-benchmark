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
	
	private Terrain terrain;
	private Density density;
	private GameObject treePrefab;
	private Vector3 originPoint;
	private float radius;
	
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
		var treeToLoad       = (string)SettingParser.getSetting(instance, "treePrefab");
		radius               = (float )SettingParser.getSetting(instance, "treeSpawnRadius");
		
		DebugLogger.Log (instance.name + ": Algorithm is " +  algorithmSetting + ", density is " + densitySetting + ". The terrain that needs to be loaded is " + terrainToLoad);
		
		//Instantiate terrain at (0, 0, 0)
		var gameObjectTerrain = Instantiate(Resources.Load (terrainToLoad), Vector3.zero, Quaternion.identity) as GameObject;
		terrain = gameObjectTerrain.GetComponent<Terrain>();
		
		//Get origin point
		originPoint = SettingParser.getTerrainOriginPoint(terrain);
		
		//Load in tree prefab from path
		treePrefab = Resources.Load (treeToLoad) as GameObject;
		
		//Set up density
		density = densityStringToEnum(densitySetting);
		
		if(density == Density.INVALID)
		{
			//Something has gone terribly wrong.
			Debug.LogError("Couldn't parse density setting. Expected values are {\"sparse\", \"medium\", \"dense\"}, and are case-insensitive!");
			return;
		}
		
		//Otherwise, spawn some trees
		spawnTrees();
	}
	
	private void spawnTrees()
	{
		uint amountToSpawn = 0;
		
		if(density == Density.LOW)
			amountToSpawn = (uint)Mathf.Pow (2, 6);
		
		else if(density == Density.MEDIUM)
			amountToSpawn = (uint)Mathf.Pow (2, 7);
			
		else if(density == Density.HIGH)
			amountToSpawn = (uint)Mathf.Pow (2, 8);
				
		for(uint i = 0; i < amountToSpawn; i++)
			Instantiate (treePrefab, randomXZAroundPoint(originPoint, radius), Quaternion.identity);
	}
	
	private Vector3 randomXZAroundPoint(Vector3 point, float magnitude)
	{
		Vector3 randomPos = point;
		
		randomPos.x += UnityEngine.Random.Range (-magnitude, magnitude);
		randomPos.z += UnityEngine.Random.Range (-magnitude, magnitude);
		
		randomPos.y = terrain.SampleHeight(randomPos);
		randomPos.y += treePrefab.GetComponent<MeshFilter>().sharedMesh.bounds.extents.y * treePrefab.transform.localScale.y;
		
		DebugLogger.Log (treePrefab.GetComponent<MeshFilter>().sharedMesh.bounds.extents.y);
		
		return randomPos;
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
