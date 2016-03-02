using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;
using System.Linq;

public abstract class PrimitiveScript : MonoBehaviour
{
	public Instance instance;
	public NetworkServer network;
	
	public void findNetworkServer()
	{
		//find network server
		network = GameObject.Find ("NetworkManager").GetComponent<NetworkServer>();
	}
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
		network.createCharacter(characterInstance.primitive.prefabPath, position, rotation); 
		var character = network.lastCharacter;
		
		if(!network.isMultiplayer)
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
			//Attach the locomotion script to the character.
			var script = (ILocomotionScript)character.AddComponent(Type.GetType(characterInstance.primitive.locomotionScriptPath));
			
			//Pass in instance settings for locomotion (to get values)
			script.instance = characterInstance;
			
			//Finally, add the controller script
			character.AddComponent(Type.GetType (controllerScript));
			
			//Hook callback
			script.onUpdate += network.onNPCUpdate;
			
			//find the amount of already attached human controllers
			//if that number of human controllers is less than no. players
			//then add the human controller via RPC
			
			/*var numberOfHumanControllers = network.characters.Count (x => x.Value.GetComponent<PlayerController> () != null);
			
			DebugLogger.Log ("the number of human controllers is " + numberOfHumanControllers);
			
			if (network.players.Count <= numberOfHumanControllers)
			{
				//we need to attach the controller for this player.
				network.networkView.RPC("setHumanControlledCharacter", network.players[numberOfHumanControllers-1], null);
			}*/
		}
		
		Debug.Log ("character added");
		
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

		if (character.tag == "Player") {
			// for human models.
			temp.y = -0.02072453f;
		} else {
			temp.y = Terrain.activeTerrain.SampleHeight (temp);
			//temp.y += character.GetComponent<MeshFilter>().mesh.bounds.extents.y * character.transform.localScale.y;
			temp.y += character.GetComponent<Collider> ().bounds.extents.y * character.transform.localScale.y;
		
			character.transform.position = temp;
		}
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