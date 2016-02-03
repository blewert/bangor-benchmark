using UnityEngine;
using System.Collections;
using System.Linq;
using System.Collections.Generic;

public class NetworkServer : MonoBehaviour
{
	[Header("Multiplayer settings")]
	public static bool isMultiplayer = false;
	public static int NUMBER_OF_CONNECTIONS = 8;
	
	[Header("Connection settings")]
	public static bool startTypeClient = false;
	
	//Shared network view for this player/server/whatever
	private static NetworkView networkView;
	
	//List of objects
	public static List<GameObject> objects = new List<GameObject>();
	public static List<NetworkPlayer> players = new List<NetworkPlayer>();
	
	public static void connect(string ip, int port, string password)
	{
		//A server cannot connect to stuff..
		if(!startTypeClient)
			return;
			
		//Get the attached network view for RPCs
		networkView = new NetworkView();
		
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
	public static void start(int port, string serverPassword) 
	{
		//Clients can't start the server..
		if(startTypeClient)
			return;
			
		//Set up server. First set the incoming password.
		Network.incomingPassword = serverPassword;
		
		//And finally initialize the server
		Network.InitializeServer(NUMBER_OF_CONNECTIONS, port);
		
		//Get network view for script
		networkView = new NetworkView();
	} 
	
	
	public static GameObject createObject(Object prefab, Vector3 position, Quaternion rotation)
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
