using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

/// <summary>
/// Extension methods provided for usage with the bangor-benchmark project.
/// </summary>
public static class ExtensionMethods
{
	//The current id to store for newly assigned ids of gameobjects
	private static uint currentId = default(uint);
	
	//The index of IDs for each gameobject that has been assigned (map obj -> uint)
	private static Dictionary<GameObject, uint> idIndex = new Dictionary<GameObject, uint>();
	
	//The index of teams (tags) for each gameobject that been assigned (map obj -> string)	
	private static Dictionary<GameObject, string> teamIndex = new Dictionary<GameObject, string>();
	
	private static Dictionary<GameObject, string> dataIndex = new Dictionary<GameObject, string>();
	
	/// <summary>
	/// Assigns an unique id to the GameObject, based on previous assignments.
	/// </summary>
	public static void assignID(this GameObject obj)
	{
		idIndex[obj] = currentId++;
	}
	
	/// <summary>
	/// Gets the unique id of this gameobject.
	/// </summary>
	public static uint getID(this GameObject obj)
	{
		return idIndex[obj];
	}	
	
	public static string getData(this GameObject obj)
	{
		//Return the value in the dictionary where the key matches the object, or return "unspecified"
		return dataIndex.Where(x => x.Key == obj).Select (x => x.Value).FirstOrDefault() ?? "None";
	}
	
	public static void setData(this GameObject obj, string data)
	{
		dataIndex[obj] = data;
	}
	
	/// <summary>
	/// Assigns a team label to the current object.
	/// </summary>
	/// <param name="team">The team name to assign</param>
	public static void setTeam(this GameObject obj, string team)
	{
		teamIndex[obj] = team;
	}
	
	/// <summary>
	/// Gets the team label for the current object.
	/// </summary>
	public static string getTeam(this GameObject obj)
	{
		//Return the value in the dictionary where the key matches the object, or return "unspecified"
		return teamIndex.Where(x => x.Key == obj).Select (x => x.Value).FirstOrDefault() ?? "None";
	}
	
	/// <summary>
	/// Selects a random element from the list.
	/// </summary>
	/// <returns>A random element.</returns>
	public static T randomElement<T>(this List<T> list)
	{
		return list[UnityEngine.Random.Range (0, list.Count)];
	}
}

