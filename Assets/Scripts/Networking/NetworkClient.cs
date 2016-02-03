using UnityEngine;
using System.Collections;

public class NetworkClient : MonoBehaviour 
{
	[Header("Network connection settings")]
	public string connectionAddress;
	public int connectionPort;
	public string connectionPassword;

	// Use this for initialization
	void Start () 
	{
		Network.Connect(connectionAddress, connectionPort, connectionPassword);
		
		Debug.Log ("[client] Connecting to " + connectionAddress + ":" + connectionPort + " with password " + connectionPassword);
		Debug.Log ("[client] Initialized client? " + Network.isClient);	
	}
	
	// Update is called once per frame
	void OnConnectedToServer() 
	{
		Debug.Log ("[client] Connected!");
	}
}
