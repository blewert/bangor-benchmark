using UnityEngine;
using System.Collections;
using System.Linq;

public class PropSpawner : MonoBehaviour 
{
	public int spawnAmount = 3;
	public GameObject[] propPrefabs;
	public GameObject originPoint;
	public float range;
	public float yCull;
	
	public void Start()
	{
		var flags = GetComponent<FlagSpawner>().flags;
		
		for(int i = 0; i < spawnAmount; i++)
		{
			Vector3 consideredPoint = ForestGeneration.randomXZAroundPoint(originPoint.transform.position, range);
			
			int t = 0;
			
			while(consideredPoint.y <= yCull)
			{
				var tempPoint = ForestGeneration.randomXZAroundPoint(originPoint.transform.position, range);
			
				if(++t >= 1000)
				{
					consideredPoint = tempPoint;
					break;
				}
				
				if(flags.Any (x => Vector3.Distance (x.transform.position, tempPoint) <= 25f))
					continue;
					
				consideredPoint = tempPoint;
			}
				
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
