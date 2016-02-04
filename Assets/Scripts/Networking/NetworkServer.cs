using UnityEngine;
using System.Collections;
using System.Linq;
using System.Collections.Generic;

public class NetworkServer : MonoBehaviour
{
	[Header("Multiplayer settings")]
	public bool isMultiplayer = false;
	public static int NUMBER_OF_CONNECTIONS = 8;
	
	[Header("Connection settings")]
	public bool startTypeClient = false;
	
	//Shared network view for this player/server/whatever
	public NetworkView networkView;
	
	//List of objects
	public List<GameObject> objects = new List<GameObject>();
	public List<NetworkPlayer> players = new List<NetworkPlayer>();
	public Dictionary<int, GameObject> characters = new Dictionary<int, GameObject>();
	
	public static CharacterInstance passedInstance;
	
	public void Start()
	{
		networkView = GetComponent<NetworkView>();
		Debug.Log ("Network view created: " + networkView);	
	}
	
	public void connect(string ip, int port, string password)
	{
		//A server cannot connect to stuff..
		if(!startTypeClient)
			return;
			
		//Attempt to connect to the server
		Network.Connect(ip, port, password);
	}
	
	public void OnConnectedToServer()
	{
		Debug.Log ("Connected to server.");
	}
	
	void OnPlayerConnected(NetworkPlayer player) 
	{
		if(!isMultiplayer)
			return;
		
		players.Add(player);
	}
	
	void OnPlayerDisconnected(NetworkPlayer player) 
	{
		if(!isMultiplayer)
			return;
		
		players.Remove(player);
	}
	
	// Use this for initialization
	public void start(int port, string serverPassword) 
	{
		//Clients can't start the server..
		if(startTypeClient)
			return;
			
		//Set up server. First set the incoming password.
		Network.incomingPassword = serverPassword;
		
		//And finally initialize the server
		Network.InitializeServer(NUMBER_OF_CONNECTIONS, port);
		
		Debug.Log ("Initialized the server... " + Network.isServer);
	} 
	
	public void onNPCUpdate(int id, Vector3 newPosition, Quaternion newRotation)
	{
		networkView.RPC ("onClientNPCUpdate", RPCMode.Others, id, newPosition, newRotation);
	}
	
	[RPC]
	public void onClientNPCUpdate(int id, Vector3 newPosition, Quaternion newRotation)
	{
		var foundCharacter = characters[id];
		
		Debug.Log ("char null: " + characters == null);
		
		foundCharacter.transform.position = newPosition;
		foundCharacter.transform.rotation = newRotation;
	}
	
	[RPC]
	public void testRPC(string message)
	{
		Debug.Log ("RPC ... " + message);
	}
	
	[RPC]
	public void addAIScriptsToCharacter(int characterId, string locomotionPath, string controllerScript)
	{
		var foundCharacter = characters[characterId];
		
		//Attach scripts
		var script = (ILocomotionScript)foundCharacter.AddComponent(System.Type.GetType(locomotionPath));
			
		script.instance = passedInstance;
		
		//Finally, add the controller script
		foundCharacter.AddComponent(System.Type.GetType (controllerScript));
	}
	
	public GameObject createCharacter(Object prefab, Vector3 position, Quaternion rotation)
	{
		//Creates an object locally or on the network.
		//..
		
		GameObject addedObject = null;
		
		if(isMultiplayer)
			addedObject = Network.Instantiate(prefab, position, rotation, 0) as GameObject;
		else
			addedObject = (GameObject)Instantiate(prefab, position, rotation);

		addedObject.setTeam("Default");
		addedObject.assignID();
		addedObject.setData ("NPC");	

		return addedObject;
	}

	public GameObject createObject(Object prefab, Vector3 position, Quaternion rotation)
	{
		//Creates an object locally or on the network.
		//..
		
		GameObject addedObject = null;
		
		if(isMultiplayer)
			addedObject = Network.Instantiate(prefab, position, rotation, 0) as GameObject;
		else
			addedObject = (GameObject)Instantiate(prefab, position, rotation);
		
		objects.Add (addedObject);
		
		return addedObject;	
	}
	
	public void OnApplicationQuit()
	{
		Network.Disconnect();
		MasterServer.UnregisterHost();
	}
}
