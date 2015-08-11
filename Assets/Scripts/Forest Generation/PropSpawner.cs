using UnityEngine;
using System.Collections;

public class PropSpawner : MonoBehaviour 
{
	public int spawnAmount = 3;
	public GameObject[] propPrefabs;
	public GameObject originPoint;
	public float range;
	public float yCull;
	
	public void Awake()
	{
		for(int i = 0; i < spawnAmount; i++)
		{
			Vector3 consideredPoint = ForestGeneration.randomXZAroundPoint(originPoint.transform.position, range);
			
			while(consideredPoint.y <= yCull)
				consideredPoint = ForestGeneration.randomXZAroundPoint(originPoint.transform.position, range);
				
			Quaternion randomRotation = ForestGeneration.randomYRotation(Quaternion.identity);
			
			var spawnedObject = (GameObject)Instantiate((Object)propPrefabs[Random.Range (0, propPrefabs.Length)], consideredPoint, randomRotation);
			spawnedObject.tag = "Prop";
		}
	}
	
	public void Update()
	{
//		Debug.DrawRay (originPoint.transform.position, Vector3.forward * range);
//		Debug.DrawRay (originPoint.transform.position, Vector3.forward * -range);
	}
}
