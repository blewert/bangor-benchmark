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
	
	//Timing
	private float nextNetworkUpdateTime = 0.0f;
	public float networkUpdateIntervalMax = 0.05f;
		
	public GameObject lastCharacter;
	
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
		if(Time.realtimeSinceStartup > nextNetworkUpdateTime)
		{
			nextNetworkUpdateTime = Time.realtimeSinceStartup + networkUpdateIntervalMax;
			networkView.RPC ("onClientNPCUpdate", RPCMode.Others, id, newPosition, newRotation);
		}
	}

	[RPC]
	public void onClientNPCUpdate(int id, Vector3 newPosition, Quaternion newRotation)
	{
		var foundCharacter = characters[id];
		
		foundCharacter.transform.position = newPosition;
		foundCharacter.transform.rotation = newRotation;
	}

	[RPC]
	public void testRPC(string message)
	{
		Debug.Log ("RPC ... " + message);
	}
	
	[RPC]
	public void createSyncedCharacter(string prefabPath, Vector3 position, Quaternion rotation)
	{
		GameObject addedObject = null;
		
		addedObject = (GameObject)Instantiate(Resources.Load (prefabPath), position, rotation);
		
		addedObject.setTeam("Default");
		addedObject.assignID();
		addedObject.setData ("NPC");	

		characters.Add((int)addedObject.getID(), addedObject);

		lastCharacter = addedObject;
	}

	// Make the client execute this method.
	[RPC]
	private void setHumanControlledCharacter(int npcID)
	{
		Debug.Log ("I need to attach a human controller to " + npcID);

		//Find the player with the given npc id
		var thePlayer = characters [npcID];

		thePlayer.AddComponent<HumansController>();
		
		thePlayer.GetComponent<HumansController>().onUpdate += onNPCUpdate;
		
		// apply playerContoller script to that character.
		thePlayer.AddComponent<PlayerController>();

		// uncheck or remove humanAIcontroller
		//thePlayer.GetComponent<HumanEnemyAI> ().enabled = false;

		//Get rid of the main camera for now
		Camera.main.enabled = false;
		thePlayer.transform.Find("Main Camera").gameObject.SetActive(true);
	
		
	}

	[RPC]
	public void takeHealthAndUpdate(int whoHasBeenHit){
		characters[whoHasBeenHit].GetComponent<ILocomotionScript> ().takeHealth (10.0f);
	}

	[RPC]
	public void updateSpeed(int agentToUpdateIdx, float speed){
		characters[agentToUpdateIdx].GetComponent<Animator> ().SetFloat("Speed", speed);
	}

	[RPC]
	public void updateBool(int agentToUpdateIdx, string param, bool value){
		characters[agentToUpdateIdx].GetComponent<Animator> ().SetBool(param, value);
	}

	[RPC]
	public void setTheTrigger(int agentToUpdateIdx, string param){
		characters[agentToUpdateIdx].GetComponent<Animator> ().SetTrigger(param);
	}

	[RPC]
	public void networkWideShowOrHideAgent(int agentToUpdateIdx, bool param){
		characters [agentToUpdateIdx].gameObject.SetActive (param);
	}

	public void createCharacter(string prefabPath, Vector3 position, Quaternion rotation)
	{
		 //Creates an object locally or on the network.
		//..
	
		if(isMultiplayer)	
		{
			networkView.RPC ("createSyncedCharacter", RPCMode.All, prefabPath, position, rotation);
			return;
		}
		
		GameObject addedObject = null;
		
		addedObject = (GameObject)Instantiate(Resources.Load (prefabPath), position, rotation);

		addedObject.setTeam("Default");
		addedObject.assignID();
		addedObject.setData ("NPC");	
		
		characters.Add((int)addedObject.getID(), addedObject);

		lastCharacter = addedObject;
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
