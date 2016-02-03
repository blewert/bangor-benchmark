using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;
using System.Linq;

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
		
		temp.x += UnityEngine.Random.Range (-radius, radius);
		temp.z += UnityEngine.Random.Range (-radius, radius);
		temp.y = Terrain.activeTerrain.SampleHeight(temp);
		
		return temp;
	}
	
	public static Quaternion randomYRotation()
	{
		return Quaternion.Euler(new Vector3(0, UnityEngine.Random.Range (0, 360), 0));
	}
	
	public GameObject instantiateCharacter(Vector3 position, Quaternion rotation)
	{
		return instantiateCharacter(position, rotation, characterInstance.controllerScript);
	}
	
	public GameObject instantiateCharacter(Vector3 position, Quaternion rotation, string controllerScript)
	{
		//Instantiate the character at the position and rotation
		var character = NetworkServer.createCharacter(Resources.Load (characterInstance.primitive.prefabPath), position, rotation); 
		//(GameObject)Instantiate (Resources.Load (characterInstance.primitive.prefabPath), position, rotation);
		
		if(!NetworkServer.isMultiplayer)
		{
			//Attach the locomotion script to the character.
			var script = (ILocomotionScript)character.AddComponent(Type.GetType(characterInstance.primitive.locomotionScriptPath));
			
			//Pass in instance settings for locomotion (to get values)
			script.instance = characterInstance;
			
			//Finally, add the controller script
			character.AddComponent(Type.GetType (controllerScript));
		}
		else
		{
			if(Network.isClient)
			{
				//Attach the locomotion script to the character.
				var script = (ILocomotionScript)character.AddComponent(Type.GetType(characterInstance.primitive.locomotionScriptPath));
				
				//Pass in instance settings for locomotion (to get values)
				script.instance = characterInstance;
				
				//Finally, add the controller script
				character.AddComponent(Type.GetType (controllerScript));
				
				//Hook callback for movement so we can send an RPC to everyone else.
			}	
		}
				
		//Set up team and assign an id
		character.setTeam("Default");
		character.assignID();
		
		character.setData ("NPC");
		
		//Return the character
		return character;
	}
	
	public GameObject instantiateCharacterOnTerrain(Vector3 position, Quaternion rotation)
	{		
		return instantiateCharacterOnTerrain(position, rotation, characterInstance.controllerScript);
	}	
	
	public GameObject instantiateCharacterOnTerrain(Vector3 position, Quaternion rotation, string controllerScript)
	{		
		var character = instantiateCharacter(position, rotation, controllerScript);
		
		Vector3 temp = position;
		temp.y = Terrain.activeTerrain.SampleHeight(temp);
		temp.y += character.GetComponent<MeshFilter>().mesh.bounds.extents.y * character.transform.localScale.y;
		
		character.transform.position = temp;
		
		return character;
	}	
	
	public List<GameObject> spawnTeam(Func<Vector3> positionMethod, int amount, string controllerScript, string team)
	{
		List<GameObject> returnObjects = new List<GameObject>();
		
		for(int i = 0; i < amount; i++)
		{
			Vector3 position = positionMethod.Invoke();
			
			var character = instantiateCharacterOnTerrain(position, randomYRotation(), controllerScript);
			
			character.setTeam (team);
			
			returnObjects.Add (character);
		}
		
		return returnObjects;
	}
}

