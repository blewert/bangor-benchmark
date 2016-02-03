using UnityEngine;
using System.Collections;
using System.Linq;
using System.Collections.Generic;

public class NetworkServer : MonoBehaviour
{
	[Header("Multiplayer settings")]
	public static bool isMultiplayer = false;
	public static int NUMBER_OF_CONNECTIONS = 8;
	
	private static NetworkView networkView;
	
	public static List<GameObject> objects = new List<GameObject>();
	 
	// Use this for initialization
	public static void start(int port, string serverPassword) 
	{
		//Set up server. First set the incoming password.
		Network.incomingPassword = serverPassword;
		
		//And finally initialize the server
		Network.InitializeServer(NUMBER_OF_CONNECTIONS, port);
		
		networkView = new NetworkView();
				
		DebugLogger.Log ("Initialized server? " + Network.isServer + " .. with password \"" + serverPassword + "\"");
	} 
	
	
	public static GameObject createObject(Object prefab, Vector3 position, Quaternion rotation)
	{
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
