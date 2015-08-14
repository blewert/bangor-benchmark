using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MasterForestGenerator : MonoBehaviour 
{
	public enum ForestGenerationMethod
	{ 
		NAIVE, NAIVEWITHPROPS
	}
	
	[Header("Tree settings")]
	public GameObject treePrefab;
	public float cullY;
	
	[Header("Generation settings")]
	public ForestGenerationMethod generationMethod;
	public ForestGeneration.Density generationDensity; 
	public GameObject originPoint;
	
	private IForestGenerator forestGenerator;
	private List<GameObject> treePrefabs = new List<GameObject>();
	
	public void Start()
	{
		if(generationMethod == ForestGenerationMethod.NAIVE)
			forestGenerator = new NaiveForestGenerator();
			
		else if(generationMethod == ForestGenerationMethod.NAIVEWITHPROPS)
			forestGenerator = new NaiveWithPropsForestGenerator();
	
		forestGenerator.setOriginPoint(originPoint.transform.position);
		forestGenerator.setCullY(cullY);
		forestGenerator.setDensity(generationDensity);
		forestGenerator.generateTreePositions();
		
		instantiatePrefabs();
	}
	
	private void instantiatePrefabs()
	{
		var treePositions = forestGenerator.getTreePositions();
		
		foreach(Vector3 position in treePositions)
		{
			var tree = (GameObject)Instantiate((GameObject)treePrefab, position, ForestGeneration.randomYRotation(Quaternion.identity));
			treePrefabs.Add (tree);
			
			tree.transform.localScale = new Vector3(0.6f, 0.6f, 0.6f);
			
			tree.tag = "Obstacle";
		}
	}
}
