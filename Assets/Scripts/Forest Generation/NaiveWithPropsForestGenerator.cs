using UnityEngine;
using System.Collections;

public class NaiveWithPropsForestGenerator : IForestGenerator 
{	
	private float generatorRange = 150f; 
	private int treeSpawnAmount;
	
	public override void generateTreePositions()
	{
		if(density == ForestGeneration.Density.LOW)
			treeSpawnAmount = 250;
		
		else if(density == ForestGeneration.Density.MEDIUM)
			treeSpawnAmount = 450;
		
		else
			treeSpawnAmount = 1300;
											
		for(int i = 0; i < treeSpawnAmount; i++)
		{
			Vector3 consideredTree = ForestGeneration.randomXZAroundPoint(originPoint, generatorRange);
			treePositions.Add(consideredTree);
		}
		
		cullTrees();
	}	
	
	private void cullTrees()
	{
		treePositions.RemoveAll(t => t.y <= yCulling);
		
		var props = GameObject.FindGameObjectsWithTag("Prop");
		
		foreach(var prop in props)
		{
			var renderer = prop.GetComponent<Renderer>();

			float radius = renderer.bounds.extents.magnitude;
			
			treePositions.RemoveAll (t => Vector3.Distance (t, prop.transform.position) < radius);	
		}
	}
	
}
