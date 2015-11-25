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
	
	public Vector3 getOriginPoint()
	{
		return SettingParser.getTerrainOriginPoint(Terrain.activeTerrain);
	}
}