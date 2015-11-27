using UnityEngine;
using System.Collections;

public abstract class PrimitiveScript : MonoBehaviour
{
	public Instance instance;
}

public abstract class GamemodeScript : PrimitiveScript
{
	public CharacterInstance characterInstance;
	public EnvironmentInstance environmentInstance;
	
	public static Vector3 getOriginPoint()
	{
		return SettingParser.getTerrainOriginPoint(Terrain.activeTerrain);
	}
	
	public static Vector3 randomXZAroundPoint(Vector3 originPoint, float radius)
	{
		Vector3 temp = originPoint;
		
		temp.x += Random.Range (-radius, radius);
		temp.z += Random.Range (-radius, radius);
		temp.y = Terrain.activeTerrain.SampleHeight(temp);
		
		return temp;
	}
}