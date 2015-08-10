using UnityEditor;
using UnityEngine;
using System.Collections;

public class TerrainGen : MonoBehaviour {
	
	public float maxTiling = 15.0f;
	public float minTiling = 6.0f;
	
	void Start()
	{
		float tiling = Random.Range (minTiling, maxTiling);
		GenerateHeights(Terrain.activeTerrain, tiling);
	}
	
	public void GenerateHeights(Terrain terrain, float tileSize)
	{
		float[,] heights = new float[terrain.terrainData.heightmapWidth, terrain.terrainData.heightmapHeight];
		
		for (int i = 0; i < terrain.terrainData.heightmapWidth; i++)
		{
			for (int k = 0; k < terrain.terrainData.heightmapHeight; k++)
			{
				heights[i, k] = Mathf.PerlinNoise(((float)i / (float)terrain.terrainData.heightmapWidth) * tileSize, ((float)k / (float)terrain.terrainData.heightmapHeight) * tileSize)/10.0f;
			}
		}
		
		terrain.terrainData.SetHeights(0, 0, heights);
	}
}