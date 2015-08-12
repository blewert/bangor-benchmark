using UnityEngine;
using System.Collections;

public class FlagSpawner : MonoBehaviour 
{
	public GameObject basePrefab;
	public GameObject spawnPrefab;
	
	public GameObject redFlagPrefab;
	public GameObject blueFlagPrefab;
	
	public GameObject originPoint;
	public float yCull = 12f; 
	public GameObject[] flags;
	public float minimumProximity;
	public float spawnRadius;
	
	// Use this for initialization
	void Start () 
	{
		int t = 0;
		
		flags = new GameObject[2];
		
		for(int i = 0; i < 2; i++)
		{
			Vector3 randPosition = ForestGeneration.randomXZAroundPoint(originPoint.transform.position, spawnRadius);
			
			while(randPosition.y <= yCull)
			{
				var consideredPoint = ForestGeneration.randomXZAroundPoint(originPoint.transform.position, spawnRadius);
				
				t++;
				
				if(t >= 1000)
					break;
					
				if(i == 1 && Vector3.Distance(consideredPoint, flags[0].transform.position) < minimumProximity)
					continue;
					
				randPosition = consideredPoint;
			}
			
			if(i == 0)
				flags[i] = (GameObject)Instantiate(redFlagPrefab, randPosition, Quaternion.identity);
			else
				flags[i] = (GameObject)Instantiate(blueFlagPrefab, randPosition, Quaternion.identity);
				
			Instantiate(basePrefab, randPosition, Quaternion.identity);
			Instantiate(spawnPrefab, randPosition, Quaternion.identity);
		}
	}
	
	// Update is called once per frame
	void Update () 
	{
	
	}
}
