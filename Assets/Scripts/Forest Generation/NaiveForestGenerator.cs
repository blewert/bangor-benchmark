using UnityEngine;
using System.Collections;

public class NaiveForestGenerator : IForestGenerator 
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
			treeSpawnAmount = 800;
											
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
	}
}
